# 💵 ExpenseSplitter

🚧 This is still being developed and some features are not yet implemented 🚧

ExpenseSplitter is a Domain-Driven Design (DDD) implemented API, meticulously crafted in C#. This project serves as a vivid showcase of how Clean Architecture priciples can be applied, with an emphasis on creating a robust and scalable solution for managing expenses among users. The API allows users to create settlements, add participants, and manage expenses with flexible splitting options, ensuring a comprehensive expense settlement system.

# 🦩 Features / patterns

1. Bold split into: Domain, Application, Infrastructure and Presentation layers
2. DDD elements: Entities, ValueObjects, DomainEvents, DomainServices
3. Minimal API
4. Swagger
5. Docker support
6. Database migration and seeding
7. Outbox pattern
8. Authentication and Authorization
9. CQRS pattern
10. Command/query validators
11. Result driven communication
12. Unit of work
13. Strongly typed IDs
14. Dependency Injection pattern
15. CI integration
16. Automated unit and integration tests

# 🛹 Model diagram

TODO

# 🛣️ Roadmap

## 🎯 Project general

- Models diagram
- Repository description
- Integrate with Depedabot
- Integrate with CI
- docker support 

## 📃 Domain Layer

- DDD abstractions: Entity, ValueObject, DomainEvent
- Unit of work
- Results and Error
- Define domain model
- Strongly typed IDs
- Expense entity
- ExpenseParticipant entity
- Settlement entity
- Users entity 
- Participant entity
- Unit tests
- Currency support

## 🧑🏻‍💼 Application Layer

- MediatR setup
- ICommand, IQuery, ICommandHandler, IQueryHandler
- Logging for ICommandHandler
- Validators 
- Unit tests

## 🖼️ Presentation Layer

- Minimal API
- Docker support
- Swagger
- Exception handling middleware
- DB seeding
- Authorization

## 🧑🏻‍🔧 Infrastructure Layer

- EF Core - DbContext, Entity mapping
- DB migrations
- Repositories for each Domain entity
- Outbox pattern
- Authentication
- Architecture tests
- Unit tests