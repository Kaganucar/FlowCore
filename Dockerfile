FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY src/FlowCore.Domain/*.csproj src/FlowCore.Domain/
COPY src/FlowCore.Application/*.csproj src/FlowCore.Application/
COPY src/FlowCore.Infrastructure/*.csproj src/FlowCore.Infrastructure/
COPY src/FlowCore.API/*.csproj src/FlowCore.API/
RUN dotnet restore src/FlowCore.API/FlowCore.API.csproj

COPY src/ src/
RUN dotnet publish src/FlowCore.API/FlowCore.API.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "FlowCore.API.dll"]