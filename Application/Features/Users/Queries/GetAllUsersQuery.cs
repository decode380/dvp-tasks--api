
using System.Data.Common;
using System.Reflection.Metadata;
using Application.Models.Responses;
using Application.Models.Wrappers;
using Infrastructure.Contexts;
using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Users.Queries;

public class GetAllUsersQuery: PaginationProperties, IRequest<PaginationWrapper<IEnumerable<UserResponse>>> {
}

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, PaginationWrapper<IEnumerable<UserResponse>>>
{
    private readonly ApplicationContext _context;
    public GetAllUsersQueryHandler(ApplicationContext context)
    {
        this._context = context;
    }

    public async Task<PaginationWrapper<IEnumerable<UserResponse>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await (
            from user in this._context.Users
            select new UserResponse{
                UserId = user.Id,
                Name = user.Name,
                Email = user.Email
            }
        )
        .Skip((request.PageNumber -1) * request.PageSize)
        .Take(request.PageSize)
        .ToListAsync(cancellationToken: cancellationToken);

        var totalRecords = await _context.Users.CountAsync(cancellationToken);

        return new PaginationWrapper<IEnumerable<UserResponse>>{
            Data = users,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalRecords = totalRecords
        };
    }
}