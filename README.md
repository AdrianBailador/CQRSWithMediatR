### Implementing CQRS with MediatR in .NET 8: A Complete Guide

The CQRS (Command Query Responsibility Segregation) pattern is an architecture that separates the operations of reading (Query) and writing (Command) within a system. This separation allows for each operation to be scaled and optimised independently. In the .NET ecosystem, one of the most popular packages for implementing CQRS is MediatR, which facilitates the mediation of commands and queries.

## What is CQRS?

CQRS (Command Query Responsibility Segregation) is a pattern that separates the operations of state modification (Commands) from the operations of state querying (Queries). This separation facilitates application design and allows for the optimisation of each side independently.

- **Command**: Represents an operation that modifies the application's state (writes data).
- **Query**: Represents an operation that queries the application's state (reads data).

### CQRS Architecture Diagram

Below is a basic diagram illustrating how commands and queries are separated in a CQRS architecture:

![Image description](https://s3.eu-west-1.amazonaws.com/codu.uploads/uploads/clplpfs1h0000pb0162rt87y2/9_NOdNz80jHR8Juj.png)



This diagram shows how write operations (Commands) and read operations (Queries) are handled separately, with MediatR acting as the mediator directing requests to their respective handlers.

## What is MediatR?

MediatR is a .NET library that implements the Mediator pattern, allowing the decoupling of code that sends a message (command or query) from the code that handles it. This is done through dependency injection, helping to maintain cleaner and more modular code.

## Initial Setup in .NET 8

### Step 1: Create a new .NET 8 project

Start by creating a new ASP.NET Core project in .NET 8. You can do this from the command line:

```bash
dotnet new webapi -n CQRSWithMediatR
cd CQRSWithMediatR
```

### Step 2: Add MediatR dependencies

Next, add the MediatR and MediatR.Extensions.Microsoft.DependencyInjection packages to your project:

```bash
dotnet add package MediatR
dotnet add package MediatR.Extensions.Microsoft.DependencyInjection
```

## Implementing Commands and Handlers

### Step 3: Define a Command

A `Command` in CQRS is a request to perform an action that changes the state of the system. Let's create a simple command that creates a new product:

```csharp
using MediatR;

public record CreateProductCommand(string Name, decimal Price) : IRequest<int>;
```

This `CreateProductCommand` has two properties (`Name` and `Price`) and returns an integer (`int`) representing the ID of the created product.

### Step 4: Create a Command Handler

The `Handler` is responsible for handling the business logic associated with a `Command`. Let's implement the handler for `CreateProductCommand`:

```csharp
using MediatR;

public class CreateProductHandler : IRequestHandler<CreateProductCommand, int>
{
    private readonly ApplicationDbContext _context;

    public CreateProductHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Name = request.Name,
            Price = request.Price
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync(cancellationToken);

        return product.Id;
    }
}
```

This handler interacts with the database through `ApplicationDbContext` to create a new product and then returns the ID of the created product.

### Command Validation

To ensure commands are valid before being processed, we can use tools like FluentValidation to apply validation rules:

```bash
dotnet add package FluentValidation
```

Create a validator class:

```csharp
using FluentValidation;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Price).GreaterThan(0);
    }
}
```

Register the validator in the service container:

```csharp
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductCommandValidator>();
```

You can then validate the command before processing it in the handler or by using a behaviour pipeline in MediatR.

### Exception Handling

To centrally manage exceptions, you can implement middleware or use a behaviour pipeline in MediatR:

```csharp
public class ExceptionHandlingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            // Log and handle the exception
            throw;
        }
    }
}
```

Register the behaviour in `Program.cs`:

```csharp
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ExceptionHandlingBehavior<,>));
```

## Implementing Queries and Handlers

### Step 5: Define a Query

Now let's define a query to retrieve a product by its ID:

```csharp
using MediatR;

public record GetProductByIdQuery(int Id) : IRequest<Product>;
```

### Step 6: Create a Query Handler

Let's implement the handler for `GetProductByIdQuery`:

```csharp
using MediatR;
using Microsoft.EntityFrameworkCore;

public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, Product>
{
    private readonly ApplicationDbContext _context;

    public GetProductByIdHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Product> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.Products
                             .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
    }
}
```

### Optimising Queries and Commands

When handling queries and commands in large systems or with high load, it is crucial to optimise performance:

- **Eager Loading and Lazy Loading**: Use `Include` to avoid multiple database queries when you need to load related entities.
- **Pagination**: Implement pagination in queries to avoid loading large volumes of unnecessary data.
- **Projections**: Instead of returning entire entities, project only the necessary fields into DTOs.
- **Caching**: Consider using caching for data that does not change frequently.

### Data Flow Diagram

Here is a diagram showing how a command or query flows through the system:

![Image description](https://dev-to-uploads.s3.amazonaws.com/uploads/articles/9wh42aagwrgzhyabikud.png)

This flow illustrates how a command or query originates from a client, is handled by MediatR, and then processed in a handler that interacts with the database.

## Handling Domain Events

MediatR can also be used to handle domain events. A domain event is something that occurs within the business context that may affect other components of the system.

### Step 7: Define a Domain Event

```csharp
using MediatR;

public record ProductCreatedEvent(int ProductId) : INotification;
```

### Step 8: Create a Domain Event Handler

```csharp
using MediatR;

public class ProductCreatedEventHandler : INotificationHandler<ProductCreatedEvent>
{
    public Task Handle(ProductCreatedEvent notification, CancellationToken cancellationToken)
    {
        // Logic to handle the event, such as sending an email or notifying other services
        return Task.CompletedTask;
    }
}
```

Register the handler in the service container:

```csharp
builder.Services.AddMediatR(typeof(Program).Assembly);
```

Trigger the event from the command handler:

```csharp
public async Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
{
    var product = new Product { Name = request.Name, Price = request.Price };
    _context

.Products.Add(product);
    await _context.SaveChangesAsync(cancellationToken);
    
    await _mediator.Publish(new ProductCreatedEvent(product.Id), cancellationToken);
    
    return product.Id;
}
```

## Unit Testing

### Step 9: Write Unit Tests for Commands and Queries

Unit tests are essential to ensure code quality. Here is an example of how to test a command handler:

```csharp
using Xunit;
using Moq;

public class CreateProductHandlerTests
{
    [Fact]
    public async Task Handle_ShouldCreateProductAndReturnId()
    {
        // Set up an in-memory database context or a mock
        var mockContext = new Mock<ApplicationDbContext>();
        var handler = new CreateProductHandler(mockContext.Object);

        // Act
        var command = new CreateProductCommand("Test Product", 100);
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result > 0);
        mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
```

### Advanced Examples

For more advanced scenarios such as transactions or CQRS in distributed applications, consider:

- **Transactions**: Use the Unit of Work pattern to ensure the atomicity of operations across multiple aggregates.
- **CQRS in Microservices**: If implementing CQRS in a microservices architecture, use an event bus to communicate changes between different services.

## Context for Use

### When to Use CQRS

- **Complex Systems**: CQRS is ideal for systems with complex business rules where separating read and write operations can simplify the design.
- **Scalability**: If you need to scale read and write operations independently, CQRS allows you to optimise each separately.
- **Audit and Security**: The separation of commands and queries makes it easier to implement detailed audit trails and security rules.

### When to Avoid CQRS

- **Simple Applications**: In small projects or simple CRUD applications, CQRS may be over-engineering, adding unnecessary complexity.
- **Maintenance**: If the team is not familiar with CQRS, the learning curve and maintenance effort may outweigh the benefits.

## Conclusion

Implementing CQRS with MediatR in .NET 8 is an effective way to decouple read and write operations in your application, allowing for cleaner, more modular, and scalable code.


