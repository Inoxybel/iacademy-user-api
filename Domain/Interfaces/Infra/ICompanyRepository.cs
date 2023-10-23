using CrossCutting.Helpers;
using Domain.DTO;
using Domain.Entities;

namespace Domain.Interfaces.Infra;

public interface ICompanyRepository
{
    Task<Company> GetById(string companyId, CancellationToken cancellationToken = default);
    Task<Company> GetByRef(string Cnpj, CancellationToken cancellationToken = default);
    Task<string> Save(CompanyRequest request, CancellationToken cancellationToken = default);
    Task<bool> Update(string companyId, CompanyRequest request, CancellationToken cancellationToken = default);
    Task<ServiceResult<CompanyResponse>> ValidatePassword(CompanyLoginRequest sendedInfo, CancellationToken cancellationToken = default);
    Task<ServiceResult<bool>> UpdatePassword(string companyId, CompanyUpdatePasswordRequest sendedInfo, CancellationToken cancellationToken = default);
}
