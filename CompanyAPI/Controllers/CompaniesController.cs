using CompanyAPI.Application.Features.Companies.Commands.CreateCompany;
using CompanyAPI.Application.Features.Companies.DTOs;
using CompanyAPI.Application.Features.Companies.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CompanyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CompaniesController> _logger;

        public CompaniesController(IMediator mediator, ILogger<CompaniesController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Create Compmany
        /// </summary>
        /// <param name="command">Company creation data</param>
        /// <returns>Created company</returns>
        [HttpPost]
        [ProducesResponseType(typeof(CompanyDto), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<CompanyDto>> CreateCompany([FromBody] CreateCompanyCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to create company: {Error}", result.Error);
                return BadRequest(new { error = result.Error });
            }

            _logger.LogInformation("Created company with ID {CompanyId}", result.Value!.Id);
            return CreatedAtAction(nameof(GetCompanyById), new { id = result.Value.Id }, result.Value);
        }

        /// Get Company by ID
        /// </summary>
        /// <param name="id">Company ID</param>
        /// <returns>Company details</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(CompanyDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<CompanyDto>> GetCompanyById(int id)
        {
            var query = new GetCompanyByIdQuery(id);
            var result = await _mediator.Send(query);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Company with ID {CompanyId} not found", id);
                return NotFound(new { error = result.Error });
            }

            return Ok(result.Value);
        }
    }
}
