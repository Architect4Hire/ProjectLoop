using Aspire.Hosting;
using ProjectLoop.ServiceDefaults;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Infrastructure resources
var sqlServer = builder.AddSqlServer("sqldata");
var redis = builder.AddRedis("redis");
var serviceBus = builder.AddAzureServiceBus("servicebus");
var storage = builder.AddAzureStorage("storage");

var app = builder.Build();

await app.RunAsync();
