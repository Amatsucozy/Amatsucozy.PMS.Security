FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Amatsucozy.PMS.Security.Portal/Amatsucozy.PMS.Security.Portal.csproj", "Amatsucozy.PMS.Security.Portal/"]
RUN dotnet restore "Amatsucozy.PMS.Security.Portal/Amatsucozy.PMS.Security.Portal.csproj"
COPY . .
WORKDIR "/src/Amatsucozy.PMS.Security.Portal"
RUN dotnet build "Amatsucozy.PMS.Security.Portal.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Amatsucozy.PMS.Security.Portal.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Amatsucozy.PMS.Security.Portal.dll"]
