# Todo API

A RESTful Todo API built with .NET 8 Web API and Entity Framework Core In-Memory Database.

## Features

- ✅ Full CRUD operations for Todo items
- ✅ Filtering by completion status and priority
- ✅ Pagination support
- ✅ Todo statistics endpoint
- ✅ Input validation and error handling
- ✅ Swagger/OpenAPI documentation
- ✅ CORS support
- ✅ Health check endpoint

## Todo Item Properties

- **Id**: Unique identifier (auto-generated)
- **Title**: Required, max 200 characters
- **Description**: Optional, max 1000 characters
- **IsCompleted**: Boolean status (default: false)
- **Priority**: Low, Medium, or High (default: Medium)
- **CreatedAt**: Timestamp when created
- **UpdatedAt**: Timestamp when last modified
- **CompletedAt**: Timestamp when marked as completed
- **DueDate**: Optional due date

## API Endpoints

### Todo Operations

- `GET /api/todos` - Get all todos with optional filtering and pagination

  - Query parameters:
    - `isCompleted` (bool): Filter by completion status
    - `priority` (string): Filter by priority (Low, Medium, High)
    - `page` (int): Page number (default: 1)
    - `pageSize` (int): Items per page (default: 10)

- `GET /api/todos/{id}` - Get a specific todo by ID

- `POST /api/todos` - Create a new todo

  - Request body: `CreateTodoItemDto`

- `PUT /api/todos/{id}` - Update an existing todo

  - Request body: `UpdateTodoItemDto`

- `DELETE /api/todos/{id}` - Delete a todo

### Statistics

- `GET /api/todos/stats` - Get todo statistics
  - Returns: total todos, completed, pending, overdue, completion rate

### System

- `GET /health` - Health check endpoint

## Data Transfer Objects (DTOs)

### CreateTodoItemDto

```json
{
  "title": "string (required, 1-200 chars)",
  "description": "string (optional, max 1000 chars)",
  "priority": "Low|Medium|High (required, default: Medium)",
  "dueDate": "datetime (optional)"
}
```

### UpdateTodoItemDto

```json
{
  "title": "string (optional, 1-200 chars)",
  "description": "string (optional, max 1000 chars)",
  "isCompleted": "boolean (optional)",
  "priority": "Low|Medium|High (optional)",
  "dueDate": "datetime (optional)"
}
```

### TodoItemDto (Response)

```json
{
  "id": "integer",
  "title": "string",
  "description": "string",
  "isCompleted": "boolean",
  "createdAt": "datetime",
  "completedAt": "datetime",
  "updatedAt": "datetime",
  "priority": "string",
  "dueDate": "datetime"
}
```

## Running the Application

1. **Prerequisites**:

   - .NET 8 SDK installed

2. **Run the application**:

   ```bash
   dotnet run
   ```

3. **Access the API**:
   - Swagger UI: `https://localhost:7xxx` (HTTPS) or `http://localhost:5xxx` (HTTP)
   - API Base URL: `https://localhost:7xxx/api` or `http://localhost:5xxx/api`

## Example Usage

### Create a new todo

```bash
curl -X POST "https://localhost:7xxx/api/todos" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Learn .NET 8",
    "description": "Study the new features in .NET 8",
    "priority": "High",
    "dueDate": "2024-12-31T23:59:59Z"
  }'
```

### Get all todos with filtering

```bash
curl "https://localhost:7xxx/api/todos?isCompleted=false&priority=High&page=1&pageSize=5"
```

### Update a todo

```bash
curl -X PUT "https://localhost:7xxx/api/todos/1" \
  -H "Content-Type: application/json" \
  -d '{
    "isCompleted": true
  }'
```

### Get statistics

```bash
curl "https://localhost:7xxx/api/todos/stats"
```

## Sample Data

The application comes with three pre-seeded todo items:

1. "Learn .NET 8" (High priority)
2. "Build Todo API" (Medium priority)
3. "Write unit tests" (High priority, completed)

## Architecture

- **Models**: Entity classes representing the data structure
- **DTOs**: Data transfer objects for API requests/responses
- **Data**: Entity Framework DbContext and database configuration
- **Controllers**: API controllers handling HTTP requests
- **Middleware**: CORS, Swagger, error handling

## Technology Stack

- .NET 8 Web API
- Entity Framework Core (In-Memory Database)
- Swagger/OpenAPI for documentation
- Built-in dependency injection
- Model validation with Data Annotations
