FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
RUN apt-get update; apt-get install -y fontconfig
RUN fc-cache -f -v
USER app
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Web/Web.MVC/Web.MVC/Web.MVC.csproj", "src/Web/Web.MVC/Web.MVC/"]
COPY ["src/Libs/GeneralClassesLib/GeneralClassesLib.csproj", "src/Libs/GeneralClassesLib/"]
RUN dotnet restore "./src/Web/Web.MVC/Web.MVC/./Web.MVC.csproj"
COPY . .
WORKDIR "/src/src/Web/Web.MVC/Web.MVC"
RUN dotnet build "./Web.MVC.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Web.MVC.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Web.MVC.dll"]