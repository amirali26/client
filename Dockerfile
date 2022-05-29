  # syntax=docker/dockerfile:1
  FROM mcr.microsoft.com/dotnet/aspnet:5.0
  COPY bin/Release/net5.0/publish/ .
  ENV ASPNETCORE_URLS http://+:8080
  EXPOSE 8080
  ENTRYPOINT ["dotnet", "client.dll"]
 