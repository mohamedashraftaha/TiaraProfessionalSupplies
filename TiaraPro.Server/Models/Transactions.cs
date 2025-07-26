using System.ComponentModel.DataAnnotations;
namespace TiaraPro.Server.Models;

public class Transactions
{
    [Key]
    public int Id { get; set; }
    public Guid TransactionGuid { get; set; } = Guid.NewGuid();
    public string S3Url { get; set; } = string.Empty;

    public DateTime DateCreated { get; set; } = DateTime.UtcNow;

    public int UserId { get; set; }
    public User? User { get; set; }
    public string Status { get; set; } = "running";
    public string? DentalMeshResponseStlFolder { get; set; } = null;
    public string? DentalMeshResponseStlViewUrl { get; set; } = null;
}
