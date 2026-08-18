using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using ProjectLoop.Identity;
using ProjectLoop.Identity.Core;
using Xunit;

namespace ProjectLoop.Identity.Tests;

public class TenantContextMiddlewareTests
{
    private sealed class StubTenantContextResolver : ITenantContextResolver
    {
        private readonly TenantContextResolution _result;

        public StubTenantContextResolver(TenantContextResolution result) => _result = result;

        public bool Called { get; private set; }

        public Task<TenantContextResolution> ResolveAsync(string userId, Guid requestedTenantId, CancellationToken cancellationToken = default)
        {
            Called = true;
            return Task.FromResult(_result);
        }
    }

    private static async Task<bool> InvokeAsync(
        DefaultHttpContext context,
        ITenantContextResolver resolver,
        ICurrentTenantContextAccessor accessor)
    {
        var nextCalled = false;
        var middleware = new TenantContextMiddleware(_ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        });

        await middleware.InvokeAsync(context, resolver, accessor);
        return nextCalled;
    }

    [Fact]
    public async Task Unauthenticated_Request_Returns_401_And_Does_Not_Call_Next()
    {
        var context = new DefaultHttpContext();
        var resolver = new StubTenantContextResolver(TenantContextResolution.Denied(TenantContextDenialReason.MembershipNotFound));
        var accessor = new CurrentTenantContextAccessor();

        var nextCalled = await InvokeAsync(context, resolver, accessor);

        Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);
        Assert.False(nextCalled);
        Assert.False(resolver.Called);
        Assert.Null(accessor.Current);
    }

    [Fact]
    public async Task Missing_Tenant_Header_Returns_400_And_Does_Not_Call_Next()
    {
        var context = new DefaultHttpContext { User = AuthenticatedUser("user-1") };
        var resolver = new StubTenantContextResolver(TenantContextResolution.Denied(TenantContextDenialReason.MembershipNotFound));
        var accessor = new CurrentTenantContextAccessor();

        var nextCalled = await InvokeAsync(context, resolver, accessor);

        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
        Assert.False(nextCalled);
        Assert.False(resolver.Called);
        Assert.Null(accessor.Current);
    }

    [Fact]
    public async Task Denied_Resolution_Returns_403_And_Does_Not_Call_Next_Or_Fall_Back()
    {
        var context = new DefaultHttpContext { User = AuthenticatedUser("user-1") };
        context.Request.Headers["X-Tenant-Id"] = Guid.NewGuid().ToString();
        var resolver = new StubTenantContextResolver(TenantContextResolution.Denied(TenantContextDenialReason.MembershipNotActive));
        var accessor = new CurrentTenantContextAccessor();

        var nextCalled = await InvokeAsync(context, resolver, accessor);

        Assert.Equal(StatusCodes.Status403Forbidden, context.Response.StatusCode);
        Assert.False(nextCalled);
        Assert.Null(accessor.Current);
    }

    [Fact]
    public async Task Allowed_Resolution_Sets_Tenant_Context_And_Calls_Next()
    {
        var context = new DefaultHttpContext { User = AuthenticatedUser("user-1") };
        var tenantId = Guid.NewGuid();
        context.Request.Headers["X-Tenant-Id"] = tenantId.ToString();

        var tenantContext = new TenantContext { TenantId = tenantId, UserId = "user-1", Role = TenantMembershipRole.Member };
        var resolver = new StubTenantContextResolver(TenantContextResolution.Allowed(tenantContext));
        var accessor = new CurrentTenantContextAccessor();

        var nextCalled = await InvokeAsync(context, resolver, accessor);

        Assert.Equal(StatusCodes.Status200OK, context.Response.StatusCode);
        Assert.True(nextCalled);
        Assert.Same(tenantContext, accessor.Current);
    }

    [Fact]
    public async Task SkipTenantContext_Endpoint_Bypasses_Tenant_Resolution_But_Still_Requires_Authentication()
    {
        var context = new DefaultHttpContext { User = AuthenticatedUser("user-1") };
        var endpoint = new Endpoint(
            requestDelegate: null,
            metadata: new EndpointMetadataCollection(new SkipTenantContextAttribute()),
            displayName: "test-endpoint");
        context.SetEndpoint(endpoint);

        var resolver = new StubTenantContextResolver(TenantContextResolution.Denied(TenantContextDenialReason.MembershipNotFound));
        var accessor = new CurrentTenantContextAccessor();

        var nextCalled = await InvokeAsync(context, resolver, accessor);

        Assert.True(nextCalled);
        Assert.False(resolver.Called);
        Assert.Null(accessor.Current);
    }

    [Fact]
    public async Task SkipTenantContext_Endpoint_Still_Returns_401_When_Unauthenticated()
    {
        var context = new DefaultHttpContext();
        var endpoint = new Endpoint(
            requestDelegate: null,
            metadata: new EndpointMetadataCollection(new SkipTenantContextAttribute()),
            displayName: "test-endpoint");
        context.SetEndpoint(endpoint);

        var resolver = new StubTenantContextResolver(TenantContextResolution.Denied(TenantContextDenialReason.MembershipNotFound));
        var accessor = new CurrentTenantContextAccessor();

        var nextCalled = await InvokeAsync(context, resolver, accessor);

        Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);
        Assert.False(nextCalled);
    }

    private static ClaimsPrincipal AuthenticatedUser(string userId)
    {
        var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId) }, "Test");
        return new ClaimsPrincipal(identity);
    }
}
