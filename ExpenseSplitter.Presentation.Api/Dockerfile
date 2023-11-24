FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["ExpenseSplitter.Presentation.Api/ExpenseSplitter.Presentation.Api.csproj", "ExpenseSplitter.Presentation.Api/"]
RUN dotnet restore "ExpenseSplitter.Presentation.Api/ExpenseSplitter.Presentation.Api.csproj"
COPY . .
WORKDIR "/src/ExpenseSplitter.Presentation.Api"
RUN dotnet build "ExpenseSplitter.Presentation.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ExpenseSplitter.Presentation.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ExpenseSplitter.Presentation.Api.dll"]
