using CompanyAPI.Domain.Entities;

namespace CompanyAPI.Application.Common.Interfaces.Companies
{
    public interface ICompanyRepository
    {
        Task<Company?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task AddAsync(Company company, CancellationToken cancellationToken = default);

    }
}
