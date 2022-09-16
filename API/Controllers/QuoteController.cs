using API.Services;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers;

public static class QuoteController
{
    public static void AddQuoteEndpoints(this WebApplication app)
    {
        app.MapGet("/quote/top", [Authorize(Roles = "User")]async (QuoteService service) =>
        {
            var count = await service.GetCount();
            return Task.FromResult(Results.Ok(count));
        });
        app.MapGet("/quote/list", [Authorize(Roles = "User,Admin")](QuoteService service) => Task.FromResult(Results.NoContent()));
        app.MapPut("/quote/vote/{id:guid}", [Authorize(Roles = "User,Admin")](Guid id, QuoteService service) => Task.FromResult(Results.NoContent()));
        app.MapPut("/quote/reset", [Authorize(Roles = "Admin")](QuoteService service) => Task.FromResult(Results.NoContent()));
    }
}