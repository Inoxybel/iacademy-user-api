using Domain.Entities;

namespace Domain.DTO;

public class CompanyRequest
{
    public string Name { get; set; }
    public string Cnpj { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    public List<CompanyGroup> Groups { get; set; }
}
