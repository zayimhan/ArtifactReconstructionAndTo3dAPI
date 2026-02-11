# ---- Build Stage ----
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

COPY . .

RUN dotnet restore ./src/ArtifactReconstruction.API/ArtifactReconstruction.API.csproj
RUN dotnet publish ./src/ArtifactReconstruction.API/ArtifactReconstruction.API.csproj -c Release -o /app/publish

# ---- Runtime Stage ----
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview
WORKDIR /app

# 🔥 Render + Linux FS watcher crash fix
ENV DOTNET_USE_POLLING_FILE_WATCHER=true

# 🔥 ASP.NET Core port binding
ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "ArtifactReconstruction.API.dll"]
