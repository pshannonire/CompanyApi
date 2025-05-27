using CompanyAPI.Domain.Entities;

namespace CompanyAPI.Application.Common.Interfaces.Companies
{
    public interface ICompanyRepository
    {
        Task<Company?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task AddAsync(Company company, CancellationToken cancellationToken = default);
        Task<bool> ExistsByIsinAsync(string isin, CancellationToken cancellationToken);
        Task Update(Company company, CancellationToken cancellationToken);
        Task<Company?> GetByIsinAsync(string isin, CancellationToken cancellationToken);
        Task<(List<Company> Items, int TotalCount)> GetAllAsync(int page, int pageSize,CancellationToken cancellationToken = default);
    }
}
