# Person Management API

A modern, production-ready **.NET 9 Web API** for managing persons and person types, designed with **Clean Architecture** and best practices in mind.

## ✨ Features

- **Clean Architecture** (Domain, Application, Infrastructure, API)
- **CQRS** pattern using MediatR-style command/query handlers
- **Health Checks** with customizable UI dashboard
- **Observability** via OpenTelemetry, Jaeger, Prometheus, Grafana
- **Structured Logging** using Serilog with OTLP and console support
- **SQL Server** support
- **Docker Compose** for seamless local development
- **Swagger** for API documentation
- **Integration Testing** with Testcontainers

---

## 🏗️ Architecture Overview

This application follows Clean Architecture principles:

- **Domain Layer**: Core business entities and rules (e.g., `Person`, `PersonType`)
- **Application Layer**: CQRS handlers and use cases
- **Infrastructure Layer**: SQL Server database, health checks, external services
- **API Layer**: HTTP controllers, DI configuration, Swagger, health endpoints

### 📁 Project Structure
src/
Person.Domain/ # Business entities
Person.Application/ # CQRS logic, use cases
Person.Infrastructure/ # Data access, health checks
Person.Api/ # API controllers, DI setup

tests/
Person.Integration.Tests/ # Testcontainers-based integration tests


---

## 🚀 Getting Started

### Run with Docker (recommended)

```bash```
docker compose up --build

Local Services:
- API: http://localhost:7017
- Swagger UI: http://localhost:7017/swagger
- Health UI: http://localhost:7017/health-ui
- Jaeger: http://localhost:16686
- Grafana: http://localhost:3000 (admin/admin)
- Prometheus: http://localhost:9090

###  Working with Multiple DbContext Classes
The  Solution contains more than one DbContext

PersonDbContext – your application's primary DbContext

HealthChecksDb – a DbContext used by HealthChecksUI (usually automatically configured)

You must specify the correct DbContext when adding migrations or updating the database, to avoid confusion or errors.

🧱 Add Migration & Update Database (PowerShell Example)
To target a specific DbContext (like PersonDbContext) in Package Manager Console (PMC):

```powershell```

Add-Migration MigrationName -Context PersonDbContext

🧱 Update the Database:
```powershell```

Update-Database -Context PersonDbContext

This ensures the migration is added and applied only for the PersonDbContext—not for other contexts like HealthChecksDb.

🌐 If You're Using dotnet CLI Instead of PMC
Use --context:

dotnet ef migrations add MigrationName --context PersonDbContext
dotnet ef database update --context PersonDbContext
⚠️ Important Notes:
If migrations already exist, and you've verified they're for PersonDbContext, you don't need to add a new one.
✅ Just run:

```powershell```
Update-Database -Context PersonDbContext
This applies the existing migration to the database.

If you forget to specify the -Context or --context, EF might pick the wrong one, leading to errors or migration files being created for the wrong DbContext.

✅ Summary
Task	PowerShell Command	dotnet CLI Command
Add Migration	Add-Migration Name -Context PersonDbContext	dotnet ef migrations add Name --context PersonDbContext
Update Database	Update-Database -Context PersonDbContext	dotnet ef database update --context PersonDbContext

### Local Development:
1. Run SQL Server
2. Update appsettings.Development.json
3. Run: dotnet run --project src/Person.Api

## 📚 API Endpoints 

GET    /api/persons
GET    /api/persons/{id}
POST   /api/persons
PUT    /api/persons/{id}
DELETE /api/persons/{id}
GET    /api/persons/persontypes
GET    /health
GET    /health-ui


Update appsettings.Development.json:

Set your SQL Server connection string

Configure OTLP telemetry endpoints

Start the API:

dotnet run --project src/Person.Api

| Method | Endpoint                   | Description           |
| ------ | -------------------------- | --------------------- |
| GET    | `/api/persons`             | List all persons      |
| GET    | `/api/persons/{id}`        | Get person by ID      |
| POST   | `/api/persons`             | Create a new person   |
| PUT    | `/api/persons/{id}`        | Update a person       |
| DELETE | `/api/persons/{id}`        | Delete a person       |
| GET    | `/api/persons/persontypes` | List all person types |
| GET    | `/health`                  | Health check endpoint |
| GET    | `/health-ui`               | Health dashboard UI   |

🛠️ Configuration
Configure via appsettings.json:

SQL Server:

"ConnectionStrings": {
  "DefaultConnection": "Your_SQL_Server_Connection_String"
}


CORS:

"CorsOrigins_PermittedClients": [ "http://localhost:5173" ]
You might  need to change this to address of th front End React App
OpenTelemetry/OTLP:

"Otlp": {
  "Endpoint": "http://localhost:4317",
  "ServiceName": "Person.Api"
}

❤️ Health Checks
Custom health checks for SQL Server

UI Dashboard: /health-ui 

https://localhost:7017/health-ui

Example Output:
{
  "status": "Healthy",
  "checksPoints": [
    {
      "Name": "Person_Health",
      "value": "Healthy",
      "description": "Database connection successful",
      "data": {
        "Provider": "Microsoft.EntityFrameworkCore.SqlServer",
        "ElapsedMilliseconds": 12,
        "Database": "PersonDb",
        "DataSource": "localhost"
      }
    }
  ]
}

### Observability Stack
Tracing: Jaeger + OpenTelemetry
 http://localhost:16686/ 

Metrics: Prometheus + Grafana Dashboards

Logging: Serilog ( + OTLP)

Metrics and log are are found at 
http://localhost:3000/


🧪 Testing
Integration tests powered by Testcontainers for SQL Server

dotnet test
🌱 Database Seeding
On first run: seeds PersonType and sample Person data

Production: seeds only PersonType

🧰 Troubleshooting
Issue	Solution
