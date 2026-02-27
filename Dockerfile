# Build stage - ¡NECESITA UNA IMAGEN BASE!
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copiar archivos de proyecto y restaurar dependencias
COPY *.csproj .
RUN dotnet restore

# Copiar el resto del código y publicar
COPY . .
RUN dotnet publish -c Release -o publicacion

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Crear carpetas necesarias
RUN mkdir -p /app/wwwroot/Files/Images

# Copiar la aplicación publicada desde la etapa 'build'
COPY --from=build /app/publicacion .
COPY wwwroot ./wwwroot

# Exponer el puerto
EXPOSE 5081

# Configurar entorno para producción
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://0.0.0.0:5081

ENTRYPOINT ["dotnet", "api_netcore.dll"]