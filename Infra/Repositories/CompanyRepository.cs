using CrossCutting.Helpers;
using Domain.DTO;
using Domain.Entities;
using Domain.Interfaces.Infra;
using MongoDB.Driver;

namespace Infra.Repositories;

public class CompanyRepository : ICompanyRepository
{
    private readonly DbContext _dbContext;

    public CompanyRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Company> GetById(string companyId, CancellationToken cancellationToken = default) =>
        await (await _dbContext.Company.FindAsync(c => c.Id == companyId, cancellationToken: cancellationToken))
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<Company> GetByRef(string Cnpj, CancellationToken cancellationToken = default) =>
        await (await _dbContext.Company.FindAsync(c => c.Cnpj == Cnpj, cancellationToken: cancellationToken))
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<string> Save(CompanyRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var newCompany = new Company()
            {
                Id = Guid.NewGuid().ToString(),
                Name = request.Name,
                Cnpj = request.Cnpj,
                Groups = request.Groups,
                PasswordHash = hashedPassword
            };

            await _dbContext.Company.InsertOneAsync(newCompany, null, cancellationToken);

            return newCompany.Id;
        }
        catch
        {
            return string.Empty;
        }
    }

    public async Task<bool> Update(string companyId, CompanyRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var filterDefinition = Builders<Company>.Filter.Eq(u => u.Id, companyId);

            var filterUpdate = Builders<Company>.Update
                .Set(u => u.Name, request.Name)
                .Set(u => u.Cnpj, request.Cnpj)
                .Set(u => u.Groups, request.Groups);

            var result = await _dbContext.Company.UpdateOneAsync(filterDefinition, filterUpdate, null, cancellationToken);

            return result.ModifiedCount > 0;
        }
        catch
        {
            return false;
        }
    }

    public async Task<ServiceResult<CompanyResponse>> ValidatePassword(CompanyLoginRequest sendedInfo, CancellationToken cancellationToken = default)
    {
        try
        {
            var company = await _dbContext.Company.Find(u => u.Cnpj == sendedInfo.Cnpj)
                .FirstOrDefaultAsync(cancellationToken);

            if (company is not null)
            {
                if (BCrypt.Net.BCrypt.Verify(sendedInfo.Password, company.PasswordHash))
                {
                    var companyResponse = new CompanyResponse()
                    {
                        Id = company.Id,
                        Name = company.Name,
                        Cnpj = company.Cnpj,
                        LimitPlan = company.LimitPlan,
                        Groups = company.Groups
                    };

                    return ServiceResult<CompanyResponse>.MakeSuccessResult(companyResponse);
                }

                return ServiceResult<CompanyResponse>.MakeErrorResult("Invalid Credentials");
            }

            return ServiceResult<CompanyResponse>.MakeErrorResult("Company not found.");
        }
        catch
        {
            return ServiceResult<CompanyResponse>.MakeErrorResult("Error on validation process.");
        }
    }

    public async Task<ServiceResult<bool>> UpdatePassword(
        string companyId,
        CompanyUpdatePasswordRequest sendedInfo,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var company = await _dbContext.Company.Find(u => u.Id == companyId)
                .FirstOrDefaultAsync(cancellationToken);

            if (company != null && BCrypt.Net.BCrypt.Verify(sendedInfo.OldPassword, company.PasswordHash))
            {
                if (sendedInfo.NewPassword == sendedInfo.ConfirmPassword)
                {
                    var newHashedPassword = BCrypt.Net.BCrypt.HashPassword(sendedInfo.NewPassword);

                    var filterDefinition = Builders<Company>.Filter.Eq(u => u.Id, companyId);

                    var filterUpdate = Builders<Company>.Update
                        .Set(u => u.PasswordHash, newHashedPassword);

                    var result = await _dbContext.Company
                        .UpdateOneAsync(filterDefinition, filterUpdate, null, cancellationToken);

                    if (result.ModifiedCount > 0)
                        return ServiceResult<bool>.MakeSuccessResult(true);

                    return ServiceResult<bool>.MakeErrorResult("Error: password not updated.");
                }
            }

            return ServiceResult<bool>.MakeErrorResult("Review informed credentials."); ;
        }
        catch
        {
            return ServiceResult<bool>.MakeErrorResult("Error on password update process."); ;
        }
    }
}
