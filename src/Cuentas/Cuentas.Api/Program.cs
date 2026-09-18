using Wolverine;
using Wolverine.RabbitMQ;
using Cuentas.Infrastructure;
using Shared.Api.Exceptions;

var builder = WebApplication.CreateBuilder(args);

// Exceptions
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Database
var connectionString = builder.Configuration.GetConnectionString("Database")
    ?? throw new InvalidOperationException("Falta configurar ConnectionStrings:Database");
builder.Services.AddInfrastructure(connectionString);

// RabbitMQ
var rabbitConnection = builder.Configuration.GetConnectionString("RabbitMQ")
    ?? throw new InvalidOperationException("Falta configurar ConnectionStrings:RabbitMQ");

builder.Host.UseWolverine(options =>
{
    options.UseRabbitMq(new Uri(rabbitConnection)).AutoProvision();
    options.ListenToRabbitQueue("cuentas");
});

var app = builder.Build();

// middlewares
app.UseExceptionHandler();

app.Run();
