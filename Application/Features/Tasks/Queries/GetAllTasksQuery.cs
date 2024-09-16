
using Application.Models.Responses;
using Application.Models.Wrappers;
using Domain.Entities;
using Infrastructure.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Tasks.Queries;

public class GetAllTasksQuery: PaginationProperties, IRequest<PaginationWrapper<List<TaskResponse>>>
{}

public class GetAllTasksQueryHandler : IRequestHandler<GetAllTasksQuery, PaginationWrapper<List<TaskResponse>>>
{
    private readonly ApplicationContext _context;

    public GetAllTasksQueryHandler(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<PaginationWrapper<List<TaskResponse>>> Handle(GetAllTasksQuery request, CancellationToken cancellationToken)
    {
        var allTasks = await (
            from task in _context.TaskJobs
            join user in _context.Users on task.UserId equals user.Id
            join state in _context.States on task.StateId equals state.Id
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

        var totalRecords = await _context.TaskJobs.CountAsync(cancellationToken);

        var response = new PaginationWrapper<List<TaskResponse>> {
            Data = allTasks,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalRecords = totalRecords
        };

        return response;
    }
}