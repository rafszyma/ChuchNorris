using API.Business;

namespace API.Controllers;

public static class UserController
{
    public static void AddUserEndpoints(this WebApplication app)
    {
        app.MapGet("/user/{role}",
            (UserRoles role, UserService service) => Results.Ok(service.GetTokenForRole(role)));
    }
}