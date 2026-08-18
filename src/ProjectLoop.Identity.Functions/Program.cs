using Azure.Messaging.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProjectLoop.Identity.Core;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((context, services) =>
    {
        services.AddDbContext<IdentityDbContext>(options =>
            options.UseSqlServer(context.Configuration.GetConnectionString("identitydb")));

        services.AddOptions<OutboxServiceBusOptions>()
            .Bind(context.Configuration.GetSection(OutboxServiceBusOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton(_ => new ServiceBusClient(context.Configuration.GetConnectionString("servicebus")));
        services.AddScoped<IOutboxMessageRepository, OutboxMessageRepository>();
        services.AddScoped<IOutboxMessagePublisher, ServiceBusOutboxMessagePublisher>();
        services.AddScoped<IOutboxRelay, OutboxRelay>();
    })
    .Build();

host.Run();
