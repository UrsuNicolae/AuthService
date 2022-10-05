#Building APP
FROM mcr.microsoft.com/dotnet/sdk:6.0 as build 
WORKDIR /app
COPY *.csproj ./
RUN dotnet restore
COPY . ./
RUN dotnet publish -c Release -o out
#Finished Building, Starting app
FROM mcr.microsoft.com/dotnet/aspnet:6.0 as runtime 
EXPOSE 80 
EXPOSE 443
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT [ "dotnet", "/app/AuthService.dll" ]