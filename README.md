# Sample CQRS Project

A simple **CQRS (Command Query Responsibility Segregation)** implementation using **ASP.NET Core / .NET 10**, **MediatR**, **Entity Framework Core**, **SQLite**, and **FluentValidation**.

The main goal of this project is to demonstrate how the **Command and Query sides can be separated** in a real-world order management scenario.

> This project is mainly created for learning and demonstrating CQRS concepts in .NET.

---

## 🚀 Tech Stack

* **.NET 10**
* **ASP.NET Core Minimal API**
* **C#**
* **MediatR**
* **Entity Framework Core 10**
* **SQLite**
* **FluentValidation**
* **CQRS**
* **Event-Driven Concepts**
* **Projection / Read Model**

---

## 🧠 What is CQRS?

CQRS stands for:

> **Command Query Responsibility Segregation**

The main idea is to separate operations that **change application state** from operations that **read application state**.

Instead of having one model responsible for both reading and writing, CQRS introduces two different paths:

```text
                Client
                  |
          ┌───────┴───────┐
          |               |
       Command           Query
          |               |
     CommandHandler   QueryHandler
          |               |
       Write DB          Read DB
```

### Command

A Command represents an intention to **change the state** of the system.

For example:

```text
CreateOrderCommand
```

means:

> "I want to create a new order."

The command is sent through MediatR to the appropriate handler.

### Query

A Query represents an intention to **retrieve data** without changing the state of the system.

Examples:

```text
GetOrderByIdQuery
GetOrderSummeriesQuery
```

---

# 🏗️ Architecture

This project separates the **write side** and **read side**.

```text
                         ┌──────────────────┐
                         │      Client      │
                         └────────┬─────────┘
                                  │
                         ASP.NET Core API
                                  │
                     ┌────────────┴────────────┐
                     │                         │
                  COMMAND                    QUERY
                     │                         │
                     ▼                         ▼
          CreateOrderCommand        GetOrderByIdQuery
                     │               GetOrderSummeriesQuery
                     ▼                         │
              MediatR Handler                 │
                     │                         │
                     ▼                         ▼
                WriteDbContext           ReadDbContext
                     │                         │
                     ▼                         ▼
                 WriteDb.db                ReadDb.db
                     │
                     ▼
              Domain/Event Flow
                     │
                     ▼
              OrderCreatedEvent
                     │
                     ▼
                 Projection
                     │
                     ▼
                  Read DB
```

The application registers both `ReadDbContext` and `WriteDbContext` with SQLite, allowing the read and write models to evolve independently.

---

# 📁 Project Structure

```text
SampleCQRS-project/
│
├── Commands/
│   └── CreateOrderCommand.cs
│
├── Queries/
│   ├── GetOrderByIdQuery.cs
│   └── GetOrderSummeriesQuery.cs
│
├── Handlers/
│   ├── CreateOrderCommandHandler.cs
│   ├── GetOrderByIdQueryHandler.cs
│   ├── GetOrderSummeriesQueryHandler.cs
│   ├── ICommandHandler.cs
│   └── IQueryHandler.cs
│
├── Events/
│   ├── OrderCreatedEvent.cs
│   ├── IEventHandler.cs
│   ├── IEventPublisher.cs
│   ├── InProcessEventPublisher.cs
│   └── ConsoleEvenetPublisher.cs
│
├── Projections/
│   └── OrderCreatedProjectionHandler.cs
│
├── Data/
│   ├── AppDbContext.cs
│   ├── ReadDbContext.cs
│   └── WriteDbContext.cs
│
├── Models/
│   └── Order.cs
│
├── Dtos/
│
├── Migrations/
│
├── Program.cs
├── OrdersAPI.csproj
├── OrdersAPI.sln
│
├── ReadDb.db
├── WriteDb.db
└── NOCQRS.db
```

The repository currently contains dedicated folders for Commands, Queries, Handlers, Events, Projections, Data, Models, DTOs and Migrations.

---

# 🔄 Request Flow

## Creating an Order

When a client sends a request to:

```http
POST /api/orders
```

the following flow occurs:

```text
HTTP Request
     │
     ▼
CreateOrderCommand
     │
     ▼
MediatR
     │
     ▼
CreateOrderCommandHandler
     │
     ▼
WriteDbContext
     │
     ▼
WriteDb.db
     │
     ▼
OrderCreatedEvent
     │
     ▼
Projection
     │
     ▼
ReadDbContext
     │
     ▼
ReadDb.db
```

This demonstrates an important CQRS concept:

> The database used for writing does not have to be the same database used for reading.

---

# 📖 Query Flow

For example, when requesting:

```http
GET /api/orders/1
```

the request is converted into:

```text
GetOrderByIdQuery
        │
        ▼
      MediatR
        │
        ▼
GetOrderByIdQueryHandler
        │
        ▼
   ReadDbContext
        │
        ▼
     ReadDb.db
        │
        ▼
     Order DTO
```

The query side does not need to modify the write model.

---

# 📨 Commands

Commands represent operations that modify the state of the application.

Example:

```csharp
public record CreateOrderCommand(
    string CustomerName,
    decimal TotalAmount
);
```

The command is handled by:

```text
CreateOrderCommandHandler
```

The handler is responsible for processing the command and updating the write model.

---

# 🔎 Queries

Queries are responsible only for retrieving information.

Currently the project contains queries such as:

```text
GetOrderByIdQuery
GetOrderSummeriesQuery
```

Each query has its own handler.

This keeps read operations isolated from write operations.

---

# 📢 Events

The project also demonstrates an event-driven approach.

For example:

```text
OrderCreatedEvent
```

represents the fact that an order has been created.

The event can be published and consumed by event handlers.

The repository currently contains an in-process event publisher implementation together with event abstractions.

---

# 📊 Projections

A projection is responsible for transforming events into data suitable for the read side.

For example:

```text
OrderCreatedEvent
        │
        ▼
OrderCreatedProjectionHandler
        │
        ▼
     Read Model
        │
        ▼
     ReadDb.db
```

This allows the read model to be optimized independently from the write model.

---

# 💾 Database Design

This project uses SQLite and separates the database contexts:

```text
WriteDbContext
      │
      ▼
  WriteDb.db


ReadDbContext
      │
      ▼
   ReadDb.db
```

The application configuration registers both contexts independently.

This separation is one of the key ideas demonstrated by the project.

---

# 🛡️ Validation

The project uses **FluentValidation** to validate commands before they are processed.

For example:

```text
CreateOrderCommand
        │
        ▼
CreateOrderCommandValidator
        │
   ┌────┴────┐
   │         │
 Valid     Invalid
   │         │
   ▼         ▼
Handler    400 Bad Request
```

Validation failures are converted into a `400 Bad Request` response by the API.

---

# 🌐 API Endpoints

## Create Order

```http
POST /api/orders
```

Creates a new order.

---

## Get Order

```http
GET /api/orders/{id}
```

Returns an order by its ID.

Example:

```http
GET /api/orders/1
```

---

## Get Orders

```http
GET /api/orders
```

Returns order summaries.

The current API exposes these three endpoints through ASP.NET Core Minimal APIs.

---

# ⚙️ Getting Started

## Prerequisites

Make sure you have the following installed:

* [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
* Git
* Visual Studio Code or another C# IDE

The project targets `net10.0` and uses EF Core 10, MediatR, FluentValidation, and SQLite.

---

## Clone the Repository

```bash
git clone https://github.com/Mehrad44/SampleCQRS-project.git
```

Navigate into the project:

```bash
cd SampleCQRS-project
```

---

## Restore Dependencies

```bash
dotnet restore
```

---

## Build the Project

```bash
dotnet build
```

---

## Run the Application

```bash
dotnet run
```

The application will start on the configured ASP.NET Core development URL.

---

# 🧪 Testing the API

You can use:

* VS Code REST Client
* Postman
* curl
* Swagger, if enabled/configured in the future

Example:

```http
POST /api/orders
Content-Type: application/json

{
    "customerName": "John Doe",
    "totalAmount": 150
}
```

Then retrieve the order:

```http
GET /api/orders/1
```

Or retrieve all order summaries:

```http
GET /api/orders
```

---

# 🧩 Why CQRS?

CQRS can be useful when the read and write workloads have different requirements.

For example:

```text
                 Application
                     │
          ┌──────────┴──────────┐
          │                     │
       WRITE SIDE            READ SIDE
          │                     │
   Complex business         Optimized
       logic                 queries
          │                     │
      Write DB                Read DB
```

This separation can make it easier to:

* Optimize read operations independently
* Scale read and write workloads separately
* Build specialized read models
* Keep business commands isolated
* Introduce event-driven processing
* Evolve the read model independently

However, CQRS is not required for every application. For simple CRUD applications, introducing CQRS can add unnecessary complexity.

---

# 🎯 Learning Goals

This project is intended to demonstrate:

* CQRS fundamentals
* Command and Query separation
* MediatR
* Command Handlers
* Query Handlers
* Event publishing
* Event handlers
* Projections
* Separate read/write DbContexts
* Entity Framework Core
* SQLite
* FluentValidation
* ASP.NET Core Minimal APIs

---

# 🚧 Future Improvements

Possible improvements for this project:

* [ ] Add Swagger / OpenAPI
* [ ] Add unit tests
* [ ] Add integration tests
* [ ] Add Docker support
* [ ] Replace SQLite with SQL Server/PostgreSQL
* [ ] Introduce RabbitMQ
* [ ] Introduce MassTransit
* [ ] Implement a real message broker
* [ ] Improve event publishing architecture
* [ ] Add structured logging
* [ ] Add global exception handling
* [ ] Add authentication and authorization
* [ ] Improve project separation into multiple class libraries

---

# 📚 Concepts Demonstrated

```text
CQRS
 │
 ├── Commands
 │    └── Command Handlers
 │
 ├── Queries
 │    └── Query Handlers
 │
 ├── Events
 │    └── Event Handlers
 │
 ├── Projections
 │
 └── Separate Read / Write Models
```

---

# 👨‍💻 Author

**Mehrad**

GitHub:

https://github.com/Mehrad44

Repository:

https://github.com/Mehrad44/SampleCQRS-project

---

## ⭐ If this project helped you understand CQRS

Feel free to star the repository and use it as a learning reference.

---

## 📄 License

This project is intended for educational and demonstration purposes.
