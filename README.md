﻿[![Continuous integration](https://github.com/rutkowski-tomasz/ExpenseSplitter/actions/workflows/continuous-integration.yml/badge.svg)](https://github.com/rutkowski-tomasz/ExpenseSplitter/actions/workflows/continuous-integration.yml)

# 💵 ExpenseSplitter

🚧 This is still being developed and some features are not yet implemented 🚧

ExpenseSplitter is a Domain-Driven Design (DDD) implemented API, meticulously crafted in C#. This project serves as a vivid showcase of how Clean Architecture priciples can be applied, with an emphasis on creating a robust and scalable solution for managing expenses among users. The API allows users to create settlements, add participants, and manage expenses with flexible splitting options, ensuring a comprehensive expense settlement system.

# 🦩 Features / patterns

1. Bold split into: Domain, Application, Infrastructure and Presentation layers
2. DDD elements: Entities, ValueObjects, DomainEvents, DomainServices
3. Minimal API with Swagger documentation
4. Docker support with docker-compose orchestration
5. Database migration and seeding
6. Outbox pattern
7. Authentication and Authorization
8. CQRS pattern with custom validators and behaviors
9. Result driven communication
10. Primitive obsession solved, strongly typed IDs
11. Dependency Injection pattern
12. Automated tests with CI Integration

# 🛹 Model diagram

TODO

# 🛣️ Roadmap

## 🎯 Project general

- ✅ Repository description
- ✅ Integrate with Depedabot
- ✅ Continuous integration, badge, code coverage
- ✅ docker & docker-compose support 

## 📃 Domain Layer

- ✅ DDD abstractions: Entity, ValueObject, DomainEvent
- ✅ Model entities: Expense, ExpenseAllocation, Participant, Settlement, User
- ✅ Results and Error
- ✅ Strongly typed IDs
- Solve primitive obsession

## 🧑🏻‍💼 Application Layer

- ✅ MediatR with ICommand, IQuery, ICommandHandler, IQueryHandler abstractions
- ✅ Logging for ICommandHandler
- ✅ Validators with handling middleware

## 🖼️ Presentation Layer

- ✅ Minimal API with Swagger documentation
- 🔄 DB seeding
- ✅ Trace ID middleware

## 🧑🏻‍🔧 Infrastructure Layer

- ✅ EF Core - DbContext, Entity mapping, DB migrations, Repositories
- Outbox pattern
- ✅ Authentication & Authorization
- ✅ Architecture tests

## 📈 Business use-cases

| Status  | Method  | Path   | Notes  |
|---|---|---|---|
| ✅ Done | POST | /user/login | |
| ✅ Done | POST | /user/register | |
| ✅ Done | GET | /user/me | |
| ✅ Done | POST | /settlements | Generate invite code |
| ✅ Done | GET | /settlements | Paging |
| ✅ Done | GET | /settlements/{settlementId} | |
| TODO | PUT | /settlements/{settlementId} | |
| ✅ Done | DELETE | /settlements/{settlementId} | |
| ✅ Done | GET | /settlements/{settlementId}/expenses | Paging |
| ✅ Done | PUT | /settlements/join | |
| ✅ Done | PUT | /settlements/{settlementId}/leave | |
| TODO | GET | /settlements/{settlementId}/reimbrusement | Balances (participant + amount) and reimbrusements (from, to, amount) |
| ✅ Done | PATCH | /settlements/{settlementId}/participants/{participantId}/claim | |
| ✅ Done | POST | /expenses | |
| ✅ Done | GET | /expenses/{expenseId} | |
| TODO | PUT | /expenses/{expenseId} | |
| ✅ Done | DELETE | /expenses/{expenseId} | |

### Notes

1. When dealing with expenses always include allocations
2. When dealing with settlements always include participants
3. Add websockets for real time updates
4. Currency support
5. Cascade delete

# Development

## Create migration

```sh
dotnet ef migrations add --startup-project src/ExpenseSplitter.Api.Presentation --project src/ExpenseSplitter.Api.Infrastructure ...
```
