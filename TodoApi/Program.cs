using Microsoft.EntityFrameworkCore;
using TodoApi.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Add Entity Framework with In-Memory Database
builder.Services.AddDbContext<TodoContext>(options =>
    options.UseInMemoryDatabase("TodoDb"));

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Todo API",
        Version = "v1",
        Description = "A simple Todo API built with .NET 8 Web API",
        Contact = new()
        {
            Name = "Todo API Team",
            Email = "support@todoapi.com"
        }
    });
});

var app = builder.Build();

// Ensure database is created and seeded
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<TodoContext>();
    context.Database.EnsureCreated();
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo API v1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at app root
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

// Add a health check endpoint
app.MapGet("/health", () => new
{
    Status = "Healthy",
    Timestamp = DateTime.UtcNow,
    Environment = app.Environment.EnvironmentName
})
.WithName("HealthCheck")
.WithOpenApi();

app.Run();

// Make Program class accessible for integration tests
public partial class Program { }
