using Microsoft.Extensions.DependencyInjection;
using ProductService.Application.Abstractions;
using ProductService.Application.Commands.Categories;
using ProductService.Application.Commands.Products;
using ProductService.Application.DTOs;
using ProductService.Application.Queries.Categories;
using ProductService.Application.Queries.Products;

namespace ProductService.Application.Extentions.App
{
    public static class ApplicationServicesCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Product Commands
            services.AddScoped<ICommandHandler<ActivateProductCommand>, ActivateProductCommandHandler>();
            services.AddScoped<ICommandHandler<CreateProductCommand, Guid>, CreateProductCommandHandler>();
            services.AddScoped<ICommandHandler<DeactivateProductCommand>, DeactivateProductCommandHandler>();
            services.AddScoped<ICommandHandler<DecreaseStockCommand, int>, DecreaseStockCommandHandler>();
            services.AddScoped<ICommandHandler<DeleteProductCommand>, DeleteProductCommandHandler>();
            services.AddScoped<ICommandHandler<IncreaseStockCommand, int>, IncreaseStockCommandHandler>();
            services.AddScoped<ICommandHandler<UpdateProductCommand>, UpdateProductCommandHandler>();

            // Category Commands
            services.AddScoped<ICommandHandler<CreateCategoryCommand, Guid>, CreateCategoryCommandHandler>();
            services.AddScoped<ICommandHandler<DeleteCategoryCommand>, DeleteCategoryCommandHandler>();
            services.AddScoped<ICommandHandler<UpdateCategoryCommand>, UpdateCategoryCommandHandler>();

            // Product Queries
            services.AddScoped<IQueryHandler<GetProductByIdQuery, ProductDto?>, GetProductByIdQueryHandler>();
            services.AddScoped<IQueryHandler<GetProductByNameQuery, ProductDto?>, GetProductByNameQueryHandler>();
            services.AddScoped<IQueryHandler<GetProductsQuery, IReadOnlyList<ProductDto>>, GetProductsQueryHandler>();
            services.AddScoped<IQueryHandler<SearchProductsByNameQuery, IReadOnlyList<ProductDto>>, SearchProductsByNameQueryHandler>();

            // Category Queries
            services.AddScoped<IQueryHandler<GetCategoriesQuery, IReadOnlyList<CategoryDto>>, GetCategoriesQueryHandler>();
            services.AddScoped<IQueryHandler<GetCategoryByIdQuery, CategoryDto?>, GetCategoryByIdQueryHandler>();
            services.AddScoped<IQueryHandler<GetCategoryByNameQuery, CategoryDto?>, GetCategoryByNameQueryHandler>();
            services.AddScoped<IQueryHandler<SearchCategoriesByNameQuery, IReadOnlyList<CategoryDto>>, SearchCategoriesByNameQueryHandler>();

            return services;
        }
    }
}
