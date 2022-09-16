using API.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers;

public static class QuoteController
{
    public static void AddQuoteEndpoints(this WebApplication app)
    {
        app.MapGet("/quote/top", [Authorize(Roles = "User")]async (IQuoteService service) => Task.FromResult(Task.FromResult(Results.Ok(await service.ListTop(10)))));
        app.MapGet("/quote/list", [Authorize(Roles = "User,Admin")]async (IQuoteService service, int? skip, int? take) => Task.FromResult(Results.Ok(await service.ListQuotes(skip, take))));
        app.MapPut("/quote/vote/{id:guid}", [Authorize(Roles = "User,Admin")]async (Guid id, IQuoteService service) =>
        {
            await service.VoteForQuote(id);
            return Task.FromResult(Results.Ok());
        });
        app.MapPut("/quote/reset/{id:guid}", [Authorize(Roles = "Admin")]async (Guid id, IQuoteService service) =>
        {
            await service.ResetVotes(id);
            return Task.FromResult(Results.Ok());
        });
    }
}