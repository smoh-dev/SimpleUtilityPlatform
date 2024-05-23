FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Sup.Mm.Api/Sup.Mm.Api.csproj", "Sup.Mm.Api/"]
RUN dotnet restore "Sup.Mm.Api/Sup.Mm.Api.csproj"
COPY . .
WORKDIR "/src/Sup.Mm.Api"
RUN dotnet build "Sup.Mm.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Sup.Mm.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sup.Mm.Api.dll"]
