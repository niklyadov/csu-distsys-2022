FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY "TestApp.Worker/TestApp.Worker.csproj" "TestApp.Worker/"
RUN dotnet restore "TestApp.Worker/TestApp.Worker.csproj"
COPY . .
WORKDIR "/src/TestApp.Worker"
RUN dotnet build "TestApp.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TestApp.Worker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TestApp.Worker.dll"]
