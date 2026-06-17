using System.ComponentModel.DataAnnotations;

namespace CodingChallenge.Api.Models;

public class UserCreateDto
{
    [Required]
    [MaxLength(255)]
    public string DisplayName { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public DateTime DateOfBirth { get; set; }
}

public class UserUpdateDto
{
    [MaxLength(255)]
    public string? DisplayName { get; set; }

    [MaxLength(255)]
    public string? FirstName { get; set; }

    [MaxLength(255)]
    public string? LastName { get; set; }

    [EmailAddress]
    [MaxLength(255)]
    public string? Email { get; set; }

    public DateTime? DateOfBirth { get; set; }
}

public class UserResponseDto
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
}
