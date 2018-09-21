FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 7222
EXPOSE 44338

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["AzureDevOpsTools.ExceptionService/AzureDevOpsTools.ExceptionService.csproj", "AzureDevOpsTools.ExceptionService/"]
COPY ["AzureDevOpsTools.Exception.Common/AzureDevOpsTools.Exception.Common.csproj", "AzureDevOpsTools.Exception.Common/"]
RUN dotnet restore "AzureDevOpsTools.ExceptionService/AzureDevOpsTools.ExceptionService.csproj"
COPY . .
WORKDIR "/src/AzureDevOpsTools.ExceptionService"
RUN dotnet build "AzureDevOpsTools.ExceptionService.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "AzureDevOpsTools.ExceptionService.csproj" -c Release -o /app


FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "AzureDevOpsTools.ExceptionService.dll"]