using CrossCutting.Helpers;
using Domain.Entities;

namespace Domain.Interfaces.Infra;

public interface IActivationCodeRepository
{
    Task<ActivationCode> GetByUserId(string userId, CancellationToken cancellationToken = default);
    Task<string> Save(ActivationCode activationCode, CancellationToken cancellationToken = default);
    Task<bool> Update(string activationCodeId, CancellationToken cancellationToken = default);
}
