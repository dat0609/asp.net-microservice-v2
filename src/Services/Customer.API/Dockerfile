FROM mcr.microsoft.com/dotnet/aspnet:8.0 as base 
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 as build
WORKDIR /src
COPY ["Services/Customer.API/Customer.API.csproj", "Services/Customer.API/"]
COPY ["BuildingBlocks/Common.Logging/Common.Logging.csproj", "BuildingBlocks/Common.Logging/"]
COPY ["BuildingBlocks/Contracts/Contracts.csproj", "BuildingBlocks/Contracts/"]
COPY ["BuildingBlocks/Infrastructure/Infrastructure.csproj", "BuildingBlocks/Infrastructure/"]
COPY ["BuildingBlocks/Shared/Shared.csproj", "BuildingBlocks/Shared/"]

RUN dotnet restore "Services/Customer.API/Customer.API.csproj"
COPY . .
WORKDIR "/src/Services/Customer.API"
RUN dotnet build "Customer.API.csproj" -c Release -o /app/build

FROM build as publish
RUN dotnet publish "Customer.API.csproj" -c Release -o /app/publish

FROM base as final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Customer.API.dll"]
