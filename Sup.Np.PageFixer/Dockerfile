# Run below docker build command at solution directory.
# docker build -f NpPageFixerDockerFile -t sup/np-page-fixer:0.0.1 .

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Sup.Np.PageFixer/Sup.Np.PageFixer.csproj", "Sup.Np.PageFixer/"]
COPY ["Sup.Common/Sup.Common.csproj", "Sup.Common/"]
COPY ["Sup.Common.Logger/Sup.Common.Logger.csproj", "Sup.Common.Logger/"]
RUN dotnet restore "Sup.Np.PageFixer/Sup.Np.PageFixer.csproj"
COPY . .
WORKDIR "/src/Sup.Np.PageFixer"
RUN dotnet build "Sup.Np.PageFixer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sup.Np.PageFixer.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sup.Np.PageFixer.dll"]
