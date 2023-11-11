using Domain.Interfaces.Infra;
using Domain.Interfaces.Services;
using ActivationCodeEntity = Domain.Entities.ActivationCode;

namespace Services;

public class ActivationCodeService : IActivationCodeService
{
    private readonly IActivationCodeRepository _activationRepository;

    public ActivationCodeService(
        IActivationCodeRepository activationCodeRepository)
    {
        _activationRepository = activationCodeRepository;
    }

    public async Task<ActivationCodeEntity> Get(string userId, CancellationToken cancellationToken = default)
    {
        var result = await _activationRepository.GetByUserId(userId, cancellationToken);

        if (result is null)
        {
            return new();
        }

        return result;
    }

    public async Task<string> Save(ActivationCodeEntity activationCode, CancellationToken cancellationToken = default)
    {
        return await _activationRepository.Save(activationCode, cancellationToken);
    }

    public async Task<bool> Update(string activationCodeId, CancellationToken cancellationToken = default)
    {
        return await _activationRepository.Update(activationCodeId, cancellationToken);
    }
}
