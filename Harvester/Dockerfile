FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Harvester/Harvester.csproj", "Harvester/"]
RUN dotnet restore "Harvester/Harvester.csproj"
COPY . .
WORKDIR "/src/Harvester"
RUN dotnet build "Harvester.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Harvester.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Harvester.dll"]
