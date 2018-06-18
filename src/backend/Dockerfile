FROM microsoft/dotnet:2.0-runtime AS base
WORKDIR /app
EXPOSE 8080 11111 30000 40000 50000

FROM microsoft/dotnet:2.0-sdk AS build
WORKDIR /src
COPY backend/silo.csproj backend/
COPY common/grains/grains.csproj common/grains/
COPY common/interfaces/interfaces.csproj common/interfaces/
RUN dotnet restore backend/silo.csproj
COPY . .
WORKDIR /src/backend
RUN dotnet build silo.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish silo.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "silo.dll"]
