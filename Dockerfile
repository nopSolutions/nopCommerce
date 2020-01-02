# create the build instance 
FROM microsoft/dotnet:2.2-sdk AS build

WORKDIR /src                                                                    
COPY ./src ./

# restore solution
RUN dotnet restore NopCommerce.sln

WORKDIR /src/Presentation/Nop.Web   

# build project   
RUN dotnet build Nop.Web.csproj -c Release

# build plugins
WORKDIR /src/Plugins/Nop.Plugin.DiscountRules.CustomerRoles
RUN dotnet build Nop.Plugin.DiscountRules.CustomerRoles.csproj
WORKDIR /src/Plugins/Nop.Plugin.ExchangeRate.EcbExchange
RUN dotnet build Nop.Plugin.ExchangeRate.EcbExchange.csproj
WORKDIR /src/Plugins/Nop.Plugin.ExternalAuth.Facebook
RUN dotnet build Nop.Plugin.ExternalAuth.Facebook.csproj
WORKDIR /src/Plugins/Nop.Plugin.Misc.SendinBlue
RUN dotnet build Nop.Plugin.Misc.SendinBlue.csproj
WORKDIR /src/Plugins/Nop.Plugin.Payments.CheckMoneyOrder
RUN dotnet build Nop.Plugin.Payments.CheckMoneyOrder.csproj
WORKDIR /src/Plugins/Nop.Plugin.Payments.Manual
RUN dotnet build Nop.Plugin.Payments.Manual.csproj
WORKDIR /src/Plugins/Nop.Plugin.Payments.PayPalStandard
RUN dotnet build Nop.Plugin.Payments.PayPalStandard.csproj
WORKDIR /src/Plugins/Nop.Plugin.Payments.Qualpay
RUN dotnet build Nop.Plugin.Payments.Qualpay.csproj
WORKDIR /src/Plugins/Nop.Plugin.Payments.Square
RUN dotnet build Nop.Plugin.Payments.Square.csproj
WORKDIR /src/Plugins/Nop.Plugin.Pickup.PickupInStore
RUN dotnet build Nop.Plugin.Pickup.PickupInStore.csproj
WORKDIR /src/Plugins/Nop.Plugin.Shipping.FixedByWeightByTotal
RUN dotnet build Nop.Plugin.Shipping.FixedByWeightByTotal.csproj
WORKDIR /src/Plugins/Nop.Plugin.Shipping.UPS
RUN dotnet build Nop.Plugin.Shipping.UPS.csproj
WORKDIR /src/Plugins/Nop.Plugin.Tax.Avalara
RUN dotnet build Nop.Plugin.Tax.Avalara.csproj
WORKDIR /src/Plugins/Nop.Plugin.Tax.FixedOrByCountryStateZip
RUN dotnet build Nop.Plugin.Tax.FixedOrByCountryStateZip.csproj
WORKDIR /src/Plugins/Nop.Plugin.Widgets.GoogleAnalytics
RUN dotnet build Nop.Plugin.Widgets.GoogleAnalytics.csproj
WORKDIR /src/Plugins/Nop.Plugin.Widgets.NivoSlider
RUN dotnet build Nop.Plugin.Widgets.NivoSlider.csproj

# publish project
WORKDIR /src/Presentation/Nop.Web   
RUN dotnet publish Nop.Web.csproj -c Release -o /app/published

# create the runtime instance 
FROM microsoft/dotnet:2.2-aspnetcore-runtime-alpine AS runtime 

# add globalization support
RUN apk add --no-cache icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

WORKDIR /app        
RUN mkdir bin
RUN mkdir logs  
                                                            
COPY --from=build /app/published .
                            
ENTRYPOINT ["dotnet", "Nop.Web.dll"]
