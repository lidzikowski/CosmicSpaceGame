FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["CosmicSpaceWebsite/CosmicSpaceWebsite.csproj", "CosmicSpaceWebsite/"]
RUN dotnet restore "CosmicSpaceWebsite/CosmicSpaceWebsite.csproj"
COPY . .
WORKDIR "/src/CosmicSpaceWebsite"
RUN dotnet build "CosmicSpaceWebsite.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CosmicSpaceWebsite.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CosmicSpaceWebsite.dll"]