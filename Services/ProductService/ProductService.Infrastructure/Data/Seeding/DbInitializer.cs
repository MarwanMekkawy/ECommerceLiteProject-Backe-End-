using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Entities;
using ProductService.Domain.Enums;
using ProductService.Domain.Value_Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProductService.Infrastructure.Data.Seeding
{
    public static class DbInitializer
    {
        //{Accessories: 16 ,Computers: 15 ,Networking: 14 ,Gaming: 13 ,Monitors: 13 ,Electronics: 11 ,Storage: 11 ,Peripherals: 7}
        public static async Task InitializeAsync(ProductDbContext context, string contentRootPath)
        {
            await context.Database.MigrateAsync();
            var seedingPath = Path.Combine(AppContext.BaseDirectory, "Data", "Seeding");
            var categoriesPath = Path.Combine(seedingPath, "categories.json");
            var productsPath = Path.Combine(seedingPath, "products.json");
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            if (!await context.Categories.AnyAsync())
            {
                var categoriesJson = await File.ReadAllTextAsync(categoriesPath);

                var categoryData = JsonSerializer.Deserialize<List<CategorySeedData>>(categoriesJson, jsonOptions);

                if (categoryData is null || categoryData.Count == 0) return;

                var categories = categoryData.Select(c => new Category(c.Name, c.Description)).ToList();

                await context.Categories.AddRangeAsync(categories);
                await context.SaveChangesAsync();
            }

            if (!await context.Products.AnyAsync())
            {
                var productData = JsonSerializer.Deserialize<List<ProductSeedData>>(
                    await File.ReadAllTextAsync(productsPath), jsonOptions);

                if (productData is null || productData.Count == 0) return;

                var categories = await context.Categories.ToListAsync();

                var categoryIds = categories.ToDictionary(c => c.Name, c => c.Id);

                var products = productData.Select(p =>
                    new Product(p.Name, p.Description,
                    new Money(p.Amount, Enum.Parse<CurrencyCode>(p.Currency)), p.StockQuantity, categoryIds[p.CategoryName]))
                    .ToList();

                await context.Products.AddRangeAsync(products);
                await context.SaveChangesAsync();
            }
        }

        private class CategorySeedData
        {
            public string Name { get; set; } = null!;
            public string? Description { get; set; }
        }

        private class ProductSeedData
        {
            public string Name { get; set; } = null!;
            public string? Description { get; set; }
            public decimal Amount { get; set; }
            public string Currency { get; set; } = null!;
            public int StockQuantity { get; set; }
            public string CategoryName { get; set; } = null!;
        }
    }
}
