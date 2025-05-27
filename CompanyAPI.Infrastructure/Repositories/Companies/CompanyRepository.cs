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

        public async Task<bool> ExistsByIsinAsync(string isin, CancellationToken cancellationToken = default)
        {
            return await _context.Companies
                .AnyAsync(c => c.Isin == isin, cancellationToken);
        }

        public async Task<Company?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Companies
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<Company?> GetByIsinAsync(string isin, CancellationToken cancellationToken = default)
        {
            return await _context.Companies
                .FirstOrDefaultAsync(c => c.Isin == isin, cancellationToken);
        }

        public async Task Update(Company company, CancellationToken cancellationToken = default)
        {
            _context.Companies.Update(company);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<(List<Company> Items, int TotalCount)> GetAllAsync(int page, int pageSize,
         CancellationToken cancellationToken = default)
        {
            var query = _context.Companies.AsQueryable();

            var totalCount = await query.CountAsync(cancellationToken);

            var skip = (page - 1) * pageSize;
            var items = await query
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }
    }
}
