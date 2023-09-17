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

    public async Task<User> Get(string userId, CancellationToken cancellationToken = default)
    {
        return await (await _dbContext.User.FindAsync(u => u.Id == userId, cancellationToken: cancellationToken))
            .FirstOrDefaultAsync(cancellationToken);
    }

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

    public async Task<bool> Update(string userId, UserRequest user, CancellationToken cancellationToken = default)
    {
        try
        {
            var userRecovered = await Get(userId, cancellationToken);

            if (userRecovered is null)
                return false;

            if (!BCrypt.Net.BCrypt.Verify(user.Password, userRecovered.PasswordHash))
                return false;

            var filterDefinition = Builders<User>.Filter.Eq(u => u.Id, userId);

            var filterUpdate = Builders<User>.Update
                .Set(u => u.Name, user.Name)
                .Set(u => u.Email, user.Email)
                .Set(u => u.CompanyRef, user.CompanyRef);

            var result = await _dbContext.User.UpdateOneAsync(filterDefinition, filterUpdate, null, cancellationToken);

            return result.ModifiedCount > 0;
        }
        catch
        {
            return false;
        }
    }

    public async Task<UserResponse> ValidatePassword(LoginRequest sendedInfo, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _dbContext.User.Find(u => u.Email == sendedInfo.Email)
                .FirstOrDefaultAsync(cancellationToken);

            if (user is not null)
            {
                if(BCrypt.Net.BCrypt.Verify(sendedInfo.Password, user.PasswordHash))
                {
                    return new()
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Email = user.Email,
                        CompanyRef = user.CompanyRef
                    };
                }

                return new();
            }

            return new();
        }
        catch
        {
            return new();
        }
    }

    public async Task<bool> UpdatePassword(
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

                    return result.ModifiedCount > 0;
                }
            }
            return false;
        }
        catch
        {
            return false;
        }
    }
}
