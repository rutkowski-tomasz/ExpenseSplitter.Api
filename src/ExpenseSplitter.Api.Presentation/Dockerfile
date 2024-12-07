ARG PROJECT_NAME=ExpenseSplitter.Api.Presentation

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG PROJECT_NAME
WORKDIR /build
COPY [".editorconfig", "./"]
COPY ["src/*/*.csproj", "./"]
RUN for file in $(ls *.csproj); do mkdir -p src/${file%.*}/ && mv $file src/${file%.*}/; done
RUN dotnet restore "src/${PROJECT_NAME}/${PROJECT_NAME}.csproj"
COPY . .
RUN dotnet build "src/${PROJECT_NAME}/${PROJECT_NAME}.csproj" -c Release -o /app/build

FROM build AS publish
ARG PROJECT_NAME
RUN dotnet publish "src/${PROJECT_NAME}/${PROJECT_NAME}.csproj" -c Release -o /app/publish

FROM base AS final
ARG PROJECT_NAME
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "${PROJECT_NAME}.dll"]
