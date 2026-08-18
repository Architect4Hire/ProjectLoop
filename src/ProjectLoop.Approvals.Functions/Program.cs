using Azure.Messaging.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProjectLoop.Approvals.Core;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((context, services) =>
    {
        services.AddDbContext<ApprovalsDbContext>(options =>
            options.UseSqlServer(context.Configuration.GetConnectionString("approvalsdb")));

        services.AddOptions<OutboxServiceBusOptions>()
            .Bind(context.Configuration.GetSection(OutboxServiceBusOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton(_ => new ServiceBusClient(context.Configuration.GetConnectionString("servicebus")));
        services.AddScoped<IOutboxMessageRepository, OutboxMessageRepository>();
        services.AddScoped<IOutboxMessagePublisher, ServiceBusOutboxMessagePublisher>();
        services.AddScoped<IOutboxRelay, OutboxRelay>();

        services.AddScoped<IApprovalRequestRepository, ApprovalRequestRepository>();
        services.AddScoped<IDocumentPublishedConsumerTransaction, DocumentPublishedConsumerTransaction>();
        services.AddScoped<IDocumentPublishedConsumer, DocumentPublishedConsumer>();
    })
    .Build();

host.Run();
