FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src/Microservices/Register
RUN mkdir RegisterMicroservice.Api
WORKDIR /src/Microservices/Register/RegisterMicroservice.Api
COPY RegisterMicroservice.Api.csproj .
RUN dotnet restore ./Microservices/Register/RegisterMicroservice.Api.csproj
COPY . .
RUN dotnet build ./Microservices/Register/RegisterMicroservice.Api.csproj -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish ./Microservices/Register/RegisterMicroservice.Api.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RegisterMicroservice.Api.dll"]