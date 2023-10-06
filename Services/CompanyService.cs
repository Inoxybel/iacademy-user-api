using CrossCutting.Helpers;
using Domain.DTO;
using Domain.Entities;
using Domain.Interfaces.Infra;
using Domain.Interfaces.Services;

namespace Services;

public class CompanyService : ICompanyService
{
    private readonly ICompanyRepository _companyRepository;

    public CompanyService(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task<Company> GetById(string companyId, CancellationToken cancellationToken = default)
    {
        return await _companyRepository.GetById(companyId, cancellationToken);
    }

    public async Task<Company> GetByRef(string Cnpj, CancellationToken cancellationToken = default)
    {
        return await _companyRepository.GetByRef(Cnpj, cancellationToken);
    }

    public async Task<ServiceResult<string>> Save(CompanyRequest request, CancellationToken cancellationToken = default)
    {
        if (DocumentValidator.IsValidCnpj(request.Cnpj))
        {
            var repositoryResult = await _companyRepository.GetByRef(request.Cnpj);

            if (repositoryResult is not null)
                return ServiceResult<string>.MakeErrorResult("Company already exists.");
                
            var saveResponse = await _companyRepository.Save(request, cancellationToken);

            if (string.IsNullOrEmpty(saveResponse))
                return ServiceResult<string>.MakeErrorResult("Error on save process.");

            return ServiceResult<string>.MakeSuccessResult(saveResponse);
        }

        return ServiceResult<string>.MakeErrorResult("Invalid Cnpj.");
    }

    public async Task<bool> Update(string companyId, CompanyRequest request, CancellationToken cancellationToken = default)
    {
        if (DocumentValidator.IsValidCnpj(request.Cnpj))
            return await _companyRepository.Update(companyId, request, cancellationToken);

        return false;
    }
}
