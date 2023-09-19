using Domain.DTO;
using Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mime;
using System.Security.Claims;
using System.Text;

namespace IAcademy_User_API.Controllers.v1
{
    [ApiController]
    [Route("api/user")]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public UserController(
            ILogger<UserController> logger, 
            IUserService userService,
            IConfiguration configuration)
        {
            _logger = logger;
            _userService = userService;
            _configuration = configuration;
        }

        /// <summary>
        /// Recuperar dados de um usuário pelo Id
        /// </summary>
        /// <param name="userId">Id em forma de GUID (36 caracteres)</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Objeto contendo:
        /// Id - Identificação em forma de GUID (36 caracteres)
        /// Name - Nome do usuário (mínimo de 3 caracteres)
        /// Email - Email do usuário
        /// CompanyRef - CNPJ da empresa que o usuário está agregado</returns>
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
        /// Cadastrar um novo usuário
        /// </summary>
        /// <param name="userRequest">Objeto contendo os dados a serem cadastrados. PS: CompanyRef é opcional</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Id do usuário cadastrado em forma de GUID (36 caracteres)</returns>
        [HttpPost]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> Save([FromBody] UserRequest userRequest, CancellationToken cancellationToken = default)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userId = await _userService.Save(userRequest, cancellationToken);

                return Created(string.Empty, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving user");
                return BadRequest();
            }
        }

        /// <summary>
        /// Atualizar um usuário já cadastrado
        /// </summary>
        /// <param name="userId">Identificação do usuário (GUID de 36 caracteres)</param>
        /// <param name="userRequest">Objeto contendo os novos dados a serem salvos. PS: CompanyRef é opcional.</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("{userId}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> Update([FromRoute] string userId, [FromBody] UserRequest userRequest, CancellationToken cancellationToken = default)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _userService.Update(userId, userRequest, cancellationToken);

                return result ? NoContent() : NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user with ID: {UserId}", userId);
                return BadRequest();
            }
        }

        /// <summary>
        /// Obter acesso à plataforma
        /// </summary>
        /// <param name="loginRequest">Objeto contendo credeciais de acesso</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Objeto contendo o Token que dá acesso aos demais endpoints</returns>
        [HttpPost("login")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest, CancellationToken cancellationToken = default)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = await _userService.ValidatePassword(loginRequest, cancellationToken);

                if(string.IsNullOrEmpty(user.Id))
                    return Unauthorized("Invalid credentials");

                var expirationTimeInMinutes = int.Parse(_configuration.GetValue<string>("IAcademy:JwtSettings:ExpirationTimeInMinutes"));
                var secretKey = _configuration.GetValue<string>("IAcademy:JwtSettings:SecretKey");
                var key = Encoding.ASCII.GetBytes(secretKey);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.PrimarySid, user.Id)
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
                     Token = tokenHandler.WriteToken(token) 
                 });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating password for user with ID: {Email}", loginRequest.Email);
                return BadRequest();
            }
        }

        /// <summary>
        /// Atualizar senha do usuário
        /// </summary>
        /// <param name="userId">Idenificação do usuário (GUID de 36 caracteres)</param>
        /// <param name="updatePasswordRequest">Objeto contendo informações das credenciais</param>
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
                
                return result ? NoContent() : BadRequest("Unable to update password");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating password for user with ID: {UserId}", userId);
                return BadRequest();
            }
        }
    }
}