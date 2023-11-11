using CrossCutting.Helpers;
using Domain.DTO;
using Domain.Entities;
using Domain.Interfaces.Infra;
using MongoDB.Driver;

namespace Infra.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DbContext _dbContext;

    public UserRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User> GetById(string userId, CancellationToken cancellationToken = default) => 
        await (await _dbContext.User.FindAsync(u => u.Id == userId, cancellationToken: cancellationToken))
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<User> GetByCpf(string Cpf, CancellationToken cancellationToken = default) => 
        await (await _dbContext.User.FindAsync(u => u.Cpf == Cpf, cancellationToken: cancellationToken))
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<string> Save(UserRequest user, CancellationToken cancellationToken = default)
    {
        try
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);

            var newUser = new User()
            {
                Id = Guid.NewGuid().ToString(),
                Name = user.Name,
                Email = user.Email,
                CellphoneNumberWithDDD = user.CellphoneNumberWithDDD,
                Cpf = user.Cpf,
                IsActivated = false,
                PasswordHash = hashedPassword,
                CompanyRef = user.CompanyRef
            };

            await _dbContext.User.InsertOneAsync(newUser, null, cancellationToken);

            return newUser.Id;
        }
        catch
        {
            return string.Empty;
        }
    }

    public async Task<ServiceResult<bool>> Update(User userRecovered, UserUpdateRequest user, CancellationToken cancellationToken = default)
    {
        try
        { 
            if (!BCrypt.Net.BCrypt.Verify(user.Password, userRecovered.PasswordHash))
                return ServiceResult<bool>.MakeErrorResult("Invalid credentials.");

            var filterDefinition = Builders<User>.Filter.Eq(u => u.Id, userRecovered.Id);

            var filterUpdate = Builders<User>.Update
                .Set(u => u.Name, user.Name)
                .Set(u => u.Email, user.Email)
                .Set(u => u.CompanyRef, user.CompanyRef);

            var result = await _dbContext.User.UpdateOneAsync(filterDefinition, filterUpdate, null, cancellationToken);

            if (result.ModifiedCount > 0)
                return ServiceResult<bool>.MakeSuccessResult(true);

            return ServiceResult<bool>.MakeErrorResult("Error: user not updated.");
        }
        catch
        {
            return ServiceResult<bool>.MakeErrorResult("Error: user not updated.");
        }
    }

    public async Task<ServiceResult<bool>> Update(User modifiedUser, CancellationToken cancellationToken = default)
    {
        try
        {
            var filterDefinition = Builders<User>.Filter.Eq(u => u.Id, modifiedUser.Id);

            var updateDefinitions = new List<UpdateDefinition<User>>();

            if (modifiedUser.Name != null)
                updateDefinitions.Add(Builders<User>.Update.Set(u => u.Name, modifiedUser.Name));

            if (modifiedUser.Email != null)
                updateDefinitions.Add(Builders<User>.Update.Set(u => u.Email, modifiedUser.Email));

            if (modifiedUser.CompanyRef != null)
                updateDefinitions.Add(Builders<User>.Update.Set(u => u.CompanyRef, modifiedUser.CompanyRef));

            if (modifiedUser.IsActivated != null)
                updateDefinitions.Add(Builders<User>.Update.Set(u => u.IsActivated, true));

            if (modifiedUser.GenrePreferences != null)
                updateDefinitions.Add(Builders<User>.Update.Set(u => u.GenrePreferences, modifiedUser.GenrePreferences));

            if (!updateDefinitions.Any())
                return ServiceResult<bool>.MakeErrorResult("No fields to update.");

            var combinedUpdate = Builders<User>.Update.Combine(updateDefinitions);

            var result = await _dbContext.User.UpdateOneAsync(filterDefinition, combinedUpdate, null, cancellationToken);

            if (result.ModifiedCount > 0)
                return ServiceResult<bool>.MakeSuccessResult(true);

            return ServiceResult<bool>.MakeErrorResult("Error: user not updated.");
        }
        catch
        {
            return ServiceResult<bool>.MakeErrorResult("Error: user not updated.");
        }
    }

    public async Task<ServiceResult<UserWithPreferencesResponse>> ValidatePassword(LoginRequest sendedInfo, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _dbContext.User.Find(u => u.Email == sendedInfo.Email)
                .FirstOrDefaultAsync(cancellationToken);

            if (user is not null)
            {
                if(BCrypt.Net.BCrypt.Verify(sendedInfo.Password, user.PasswordHash))
                {
                    var userResponse = new UserWithPreferencesResponse()
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Cpf = user.Cpf,
                        Email = user.Email,
                        IsActivated = user.IsActivated,
                        CompanyRef = user.CompanyRef,
                        GenrePreferences = user.GenrePreferences
                    };

                    return ServiceResult<UserWithPreferencesResponse>.MakeSuccessResult(userResponse);
                }

                return ServiceResult<UserWithPreferencesResponse>.MakeErrorResult("Invalid Credentials");
            }

            return ServiceResult<UserWithPreferencesResponse>.MakeErrorResult("User not found.");
        }
        catch
        {
            return ServiceResult<UserWithPreferencesResponse>.MakeErrorResult("Error on validation process.");
        }
    }

    public async Task<ServiceResult<bool>> UpdatePassword(
        string userId,
        UserUpdatePasswordRequest sendedInfo,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _dbContext.User.Find(u => u.Id == userId)
                .FirstOrDefaultAsync(cancellationToken);

            if (user != null && BCrypt.Net.BCrypt.Verify(sendedInfo.OldPassword, user.PasswordHash))
            {
                if (sendedInfo.NewPassword == sendedInfo.ConfirmPassword)
                {
                    var newHashedPassword = BCrypt.Net.BCrypt.HashPassword(sendedInfo.NewPassword);

                    var filterDefinition = Builders<User>.Filter.Eq(u => u.Id, userId);

                    var filterUpdate = Builders<User>.Update
                        .Set(u => u.PasswordHash, newHashedPassword);

                    var result = await _dbContext.User
                        .UpdateOneAsync(filterDefinition, filterUpdate, null, cancellationToken);

                    if (result.ModifiedCount > 0)
                        return ServiceResult<bool>.MakeSuccessResult(true);

                    return ServiceResult<bool>.MakeErrorResult("Error: password not updated.");
                }
            }

            return ServiceResult<bool>.MakeErrorResult("Review informed credentials."); ;
        }
        catch
        {
            return ServiceResult<bool>.MakeErrorResult("Error on password update process."); ;
        }
    }
}
