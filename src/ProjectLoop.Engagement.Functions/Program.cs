using Azure.Messaging.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProjectLoop.Engagement.Core;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((context, services) =>
    {
        services.AddDbContext<EngagementDbContext>(options =>
            options.UseSqlServer(context.Configuration.GetConnectionString("engagementdb")));

        services.AddOptions<OutboxServiceBusOptions>()
            .Bind(context.Configuration.GetSection(OutboxServiceBusOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton(_ => new ServiceBusClient(context.Configuration.GetConnectionString("servicebus")));
        services.AddScoped<IOutboxMessageRepository, OutboxMessageRepository>();
        services.AddScoped<IOutboxMessagePublisher, ServiceBusOutboxMessagePublisher>();
        services.AddScoped<IOutboxRelay, OutboxRelay>();

        services.AddScoped<IApprovalGrantedMilestoneConsumerTransaction, ApprovalGrantedMilestoneConsumerTransaction>();
        services.AddScoped<IApprovalGrantedMilestoneConsumer, ApprovalGrantedMilestoneConsumer>();

        services.AddScoped<IApprovalRejectedMilestoneConsumerTransaction, ApprovalRejectedMilestoneConsumerTransaction>();
        services.AddScoped<IApprovalRejectedMilestoneConsumer, ApprovalRejectedMilestoneConsumer>();
    })
    .Build();

host.Run();
