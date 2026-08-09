using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Orders;
using OrderService.InfraStructure;
using OrderService.InfraStructure.Repositories;
using Xunit;

namespace OrderService.Infrastructure.Tests
{
    public class UnitOfWorkTests
    {
        [Fact]
        public async Task SaveChangesAsync_ShouldPersistChanges()
        {
            // Arrange
            var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync(TestContext.Current.CancellationToken);

            var options = new DbContextOptionsBuilder<OrderDbContext>().UseSqlite(connection).Options;

            await using var context = new OrderDbContext(options);

            await context.Database.EnsureCreatedAsync(TestContext.Current.CancellationToken);

            var order = new Order(Guid.NewGuid());

            context.Orders.Add(order);

            var unitOfWork = new UnitOfWork(context);

            // Act
            await unitOfWork.SaveChangesAsync(TestContext.Current.CancellationToken);

            // Assert
            var savedOrder = await context.Orders.SingleAsync(TestContext.Current.CancellationToken);

            Assert.Equal(order.Id, savedOrder.Id);
        }
    }
}
