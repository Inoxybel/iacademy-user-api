namespace Domain.Entities;

public class CompanyGroup
{
    public string GroupName { get; set; }
    public List<string> UsersDocument { get; set; }
    public int LimitPlan { get; set; }
    public List<string> AuthorizedTrainingIds { get; set; }
}
