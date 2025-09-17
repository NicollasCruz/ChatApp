using System;
using System.Collections.Concurrent;
using API.Data;
using API.DTOs;
using API.Extentions;
using API.Models;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace API.Hubs;

[Authorize]
public class ChatHub(UserManager<AppUser> userManager, AppDbContext context) : Hub
{
    public static readonly ConcurrentDictionary<string, OnlineUserDTO> onlineUsers = new();

    public async Task LoadMessages(string receiverId, int pageNumber = 1)
    {
        int pageSize = 10;
        var userName = Context.User!.Identity!.Name;
        var currentUser = await userManager.FindByNameAsync(userName!);

        if (currentUser == null) return;

        List<MessageResponseDTO> messages = await context.Messages
            .Where(m => (m.SenderId == currentUser.Id && m.ReceiverId == receiverId) ||
                        (m.SenderId == receiverId && m.ReceiverId == currentUser.Id))
            .OrderByDescending(m => m.CreatedDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(m => new MessageResponseDTO
            {
                Id = m.Id,
                Content = m.Content,
                CreatedDate = m.CreatedDate,
                SenderId = m.SenderId,
                ReceiverId = m.ReceiverId
            })
            .OrderBy(m => m.CreatedDate)
            .ToListAsync();

        foreach (var message in messages)
        {
            var msg = await context.Messages.FirstOrDefaultAsync(o => o.Id == message.Id);

            if (msg != null && msg.ReceiverId == currentUser.Id)
            {
                msg.IsRead = true;
                await context.SaveChangesAsync();
            }
        }

        await Clients.User(currentUser.Id).SendAsync("MessagesLoaded", messages);
    }
    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var receiverId = httpContext?.Request.Query["receiverId"].ToString();
        var userName = Context.User?.Identity?.Name;
        var currentUser = await userManager.FindByNameAsync(userName!);
        var connectionId = Context.ConnectionId;

        if (onlineUsers.ContainsKey(userName))
        {
            onlineUsers[userName].ConnectionId = connectionId;
        }
        else
        {
            var user = new OnlineUserDTO
            {
                ConnectionId = connectionId,
                UserName = userName ?? string.Empty,
                FullName = currentUser?.FullName ?? string.Empty,
                ProfilePicture = currentUser?.ProfileImage ?? string.Empty,
            };

            onlineUsers.TryAdd(userName, user);

            await Clients.AllExcept(connectionId).SendAsync("Notify", currentUser);
        }

        if (!string.IsNullOrEmpty(receiverId))
        {
            await LoadMessages(receiverId);
        }

        await Clients.All.SendAsync("OnlineUsers", await GetAllUsers());

    }

    public async Task SendMessage(MessageRequestDTO message)
    {
        var senderId = Context.User!.Identity.Name;
        var recipientId = message.ReceiverId;

        var newMessage = new Message
        {
            Content = message.Content ?? string.Empty,
            Sender = await userManager.FindByNameAsync(senderId),
            Receiver = await userManager.FindByIdAsync(recipientId),
            IsRead = false,
            CreatedDate = DateTime.UtcNow
        };

        context.Messages.Add(newMessage);
        await context.SaveChangesAsync();

        await Clients.User(recipientId).SendAsync("NewMessageReceived", newMessage);
    }

    public async Task NotifyTyping(string receiverId)
    {
        var senderName = Context.User!.Identity!.Name;
        var sender = await userManager.FindByNameAsync(senderName!);

        if (sender == null) return;

        var connectionId = onlineUsers.Values.FirstOrDefault(o => o.UserName == receiverId)?.ConnectionId;

        if(connectionId != null)
        {
            await Clients.Client(connectionId).SendAsync("UserTyping", senderName);
        }

    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userName = Context.User?.Identity?.Name;
        onlineUsers.TryRemove(userName!, out _);
        await Clients.All.SendAsync("OnlineUsers", await GetAllUsers());
    }
    private async Task<IEnumerable<OnlineUserDTO>> GetAllUsers()
    {
        var username = Context.User!.GetUserName();

        var onlineUsersSet = new HashSet<string>(onlineUsers.Keys);

        var users = await userManager.Users.Select(o => new OnlineUserDTO
        {
            Id = o.Id,
            UserName = o.UserName!,
            FullName = o.FullName,
            ProfilePicture = o.ProfileImage,
            IsOnline = onlineUsersSet.Contains(o.UserName!),
            UnReadCount = context.Messages.Count(m => m.ReceiverId == username && m.SenderId == o.Id && !m.IsRead),
        })
        .OrderByDescending(u => u.IsOnline)
        .ToListAsync();

        return users;
    }
}
