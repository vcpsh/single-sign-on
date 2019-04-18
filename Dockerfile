FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
EXPOSE 80
WORKDIR /app
COPY sh.vcp.sso.server/bin/Release/netcoreapp2.2/publish/server .
COPY client/dist/client ./wwwroot/
ENTRYPOINT ["dotnet", "sh.vcp.sso.server.dll"]
