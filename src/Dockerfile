FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["Presentation/Nop.Web/Nop.Web.csproj", "Presentation/Nop.Web/"]
COPY ["Libraries/Nop.Services/Nop.Services.csproj", "Libraries/Nop.Services/"]
COPY ["Libraries/Nop.Core/Nop.Core.csproj", "Libraries/Nop.Core/"]
COPY ["Libraries/Nop.Data/Nop.Data.csproj", "Libraries/Nop.Data/"]
COPY ["Presentation/Nop.Web.Framework/Nop.Web.Framework.csproj", "Presentation/Nop.Web.Framework/"]
RUN dotnet restore "Presentation/Nop.Web/Nop.Web.csproj"
COPY . .
WORKDIR "/src/Presentation/Nop.Web"
RUN dotnet build "Nop.Web.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Nop.Web.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Nop.Web.dll"]