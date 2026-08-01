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

        public async Task<IReadOnlyList<Product>> GetPaginatedUntrackedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            return await _context.Products.AsNoTracking().Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
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
