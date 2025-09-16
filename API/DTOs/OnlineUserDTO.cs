using System;

namespace API.DTOs;

public class OnlineUserDTO
{
    public string Id { get; set; } = string.Empty;
    public string ConnectionId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string ProfilePicture { get; set; } = string.Empty;
    public bool IsOnline { get; set; } = false;
    public int UnReadCount { get; set; } = 0;
}
