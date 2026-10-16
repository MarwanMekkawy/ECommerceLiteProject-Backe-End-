using Gateway.API.Abstractions;
using Gateway.API.Clients;
using Gateway.API.GraphQL.Mutations;
using Gateway.API.GraphQL.Queries;

namespace Gateway.API.Extentions
{
    public static class GatwayExtentions
    {
        public static IServiceCollection AddGatwayServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddHttpContextAccessor();

            services.AddHttpClient<IIdentityServiceClient, IdentityServiceClient>(client => { client.BaseAddress = new Uri(config["HttpClients:IdentityService:BaseUrl"] 
                ?? throw new InvalidOperationException("IdentityService URL is not configured.")); })
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { UseCookies = false, AllowAutoRedirect = false });
            services.AddHttpClient<IOrderServiceClient, OrderServiceClient>(client => { client.BaseAddress = new Uri(config["HttpClients:OrderService:BaseUrl"] 
                ?? throw new InvalidOperationException("OrderService URL is not configured.")); })
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { UseCookies = false, AllowAutoRedirect = false });
            services.AddHttpClient<IPaymentServiceClient, PaymentServiceClient>(client => { client.BaseAddress = new Uri(config["HttpClients:PaymentService:BaseUrl"] 
                ?? throw new InvalidOperationException("PaymentService URL is not configured.")); })
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { UseCookies = false, AllowAutoRedirect = false });
            services.AddHttpClient<IProductServiceClient, ProductServiceClient>(client => { client.BaseAddress = new Uri(config["HttpClients:ProductService:BaseUrl"] 
                ?? throw new InvalidOperationException("ProductService URL is not configured.")); })
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { UseCookies = false, AllowAutoRedirect = false });

            services.AddGraphQLServer().AddQueryType<Query>().AddMutationType<Mutation>()
                .AddTypeExtension<ProductQueries>().AddTypeExtension<ProductMutations>()
                .AddTypeExtension<OrderQueries>().AddTypeExtension<OrderMutations>()
                .AddTypeExtension<IdentityQueries>().AddTypeExtension<IdentityMutations>()
                .AddTypeExtension<PaymentMutations>();

            return services;
        }
    }
}