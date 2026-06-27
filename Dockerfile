# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["AscarServiceHub/AscarServiceHub.csproj", "AscarServiceHub/"]
COPY ["NuGet.Config", "."]
RUN dotnet restore "AscarServiceHub/AscarServiceHub.csproj" --configfile "NuGet.Config"

COPY . .
RUN dotnet publish "AscarServiceHub/AscarServiceHub.csproj" -c Release -o /app/publish --no-restore

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

RUN mkdir -p /app/App_Data
RUN mkdir -p /app/wwwroot/uploads

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080
ENV Database__JsonDataDir=App_Data

ENTRYPOINT ["dotnet", "AscarServiceHub.dll"]
