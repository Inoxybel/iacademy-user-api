using CrossCutting.Helpers;
using Domain.DTO;
using Domain.Entities;

namespace Domain.Interfaces.Services;

public interface ICompanyService
{
    Task<Company> GetById(string companyId, CancellationToken cancellationToken = default);
    Task<Company> GetByRef(string Cnpj, CancellationToken cancellationToken = default);
    Task<ServiceResult<string>> Save(CompanyRequest request, CancellationToken cancellationToken = default);
    Task<bool> Update(string companyId, CompanyRequest company, CancellationToken cancellationToken = default);
}

