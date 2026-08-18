using ProjectLoop.Approvals;
using ProjectLoop.Approvals.Core;
using ProjectLoop.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddSqlServerDbContext<ApprovalsDbContext>("approvalsdb");

builder.Services.AddScoped<ICurrentTenantContextAccessor, CurrentTenantContextAccessor>();
builder.Services.AddScoped<IApprovalRequestRepository, ApprovalRequestRepository>();
builder.Services.AddScoped<IApprovalDecisionRepository, ApprovalDecisionRepository>();
builder.Services.AddScoped<IApprovalDecisionTransaction, ApprovalDecisionTransaction>();
builder.Services.AddScoped<IApprovalRequestReadFacade, ApprovalRequestReadFacade>();
builder.Services.AddScoped<IApprovalApproveFacade, ApprovalApproveFacade>();
builder.Services.AddScoped<IApprovalRejectFacade, ApprovalRejectFacade>();

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

