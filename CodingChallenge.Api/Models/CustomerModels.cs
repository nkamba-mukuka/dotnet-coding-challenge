using System.ComponentModel.DataAnnotations;

namespace CodingChallenge.Api.Models;

public class CustomerCreateDto
{
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? WebSite { get; set; }
}

public class CustomerUpdateDto
{
    [MaxLength(255)]
    public string? Name { get; set; }

    [MaxLength(255)]
    public string? WebSite { get; set; }
}

public class CustomerResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? WebSite { get; set; }
}
