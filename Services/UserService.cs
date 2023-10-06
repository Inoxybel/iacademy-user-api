using CrossCutting.Helpers;
using Domain.Constants;
using Domain.DTO;
using Domain.Interfaces.Infra;
using Domain.Interfaces.Services;

namespace Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ICompanyService _companyService;

    public UserService(
        IUserRepository userRepository,
        ICompanyService companyService)
    {
        _userRepository = userRepository;
        _companyService = companyService;
    }

    public async Task<UserResponse> Get(string userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetById(userId, cancellationToken);

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

    public async Task<ServiceResult<string>> Save(UserRequest user, CancellationToken cancellationToken = default)
    {
        var companyRef = ValidateCompanyRef(user.CompanyRef);

        if (string.IsNullOrEmpty(companyRef))
            return ServiceResult<string>.MakeErrorResult("Invalid Cnpj (CompanyRef).");

        var getResult = await _userRepository.GetByCpf(user.Cpf, cancellationToken);

        if (getResult is not null)
            return ServiceResult<string>.MakeErrorResult("User already exists");

        var userId = string.Empty;

        user.CompanyRef = companyRef;

        if(companyRef == Constants.MasterCnpj)
        {
            userId = await _userRepository.Save(user, cancellationToken);

            return MakeReturn(userId);
        }
            
        var company = await _companyService.GetByRef(companyRef, cancellationToken);

        if (company is null)
            return ServiceResult<string>.MakeErrorResult("Company not found.");

        var userIsIntegrated = company.ContainsUserDocument(user.Cpf);

        if (!userIsIntegrated)
            return ServiceResult<string>.MakeErrorResult("Registration failed. Please contact your company.");

        userId = await _userRepository.Save(user, cancellationToken);

        return MakeReturn(userId);
    }

    public async Task<ServiceResult<bool>> Update(string userId, UserUpdateRequest request, CancellationToken cancellationToken = default)
    {
        var userRecovered = await _userRepository.GetById(userId, cancellationToken);

        if (userRecovered is null)
            return ServiceResult<bool>.MakeErrorResult("User not found.");

        if(userRecovered.CompanyRef == request.CompanyRef)
            return await _userRepository.Update(userRecovered, request, cancellationToken);

        var company = await _companyService.GetByRef(request.CompanyRef, cancellationToken);

        if (company is null)
            return ServiceResult<bool>.MakeErrorResult("Company not found.");

        var userIsIntegrated = company.ContainsUserDocument(request.Cpf);

        if (!userIsIntegrated)
            return ServiceResult<bool>.MakeErrorResult("Registration failed. Please contact your company.");

        return await _userRepository.Update(userRecovered, request, cancellationToken);
    }

    public async Task<ServiceResult<UserResponse>> ValidatePassword(LoginRequest sendedInfo, CancellationToken cancellationToken = default)
    {
        return await _userRepository.ValidatePassword(sendedInfo, cancellationToken);
    }

    public async Task<ServiceResult<bool>> UpdatePassword(string userId, UserUpdatePasswordRequest sendedInfo, CancellationToken cancellationToken = default)
    {
        return await _userRepository.UpdatePassword(userId, sendedInfo, cancellationToken);
    }

    private static string ValidateCompanyRef(string companyRef)
    {
        if (string.IsNullOrEmpty(companyRef))
            return Constants.MasterCnpj;

        if (DocumentValidator.IsValidCnpj(companyRef))
            return companyRef;

        return string.Empty;
    }


    private static ServiceResult<string> MakeReturn(string userId)
    {
        if (string.IsNullOrEmpty(userId))
            return ServiceResult<string>.MakeErrorResult("Error on save process.");

        return ServiceResult<string>.MakeSuccessResult(userId);
    }
}
