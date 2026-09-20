# FlowCore

[![CI](https://github.com/Kaganucar/FlowCore/actions/workflows/ci.yml/badge.svg)](https://github.com/Kaganucar/FlowCore/actions/workflows/ci.yml)

An e-commerce backend built with .NET 9 and Clean Architecture, paired with a React storefront.
Built as a personal project to practise production-oriented backend development: layered
architecture, CQRS, JWT authentication, containerisation and CI.

## Stack

| Layer | Technologies |
|---|---|
| Backend | .NET 9, ASP.NET Core Web API, MediatR 14 (CQRS), FluentValidation |
| Data | EF Core 9, PostgreSQL 16, code-first migrations |
| Security | JWT access/refresh tokens, BCrypt password hashing, role-based authorisation |
| Frontend | React 19, Vite, React Router, Tailwind CSS |
| Testing | xUnit, Moq |
| DevOps | Docker (multi-stage), Docker Compose, GitHub Actions |

## Architecture

Four layers with dependencies pointing inwards. `FlowCore.Domain` has no package
references at all, so business rules stay independent of the framework and the database.

```mermaid
flowchart TB
&#x20;   Client["React 19 SPA<br/>flowcore-client"]
&#x20;   API["FlowCore.API<br/>controllers, DI, middleware"]
&#x20;   App["FlowCore.Application<br/>CQRS handlers, DTOs, validators"]
&#x20;   Infra["FlowCore.Infrastructure<br/>EF Core, repositories, JWT"]
&#x20;   Domain["FlowCore.Domain<br/>entities, enums"]
&#x20;   DB[("PostgreSQL 16")]

&#x20;   Client -->|"REST + JWT"| API
&#x20;   API --> App
&#x20;   API --> Infra
&#x20;   Infra --> App
&#x20;   App --> Domain
&#x20;   Infra --> DB
```

## Getting started

Requires Docker Desktop.

```bash
git clone https://github.com/Kaganucar/FlowCore.git
cd FlowCore
cp .env.example .env          # then fill in the two secrets
docker compose up -d --build
```

Generate a signing key for `JWT_KEY` (PowerShell):

```powershell
$bytes = New-Object byte[] 64
[Security.Cryptography.RandomNumberGenerator]::Create().GetBytes($bytes)
[Convert]::ToBase64String($bytes)
```

The API listens on `http://localhost:8080`. Swagger UI is available at
`http://localhost:8080/swagger` in the Development environment only. Migrations are
applied automatically on startup.

Frontend:

```bash
cd flowcore-client
npm install
npm run dev                   # http://localhost:5173
```

## API

All routes are prefixed with `/api`.

| Method | Route | Auth | Description |
|---|---|---|---|
| POST | `/Auth/register` | – | Create an account, returns tokens |
| POST | `/Auth/login` | – | Authenticate, returns tokens |
| POST | `/Auth/refresh` | – | Exchange a refresh token |
| GET | `/Product` | – | List products |
| GET | `/Product/in-stock` | – | List products with stock > 0 |
| GET | `/Product/{id}` | – | Product detail |
| POST | `/Product` | Admin | Create product |
| PUT | `/Product/{id}` | Admin | Update product |
| DELETE | `/Product/{id}` | Admin | Delete product |
| GET | `/Category` | – | List categories |
| GET | `/Category/{id}` | – | Category detail |
| POST | `/Category` | Admin | Create category |
| DELETE | `/Category/{id}` | Admin | Delete category |
| GET | `/Order` | User | List own orders; admins see all |
| GET | `/Order/{id}` | User | Order detail, ownership enforced |
| POST | `/Order` | User | Place an order, decrements stock |
| POST | `/Order/{id}/cancel` | User | Cancel an order, restores stock |

## Design decisions

**CQRS with MediatR.** Every use case is a command or a query with its own handler, kept
in a vertical slice under `Features/{Entity}/{Commands|Queries}/{UseCase}`. Controllers
only dispatch; they contain no business logic.

**Result pattern instead of exceptions for expected failures.** Handlers return
`Result<T>` carrying a status code, so "category not found" is a normal return value
rather than a thrown exception. Unexpected failures still go through a global exception
handler that produces a consistent JSON error shape.

**Specification pattern.** Query logic (filters, includes, ordering) lives in reusable
specification classes instead of being duplicated across repositories.

**Validation as a pipeline behaviour.** `ValidationBehavior<TRequest, TResponse>` runs
every registered FluentValidation validator before a command reaches its handler, so no
handler needs to validate its own input.

**Repository and Unit of Work.** Handlers depend only on `IUnitOfWork`; a single
`SaveChangesAsync` per request keeps an order and its stock decrement in one transaction.

**Prices are never taken from the client.** The order handler reads product prices from
the database, so a tampered request cannot change what the customer is charged.

**Secrets are never committed.** Configuration comes from user-secrets in development
and environment variables in containers; the application fails fast at startup with an
explicit message if either is missing.

## Testing

xUnit with Moq at the handler level; `IUnitOfWork` is mocked so tests need no database.
Coverage is currently limited to the product creation handler — expanding it, including
integration tests against a real database, is the next priority.

Run locally:

```bash
dotnet test FlowCore.sln
```

## Continuous integration

`.github/workflows/ci.yml` runs on every push and pull request to `main`:

- **Backend** — restore, build with warnings treated as errors, run the test suite
- **Frontend** — `npm ci`, ESLint with zero tolerance for warnings, production build

## Known limitations

Deliberately listed rather than hidden; these are the next items on the roadmap.

- No structured logging, health checks or distributed tracing
- No caching layer; list endpoints return the full table without pagination
- Test coverage is thin, with no integration tests
- Stock is read and written without an optimistic concurrency token, so two concurrent
&#x20; orders for the last item can oversell
- No API versioning or rate limiting
- Not deployed; runs locally via Docker Compose only

## Licence

Personal project, no licence granted.
