using Shared.Api.Exceptions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
var app = builder.Build();

// middlewares
app.UseExceptionHandler();

app.Run();
