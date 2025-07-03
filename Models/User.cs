using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class User
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = null!;

    [Required, EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    public string PasswordHash { get; set; } = null!;

    public string Status { get; set; } = "Active"; // or "Blocked"

    public DateTime? LastLoginTime { get; set; }

    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
}