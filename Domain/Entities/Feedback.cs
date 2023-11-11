namespace Domain.Entities;

public class Feedback
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Comment { get; set; }
    public DateTime CreatedDate { get; set; }
}
