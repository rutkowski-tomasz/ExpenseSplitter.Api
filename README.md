[![Continuous integration](https://github.com/rutkowski-tomasz/ExpenseSplitter/actions/workflows/continuous-integration.yml/badge.svg)](https://github.com/rutkowski-tomasz/ExpenseSplitter/actions/workflows/continuous-integration.yml)

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

1. ✅ Register - POST /auth/register
2. ✅ Login - POST /auth/login
3. Logout - POST /auth/logout
4. ✅ Create settlement (with participants, generate InviteCode) - POST /settlements
5. ✅ Join settlement - POST /settlements/{settlementId}/join
6. Leave settlement - POST /settlements/{settlementId}/leave
7. ✅ Get all settlements - GET /settlements
8. ✅ Get settlement details (Name, MyTotal, TotalExpenses, InviteCode) - GET /settlements/{settlementId}
9. ✅ Get settlement participants - GET /participants/{settlementId}
10. Modify settlement (Name) - PUT /settlements/{settlementId}
11. ✅ Delete settlement (SettlementId) - DELETE /settlements/{settlementId}
12. Get settlement expenses - GET /settlements/{id}/expenses (paging)
13. Create expense (Title, Type, Amount, Date, PayingParticipantId, ExpenseAllocations [ParticipantId, Amount]) - POST /expenses
14. Modify expense (Title, AMount, Date, PayingParticipantId, ExpenseAllocations [ParticipantId, Amount]) - PUT /settlements/{settlementId}
15. Delete expense (ExpenseId) - DELETE /expenses/{id}
16. Settlement calculate balances (ParticipantName + Amount), reimbrusements (FromParticipantId, FromParticipantName, ToParticipantId, ToParticipantName, Amount) - GET /settlements/{id}/balances
17. Websockets for real time updates
18. Currency support


# Development

## Create migration

```sh
dotnet ef migrations add --startup-project src/ExpenseSplitter.Api.Presentation --project src/ExpenseSplitter.Api.Infrastructure ...
```
