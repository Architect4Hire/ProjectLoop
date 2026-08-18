using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using ProjectLoop.Engagement;
using ProjectLoop.Engagement.Core;
using Xunit;

namespace ProjectLoop.Engagement.Tests;

public class TenantContextMiddlewareTests
{
    private static async Task<bool> InvokeAsync(DefaultHttpContext context, ICurrentTenantContextAccessor accessor)
    {
        var nextCalled = false;
        var middleware = new TenantContextMiddleware(_ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        });

        await middleware.InvokeAsync(context, accessor);
        return nextCalled;
    }

    [Fact]
    public async Task Unauthenticated_Request_Returns_401_And_Does_Not_Call_Next()
    {
        var context = new DefaultHttpContext();
        var accessor = new CurrentTenantContextAccessor();

        var nextCalled = await InvokeAsync(context, accessor);

        Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);
        Assert.False(nextCalled);
        Assert.Null(accessor.Current);
    }

    [Fact]
    public async Task Missing_TenantId_Claim_Returns_401_And_Does_Not_Call_Next()
    {
        var context = new DefaultHttpContext { User = AuthenticatedUser("user-1", tenantId: null) };
        var accessor = new CurrentTenantContextAccessor();

        var nextCalled = await InvokeAsync(context, accessor);

        Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);
        Assert.False(nextCalled);
        Assert.Null(accessor.Current);
    }

    [Fact]
    public async Task Valid_Claims_Set_Tenant_Context_And_Call_Next()
    {
        var tenantId = Guid.NewGuid();
        var context = new DefaultHttpContext { User = AuthenticatedUser("user-1", tenantId) };
        var accessor = new CurrentTenantContextAccessor();

        var nextCalled = await InvokeAsync(context, accessor);

        Assert.Equal(StatusCodes.Status200OK, context.Response.StatusCode);
        Assert.True(nextCalled);
        Assert.Equal(tenantId, accessor.Current?.TenantId);
        Assert.Equal("user-1", accessor.Current?.UserId);
    }

    private static ClaimsPrincipal AuthenticatedUser(string userId, Guid? tenantId)
    {
        var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, userId) };
        if (tenantId is not null)
        {
            claims.Add(new Claim("tenant_id", tenantId.Value.ToString()));
        }

        var identity = new ClaimsIdentity(claims, "Test");
        return new ClaimsPrincipal(identity);
    }
}
