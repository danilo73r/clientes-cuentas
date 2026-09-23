using JasperFx;
using Clientes.Api.Endpoints;
using Clientes.Application;
using Clientes.Infrastructure;
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

// endpoints
var api = app.MapGroup("/api");
api.MapClientesEndpoints();

// habilita db-dump para sql de wolverine
return await app.RunJasperFxCommands(args);
