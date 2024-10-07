FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["MarketAssetAPI.csproj", "./"]
RUN dotnet restore "MarketAssetAPI.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "MarketAssetAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MarketAssetAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MarketAssetAPI.dll"]

// appsettings.json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Fintacharts": {
    "URI": "https://platform.fintacharts.com",
    "URI_WSS": "wss://platform.fintacharts.com",
    "USERNAME": "r_test@fintatech.com",
    "PASSWORD": "kisfiz-vUnvy9-sopnyv"
  }
}