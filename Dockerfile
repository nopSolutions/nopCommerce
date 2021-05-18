# create the build instance 
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build

WORKDIR /src

# restore solution
COPY ./src/NopCommerce.sln .
COPY **/*.csproj **/**/*.csproj **/**/**/*.csproj ./

RUN dotnet sln NopCommerce.sln list | while read line ; do mkdir -pv $(dirname $line) && mv $(basename $line) $line; done
RUN dotnet restore NopCommerce.sln

COPY ./src ./

WORKDIR /src/Presentation/Nop.Web

# build project
RUN dotnet build Nop.Web.csproj -c Release --no-restore

# build plugins
WORKDIR /src/Plugins/Nop.Plugin.DiscountRules.CustomerRoles
RUN dotnet build Nop.Plugin.DiscountRules.CustomerRoles.csproj -c Release
WORKDIR /src/Plugins/Nop.Plugin.ExchangeRate.EcbExchange
RUN dotnet build Nop.Plugin.ExchangeRate.EcbExchange.csproj -c Release
WORKDIR /src/Plugins/Nop.Plugin.ExternalAuth.ExtendedAuth
RUN dotnet build Nop.Plugin.ExternalAuth.ExtendedAuth.csproj -c Release
WORKDIR /src/Plugins/Nop.Plugin.Payments.CheckMoneyOrder
RUN dotnet build Nop.Plugin.Payments.CheckMoneyOrder.csproj -c Release
WORKDIR /src/Plugins/Nop.Plugin.Shipping.FixedByWeightByTotal
RUN dotnet build Nop.Plugin.Shipping.FixedByWeightByTotal.csproj -c Release
WORKDIR /src/Plugins/Nop.Plugin.Widgets.AccessiBe
RUN dotnet build Nop.Plugin.Widgets.AccessiBe.csproj -c Release
WORKDIR /src/Plugins/Nop.Plugin.Tax.FixedOrByCountryStateZip
RUN dotnet build Nop.Plugin.Tax.FixedOrByCountryStateZip.csproj -c Release
WORKDIR /src/Plugins/Nop.Plugin.Widgets.GoogleAnalytics
RUN dotnet build Nop.Plugin.Widgets.GoogleAnalytics.csproj -c Release
WORKDIR /src/Plugins/Nop.Plugin.Widgets.NivoSlider
RUN dotnet build Nop.Plugin.Widgets.NivoSlider.csproj -c Release
WORKDIR /src/Plugins/Nop.Plugin.BuyAmScraper
RUN dotnet build Nop.Plugin.BuyAmScraper.csproj -c Release

# publish project
WORKDIR /src/Presentation/Nop.Web
RUN dotnet publish Nop.Web.csproj -c Release -o /app/published

# create the runtime instance 
FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine AS runtime 

# add globalization support
RUN apk add --no-cache icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# installs required packages
RUN apk add libgdiplus --no-cache --repository http://dl-3.alpinelinux.org/alpine/edge/testing/ --allow-untrusted
RUN apk add libc-dev --no-cache
RUN apk add --no-cache tzdata

# copy entrypoint script
COPY ./entrypoint.sh /entrypoint.sh
RUN chmod 755 /entrypoint.sh

WORKDIR /app        
RUN mkdir bin
RUN mkdir logs

COPY --from=build /app/published .

# call entrypoint script instead of dotnet                            
ENTRYPOINT "/entrypoint.sh"
