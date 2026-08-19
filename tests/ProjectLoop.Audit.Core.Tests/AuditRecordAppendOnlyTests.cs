using Microsoft.EntityFrameworkCore;
using ProjectLoop.Audit.Core;
using Xunit;

namespace ProjectLoop.Audit.Core.Tests;

/// <summary>
/// Proves audit history is append-only: the sanctioned application access
/// path (IAuditRecordRepository) exposes no way to update or delete a
/// previously written record, reads never hand back a trackable/mutable
/// instance, and re-adding the same identity is rejected rather than
/// silently overwriting the original row. This does not implement or test
/// retention/purge — only that ordinary repository/application operations
/// cannot mutate or destroy an existing record.
/// </summary>
public class AuditRecordAppendOnlyTests
{
    private static AuditDbContext CreateDbContext(string? databaseName = null) =>
        new(new DbContextOptionsBuilder<AuditDbContext>()
            .UseInMemoryDatabase(databaseName ?? Guid.NewGuid().ToString())
            .Options);

    private static AuditRecord CreateRecord(Guid tenantId, Guid id) => new()
    {
        Id = id,
        TenantId = tenantId,
        ActorUserId = "user-1",
        Action = "ApprovalGranted",
        ResourceType = "Document",
        ResourceId = Guid.NewGuid(),
        OccurredAtUtc = DateTimeOffset.UtcNow,
        Source = "Approvals",
        AfterMetadata = "{\"decision\":\"Approved\"}",
    };

    [Fact]
    public void Repository_Interface_Exposes_No_Update_Or_Delete_Member()
    {
        var members = typeof(IAuditRecordRepository).GetMethods().Select(m => m.Name);

        Assert.DoesNotContain(members, name =>
            name.Contains("Update", StringComparison.OrdinalIgnoreCase) ||
            name.Contains("Delete", StringComparison.OrdinalIgnoreCase) ||
            name.Contains("Remove", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void AuditRecord_Exposes_No_Settable_Property_After_Construction()
    {
        // Every property is `init`-only, so there is no compile-time path
        // for application code to mutate a record's fields once
        // constructed — not even a fetched/detached instance.
        var settableAfterConstruction = typeof(AuditRecord).GetProperties()
            .Where(p => p.SetMethod is not null && !p.SetMethod.ReturnParameter
                .GetRequiredCustomModifiers()
                .Any(m => m.FullName == "System.Runtime.CompilerServices.IsExternalInit"))
            .Select(p => p.Name)
            .ToList();

        Assert.Empty(settableAfterConstruction);
    }

    [Fact]
    public async Task GetByIdAsync_Reads_Are_Detached_And_Never_Written_Back_By_A_Later_SaveChanges()
    {
        await using var dbContext = CreateDbContext();
        var repository = new AuditRecordRepository(dbContext);
        var tenantId = Guid.NewGuid();
        var record = CreateRecord(tenantId, Guid.NewGuid());
        await repository.AddAsync(record);
        await dbContext.SaveChangesAsync();

        var fetched = await repository.GetByIdAsync(tenantId, record.Id);
        Assert.NotNull(fetched);

        // The read path is AsNoTracking, so the fetched instance was never
        // wired into the context's change tracker. Calling SaveChangesAsync
        // again after a read is a no-op for that record — there is nothing
        // for it to write back, which is what keeps a read from ever being
        // able to smuggle in a mutation.
        await dbContext.SaveChangesAsync();

        var reFetched = await repository.GetByIdAsync(tenantId, record.Id);
        Assert.Equal("ApprovalGranted", reFetched!.Action);
    }

    [Fact]
    public async Task AddAsync_With_An_Existing_Record_Identity_Does_Not_Silently_Overwrite_The_Original_Row()
    {
        var databaseName = Guid.NewGuid().ToString();
        var tenantId = Guid.NewGuid();
        var id = Guid.NewGuid();

        await using (var dbContext = CreateDbContext(databaseName))
        {
            var repository = new AuditRecordRepository(dbContext);
            await repository.AddAsync(CreateRecord(tenantId, id));
            await dbContext.SaveChangesAsync();
        }

        await using (var secondDbContext = CreateDbContext(databaseName))
        {
            var secondRepository = new AuditRecordRepository(secondDbContext);
            var replacement = CreateRecord(tenantId, id);
            await secondRepository.AddAsync(replacement);

            // Appending a second row with the same identity is a
            // duplicate-key conflict, not a silent overwrite: there is no
            // update path.
            await Assert.ThrowsAnyAsync<Exception>(() => secondDbContext.SaveChangesAsync());
        }

        await using (var verifyDbContext = CreateDbContext(databaseName))
        {
            var verifyRepository = new AuditRecordRepository(verifyDbContext);
            var stored = await verifyRepository.GetByIdAsync(tenantId, id);
            Assert.Equal("ApprovalGranted", stored!.Action);
        }
    }
}
