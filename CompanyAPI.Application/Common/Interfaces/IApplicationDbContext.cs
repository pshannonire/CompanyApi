using CompanyAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CompanyAPI.Application.Common.Interfaces
{
    /// <summary>
    /// Database context interface for the Application layer.
    /// </summary>
    public interface IApplicationDbContext
    {
        /// <summary>
        /// Companies DbSet for database operations
        /// </summary>
        DbSet<Company> Companies { get; }

        /// <summary>
        /// Save changes to the database
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Number of affected records</returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
