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
        }

        public async Task<(List<Company> Items, int TotalCount)> GetAllAsync(int page, int pageSize, string? sortBy, bool sortDescending,
            string? searchTerm, CancellationToken cancellationToken = default)
        {
            var query = _context.Companies.AsQueryable();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var searchLower = searchTerm.ToLower();
                query = query.Where(c =>
                    c.Name.ToLower().Contains(searchLower) ||
                    c.Ticker.ToLower().Contains(searchLower) ||
                    c.Isin.ToLower().Contains(searchLower) ||
                    c.Exchange.ToLower().Contains(searchLower));
            }

            // Apply sorting
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                query = sortBy.ToLower() switch
                {
                    "name" => sortDescending ? query.OrderByDescending(c => c.Name) : query.OrderBy(c => c.Name),
                    "ticker" => sortDescending ? query.OrderByDescending(c => c.Ticker) : query.OrderBy(c => c.Ticker),
                    "exchange" => sortDescending ? query.OrderByDescending(c => c.Exchange) : query.OrderBy(c => c.Exchange),
                    "isin" => sortDescending ? query.OrderByDescending(c => c.Isin) : query.OrderBy(c => c.Isin),
                    "createdat" => sortDescending ? query.OrderByDescending(c => c.CreatedAt) : query.OrderBy(c => c.CreatedAt),
                    _ => query.OrderBy(c => c.Id)
                };
            }
            else
            {
                query = query.OrderBy(c => c.Id);
            }

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
