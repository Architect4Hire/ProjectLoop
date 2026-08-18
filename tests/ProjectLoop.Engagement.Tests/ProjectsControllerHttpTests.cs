using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProjectLoop.Engagement.Core;
using Xunit;

namespace ProjectLoop.Engagement.Tests;

public class ProjectsControllerHttpTests
{
    private const string TestScheme = "Test";

    private sealed class StubProjectDetailFacade : IProjectDetailFacade
    {
        private readonly ProjectDetailFacadeResult _result;

        public StubProjectDetailFacade(ProjectDetailFacadeResult result) => _result = result;

        public Task<ProjectDetailFacadeResult> GetProjectDetailAsync(Guid projectId, CancellationToken cancellationToken = default) =>
            Task.FromResult(_result);
    }

    private sealed class StubProjectMilestonesFacade : IProjectMilestonesFacade
    {
        private readonly ProjectMilestonesFacadeResult _result;

        public StubProjectMilestonesFacade(ProjectMilestonesFacadeResult result) => _result = result;

        public Task<ProjectMilestonesFacadeResult> GetProjectMilestonesAsync(Guid projectId, CancellationToken cancellationToken = default) =>
            Task.FromResult(_result);
    }

    private sealed class StubProjectHealthFacade : IProjectHealthFacade
    {
        private readonly ProjectHealthFacadeResult _result;

        public StubProjectHealthFacade(ProjectHealthFacadeResult result) => _result = result;

        public Task<ProjectHealthFacadeResult> GetProjectHealthAsync(Guid projectId, CancellationToken cancellationToken = default) =>
            Task.FromResult(_result);
    }

    private sealed class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "user-1"),
                new Claim("tenant_id", Guid.NewGuid().ToString()),
            };
            var identity = new ClaimsIdentity(claims, TestScheme);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, TestScheme);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }

    private static WebApplicationFactory<Program> CreateFactory(
        IProjectDetailFacade? detailFacade = null,
        IProjectMilestonesFacade? milestonesFacade = null,
        IProjectHealthFacade? healthFacade = null)
    {
        return new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseSetting(
                "ConnectionStrings:engagementdb",
                "Server=(localdb)\\MSSQLLocalDB;Database=ProjectLoopEngagement_UnusedInHttpTests;Trusted_Connection=True;TrustServerCertificate=True;");

            builder.ConfigureServices(services =>
            {
                services.AddAuthentication(TestScheme)
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestScheme, _ => { });

                if (detailFacade is not null)
                {
                    services.RemoveAll<IProjectDetailFacade>();
                    services.AddScoped(_ => detailFacade);
                }

                if (milestonesFacade is not null)
                {
                    services.RemoveAll<IProjectMilestonesFacade>();
                    services.AddScoped(_ => milestonesFacade);
                }

                if (healthFacade is not null)
                {
                    services.RemoveAll<IProjectHealthFacade>();
                    services.AddScoped(_ => healthFacade);
                }
            });
        });
    }

    [Fact]
    public async Task GetById_Returns_200_With_Project_When_Found()
    {
        var response = new ProjectDetailResponse
        {
            Id = Guid.NewGuid(),
            Name = "Portal Modernization",
            Status = ProjectStatus.Active,
            Health = ProjectHealth.Green,
            UpdatedAtUtc = DateTimeOffset.UtcNow,
        };
        var facade = new StubProjectDetailFacade(ProjectDetailFacadeResult.Success(response));

        using var factory = CreateFactory(detailFacade: facade);
        using var client = factory.CreateClient();

        var httpResponse = await client.GetAsync($"/projects/{response.Id}");
        var body = await httpResponse.Content.ReadFromJsonAsync<ProjectDetailResponse>();

        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
        Assert.Equal(response.Id, body?.Id);
        Assert.Equal("Portal Modernization", body?.Name);
    }

    [Fact]
    public async Task GetById_Returns_404_When_Not_Found()
    {
        var facade = new StubProjectDetailFacade(ProjectDetailFacadeResult.Failure(ProjectDetailFacadeError.NotFound));

        using var factory = CreateFactory(detailFacade: facade);
        using var client = factory.CreateClient();

        var httpResponse = await client.GetAsync($"/projects/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, httpResponse.StatusCode);
    }

    [Fact]
    public async Task GetMilestones_Returns_200_With_Milestones_When_Found()
    {
        var milestones = new[]
        {
            new MilestoneSummaryResponse
            {
                Id = Guid.NewGuid(),
                Name = "Discovery Complete",
                Status = MilestoneStatus.Completed,
                UpdatedAtUtc = DateTimeOffset.UtcNow,
            },
        };
        var facade = new StubProjectMilestonesFacade(ProjectMilestonesFacadeResult.Success(milestones));

        using var factory = CreateFactory(milestonesFacade: facade);
        using var client = factory.CreateClient();

        var httpResponse = await client.GetAsync($"/projects/{Guid.NewGuid()}/milestones");
        var body = await httpResponse.Content.ReadFromJsonAsync<MilestoneSummaryResponse[]>();

        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
        Assert.Single(body!);
        Assert.Equal("Discovery Complete", body![0].Name);
    }

    [Fact]
    public async Task GetMilestones_Returns_404_When_Project_Not_Found()
    {
        var facade = new StubProjectMilestonesFacade(ProjectMilestonesFacadeResult.Failure(ProjectMilestonesFacadeError.NotFound));

        using var factory = CreateFactory(milestonesFacade: facade);
        using var client = factory.CreateClient();

        var httpResponse = await client.GetAsync($"/projects/{Guid.NewGuid()}/milestones");

        Assert.Equal(HttpStatusCode.NotFound, httpResponse.StatusCode);
    }

    [Fact]
    public async Task GetHealth_Returns_200_With_Health_When_Found()
    {
        var response = new ProjectHealthResponse
        {
            ProjectId = Guid.NewGuid(),
            Status = ProjectStatus.Active,
            Health = ProjectHealth.Amber,
            UpdatedAtUtc = DateTimeOffset.UtcNow,
        };
        var facade = new StubProjectHealthFacade(ProjectHealthFacadeResult.Success(response));

        using var factory = CreateFactory(healthFacade: facade);
        using var client = factory.CreateClient();

        var httpResponse = await client.GetAsync($"/projects/{response.ProjectId}/health");
        var body = await httpResponse.Content.ReadFromJsonAsync<ProjectHealthResponse>();

        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
        Assert.Equal(response.ProjectId, body?.ProjectId);
        Assert.Equal(ProjectHealth.Amber, body?.Health);
    }

    [Fact]
    public async Task GetHealth_Returns_404_When_Not_Found()
    {
        var facade = new StubProjectHealthFacade(ProjectHealthFacadeResult.Failure(ProjectHealthFacadeError.NotFound));

        using var factory = CreateFactory(healthFacade: facade);
        using var client = factory.CreateClient();

        var httpResponse = await client.GetAsync($"/projects/{Guid.NewGuid()}/health");

        Assert.Equal(HttpStatusCode.NotFound, httpResponse.StatusCode);
    }

    [Fact]
    public async Task GetHealth_Returns_401_When_No_Tenant_Context()
    {
        var facade = new StubProjectHealthFacade(ProjectHealthFacadeResult.Failure(ProjectHealthFacadeError.NoTenantContext));

        using var factory = CreateFactory(healthFacade: facade);
        using var client = factory.CreateClient();

        var httpResponse = await client.GetAsync($"/projects/{Guid.NewGuid()}/health");

        Assert.Equal(HttpStatusCode.Unauthorized, httpResponse.StatusCode);
    }
}
