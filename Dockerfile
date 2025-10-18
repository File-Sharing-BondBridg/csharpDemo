# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore first for better layer caching
COPY ["EncryptionService/EncryptionService.csproj", "EncryptionService/"]
RUN dotnet restore "EncryptionService/EncryptionService.csproj"

# Copy everything and publish
COPY . .
WORKDIR /src/EncryptionService
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Expose port and configure ASP.NET Core
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

ENTRYPOINT ["dotnet", "EncryptionService.dll"]