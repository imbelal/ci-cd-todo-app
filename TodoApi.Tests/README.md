# TodoApi.Tests

Comprehensive unit and integration tests for the Todo API.

## Test Structure

### Unit Tests

#### Data Layer Tests (`Data/TodoContextTests.cs`)

- Database context creation and configuration
- Seed data verification
- CRUD operations
- Entity relationships

#### Controller Tests (`Controllers/TodosControllerTests.cs`)

- All API endpoints (GET, POST, PUT, DELETE)
- Filtering and pagination
- Error handling scenarios
- Model validation
- Response formatting

#### Model Tests (`Models/TodoItemTests.cs`)

- Model validation
- Data annotations
- Default values
- DTO validation

### Integration Tests (`Integration/TodoApiIntegrationTests.cs`)

- End-to-end API testing
- HTTP status codes
- Request/response flow
- Real database operations
- Authentication scenarios

## Test Categories

### Data Tests

- ✅ Context initialization
- ✅ Seed data verification
- ✅ CRUD operations
- ✅ Entity validation

### API Tests

- ✅ GET all todos with filtering and pagination
- ✅ GET single todo by ID
- ✅ POST create new todo
- ✅ PUT update existing todo
- ✅ DELETE todo
- ✅ GET statistics endpoint
- ✅ Error handling (404, 400, 500)

### Model Validation Tests

- ✅ Required field validation
- ✅ String length validation
- ✅ Enum validation (Priority)
- ✅ Data type validation

### Integration Tests

- ✅ Full HTTP request/response cycle
- ✅ Database persistence
- ✅ Error responses
- ✅ Content negotiation
- ✅ Headers validation

## Running Tests

### Run All Tests

```bash
dotnet test
```

### Run with Coverage

```bash
dotnet test --collect:"XPlat Code Coverage"
```

### Run Specific Test Category

```bash
# Unit tests only
dotnet test --filter "FullyQualifiedName!~Integration"

# Integration tests only
dotnet test --filter "FullyQualifiedName~Integration"

# Controller tests only
dotnet test --filter "FullyQualifiedName~Controllers"
```

### Run with Verbose Output

```bash
dotnet test --verbosity normal
```

## Test Features

### Test Isolation

- Each test uses a separate in-memory database
- Tests are independent and can run in parallel
- Proper cleanup with `IDisposable`

### Mock Dependencies

- Logger mocking with Moq
- Isolated controller testing
- Dependency injection testing

### Data Validation

- Model validation testing
- Business rule validation
- Edge case handling

### Error Scenarios

- Invalid input handling
- Not found scenarios
- Server error simulation

## Test Dependencies

- **xUnit**: Testing framework
- **Microsoft.EntityFrameworkCore.InMemory**: In-memory database for testing
- **Microsoft.AspNetCore.Mvc.Testing**: Integration testing framework
- **Moq**: Mocking framework

## Test Coverage

The test suite covers:

- ✅ All API endpoints
- ✅ All business logic
- ✅ Data access layer
- ✅ Model validation
- ✅ Error handling
- ✅ Edge cases
- ✅ Integration scenarios

## Best Practices Implemented

1. **AAA Pattern**: Arrange, Act, Assert
2. **Test Isolation**: Each test is independent
3. **Descriptive Names**: Test names clearly describe the scenario
4. **Single Responsibility**: Each test verifies one specific behavior
5. **Mock External Dependencies**: Use mocks for external services
6. **Test Data**: Use meaningful test data
7. **Cleanup**: Proper resource disposal

## Example Test Run Output

```
Starting test execution, please wait...
A total of 25 test files matched the specified pattern.

Test Run Successful.
Total tests: 25
     Passed: 25
     Failed: 0
    Skipped: 0
 Total time: 2.5s
```

## Continuous Integration

These tests are designed to run in CI/CD pipelines:

- Fast execution (all tests complete in under 5 seconds)
- No external dependencies
- Deterministic results
- Comprehensive coverage
