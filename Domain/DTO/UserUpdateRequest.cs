namespace Domain.DTO;

public class UserUpdateRequest
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Cpf { get; set; }
    public string CompanyRef { get; set; }
    public string Password { get; set; }
}
