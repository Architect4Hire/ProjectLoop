using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProjectLoop.Documents.Core;
using Xunit;

namespace ProjectLoop.Documents.Tests;

/// <summary>
/// Exercises the full Controller -> Facade -> Repository -> DbContext chain
/// (no stubbed facade) to prove tenant A can never enumerate tenant B's
/// document metadata, even when both requests target the same ProjectId.
/// </summary>
public class DocumentListCrossTenantDenialTests
{
    private const string TestScheme = "Test";

    private sealed class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public static Guid TenantId { get; set; }

        public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, "user-1"),
                new("tenant_id", TenantId.ToString()),
            };

            var identity = new ClaimsIdentity(claims, TestScheme);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, TestScheme);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }

    private static WebApplicationFactory<Program> CreateFactory(string databaseName, Guid tenantId)
    {
        TestAuthHandler.TenantId = tenantId;

        return new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseSetting(
                "ConnectionStrings:documentsdb",
                "Server=(localdb)\\MSSQLLocalDB;Database=ProjectLoopDocuments_UnusedInCrossTenantListTest;Trusted_Connection=True;TrustServerCertificate=True;");
            builder.UseSetting("ConnectionStrings:documents-blob", "UseDevelopmentStorage=true");

            builder.ConfigureServices(services =>
            {
                services.AddAuthentication(TestScheme)
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestScheme, _ => { });

                var documentsDbContextDescriptors = services
                    .Where(d => d.ServiceType == typeof(DocumentsDbContext)
                        || (d.ServiceType.IsGenericType && d.ServiceType.GetGenericArguments().Contains(typeof(DocumentsDbContext))))
                    .ToList();
                foreach (var descriptor in documentsDbContextDescriptors)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<DocumentsDbContext>(options => options.UseInMemoryDatabase(databaseName));
            });
        });
    }

    [Fact]
    public async Task TenantA_Cannot_Enumerate_TenantB_Documents_For_The_Same_Project()
    {
        var tenantAId = Guid.NewGuid();
        var tenantBId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var databaseName = Guid.NewGuid().ToString();

        var now = DateTimeOffset.UtcNow;
        var tenantBDocument = new Document
        {
            Id = Guid.NewGuid(),
            TenantId = tenantBId,
            ProjectId = projectId,
            Title = "Tenant B Confidential Document",
            Category = "Contract",
            Status = DocumentStatus.Draft,
            Visibility = DocumentVisibility.Internal,
            CreatedAtUtc = now,
            UpdatedAtUtc = now,
        };

        using var factory = CreateFactory(databaseName, tenantAId);

        using (var scope = factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<DocumentsDbContext>();
            dbContext.Documents.Add(tenantBDocument);
            await dbContext.SaveChangesAsync();
        }

        using var client = factory.CreateClient();

        var response = await client.GetAsync($"/documents?projectId={projectId}");
        var body = await response.Content.ReadFromJsonAsync<DocumentListResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(0, body?.TotalCount);
        Assert.Empty(body!.Items);
    }
}
