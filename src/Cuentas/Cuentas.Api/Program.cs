using Wolverine;
using Wolverine.RabbitMQ;
using Cuentas.Infrastructure;
using Shared.Api.Exceptions;
using Shared.Application.Tiempo;
using Shared.Application.Identidad;

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

// Guids
builder.Services.AddSingleton<GeneradorId>();

// Timezone
var zonaId = builder.Configuration["Tiempo:ZonaHoraria"]
    ?? throw new InvalidOperationException("Falta configurar Tiempo:ZonaHoraria");

var zonaHoraria = TimeZoneInfo.FindSystemTimeZoneById(zonaId);
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton(services => new Reloj(
    services.GetRequiredService<TimeProvider>(),
    zonaHoraria));

var app = builder.Build();

// middlewares
app.UseExceptionHandler();

app.Run();
