using ProjectLoop.Identity.Core;
using ProjectLoop.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddSqlServerDbContext<IdentityDbContext>("identitydb");

var app = builder.Build();

app.MapDefaultHealthChecks();

app.UseHttpsRedirection();

await app.RunAsync();
