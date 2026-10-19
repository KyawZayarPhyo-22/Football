# ==========================================
# Multi-stage Dockerfile for Football Prediction
# Hosts ASP.NET Core API + Blazor WASM together
# Compatible with Render, Koyeb, Fly.io, Railway, VPS
# ==========================================

# 1. Build Stage (using .NET 8 SDK)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project definitions for layer caching
COPY FootballPrediction.slnx ./
COPY src/FootballPrediction.Shared/*.csproj ./src/FootballPrediction.Shared/
COPY src/FootballPrediction.Api/*.csproj ./src/FootballPrediction.Api/
COPY src/FootballPrediction.Client/*.csproj ./src/FootballPrediction.Client/

# Restore dependencies
RUN dotnet restore FootballPrediction.slnx

# Copy all remaining source code
COPY src/ ./src/

# Publish Blazor WASM Client (Frontend)
WORKDIR /src/src/FootballPrediction.Client
RUN dotnet publish -c Release -o /app/client_out

# Publish ASP.NET Core API (Backend)
WORKDIR /src/src/FootballPrediction.Api
RUN dotnet publish -c Release -o /app/api_out

# Copy Blazor WASM static files into API's wwwroot folder
RUN mkdir -p /app/api_out/wwwroot && cp -r /app/client_out/wwwroot/* /app/api_out/wwwroot/

# 2. Runtime Stage (Lightweight ASP.NET 8 Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy published application containing both API & Client
COPY --from=build /app/api_out .

# Render, Koyeb, and Cloud Run provide dynamic $PORT
ENV PORT=8080
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "FootballPrediction.Api.dll"]
