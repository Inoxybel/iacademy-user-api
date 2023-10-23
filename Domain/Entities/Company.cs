namespace Domain.Entities;

public class Company
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Cnpj {  get; set; }
    public List<CompanyGroup> Groups { get; set; }
    public int LimitPlan { get; set; }
    public string PasswordHash { get; set; }

    public bool ContainsUserDocument(string userDocument) =>
        Groups.Any(group => group.Users.Any(u => u.Document == userDocument));
}
