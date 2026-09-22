using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Contracts;
using OrderService.Domain.Enums;
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

            var order = new Order(Guid.NewGuid(), "example@gmail.com");

            context.Orders.Add(order);
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);

            var repository = new OrderRepository(context);

            // Act
            var result = await repository.GetByIdUnTrackedAsync(order.Id, TestContext.Current.CancellationToken);

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
            var result = await repository.GetByIdUnTrackedAsync(Guid.NewGuid(), TestContext.Current.CancellationToken);

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

                var order = new Order(userId, "example@gmail.com");

                order.AddItem(Guid.NewGuid(), 2);
                order.AddItem(Guid.NewGuid(), 3);

                orderId = order.Id;

                context.Orders.Add(order);

                await context.SaveChangesAsync(TestContext.Current.CancellationToken);
            }

            await using var context2 = new OrderDbContext(options);

            var repository = new OrderRepository(context2);

            // Act
            var result = await repository.GetByIdUnTrackedAsync(orderId, TestContext.Current.CancellationToken);

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
            var order = new Order(Guid.NewGuid(), "example@gmail.com");

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
                .UseSqlite(connection).Options;

            await using var context = new OrderDbContext(options);

            await context.Database.EnsureCreatedAsync(TestContext.Current.CancellationToken);

            var order = new Order(Guid.NewGuid(), "example@gmail.com");

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
        public async Task GetPagedByUserIdAsync_ShouldReturnOnlyUserOrders()
        {
            // Arrange
            var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync(TestContext.Current.CancellationToken);

            var options = new DbContextOptionsBuilder<OrderDbContext>().UseSqlite(connection).Options;

            await using var context = new OrderDbContext(options);

            await context.Database.EnsureCreatedAsync(TestContext.Current.CancellationToken);

            var userId = Guid.NewGuid();
            var email = "example@gmail.com";
            var otherUserId = Guid.NewGuid();
            var otherEmail = "otherexample@gmail.com";

            var order1 = new Order(userId, email);
            var order2 = new Order(otherUserId, otherEmail);

            context.Orders.AddRange(order1, order2);

            await context.SaveChangesAsync(TestContext.Current.CancellationToken);

            var repository = new OrderRepository(context);

            // Act
            var result = await repository.GetPagedByUserIdAsync(userId, 1, 10, TestContext.Current.CancellationToken);

            // Assert
            Assert.Single(result);
            Assert.Equal(userId, result[0].UserId);
        }

        [Fact]
        public async Task GetByIdAndUserIdUntrackedAsync_ShouldReturnOrder_WhenOrderBelongsToUser()
        {
            // Arrange
            var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync(TestContext.Current.CancellationToken);

            var options = new DbContextOptionsBuilder<OrderDbContext>().UseSqlite(connection).Options;

            await using var context = new OrderDbContext(options);

            await context.Database.EnsureCreatedAsync(TestContext.Current.CancellationToken);

            var userId = Guid.NewGuid();
            var email = "example@gmail.com";
            var order = new Order(userId,email);

            context.Orders.Add(order);
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);

            context.ChangeTracker.Clear();

            var repository = new OrderRepository(context);

            // Act
            var result = await repository.GetByIdAndUserIdUnTrackedAsync(order.Id, userId, TestContext.Current.CancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(order.Id, result.Id);
            Assert.Equal(userId, result.UserId);
            Assert.Equal(EntityState.Detached, context.Entry(result).State);
        }

        [Fact]
        public async Task GetByIdAndUserIdUntrackedAsync_ShouldReturnNull_WhenOrderBelongsToAnotherUser()
        {
            // Arrange
            var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync(TestContext.Current.CancellationToken);

            var options = new DbContextOptionsBuilder<OrderDbContext>().UseSqlite(connection).Options;

            await using var context = new OrderDbContext(options);

            await context.Database.EnsureCreatedAsync(TestContext.Current.CancellationToken);

            var userId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();
            var email = "example@gmail.com";

            var order = new Order(userId, email);

            context.Orders.Add(order);
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);

            var repository = new OrderRepository(context);

            // Act
            var result = await repository.GetByIdAndUserIdUnTrackedAsync(order.Id, otherUserId, TestContext.Current.CancellationToken);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdAndUserIdUntrackedAsync_ShouldReturnOrderWithItems()
        {
            // Arrange
            var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync(TestContext.Current.CancellationToken);

            var options = new DbContextOptionsBuilder<OrderDbContext>().UseSqlite(connection).Options;

            Guid orderId;
            var userId = Guid.NewGuid();
            var email = "example@gmail.com";

            await using (var context = new OrderDbContext(options))
            {
                await context.Database.EnsureCreatedAsync(TestContext.Current.CancellationToken);

                var order = new Order(userId, email);

                order.AddItem(Guid.NewGuid(), 2);
                order.AddItem(Guid.NewGuid(), 3);

                orderId = order.Id;

                context.Orders.Add(order);

                await context.SaveChangesAsync(TestContext.Current.CancellationToken);
            }

            await using var context2 = new OrderDbContext(options);

            var repository = new OrderRepository(context2);

            // Act
            var result = await repository.GetByIdAndUserIdUnTrackedAsync(orderId, userId, TestContext.Current.CancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(orderId, result.Id);
            Assert.Equal(userId, result.UserId);
            Assert.Equal(2, result.Items.Count);
        }

        [Fact]
        public async Task GetByIdAndUserIdTrackedAsync_ShouldReturnOrder_WhenOrderBelongsToUser()
        {
            // Arrange
            var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync(TestContext.Current.CancellationToken);

            var options = new DbContextOptionsBuilder<OrderDbContext>().UseSqlite(connection).Options;

            await using var context = new OrderDbContext(options);

            await context.Database.EnsureCreatedAsync(TestContext.Current.CancellationToken);

            var userId = Guid.NewGuid();
            var email = "example@gmail.com";
            var order = new Order(userId, email);

            context.Orders.Add(order);
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);

            context.ChangeTracker.Clear();

            var repository = new OrderRepository(context);

            // Act
            var result = await repository.GetByIdAndUserIdTrackedAsync(order.Id, userId, TestContext.Current.CancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(order.Id, result.Id);
            Assert.Equal(userId, result.UserId);
            Assert.True(context.Entry(result).State == EntityState.Unchanged);
        }

        [Fact]
        public async Task GetByIdAndUserIdTrackedAsync_ShouldReturnNull_WhenOrderBelongsToAnotherUser()
        {
            // Arrange
            var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync(TestContext.Current.CancellationToken);

            var options = new DbContextOptionsBuilder<OrderDbContext>().UseSqlite(connection).Options;

            await using var context = new OrderDbContext(options);

            await context.Database.EnsureCreatedAsync(TestContext.Current.CancellationToken);

            var userId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();
            var email = "example@gmail.com";

            var order = new Order(userId, email);

            context.Orders.Add(order);
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);

            var repository = new OrderRepository(context);

            // Act
            var result = await repository.GetByIdAndUserIdTrackedAsync(order.Id, otherUserId, TestContext.Current.CancellationToken);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetLatestByUserIdUntrackedAsync_ShouldReturnLatestOrder()
        {
            // Arrange
            var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync(TestContext.Current.CancellationToken);

            var options = new DbContextOptionsBuilder<OrderDbContext>().UseSqlite(connection).Options;

            await using var context = new OrderDbContext(options);

            await context.Database.EnsureCreatedAsync(TestContext.Current.CancellationToken);

            var userId = Guid.NewGuid();
            var email = "example@gmail.com";

            var olderOrder = new Order(userId,email);
            var latestOrder = new Order(userId,email);

            olderOrder.CreatedAt = DateTime.UtcNow.AddMinutes(-10);
            latestOrder.CreatedAt = DateTime.UtcNow;

            olderOrder.Cancel();

            context.Orders.AddRange(olderOrder, latestOrder);
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);

            context.ChangeTracker.Clear();

            var repository = new OrderRepository(context);

            // Act
            var result = await repository.GetLatestByUserIdUnTrackedAsync(userId, TestContext.Current.CancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(latestOrder.Id, result.Id);
            Assert.Equal(userId, result.UserId);
        }

        [Fact]
        public async Task GetLatestByUserIdUntrackedAsync_ShouldReturnNull_WhenUserHasNoOrders()
        {
            // Arrange
            var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync(TestContext.Current.CancellationToken);

            var options = new DbContextOptionsBuilder<OrderDbContext>().UseSqlite(connection).Options;

            await using var context = new OrderDbContext(options);

            await context.Database.EnsureCreatedAsync(TestContext.Current.CancellationToken);

            var repository = new OrderRepository(context);

            // Act
            var result = await repository.GetLatestByUserIdUnTrackedAsync(Guid.NewGuid(), TestContext.Current.CancellationToken);

            // Assert
            Assert.Null(result);
        }
        [Fact]
        public async Task GetPendingByUserIdTrackedAsync_ShouldReturnPendingOrder_WhenOrderExists()
        {
            // Arrange
            var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync(TestContext.Current.CancellationToken);

            var options = new DbContextOptionsBuilder<OrderDbContext>().UseSqlite(connection).Options;

            await using var context = new OrderDbContext(options);

            await context.Database.EnsureCreatedAsync(TestContext.Current.CancellationToken);

            var userId = Guid.NewGuid();
            var email = "example@gmail.com";
            var order = new Order(userId, email);

            context.Orders.Add(order);
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);

            context.ChangeTracker.Clear();

            var repository = new OrderRepository(context);

            // Act
            var result = await repository.GetPendingByUserIdTrackedAsync(userId, TestContext.Current.CancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(order.Id, result.Id);
            Assert.Equal(userId, result.UserId);
            Assert.Equal(OrderStatus.Pending, result.Status);
            Assert.Equal(EntityState.Unchanged, context.Entry(result).State);
        }


        [Fact]
        public async Task GetPendingByUserIdTrackedAsync_ShouldReturnNull_WhenPendingOrderDoesNotExist()
        {
            // Arrange
            var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync(TestContext.Current.CancellationToken);

            var options = new DbContextOptionsBuilder<OrderDbContext>().UseSqlite(connection).Options;

            await using var context = new OrderDbContext(options);

            await context.Database.EnsureCreatedAsync(TestContext.Current.CancellationToken);

            var repository = new OrderRepository(context);

            // Act
            var result = await repository.GetPendingByUserIdTrackedAsync(Guid.NewGuid(), TestContext.Current.CancellationToken);

            // Assert
            Assert.Null(result);
        }


        [Fact]
        public async Task GetPendingByUserIdTrackedAsync_ShouldReturnNull_WhenUserOnlyHasNonPendingOrders()
        {
            // Arrange
            var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync(TestContext.Current.CancellationToken);

            var options = new DbContextOptionsBuilder<OrderDbContext>().UseSqlite(connection).Options;

            await using var context = new OrderDbContext(options);

            await context.Database.EnsureCreatedAsync(TestContext.Current.CancellationToken);

            var userId = Guid.NewGuid();
            var email = "example@gmail.com";
            var order = new Order(userId, email);

            order.Cancel();

            context.Orders.Add(order);
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);

            var repository = new OrderRepository(context);

            // Act
            var result = await repository.GetPendingByUserIdTrackedAsync(userId,TestContext.Current.CancellationToken);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetConfirmedOrdersPastExpiryDateAsync_ShouldReturnExpiredConfirmedOrder()
        {
            // Arrange
            var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync(TestContext.Current.CancellationToken);

            var options = new DbContextOptionsBuilder<OrderDbContext>()
                .UseSqlite(connection).Options;

            await using var context = new OrderDbContext(options);

            await context.Database.EnsureCreatedAsync(TestContext.Current.CancellationToken);

            var order = new Order(Guid.NewGuid(), "example@gmail.com");
            var productId = Guid.NewGuid();

            order.AddItem(productId, 2);

            order.Confirm(
                new Dictionary<Guid, (decimal UnitPrice, CurrencyCode Currency)>
                {
                    [productId] = (100m, CurrencyCode.USD)
                },
                DateTime.UtcNow.AddDays(-4));

            context.Orders.Add(order);
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);

            var repository = new OrderRepository(context);

            // Act
            var result = await repository.GetConfirmedOrdersPastExpiryDateAsync(
                TestContext.Current.CancellationToken);

            // Assert
            Assert.Single(result);
            Assert.Equal(order.Id, result[0].Id);
        }
    }
}
