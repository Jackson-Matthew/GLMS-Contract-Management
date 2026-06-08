FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["GLMS_ST10445830/GLMS_ST10445830.csproj", "GLMS_ST10445830/"]
COPY ["GLMS_ST10445830.API/GLMS_ST10445830.API.csproj", "GLMS_ST10445830.API/"]
RUN dotnet restore "GLMS_ST10445830/GLMS_ST10445830.csproj"
COPY . .
WORKDIR "/src/GLMS_ST10445830"
RUN dotnet build "GLMS_ST10445830.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "GLMS_ST10445830.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false /p:ErrorOnDuplicatePublishOutputFiles=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GLMS_ST10445830.dll"]
