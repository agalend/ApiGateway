# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.sln .
COPY PaySmartly.ApiGateway/*.csproj ./PaySmartly.ApiGateway/
RUN dotnet restore

# copy everything else and build app
COPY PaySmartly.ApiGateway/. ./PaySmartly.ApiGateway/
WORKDIR /source/PaySmartly.ApiGateway
RUN dotnet publish -c release -o /PaySmartly.ApiGateway --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:8.0

LABEL author="Stefan Bozov"

WORKDIR /PaySmartly.ApiGateway
COPY --from=build /PaySmartly.ApiGateway ./
ENTRYPOINT ["dotnet", "PaySmartly.ApiGateway.dll"]