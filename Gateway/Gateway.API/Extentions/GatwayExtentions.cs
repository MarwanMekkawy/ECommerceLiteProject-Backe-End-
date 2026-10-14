using Gateway.API.Abstractions;
using Gateway.API.Clients;

namespace Gateway.API.Extentions
{
    public static class GatwayExtentions
    {
        public static IServiceCollection addGatwayServices(this IServiceCollection services, IConfiguration config)
        {
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

            return services;
        }
    }
}