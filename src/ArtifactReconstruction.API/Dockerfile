FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/ArtifactReconstruction.API/ArtifactReconstruction.API.csproj", "src/ArtifactReconstruction.API/"]
COPY ["src/ArtifactReconstruction.Application/ArtifactReconstruction.Application.csproj", "src/ArtifactReconstruction.Application/"]
COPY ["src/ArtifactReconstruction.Domain/ArtifactReconstruction.Domain.csproj", "src/ArtifactReconstruction.Domain/"]
COPY ["src/ArtifactReconstruction.Infrastructure/ArtifactReconstruction.Infrastructure.csproj", "src/ArtifactReconstruction.Infrastructure/"]
RUN dotnet restore "src/ArtifactReconstruction.API/ArtifactReconstruction.API.csproj"
COPY . .
WORKDIR "/src/src/ArtifactReconstruction.API"
RUN dotnet build "./ArtifactReconstruction.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ArtifactReconstruction.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ArtifactReconstruction.API.dll"]
