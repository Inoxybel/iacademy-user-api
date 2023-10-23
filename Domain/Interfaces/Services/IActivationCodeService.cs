using CrossCutting.Helpers;
using Domain.Entities;

namespace Domain.Interfaces.Services;

public interface IActivationCodeService
{
    Task<ActivationCode> Get(string userId, CancellationToken cancellationToken = default);
    Task<string> Save(ActivationCode activationCode, CancellationToken cancellationToken = default);
    Task<bool> Update(string activationCodeId, CancellationToken cancellationToken = default);
}
