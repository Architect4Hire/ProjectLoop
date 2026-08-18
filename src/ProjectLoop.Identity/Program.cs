using ProjectLoop.Identity;
using ProjectLoop.Identity.Core;
using ProjectLoop.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddSqlServerDbContext<IdentityDbContext>("identitydb");

builder.Services.AddScoped<ITenantContextResolver, TenantContextResolver>();
builder.Services.AddScoped<ICurrentTenantContextAccessor, CurrentTenantContextAccessor>();

var app = builder.Build();

app.MapDefaultHealthChecks();

app.UseHttpsRedirection();

app.UseMiddleware<TenantContextMiddleware>();

await app.RunAsync();
