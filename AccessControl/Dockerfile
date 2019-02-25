FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["AccessControl/AccessControl.csproj", "AccessControl/"]
RUN dotnet restore "AccessControl/AccessControl.csproj"
COPY . .
WORKDIR "/src/AccessControl"
RUN dotnet build "AccessControl.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "AccessControl.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "AccessControl.dll"]