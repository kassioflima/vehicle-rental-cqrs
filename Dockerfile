FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/vehicle-rental.api/vehicle-rental.api.csproj", "src/vehicle-rental.api/"]
COPY ["src/vehicle-rental.application/vehicle-rental.application.csproj", "src/vehicle-rental.application/"]
COPY ["src/vehicle-rental.domain/vehicle-rental.domain.csproj", "src/vehicle-rental.domain/"]
COPY ["src/vehicle-rental.data.postgresql/vehicle-rental.data.postgresql.csproj", "src/vehicle-rental.data.postgresql/"]
RUN dotnet restore "src/vehicle-rental.api/vehicle-rental.api.csproj"
COPY . .
WORKDIR "/src/src/vehicle-rental.api"
RUN dotnet build "vehicle-rental.api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "vehicle-rental.api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "vehicle-rental.api.dll"]
