namespace Domain.Entities;

public class CompanyGroup
{
    public string GroupName { get; set; }
    public List<UserInfo> Users { get; set; }
    public List<string> AuthorizedTrainingIds { get; set; }
}
