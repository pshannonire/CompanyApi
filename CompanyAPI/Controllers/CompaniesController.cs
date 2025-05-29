using CompanyAPI.Application.Common.Models;
using CompanyAPI.Application.Features.Companies.Commands.CreateCompany;
using CompanyAPI.Application.Features.Companies.Commands.UpdateCompany;
using CompanyAPI.Application.Features.Companies.DTOs;
using CompanyAPI.Application.Features.Companies.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CompanyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CompaniesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CompaniesController> _logger;

        public CompaniesController(IMediator mediator, ILogger<CompaniesController> logger)
        {
            _mediator = mediator;
            _logger = logger;
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


        /// Get Company by ISIN
        /// </summary>
        /// <param name="isin">ISIN</param>
        /// <returns>Company details</returns>
        [HttpGet("isin/{isin}")]
        [ProducesResponseType(typeof(CompanyDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<CompanyDto>> GetCompanyByIsin(string isin)
        {
            var query = new GetCompanyByIsinQuery(isin);
            var result = await _mediator.Send(query);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Company with ISIN {ISIN} not found", isin);
                return NotFound(new { error = result.Error });
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Get all companies with pagination
        /// </summary>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="sortBy">Sort field</param>
        /// <param name="sortDescending">Sort direction</param>
        /// <param name="searchTerm">Search term (filter on ISIN, Exchange, Company name or Ticker</param>
        /// <returns>Paginated list of companies</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedList<CompanyDto>), 200)]
        public async Task<ActionResult<PaginatedList<CompanyDto>>> GetAllCompanies(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = null,
            [FromQuery] bool sortDescending = false,
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? exchange = null)
        {
            var query = new GetAllCompaniesQuery
            {
                Page = pageNumber,
                PageSize = pageSize,
                SortBy = sortBy,
                SortDescending = sortDescending,
                SearchTerm = searchTerm,
            };

            var result = await _mediator.Send(query);

            if (!result.IsSuccess)
            {
                _logger.LogError("Failed to retrieve companies: {Error}", result.Error);
                return BadRequest(new { error = result.Error });
            }

            _logger.LogInformation("Retrieved {CompanyCount} companies (page {Page} of {TotalPages})",
                result.Value!.Items.Count, result.Value.PageNumber, result.Value.TotalPages);

            return Ok(result.Value);
        }

        /// <summary>
        /// Create Company
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

        /// <summary>
        /// Update Company
        /// </summary>
        /// <param name="command">Company to update</param>
        /// <returns>Updated company</returns>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(CompanyDto), 200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<CreateCompanyDto>> UpdateCompany([FromBody] UpdateCompanyCommand command)
        {
            command.Id = int.Parse(RouteData.Values["id"]?.ToString() ?? "0");
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to update company: {Error}", result.Error);
                return BadRequest(new { error = result.Error });
            }

            _logger.LogInformation("Updated company with ID {CompanyId}", result.Value!.Id);
            return Ok(result.Value);
        }
    }
}
