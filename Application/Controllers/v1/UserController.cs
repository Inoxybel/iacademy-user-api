using Domain.Constants;
using Domain.DTO;
using Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mime;
using System.Security.Claims;
using System.Text;

namespace IAcademyUserAPI.Controllers.v1
{
    [ApiController]
    [Route("api/user")]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;
        private readonly IActivationCodeService _activationCodeService;
        private readonly IConfiguration _configuration;

        public UserController(
            ILogger<UserController> logger,
            IUserService userService,
            IConfiguration configuration,
            IActivationCodeService activationCodeService)
        {
            _logger = logger;
            _userService = userService;
            _configuration = configuration;
            _activationCodeService = activationCodeService;
        }

        /// <summary>
        /// Recuperar dados de um usuario pelo id
        /// </summary>
        /// <param name="userId">Id em forma de GUID (36 caracteres)</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Objeto contendo:
        /// Id - Identificacao em forma de GUID (36 caracteres)
        /// Name - Nome do usuario (minimo de 3 caracteres)
        /// Email - Email do usuario
        /// CompanyRef - Cnpj da empresa que o usuario esta agregado</returns>
        [Authorize]
        [HttpGet("{userId}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get([FromRoute] string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var user = await _userService.Get(userId, cancellationToken);

                if (!string.IsNullOrEmpty(user.Id))
                    return Ok(user);

                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user with ID: {UserId}", userId);
                return BadRequest();
            }
        }

        /// <summary>
        /// Cadastrar um novo usuario
        /// </summary>
        /// <param name="userRequest">Objeto contendo os dados a serem cadastrados. PS: CompanyRef e opcional</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Id do usuario cadastrado em forma de GUID (36 caracteres)</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(string))]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> Save([FromBody] UserRequest userRequest, CancellationToken cancellationToken = default)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var saveResult = await _userService.Save(userRequest, cancellationToken);

                if (string.IsNullOrEmpty(saveResult.Data))
                    return BadRequest("Cnpj invalido.");

                return Created(string.Empty, saveResult.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving user");
                return BadRequest("Error saving user");
            }
        }

        /// <summary>
        /// Atualizar um usuario ja cadastrado
        /// </summary>
        /// <param name="userId">Identificacao do usuario (GUID de 36 caracteres)</param>
        /// <param name="userRequest">Objeto contendo os novos dados a serem salvos. PS: CompanyRef e opcional.</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("{userId}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> Update([FromRoute] string userId, [FromBody] UserUpdateRequest userRequest, CancellationToken cancellationToken = default)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var id = User.FindFirst("OwnerId")?.Value;

                if (userId != id && id != Constants.MasterCnpj)
                    return BadRequest("Invalid Token");

                var result = await _userService.Update(userId, userRequest, cancellationToken);

                return result.Success ? NoContent() : BadRequest(result.ErrorMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user with ID: {UserId}", userId);
                return BadRequest();
            }
        }

        /// <summary>
        /// Endpoint para ativar um usuario recem cadastrado
        /// </summary>
        /// <param name="userId">Identificacao do usuario (GUID de 36 caracteres)</param>
        /// <param name="code">Codigo de 6 digitos numericos</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Action Result</returns>
        [HttpPost("{userId}/active/{code}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ActiveUser([FromRoute] string userId, [FromRoute] string code, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userService.Get(userId, cancellationToken);

                if (string.IsNullOrEmpty(user.Id))
                    return BadRequest("User not found");

                if (user.IsActivated)
                    return Ok("User is already active");

                var activationCode = await _activationCodeService.Get(userId, cancellationToken);

                if (string.IsNullOrEmpty(activationCode.Id))
                    return BadRequest("Code not found");

                if (code != activationCode.Code)
                    return BadRequest("Invalid code for this user");

                var activationUserResponse = await _userService.ActivateUser(userId, activationCode.Id, cancellationToken);

                if (!activationUserResponse)
                    return BadRequest("Error to validate user");

                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest($"An error occoured: {ex}");
            }
        }

        /// <summary>
        /// Obter acesso a plataforma
        /// </summary>
        /// <param name="loginRequest">Objeto contendo credeciais de acesso</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Objeto contendo o Token que concede acesso aos demais endpoints</returns>
        [HttpPost("login")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest, CancellationToken cancellationToken = default)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var validateResult = await _userService.ValidatePassword(loginRequest, cancellationToken);

                if (string.IsNullOrEmpty(validateResult.Data?.Id))
                    return Unauthorized("Invalid credentials");

                if(!validateResult.Data.IsActivated)
                    return Unauthorized("User is not active");

                var expirationTimeInMinutes = int.Parse(_configuration.GetValue<string>("JwtSettings:ExpirationTimeInMinutes"));
                var secretKey = _configuration.GetValue<string>("JwtSettings:SecretKey");
                var key = Encoding.ASCII.GetBytes(secretKey);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim("OwnerId", validateResult.Data.Id),
                        new Claim("Document", validateResult.Data.Cpf),
                        new Claim("CompanyRef", validateResult.Data.CompanyRef),
                        new Claim("TextGenres", "[" + string.Join(",", validateResult.Data.GenrePreferences.Select(g => $"\"{g.Genre}\"")) + "]")
                    }),
                    Expires = DateTime.UtcNow.AddMinutes(expirationTimeInMinutes),
                    SigningCredentials = new SigningCredentials(
                         new SymmetricSecurityKey(key),
                         SecurityAlgorithms.HmacSha256Signature
                     )
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);

                return Ok(new LoginResponse
                {
                    Id = validateResult.Data.Id,
                    Name = validateResult.Data.Name,
                    Cpf = validateResult.Data.Cpf,
                    Email = validateResult.Data.Email,
                    CompanyRef = validateResult.Data.CompanyRef,
                    Token = tokenHandler.WriteToken(token)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating password for user with ID: {Email}", loginRequest.Email);
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Atualizar senha do usuario
        /// </summary>
        /// <param name="userId">Idenificacao do usuario (GUID de 36 caracteres)</param>
        /// <param name="updatePasswordRequest">Objeto contendo informacoes das credenciais</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("{userId}/update-password")]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdatePassword([FromRoute] string userId, [FromBody] UserUpdatePasswordRequest updatePasswordRequest, CancellationToken cancellationToken = default)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _userService.UpdatePassword(userId, updatePasswordRequest, cancellationToken);

                return result.Success ? NoContent() : BadRequest(result.ErrorMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating password for user with ID: {userId}");
                return BadRequest();
            }
        }
    }
}