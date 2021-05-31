#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Fitlog/Fitlog.csproj", "./Fitlog/Fitlog.csproj"]
COPY ["Fitlog.Api/Fitlog.Api.csproj", "./Fitlog.Api/Fitlog.Api.csproj"]
COPY ["Fitlog.Core/Fitlog.Core.csproj", "./Fitlog.Core/Fitlog.Core.csproj"]
RUN dotnet restore "./Fitlog/Fitlog.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "Fitlog/Fitlog.csproj" -c Release -o /app/build

FROM build AS publish
RUN apt-get update -y
RUN apt-get install -y curl
RUN curl --silent --location https://deb.nodesource.com/setup_12.x | bash -
RUN apt-get install -y nodejs
RUN npm install @quasar/cli -g
WORKDIR "/src/client/fitlog.fi/."
RUN npm install
RUN cp fi.js node_modules/quasar-framework/i18n
RUN quasar build
RUN rm -r /src/Fitlog/wwwroot/*
RUN cp -r dist/spa-mat /src/Fitlog/wwwroot
WORKDIR "/src/."
RUN dotnet publish "Fitlog/Fitlog.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Fitlog.dll"]