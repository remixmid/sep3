# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY BlazorApp1.csproj ./
RUN dotnet restore "./BlazorApp1.csproj"

# Copy the rest of the source and publish
COPY . .
RUN dotnet publish "./BlazorApp1.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Expose the port the app will listen on inside the container
EXPOSE 8080

# ASP.NET Core in containers defaults to binding to http://+:8080 when this is set
ENV ASPNETCORE_URLS=http://+:8080

# Copy published output
COPY --from=build /app/publish .

# Start the app
ENTRYPOINT ["dotnet", "BlazorApp1.dll"]

