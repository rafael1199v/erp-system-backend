# ERP System UCB

Microservices-based ERP system built with ASP.NET Core 10 (.NET 10) and PostgreSQL. Consists of three independent services that communicate via HTTP contracts.

---

## Architecture Overview

```
┌─────────────────────────────────────────────────────────┐
│                     ERP System UCB                      │
│                                                         │
│  ┌─────────────────┐    ┌─────────────────┐             │
│  │  Purchasing API │    │    Sales API    │             │
│  │   :5229         │    │   :5074         │             │
│  └────────┬────────┘    └────────┬────────┘             │
│           │ HTTP Contracts       │ HTTP Contracts        │
│           └──────────┬───────────┘                      │
│                      ▼                                   │
│           ┌─────────────────────┐                        │
│           │   Inventory API     │                        │
│           │      :5143          │                        │
│           └─────────────────────┘                        │
│                                                         │
│  ┌──────────────────────────────────────┐               │
│  │           PostgreSQL                 │               │
│  │  Schema: inv | Schema: pur | Schema: sal │           │
│  └──────────────────────────────────────┘               │
└─────────────────────────────────────────────────────────┘
```

**Inventory** is the central service. Purchasing and Sales depend on it to validate products, check stock availability, and consume stock when orders are confirmed or payments are processed.

---

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [PostgreSQL 15+](https://www.postgresql.org/download/)
- [Docker](https://www.docker.com/) *(optional, for containerized deployment)*

---

## Module Summaries

### Inventory System (`erp-inventory-system`)

Manages the product catalog, warehouses, and stock levels. It is the **source of truth** for all product and inventory data in the system.

- Product catalog with categories, units of measure, and statuses
- Multi-warehouse stock tracking (products can exist across multiple warehouses)
- Inventory movements with full transaction history and audit trail
- Dashboard with low-stock analytics
- **Contract endpoints** consumed by Purchasing and Sales for stock validation, stock consumption, product lookups, and company information

### Purchasing System (`erp-purchasing-system`)

Handles the procurement process — creating purchase orders with suppliers and tracking their status.

- Supplier management
- Purchase order creation with line items
- Order confirmation workflow (validates and consumes inventory stock via Inventory contracts)
- Purchase history and status tracking

### Sales System (`erp-sales-system`)

Restaurant point-of-sale (PoS) system for managing table orders, payments, and kitchen operations.

- Table order management (create, update, close tickets)
- Order detail line items with product pricing
- Multiple payment type support with configurable tax rates per company
- PDF generation for receipts and order tickets (QuestPDF)
- Kitchen Display System (KDS) integration for order routing to stations
- Waiter and team management
- Sales dashboard and analytics
- Stock consumption via Inventory contracts when payments are processed

### Inventory Contracts (`Inventory.Contracts`)

Shared class library (not a deployable service) that defines the DTOs and service interfaces used for HTTP communication between Inventory and the other two services. Uses CEN codes (unique business identifiers) instead of database IDs for cross-service references.

---

## Running Locally (Without Docker)

### Step 1 — Set up PostgreSQL

Create a PostgreSQL database (or use an existing one). The three systems use separate schemas (`inv`, `pur`, `sal`) so they can share a single database instance.

```sql
-- Connect to PostgreSQL and create the database
CREATE DATABASE erp_ucb;
```

The schemas are created automatically by Entity Framework migrations on first run.

### Step 2 — Configure each service

Each service reads its connection string from the `ConnectionStrings:DefaultConnection` configuration key. Set it via environment variable or `appsettings.Development.json`.

**Option A — Environment variable (recommended):**

```bash
# PowerShell
$env:ConnectionStrings__DefaultConnection = "Host=localhost;Port=5432;Database=erp_ucb;Username=postgres;Password=yourpassword"
```

```bash
# Bash / WSL
export ConnectionStrings__DefaultConnection="Host=localhost;Port=5432;Database=erp_ucb;Username=postgres;Password=yourpassword"
```

> Note: ASP.NET Core maps `__` (double underscore) to `:` in environment variable names.

**Option B — `appsettings.Development.json`:**

Add the following to each system's `appsettings.Development.json` (inside `Inventory.API/`, `Purchasing.API/`, `Sales.API/`):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=erp_ucb;Username=postgres;Password=yourpassword"
  }
}
```

### Step 3 — Apply database migrations

Run from the repository root:

```bash
# Inventory
dotnet ef database update --project erp-inventory-system/Erp.Inventory.Infrastructure --startup-project erp-inventory-system/Inventory.API

# Purchasing
dotnet ef database update --project erp-purchasing-system/Erp.Purchasing.Infrastructure --startup-project erp-purchasing-system/Purchasing.API

# Sales
dotnet ef database update --project erp-sales-system/Erp.Sales.Infrastructure --startup-project erp-sales-system/Sales.API
```

> If `dotnet-ef` is not installed: `dotnet tool install --global dotnet-ef`

### Step 4 — Start the services

Start **Inventory first** — the other two services depend on it.

```bash
# Terminal 1 — Inventory (must start first)
dotnet run --project erp-inventory-system/Inventory.API

# Terminal 2 — Purchasing
dotnet run --project erp-purchasing-system/Purchasing.API

# Terminal 3 — Sales
dotnet run --project erp-sales-system/Sales.API
```

---

## Running with Docker

Each service has its own `Dockerfile` and `compose.yaml`. Use them individually or build all images manually.

### Build and run individually

```bash
# Inventory
cd erp-inventory-system
docker compose up --build

# Purchasing (in a new terminal)
cd erp-purchasing-system
docker compose up --build

# Sales (in a new terminal)
cd erp-sales-system
docker compose up --build
```

When running in Docker, override the environment variables so the services can reach each other and the database:

```bash
docker run \
  -e ConnectionStrings__DefaultConnection="Host=host.docker.internal;Port=5432;Database=erp_ucb;Username=postgres;Password=yourpassword" \
  -e Modules__InventoryBaseUrl="http://host.docker.internal:5143" \
  -p 5143:8080 \
  erp-inventory-system
```

> Use `host.docker.internal` to reach services running on the host machine from inside a container (Windows/Mac). On Linux, use the host's actual IP or configure a Docker network.

---

## Environment Variables Reference

| Variable | Required By | Description | Default |
|---|---|---|---|
| `ConnectionStrings__DefaultConnection` | All three | PostgreSQL connection string | *(none — must be set)* |
| `Modules__InventoryBaseUrl` | Purchasing, Sales | Base URL of the Inventory API | `http://localhost:5143` |
| `ASPNETCORE_ENVIRONMENT` | All three | Runtime environment | `Production` |

### Connection string format

```
Host=<host>;Port=<port>;Database=<dbname>;Username=<user>;Password=<password>
```

Example for local development:

```
Host=localhost;Port=5432;Database=erp_ucb;Username=postgres;Password=secret
```

### Important considerations

- The **Inventory service must be reachable** before starting Purchasing or Sales. If `Modules__InventoryBaseUrl` points to an unreachable host, stock validation and product lookup calls will fail at runtime (not at startup).
- All three services use **CORS `AllowAnyOrigin`** — suitable for development; restrict in production.
- The Sales API enables **HTTPS redirect** by default. Disable it or configure a valid certificate for local HTTP-only setups.
- **Schemas are isolated**: `inv` (Inventory), `pur` (Purchasing), `sal` (Sales). Running migrations on a shared database is safe — they will not conflict.
- The Sales system uses **QuestPDF Community license** for PDF generation. No additional license key is required for non-commercial use.

---

## API Documentation (Swagger UI)

All three services expose a Swagger UI when running in the `Development` environment. Navigate to the root path after starting each service:

| Service | URL |
|---|---|
| Inventory API | http://localhost:5143/swagger |
| Purchasing API | http://localhost:5229/swagger |
| Sales API | http://localhost:5074/swagger |

The raw OpenAPI JSON spec is available at `/openapi/v1.json` for each service (e.g., http://localhost:5143/openapi/v1.json).

> Swagger UI is **only enabled when `ASPNETCORE_ENVIRONMENT=Development`**. Set this environment variable if it is not already set:
>
> ```bash
> # PowerShell
> $env:ASPNETCORE_ENVIRONMENT = "Development"
>
> # Bash
> export ASPNETCORE_ENVIRONMENT=Development
> ```

---

## Default Ports

| Service | HTTP | HTTPS |
|---|---|---|
| Inventory API | 5143 | 7019 |
| Purchasing API | 5229 | 7252 |
| Sales API | 5074 | 7278 |
| Docker (all services) | 8080 | 8081 |

---

## Project Structure

```
erp-system-ucb/
├── erp-system.slnx                  # Solution file
├── Directory.Packages.props         # Centralized NuGet versions
├── README.md
│
├── Inventory.Contracts/             # Shared DTOs and interfaces (not a service)
│   └── IInventoryService.cs         # Contract interface for HTTP clients
│
├── erp-inventory-system/
│   ├── Inventory.API/               # Entry point — port 5143
│   ├── Erp.Inventory.Domain/        # Entities and domain logic
│   ├── Erp.Inventory.Application/   # Use cases and application services
│   ├── Erp.Inventory.Infrastructure/# EF Core, DbContext, repositories
│   └── Erp.Inventory.Presentation/  # Controllers and DI registration
│
├── erp-purchasing-system/
│   ├── Purchasing.API/              # Entry point — port 5229
│   ├── Erp.Purchasing.Domain/
│   ├── Erp.Purchasing.Application/
│   ├── Erp.Purchasing.Infrastructure/
│   └── Erp.Purchasing.Presentation/
│
└── erp-sales-system/
    ├── Sales.API/                   # Entry point — port 5074
    ├── Erp.Sales.Domain/
    ├── Erp.Sales.Application/
    ├── Erp.Sales.Infrastructure/
    └── Erp.Sales.Presentation/
```

---

## Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core 10.0 |
| ORM | Entity Framework Core 10.0 |
| Database | PostgreSQL (Npgsql driver) |
| API Documentation | OpenAPI + Swashbuckle Swagger UI |
| PDF Generation | QuestPDF 2026.x (Community license) |
| Architecture | Clean Architecture (Domain / Application / Infrastructure / Presentation) |
