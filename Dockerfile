FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /webapi

EXPOSE 80
EXPOSE 5203

# copy csproj and restore as distinct layers
COPY ./*.csproj ./
RUN dotnet restore

# copy everything else and build app
COPY . .
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/sdk:6.0 
WORKDIR /webapi
COPY --from=build /webapi/out .

ENTRYPOINT ["dotnet", "apiRestBot.dll"]