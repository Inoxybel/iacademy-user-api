using CrossCutting.Helpers;
using Domain.DTO;
using Domain.Entities;
using Domain.Interfaces.Infra;
using Domain.Interfaces.Services;

namespace Services;

public class PlanService : IPlanService
{
    private readonly IPlanRepository _planRepository;

    public PlanService(IPlanRepository planRepository)
    {
        _planRepository = planRepository;
    }

    public async Task<Plan> GetById(string planId, CancellationToken cancellationToken = default)
    {
        return await _planRepository.GetById(planId, cancellationToken);
    }

    public async Task<string> Save(CreatePlanRequest createPlanDTO, CancellationToken cancellationToken = default)
    {
        return await _planRepository.Save(createPlanDTO, cancellationToken);
    }

    public async Task<ServiceResult<bool>> Update(UpdatePlanRequest updatePlanRequest, CancellationToken cancellationToken = default)
    {
        return await _planRepository.Update(updatePlanRequest, cancellationToken);
    }
}
