using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HardWorkAPI.Models;
public class Payment
{
    [Key]
    public long Id { get; set; }

    [ForeignKey("Student")]
    public long StudentId { get; set; }
    public User Student { get; set; }

    [ForeignKey("Trainer")]
    public long? TrainerId { get; set; }
    public User? Trainer { get; set; }

    [Required]
    public decimal Amount { get; set; }

    [Required]
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    [Required]
    public DateTime DueDate { get; set; }

    public DateTime? PaidAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public enum PaymentStatus
{
    Pending,
    Paid,
    Failed
}
