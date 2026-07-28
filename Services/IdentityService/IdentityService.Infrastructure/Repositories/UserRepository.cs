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
    public class UserRepository(IdentityDbContext _context) : IUserRepository
    {
        public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
        }

        public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken)
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


        public async Task AddAsync(User user, CancellationToken cancellationToken   )
        {
            await _context.Users.AddAsync(user, cancellationToken);
        }
      
        public void Delete(User user)
        {
            _context.Users.Remove(user);
        }
    }
}
