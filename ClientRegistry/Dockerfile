FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

#RUN dotnet dev-certs https
#RUN dotnet dev-certs https --trust

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ClientRegistry/ClientRegistry.csproj", "ClientRegistry/"]
RUN dotnet restore "ClientRegistry/ClientRegistry.csproj"
COPY . .
WORKDIR "/src/ClientRegistry"
RUN dotnet build "ClientRegistry.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
RUN dotnet publish "ClientRegistry.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

#RUN dotnet ef migrations add InitMigration
#RUN dotnet ef database update

FROM base AS final
WORKDIR /app
#RUN dotnet ef database update
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ClientRegistry.dll"]
