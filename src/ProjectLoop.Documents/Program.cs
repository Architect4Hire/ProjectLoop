using ProjectLoop.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddAzureBlobServiceClient("documents-blob");

var app = builder.Build();

app.MapDefaultHealthChecks();

await app.RunAsync();

