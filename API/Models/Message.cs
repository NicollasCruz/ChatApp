using System;

namespace API.Models;

public class Message
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public string SenderId { get; set; } = string.Empty;
    public AppUser Sender { get; set; } = null!;
    public string ReceiverId { get; set; } = string.Empty;
    public AppUser Receiver { get; set; } = null!;
    public bool IsRead { get; set; } = false;
}
