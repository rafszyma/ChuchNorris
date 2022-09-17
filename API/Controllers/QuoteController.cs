using API.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers;

public static class QuoteController
{
    public static void AddQuoteEndpoints(this WebApplication app)
    {
        app.MapGet("/quote/top",
             [Authorize(Roles = $"{nameof(UserRoles.User)},{nameof(UserRoles.Admin)}")]
            async (IQuoteService service) => Results.Ok(await service.ListTop(10)));
        
        app.MapGet("/quote/list",  
            [Authorize(Roles = $"{nameof(UserRoles.User)},{nameof(UserRoles.Admin)}")]
            async (IQuoteService service, int? skip, int? take) =>
                Results.Ok(await service.ListQuotes(skip, take)));
        
        app.MapPut("/quote/vote/{id:guid}", [Authorize(Roles = $"{nameof(UserRoles.User)},{nameof(UserRoles.Admin)}")]async (Guid id, IQuoteService service) =>
        {
            var quote = await service.VoteForQuote(id);
            return quote == null ? Results.NotFound() : Results.Ok();
        });
        
        app.MapPut("/quote/reset/{id:guid}", [Authorize(Roles = nameof(UserRoles.Admin))]async (Guid id, IQuoteService service) =>
        {
            var quote = await service.ResetVotes(id);
            return quote == null ? Results.NotFound() : Results.Ok();
        });
    }
}