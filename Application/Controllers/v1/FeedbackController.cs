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
    [Route("api/feedback")]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    public class FeedbackController : ControllerBase
    {
        private readonly ILogger<CompanyController> _logger;
        private readonly ICompanyService _companyService;

        public FeedbackController(
            ILogger<CompanyController> logger,
            ICompanyService companyService)
        {
            _logger = logger;
            _companyService = companyService;
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
    }
}
