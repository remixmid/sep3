# AspCoreWebAPI – Help

This service is an ASP.NET Core Web API with EF Core. It supports two run modes:
- Local development with EF Core InMemory (default)
- Docker + PostgreSQL (optional)

## Local quick start (InMemory)
- Requires .NET 8 SDK

Commands (cmd.exe):
```cmd
cd C:\JAVA\sep3\AspCoreWebAPI
Dotnet restore
Dotnet run --launch-profile AspCoreWebAPI
```
URLs:
- Root (redirects to Swagger): http://localhost:8082/
- Swagger: http://localhost:8082/swagger
- Health: http://localhost:8082/api/health

Notes:
- HTTPS redirection is disabled for local dev; always use http.
- CORS is open for dev.

## Docker + PostgreSQL
From repo root:
```cmd
cd C:\JAVA\sep3
docker-compose up --build
```
Default ports (compose):
- API: http://localhost:6000
- Swagger: http://localhost:6000/swagger
- PostgreSQL (host): 5433

## Switch API to PostgreSQL (outside Docker)
1) Run a Postgres container:
```cmd
docker run -d --name aspcore-db -e POSTGRES_DB=aspcore_webapi_db -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=postgres -p 5434:5432 postgres:15
```
2) In `Program.cs`, swap InMemory for Npgsql:
```csharp
// options.UseInMemoryDatabase("AspCoreWebApiDb");
options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
```
3) In `appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5434;Database=aspcore_webapi_db;Username=postgres;Password=postgres"
}
```
4) (Optional) Migrations:
```cmd
dotnet tool install --global dotnet-ef
cd C:\JAVA\sep3\AspCoreWebAPI
dotnet ef migrations add InitialCreate
dotnet ef database update
```

## Troubleshooting
- Free a busy port:
```cmd
netstat -ano | findstr :8082
Tasklist | findstr <PID>
Taskkill /PID <PID> /F
```
- If root doesn’t load: use http (not https). Root '/' redirects to '/swagger'. Try incognito.
- Compose port conflicts: edit `docker-compose.yml` host ports.

---
This HELP.md explains local and Docker usage, and how to switch to PostgreSQL.

