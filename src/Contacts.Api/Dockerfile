FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Contacts.Api/Contacts.Api.csproj", "src/Contacts.Api/"]
COPY ["src/Contacts.Domain/Contacts.Domain.csproj", "src/Contacts.Domain/"]
COPY ["src/Contacts.DataAccess/Contacts.DataAccess.csproj", "src/Contacts.DataAccess/"]
COPY ["src/Contacts.Contracts/Contacts.Contracts.csproj", "src/Contacts.Contracts/"]
COPY ["src/Contacts.Application/Contacts.Application.csproj", "src/Contacts.Application/"]
COPY ["src/Contacts.Abstractions/Contacts.Abstractions.csproj", "src/Contacts.Abstractions/"]
COPY . .
RUN dotnet restore "src/Contacts.Api/Contacts.Api.csproj"
WORKDIR "/src/src/Contacts.Api"
RUN dotnet build "./Contacts.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Contacts.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Contacts.Api.dll"]
