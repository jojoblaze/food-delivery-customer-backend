
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src


COPY ["customer-backend.csproj", "customer-backend/"]
RUN dotnet restore "customer-backend/customer-backend.csproj"

WORKDIR "/src/customer-backend"
COPY . .

RUN dotnet build "customer-backend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "customer-backend.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "customer-backend.dll"]

