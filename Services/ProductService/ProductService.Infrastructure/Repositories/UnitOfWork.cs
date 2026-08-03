using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Contracts;
using ProductService.Domain.Entities;

namespace ProductService.Infrastructure.Repositories
{
    public class UnitOfWork(ProductDbContext _context) : IUnitOfWork
    {
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // update UpdatedAt time with saving
            foreach (var entry in _context.ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Modified)
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }   
}
