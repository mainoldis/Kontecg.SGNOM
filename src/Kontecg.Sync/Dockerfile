FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
EXPOSE 9000

FROM base AS debug
ENV MS_VS_CONTAINERS_TOOLS_LOGGING_ENABLED=True

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Build.props", "."]
COPY ["NuGet.Config", "."]
COPY ["src/Kontecg.Sync/Kontecg.Sync.csproj", "src/Kontecg.Sync/"]
RUN dotnet restore "./src/Kontecg.Sync/./Kontecg.Sync.csproj"
COPY . .
WORKDIR "/src/src/Kontecg.Sync"
RUN dotnet build "./Kontecg.Sync.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Kontecg.Sync.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Kontecg.Sync.dll"]