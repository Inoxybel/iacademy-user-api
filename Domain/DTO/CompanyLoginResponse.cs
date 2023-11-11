namespace Domain.DTO;

public class CompanyLoginResponse
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Cnpj { get; set; }
    public int LimitPlan { get; set; }
    public string Token { get; set; }
}
