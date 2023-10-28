using CrossCutting.Helpers;
using Domain.DTO;
using Domain.Entities;
using Domain.Interfaces.Infra;
using MongoDB.Driver;

namespace Infra.Repositories;

public class PlanRepository : IPlanRepository
{
    private readonly DbContext _dbContext;

    public PlanRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Plan> GetById(string planId, CancellationToken cancellationToken = default) =>
        await (await _dbContext.Plan.FindAsync(u => u.Id == planId, cancellationToken: cancellationToken))
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<string> Save(CreatePlanRequest createPlanDTO, CancellationToken cancellationToken = default)
    {
        try
        {
            var newPlan = new Plan()
            {
                Id = Guid.NewGuid().ToString(),
                Name = createPlanDTO.Name,
                Type = createPlanDTO.Type,
                AccessLimit = createPlanDTO.AccessLimit,
                Value = createPlanDTO.Value
            };

            await _dbContext.Plan.InsertOneAsync(newPlan, null, cancellationToken);

            return newPlan.Id;
        }
        catch
        {
            return string.Empty;
        }
    }

    public async Task<ServiceResult<bool>> Update(UpdatePlanRequest updatePlanRequest, CancellationToken cancellationToken = default)
    {
        try
        {
            var filterDefinition = Builders<Plan>.Filter.Eq(u => u.Id, updatePlanRequest.Id);

            var updateDefinitions = new List<UpdateDefinition<Plan>>();

            if (updatePlanRequest.Name != null)
                updateDefinitions.Add(Builders<Plan>.Update.Set(u => u.Name, updatePlanRequest.Name));

            if (updatePlanRequest?.Type != null)
                updateDefinitions.Add(Builders<Plan>.Update.Set(u => u.Type, updatePlanRequest.Type));

            if (updatePlanRequest?.Value != null)
                updateDefinitions.Add(Builders<Plan>.Update.Set(u => u.Value, updatePlanRequest.Value));

            if (updatePlanRequest?.AccessLimit != null)
                updateDefinitions.Add(Builders<Plan>.Update.Set(u => u.AccessLimit, updatePlanRequest.AccessLimit));

            if (!updateDefinitions.Any())
                return ServiceResult<bool>.MakeErrorResult("No fields to update.");

            var combinedUpdate = Builders<Plan>.Update.Combine(updateDefinitions);

            var result = await _dbContext.Plan.UpdateOneAsync(filterDefinition, combinedUpdate, null, cancellationToken);

            if (result.ModifiedCount > 0)
                return ServiceResult<bool>.MakeSuccessResult(true);

            return ServiceResult<bool>.MakeErrorResult("Error: plan not updated.");
        }
        catch
        {
            return ServiceResult<bool>.MakeErrorResult("Error: plan not updated.");
        }
    }
}
