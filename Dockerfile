FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM base AS final
WORKDIR /app
COPY ./sh.vcp.sso.server/bin/Release/netcoreapp2.2/publish/ .
COPY ./client/dist/client ./wwwroot/
ENTRYPOINT ["dotnet", "sh.vcp.sso.server.dll"]
