# create the build instance 
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build

WORKDIR /src                                                                    
COPY ./src ./

# restore solution
RUN dotnet restore NopCommerce.sln

WORKDIR /src/Presentation/Nop.Web   

# build project   
RUN dotnet build Nop.Web.csproj -c Release

# build plugins
WORKDIR /src/Plugins
RUN set -eux; \
    for dir in *; do \
        if [ -d "$dir" ]; then \
            dotnet build "$dir/$dir.csproj" -c Release; \
        fi; \
    done

# publish project
WORKDIR /src/Presentation/Nop.Web   
RUN dotnet publish Nop.Web.csproj -c Release -o /app/published

WORKDIR /app/published

RUN mkdir logs
RUN mkdir bin

RUN chmod 775 App_Data/
RUN chmod 775 App_Data/DataProtectionKeys
RUN chmod 775 bin
RUN chmod 775 logs
RUN chmod 775 Plugins
RUN chmod 775 wwwroot/bundles
RUN chmod 775 wwwroot/db_backups
RUN chmod 775 wwwroot/files/exportimport
RUN chmod 775 wwwroot/icons
RUN chmod 775 wwwroot/images
RUN chmod 775 wwwroot/images/thumbs
RUN chmod 775 wwwroot/images/uploaded

# create the runtime instance 
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime 

# add globalization support
RUN apk add --no-cache icu-libs icu-data-full
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# installs required packages
RUN apk add tiff --no-cache --repository http://dl-3.alpinelinux.org/alpine/edge/main/ --allow-untrusted
RUN apk add libgdiplus --no-cache --repository http://dl-3.alpinelinux.org/alpine/edge/community/ --allow-untrusted
RUN apk add libc-dev --no-cache
RUN apk add tzdata --no-cache

# copy entrypoint script
COPY ./entrypoint.sh /entrypoint.sh
RUN chmod 755 /entrypoint.sh

WORKDIR /app

COPY --from=build /app/published .

EXPOSE 80
                            
ENTRYPOINT "/entrypoint.sh"
