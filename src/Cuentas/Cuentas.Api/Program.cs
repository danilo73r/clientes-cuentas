using Cuentas.Infrastructure;
using Shared.Api.Exceptions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var connectionString = builder
    .Configuration
    .GetConnectionString("Database")
    ?? throw new InvalidOperationException("Falta configurar ConnectionString");
builder.Services.AddInfrastructure(connectionString);

var app = builder.Build();

// middlewares
app.UseExceptionHandler();

app.Run();
