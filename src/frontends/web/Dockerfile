FROM microsoft/aspnetcore:2.0 AS base
ENV ASPNETCORE_URLS http://*:5000
ENV NODE_VERSION 9.10.0
ENV NODE_DOWNLOAD_URL https://nodejs.org/dist/v$NODE_VERSION/node-v$NODE_VERSION-linux-x64.tar.gz
ENV NODE_DOWNLOAD_SHA 21a69c0f0181ec451444739d5c2f1df27cb96e7f328461dfa658e65846dc99ef
RUN curl -SL "$NODE_DOWNLOAD_URL" --output nodejs.tar.gz \ 
	&& echo "$NODE_DOWNLOAD_SHA nodejs.tar.gz" | sha256sum -c - \ 
    && tar -xzf "nodejs.tar.gz" -C /usr/local --strip-components=1 \ 
    && rm nodejs.tar.gz \ 
	&& ln -s /usr/local/bin/node /usr/local/bin/nodejs
WORKDIR /app
EXPOSE 80 5000 5001

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY frontends/web/Rocketcon-web.csproj frontends/web/
COPY common/interfaces/interfaces.csproj common/interfaces/
COPY common/grains/grains.csproj common/grains/
RUN dotnet restore frontends/web/Rocketcon-web.csproj
COPY . .
WORKDIR /src/frontends/web
RUN dotnet build Rocketcon-web.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Rocketcon-web.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Rocketcon-web.dll"]
