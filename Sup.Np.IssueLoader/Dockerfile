FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Sup.Np.IssueLoader/Sup.Np.IssueLoader.csproj", "Sup.Np.IssueLoader/"]
RUN dotnet restore "Sup.Np.IssueLoader/Sup.Np.IssueLoader.csproj"
COPY . .
WORKDIR "/src/Sup.Np.IssueLoader"
RUN dotnet build "Sup.Np.IssueLoader.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Sup.Np.IssueLoader.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sup.Np.IssueLoader.dll"]
