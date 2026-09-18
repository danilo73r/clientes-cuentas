# Etapa 1: release
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source

COPY global.json ./
COPY src/Cuentas/ ./src/Cuentas/
COPY src/Shared/ ./src/Shared/

RUN dotnet publish src/Cuentas/Cuentas.Api/Cuentas.Api.csproj \
    --configuration Release \
    --output /app/publish \
    /p:UseAppHost=false


# Etapa 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

COPY --from=build /app/publish .

# principio de menor privilegio
USER $APP_UID

ENTRYPOINT ["dotnet", "Cuentas.Api.dll"]