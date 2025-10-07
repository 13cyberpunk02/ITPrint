using ITPrint.Core.Enums;

namespace ITPrint.Core.DTOs.Users;

public class UserListDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public int TotalPrintJobs { get; set; }
}