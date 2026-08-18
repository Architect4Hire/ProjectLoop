using ProjectLoop.Documents;
using ProjectLoop.Documents.Core;
using ProjectLoop.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddAzureBlobServiceClient("documents-blob");
builder.AddSqlServerDbContext<DocumentsDbContext>("documentsdb");

builder.Services.AddOptions<DocumentBlobStorageOptions>()
    .Bind(builder.Configuration.GetSection(DocumentBlobStorageOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();
builder.Services.AddOptions<DocumentUploadLimitsOptions>()
    .Bind(builder.Configuration.GetSection(DocumentUploadLimitsOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddScoped<ICurrentTenantContextAccessor, CurrentTenantContextAccessor>();
builder.Services.AddSingleton<IBlobDocumentStore, AzureBlobDocumentStore>();
builder.Services.AddScoped<IDocumentUploadSizeValidator, DocumentUploadSizeValidator>();
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
builder.Services.AddScoped<IDocumentVersionRepository, DocumentVersionRepository>();
builder.Services.AddScoped<IDocumentUploadTransaction, DocumentUploadTransaction>();
builder.Services.AddScoped<IDocumentUploadFacade, DocumentUploadFacade>();
builder.Services.AddScoped<IDocumentListFacade, DocumentListFacade>();
builder.Services.AddScoped<IDocumentAddVersionTransaction, DocumentAddVersionTransaction>();
builder.Services.AddScoped<IDocumentAddVersionFacade, DocumentAddVersionFacade>();
builder.Services.AddScoped<IDocumentPublishTransaction, DocumentPublishTransaction>();
builder.Services.AddScoped<IDocumentPublishFacade, DocumentPublishFacade>();
builder.Services.AddScoped<IDocumentDownloadFacade, DocumentDownloadFacade>();

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

