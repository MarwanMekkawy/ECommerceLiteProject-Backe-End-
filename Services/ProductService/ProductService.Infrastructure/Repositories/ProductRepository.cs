using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Contracts;
using ProductService.Domain.Entities;


namespace ProductService.Infrastructure.Repositories
{
    public class ProductRepository(ProductDbContext _context) : IProductRepository
    {
        public async Task<Product?> GetByIdTrackedAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Products.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<Product?> GetByIdUntrackedAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<Product?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _context.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
        }

        public async Task<IReadOnlyList<Product>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default)
        {
            return await _context.Products.AsNoTracking().Where(p => p.Name.Contains(searchTerm) && p.IsActive).OrderBy(p => p.Name).ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Product>> SearchByNameIncludeInactiveAsync(string searchTerm, CancellationToken cancellationToken = default)
        {
            return await _context.Products.AsNoTracking().Where(p => p.Name.Contains(searchTerm)).OrderBy(p => p.Name).ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Product>> GetPaginatedUntrackedAsync
            (int pageNumber, int pageSize, Guid? categoryId, bool includeInactives, CancellationToken cancellationToken = default)
        {
            var query = _context.Products.AsNoTracking().AsQueryable();

            if (categoryId.HasValue) query = query.Where(p => p.CategoryId == categoryId.Value);

            if (!includeInactives) query = query.Where(p => p.IsActive);

            return await query.OrderBy(p => p.Name).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        }

        public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
        {
            await _context.Products.AddAsync(product, cancellationToken);
        }

        public void Remove(Product product)
        {
            _context.Products.Remove(product);
        }
    }
}
