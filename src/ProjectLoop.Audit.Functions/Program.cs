using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProjectLoop.Audit.Core;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((context, services) =>
    {
        services.AddDbContext<AuditDbContext>(options =>
            options.UseSqlServer(context.Configuration.GetConnectionString("auditdb")));

        services.AddScoped<IAuditRecordRepository, AuditRecordRepository>();

        services.AddScoped<IApprovalRequestedAuditConsumerTransaction, ApprovalRequestedAuditConsumerTransaction>();
        services.AddScoped<IApprovalRequestedAuditConsumer, ApprovalRequestedAuditConsumer>();

        services.AddScoped<IApprovalGrantedAuditConsumerTransaction, ApprovalGrantedAuditConsumerTransaction>();
        services.AddScoped<IApprovalGrantedAuditConsumer, ApprovalGrantedAuditConsumer>();

        services.AddScoped<IApprovalRejectedAuditConsumerTransaction, ApprovalRejectedAuditConsumerTransaction>();
        services.AddScoped<IApprovalRejectedAuditConsumer, ApprovalRejectedAuditConsumer>();
    })
    .Build();

host.Run();
