using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProjectLoop.Notifications.Core;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((context, services) =>
    {
        services.AddDbContext<NotificationsDbContext>(options =>
            options.UseSqlServer(context.Configuration.GetConnectionString("notificationsdb")));

        services.AddScoped<IEmailNotificationSender, DevelopmentEmailNotificationSender>();

        services.AddScoped<IApprovalRequestedNotificationConsumerTransaction, ApprovalRequestedNotificationConsumerTransaction>();
        services.AddScoped<IApprovalRequestedNotificationConsumer, ApprovalRequestedNotificationConsumer>();

        services.AddScoped<IApprovalGrantedNotificationConsumerTransaction, ApprovalGrantedNotificationConsumerTransaction>();
        services.AddScoped<IApprovalGrantedNotificationConsumer, ApprovalGrantedNotificationConsumer>();

        services.AddScoped<IApprovalRejectedNotificationConsumerTransaction, ApprovalRejectedNotificationConsumerTransaction>();
        services.AddScoped<IApprovalRejectedNotificationConsumer, ApprovalRejectedNotificationConsumer>();
    })
    .Build();

host.Run();
