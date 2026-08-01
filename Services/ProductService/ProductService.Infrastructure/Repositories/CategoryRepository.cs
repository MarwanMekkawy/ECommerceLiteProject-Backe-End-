using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Contracts;
using ProductService.Domain.Entities;


namespace ProductService.Infrastructure.Repositories
{
    public class CategoryRepository(ProductDbContext _context) : ICategoryRepository
    {
        public async Task<Category?> GetByIdTrackedAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Categories.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<Category?> GetByIdUntrackedAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<Category>> GetPaginatedUntrackedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            return await _context.Categories.AsNoTracking().Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task AddAsync(Category category, CancellationToken cancellationToken = default)
        {
            await _context.Categories.AddAsync(category, cancellationToken);
        }

        public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Categories.AnyAsync(x => x.Id == id, cancellationToken);
        }

        public void Remove(Category category)
        {
            _context.Categories.Remove(category);
        }
    }
}
