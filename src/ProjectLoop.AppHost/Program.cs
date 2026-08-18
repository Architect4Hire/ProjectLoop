using Aspire.Hosting;
using ProjectLoop.ServiceDefaults;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Infrastructure resources.
// Azure resources run against local emulators; this AppHost never provisions
// real Azure infrastructure. Production connections are wired independently.
var sqlServer = builder.AddSqlServer("sqldata");
var redis = builder.AddRedis("redis");
var serviceBus = builder.AddAzureServiceBus("servicebus")
    .RunAsEmulator();
var storage = builder.AddAzureStorage("storage")
    .RunAsEmulator();

var app = builder.Build();

await app.RunAsync();
