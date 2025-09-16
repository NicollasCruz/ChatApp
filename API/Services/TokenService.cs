using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;

public class TokenService
{
    private readonly IConfiguration _config;
    public TokenService(IConfiguration config)
    {
        _config = config;
    }

    public string GenerateToken(string userId, string userName)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_config["JWTSetting:SecurityKey"]!);

        var claim = new[]
        {
            new Claim("nameId", userId),
            new Claim(JwtRegisteredClaimNames.UniqueName, userName)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claim),
            Expires = DateTime.UtcNow.AddDays(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
