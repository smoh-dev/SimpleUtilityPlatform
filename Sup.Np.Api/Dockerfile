# Run below docker build command at solution directory.
# docker build -f .\NpApiDockerfile -t sup/np-api:0.0.1 .

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Sup.Np.Api/Sup.Np.Api.csproj", "Sup.Np.Api/"]
COPY ["Sup.Common/Sup.Common.csproj", "Sup.Common/"]
RUN dotnet restore "Sup.Np.Api/Sup.Np.Api.csproj"
COPY . .
WORKDIR "/src/Sup.Np.Api"
RUN dotnet build "Sup.Np.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sup.Np.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sup.Np.Api.dll"]
