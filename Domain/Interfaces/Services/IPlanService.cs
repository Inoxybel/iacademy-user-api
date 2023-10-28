using CrossCutting.Helpers;
using Domain.DTO;
using Domain.Entities;

namespace Domain.Interfaces.Services;

public interface IPlanService
{
    Task<Plan> GetById(string planId, CancellationToken cancellationToken = default);
    Task<string> Save(CreatePlanRequest createPlanDTO, CancellationToken cancellationToken = default);
    Task<ServiceResult<bool>> Update(UpdatePlanRequest updatePlanRequest, CancellationToken cancellationToken = default);
}
