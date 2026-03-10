# MoneyBee

MoneyBee is a modular .NET 8 money transfer system built with a layered service architecture.

The solution currently contains:

- **Auth Service**
- **Customer Service**
- **Transfer Service**
- **API Gateway**

The system supports:

- authentication and JWT issuance
- customer management
- money transfer creation
- transfer pickup/completion
- transfer cancellation
- transfer history
- idempotent create operations
- soft delete
- gateway-based routing
- unit, integration, and architecture tests

---

## 1. Architecture Overview

The solution follows a layered modular architecture with clear separation of concerns.

Each service is split into projects such as:

- `Abstraction`
- `API`
- `Application`
- `Contracts`
- `Domain`
- `Infrastructure`
- `Repository`
- `UseCase`

Typical flow:

```text
HTTP Request
  -> API Controller
  -> Application Request Handler
  -> UseCase
  -> Repository / Infrastructure Adapters
  -> Database / External Service
````

### Design principles

* **Ports & Adapters / Hexagonal style**
* **UseCase-centered business logic**
* **Repository abstraction**
* **Shared building blocks for common concerns**
* **Gateway-based ingress**
* **Testable modular boundaries**
* **Soft delete**
* **Idempotency**
* **Error code standardization**
* **Optimistic concurrency where needed**
* **Locking where aggregate race conditions matter**

---

## 2. Solution Structure

Example high-level structure:

```text
src/
  Shared/
    MoneyBee.Shared.API
    MoneyBee.Shared.Application
    MoneyBee.Shared.Core

  Services/
    Auth/
      MoneyBee.Auth.API
      MoneyBee.Auth.Application
      MoneyBee.Auth.UseCase
      MoneyBee.Auth.Abstraction
      MoneyBee.Auth.Infrastructure
      MoneyBee.Auth.Repository
      MoneyBee.Auth.Domain
      MoneyBee.Auth.Contracts

    Customer/
      MoneyBee.Customer.API
      MoneyBee.Customer.Application
      MoneyBee.Customer.UseCase
      MoneyBee.Customer.Abstraction
      MoneyBee.Customer.Infrastructure
      MoneyBee.Customer.Repository
      MoneyBee.Customer.Domain
      MoneyBee.Customer.Contracts

    Transfer/
      MoneyBee.Transfer.API
      MoneyBee.Transfer.Application
      MoneyBee.Transfer.UseCase
      MoneyBee.Transfer.Abstraction
      MoneyBee.Transfer.Infrastructure
      MoneyBee.Transfer.Repository
      MoneyBee.Transfer.Domain
      MoneyBee.Transfer.Contracts

  Gateways/
    MoneyBee.ApiGateway

tests/
  MoneyBee.Application.UnitTests
  MoneyBee.ArchitectureTests
  MoneyBee.Auth.UnitTests
  MoneyBee.Customer.UnitTests
  MoneyBee.Transfer.UnitTests
  MoneyBee.IntegrationTests
```

---

## 3. Services

## 3.1 Auth Service

Responsible for:

* login
* JWT token generation
* authentication-related business rules

Typical output:

* access token
* token metadata

---

## 3.2 Customer Service

Responsible for:

* create customer
* update customer
* delete customer (soft delete)
* get customer
* search customer
* internal summary endpoint for transfer flow

---

## 3.3 Transfer Service

Responsible for:

* create transfer
* get transfer by code
* complete transfer
* cancel transfer
* transfer history

Important business protections:

* sender daily limit check
* sender/day scoped create lock
* idempotent transfer creation
* receiver validation on completion
* row version / concurrency protection on complete/cancel
* soft delete filtering

---

## 3.4 API Gateway

Responsible for:

* single public entry point
* forwarding requests to services
* JWT validation at gateway level
* route-based authorization

Routing examples:

* `/gateway/api/auth/**`
* `/gateway/api/customers/**`
* `/gateway/api/transfers/**`

---

## 4. Shared

## 4.1 MoneyBee.Shared.Core

Contains:

* exception base classes
* shared domain-oriented primitives
* error codes

## 4.2 MoneyBee.Shared.Application

Contains:

* base request types
* base request handlers
* base use case abstractions

## 4.3 MoneyBee.Shared.API

Contains:

* centralized exception middleware
* common HTTP error response model
* migrate database on startup helper

---

## 5. Key Technical Features

### Soft Delete

Entities use soft delete instead of physical deletion.

Typical fields:

* `Active`
* `DeletedAtUtc`

EF global query filters ensure deleted rows are excluded by default.

---

### Use Case Level Caching

The system supports **use case-level caching** through a decorator-based approach.

Caching is applied at the `IUseCase<TIn, TOut>` boundary rather than at controller or gateway level.

This provides the following benefits:

- caching remains close to business intent
- query use cases can be cached without polluting controller code
- non-cacheable use cases remain unaffected
- caching can be enabled centrally through dependency injection

Caching is enabled through a `[Caching(...)]` attribute placed on eligible use case `Execute(...)` methods.

Example:

```csharp
[Caching(60, Prefix = "customer-by-id", VaryByProperties = [ nameof(GetCustomerByIdUseCaseInput.Id) ])]
public override Task<GetCustomerByIdUseCaseOutput> Execute(CancellationToken cancellationToken = default)
{
    return base.Execute(cancellationToken);
}
```

---

### Idempotency

`CreateTransfer` supports idempotency using the `Idempotency-Key` request header.

Behavior:

* same key + same payload -> returns previously stored response
* same key + different payload -> returns conflict

---

### Concurrency

Transfer completion and cancellation are protected with optimistic concurrency.

This prevents invalid state transitions under concurrent requests.

---

### Locking

Transfer creation uses sender-scoped locking to avoid daily-limit race conditions.

This protects the following pattern:

* read today’s transfers
* validate limit
* create transfer

without concurrent double-approval issues.

---

### Error Codes

Errors return both human-readable messages and machine-readable codes.

Example codes:

* `validation_error`
* `transfer_not_found`
* `daily_limit_exceeded`
* `receiver_customer_mismatch`
* `idempotency_key_payload_mismatch`
* `concurrency_conflict`

---

## 6. Prerequisites

To run locally you need:

* .NET 8 SDK
* PostgreSQL
* Docker Desktop (optional, recommended)
* Visual Studio / Rider / VS Code

Optional:

* pgAdmin or another PostgreSQL client

---

## 7. Configuration

Configuration is loaded from:

* `appsettings.json`
* `appsettings.{Environment}.json`
* environment variables

Environment variables override appsettings values.

Typical settings include:

* connection strings
* JWT issuer/audience/secret
* downstream service base URLs
* reverse proxy destination URLs

---

## 8. Running with Docker

Docker support is provided for:

* Auth API
* Customer API
* Transfer API
* API Gateway
* PostgreSQL

## Start the full system

```bash
docker compose up --build
```

## Exposed ports

* Gateway: `http://localhost:8080`
* Auth API: `http://localhost:7001`
* Customer API: `http://localhost:7002`
* Transfer API: `http://localhost:7003`
* PostgreSQL: `localhost:5432`

---

## 9. Running Locally Without Docker

### Step 1: Start PostgreSQL

Make sure PostgreSQL is running and databases are available.

Suggested databases:

* `moneybee_auth_db`
* `moneybee_customer_db`
* `moneybee_transfer_db`

### Step 2: Configure connection strings

Update each service’s `appsettings.Development.json` or use environment variables.

Examples:

```json
{
  "ConnectionStrings": {
    "AuthDb": "Host=localhost;Port=5432;Database=moneybee_auth_db;Username=postgres;Password=postgres"
  }
}
```

```json
{
  "ConnectionStrings": {
    "CustomerDb": "Host=localhost;Port=5432;Database=moneybee_customer_db;Username=postgres;Password=postgres"
  }
}
```

```json
{
  "ConnectionStrings": {
    "TransferDb": "Host=localhost;Port=5432;Database=moneybee_transfer_db;Username=postgres;Password=postgres"
  },
  "CustomerService": {
    "BaseUrl": "https://localhost:7002"
  }
}
```

### Step 3: Configure JWT

All services and gateway must share consistent JWT settings:

* `Issuer`
* `Audience`
* `SecretKey`

### Step 4: Start services in order

Recommended order:

1. Auth API
2. Customer API
3. Transfer API
4. API Gateway

---

## 10. Database / Migration

If migrations are used, run them per service repository project.

Typical commands look like:

### Auth

```bash
dotnet ef database update \
  --project src/Services/Auth/MoneyBee.Auth.Repository/MoneyBee.Auth.Repository.csproj \
  --startup-project src/Services/Auth/MoneyBee.Auth.API/MoneyBee.Auth.API.csproj
```

### Customer

```bash
dotnet ef database update \
  --project src/Services/Customer/MoneyBee.Customer.Repository/MoneyBee.Customer.Repository.csproj \
  --startup-project src/Services/Customer/MoneyBee.Customer.API/MoneyBee.Customer.API.csproj
```

### Transfer

```bash
dotnet ef database update \
  --project src/Services/Transfer/MoneyBee.Transfer.Repository/MoneyBee.Transfer.Repository.csproj \
  --startup-project src/Services/Transfer/MoneyBee.Transfer.API/MoneyBee.Transfer.API.csproj
```

If startup auto-migration is enabled in development, manual migration may not be necessary.

---

## 11. Authentication Flow

1. Login via Auth Service
2. Receive JWT
3. Use JWT in `Authorization: Bearer <token>` header
4. Access protected Customer and Transfer endpoints directly or via Gateway

---

## 12. Gateway Routes

Examples:

### Auth

```http
POST /gateway/api/auth/login
```

### Customer

```http
GET /gateway/api/customers/{id}
POST /gateway/api/customers
```

### Transfer

```http
POST /gateway/api/transfers
GET /gateway/api/transfers/by-code/{transactionCode}
POST /gateway/api/transfers/complete
POST /gateway/api/transfers/cancel
GET /gateway/api/transfers/history?customerId={id}&role=sender
```

---

## 13. Sample API Usage

## 13.1 Login

```http
POST /gateway/api/auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "123456"
}
```

Response includes a JWT access token.

---

## 13.2 Create Customer

```http
POST /gateway/api/customers
Authorization: Bearer <token>
Content-Type: application/json

{
  "firstName": "Ali",
  "lastName": "Veli",
  "phoneNumber": "5551112233",
  "address": "Istanbul",
  "dateOfBirth": "1990-01-01",
  "identityNumber": "12345678901"
}
```

---

## 13.3 Create Transfer

```http
POST /gateway/api/transfers
Authorization: Bearer <token>
Idempotency-Key: create-transfer-001
Content-Type: application/json

{
  "senderCustomerId": "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",
  "receiverCustomerId": "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb",
  "amount": 500
}
```

---

## 13.4 Complete Transfer

```http
POST /gateway/api/transfers/complete
Authorization: Bearer <token>
Content-Type: application/json

{
  "transactionCode": "TRX-20260101010101000-123456",
  "receiverCustomerId": "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"
}
```

---

## 13.5 Cancel Transfer

```http
POST /gateway/api/transfers/cancel
Authorization: Bearer <token>
Content-Type: application/json

{
  "transactionCode": "TRX-20260101010101000-123456"
}
```

---

## 14. Tests

### Run all tests

```bash
dotnet test
```

### Run transfer unit tests

```bash
dotnet test tests/MoneyBee.Transfer.UnitTests/MoneyBee.Transfer.UnitTests.csproj
```

### Run integration tests

```bash
dotnet test tests/MoneyBee.IntegrationTests/MoneyBee.IntegrationTests.csproj
```

### Run architecture tests

```bash
dotnet test tests/MoneyBee.ArchitectureTests/MoneyBee.ArchitectureTests.csproj
```

---

## 15. Important Business Rules

### Transfer creation

* sender and receiver must exist
* sender and receiver cannot be the same
* amount must be positive
* daily limit is enforced
* create is idempotent
* sender/day race condition is protected by lock

### Transfer completion

* transaction must exist
* receiver must match
* completed transfer cannot be completed again
* cancelled transfer cannot be completed
* concurrency conflict is protected

### Transfer cancellation

* transaction must exist
* completed transfer cannot be cancelled
* cancelled transfer cannot be cancelled again
* concurrency conflict is protected

### Customer deletion

* customer deletion is soft delete
* deleted entities are excluded by default query filters

---

## 16. Caching

Caching is implemented at the **use case level** using decorator.

### Current strategy

- cache is applied only to selected **read/query use cases**
- cache is not applied to write/command use cases
- caching is activated by the `[Caching]` attribute
- cache keys are generated from selected input values
- current default implementation uses an in-memory key-value store

### Recommended cache candidates

Examples of good cache candidates:

- `GetCustomerByIdUseCase`
- internal customer summary query use cases
- read-only lookup style queries

Examples of operations that should **not** be cached:

- `CreateTransfer`
- `CompleteTransfer`
- `CancelTransfer`
- login / token issuance

### Cache registration

Caching is registered centrally through dependency injection:

```csharp
services.AddUseCaseCaching();
```

## 17. Error Handling

All services use centralized exception middleware.

Error response shape:

```json
{
  "message": "Daily transfer limit exceeded.",
  "errorCode": "daily_limit_exceeded",
  "statusCode": 400,
  "correlationId": "0HMP123ABC",
  "detail": null
}
```

---
## 18. Testing

The solution uses multiple testing layers to validate business rules, API behavior, infrastructure integration, gateway routing, and end-to-end consumer scenarios.

The testing strategy is intentionally split into several levels so that each concern can be validated with the appropriate balance of speed, isolation, and realism.

### 18.1 Unit Tests

Unit tests verify isolated business logic without requiring real infrastructure.

These tests are used for fast feedback and typically cover:

* domain rules
* use case logic
* validation behavior
* idempotency behavior
* concurrency-related business rules
* error code expectations
* mapper or handler-level logic where external dependencies are mocked or faked

Unit tests are expected to be:

* fast
* deterministic
* independent from Docker, PostgreSQL, or external services

Use unit tests during daily development to validate core logic quickly.

To run all automated tests:

```bash
dotnet test
```

If unit tests are separated into dedicated projects, they can also be run individually by targeting the relevant test project file.

---

### 18.2 Integration Tests

Integration tests validate real application behavior using the full HTTP pipeline, EF Core, PostgreSQL, and service configuration.

These tests are intended to verify that the application behaves correctly when the main runtime components work together.

Integration coverage includes:

* Auth API behavior
* Customer API behavior
* Transfer API behavior
* request/response contracts
* persistence behavior
* migrations
* seeded test data behavior where applicable
* API authentication and authorization behavior
* idempotency and transaction lifecycle flows

Integration tests use [Testcontainers-based](https://github.com/testcontainers/testcontainers-dotnet) PostgreSQL instances and are designed to exercise real infrastructure rather than mocks wherever practical.

At the service level, the following test boundaries apply:

* **Auth integration tests** validate authentication endpoints, seeded employees, token generation, and authenticated profile access
* **Customer integration tests** validate customer creation, retrieval, update, delete, search, and validation errors
* **Transfer integration tests** validate transfer creation, completion, cancellation, history, daily limits, and idempotency behavior

These tests are slower than unit tests but provide significantly stronger confidence in service correctness.

---

### 18.3 Gateway Integration Tests

Gateway integration tests validate the API Gateway separately from the business services.

These tests ensure that the gateway correctly proxies requests to downstream services and enforces the expected routing and authorization behavior.

Coverage includes:

* YARP route forwarding
* request path transformation
* authorization forwarding
* downstream proxy behavior
* gateway-level authentication and unauthorized scenarios

Gateway tests are especially important after any change involving:

* proxy routes
* downstream service addresses
* JWT validation
* forwarding headers
* gateway authentication hardening

These tests help ensure that requests sent to the gateway reach the correct downstream services with the expected headers and route mappings.

---

### 18.4 Postman Scenario Tests

Postman scenario tests complement the automated test projects by validating end-to-end flows from an API consumer perspective.

They are intended for:

* manual verification
* QA checks
* demo flows
* exploratory validation
* end-to-end scenario execution
* regression checks for happy paths and edge cases

The repository includes Postman [collections and environment files](https://github.com/bcrona-ogr/moneybee/tree/main/tests/postman) for validating both happy paths and error paths across the MoneyBee services.

#### Available Collections

* `MoneyBee.postman_collection`

#### Available Environment

* `MoneyBee.local.postman_environment.json`


These files are intended for local execution against the Docker Compose environment.

#### Covered Scenarios

The Postman collections include scenario folders such as:

* `Scenario - Transfer Happy Path`
* `Scenario - Transfer Cancel Path`
* `Scenario - Transfer Idempotency`
* `Scenario - Transfer Already Completed / Completed Cancel Rules`
* `Scenario - Transfer Cancelled / Cannot Complete Cancelled`
* `Edge Cases - Customer`
* `Edge Cases - Transfer`

#### Assertions

The assertion-enabled collection includes response checks for expected status codes such as:

* `200`
* `204`
* `400`
* `401`
* `404`
* `409`
* `422`

This makes the collection suitable for use with Postman Runner as a lightweight scenario-based validation suite.

#### Multi-Iteration Safety

The multi-iteration safe collection is designed for repeated Runner execution.

It includes scenario initialization scripts that:

* reset transient collection variables
* generate unique customer identity numbers per iteration
* generate unique idempotency keys per iteration
* preserve deterministic duplicate and idempotency behavior within the same iteration

This reduces collisions between iterations and improves repeatability.

#### Recommended Usage

For best results:

1. start the full local system
2. import the environment file
3. import the collection file
4. select the local environment
5. run one scenario folder at a time

Recommended manual execution order:

1. `Scenario - Transfer Happy Path`
2. `Scenario - Transfer Idempotency`
3. `Scenario - Transfer Already Completed / Completed Cancel Rules`
4. `Scenario - Transfer Cancelled / Cannot Complete Cancelled`
5. `Edge Cases - Customer`
6. `Edge Cases - Transfer`

#### Postman Runner

The collections are designed to work with Postman Runner.

Recommended Runner settings:

* select a single scenario folder instead of the full collection
* use `Iterations = 1` for standard manual validation
* use the multi-iteration safe collection for repeated executions
* keep the correct local environment selected before starting the run

---

### 18.5 Running Tests Locally

A typical local validation flow is:

1. start all services and infrastructure
2. run automated tests
3. run Postman scenario folders as needed

Start the local system with Docker Compose:

```bash
docker compose up --build
```

Run the automated test suite:

```bash
dotnet test
```

Then use Postman Runner or manual Postman execution for scenario-based validation.

For end-to-end flow validation, the recommended sequence is:

* run the gateway and downstream services
* verify login
* create sender and receiver customers
* create a transfer
* complete or cancel the transfer depending on the scenario
* validate history or error outcomes

---

### 18.6 Prerequisites

Before running integration or scenario tests, ensure that:

* Docker Desktop is running
* Docker Compose services are up and healthy
* PostgreSQL containers are available
* the gateway is reachable through the configured local port
* downstream services are reachable from the gateway
* the selected Postman environment matches the local runtime ports

If gateway requests return `502 Bad Gateway`, first verify downstream service availability and gateway downstream address configuration.

---

### 18.7 Testing Responsibilities

At a high level, each testing layer has a distinct purpose:

* **Unit tests** verify isolated business logic
* **Integration tests** verify real service behavior with real persistence
* **Gateway integration tests** verify routing and forwarding behavior
* **Postman scenario tests** verify consumer-facing end-to-end flows

This separation helps keep the test strategy both fast and reliable:

* unit tests provide fast feedback during development
* integration tests validate correctness at the service level
* gateway tests validate routing infrastructure
* Postman scenarios validate complete consumer journeys

---
