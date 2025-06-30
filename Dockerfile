# Use the official .NET 9 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Set working directory
WORKDIR /src

# Copy csproj file and restore dependencies (for better caching)
COPY ["devlife-backend.csproj", "./"]
RUN dotnet restore

# Copy everything else and build
COPY . .
RUN dotnet build -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Use the .NET 9 runtime image for the final stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final

# Create app user for security
RUN addgroup --group devlife --gid 2000 \
    && adduser \
    --uid 1000 \
    --gid 2000 \
    --disabled-password \
    --gecos "" \
    "devlife" \
    && mkdir -p /app \
    && chown devlife:devlife /app

# Set working directory
WORKDIR /app

# Expose ports
EXPOSE 8080

# Switch to non-root user
USER devlife:devlife

# Copy the published app from build stage
COPY --from=publish --chown=devlife:devlife /app/publish .

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=30s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Set entry point
ENTRYPOINT ["dotnet", "devlife-backend.dll"]