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

        public async Task<Category?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _context.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Name == name.Trim(), cancellationToken);
        }

        public async Task<IReadOnlyList<Category>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default)
        {
            return await _context.Categories.AsNoTracking().Where(p => p.Name.Contains(searchTerm.Trim()) && p.IsActive).OrderBy(p => p.Name).ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Category>> SearchByNameIncludingInactiveAsync(string searchTerm, CancellationToken cancellationToken = default)
        {
            return await _context.Categories.AsNoTracking().Where(p => p.Name.Contains(searchTerm.Trim())).OrderBy(p => p.Name).ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Category>> GetPaginatedUntrackedAsync
            (int pageNumber, int pageSize, bool includeInactive, CancellationToken cancellationToken = default)
        {
            if (includeInactive)
                return await _context.Categories.AsNoTracking().Skip((pageNumber - 1) * pageSize).OrderBy(c => c.Name).Take(pageSize).ToListAsync(cancellationToken);

            return await _context.Categories.AsNoTracking().Where(c=>c.IsActive).OrderBy(c => c.Name).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        }

        public async Task AddAsync(Category category, CancellationToken cancellationToken = default)
        {
            await _context.Categories.AddAsync(category, cancellationToken);
        }

        public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Categories.AnyAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<bool> HasProductsAsync(Guid categoryId, CancellationToken cancellationToken = default)
        {
            return await _context.Products.AnyAsync(p => p.CategoryId == categoryId, cancellationToken);
        }

        public void Remove(Category category)
        {
            _context.Categories.Remove(category);
        }
    }
}
