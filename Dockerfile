# Usar la imagen oficial de .NET SDK para build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copiar archivo de proyecto
COPY *.csproj .
RUN dotnet restore

# Copiar todo el código y compilar
COPY . .
RUN dotnet publish -c Release -o publicacion

# Imagen de runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Crear carpetas necesarias
RUN mkdir -p /https
RUN mkdir -p /app/wwwroot/Files/Images

# ✅ CORREGIDO: Copiar certificado a /https
COPY https/aspnetcore.pfx /https/aspnetcore.pfx

# Copiar archivos de la publicación
COPY --from=build /app/publicacion .

# Copiar archivos estáticos
COPY wwwroot ./wwwroot

# Exponer puertos
EXPOSE 5081

# Configurar variables de entorno
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=https://0.0.0.0:5081

ENTRYPOINT ["dotnet", "api_netcore.dll"]