
using Application.Models.Exceptions;
using Application.Models.Responses;
using Infrastructure.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Users.Queries;

public class GetUserByEmailQuery: IRequest<UserWithRoleResponse>
{
    public string Email { get; set; } = default!;
}

public class GetUserByEmailQueryHandler : IRequestHandler<GetUserByEmailQuery, UserWithRoleResponse>
{
    private readonly ApplicationContext _context;

    public GetUserByEmailQueryHandler(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<UserWithRoleResponse> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
    {
        var userToFind = await(
            from user in _context.Users
            where user.Email == request.Email
            select user 
        )
        .Include(u => u.Role)
        .FirstOrDefaultAsync(cancellationToken) ?? throw new ApiException("User not found");

        return new UserWithRoleResponse {
            UserId = userToFind.Id,
            Email = userToFind.Email,
            Name = userToFind.Name,
            Role = new RoleResponse {
                Name = userToFind.Role.Name,
                RoleId = userToFind.Role.Id,
            }
        };
    }
}