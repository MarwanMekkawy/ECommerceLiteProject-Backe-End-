FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# ============================================================
# Identity
# ============================================================

COPY ["Services/IdentityService/IdentityService.API/IdentityService.API.csproj", "Services/IdentityService/IdentityService.API/"]
COPY ["Services/IdentityService/IdentityService.Application/IdentityService.Application.csproj", "Services/IdentityService/IdentityService.Application/"]
COPY ["Services/IdentityService/IdentityService.Infrastructure/IdentityService.Infrastructure.csproj", "Services/IdentityService/IdentityService.Infrastructure/"]
COPY ["Services/IdentityService/IdentityService.Domain/IdentityService.Domain.csproj", "Services/IdentityService/IdentityService.Domain/"]

# ============================================================
# Product
# ============================================================

COPY ["Services/ProductService/ProductService.API/ProductService.API.csproj", "Services/ProductService/ProductService.API/"]
COPY ["Services/ProductService/ProductService.Application/ProductService.Application.csproj", "Services/ProductService/ProductService.Application/"]
COPY ["Services/ProductService/ProductService.Infrastructure/ProductService.Infrastructure.csproj", "Services/ProductService/ProductService.Infrastructure/"]
COPY ["Services/ProductService/ProductService.Domain/ProductService.Domain.csproj", "Services/ProductService/ProductService.Domain/"]

# ============================================================
# Order
# ============================================================

COPY ["Services/OrderService/OrderService.API/OrderService.API.csproj", "Services/OrderService/OrderService.API/"]
COPY ["Services/OrderService/OrderService.Application/OrderService.Application.csproj", "Services/OrderService/OrderService.Application/"]
COPY ["Services/OrderService/OrderService.InfraStructure/OrderService.Infrastructure.csproj", "Services/OrderService/OrderService.InfraStructure/"]
COPY ["Services/OrderService/OrderService.Domain/OrderService.Domain.csproj", "Services/OrderService/OrderService.Domain/"]

# ============================================================
# Payment
# ============================================================

COPY ["Services/PaymentService/PaymentService.API/PaymentService.API.csproj", "Services/PaymentService/PaymentService.API/"]
COPY ["Services/PaymentService/PaymentService.Application/PaymentService.Application.csproj", "Services/PaymentService/PaymentService.Application/"]
COPY ["Services/PaymentService/PaymentService.Infrastructure/PaymentService.Infrastructure.csproj", "Services/PaymentService/PaymentService.Infrastructure/"]
COPY ["Services/PaymentService/PaymentService.Domain/PaymentService.Domain.csproj", "Services/PaymentService/PaymentService.Domain/"]

# ============================================================
# Notification
# ============================================================

COPY ["Services/NotificationService/NotificationService.API/NotificationService.API.csproj", "Services/NotificationService/NotificationService.API/"]
COPY ["Services/NotificationService/NotificationService.Application/NotificationService.Application.csproj", "Services/NotificationService/NotificationService.Application/"]
COPY ["Services/NotificationService/NotificationService.Infrastructure/NotificationService.Infrastructure.csproj", "Services/NotificationService/NotificationService.Infrastructure/"]
COPY ["Services/NotificationService/NotificationService.Domain/NotificationService.Domain.csproj", "Services/NotificationService/NotificationService.Domain/"]

# ============================================================
# Restore
# ============================================================

RUN dotnet restore "Services/IdentityService/IdentityService.API/IdentityService.API.csproj"
RUN dotnet restore "Services/ProductService/ProductService.API/ProductService.API.csproj"
RUN dotnet restore "Services/OrderService/OrderService.API/OrderService.API.csproj"
RUN dotnet restore "Services/PaymentService/PaymentService.API/PaymentService.API.csproj"
RUN dotnet restore "Services/NotificationService/NotificationService.API/NotificationService.API.csproj"

# ============================================================
# Copy source code
# ============================================================

COPY Services/ ./Services/

# ============================================================
# Publish
# ============================================================

RUN dotnet publish "Services/IdentityService/IdentityService.API/IdentityService.API.csproj" -c Release -o /app/Identitypublish --no-restore
RUN dotnet publish "Services/ProductService/ProductService.API/ProductService.API.csproj" -c Release -o /app/Productpublish --no-restore
RUN dotnet publish "Services/OrderService/OrderService.API/OrderService.API.csproj" -c Release -o /app/Orderpublish --no-restore
RUN dotnet publish "Services/PaymentService/PaymentService.API/PaymentService.API.csproj" -c Release -o /app/Paymentpublish --no-restore
RUN dotnet publish "Services/NotificationService/NotificationService.API/NotificationService.API.csproj" -c Release -o /app/Notificationpublish --no-restore

# ============================================================
# stage 2 [identity]
# ============================================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS identity
WORKDIR /appImage
COPY --from=build /app/Identitypublish .
EXPOSE 7050
ENTRYPOINT [ "dotnet", "IdentityService.API.dll"]

# ============================================================
# stage 3 [Product]
# ============================================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS product
WORKDIR /appImage
COPY --from=build /app/Productpublish .
EXPOSE 7060
ENTRYPOINT [ "dotnet", "ProductService.API.dll"]

# ============================================================
# stage 4 [Order]
# ============================================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS order
WORKDIR /appImage
COPY --from=build /app/Orderpublish .
EXPOSE 7070
ENTRYPOINT [ "dotnet", "OrderService.API.dll"]

# ============================================================
# stage 5 [Payment]
# ============================================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS payment
WORKDIR /appImage
COPY --from=build /app/Paymentpublish .
EXPOSE 7080
ENTRYPOINT [ "dotnet", "PaymentService.API.dll"]

# ============================================================
# stage 6 [Notification]
# ============================================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS notification
WORKDIR /appImage
COPY --from=build /app/Notificationpublish .
EXPOSE 7090
ENTRYPOINT [ "dotnet", "NotificationService.API.dll"]