# Build stage. Project files are copied first so restore is cached independently of source
# changes: editing a .cs file does not invalidate the restore layer.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Directory.Build.props Directory.Packages.props global.json ./
COPY src/BuildingBlocks/ src/BuildingBlocks/
COPY src/Modules/ src/Modules/
COPY src/Api/ src/Api/

RUN dotnet restore src/Api/ConfiOS.Api/ConfiOS.Api.csproj
RUN dotnet publish src/Api/ConfiOS.Api/ConfiOS.Api.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Runs unprivileged. Nothing in the container needs to write outside /tmp.
USER $APP_UID

COPY --from=build /app .

ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "ConfiOS.Api.dll"]
