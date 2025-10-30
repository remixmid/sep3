# Use the official ASP.NET Core runtime as a parent image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Use the SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["AspCoreWebAPI.csproj", "."]
RUN dotnet restore "./AspCoreWebAPI.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "AspCoreWebAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AspCoreWebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage/image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AspCoreWebAPI.dll"]
