using CrossCutting.Helpers;
using Domain.DTO;
using Domain.Entities;
using Domain.Interfaces.Infra;
using Domain.Interfaces.Services;
using FluentAssertions;
using Moq;
using Services;
using Xunit;

namespace Tests.Unit.Services;

public class UserServiceTests
{
    private Mock<IUserRepository> _userRepositoryMock;
    private Mock<ICompanyService> _companyServiceMock;
    private UserService _userService;

    public UserServiceTests()
    {
        _userRepositoryMock = new();
        _companyServiceMock = new();
        _userService = new UserService(
            _userRepositoryMock.Object,
            _companyServiceMock.Object
        );
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

        _userRepositoryMock.Setup(x => x.GetById(userId, It.IsAny<CancellationToken>()))
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
            Password = "P@ssw0rd"
        };

        var expectedUserId = "12345";

        _userRepositoryMock.Setup(x => x.Save(userRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUserId);

        var result = await _userService.Save(userRequest);

        result.Data.Should().Be(expectedUserId);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Update_SHOULD_ReturnExpectedResult_WHEN_UpdateIsCalled(bool updateResult)
    {
        var user = new User()
        {
            CompanyRef = "iacademy"
        };

        var userRequest = new UserUpdateRequest
        {
            Name = "Test User",
            Email = "test@example.com",
            Password = "P@ssw0rd",
            CompanyRef = user.CompanyRef
        };

        _userRepositoryMock.Setup(x => x.GetById(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        if(updateResult)
            _userRepositoryMock.Setup(x => x.Update(user, userRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<bool>.MakeSuccessResult(updateResult));
        else
            _userRepositoryMock.Setup(x => x.Update(user, userRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<bool>.MakeErrorResult("Error"));

        var result = await _userService.Update("userId", userRequest);

        result.Data.Should().Be(updateResult);
    }

    [Fact]
    public async Task ValidatePassword_SHOULD_ReturnUserResponse_WHEN_CredentialsAreValid()
    {
        var loginRequest = new LoginRequest 
        {
            Email = "test@example.com",
            Password = "P@ssw0rd"
        };

        var expectedUserResponse = new ServiceResult<UserResponse>
        {
            Success = true,
            Data = new()
            {
                Id = "12345",
                Name = "Test User",
                Email = "test@example.com",
                CompanyRef = "ABC"
            }
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
        var userId = "userId";

        var userUpdatePasswordRequest = new UserUpdatePasswordRequest 
        { 
            Email = "test@example.com",
            OldPassword = "P@ssw0rd",
            NewPassword = "N3wP@ssw0rd",
            ConfirmPassword = "N3wP@ssw0rd"
        };

        if (updateResult)
            _userRepositoryMock.Setup(x => x.UpdatePassword(userId, userUpdatePasswordRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<bool>.MakeSuccessResult(updateResult));
        else
            _userRepositoryMock.Setup(x => x.UpdatePassword(userId, userUpdatePasswordRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<bool>.MakeErrorResult("Error"));

        var result = await _userService.UpdatePassword(userId, userUpdatePasswordRequest);

        result.Data.Should().Be(updateResult);
    }

    [Fact]
    public async Task Save_SHOULD_ReturnError_WHEN_CompanyRefIsInvalidCnpj()
    {
        var userRequest = new UserRequest
        {
            Name = "Test User",
            Email = "test@example.com",
            Password = "P@ssw0rd",
            CompanyRef = "InvalidCnpj"
        };

        var result = await _userService.Save(userRequest);

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Invalid Cnpj (CompanyRef).");
    }

    [Fact]
    public async Task Save_SHOULD_ReturnError_WHEN_CompanyIsNotFound()
    {
        var userRequest = new UserRequest
        {
            CompanyRef = "27104771000185"
        };

        var result = await _userService.Save(userRequest);

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Company not found.");
    }

    [Fact]
    public async Task Save_SHOULD_ReturnError_WHEN_UserIsNotIntegratedToCompany()
    {
        var userRequest = new UserRequest
        {
            Cpf = "11111111111",
            CompanyRef = "27104771000185"
        };

        var company = new Company
        {
            Cnpj = userRequest.CompanyRef,
            Groups = new()
            {
                new CompanyGroup
                {
                    UsersDocument = new()
                }
            }
        };

        _companyServiceMock.Setup(x => x.GetByRef(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync(company);

        var result = await _userService.Save(userRequest);

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Registration failed. Please contact your company.");
    }

    [Fact]
    public async Task Update_SHOULD_ReturnError_WHEN_UserIsNotFound()
    {
        var userRequest = new UserUpdateRequest();

        var result = await _userService.Update("userId", userRequest);

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("User not found.");
    }

    [Fact]
    public async Task Update_SHOULD_ReturnError_WHEN_RequestCompanyIsNotFound()
    {
        var userRequest = new UserUpdateRequest
        {
            CompanyRef = "NewCompanyRef"
        };
        var existingUser = new User
        {
            CompanyRef = "ExistingCompanyRef"
        };

        _userRepositoryMock.Setup(x => x.GetById(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync(existingUser);

        var result = await _userService.Update("userId", userRequest);

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Company not found.");
    }

    [Fact]
    public async Task Update_SHOULD_ReturnError_WHEN_UserIsNotIntegratedToRequestCompany()
    {
        var userRequest = new UserUpdateRequest
        {
            Cpf = "11111111111",
            CompanyRef = "67601037000146"
        };

        var existingUser = new User
        {
            CompanyRef = "27104771000185"
        };

        var company = new Company
        {
            Cnpj = userRequest.CompanyRef,
            Groups = new()
            {
                new CompanyGroup
                {
                    UsersDocument = new()
                }
            }
        };

        _userRepositoryMock.Setup(x => x.GetById(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync(existingUser);

        _companyServiceMock.Setup(x => x.GetByRef(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync(company);

        var result = await _userService.Update("userId", userRequest);

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Registration failed. Please contact your company.");
    }
}

