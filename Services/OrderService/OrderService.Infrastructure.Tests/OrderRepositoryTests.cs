using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Orders;
using OrderService.InfraStructure;
using OrderService.InfraStructure.Repositories;
using Xunit;

namespace OrderService.Infrastructure.Tests
{
    public class OrderRepositoryTests
    {
        [Fact]
        public async Task AddAsync_ShouldAddOrder()
        {
            // Arrange
            var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync();

            var options = new DbContextOptionsBuilder<OrderDbContext>()
                .UseSqlite(connection)
                .Options;

            await using var context = new OrderDbContext(options);

            await context.Database.EnsureCreatedAsync();

            var repository = new OrderRepository(context);
            var order = new Order(Guid.NewGuid());

            // Act
            await repository.AddAsync(order);

            // Assert
            var entry = context.Entry(order);

            Assert.Equal(EntityState.Added, entry.State);
        }
    }
}
