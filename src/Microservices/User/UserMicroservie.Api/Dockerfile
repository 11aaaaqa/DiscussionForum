FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Microservices/User/UserMicroservie.Api/UserMicroservice.Api.csproj", "src/Microservices/User/UserMicroservie.Api/"]
COPY ["src/Building-blocks/MessageBus/MessageBus.csproj", "src/Building-blocks/MessageBus/"]
RUN dotnet restore "./src/Microservices/User/UserMicroservie.Api/./UserMicroservice.Api.csproj"
COPY . .
WORKDIR "/src/src/Microservices/User/UserMicroservie.Api"
RUN dotnet build "./UserMicroservice.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./UserMicroservice.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UserMicroservice.Api.dll"]