[![Continuous Integration](https://github.com/rutkowski-tomasz/ExpenseSplitter.Api/actions/workflows/ci.yml/badge.svg)](https://github.com/rutkowski-tomasz/ExpenseSplitter.Api/actions/workflows/ci.yml)

# 💵 ExpenseSplitter

ExpenseSplitter is a Domain-Driven Design (DDD) implemented API, meticulously crafted in C#. This project serves as a vivid showcase of how Clean Architecture priciples can be applied, with an emphasis on creating a robust and scalable solution for managing expenses among users. The API allows users to create settlements, add participants, and manage expenses with flexible splitting options, ensuring a comprehensive expense settlement system.

# 🦩 Features / patterns

1. Bold split into: Domain, Application, Infrastructure, and Presentation layers
2. **Domain layer**: Entities, ValueObjects, DomainEvents, Domain Servies, Strongly typed IDs
3. **Application Layer**: CQRS using MediatR with ICommand, IQuery, ICommandHandler, IQueryHandler abstractions, Logging for ICommandHandler, Validators with handling middleware, Result-driven communication, Idempotent/duplicated request handling, Railway-oriented programming (showcase)
4. **Presentation Layer**: Minimal API with Swagger documentation, Trace ID middleware, Rate limiting.
5. **Infrastructure Layer**: EF Core (DbContext, Entity mapping, DB migrations, Repositories), Authentication & Authorization, Database migrations
6. **Testing**: Automated testing in CI, code coverage collection and history comparison, Architecture tests, testcontainers integration testing
7. **CI/CD**: Dependabot integration, auto weekly nuget packages updates, Trivy scanning, SonarQube static-code analysis, automatic semantic-release, docker image publishing
7. Docker support with docker-compose orchestration

# 🛹 Big picture

To let you quickly understand the domain and relationships of the project entities I decided to create a class diagram.

![ExpenseSplitter.Api class diagram](ExpenseSplitter.Diagram.drawio.png "Class diagram")

## 🗺️ Endpoints map

| Method  | Path   | Notes  |
|---|---|---|
| 🟩 POST | /users/login | |
| 🟩 POST | /users/register | |
| 🟦 GET | /users/me | |
| 🟩 POST | /settlements | Generates invite code |
| 🟦 GET | /settlements | |
| 🟦 GET | /settlements/{settlementId} | |
| 🟨 PUT | /settlements/{settlementId} | |
| 🟥 DELETE | /settlements/{settlementId} | |
| 🟦 GET | /settlements/{settlementId}/expenses | |
| 🟨 PUT | /settlements/join | Join using invite code |
| 🟨 PUT | /settlements/{settlementId}/leave | |
| 🟦 GET | /settlements/{settlementId}/reimbursement | Balances and suggested reimbursements |
| 🟪 PATCH | /settlements/{settlementId}/participants/{participantId}/claim | |
| 🟩 POST | /expenses | |
| 🟦 GET | /expenses/{expenseId} | |
| 🟨 PUT | /expenses/{expenseId} | |
| 🟥 DELETE | /expenses/{expenseId} | |

# 🔭 Further development ideas

1. Add websockets for real time updates
2. Currency support
3. Outbox pattern
4. Model diagram

# 👨🏻‍💻 Development

## Start services locally

```sh
docker-compose up -d expensesplitter-db expensesplitter-idp expensesplitter-cache
```

## Create migration

```sh
dotnet ef migrations add --startup-project src/ExpenseSplitter.Api.Presentation --project src/ExpenseSplitter.Api.Infrastructure ...
```
