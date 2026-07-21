using Domain.Exceptions;
using IdentityService.Domain.Contracts;
using IdentityService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        protected readonly IdentityDbContext _context;

        public UserRepository(IdentityDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
        }

        public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _context.Users.AnyAsync(x => x.Email == email, cancellationToken);
        }

        public async Task<IReadOnlyList<User>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            return await _context.Users.AsNoTracking().OrderBy(x => x.CreatedAt).Skip((page - 1) * pageSize)
                .Take(pageSize).ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<User>> GetPagedActiveAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            return await _context.Users.AsNoTracking().Where(x => x.IsActive).OrderBy(x => x.CreatedAt)
                .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<User>> GetPagedInactiveAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            return await _context.Users.AsNoTracking().Where(x => !x.IsActive).OrderBy(x => x.CreatedAt)
                .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        }

        public async Task<int> GetTotalCountAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Users.CountAsync(cancellationToken);
        }

        public async Task<int> GetActiveCountAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Users.Where(x => x.IsActive).CountAsync(cancellationToken);
        }


        public async Task AddAsync(User user, CancellationToken cancellationToken = default)
        {
            await _context.Users.AddAsync(user, cancellationToken);
        }

        public async Task ActivateAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken) ?? throw new NotFoundException();
            user.Activate();
        }

        public async Task DeactivateAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken) ?? throw new NotFoundException();
            user.Deactivate();
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken) ?? throw new NotFoundException();
            _context.Users.Remove(user);
        }
    }
}
