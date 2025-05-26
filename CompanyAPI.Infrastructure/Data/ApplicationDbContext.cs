using CompanyAPI.Application.Common.Interfaces;
using CompanyAPI.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CompanyAPI.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        private readonly IMediator _mediator;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IMediator mediator)
            : base(options)
        {
            _mediator = mediator;
        }

        public DbSet<Company> Companies  => Set<Company>();
    }
}
