using Domain.DTO;
using Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace IAcademyUserAPI.Controllers.v1
{
    [ApiController]
    [Route("api/company")]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    public class CompanyController : ControllerBase
    {
        private readonly ILogger<CompanyController> _logger;
        private readonly ICompanyService _companyService;

        public CompanyController(
            ILogger<CompanyController> logger,
            ICompanyService companyService)
        {
            _logger = logger;
            _companyService = companyService;
        }

        /// <summary>
        /// Recupera empresa
        /// </summary>
        /// <param name="companyId">Identificacao da empresa (GUID de 36 caracteres)</param>
        /// <param name="cancellationToken">Token para cancelamento</param>
        /// <returns>Objeto com dados da empresa</returns>
        [HttpGet("{companyId}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get([FromRoute] string companyId, CancellationToken cancellationToken = default)
        {
            try
            {
                var company = await _companyService.GetById(companyId, cancellationToken);

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
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> Update([FromRoute] string companyId, [FromBody] CompanyRequest companyRequest, CancellationToken cancellationToken = default)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _companyService.Update(companyId, companyRequest, cancellationToken);

                return result ? NoContent() : BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user with ID: {CompanyId}", companyId);
                return BadRequest();
            }
        }
    }
}
