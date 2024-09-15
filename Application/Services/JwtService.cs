
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Models.Exceptions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services;

public class JwtSettings
{
    public string SecretKey { get; set; } = default!;
    public string Issuer { get; set; } = default!;
    public string Audience { get; set; } = default!;
}
public interface IJwtService {
    string GenerateJwtToken(string email);
    string GetEmailFromToken(string token);
}


public class JwtService: IJwtService{
    private readonly JwtSettings _jwtSettings;
    private readonly SymmetricSecurityKey _securityKey;
    

    public JwtService(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
        _securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
    }

    public string GenerateJwtToken(string email){

        var credentials = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha256);
        var tokenHandler = new JwtSecurityTokenHandler();

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Email, email),
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(4),
            SigningCredentials = credentials,
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }


    public string GetEmailFromToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            var emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "email");
            
            return emailClaim?.Value ?? throw new ApiException("Token no contain email");
        }
        catch (Exception)
        {
            throw;
        }
    }


}

