FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Directory.Build.props", "."]
COPY ["Presentation/Nop.Web/Nop.Web.csproj", "Presentation/Nop.Web/"]
COPY ["Libraries/Nop.Core/Nop.Core.csproj", "Libraries/Nop.Core/"]
COPY ["Libraries/Nop.Data/Nop.Data.csproj", "Libraries/Nop.Data/"]
COPY ["Libraries/Nop.Services/Nop.Services.csproj", "Libraries/Nop.Services/"]
COPY ["Presentation/Nop.Web.Framework/Nop.Web.Framework.csproj", "Presentation/Nop.Web.Framework/"]

RUN dotnet restore "./Presentation/Nop.Web/Nop.Web.csproj"
COPY . .

# Build plugins
WORKDIR /src/Plugins
RUN find . -name "*.csproj" | while read csproj; do \
      dotnet build "$csproj" -c $BUILD_CONFIGURATION || true; \
    done

# Build main project
WORKDIR /src/Presentation/Nop.Web
RUN dotnet build "./Nop.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Nop.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Set writable folder permissions
RUN mkdir -p /app/publish/logs \
             /app/publish/App_Data \
             /app/publish/Plugins \
    && chmod -R 775 /app/publish/App_Data \
                    /app/publish/logs \
                    /app/publish/Plugins \
                    /app/publish/wwwroot

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN chown -R $APP_UID:$APP_UID /app
USER $APP_UID
ENTRYPOINT ["dotnet", "Nop.Web.dll"]