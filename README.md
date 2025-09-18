# Vehicle Rental System

A comprehensive .NET 9.0 backend application for managing motorcycle rentals and delivery persons, built as part of the Mottu backend challenge. This project demonstrates modern software engineering practices, design patterns, architectural principles, and comprehensive testing strategies.

## 🏗️ Architecture Overview

The application follows **Clean Architecture** principles with clear separation of concerns across multiple layers, implementing **Domain-Driven Design (DDD)** patterns and **CQRS (Command Query Responsibility Segregation)** architecture with **MediatR**:

```
┌─────────────────────────────────────────────────────────────┐
│                    API Layer (Controllers)                 │
│  • HTTP endpoints & routing                                │
│  • Request/response handling                               │
│  • Middleware pipeline (Auth, Error, Logging)             │
│  • File upload management                                  │
│  • Swagger documentation                                   │
├─────────────────────────────────────────────────────────────┤
│                Application Layer (CQRS)                    │
│  • Command Handlers (Write operations)                     │
│  • Query Handlers (Read operations)                       │
│  • MediatR pipeline behaviors                              │
│  • Business logic orchestration                            │
│  • Message publishing/consuming                           │
│  • JWT token management                                   │
│  • Result pattern for error handling                      │
├─────────────────────────────────────────────────────────────┤
│                  Domain Layer (Entities)                   │
│  • Rich domain models with business logic                 │
│  • Domain interfaces and contracts                         │
│  • Business rules and validation                           │
│  • Domain events and aggregates                           │
│  • Value objects and enums                                │
├─────────────────────────────────────────────────────────────┤
│                Data Layer (Repositories)                   │
│  • Repository implementations                              │
│  • Entity Framework configurations                        │
│  • Database context and migrations                        │
│  • Data persistence strategies                            │
└─────────────────────────────────────────────────────────────┘
```

### **Architecture Principles**

- **Dependency Inversion**: Inner layers don't depend on outer layers
- **Separation of Concerns**: Each layer has distinct, well-defined responsibilities
- **Single Responsibility**: Each class/module has one reason to change
- **Interface Segregation**: Clients depend only on interfaces they use
- **Open/Closed**: Open for extension, closed for modification

### **Layer Responsibilities**

#### **API Layer** (`vehicle-rental.api`)
- **Controllers**: Handle HTTP requests/responses, route mapping
- **Middleware**: Cross-cutting concerns (authentication, error handling, logging)
- **Validators**: Input validation using FluentValidation
- **Models**: Request/response DTOs and validation models
- **Configuration**: Dependency injection, service registration

#### **Application Layer** (`vehicle-rental.application`)
- **Services**: Business logic orchestration and coordination
- **CQRS Handlers**: Command and query handlers for each domain feature
- **Pipeline Behaviors**: Cross-cutting concerns (validation, logging)
- **Messaging**: RabbitMQ message publishing and consuming
- **Authentication**: JWT token generation and user management
- **Mappers**: Object-to-object transformation logic
- **Result Pattern**: Consistent error handling across all operations

#### **Domain Layer** (`vehicle-rental.domain`)
- **Entities**: Rich domain models with encapsulated business logic
- **Interfaces**: Domain contracts and abstractions
- **Enums**: Domain-specific enumerations
- **DTOs**: Data transfer objects for domain operations
- **Business Rules**: Core business logic and validation rules

#### **Data Layer** (`vehicle-rental.data.postgresql`)
- **Repositories**: Data access implementations
- **Context**: Entity Framework database context
- **Configurations**: Entity mapping configurations
- **Migrations**: Database schema management

## 🎯 SOLID Principles Implementation

### 1. **Single Responsibility Principle (SRP)**
Each class has a single, well-defined responsibility:

- **Controllers**: Handle HTTP requests/responses only
- **Services**: Implement business logic for specific domains
- **Repositories**: Manage data access for specific entities
- **Entities**: Represent domain concepts with business behavior
- **Message Publishers/Consumers**: Handle specific messaging operations

### 2. **Open/Closed Principle (OCP)**
Classes are open for extension but closed for modification:

- **BaseRepository<T>**: Extensible through inheritance
- **ErrorHandlingMiddleware**: Extensible for new exception types
- **Entity Configurations**: Extensible through IEntityTypeConfiguration
- **Rental Calculation Logic**: Extensible for new rental plans

### 3. **Liskov Substitution Principle (LSP)**
Derived classes can replace their base classes:

- **Repository implementations** can replace their interfaces
- **Service implementations** can replace their interfaces
- **BaseRepository** can be substituted by any specific repository
- **Message Publisher/Consumer** implementations are interchangeable

### 4. **Interface Segregation Principle (ISP)**
Clients shouldn't depend on interfaces they don't use:

- **IRepository<T>**: Generic base interface
- **IMotorcycleRepository**: Specific motorcycle operations
- **IMessagePublisher**: Only publishing operations
- **IMessageConsumer**: Only consuming operations
- **IService<T>**: Generic service interface

### 5. **Dependency Inversion Principle (DIP)**
Depend on abstractions, not concretions:

- **Services depend on repository interfaces**, not implementations
- **Controllers depend on service interfaces**, not implementations
- **Dependency Injection** manages all dependencies
- **Message handling** depends on interfaces, not concrete implementations

## 🎨 Design Patterns Implemented

### **1. Repository Pattern**
- **Purpose**: Abstracts data access logic and provides a uniform interface
- **Implementation**: `IRepository<T>` interface with `BaseRepository<T>` implementation
- **Benefits**: Testability, maintainability, and database independence
- **Usage**: All entities have dedicated repository interfaces and implementations

### **2. Dependency Injection Pattern**
- **Purpose**: Manages object creation and lifetime throughout the application
- **Implementation**: Built-in .NET DI container with service registration
- **Benefits**: Loose coupling, testability, and configuration flexibility
- **Usage**: All dependencies injected through constructor injection

### **3. Middleware Pattern**
- **Purpose**: Handles cross-cutting concerns in the request pipeline
- **Implementation**: `ErrorHandlingMiddleware`, `AuthenticationDebugMiddleware`, `ResultHandlingMiddleware`
- **Benefits**: Separation of concerns, reusable functionality
- **Usage**: Error handling, authentication debugging, and result transformation

### **4. Factory Pattern**
- **Purpose**: Creates objects without specifying their exact classes
- **Implementation**: Entity Framework DbSet creation and configuration
- **Benefits**: Encapsulation of object creation logic
- **Usage**: Database context factory and entity configurations

### **5. Observer Pattern (Pub/Sub)**
- **Purpose**: Implements publish-subscribe for event-driven architecture
- **Implementation**: RabbitMQ messaging with topic exchange
- **Benefits**: Loose coupling between publishers and subscribers
- **Usage**: Motorcycle registration events and notifications

### **6. Strategy Pattern**
- **Purpose**: Encapsulates different algorithms and makes them interchangeable
- **Implementation**: Different rental plans with different pricing strategies
- **Benefits**: Easy addition of new rental plans without modifying existing code
- **Usage**: Rental calculation logic based on plan type

### **7. Template Method Pattern**
- **Purpose**: Defines the skeleton of an algorithm in a base class
- **Implementation**: `BaseRepository<T>` provides template for common CRUD operations
- **Benefits**: Code reuse and consistent behavior across repositories
- **Usage**: Standard CRUD operations with customizable specific implementations

### **8. Command Pattern**
- **Purpose**: Encapsulates requests as objects
- **Implementation**: Service methods encapsulate business operations as commands
- **Benefits**: Parameterization, queuing, and undo operations
- **Usage**: Business operations like creating rentals, updating motorcycles

### **9. Builder Pattern**
- **Purpose**: Constructs complex objects step by step
- **Implementation**: Entity configurations use fluent API for building entity mappings
- **Benefits**: Flexible object construction and readable configuration
- **Usage**: Entity Framework configurations and validation rules

### **10. Mediator Pattern (CQRS Implementation)**
- **Purpose**: Reduces coupling between components by using a mediator
- **Implementation**: MediatR for CQRS command and query handling
- **Benefits**: Decoupled communication and centralized control
- **Usage**: Command/query separation and pipeline behaviors
- **Structure**: Organized by features with Commands, Queries, and Handlers
- **Result Pattern**: Consistent error handling with `Result<T>` wrapper

### **11. Specification Pattern**
- **Purpose**: Encapsulates business rules and criteria
- **Implementation**: Domain validation rules and business logic
- **Benefits**: Reusable business rules and clear domain expression
- **Usage**: Rental eligibility, motorcycle availability checks

### **12. Unit of Work Pattern**
- **Purpose**: Maintains a list of objects affected by a business transaction
- **Implementation**: Entity Framework's DbContext manages transactions
- **Benefits**: Transaction management and consistency
- **Usage**: Database operations and change tracking

## 🛠️ Technologies & Frameworks

### **Core Technologies**
- **.NET 9.0**: Latest .NET framework with performance improvements and modern features
- **ASP.NET Core Web API**: RESTful API framework with comprehensive middleware support
- **C# 12**: Modern C# features including primary constructors, nullable reference types, and pattern matching

### **Authentication & Authorization**
- **ASP.NET Core Identity**: User management and authentication system
- **JWT Bearer Authentication**: Token-based authentication with configurable settings
- **Microsoft.IdentityModel.Tokens**: JWT token generation and validation
- **System.IdentityModel.Tokens.Jwt**: Core JWT functionality

### **Data Access & Persistence**
- **Entity Framework Core 9.0**: ORM with Code First approach and migrations
- **PostgreSQL**: Robust, open-source relational database with ACID compliance
- **Npgsql 9.0.3**: High-performance .NET PostgreSQL provider
- **Npgsql.EntityFrameworkCore.PostgreSQL 9.0.4**: EF Core PostgreSQL integration

### **Messaging & Event-Driven Architecture**
- **RabbitMQ**: Message broker for asynchronous communication and event-driven architecture
- **RabbitMQ.Client 6.8.1**: .NET client library with connection pooling and management
- **Topic Exchange**: Flexible message routing for motorcycle registration events

### **Logging & Monitoring**
- **Serilog 4.2.0**: Structured logging framework with rich formatting
- **Serilog.AspNetCore 8.0.2**: ASP.NET Core integration for Serilog
- **Serilog.Sinks.Console 6.0.0**: Console output with structured formatting
- **Serilog.Sinks.File 6.0.0**: File-based logging with rolling file support

### **Validation & Business Rules**
- **FluentValidation 12.0.0**: Fluent validation library for comprehensive input validation
- **FluentValidation.DependencyInjectionExtensions**: DI integration for validators
- **Custom Validation Attributes**: File type and format validation for uploads

### **Object Mapping & Transformation**
- **AutoMapper 13.0.1**: Object-to-object mapping with performance optimizations
- **Custom Mappers**: Domain-specific mapping logic for DTOs and entities

### **CQRS & Mediator Pattern**
- **MediatR 12.2.0**: Mediator pattern implementation for CQRS architecture
- **Pipeline Behaviors**: Logging and validation behaviors for cross-cutting concerns
- **Command/Query Separation**: Clear distinction between read and write operations
- **Result Pattern**: Consistent error handling with `Result<T>` wrapper
- **Feature Organization**: Commands, Queries, and Handlers organized by domain

### **Testing Framework**
- **xUnit 2.9.2**: Unit testing framework with comprehensive assertion capabilities
- **Moq 4.20.69**: Mocking framework for isolated unit testing
- **FluentAssertions 6.12.0**: Expressive assertion library for readable tests
- **Microsoft.EntityFrameworkCore.InMemory**: In-memory database for integration testing
- **Microsoft.AspNetCore.Mvc.Testing**: Integration testing with WebApplicationFactory
- **coverlet.collector**: Code coverage collection and reporting

### **API Documentation & Testing**
- **Swagger/OpenAPI**: Comprehensive API documentation with interactive testing
- **Swashbuckle.AspNetCore 6.6.2**: Swagger integration with JWT authentication support
- **Custom API Models**: Validation error responses and file upload models

### **Containerization & Orchestration**
- **Docker**: Multi-stage containerization with optimized image sizes
- **Docker Compose**: Multi-container orchestration with service dependencies
- **Health Checks**: Container health monitoring and restart policies
- **Volume Management**: Persistent data storage for database and logs

### **Development Tools**
- **Microsoft.VisualStudio.Azure.Containers.Tools.Targets**: Docker development support
- **Microsoft.EntityFrameworkCore.Design**: EF Core design-time tools
- **Microsoft.EntityFrameworkCore.Tools**: Database migration and scaffolding tools

## 📋 Methodologies & Practices

### **1. Clean Architecture**
- **Dependency Inversion**: Inner layers don't depend on outer layers
- **Separation of Concerns**: Each layer has distinct responsibilities
- **Testability**: Easy to unit test with dependency injection
- **Independence**: Business logic independent of frameworks
- **Maintainability**: Clear boundaries and responsibilities

### **2. Domain-Driven Design (DDD)**
- **Rich Domain Models**: Entities contain business logic and validation
- **Ubiquitous Language**: Domain terms used consistently across code
- **Aggregate Roots**: Entities manage their own state and invariants
- **Value Objects**: Encapsulated domain concepts (enums, DTOs)
- **Domain Services**: Complex business logic encapsulated in services
- **Bounded Contexts**: Clear domain boundaries and responsibilities

### **3. CQRS (Command Query Responsibility Segregation)**
- **Command Handlers**: Handle write operations and business logic
- **Query Handlers**: Handle read operations and data retrieval
- **MediatR Integration**: Decoupled command and query processing
- **Pipeline Behaviors**: Cross-cutting concerns for validation and logging
- **Separation of Concerns**: Clear distinction between read and write operations

#### **CQRS Implementation Details**

The project implements a comprehensive CQRS pattern using MediatR with the following structure:

```
Features/
├── Auth/
│   ├── Commands/
│   │   ├── CreateUserCommand.cs
│   │   └── LoginCommand.cs
│   ├── Handlers/
│   │   ├── CreateUserCommandHandler.cs
│   │   └── LoginCommandHandler.cs
│   └── Validators/
│       └── LoginCommandValidator.cs
├── DeliveryPersons/
│   ├── Commands/
│   │   └── CreateDeliveryPersonCommand.cs
│   ├── Queries/
│   │   ├── GetDeliveryPersonByIdQuery.cs
│   │   ├── GetDeliveryPersonsQuery.cs
│   │   └── CanRentMotorcycleQuery.cs
│   └── Handlers/
│       ├── CreateDeliveryPersonCommandHandler.cs
│       ├── GetDeliveryPersonByIdQueryHandler.cs
│       └── UpdateDeliveryPersonLicenseImageCommandHandler.cs
├── Motorcycles/
│   ├── Commands/
│   │   ├── CreateMotorcycleCommand.cs
│   │   ├── UpdateMotorcycleLicensePlateCommand.cs
│   │   └── DeleteMotorcycleCommand.cs
│   ├── Queries/
│   │   ├── GetMotorcycleByIdQuery.cs
│   │   └── GetMotorcyclesQuery.cs
│   └── Handlers/
│       ├── CreateMotorcycleCommandHandler.cs
│       └── GetMotorcycleByIdQueryHandler.cs
└── Rentals/
    ├── Commands/
    │   ├── CreateRentalCommand.cs
    │   └── CompleteRentalCommand.cs
    ├── Queries/
    │   ├── GetRentalByIdQuery.cs
    │   ├── GetRentalsByDeliveryPersonQuery.cs
    │   ├── CalculateRentalReturnQuery.cs
    │   └── IsMotorcycleAvailableQuery.cs
    └── Handlers/
        ├── CreateRentalCommandHandler.cs
        ├── CompleteRentalCommandHandler.cs
        └── GetRentalByIdQueryHandler.cs
```

#### **CQRS Benefits in This Project**
- **Clear Separation**: Commands for writes, Queries for reads
- **Scalability**: Independent scaling of read and write operations
- **Maintainability**: Single responsibility for each handler
- **Testability**: Isolated testing of business logic
- **Flexibility**: Easy to add new commands/queries without affecting existing code
- **Result Pattern**: Consistent error handling with `Result<T>` pattern

### **4. Test-Driven Development (TDD)**
- **Unit Tests**: Comprehensive test coverage for all business logic
- **Integration Tests**: End-to-end testing with real database interactions
- **Mocking**: Isolated testing with mocked dependencies
- **Arrange-Act-Assert**: Clear test structure and organization
- **Test Coverage**: High coverage percentage with meaningful tests
- **Behavior-Driven Testing**: Tests focus on behavior rather than implementation

### **5. Error Handling Strategy**
- **Problem Details**: RFC 7807 compliant error responses
- **Centralized Error Handling**: Middleware-based approach
- **Structured Logging**: Consistent error logging with context
- **Graceful Degradation**: Proper exception handling throughout
- **Custom Error Types**: Domain-specific error handling
- **Error Recovery**: Appropriate fallback mechanisms

### **6. Async/Await Pattern**
- **Non-blocking Operations**: All I/O operations are asynchronous
- **CancellationToken Support**: Proper cancellation handling
- **Performance**: Better resource utilization and scalability
- **Deadlock Prevention**: Proper async patterns throughout
- **Concurrent Operations**: Parallel processing where appropriate

### **7. Configuration Management**
- **Environment-Specific Configs**: Different settings for different environments
- **Secrets Management**: Secure handling of sensitive data
- **Validation**: Configuration validation at startup
- **Hot Reloading**: Configuration changes without restart
- **Type Safety**: Strongly-typed configuration objects

### **8. Logging Strategy**
- **Structured Logging**: JSON-formatted logs with context
- **Log Levels**: Appropriate use of different log levels
- **Correlation IDs**: Request tracing across services
- **Performance Logging**: Operation timing and metrics
- **Security Logging**: Audit trails for sensitive operations
- **Log Aggregation**: Centralized log collection and analysis

## 🚀 Features

### **Core Business Features**
- **Motorcycle Management**: CRUD operations with unique license plate validation
- **Delivery Person Registration**: Complete registration with CNH validation and image upload
- **Rental System**: Flexible rental plans with automatic pricing calculation
- **File Upload**: CNH image upload with format validation (PNG/BMP)
- **Business Logic**: Rich domain models with encapsulated business rules

### **Technical Features**
- **Event-Driven Architecture**: RabbitMQ integration for notifications and events
- **Structured Logging**: Comprehensive logging with Serilog
- **Error Handling**: Centralized error handling with Problem Details
- **API Documentation**: Swagger/OpenAPI documentation with examples
- **Containerization**: Docker and Docker Compose support with health checks
- **Database Migrations**: Entity Framework migrations for schema management
- **Health Checks**: Application health monitoring endpoints

### **Advanced Features**
- **Message Persistence**: RabbitMQ messages are persistent and durable
- **Connection Management**: Proper RabbitMQ connection lifecycle management
- **File Storage**: Organized file storage with timestamp-based naming
- **Configuration Management**: Environment-specific configurations
- **Dependency Injection**: Comprehensive DI container configuration

## 📊 Business Rules

### **Rental Rules**
- Only delivery persons with CNH type A or A+B can rent motorcycles
- Rental start date is always the day after creation
- Early return fines: 20% (7-day plan), 40% (15-day plan)
- Late return: R$ 50.00 per additional day
- Motorcycles with active rentals cannot be deleted

### **Validation Rules**
- License plates and CNPJ must be unique
- CNH images must be PNG or BMP format
- All required fields must be provided
- Business logic validation in domain entities

### **Pricing Rules**
- **7 days**: R$ 30.00/day
- **15 days**: R$ 28.00/day
- **30 days**: R$ 22.00/day
- **45 days**: R$ 20.00/day
- **50 days**: R$ 18.00/day

## 🧪 Testing Strategy

### **Test Coverage**
- **133 Total Tests**: Comprehensive test suite
- **110 Unit Tests**: Business logic and service layer testing
- **23 Integration Tests**: End-to-end testing with database interactions
- **0 Failures**: All tests passing consistently

### **Unit Testing**
- **Service Layer**: Complete coverage with mocked dependencies
- **Domain Logic**: Business rule validation and calculation testing
- **Repository Layer**: Data access pattern testing
- **Mocking**: Isolated testing with Moq framework

### **Integration Testing**
- **Database Integration**: Real database operations with Entity Framework
- **API Integration**: HTTP endpoint testing with WebApplicationFactory
- **End-to-End Flows**: Complete business process testing
- **Message Integration**: RabbitMQ messaging testing

### **Test Organization**
- **Domain Tests**: Entity behavior and business logic
- **Application Tests**: Service layer and business operations
- **Integration Tests**: Cross-layer functionality
- **End-to-End Tests**: Complete user workflows

## 🏃‍♂️ Quick Start

### **With Docker (Recommended)**
```bash
git clone <repository-url>
cd vehicle-rental
docker-compose up --build
```

### **Local Development**
```bash
# Install dependencies
dotnet restore

# Run all tests
dotnet test vehicle-rental.sln

# Start the application
cd src/vehicle-rental.api
dotnet run
```

**Application URLs:**
- API: http://localhost:5000
- Swagger UI: http://localhost:5000/swagger
- RabbitMQ Management: http://localhost:15672 (guest/guest)
- PostgreSQL: localhost:5432

## 🔐 Login e Autenticação

O sistema possui usuários padrão criados automaticamente na inicialização. Aqui está o passo a passo para fazer login:

### **Usuários Padrão Criados Automaticamente**

#### **👨‍💼 Administrador**
- **Email**: `admin@vehicle-rental.com`
- **Senha**: `Admin123!`
- **Role**: `Administrator`
- **Permissões**: Acesso completo a todas as funcionalidades

#### **🏍️ Entregador**
- **Email**: `entregador@vehicle-rental.com`
- **Senha**: `Entregador123!`
- **Role**: `DeliveryPerson`
- **Permissões**: Acesso limitado às funcionalidades de entregador

### **Passo a Passo para Login**

#### **1. Via Swagger UI (Recomendado)**
1. Acesse: http://localhost:5000/swagger
2. Localize o endpoint `POST /api/auth/login`
3. Clique em "Try it out"
4. Insira as credenciais no formato JSON:

**Para Administrador:**
```json
{
  "email": "admin@vehicle-rental.com",
  "password": "Admin123!"
}
```

**Para Entregador:**
```json
{
  "email": "entregador@vehicle-rental.com",
  "password": "Entregador123!"
}
```

5. Clique em "Execute"
6. Copie o token JWT retornado na resposta
7. Clique no botão "Authorize" no topo da página Swagger
8. Cole o token no formato: `Bearer {seu_token_aqui}`
9. Clique em "Authorize" e depois "Close"

#### **2. Via cURL**
```bash
# Login como Administrador
curl -X POST "http://localhost:5000/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@vehicle-rental.com",
    "password": "Admin123!"
  }'

# Login como Entregador
curl -X POST "http://localhost:5000/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "entregador@vehicle-rental.com",
    "password": "Entregador123!"
  }'
```

#### **3. Via Postman/Insomnia**
1. **Method**: POST
2. **URL**: `http://localhost:5000/api/auth/login`
3. **Headers**: `Content-Type: application/json`
4. **Body** (raw JSON):
   ```json
   {
     "email": "admin@vehicle-rental.com",
     "password": "Admin123!"
   }
   ```

### **Resposta de Login**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": "guid-do-usuario",
    "email": "admin@vehicle-rental.com",
    "role": "Administrator"
  },
  "expiresAt": "2024-01-15T16:00:00Z"
}
```

### **Usando o Token JWT**

Após obter o token, inclua-o no header `Authorization` de todas as requisições:

```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### **Permissões por Role**

#### **Administrator**
- ✅ Criar/editar/deletar motocicletas
- ✅ Criar/editar entregadores
- ✅ Upload de imagens de CNH
- ✅ Visualizar todos os dados
- ✅ Gerenciar usuários

#### **DeliveryPerson**
- ✅ Visualizar motocicletas disponíveis
- ✅ Criar locações
- ✅ Visualizar suas próprias locações
- ✅ Calcular valores de devolução
- ❌ Não pode criar/editar motocicletas
- ❌ Não pode criar outros entregadores

### **Troubleshooting**

#### **Erro 401 Unauthorized**
- Verifique se o email e senha estão corretos
- Certifique-se de que o usuário foi criado na inicialização
- Verifique os logs da aplicação para erros de autenticação

#### **Erro 403 Forbidden**
- Verifique se o usuário tem a role necessária para a operação
- Administradores têm acesso total, entregadores têm acesso limitado

#### **Token Expirado**
- Tokens JWT expiram em 8 horas
- Faça login novamente para obter um novo token
- Configure um refresh token se necessário

## 🔗 API Endpoints

### **Motorcycles**
- `POST /api/motorcycles` - Create motorcycle
- `GET /api/motorcycles` - List motorcycles (with optional license plate filter)
- `GET /api/motorcycles/{id}` - Get motorcycle by ID
- `PUT /api/motorcycles/{id}/license-plate` - Update license plate
- `DELETE /api/motorcycles/{id}` - Delete motorcycle

### **Delivery Persons**
- `POST /api/deliverypersons` - Create delivery person
- `GET /api/deliverypersons` - List delivery persons
- `GET /api/deliverypersons/{id}` - Get delivery person by ID
- `POST /api/deliverypersons/{id}/license-image` - Upload CNH image

### **Rentals**
- `POST /api/rentals` - Create rental
- `GET /api/rentals/{id}` - Get rental by ID
- `GET /api/rentals/delivery-person/{id}` - Get rentals by delivery person
- `POST /api/rentals/{id}/calculate-return` - Calculate return amount
- `POST /api/rentals/{id}/complete` - Complete rental

## 💰 Rental Plans & Pricing

### **Available Plans**
- **7 days**: R$ 30.00/day
- **15 days**: R$ 28.00/day
- **30 days**: R$ 22.00/day
- **45 days**: R$ 20.00/day
- **50 days**: R$ 18.00/day

### **Calculation Logic**
- **Early Return**: Fine calculation based on unused days and plan type
- **Late Return**: Additional charges for extra days
- **Total Amount**: Base amount + fines + additional charges
- **Business Logic**: Encapsulated in domain entities

## 📁 File Storage

CNH images are stored in the `uploads/licenses/` directory with the format:
`{deliveryPersonId}_{timestamp}.{extension}`

## 📨 Messaging Architecture

### **RabbitMQ Integration**
- **Exchange**: `motorcycle.events` (Topic type)
- **Routing Key**: `motorcycle.registered`
- **Message Persistence**: Durable messages with delivery guarantees
- **Connection Management**: Proper lifecycle management

### **Event Flow**
1. Motorcycle registration triggers event publication
2. RabbitMQ routes message to consumers
3. Consumer processes notifications for specific criteria
4. Notifications stored in database for tracking

## 📈 Performance Considerations

- **Async/Await**: Non-blocking I/O operations throughout
- **Connection Pooling**: Entity Framework connection pooling
- **Cancellation Tokens**: Proper request cancellation handling
- **Structured Logging**: Efficient logging with minimal overhead
- **Repository Pattern**: Optimized data access patterns
- **Message Queuing**: Asynchronous processing for scalability

## 🔒 Security Features

### **Authentication & Authorization**
- **JWT Bearer Authentication**: Secure token-based authentication
- **ASP.NET Core Identity**: Robust user management system
- **Role-Based Access Control**: Granular permission management
- **Token Expiration**: Configurable token lifetime (8 hours default)
- **Secure Token Generation**: HMAC SHA256 signing with configurable secret

### **Input Validation & Sanitization**
- **FluentValidation**: Comprehensive input validation rules
- **File Upload Security**: Restricted file types (PNG/BMP only)
- **SQL Injection Prevention**: Entity Framework parameterized queries
- **XSS Protection**: Input sanitization and encoding
- **CSRF Protection**: Built-in ASP.NET Core CSRF protection

### **Data Protection**
- **Connection String Security**: Environment-based configuration
- **Sensitive Data Handling**: Secure storage of passwords and tokens
- **Database Security**: PostgreSQL with proper access controls
- **File Storage Security**: Organized file storage with validation
- **Error Information Control**: Controlled error disclosure

### **Infrastructure Security**
- **Docker Security**: Non-root user execution in containers
- **Network Isolation**: Docker network isolation for services
- **Health Checks**: Security monitoring and alerting
- **Logging Security**: Audit trails for sensitive operations
- **Environment Separation**: Different configurations per environment

## 🐳 Docker Configuration

The application includes complete Docker support:

- **Multi-stage builds** for optimized image size
- **Docker Compose** for local development with dependencies
- **Environment-specific configurations** for different environments
- **Health checks** for container monitoring and orchestration
- **Volume Management** for persistent data storage
- **Network Isolation** for secure container communication

### **Services**
- **PostgreSQL**: Database with health checks and persistent storage
- **RabbitMQ**: Message broker with management interface
- **API**: Application with dependency management and health monitoring

## 📊 Project Statistics

### **Code Metrics**
- **Total Lines of Code**: ~4,500+ lines across all projects
- **Test Coverage**: 133 tests (110 unit + 23 integration) with 100% pass rate
- **Architecture Layers**: 4 (API, Application, Domain, Data)
- **Design Patterns**: 12+ implemented patterns
- **SOLID Principles**: All 5 principles consistently applied
- **Technologies**: 25+ modern frameworks and libraries

### **Project Structure**
- **Solution Projects**: 6 projects (API, Application, Domain, Data, Tests)
- **Test Projects**: 2 (Unit Tests, Integration Tests)
- **NuGet Packages**: 30+ dependencies with latest versions
- **Docker Services**: 3 services (API, PostgreSQL, RabbitMQ)
- **Database Migrations**: 5 migrations for schema evolution

### **Code Quality**
- **Code Coverage**: High coverage across all layers
- **Static Analysis**: Enabled with nullable reference types
- **Code Standards**: Consistent C# coding conventions
- **Documentation**: Comprehensive README and inline documentation
- **Error Handling**: RFC 7807 compliant error responses
- **Logging**: Structured logging with Serilog integration

### **Performance & Scalability**
- **Async Operations**: 100% async/await implementation
- **Connection Pooling**: Entity Framework connection pooling
- **Message Queuing**: RabbitMQ for asynchronous processing
- **Caching Strategy**: Ready for implementation with Redis
- **Database Optimization**: Indexed queries and efficient data access
- **Containerization**: Docker optimization with multi-stage builds

## 📚 Additional Resources

- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [SOLID Principles](https://en.wikipedia.org/wiki/SOLID)
- [Domain-Driven Design](https://martinfowler.com/bliki/DomainDrivenDesign.html)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/)
- [RabbitMQ Documentation](https://www.rabbitmq.com/documentation.html)
- [xUnit Testing](https://xunit.net/)

## 🤝 Contributing

1. Follow C# coding conventions and SOLID principles
2. Write comprehensive unit tests for new features
3. Add integration tests for new endpoints
4. Update documentation as needed
5. Ensure all tests pass before submitting
6. Use meaningful commit messages
7. Follow the established architecture patterns
8. Maintain test coverage above 90%

## 📄 License

This project is part of the Mottu backend challenge and follows the specified requirements and constraints.

---

## 🎉 Recent Achievements

✅ **Complete Test Suite**: 133 tests with 100% pass rate  
✅ **Rich Domain Models**: Business logic encapsulated in entities  
✅ **Event-Driven Architecture**: RabbitMQ integration with message persistence  
✅ **Comprehensive Error Handling**: RFC 7807 compliant error responses  
✅ **Docker Orchestration**: Multi-container setup with health checks  
✅ **Integration Testing**: End-to-end testing with real database  
✅ **Clean Architecture**: SOLID principles and design patterns  
✅ **Production Ready**: Logging, monitoring, and error handling  
✅ **JWT Authentication**: Secure token-based authentication system  
✅ **CQRS Implementation**: Complete Command/Query separation with MediatR  
✅ **Pipeline Behaviors**: Cross-cutting concerns with validation and logging  
✅ **Comprehensive Documentation**: Detailed README with architecture overview  
✅ **Modern C# Features**: Primary constructors, nullable reference types  
✅ **Structured Logging**: Serilog integration with file and console sinks  
✅ **Database Migrations**: Entity Framework migrations for schema management

- **Desenvolvedor**: [Kássio Fernandes Lima]
- **Email**: [kassioflima@gmail.com]