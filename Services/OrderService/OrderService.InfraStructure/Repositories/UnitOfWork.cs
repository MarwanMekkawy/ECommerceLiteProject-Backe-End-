namespace OrderService.InfraStructure.Repositories
{
    public class UnitOfWork(OrderDbContext _context) : IUnitOfWork
    {
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);

        }
    }
}
