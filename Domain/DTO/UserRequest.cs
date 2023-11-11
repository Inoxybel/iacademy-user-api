namespace Domain.DTO;

public class UserRequest
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Cpf { get; set; }
    public string CellphoneNumberWithDDD { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    public string CompanyRef { get; set; }
}
