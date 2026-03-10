# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .

RUN dotnet restore "src/Services/Auth/MoneyBee.Auth.API/MoneyBee.Auth.API.csproj"
RUN dotnet publish "src/Services/Auth/MoneyBee.Auth.API/MoneyBee.Auth.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MoneyBee.Auth.API.dll"]