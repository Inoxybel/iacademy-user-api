using Domain.Entities;
using Domain.Interfaces.Infra;
using MongoDB.Driver;

namespace Infra.Repositories;

public class ActivationCodeRepository : IActivationCodeRepository
{
    private readonly DbContext _dbContext;

    public ActivationCodeRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ActivationCode> GetByUserId(string userId, CancellationToken cancellationToken = default) =>
        await (await _dbContext.ActivationCode.FindAsync(u => u.UserId == userId, cancellationToken: cancellationToken))
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<string> Save(ActivationCode activationCode, CancellationToken cancellationToken = default)
    {
        try
        {
            activationCode.CreatedDate = DateTime.UtcNow;

            await _dbContext.ActivationCode.InsertOneAsync(activationCode, null, cancellationToken);

            return activationCode.Id;
        }
        catch
        {
            return string.Empty;
        }
    }

    public async Task<bool> Update(string activationCodeId, CancellationToken cancellationToken = default)
    {
        try
        { 
            var filterDefinition = Builders<ActivationCode>.Filter.Eq(u => u.Id, activationCodeId);

            var filterUpdate = Builders<ActivationCode>.Update
                .Set(a => a.Used, true)
                .Set(a => a.UpdatedDate, DateTime.UtcNow);

            var result = await _dbContext.ActivationCode.UpdateOneAsync(filterDefinition, filterUpdate, null, cancellationToken);

            if (result.ModifiedCount > 0)
                return true;

            return false;
        }
        catch
        {
            return false;
        }
    }
}
