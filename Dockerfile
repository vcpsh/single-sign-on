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
ENTRYPOINT ["dotnet", "sh.vcp.sso.server.dll"]
