FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY SIRest/GameCollectionApi.csproj SIRest/
RUN dotnet restore SIRest/GameCollectionApi.csproj
COPY SIRest/ SIRest/
RUN dotnet publish SIRest/GameCollectionApi.csproj -c Release -o /app/publish --no-restore /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=80
ENV ApiDocumentation__Enabled=true
EXPOSE 80
COPY --from=build /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "GameCollectionApi.dll"]
