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
    public class UserPasswordHistoryRepository(IdentityDbContext _context) : IUserPasswordHistoryRepository
    {
        public async Task AddAsync(UserPasswordHistory historyRecord, CancellationToken cancellationToken = default)
        {
            await _context.UserPasswordHistory.AddAsync(historyRecord, cancellationToken);
        }

        public async Task<IReadOnlyList<UserPasswordHistory>> GetAllByUserIdAsync(Guid userId, int count, CancellationToken cancellationToken = default)
        {
            return await _context.UserPasswordHistory
                .Where(x => x.UserId == userId).OrderByDescending(x => x.CreatedAt).Take(count).ToListAsync(cancellationToken);             
        }

        public void Delete(UserPasswordHistory oldPassword)
        {
            _context.UserPasswordHistory.Remove(oldPassword);
        }
    }
}
