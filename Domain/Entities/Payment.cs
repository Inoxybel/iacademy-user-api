using CrossCutting.Enums;

namespace Domain.Entities;

public class Payment
{
    public string Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public PaymentStatus Status { get; set; }
    public Plan Plan { get; set; }
    public string Annotations { get; set; }
}
