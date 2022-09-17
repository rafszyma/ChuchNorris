using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Business.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;


public class UserService
{

    private readonly IOptionsMonitor<TokenSettings> _optionsDelegate;

    public UserService(IOptionsMonitor<TokenSettings> optionsDelegate)
    {
        _optionsDelegate = optionsDelegate;
    }

    public string GetTokenForRole(UserRoles role)
    {
        var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_optionsDelegate.CurrentValue.Secret));
            
        var tokenHandler = new JwtSecurityTokenHandler();
        
        var claims = new List<Claim>
        {
            new(ClaimTypes.Role, role.ToString())
        };

        
        var expires = DateTime.UtcNow.AddHours(4);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expires,
            Issuer = _optionsDelegate.CurrentValue.Issuer,
            Audience = _optionsDelegate.CurrentValue.Audience,
            SigningCredentials = new SigningCredentials(mySecurityKey, SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        

        return tokenHandler.WriteToken(token);
    }
}