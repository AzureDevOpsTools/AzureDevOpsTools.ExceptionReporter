FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 7222
EXPOSE 44338

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["AzureDevOps.Exception.Service/AzureDevOps.Exception.Service.csproj", "AzureDevOps.Exception.Service/"]
COPY ["AzureDevOps.Exception.Common/AzureDevOps.Exception.Common.csproj", "AzureDevOps.Exception.Common/"]
RUN dotnet restore "AzureDevOps.Exception.Service/AzureDevOps.Exception.Service.csproj"
COPY . .
WORKDIR "/src/AzureDevOps.Exception.Service"
RUN dotnet build "AzureDevOps.Exception.Service.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "AzureDevOps.Exception.Service.csproj" -c Release -o /app


FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "AzureDevOps.Exception.Service.dll"]