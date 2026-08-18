using ProjectLoop.Engagement;
using ProjectLoop.Engagement.Core;
using ProjectLoop.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddSqlServerDbContext<EngagementDbContext>("engagementdb");

builder.Services.AddScoped<ICurrentTenantContextAccessor, CurrentTenantContextAccessor>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IMilestoneRepository, MilestoneRepository>();
builder.Services.AddScoped<IProjectDetailFacade, ProjectDetailFacade>();
builder.Services.AddScoped<IProjectMilestonesFacade, ProjectMilestonesFacade>();
builder.Services.AddScoped<IProjectHealthFacade, ProjectHealthFacade>();

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
