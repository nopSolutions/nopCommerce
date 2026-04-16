FROM mcr.microsoft.com/dotnet/sdk:9.0
ADD . /nop
WORKDIR /nop
RUN dotnet package -c Release /src/Presentation/Nop.Web
