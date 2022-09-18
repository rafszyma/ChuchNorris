using API.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers;

public static class QuoteController
{
    public static void AddQuoteEndpoints(this WebApplication app)
    {
        app.MapGet("/quote",  
            [Authorize(Roles = $"{nameof(UserRoles.User)},{nameof(UserRoles.Admin)}")]
            async (IQuoteRepository service, int? skip, int? take) =>
                Results.Ok(await service.ListQuotes(skip, take)));
        
        app.MapGet("/quote/top",
            [Authorize(Roles = $"{nameof(UserRoles.User)},{nameof(UserRoles.Admin)}")]
            async (IQuoteRepository service) => Results.Ok(await service.ListTop(10)));
        
        app.MapPut("/quote/{id:guid}/vote", [Authorize(Roles = $"{nameof(UserRoles.User)},{nameof(UserRoles.Admin)}")]async (Guid id, IQuoteRepository service) =>
        {
            var quote = await service.VoteForQuote(id);
            return quote == null ? Results.NotFound() : Results.Ok();
        });
        
        app.MapPut("/quote/{id:guid}/reset", [Authorize(Roles = nameof(UserRoles.Admin))]async (Guid id, IQuoteRepository service) =>
        {
            var quote = await service.ResetVotes(id);
            return quote == null ? Results.NotFound() : Results.Ok();
        });
    }
}