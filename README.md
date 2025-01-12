# Clean Architecture Template

This document provides an overview of the Clean Architecture Template structure, how to work within it, and how to implement various components using Domain-Driven Design (DDD) principles. This template follows Clean Architecture principles but does not enforce Command Query Responsibility Segregation (CQRS).

## Table of Contents

1. [Overview](#overview)
2. [Project Structure](#project-structure)  
   - [Application Layer](#application-layer)  
   - [Infrastructure Layer](#infrastructure-layer)  
   - [Presentation Layer](#presentation-layer)  
3. [Migrations](#migrations)
4. [Further Reading](#further-reading)

## Overview

### Clean Architecture

Clean Architecture, developed by Robert C. Martin (Uncle Bob), promotes the separation of concerns by organizing code into layers with clear boundaries. This structure enhances system flexibility, maintainability, and scalability.

### Architecture Layers

1. **Domain Layer**: Contains core business logic and rules (Entities, Value Objects, Aggregates).
2. **Application Layer**: Defines application-specific rules and coordinates interactions with the domain.
3. **Infrastructure Layer**: Handles data access, persistence, and external services.
4. **Presentation Layer**: Manages user interaction (APIs, UI components, Controllers).

## Project Structure

```text
/src
 /Application
   - DependencyInjection.cs
   - GlobalUsings.cs
   /Abstractions
   /Services
   /Domain
      /Common
      /DataModels
      /Entities
      /Enums
      /Types
 /Infrastructure
   - DependencyInjection.cs
   - GlobalUsings.cs
   /Database
   /Repositories
   /Implementations
 /Presentation
   - DependencyInjection.cs
   - GlobalUsings.cs
   /Controllers
   /EventHandlers
/tests
  - Tests.Unit
  - Tests.Integration
```

### Application Layer

- **Entities**: Represent business models. Entities have unique identities, while Value Objects do not.

  ```csharp
  public class Customer : Entity
  {
      protected Customer() { }
      public Customer(string name, string email) {
          Name = name;
          Email = email;
      }
      public string Name { get; private set; }
      public string Email { get; private set; }
  }
  ```

- **DataModels**: Data Transfer Objects (DTOs) for communication between layers or services.

  ```csharp
  [Serializable]
  [Method: JsonConstructor]
  public record CustomerDto(string Name, string Email) : IDto, IEventMessage;
  ```

- **Services**: Implements application-specific business logic.

  ```csharp
  public class CustomerService(IUnitOfWork unitOfWork)
  {
      private readonly ICustomerRepository _customerRepository
          = unitOfWork.GetRepository<ICustomerRepository>();
      public void RegisterCustomer(CustomerDto dto)
      {
          var customer = new Customer(dto.Name, dto.Email);
          _customerRepository.Add(customer);
          unitOfWork.SaveChanges();
      }
  }
  ```

- **Abstractions**: Define interfaces for application behavior.

  ```csharp
  public interface IUnitOfWork : IDisposable
  {
      IRepository<T> GetRepository<T>() where T : class;
      void SaveChanges();
  }
  ```

### Infrastructure Layer

- **Database**: Implements data access with an ORM (e.g., Entity Framework Core).

  ```csharp
  internal class PersistenceContext : DbContext, IUnitOfWork
  {
      public DbSet<Customer> Customers { get; set; }
      public void SaveChanges() => base.SaveChanges();
  }
  ```

- **Implementations**: Implement interfaces defined in the application layer.

  ```csharp
  public class EmailQueue : IMailingQueue
  {
      public bool EnqueueEmail(EmailInfoDto emailInfoDto) { }
  }
  ```

- **Repositories**: Provide database access for entities.

  ```csharp
  public class CustomerRepository : ICustomerRepository
  {
      public Customer GetByEmail(string email) { }
  }
  ```

### Presentation Layer

- **Controllers**: Handle HTTP requests and responses.

  ```csharp
  [ApiController]
  [Route("api/[controller]")]
  public class CustomerController(ICustomerService customerService) : ControllerBase
  {
      [HttpPost]
      public IActionResult RegisterCustomer([FromBody] CustomerDto dto)
      {
          customerService.RegisterCustomer(dto);
          return Ok();
      }
  }
  ```

- **EventHandlers**: Handle incoming events from other services.

  ```csharp
  public class CustomerEventHandler(ICustomerService customerService) : IEventHandler<CustomerDto>
  {
      public void Handle(CustomerDto message)
      {
          customerService.RegisterCustomer(message);
      }
  }
  ```

## Migrations

### Using EF Core CLI

- Create a migration:

  ```sh
  dotnet ef migrations add {MigrationName} -p src/Infrastructure -s src/Presentation
  ```

- Apply migrations:

  ```sh
  dotnet ef database update -p src/Infrastructure -s src/Presentation
  ```

### Creating Migration Scripts

- Generate a SQL script:

  ```sh
  dotnet ef migrations script -p src/Infrastructure -s src/Presentation
  ```

- Generate an idempotent script:

  ```sh
  dotnet ef migrations script -i -p src/Infrastructure -s src/Presentation
  ```

### Migration Bundles

- Create a migration bundle:

  ```sh
  dotnet ef migrations bundle -p src/Infrastructure -s src/Presentation -o efbundle
  ```

- Run migration bundle:

  ```sh
  ./efbundle
  ```

## Further Reading

- [Clean Architecture Overview](https://www.freecodecamp.org/news/a-quick-introduction-to-clean-architecture-990c014448d2/)
- [Clean Architecture by Uncle Bob](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Clean Architecture in .NET](https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures)
- [Domain-Driven Design](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/ddd-oriented-microservice)
- [EF Core Migrations](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/applying?tabs=dotnet-core-cli)

This template provides a solid foundation for building applications using Clean Architecture principles in .NET. Modify and extend it based on your project’s needs!

