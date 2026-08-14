FROM node:24-alpine AS web-build
WORKDIR /source/apps/web
COPY apps/web/package.json apps/web/package-lock.json ./
RUN npm ci --no-audit --no-fund
COPY apps/web/ ./
RUN npm run build

FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS api-build
WORKDIR /source
COPY TryOnReady.sln Directory.Build.props global.json ./
COPY src/TryOnReady.Domain/TryOnReady.Domain.csproj src/TryOnReady.Domain/
COPY src/TryOnReady.Application/TryOnReady.Application.csproj src/TryOnReady.Application/
COPY src/TryOnReady.Infrastructure/TryOnReady.Infrastructure.csproj src/TryOnReady.Infrastructure/
COPY src/TryOnReady.YouCam/TryOnReady.YouCam.csproj src/TryOnReady.YouCam/
COPY src/TryOnReady.Api/TryOnReady.Api.csproj src/TryOnReady.Api/
RUN dotnet restore src/TryOnReady.Api/TryOnReady.Api.csproj
COPY src/ src/
RUN dotnet publish src/TryOnReady.Api/TryOnReady.Api.csproj \
    --configuration Release \
    --no-restore \
    --output /app/publish \
    -p:SkipClientBuild=true \
    -p:UseAppHost=false
COPY --from=web-build /source/apps/web/out/ /app/publish/wwwroot/

FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS final
RUN apk add --no-cache icu-libs tzdata
ENV ASPNETCORE_URLS=http://0.0.0.0:8080 \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    Persistence__Provider=Postgres \
    PrivateStorage__RootPath=/data/private
WORKDIR /app
COPY --from=api-build /app/publish ./
COPY samples/synthetic/replay/controlled-youcam-ai-clothes-v3-marisol-moonlight.jpg /app/replay/controlled-youcam-ai-clothes-v3-marisol-moonlight.jpg
COPY deployment/entrypoint.sh /app/entrypoint.sh
RUN mkdir -p /data/private \
    && sed -i 's/\r$//' /app/entrypoint.sh \
    && chmod 0755 /app/entrypoint.sh \
    && chown -R app:app /app /data
USER app
EXPOSE 8080
ENTRYPOINT ["/app/entrypoint.sh"]
CMD ["serve"]
