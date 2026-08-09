using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Contracts;
using OrderService.Domain.Orders;
using OrderService.InfraStructure;
using Xunit;

namespace OrderService.Infrastructure.Tests
{
    public class OrderRepositoryTests
    {
        [Fact]
        public async Task GetByIdAsync_ShouldReturnOrder_WhenOrderExists()
        {
            // Arrange
            var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync(TestContext.Current.CancellationToken);

            var options = new DbContextOptionsBuilder<OrderDbContext>().UseSqlite(connection).Options;

            await using var context = new OrderDbContext(options);

            await context.Database.EnsureCreatedAsync(TestContext.Current.CancellationToken);

            var order = new Order(Guid.NewGuid());

            context.Orders.Add(order);
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);

            var repository = new OrderRepository(context);

            // Act
            var result = await repository.GetByIdAsync(order.Id, TestContext.Current.CancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(order.Id, result.Id);
        }
        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenOrderDoesNotExist()
        {
            // Arrange
            var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync(TestContext.Current.CancellationToken);

            var options = new DbContextOptionsBuilder<OrderDbContext>().UseSqlite(connection).Options;

            await using var context = new OrderDbContext(options);

            await context.Database.EnsureCreatedAsync(TestContext.Current.CancellationToken);

            var repository = new OrderRepository(context);

            // Act
            var result = await repository.GetByIdAsync(Guid.NewGuid(), TestContext.Current.CancellationToken);

            // Assert
            Assert.Null(result);
        }
        [Fact]
        public async Task GetByIdAsync_ShouldReturnOrderWithItems()
        {
            // Arrange
            var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync(TestContext.Current.CancellationToken);

            var userId = Guid.NewGuid();

            var options = new DbContextOptionsBuilder<OrderDbContext>()
                .UseSqlite(connection)
                .Options;

            Guid orderId;

            await using (var context = new OrderDbContext(options))
            {
                await context.Database.EnsureCreatedAsync(TestContext.Current.CancellationToken);

                var order = new Order(userId);

                order.AddItem(Guid.NewGuid(), 2);
                order.AddItem(Guid.NewGuid(), 3);

                orderId = order.Id;

                context.Orders.Add(order);

                await context.SaveChangesAsync(TestContext.Current.CancellationToken);
            }

            await using var context2 = new OrderDbContext(options);

            var repository = new OrderRepository(context2);

            // Act
            var result = await repository.GetByIdAsync(
                orderId,
                TestContext.Current.CancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(orderId, result.Id);
            Assert.Equal(2, result.Items.Count);
        }
        [Fact]
        public async Task AddAsync_ShouldAddOrder()
        {
            // Arrange
            var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync(TestContext.Current.CancellationToken);

            var options = new DbContextOptionsBuilder<OrderDbContext>()
                .UseSqlite(connection)
                .Options;

            await using var context = new OrderDbContext(options);

            await context.Database.EnsureCreatedAsync(TestContext.Current.CancellationToken);

            var repository = new OrderRepository(context);
            var order = new Order(Guid.NewGuid());

            // Act
            await repository.AddAsync(order, TestContext.Current.CancellationToken);

            // Assert
            var entry = context.Entry(order);

            Assert.Equal(EntityState.Added, entry.State);
        }
        [Fact]
        public async Task GetByIdTrackedAsync_ShouldReturnOrder_WhenOrderExists()
        {
            // Arrange
            var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync(TestContext.Current.CancellationToken);

            var options = new DbContextOptionsBuilder<OrderDbContext>()
                .UseSqlite(connection)
                .Options;

            await using var context = new OrderDbContext(options);

            await context.Database.EnsureCreatedAsync(TestContext.Current.CancellationToken);

            var order = new Order(Guid.NewGuid());

            context.Orders.Add(order);
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);

            context.ChangeTracker.Clear();

            var repository = new OrderRepository(context);

            // Act
            var result = await repository.GetByIdTrackedAsync(order.Id, TestContext.Current.CancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(order.Id, result.Id);
            Assert.True(context.Entry(result).State == EntityState.Unchanged);
        }

        [Fact]
        public async Task GetByIdTrackedAsync_ShouldReturnNull_WhenOrderDoesNotExist()
        {
            // Arrange
            var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync(TestContext.Current.CancellationToken);

            var options = new DbContextOptionsBuilder<OrderDbContext>().UseSqlite(connection).Options;

            await using var context = new OrderDbContext(options);

            await context.Database.EnsureCreatedAsync(TestContext.Current.CancellationToken);

            var repository = new OrderRepository(context);

            // Act
            var result = await repository.GetByIdTrackedAsync(Guid.NewGuid(), TestContext.Current.CancellationToken);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByUserIdAsync_ShouldReturnUserOrders()
        {
            // Arrange
            var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync(TestContext.Current.CancellationToken);

            var options = new DbContextOptionsBuilder<OrderDbContext>().UseSqlite(connection).Options;

            await using var context = new OrderDbContext(options);

            await context.Database.EnsureCreatedAsync(TestContext.Current.CancellationToken);

            var userId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();

            var order1 = new Order(userId);
            var order2 = new Order(userId);
            var otherOrder = new Order(otherUserId);

            context.Orders.AddRange(order1, order2, otherOrder);

            await context.SaveChangesAsync(TestContext.Current.CancellationToken);

            var repository = new OrderRepository(context);

            // Act
            var result = await repository.GetByUserIdAsync(userId, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.All(result, order => Assert.Equal(userId, order.UserId));
        }

        [Fact]
        public async Task GetByUserIdAsync_ShouldReturnEmpty_WhenUserHasNoOrders()
        {
            // Arrange
            var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync(TestContext.Current.CancellationToken);

            var options = new DbContextOptionsBuilder<OrderDbContext>().UseSqlite(connection).Options;

            await using var context = new OrderDbContext(options);

            await context.Database.EnsureCreatedAsync(TestContext.Current.CancellationToken);

            var repository = new OrderRepository(context);

            // Act
            var result = await repository.GetByUserIdAsync(Guid.NewGuid(), TestContext.Current.CancellationToken);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetPagedByUserIdAsync_ShouldReturnRequestedPage()
        {
            // Arrange
            var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync(TestContext.Current.CancellationToken);

            var options = new DbContextOptionsBuilder<OrderDbContext>().UseSqlite(connection).Options;

            await using var context = new OrderDbContext(options);

            await context.Database.EnsureCreatedAsync(TestContext.Current.CancellationToken);

            var userId = Guid.NewGuid();

            var order1 = new Order(userId);
            var order2 = new Order(userId);
            var order3 = new Order(userId);
            var order4 = new Order(userId);
            var order5 = new Order(userId);

            context.Orders.AddRange(order1, order2, order3, order4, order5);

            await context.SaveChangesAsync(TestContext.Current.CancellationToken);

            var repository = new OrderRepository(context);

            // Act
            var result = await repository.GetPagedByUserIdAsync(userId, 2, 2, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetPagedByUserIdAsync_ShouldReturnOnlyUserOrders()
        {
            // Arrange
            var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync(TestContext.Current.CancellationToken);

            var options = new DbContextOptionsBuilder<OrderDbContext>().UseSqlite(connection).Options;

            await using var context = new OrderDbContext(options);

            await context.Database.EnsureCreatedAsync(TestContext.Current.CancellationToken);

            var userId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();

            var order1 = new Order(userId);
            var order2 = new Order(otherUserId);

            context.Orders.AddRange(order1, order2);

            await context.SaveChangesAsync(TestContext.Current.CancellationToken);

            var repository = new OrderRepository(context);

            // Act
            var result = await repository.GetPagedByUserIdAsync(userId, 1, 10, TestContext.Current.CancellationToken);

            // Assert
            Assert.Single(result);
            Assert.Equal(userId, result[0].UserId);
        }
    }
}
