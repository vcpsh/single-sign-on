FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY sh.vcp.sso.sln ./
COPY sh.vcp.sso.server/sh.vcp.sso.server.csproj sh.vcp.sso.server/
COPY sh.vcp.identity/sh.vcp.identity.csproj sh.vcp.identity/
COPY sh.vcp.ldap/sh.vcp.ldap.csproj sh.vcp.ldap/
RUN dotnet restore -nowarn:msb3202,nu1503
COPY . .
WORKDIR /src/sh.vcp.sso.server
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "sh.vcp.sso.server.dll"]
