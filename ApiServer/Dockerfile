# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Use the official .NET 8 SDK image as build environment
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY ["ApiServer.csproj", "./"]
RUN dotnet restore "ApiServer.csproj"

# Copy everything else and build
COPY . ./
# Clear any local NuGet cache and restore again to avoid path issues
RUN dotnet clean "ApiServer.csproj"
RUN dotnet restore "ApiServer.csproj" --force
RUN dotnet publish "ApiServer.csproj" -c Release -o /app/out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/out ./
ENTRYPOINT ["dotnet", "ApiServer.dll"]