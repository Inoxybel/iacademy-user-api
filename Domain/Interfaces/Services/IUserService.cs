﻿using Domain.DTO;
using Domain.Entities;

namespace Domain.Interfaces.Services;

public interface IUserService
{
    Task<UserResponse> Get(string userId, CancellationToken cancellationToken = default);
    Task<string> Save(UserRequest user, CancellationToken cancellationToken = default);
    Task<bool> Update(string userId, UserRequest user, CancellationToken cancellationToken = default);
    Task<UserResponse> ValidatePassword(LoginRequest sendedInfo, CancellationToken cancellationToken = default);
    Task<bool> UpdatePassword(string userId, UserUpdatePasswordRequest sendedInfo, CancellationToken cancellationToken = default);
}

