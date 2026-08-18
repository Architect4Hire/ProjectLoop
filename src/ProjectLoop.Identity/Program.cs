using ProjectLoop.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

var app = builder.Build();

app.MapDefaultHealthChecks();

app.UseHttpsRedirection();

await app.RunAsync();
