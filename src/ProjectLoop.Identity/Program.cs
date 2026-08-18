using ProjectLoop.Identity;
using ProjectLoop.Identity.Core;
using ProjectLoop.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddSqlServerDbContext<IdentityDbContext>("identitydb");

builder.Services.AddScoped<ITenantContextResolver, TenantContextResolver>();
builder.Services.AddScoped<ICurrentTenantContextAccessor, CurrentTenantContextAccessor>();
builder.Services.AddScoped<IInvitationTokenGenerator, InvitationTokenGenerator>();
builder.Services.AddScoped<IClientInvitationRepository, ClientInvitationRepository>();
builder.Services.AddScoped<IInvitationCreationService, InvitationCreationService>();
builder.Services.AddScoped<IInvitationCreationFacade, InvitationCreationFacade>();
builder.Services.AddScoped<IInvitationAcceptanceService, InvitationAcceptanceService>();
builder.Services.AddScoped<IInvitationAcceptanceFacade, InvitationAcceptanceFacade>();

builder.Services.AddControllers();
builder.Services.AddAuthentication();

var app = builder.Build();

app.MapDefaultHealthChecks();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseMiddleware<TenantContextMiddleware>();

app.MapControllers();

await app.RunAsync();

public partial class Program
{
}
