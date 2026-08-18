using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProjectLoop.Documents.Core;
using Xunit;

namespace ProjectLoop.Documents.Tests;

/// <summary>
/// Exercises the full Controller -> Facade -> Data -> Repository -> DbContext
/// chain (no stubbed facade) to prove a document can never be written
/// without a server-established tenant context, and that the tenant a
/// document lands under always comes from that context rather than any
/// client-supplied value — the upload contract has no TenantId field at all.
/// </summary>
public class DocumentUploadCrossTenantDenialTests
{
    private const string TestScheme = "Test";

    private sealed class FakeBlobDocumentStore : IBlobDocumentStore
    {
        public Task<string> PutAsync(Stream content, string contentType, CancellationToken cancellationToken = default) =>
            Task.FromResult(Guid.NewGuid().ToString("n"));

        public Task<Stream> OpenReadAsync(string objectKey, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task DeleteIfOrphanAsync(string objectKey, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;
    }

    private sealed class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public static Guid? TenantId { get; set; }

        public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, "user-1") };
            if (TenantId is { } tenantId)
            {
                claims.Add(new Claim("tenant_id", tenantId.ToString()));
            }

            var identity = new ClaimsIdentity(claims, TestScheme);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, TestScheme);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }

    private static WebApplicationFactory<Program> CreateFactory(string databaseName, Guid? tenantId)
    {
        TestAuthHandler.TenantId = tenantId;

        return new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseSetting(
                "ConnectionStrings:documentsdb",
                "Server=(localdb)\\MSSQLLocalDB;Database=ProjectLoopDocuments_UnusedInCrossTenantTest;Trusted_Connection=True;TrustServerCertificate=True;");
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

                services.RemoveAll<IBlobDocumentStore>();
                services.AddSingleton<IBlobDocumentStore, FakeBlobDocumentStore>();
            });
        });
    }

    private static MultipartFormDataContent CreateUploadContent()
    {
        var form = new MultipartFormDataContent
        {
            { new StringContent(Guid.NewGuid().ToString()), "ProjectId" },
            { new StringContent("Statement of Work"), "Title" },
            { new StringContent("Contract"), "Category" },
            { new StringContent("Internal"), "Visibility" },
        };

        var fileContent = new ByteArrayContent(Encoding.UTF8.GetBytes("cross-tenant-denial-content"));
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
        form.Add(fileContent, "File", "sow.pdf");

        return form;
    }

    [Fact]
    public async Task Upload_Without_Tenant_Context_Is_Denied_And_Persists_No_Document()
    {
        var databaseName = Guid.NewGuid().ToString();
        using var factory = CreateFactory(databaseName, tenantId: null);
        using var client = factory.CreateClient();

        var response = await client.PostAsync("/documents", CreateUploadContent());

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DocumentsDbContext>();
        Assert.Equal(0, await dbContext.Documents.CountAsync());
    }

    [Fact]
    public async Task Upload_Always_Persists_Under_The_Authenticated_Tenant()
    {
        var databaseName = Guid.NewGuid().ToString();
        var tenantId = Guid.NewGuid();
        using var factory = CreateFactory(databaseName, tenantId);
        using var client = factory.CreateClient();

        var response = await client.PostAsync("/documents", CreateUploadContent());
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DocumentsDbContext>();
        var document = await dbContext.Documents.SingleAsync();

        Assert.Equal(tenantId, document.TenantId);
    }
}
