using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HardWorkAPI.Models;
public class ProgressPhoto
{
    [Key]
    public long Id { get; set; }

    [ForeignKey("Student")]
    public long StudentId { get; set; }
    public User Student { get; set; }

    [Required]
    public string ImageUrl { get; set; } = string.Empty;

    public string? Note { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

