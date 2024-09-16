
using Application.Models.Exceptions;
using BCrypt.Net;
using Domain.Entities;
using Infrastructure.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Users.Commands;

public class CreateUserCommand: IRequest<int>
{
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string RoleId { get; set; } = default!;
}


public class CreatedUserCommandHandler : IRequestHandler<CreateUserCommand, int>
{
    private readonly ApplicationContext _context;
    public CreatedUserCommandHandler(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        bool userExists = await (
            from user in _context.Users
            where user.Email == request.Email
            select user
        ).AnyAsync(cancellationToken: cancellationToken);
        if(userExists) throw new ApiException("User already exists");

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
        User newUser = new(){
            Email = request.Email,
            Password = hashedPassword,
            Name = request.Name,
        };

        var roleToAssign = 
            await _context.Roles
                .Where(r => r.Id == request.RoleId)
                .FirstOrDefaultAsync(cancellationToken) ?? throw new ApiException("Role not found");

        newUser.RoleId = roleToAssign.Id;

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync(cancellationToken);
        return newUser.Id;
    }
}