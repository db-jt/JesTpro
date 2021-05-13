#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["JesTpro.Web/JestProWeb.csproj", "JesTpro.Web/"]
RUN dotnet restore "JesTpro.Web/JestProWeb.csproj"
COPY . .
WORKDIR "/src/JesTpro.Web"
RUN dotnet build "JestProWeb.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "JestProWeb.csproj" -c Release -o /app/publish 

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# <wkhtmltopdf>
ENV PATH=/app;$PATH
RUN apt-get update
RUN apt-get install wget fontconfig libfreetype6 libx11-6 libxcb1 libxext6 libxrender1 xfonts-75dpi xfonts-base libjpeg62-turbo multiarch-support -y

RUN wget http://security.debian.org/debian-security/pool/updates/main/o/openssl/libssl1.0.0_1.0.1t-1+deb8u12_amd64.deb
RUN dpkg -i libssl1.0.0_1.0.1t-1+deb8u12_amd64.deb

RUN wget http://ftp.us.debian.org/debian/pool/main/libp/libpng/libpng12-0_1.2.50-2+deb8u3_amd64.deb
RUN dpkg -i libpng12-0_1.2.50-2+deb8u3_amd64.deb

RUN wget https://github.com/wkhtmltopdf/wkhtmltopdf/releases/download/0.12.5/wkhtmltox_0.12.5-1.jessie_amd64.deb
RUN dpkg -i wkhtmltox_0.12.5-1.jessie_amd64.deb

RUN cp /usr/local/bin/wkhtmlto* /usr/bin/
# </wkhtmltopdf>

ENTRYPOINT ["dotnet", "JesTproWeb.dll"]