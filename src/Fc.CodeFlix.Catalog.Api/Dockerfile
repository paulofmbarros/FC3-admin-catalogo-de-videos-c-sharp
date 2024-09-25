FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Fc.CodeFlix.Catalog.Api/Fc.CodeFlix.Catalog.Api.csproj", "src/Fc.CodeFlix.Catalog.Api/"]
RUN dotnet restore "src/Fc.CodeFlix.Catalog.Api/Fc.CodeFlix.Catalog.Api.csproj"
COPY . .
WORKDIR "/src/src/Fc.CodeFlix.Catalog.Api"
RUN dotnet build "Fc.CodeFlix.Catalog.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Fc.CodeFlix.Catalog.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Fc.CodeFlix.Catalog.Api.dll"]
