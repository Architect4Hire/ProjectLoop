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
using ProjectLoop.Documents.Core;
using Xunit;

namespace ProjectLoop.Documents.Tests;

public class DocumentsControllerHttpTests
{
    private const string TestScheme = "Test";

    private sealed class StubDocumentUploadFacade : IDocumentUploadFacade
    {
        private readonly DocumentUploadFacadeResult _result;

        public StubDocumentUploadFacade(DocumentUploadFacadeResult result) => _result = result;

        public Task<DocumentUploadFacadeResult> UploadAsync(
            UploadDocumentMetadata metadata,
            Stream content,
            string originalFileName,
            string mimeType,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(_result);
    }

    private sealed class StubDocumentListFacade : IDocumentListFacade
    {
        private readonly DocumentListFacadeResult _result;

        public StubDocumentListFacade(DocumentListFacadeResult result) => _result = result;

        public Task<DocumentListFacadeResult> ListAsync(DocumentListQuery query, CancellationToken cancellationToken = default) =>
            Task.FromResult(_result);
    }

    private sealed class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public static bool IncludeTenantClaim { get; set; } = true;

        public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, "user-1") };
            if (IncludeTenantClaim)
            {
                claims.Add(new Claim("tenant_id", Guid.NewGuid().ToString()));
            }

            var identity = new ClaimsIdentity(claims, TestScheme);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, TestScheme);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }

    private static WebApplicationFactory<Program> CreateFactory(
        IDocumentUploadFacade? uploadFacade = null,
        IDocumentListFacade? listFacade = null,
        bool includeTenantClaim = true)
    {
        TestAuthHandler.IncludeTenantClaim = includeTenantClaim;

        return new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseSetting(
                "ConnectionStrings:documentsdb",
                "Server=(localdb)\\MSSQLLocalDB;Database=ProjectLoopDocuments_UnusedInHttpTests;Trusted_Connection=True;TrustServerCertificate=True;");
            builder.UseSetting("ConnectionStrings:documents-blob", "UseDevelopmentStorage=true");

            builder.ConfigureServices(services =>
            {
                services.AddAuthentication(TestScheme)
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestScheme, _ => { });

                if (uploadFacade is not null)
                {
                    services.RemoveAll<IDocumentUploadFacade>();
                    services.AddScoped(_ => uploadFacade);
                }

                if (listFacade is not null)
                {
                    services.RemoveAll<IDocumentListFacade>();
                    services.AddScoped(_ => listFacade);
                }
            });
        });
    }

    private static MultipartFormDataContent CreateUploadContent(
        Guid projectId,
        string title = "Statement of Work",
        string category = "Contract",
        string visibility = "Internal",
        string fileName = "sow.pdf",
        string content = "document content")
    {
        var form = new MultipartFormDataContent
        {
            { new StringContent(projectId.ToString()), "ProjectId" },
            { new StringContent(title), "Title" },
            { new StringContent(category), "Category" },
            { new StringContent(visibility), "Visibility" },
        };

        var fileContent = new ByteArrayContent(System.Text.Encoding.UTF8.GetBytes(content));
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");
        form.Add(fileContent, "File", fileName);

        return form;
    }

    [Fact]
    public async Task Upload_Returns_201_With_Result_On_Success()
    {
        var uploadResult = new UploadDocumentResult
        {
            DocumentId = Guid.NewGuid(),
            VersionId = Guid.NewGuid(),
            VersionNumber = 1,
            Title = "Statement of Work",
            Category = "Contract",
            Status = DocumentStatus.Draft,
            Visibility = DocumentVisibility.Internal,
            SizeBytes = 1024,
            CreatedAtUtc = DateTimeOffset.UtcNow,
        };
        var facade = new StubDocumentUploadFacade(DocumentUploadFacadeResult.Success(uploadResult));

        using var factory = CreateFactory(uploadFacade: facade);
        using var client = factory.CreateClient();

        using var httpResponse = await client.PostAsync("/documents", CreateUploadContent(Guid.NewGuid()));
        var body = await httpResponse.Content.ReadFromJsonAsync<UploadDocumentResult>();

        Assert.Equal(HttpStatusCode.Created, httpResponse.StatusCode);
        Assert.Equal(uploadResult.DocumentId, body?.DocumentId);
    }

    [Fact]
    public async Task Upload_Returns_401_When_No_Tenant_Claim_Present()
    {
        using var factory = CreateFactory(includeTenantClaim: false);
        using var client = factory.CreateClient();

        using var httpResponse = await client.PostAsync("/documents", CreateUploadContent(Guid.NewGuid()));

        Assert.Equal(HttpStatusCode.Unauthorized, httpResponse.StatusCode);
    }

    [Fact]
    public async Task Upload_Returns_415_For_Unsupported_Media_Type()
    {
        var facade = new StubDocumentUploadFacade(DocumentUploadFacadeResult.Failure(DocumentUploadFacadeError.UnsupportedMediaType));

        using var factory = CreateFactory(uploadFacade: facade);
        using var client = factory.CreateClient();

        using var httpResponse = await client.PostAsync("/documents", CreateUploadContent(Guid.NewGuid()));

        Assert.Equal(HttpStatusCode.UnsupportedMediaType, httpResponse.StatusCode);
    }

    [Fact]
    public async Task List_Returns_200_With_Catalog_When_Authorized()
    {
        var projectId = Guid.NewGuid();
        var listResponse = new DocumentListResponse
        {
            Items =
            [
                new DocumentSummaryResponse
                {
                    Id = Guid.NewGuid(),
                    ProjectId = projectId,
                    Title = "Statement of Work",
                    Category = "Contract",
                    Status = DocumentStatus.Draft,
                    Visibility = DocumentVisibility.Internal,
                    HasCurrentVersion = true,
                    UpdatedAtUtc = DateTimeOffset.UtcNow,
                },
            ],
            Page = 1,
            PageSize = 20,
            TotalCount = 1,
        };
        var facade = new StubDocumentListFacade(DocumentListFacadeResult.Success(listResponse));

        using var factory = CreateFactory(listFacade: facade);
        using var client = factory.CreateClient();

        using var httpResponse = await client.GetAsync($"/documents?projectId={projectId}");
        var body = await httpResponse.Content.ReadFromJsonAsync<DocumentListResponse>();

        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
        Assert.Equal(1, body?.TotalCount);
        Assert.Single(body!.Items);
    }

    [Fact]
    public async Task List_Returns_401_When_No_Tenant_Claim_Present()
    {
        using var factory = CreateFactory(includeTenantClaim: false);
        using var client = factory.CreateClient();

        using var httpResponse = await client.GetAsync($"/documents?projectId={Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.Unauthorized, httpResponse.StatusCode);
    }
}
