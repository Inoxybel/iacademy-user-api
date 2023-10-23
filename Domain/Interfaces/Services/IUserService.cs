using CrossCutting.Helpers;
using Domain.DTO;

namespace Domain.Interfaces.Services;

public interface IUserService
{
    Task<UserResponse> Get(string userId, CancellationToken cancellationToken = default);
    Task<ServiceResult<string>> Save(UserRequest user, CancellationToken cancellationToken = default);
    Task<ServiceResult<bool>> Update(string userId, UserUpdateRequest user, CancellationToken cancellationToken = default);
    Task<ServiceResult<UserWithPreferencesResponse>> ValidatePassword(LoginRequest sendedInfo, CancellationToken cancellationToken = default);
    Task<ServiceResult<bool>> UpdatePassword(string userId, UserUpdatePasswordRequest sendedInfo, CancellationToken cancellationToken = default);
    Task<bool> ActivateUser(string userId, string activationCode, CancellationToken cancellationToken = default);
}

