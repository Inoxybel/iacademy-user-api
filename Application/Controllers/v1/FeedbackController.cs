using Domain.DTO;
using Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace IAcademyUserAPI.Controllers.v1
{
    [ApiController]
    [Route("api/feedback")]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    public class FeedbackController : ControllerBase
    {
        private readonly ILogger<CompanyController> _logger;
        private readonly IFeedbackService _feedbackService;

        public FeedbackController(
            ILogger<CompanyController> logger,
            IFeedbackService feedbackService)
        {
            _logger = logger;
            _feedbackService = feedbackService;
        }

        /// <summary>
        /// Cadastrar novo feedback
        /// </summary>
        /// <param name="request">Objeto com dados do feedback</param>
        /// <param name="cancellationToken">Token para cancelamento</param>
        /// <returns>Identificacao do feedback criado</returns>
        [HttpPost]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> Save([FromBody] FeedbackRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var feedbackId = await _feedbackService.Save(request, cancellationToken);

                return !string.IsNullOrEmpty(feedbackId) ? Created(string.Empty, feedbackId) : BadRequest("Erro ao salvar feedback");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving feedback");
                return BadRequest();
            }
        }
    }
}
