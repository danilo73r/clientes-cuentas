using Cuentas.Infrastructure;
using Shared.Application;
using Shared.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddSharedApi()
    .AddSharedApplication(builder.Configuration)
    .AddInfrastructure(builder.Configuration);

builder.Host
    .AddInfrastructure(builder.Configuration);

var app = builder.Build();

// middlewares
app.UseExceptionHandler();

app.Run();
