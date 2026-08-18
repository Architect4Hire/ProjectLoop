using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProjectLoop.Engagement.Core;
using Xunit;

namespace ProjectLoop.Engagement.Tests;

/// <summary>
/// Exercises the full Controller -> Facade -> Data -> Repository -> DbContext
/// chain (no stubbed facade) to prove tenant isolation holds end to end, not
/// just at the repository/facade unit-test level.
/// </summary>
public class CrossTenantDenialIntegrationTests
{
    private const string TestScheme = "Test";
    private const string TestTenantHeaderName = "X-Test-TenantId";

    private sealed class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, "user-1") };

            var tenantIdHeader = Request.Headers[TestTenantHeaderName].FirstOrDefault();
            if (tenantIdHeader is not null)
            {
                claims.Add(new Claim("tenant_id", tenantIdHeader));
            }

            var identity = new ClaimsIdentity(claims, TestScheme);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, TestScheme);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }

    private static WebApplicationFactory<Program> CreateFactory(string databaseName)
    {
        return new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseSetting(
                "ConnectionStrings:engagementdb",
                "Server=(localdb)\\MSSQLLocalDB;Database=ProjectLoopEngagement_UnusedInCrossTenantTest;Trusted_Connection=True;TrustServerCertificate=True;");

            builder.ConfigureServices(services =>
            {
                services.AddAuthentication(TestScheme)
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestScheme, _ => { });

                var engagementDbContextDescriptors = services
                    .Where(d => d.ServiceType == typeof(EngagementDbContext)
                        || (d.ServiceType.IsGenericType && d.ServiceType.GetGenericArguments().Contains(typeof(EngagementDbContext))))
                    .ToList();
                foreach (var descriptor in engagementDbContextDescriptors)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<EngagementDbContext>(options => options.UseInMemoryDatabase(databaseName));
            });
        });
    }

    [Fact]
    public async Task TenantA_Cannot_Fetch_TenantB_Project_Detail()
    {
        var tenantAId = Guid.NewGuid();
        var tenantBId = Guid.NewGuid();
        var databaseName = Guid.NewGuid().ToString();

        var now = DateTimeOffset.UtcNow;
        var tenantBProject = new Project
        {
            Id = Guid.NewGuid(),
            TenantId = tenantBId,
            Name = "Tenant B Confidential Project",
            Status = ProjectStatus.Active,
            Health = ProjectHealth.Green,
            CreatedAtUtc = now,
            UpdatedAtUtc = now,
        };

        using var factory = CreateFactory(databaseName);

        using (var scope = factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<EngagementDbContext>();
            dbContext.Projects.Add(tenantBProject);
            await dbContext.SaveChangesAsync();
        }

        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add(TestTenantHeaderName, tenantAId.ToString());

        var response = await client.GetAsync($"/projects/{tenantBProject.Id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task TenantA_Cannot_Fetch_TenantB_Project_Milestones()
    {
        var tenantAId = Guid.NewGuid();
        var tenantBId = Guid.NewGuid();
        var databaseName = Guid.NewGuid().ToString();

        var now = DateTimeOffset.UtcNow;
        var tenantBProject = new Project
        {
            Id = Guid.NewGuid(),
            TenantId = tenantBId,
            Name = "Tenant B Confidential Project",
            Status = ProjectStatus.Active,
            Health = ProjectHealth.Green,
            CreatedAtUtc = now,
            UpdatedAtUtc = now,
        };
        var tenantBMilestone = new Milestone
        {
            Id = Guid.NewGuid(),
            TenantId = tenantBId,
            ProjectId = tenantBProject.Id,
            Name = "Tenant B Confidential Milestone",
            Status = MilestoneStatus.InProgress,
            CreatedAtUtc = now,
            UpdatedAtUtc = now,
        };

        using var factory = CreateFactory(databaseName);

        using (var scope = factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<EngagementDbContext>();
            dbContext.Projects.Add(tenantBProject);
            dbContext.Milestones.Add(tenantBMilestone);
            await dbContext.SaveChangesAsync();
        }

        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add(TestTenantHeaderName, tenantAId.ToString());

        var response = await client.GetAsync($"/projects/{tenantBProject.Id}/milestones");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
