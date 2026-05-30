FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build

WORKDIR /src

# Restore using both project files (worker references the contracts library).
COPY worker/Worker.csproj worker/
COPY contracts/Contracts.csproj contracts/
RUN dotnet restore worker/Worker.csproj

COPY worker/ worker/
COPY contracts/ contracts/
RUN dotnet publish worker/Worker.csproj -c Release -o /app/published

FROM mcr.microsoft.com/dotnet/runtime:10.0-alpine AS runtime

WORKDIR /app
COPY --from=build /app/published .

ENTRYPOINT ["dotnet", "Omnichannel.Worker.dll"]
