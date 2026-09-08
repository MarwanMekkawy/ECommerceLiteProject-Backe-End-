using Microsoft.EntityFrameworkCore;
using NotificationService.Domain.Entities;

namespace NotificationService.Infrastructure
{
    public class NotificationServiceDbContext : DbContext
    {
        public NotificationServiceDbContext(DbContextOptions<NotificationServiceDbContext> options) : base(options) { }
        
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<ProcessedMailjetEvent> ProcessedMailjetEvents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(NotificationServiceDbContext).Assembly);
        }
    }
}
