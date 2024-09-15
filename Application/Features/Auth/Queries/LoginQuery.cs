
using Application.Models.Exceptions;
using Application.Services;
using BCrypt.Net;
using Infrastructure.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Auth.Queries;

public class LoginQuery: IRequest<string>
{
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
}

public class LoginQueryHandler : IRequestHandler<LoginQuery, string>
{
    private readonly ApplicationContext _context;
    private readonly IJwtService _jwtService;

    public LoginQueryHandler(ApplicationContext context, IJwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    public async Task<string> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        var userToLogin = await (
            from user in _context.Users
            where user.Email == request.Email
            select user
        ).FirstOrDefaultAsync(cancellationToken: cancellationToken) ?? throw new ApiException("Incorrect email or password");

        bool validPassword = BCrypt.Net.BCrypt.Verify(request.Password, userToLogin.Password);
        if (!validPassword) throw new ApiException("Incorrect email or password");

        string jwtToken = _jwtService.GenerateJwtToken(userToLogin.Email);
        return jwtToken;
        
    }
}