using IdentityService.Domain.Contracts;
using IdentityService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityService.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IdentityDbContext _context;
        public IUserRepository users { get; }
        public IRefreshTokenRepository refreshTokens { get; }
        public IEmailVerificationTokenRepository emailVerificationTokens { get; }
        public IEmailChangeTokenRepository emailChangeTokens { get; }
        public IPasswordResetTokenRepository passwordResetTokens { get; }

        public UnitOfWork(IdentityDbContext context, IUserRepository userRepo, IRefreshTokenRepository tokenRepository, 
                          IEmailVerificationTokenRepository emailVerificationTokenRepository, IEmailChangeTokenRepository emailChangeTokenRepository, 
                          IPasswordResetTokenRepository passwordResetTokenRepository)
        {
            _context = context;
            users = userRepo;
            refreshTokens = tokenRepository;
            emailVerificationTokens = emailVerificationTokenRepository;
            emailChangeTokens = emailChangeTokenRepository;
            passwordResetTokens = passwordResetTokenRepository;
        }

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
