using Domain.Entities;

namespace Domain.DTO;

public class CompanyRequest
{
    public string Name { get; set; }
    public string Cnpj { get; set; }
    public List<CompanyGroup> Groups { get; set; }
}
