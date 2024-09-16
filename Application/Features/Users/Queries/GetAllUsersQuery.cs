
using System.Data.Common;
using System.Reflection.Metadata;
using Application.Models.Exceptions;
using Application.Models.Responses;
using Application.Models.Wrappers;
using Domain.Entities;
using Infrastructure.Contexts;
using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Users.Queries;

public class GetAllUsersQuery: PaginationProperties, IRequest<PaginationWrapper<IEnumerable<UserWithRoleResponse>>> {
}

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, PaginationWrapper<IEnumerable<UserWithRoleResponse>>>
{
    private readonly ApplicationContext _context;
    public GetAllUsersQueryHandler(ApplicationContext context)
    {
        this._context = context;
    }

    public async Task<PaginationWrapper<IEnumerable<UserWithRoleResponse>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await (
            from user in this._context.Users.Include(u => u.Role)
            select new UserWithRoleResponse{
                UserId = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = new RoleResponse {
                    Name = user.Role.Name,
                    RoleId = user.Role.Id
                }
            }
        )
        .Skip((request.PageNumber -1) * request.PageSize)
        .Take(request.PageSize)
        .OrderBy(e => e.Name)
        .ToListAsync(cancellationToken: cancellationToken);

        var totalRecords = await _context.Users.CountAsync(cancellationToken);

        return new PaginationWrapper<IEnumerable<UserWithRoleResponse>>{
            Data = users,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalRecords = totalRecords
        };
    }
}