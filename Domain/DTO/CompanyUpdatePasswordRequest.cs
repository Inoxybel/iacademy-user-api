namespace Domain.DTO;

public class CompanyUpdatePasswordRequest
{
    public string Cnpj { get; set; }
    public string OldPassword { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmPassword {  get; set; }
}
