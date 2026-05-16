FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY global.json ./
COPY src/VerdeMart.OpenBoxesBridge/*.csproj src/VerdeMart.OpenBoxesBridge/
RUN dotnet restore src/VerdeMart.OpenBoxesBridge/VerdeMart.OpenBoxesBridge.csproj

COPY src/VerdeMart.OpenBoxesBridge/ src/VerdeMart.OpenBoxesBridge/
RUN dotnet publish src/VerdeMart.OpenBoxesBridge/VerdeMart.OpenBoxesBridge.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./

RUN mkdir -p /data
VOLUME ["/data"]

ENTRYPOINT ["dotnet", "VerdeMart.OpenBoxesBridge.dll"]
