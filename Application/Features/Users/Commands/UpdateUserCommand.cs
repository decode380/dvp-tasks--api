
using Application.Models.Exceptions;
using Application.Models.Responses;
using BCrypt.Net;
using Domain.Entities;
using Infrastructure.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Users;

public class UpdateUserCommand: IRequest<UserResponse>
{
    public int Id { get; set; }
    public string Name { get; set; }= default!;
    public string Password { get; set; } = default!;
}
public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserResponse>
{
    private readonly ApplicationContext _context;
    public UpdateUserCommandHandler(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<UserResponse> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        User userToEdit = await (
            from user in _context.Users
            where user.Id == request.Id
            select user
        ).FirstOrDefaultAsync(cancellationToken: cancellationToken) 
            ?? throw new ApiException("User not found");

        userToEdit.Name = request.Name;
        userToEdit.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
        await _context.SaveChangesAsync();

        return new UserResponse{
            UserId = userToEdit.Id,
            Name = userToEdit.Name,
            Email = userToEdit.Email
        };
    }
}