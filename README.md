# Order Management System

![.NET 8](https://img.shields.io/badge/.NET-8.0-blueviolet)
![Clean Architecture](https://img.shields.io/badge/Architecture-Clean-brightgreen)
![CQRS + MediatR](https://img.shields.io/badge/Patterns-CQRS%20%2B%20MediatR-yellowgreen)

A .NET 8 minimal API for Order Management System (OMS) for a takeaway restaurant that supports the complete lifecycle of an order, including preparation and delivery. 

## Features

1. Customer Features:
    - Place an order by selecting items from a menu (include e.g. customer details, quantities, special instructions etc.). Include a pickup or delivery option.
    - View the status of an order:
        - For Pickup: “Pending”, “Preparing”, “Ready for Pickup.”
        - For Delivery: “Pending”, “Preparing”, “Ready for delivery”, “Out for Delivery,” “Delivered.”

2.	Restaurant Staff Features:
    - View a list of orders with their details and statuses with optional filters (e.g., by status or customer).
    - Update the status of an order:
        - Move orders through preparation stages up to “Ready for Pickup” or “Ready for Delivery.”
        - Cancel an order if it has not yet been prepared.
    -	Assign orders to delivery staff.

3.	Delivery Staff Features:
    - View user delivery assignments (orders marked as “Out for Delivery”).
    - Update the status of an order to “Delivered” or “Unable to Deliver”

4.	Admin Features:
    - Manage menu items (add, update, delete menu items).
    - View some useful statistics (e.g. average order fulfillment time etc.)

5. Dummy Authentication with stateless JWTs (no refresh/logout for simplicity - only userid as "login") and role-based access control (e.g., customer, staff, delivery personnel, admin).

6. Integration with a mapping API (OpenRouteService) to calculate estimated delivery times.

7. Dummy Routing module to automatically assign orders to delivery staff.

8. Rate limiter

## Architecture

| Concern | Tech |
|---|---|
| Architecture style | Clean Architecture + Ports |
| Use-cases | CQRS + MediatR |
| Domain rules | DDD (Entities, Rules) |
| Resilience / Cross-cutting | Exception Middleware, Rate Limiting, FluentValidation |
| Storage | SQL Server + EF Core |
| Tests | xUnit |

---

## Project Structure (high level)

```jsonc
src/
  OrderManagement.API/                # Minimal APIs, middleware, rate limiting, DI
  OrderManagement.Application/        # Commands/Queries (MediatR), validators, ports
  OrderManagement.Contracts/          # DTOs (Requests, Responses), Error format
  OrderManagement.Domain/             # Entities, Domain Exceptions, Rules
  OrderManagement.Infrastructure/     # EF Core, Repositories
tests/
  OrderManagement.UnitTests/          # xUnit: domain rules, handlers
  OrderManagement.IntegrationTests/   # Integration Tests
```

## Running the Service

### Local (HTTP) — easiest setup
Select the **OrderManagement.API (HTTP)** debug profile.

1) Replace API Key of OpenRouteService

\* EF migrations auto-create the database (LocalDB) and tables/data

### Docker / External SQL Server
Steps
1) Uncomment the alternative connection string in `appsettings.Development.json`:

```jsonc
//"ConnectionString": "Server=host.docker.internal,1433;Database=OrderManagement;User Id=ordermanagement_app;Password=StrongPassword123!;Encrypt=True;TrustServerCertificate=True"
```
2) Enable SQL Auth + TCP 1433
3) Execute script:
```sql
CREATE DATABASE OrderManagement;
USE OrderManagement;
CREATE LOGIN ordermanagement_app WITH PASSWORD = 'StrongPassword123!';
CREATE USER  ordermanagement_app FOR LOGIN ordermanagement_app;
EXEC sp_addrolemember N'db_datareader', N'ordermanagement_app';
EXEC sp_addrolemember N'db_datawriter', N'ordermanagement_app';
EXEC sp_addrolemember N'db_ddladmin',   N'ordermanagement_app';
```
4) Replace API Key of OpenRouteService

\* EF migrations auto-create the tables/data

## Assumptions / Future improvement ideas

- If menu/orders grows large, we could add paging parameters. For simplicity it was skipped.
- Customers/Staff management was skipped for simplicity and initialized only with Migrations / Mock data.
- We could add a 2 level cache layer (Memory cache + Redis - Decorator Pattern) for menu or customers/staff for better performance. For simplicity it was skipped.
- It's not an application I would expect lots of exceptions. In case of many exceptions and many RPS Result pattern should be consired. Result pattern leads to long lines of code and I avoid it if not nessasary.
- Refit is a library that could be considered to be used but in the architecture of the project the interface is a port in Application. Application should not know url/header related info.
- The exception model intentionally remains simple. In real-world systems, we might introduce structured error taxonomies, centralized registries, or Roslyn source generators to eliminate reflection and precompute metadata for higher performance.
- JWT tokens should not be stateless and logout/refresh functionalityies should be added. Login should not have only userid as "login".
- Delivery time should be calculated with including the time for preparation somehow. For simplicity it was skipped.