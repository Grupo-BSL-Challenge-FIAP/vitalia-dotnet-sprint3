FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY ["Vitalia.API/Vitalia.API.csproj", "Vitalia.API/"]
COPY ["Vitalia.Application/Vitalia.Application.csproj", "Vitalia.Application/"]
COPY ["Vitalia.Domain/Vitalia.Domain.csproj", "Vitalia.Domain/"]
COPY ["Vitalia.Infrastructure/Vitalia.Infrastructure.csproj", "Vitalia.Infrastructure/"]

RUN dotnet restore "Vitalia.API/Vitalia.API.csproj"

COPY . .

WORKDIR "/src/Vitalia.API"

RUN dotnet publish \
    "Vitalia.API.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false


FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "Vitalia.API.dll"]