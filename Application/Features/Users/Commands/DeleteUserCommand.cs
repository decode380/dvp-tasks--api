
using Application.Models.Exceptions;
using Infrastructure.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Users.Commands;

public class DeleteUserCommand: IRequest<int>
{
    public int UserId { get; set; }
}

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, int>
{
    private readonly ApplicationContext _context;
    public DeleteUserCommandHandler(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var userToDelete = await(
            from user in _context.Users
            where user.Id == request.UserId
            select user
        ).FirstOrDefaultAsync(cancellationToken: cancellationToken) ?? throw new ApiException("User not found");

        _context.Users.Remove(userToDelete);
        await _context.SaveChangesAsync();

        return userToDelete.Id;
    }
}