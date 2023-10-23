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
    [Route("api/company")]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    public class CompanyController : ControllerBase
    {
        private readonly ILogger<CompanyController> _logger;
        private readonly ICompanyService _companyService;
        private readonly IConfiguration _configuration;

        public CompanyController(
            ILogger<CompanyController> logger,
            ICompanyService companyService,
            IConfiguration configuration)
        {
            _logger = logger;
            _companyService = companyService;
            _configuration = configuration;
        }

        /// <summary>
        /// Recupera empresa
        /// </summary>
        /// <param name="companyId">Identificacao da empresa (GUID de 36 caracteres)</param>
        /// <param name="cancellationToken">Token para cancelamento</param>
        /// <returns>Objeto com dados da empresa</returns>
        [Authorize]
        [HttpGet("{companyId}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get([FromRoute] string companyId, CancellationToken cancellationToken = default)
        {
            try
            {
                var id = User.FindFirst("Id")?.Value;

                if (string.IsNullOrEmpty(id))
                    return BadRequest("Invalid Token");

                var company = await _companyService.GetById(id, cancellationToken);

                return string.IsNullOrEmpty(company.Id) ? NotFound() : Ok(company);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving company with ID: {CompanyId}", companyId);
                return BadRequest();
            }
        }

        /// <summary>
        /// Cadastrar uma nova empresa
        /// </summary>
        /// <param name="companyRequest">Objeto com dados da empresa</param>
        /// <param name="cancellationToken">Token para cancelamento</param>
        /// <returns>Identificacao da empresa criada, persistida no banco de dados</returns>
        [HttpPost]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> Save([FromBody] CompanyRequest companyRequest, CancellationToken cancellationToken = default)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var saveResult = await _companyService.Save(companyRequest, cancellationToken);

                return saveResult.Success ? Created(string.Empty, saveResult.Data) : BadRequest(saveResult.ErrorMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving company");
                return BadRequest();
            }
        }

        /// <summary>
        /// Atualizar empresa
        /// </summary>
        /// <param name="companyId">Identificacao da empresa (GUID de 36 caracteres)</param>
        /// <param name="companyRequest">Objeto com as novas informacoes da empresa</param>
        /// <param name="cancellationToken">Token para cancelamento</param>
        /// <returns></returns>
        [HttpPut("{companyId}")]
        [Authorize]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> Update([FromRoute] string companyId, [FromBody] CompanyRequest companyRequest, CancellationToken cancellationToken = default)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var id = User.FindFirst("Id")?.Value;

                if (string.IsNullOrEmpty(id))
                    return BadRequest("Invalid Token");

                var result = await _companyService.Update(id, companyRequest, cancellationToken);

                return result ? NoContent() : BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user with ID: {CompanyId}", companyId);
                return BadRequest();
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
        [ProducesResponseType(typeof(CompanyLoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Login([FromBody] CompanyLoginRequest loginRequest, CancellationToken cancellationToken = default)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var validateResult = await _companyService.ValidatePassword(loginRequest, cancellationToken);

                if (string.IsNullOrEmpty(validateResult.Data?.Id))
                    return Unauthorized("Invalid credentials");

                var expirationTimeInMinutes = int.Parse(_configuration.GetValue<string>("JwtSettings:ExpirationTimeInMinutes"));
                var secretKey = _configuration.GetValue<string>("JwtSettings:SecretKey");
                var key = Encoding.ASCII.GetBytes(secretKey);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim("Id", validateResult.Data.Id),
                        new Claim("Cnpj", validateResult.Data.Cnpj),
                        new Claim("Name", validateResult.Data.Name),
                        new Claim("LimitPlan", validateResult.Data.LimitPlan.ToString())
                    }),
                    Expires = DateTime.UtcNow.AddMinutes(expirationTimeInMinutes),
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature
                    )
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);

                return Ok(new CompanyLoginResponse
                {
                    Id = validateResult.Data.Id,
                    Name = validateResult.Data.Name,
                    Cnpj = validateResult.Data.Cnpj,
                    LimitPlan = validateResult.Data.LimitPlan,
                    Token = tokenHandler.WriteToken(token)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error validating password for company with CNPJ: {loginRequest.Cnpj}");
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Atualizar senha da empresa
        /// </summary>
        /// <param name="companyId">Idenificacao da empresa (GUID de 36 caracteres)</param>
        /// <param name="updatePasswordRequest">Objeto contendo informacoes das credenciais</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("{companyId}/update-password")]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdatePassword([FromRoute] string companyId, [FromBody] CompanyUpdatePasswordRequest updatePasswordRequest, CancellationToken cancellationToken = default)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var id = User.FindFirst("Id")?.Value;

                if (string.IsNullOrEmpty(id))
                    return BadRequest("Invalid Token");

                var result = await _companyService.UpdatePassword(id, updatePasswordRequest, cancellationToken);

                return result.Success ? NoContent() : BadRequest(result.ErrorMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating password for company with ID: {companyId}");
                return BadRequest();
            }
        }
    }
}
