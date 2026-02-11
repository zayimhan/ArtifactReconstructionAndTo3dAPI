FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["tests/ArtifactReconstruction.Tests/ArtifactReconstruction.Tests.csproj", "tests/ArtifactReconstruction.Tests/"]
RUN dotnet restore "tests/ArtifactReconstruction.Tests/ArtifactReconstruction.Tests.csproj"
COPY . .
WORKDIR "/src/tests/ArtifactReconstruction.Tests"
RUN dotnet build "./ArtifactReconstruction.Tests.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ArtifactReconstruction.Tests.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ArtifactReconstruction.Tests.dll"]
