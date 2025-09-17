# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/vehicle-rental.api/vehicle-rental.api.csproj", "src/vehicle-rental.api/"]
COPY ["src/vehicle-rental.domain/vehicle-rental.domain.csproj", "src/vehicle-rental.domain/"]
COPY ["src/vehicle-rental.data.postgresql/vehicle-rental.data.postgresql.csproj", "src/vehicle-rental.data.postgresql/"]
COPY ["src/vehicle-rental.application/vehicle-rental.application.csproj", "src/vehicle-rental.application/"]
RUN dotnet restore "./src/vehicle-rental.api/vehicle-rental.api.csproj"
COPY . .
WORKDIR "/src/src/vehicle-rental.api"
RUN dotnet build "./vehicle-rental.api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./vehicle-rental.api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "vehicle-rental.api.dll"]