FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
EXPOSE 80
WORKDIR /app
COPY client/dist/client ./wwwroot/
COPY sh.vcp.sso.server/bin/Release/netcoreapp2.2/publish/ .
ENTRYPOINT ["dotnet", "sh.vcp.sso.server.dll"]
