FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

COPY ["devlife-backend.csproj", "./"]
RUN dotnet restore

COPY . .
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final

RUN addgroup --group devlife --gid 2000 \
    && adduser \
    --uid 1000 \
    --gid 2000 \
    --disabled-password \
    --gecos "" \
    "devlife" \
    && mkdir -p /app \
    && chown devlife:devlife /app

WORKDIR /app

EXPOSE 8080

USER devlife:devlife

COPY --from=publish --chown=devlife:devlife /app/publish .

HEALTHCHECK --interval=30s --timeout=10s --start-period=30s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "devlife-backend.dll"]