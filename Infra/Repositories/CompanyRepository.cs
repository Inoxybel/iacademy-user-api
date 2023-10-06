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
            var newCompany = new Company()
            {
                Id = Guid.NewGuid().ToString(),
                Name = request.Name,
                Cnpj = request.Cnpj,
                Groups = request.Groups
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
}
