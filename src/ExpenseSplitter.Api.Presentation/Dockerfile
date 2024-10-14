FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ExpenseSplitter.Api.Presentation/ExpenseSplitter.Api.Presentation.csproj", "ExpenseSplitter.Api.Presentation/"]
COPY ["ExpenseSplitter.Api.Application/ExpenseSplitter.Api.Application.csproj", "ExpenseSplitter.Api.Application/"]
COPY ["ExpenseSplitter.Api.Domain/ExpenseSplitter.Api.Domain.csproj", "ExpenseSplitter.Api.Domain/"]
COPY ["ExpenseSplitter.Api.Infrastructure/ExpenseSplitter.Api.Infrastructure.csproj", "ExpenseSplitter.Api.Infrastructure/"]
RUN dotnet restore "ExpenseSplitter.Api.Presentation/ExpenseSplitter.Api.Presentation.csproj"
COPY . .
WORKDIR "/src/ExpenseSplitter.Api.Presentation"
RUN dotnet build "ExpenseSplitter.Api.Presentation.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ExpenseSplitter.Api.Presentation.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ExpenseSplitter.Api.Presentation.dll"]
