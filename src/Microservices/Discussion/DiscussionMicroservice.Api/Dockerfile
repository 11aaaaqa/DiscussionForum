FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Microservices/Discussion/DiscussionMicroservice.Api/DiscussionMicroservice.Api.csproj", "src/Microservices/Discussion/DiscussionMicroservice.Api/"]
COPY ["src/Building-blocks/MessageBus/MessageBus.csproj", "src/Building-blocks/MessageBus/"]
RUN dotnet restore "./src/Microservices/Discussion/DiscussionMicroservice.Api/./DiscussionMicroservice.Api.csproj"
COPY . .
WORKDIR "/src/src/Microservices/Discussion/DiscussionMicroservice.Api"
RUN dotnet build "./DiscussionMicroservice.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DiscussionMicroservice.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DiscussionMicroservice.Api.dll"]