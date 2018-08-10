FROM node:alpine AS yarninstall
RUN apk add yarn
RUN yarn global add @angular/cli
WORKDIR /src
COPY client/package.json ./package.json
COPY client/yarn.lock ./yarn.lock
RUN yarn --pure-lockfile

FROM yarninstall AS yarninstall_projects
COPY client/projects/vcpsh/sso-client-lib/package.json ./projects/vcpsh/sso-client-lib/package.json
COPY client/projects/vcpsh/sso-client-lib/yarn.lock ./projects/vcpsh/sso-client-lib/yarn.lock
WORKDIR /src/projects/vcpsh/sso-client-lib
RUN yarn --pure-lockfile

FROM yarninstall_projects AS copy_client_sources
WORKDIR /src
COPY client .

FROM copy_client_sources AS ngbuild_projects
WORKDIR /src
RUN ng build --project @vcpsh/sso-client-lib

FROM ngbuild_projects AS ngbuild
WORKDIR /src
RUN yarn run build:production

FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY sh.vcp.sso.server/sh.vcp.sso.server.csproj sh.vcp.sso.server/
COPY sh.vcp.ldap/sh.vcp.ldap.csproj sh.vcp.ldap/
COPY sh.vcp.identity/sh.vcp.identity.csproj sh.vcp.identity/
RUN dotnet restore sh.vcp.sso.server/sh.vcp.sso.server.csproj
COPY . .
WORKDIR /src/sh.vcp.sso.server
RUN dotnet build sh.vcp.sso.server.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish sh.vcp.sso.server.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
COPY --from=ngbuild /src/dist/client/ ./wwwroot/
ENTRYPOINT ["dotnet", "sh.vcp.sso.server.dll"]
