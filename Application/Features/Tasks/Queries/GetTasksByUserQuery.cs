
using Application.Models.Exceptions;
using Application.Models.Responses;
using Application.Models.Wrappers;
using Domain.Entities;
using Infrastructure.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Tasks.Queries;

public class GetTasksByUserQuery: PaginationProperties, IRequest<PaginationWrapper<List<TaskResponse>>>
{
    public string FromTokenEmail { get; set; } = default!;
}
public class GetTasksByUserQueryHandler : IRequestHandler<GetTasksByUserQuery, PaginationWrapper<List<TaskResponse>>>
{
    private readonly ApplicationContext _context;

    public GetTasksByUserQueryHandler(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<PaginationWrapper<List<TaskResponse>>> Handle(GetTasksByUserQuery request, CancellationToken cancellationToken)
    {
        var userToGet = 
            await _context.Users.Where(u => u.Email == request.FromTokenEmail).FirstOrDefaultAsync(cancellationToken)
            ?? throw new ApiException("User not found");

        var allTasks = await (
            from task in _context.TaskJobs
                join user in _context.Users on task.UserId equals user.Id
                join state in _context.States on task.StateId equals state.Id
            where task.UserId == userToGet.Id
            select new TaskResponse {
                Name = task.Name,
                TaskId = task.Id,
                State = new StateResponse {
                    StateId = state.Id,
                    Name = state.Name ?? "",
                },
                User = new UserResponse {
                    UserId = user.Id,
                    Email = user.Email,
                    Name = user.Name
                }
            }
        )
        .Skip((request.PageNumber - 1) * request.PageSize)
        .Take(request.PageSize)
        .OrderBy(e => e.Name)
        .ToListAsync(cancellationToken: cancellationToken);

        var totalRecords = 
            await _context.TaskJobs
                .Where(t => t.UserId == userToGet.Id)
                .CountAsync(cancellationToken);

        var response = new PaginationWrapper<List<TaskResponse>> {
            Data = allTasks,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalRecords = totalRecords
        };

        return response;
    }
}