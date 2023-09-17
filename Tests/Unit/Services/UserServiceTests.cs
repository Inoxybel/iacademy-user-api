using Domain.DTO;
using Domain.Entities;
using Domain.Interfaces.Infra;
using FluentAssertions;
using Moq;
using Services;
using Xunit;

namespace Tests.Unit.Services;

public class UserServiceTests
{
    private Mock<IUserRepository> _userRepositoryMock;
    private UserService _userService;

    public UserServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _userService = new UserService(_userRepositoryMock.Object);
    }

    [Fact]
    public async Task Get_SHOULD_ReturnNewUserResponse_WHEN_UserIsNull()
    {
        string userId = "123";

        var result = await _userService.Get(userId);

        result.Should().BeEquivalentTo(new UserResponse());
    }

    [Fact]
    public async Task Get_SHOULD_ReturnUserResponse_WHEN_UserIsNotNull()
    {
        string userId = "123";
        var user = new User 
        { 
            Id = userId, 
            Name = "Test User",
            Email = "test@example.com",
            CompanyRef = "ABC" 
        };

        _userRepositoryMock.Setup(x => x.Get(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var result = await _userService.Get(userId);

        result.Should().BeEquivalentTo(new UserResponse
        {
            Id = userId,
            Name = "Test User",
            Email = "test@example.com",
            CompanyRef = "ABC"
        });
    }

    [Fact]
    public async Task Save_SHOULD_ReturnUserId_WHEN_UserIsSavedSuccessfully()
    {
        var userRequest = new UserRequest
        {
            Name = "Test User",
            Email = "test@example.com",
            Password = "P@ssw0rd",
            CompanyRef = "ABC"
        };

        var expectedUserId = "12345";

        _userRepositoryMock.Setup(x => x.Save(userRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUserId);

        var result = await _userService.Save(userRequest);

        result.Should().Be(expectedUserId);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Update_SHOULD_ReturnExpectedResult_WHEN_UpdateIsCalled(bool updateResult)
    {
        string userId = "12345";

        var userRequest = new UserRequest
        {
            Name = "Test User",
            Email = "test@example.com",
            Password = "P@ssw0rd",
            CompanyRef = "ABC"
        };

        _userRepositoryMock.Setup(x => x.Update(userId, userRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(updateResult);

        var result = await _userService.Update(userId, userRequest);

        result.Should().Be(updateResult);
    }

    [Fact]
    public async Task ValidatePassword_SHOULD_ReturnUserResponse_WHEN_CredentialsAreValid()
    {
        var loginRequest = new LoginRequest 
        {
            Email = "test@example.com",
            Password = "P@ssw0rd"
        };

        var expectedUserResponse = new UserResponse
        {
            Id = "12345",
            Name = "Test User",
            Email = "test@example.com",
            CompanyRef = "ABC"
        };

        _userRepositoryMock.Setup(x => x.ValidatePassword(loginRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUserResponse);

        var result = await _userService.ValidatePassword(loginRequest);

        result.Should().BeEquivalentTo(expectedUserResponse);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task UpdatePassword_SHOULD_ReturnExpectedResult_WHEN_UpdateIsCalled(bool updateResult)
    {
        string userId = "12345";

        var userUpdatePasswordRequest = new UserUpdatePasswordRequest 
        { 
            Email = "test@example.com",
            OldPassword = "P@ssw0rd",
            NewPassword = "N3wP@ssw0rd",
            ConfirmPassword = "N3wP@ssw0rd"
        };

        _userRepositoryMock.Setup(x => x.UpdatePassword(userId, userUpdatePasswordRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(updateResult);

        var result = await _userService.UpdatePassword(userId, userUpdatePasswordRequest);

        result.Should().Be(updateResult);
    }

}

