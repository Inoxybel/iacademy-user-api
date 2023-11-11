using Domain.Constants;
using Domain.DTO;
using Domain.Entities;
using Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Numerics;

namespace IAcademyUserAPI.Controllers.v1;

[ApiController]
[Route("api/plan")]
[Authorize]
public class PlanController : ControllerBase
{
    private readonly ILogger<PlanController> _logger;
    private readonly IPlanService _planService;

    public PlanController(
        ILogger<PlanController> logger,
        IPlanService planService)
    {
        _logger = logger;
        _planService = planService;
    }

    /// <summary>
    /// Recupera plano
    /// </summary>
    /// <param name="planId">Id do plano</param>
    /// <param name="cancellationToken">Token para cancelamento</param>
    /// <returns>Objeto com dados da empresa</returns>
    [HttpGet("{planId}")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(Plan), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] string planId, CancellationToken cancellationToken = default)
    {
        try
        {
            var ownerId = User.FindFirst("Id")?.Value;

            if (string.IsNullOrEmpty(ownerId) || ownerId != Constants.MasterCnpj)
                return BadRequest("Invalid Token");

            var plan = await _planService.GetById(planId, cancellationToken);

            return string.IsNullOrEmpty(plan.Id) ? NotFound() : Ok(plan);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving plan with ID: {PlanId}", planId);
            return BadRequest();
        }
    }

    /// <summary>
    /// Cria novo plano
    /// </summary>
    /// <param name="request">Objeto de criação do plano</param>
    /// <param name="cancellationToken">Token para cancelamento</param>
    /// <returns>Id do plano criado</returns>
    [HttpPost]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Save([FromBody] CreatePlanRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var ownerId = User.FindFirst("Id")?.Value;

            if (string.IsNullOrEmpty(ownerId) || ownerId != Constants.MasterCnpj)
                return BadRequest("Invalid Token");

            var planId = await _planService.Save(request, cancellationToken);

            return string.IsNullOrEmpty(planId) ? NotFound() : Ok(planId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error to save plan requested: {Plan}", request);
            return BadRequest();
        }
    }

    /// <summary>
    /// Atualiza plano
    /// </summary>
    /// <param name="planId">Id do plano</param>
    /// <param name="request">Objeto de atualização</param>
    /// <param name="cancellationToken">Token para cancelamento</param>
    /// <returns>Objeto com dados da empresa</returns>
    [HttpPut("{planId}")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromRoute] string planId, [FromBody] UpdatePlanRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var ownerId = User.FindFirst("Id")?.Value;

            if (string.IsNullOrEmpty(ownerId) || ownerId != Constants.MasterCnpj)
                return BadRequest("Invalid Token");

            var planResult = await _planService.Update(request, cancellationToken);

            return planResult.Success ? Ok() : NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error update plan with ID: {PlanId}", planId);
            return BadRequest();
        }
    }
}

