# Order Management System

![.NET 8](https://img.shields.io/badge/.NET-8.0-blueviolet)
![Clean Architecture](https://img.shields.io/badge/Architecture-Clean-brightgreen)
![CQRS + MediatR](https://img.shields.io/badge/Patterns-CQRS%20%2B%20MediatR-yellowgreen)

A .NET 8 minimal API for order management.

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