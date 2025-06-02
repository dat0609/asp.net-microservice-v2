FROM mcr.microsoft.com/dotnet/aspnet:8.0 as base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 as build
WORKDIR /src
COPY ["WebApps\WebHealthStatus\WebHealthStatus.csproj", "WebApps/WebHealthStatus"]
RUN dotnet restore "WebApps\WebHealthStatus\WebHealthStatus.csproj"
COPY . .
WORKDIR "/src/WebApps/WebHealthStatus"
RUN dotnet build "WebHealthStatus.csproj" -c Release -o /app/build

FROM build as publish
RUN dotnet publish "WebHealthStatus.csproj" -c Release -o /app/publish

FROM base as final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebHealthStatus.dll"]