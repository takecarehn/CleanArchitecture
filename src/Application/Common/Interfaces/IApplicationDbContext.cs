using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Log> Logs { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
