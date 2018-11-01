FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["TapThat.Server/TapThat.Server.csproj", "TapThat.Server/"]
RUN dotnet restore "TapThat.Server/TapThat.Server.csproj"
COPY . .
WORKDIR "/src/TapThat.Server"
RUN dotnet build "TapThat.Server.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "TapThat.Server.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "TapThat.Server.dll"]