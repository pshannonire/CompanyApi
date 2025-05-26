using CompanyAPI.Application.Common.Interfaces.Companies;
using CompanyAPI.Domain.Entities;
using CompanyAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CompanyAPI.Infrastructure.Repositories.Companies
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly ApplicationDbContext _context;

        public CompanyRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Company company, CancellationToken cancellationToken = default)
        {
            await _context.Companies.AddAsync(company, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<Company?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Companies
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }
    }
}
