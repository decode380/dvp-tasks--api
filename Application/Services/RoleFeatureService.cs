
using Domain.Entities;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public interface IRoleFeatureService{
    Task<bool> UserHasValidRoleFromToken(string bearerToken, string role);
}

public class RoleFeatureService : IRoleFeatureService
{
    private readonly ApplicationContext _context;
    private readonly IJwtService _jwtService;

    public RoleFeatureService(ApplicationContext context, IJwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    public async Task<bool> UserHasValidRoleFromToken(string bearerToken, string role)
    {
        string userEmail = _jwtService.GetEmailFromToken(bearerToken["Bearer ".Length..].Trim());

        // var userHasRole = await _context.Users
        //     .Where(u => u.Id == idUser)
        //     .SelectMany(u => u.Roles)
        //     .AnyAsync(r => r.Id == role);

        var userHasRole = await (
            from user in _context.Users
                where user.Email == userEmail && user.RoleId == role
            select user
        ).AnyAsync();

        return userHasRole;
    }
}