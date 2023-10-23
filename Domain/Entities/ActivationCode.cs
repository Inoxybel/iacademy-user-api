namespace Domain.Entities;

public class ActivationCode
{
    public string Id { get; set; }
    public string Code { get; set; }
    public string UserId { get; set; }
    public bool Used { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
}
