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

        public async Task<Product?> GetByIdTrackedWithCategoryAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<Product?> GetByIdUntrackedAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Products.AsNoTracking().Include(p => p.Category).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<Product?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _context.Products.AsNoTracking().Include(p => p.Category).FirstOrDefaultAsync(x => x.Name == name.Trim(), cancellationToken);
        }

        public async Task<IReadOnlyList<Product>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default)
        {
            return await _context.Products.AsNoTracking()
                .Where(p => p.Name.Contains(searchTerm.Trim()) && p.IsActive && p.Category.IsActive).OrderBy(p => p.Name).ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Product>> SearchByNameIncludeInactiveAsync(string searchTerm, CancellationToken cancellationToken = default)
        {
            return await _context.Products.AsNoTracking().Where(p => p.Name.Contains(searchTerm.Trim())).OrderBy(p => p.Name).ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Product>> GetPaginatedUntrackedAsync
            (int pageNumber, int pageSize, Guid? categoryId, bool includeInactive, CancellationToken cancellationToken = default)
        {
            var query = _context.Products.AsNoTracking().AsQueryable();

            if (categoryId.HasValue) query = query.Where(p => p.CategoryId == categoryId.Value);

            if (!includeInactive) 
                query = query.Where(p => p.IsActive && p.Category.IsActive);
            else
                query = query.Include(p => p.Category);

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
