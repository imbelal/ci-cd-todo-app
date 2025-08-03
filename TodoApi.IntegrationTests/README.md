# TodoApi Integration Tests

This project contains integration tests for the TodoApi application using ASP.NET Core's test host.

## Overview

Integration tests verify that the API endpoints work correctly when the entire application is running. These tests use the `Microsoft.AspNetCore.Mvc.Testing` package to create a test server and make HTTP requests to the API.

## Test Coverage

The integration tests cover:

### API Endpoints

- **GET /health** - Health check endpoint
- **GET /api/todos** - Retrieve all todos with filtering and pagination
- **GET /api/todos/{id}** - Retrieve a specific todo by ID
- **POST /api/todos** - Create a new todo
- **PUT /api/todos/{id}** - Update an existing todo
- **DELETE /api/todos/{id}** - Delete a todo
- **GET /api/todos/stats** - Get todo statistics

### Test Scenarios

- **Happy Path Tests**: Valid requests that should succeed
- **Error Handling Tests**: Invalid requests that should return appropriate error responses
- **Data Validation Tests**: Tests for model validation and business rules
- **Filtering and Pagination**: Tests for query parameters and result filtering

## Test Database

The integration tests use an in-memory database (`Microsoft.EntityFrameworkCore.InMemory`) to isolate tests from each other and avoid dependencies on external databases.

## Running the Tests

```bash
# Navigate to the integration test project
cd TodoApi.IntegrationTests

# Run all integration tests
dotnet test

# Run tests with detailed output
dotnet test --verbosity normal

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"
```

## Test Structure

Each test follows the Arrange-Act-Assert pattern:

- **Arrange**: Set up test data and dependencies
- **Act**: Execute the operation being tested
- **Assert**: Verify the expected outcome

## Dependencies

- `Microsoft.AspNetCore.Mvc.Testing` - For creating test servers
- `Microsoft.EntityFrameworkCore.InMemory` - For in-memory database
- `xunit` - Testing framework
- `System.Net.Http.Json` - For JSON serialization in HTTP requests
