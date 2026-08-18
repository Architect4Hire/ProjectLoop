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
using ProjectLoop.Identity.Core;
using Xunit;

namespace ProjectLoop.Identity.Tests;

public class InvitationsControllerHttpTests
{
    private const string TestScheme = "Test";

    private sealed class StubTenantContextResolver : ITenantContextResolver
    {
        private readonly TenantContextResolution _result;

        public StubTenantContextResolver(TenantContextResolution result) => _result = result;

        public Task<TenantContextResolution> ResolveAsync(string userId, Guid requestedTenantId, CancellationToken cancellationToken = default) =>
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
            var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, "user-1") }, TestScheme);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, TestScheme);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }

    private sealed class StubInvitationCreationFacade : IInvitationCreationFacade
    {
        private readonly InvitationCreationFacadeResult _result;

        public StubInvitationCreationFacade(InvitationCreationFacadeResult result) => _result = result;

        public Task<InvitationCreationFacadeResult> CreateInvitationAsync(string email, CancellationToken cancellationToken = default) =>
            Task.FromResult(_result);
    }

    private static WebApplicationFactory<Program> CreateFactory(
        ITenantContextResolver resolver,
        IInvitationCreationFacade? facade = null)
    {
        return new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseSetting(
                "ConnectionStrings:identitydb",
                "Server=(localdb)\\MSSQLLocalDB;Database=ProjectLoopIdentity_UnusedInHttpTests;Trusted_Connection=True;TrustServerCertificate=True;");

            builder.ConfigureServices(services =>
            {
                services.AddAuthentication(TestScheme)
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestScheme, _ => { });

                services.RemoveAll<ITenantContextResolver>();
                services.AddScoped(_ => resolver);

                if (facade is not null)
                {
                    services.RemoveAll<IInvitationCreationFacade>();
                    services.AddScoped(_ => facade);
                }
            });
        });
    }

    [Fact]
    public async Task Create_Returns_201_When_Authorized_Admin_Sends_Valid_Email()
    {
        var tenantId = Guid.NewGuid();
        var tenantContext = new TenantContext { TenantId = tenantId, UserId = "user-1", Role = TenantMembershipRole.Admin };
        var resolver = new StubTenantContextResolver(TenantContextResolution.Allowed(tenantContext));

        var invitation = new ClientInvitation
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Email = "client@example.com",
            TokenHash = "hashed-token",
            ExpiresAtUtc = DateTimeOffset.UtcNow.AddDays(7),
            Status = ClientInvitationStatus.Pending,
            InvitedByUserId = "user-1",
            CreatedAtUtc = DateTimeOffset.UtcNow,
            UpdatedAtUtc = DateTimeOffset.UtcNow,
        };
        var facade = new StubInvitationCreationFacade(
            InvitationCreationFacadeResult.Success(new InvitationCreationResult(invitation, "raw-token")));

        using var factory = CreateFactory(resolver, facade);
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant-Id", tenantId.ToString());

        var response = await client.PostAsJsonAsync("/invitations", new { Email = "client@example.com" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Create_Returns_403_When_Caller_Is_Not_Admin()
    {
        var tenantId = Guid.NewGuid();
        var tenantContext = new TenantContext { TenantId = tenantId, UserId = "user-1", Role = TenantMembershipRole.Member };
        var resolver = new StubTenantContextResolver(TenantContextResolution.Allowed(tenantContext));

        using var factory = CreateFactory(resolver);
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant-Id", tenantId.ToString());

        var response = await client.PostAsJsonAsync("/invitations", new { Email = "client@example.com" });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Create_Returns_403_When_Middleware_Denies_Tenant_Membership()
    {
        var resolver = new StubTenantContextResolver(TenantContextResolution.Denied(TenantContextDenialReason.MembershipNotFound));

        using var factory = CreateFactory(resolver);
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant-Id", Guid.NewGuid().ToString());

        var response = await client.PostAsJsonAsync("/invitations", new { Email = "client@example.com" });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
