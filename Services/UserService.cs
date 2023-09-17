using Domain.DTO;
using Domain.Interfaces.Infra;
using Domain.Interfaces.Services;

namespace Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserResponse> Get(string userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.Get(userId, cancellationToken);

        if (user is null)
            return new();

        return new()
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            CompanyRef = user.CompanyRef
        };
    }

    public async Task<string> Save(UserRequest user, CancellationToken cancellationToken = default)
    {
        return await _userRepository.Save(user, cancellationToken);
    }

    public async Task<bool> Update(string userId, UserRequest user, CancellationToken cancellationToken = default)
    {
        return await _userRepository.Update(userId, user, cancellationToken);
    }

    public async Task<UserResponse> ValidatePassword(LoginRequest sendedInfo, CancellationToken cancellationToken = default)
    {
        return await _userRepository.ValidatePassword(sendedInfo, cancellationToken);
    }

    public async Task<bool> UpdatePassword(string userId, UserUpdatePasswordRequest sendedInfo, CancellationToken cancellationToken = default)
    {
        return await _userRepository.UpdatePassword(userId, sendedInfo, cancellationToken);
    }
}
