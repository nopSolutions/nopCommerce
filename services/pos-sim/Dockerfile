FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build

WORKDIR /src
COPY POS.Simulator.csproj ./
RUN dotnet restore POS.Simulator.csproj

COPY . ./
RUN dotnet publish POS.Simulator.csproj -c Release -o /app/published

FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS runtime

WORKDIR /app
COPY --from=build /app/published .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "POS.Simulator.dll"]
