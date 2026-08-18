using System.Linq;
using System.Reflection;
using ProjectLoop.Engagement.Core;
using Xunit;

namespace ProjectLoop.Engagement.Core.Tests;

public class IProjectRepositoryTests
{
    [Fact]
    public void FindByIdAsync_Requires_TenantId_And_ProjectId()
    {
        var method = typeof(IProjectRepository).GetMethod(nameof(IProjectRepository.FindByIdAsync))!;
        var parameters = method.GetParameters();

        Assert.Equal(typeof(Task<Project?>), method.ReturnType);
        Assert.Equal("tenantId", parameters[0].Name);
        Assert.Equal(typeof(Guid), parameters[0].ParameterType);
        Assert.Equal("projectId", parameters[1].Name);
        Assert.Equal(typeof(Guid), parameters[1].ParameterType);
    }

    [Fact]
    public void Contract_Exposes_Only_TenantScoped_Lookup()
    {
        var members = typeof(IProjectRepository)
            .GetMethods()
            .Select(m => m.Name)
            .ToArray();

        Assert.Equal(new[] { nameof(IProjectRepository.FindByIdAsync) }, members);
    }

    private sealed class StubProjectRepository : IProjectRepository
    {
        public Task<Project?> FindByIdAsync(Guid tenantId, Guid projectId, CancellationToken cancellationToken = default) =>
            Task.FromResult<Project?>(null);
    }

    [Fact]
    public void Contract_Is_Implementable()
    {
        IProjectRepository repository = new StubProjectRepository();

        Assert.NotNull(repository);
    }
}
