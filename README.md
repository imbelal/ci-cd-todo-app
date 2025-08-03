# Todo API Project

A comprehensive .NET 8 Web API application for managing todo items with separate unit and integration test projects.

## Project Structure

```
ci-cd-todo-app/
├── TodoApi/                          # Main Web API project
│   ├── Controllers/TodosController.cs # API endpoints for todo operations
│   ├── Models/TodoItem.cs            # Todo entity model
│   ├── Data/TodoContext.cs           # Entity Framework DbContext
│   ├── DTOs/                         # Data Transfer Objects
│   └── Program.cs                    # Application entry point
├── TodoApi.Tests/                    # Unit tests project
│   ├── Controllers/                  # Controller unit tests
│   ├── Models/                       # Model validation tests
│   ├── Data/                         # Data layer tests
│   └── README.md                     # Unit test documentation
├── TodoApi.IntegrationTests/         # Integration tests project
│   ├── TodoApiIntegrationTests.cs    # End-to-end API tests
│   └── README.md                     # Integration test documentation
└── ci-cd-todo-app.sln               # Solution file
```

## Features

### API Functionality

- ✅ **CRUD Operations**: Create, Read, Update, Delete todos
- ✅ **Filtering**: Filter by completion status, priority, due date
- ✅ **Pagination**: Page through large result sets
- ✅ **Statistics**: Get completion rates and todo counts
- ✅ **Validation**: Comprehensive input validation
- ✅ **Health Checks**: Application health monitoring
- ✅ **Swagger Documentation**: Interactive API documentation

### Data Management

- ✅ **Entity Framework Core**: Object-relational mapping
- ✅ **In-Memory Database**: Development and testing database
- ✅ **Seed Data**: Automatic sample data generation
- ✅ **Data Annotations**: Model validation attributes

### Testing Coverage

- ✅ **Unit Tests**: 22 tests covering controllers, models, and data layer
- ✅ **Integration Tests**: 11 tests covering end-to-end API functionality
- ✅ **Mocking**: Using Moq for dependency isolation
- ✅ **Test Isolation**: Separate test databases and clean test state

## Getting Started

### Prerequisites

- .NET 8.0 SDK
- Visual Studio, Visual Studio Code, or JetBrains Rider

### Running the Application

```bash
# Clone and navigate to project
cd ci-cd-todo-app

# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Run the API
dotnet run --project TodoApi

# The API will be available at:
# - HTTP: http://localhost:5052
# - HTTPS: https://localhost:7052
# - Swagger UI: http://localhost:5052/swagger
```

### Running Tests

```bash
# Run all tests
dotnet test

# Run only unit tests
dotnet test TodoApi.Tests

# Run only integration tests
dotnet test TodoApi.IntegrationTests

# Run with detailed output
dotnet test --verbosity normal

# Run with code coverage
dotnet test --collect:"XPlat Code Coverage"
```

## API Endpoints

| Method | Endpoint           | Description                               |
| ------ | ------------------ | ----------------------------------------- |
| GET    | `/api/todos`       | Get all todos (with filtering/pagination) |
| GET    | `/api/todos/{id}`  | Get a specific todo                       |
| POST   | `/api/todos`       | Create a new todo                         |
| PUT    | `/api/todos/{id}`  | Update an existing todo                   |
| DELETE | `/api/todos/{id}`  | Delete a todo                             |
| GET    | `/api/todos/stats` | Get todo statistics                       |
| GET    | `/health`          | Health check endpoint                     |

### Query Parameters for GET /api/todos

- `isCompleted` (bool): Filter by completion status
- `priority` (string): Filter by priority (Low, Medium, High)
- `dueDateFrom` (DateTime): Filter todos due after this date
- `dueDateTo` (DateTime): Filter todos due before this date
- `page` (int): Page number for pagination (default: 1)
- `pageSize` (int): Items per page (default: 10, max: 100)

## Example Usage

### Create a Todo

```bash
curl -X POST "http://localhost:5052/api/todos" \
     -H "Content-Type: application/json" \
     -d '{
       "title": "Complete project",
       "description": "Finish the todo API implementation",
       "priority": "High",
       "dueDate": "2025-08-10T00:00:00Z"
     }'
```

### Get All Todos with Filtering

```bash
curl "http://localhost:5052/api/todos?isCompleted=false&priority=High&page=1&pageSize=5"
```

### Update a Todo

```bash
curl -X PUT "http://localhost:5052/api/todos/1" \
     -H "Content-Type: application/json" \
     -d '{
       "title": "Complete project - Updated",
       "isCompleted": true,
       "priority": "Medium"
     }'
```

## Architecture Highlights

### Separation of Concerns

- **Controllers**: Handle HTTP requests and responses
- **Models**: Define data structure and validation rules
- **DTOs**: Control data transfer and API contracts
- **Data Layer**: Manage database operations and context

### Testing Strategy

- **Unit Tests**: Fast, isolated tests for individual components
- **Integration Tests**: Full application testing with HTTP client
- **Test Separation**: Different projects for different test types
- **Database Isolation**: Each test uses fresh in-memory database

### Development Best Practices

- **SOLID Principles**: Single responsibility, dependency injection
- **Clean Architecture**: Clear separation between layers
- **API Documentation**: Comprehensive Swagger/OpenAPI specs
- **Error Handling**: Consistent error responses and validation
- **Code Coverage**: Comprehensive test coverage across all layers

## Recent Changes

The integration tests have been moved to a separate project (`TodoApi.IntegrationTests`) to:

- ✅ **Better Organization**: Separate unit tests from integration tests
- ✅ **Independent Execution**: Run test types independently
- ✅ **Cleaner Dependencies**: Unit tests don't need integration test packages
- ✅ **CI/CD Flexibility**: Different deployment strategies for different test types

## Next Steps

Consider implementing:

- Database migrations for production use
- Authentication and authorization
- Logging and monitoring
- Docker containerization
- CI/CD pipeline configuration
- API versioning
- Rate limiting
- Caching strategies
