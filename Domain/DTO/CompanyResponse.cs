using Domain.Entities;

namespace Domain.DTO;

public class CompanyResponse
{
    public string Id { get; set; }
    public string Cnpj { get; set; }
    public string Name { get; set; }
    public int LimitPlan { get; set; }
    public List<CompanyGroup> Groups { get; set; }
}
