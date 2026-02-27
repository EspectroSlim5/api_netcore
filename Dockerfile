# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Crear carpeta para wwwroot
RUN mkdir -p /app/wwwroot/Files/Images

# Copiar la aplicación publicada
COPY --from=build /app/publicacion .
COPY wwwroot ./wwwroot

# Exponer puerto
EXPOSE 5081

# Configurar variables de entorno
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://0.0.0.0:5081

ENTRYPOINT ["dotnet", "api_netcore.dll"]