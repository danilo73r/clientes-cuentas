using Cuentas.Application;
using JasperFx;
using Cuentas.Infrastructure;
using Shared.Application;
using Shared.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddSharedApi()
    .AddSharedApplication(builder.Configuration)
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

builder.Host
    .AddInfrastructure(builder.Configuration);

var app = builder.Build();

// middlewares
app.UseExceptionHandler();

// habilita db-dump para sql de wolverine
return await app.RunJasperFxCommands(args);
