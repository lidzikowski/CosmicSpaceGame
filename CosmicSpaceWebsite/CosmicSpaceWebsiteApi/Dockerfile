FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["CosmicSpaceWebsiteApi/CosmicSpaceWebsiteApi.csproj", "CosmicSpaceWebsiteApi/"]
RUN dotnet restore "CosmicSpaceWebsiteApi/CosmicSpaceWebsiteApi.csproj"
COPY . .
WORKDIR "/src/CosmicSpaceWebsiteApi"
RUN dotnet build "CosmicSpaceWebsiteApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CosmicSpaceWebsiteApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CosmicSpaceWebsiteApi.dll"]