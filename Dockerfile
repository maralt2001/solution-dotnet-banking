FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["./BankingApi/BankingApi.csproj", "BankingApi/"]
COPY ["./MongoDBContext/MongoService.csproj", "MongoDBContext/"]
RUN dotnet restore "BankingApi/BankingApi.csproj"
COPY . .
WORKDIR "/src/BankingApi"
RUN dotnet build "BankingApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BankingApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BankingApi.dll"]