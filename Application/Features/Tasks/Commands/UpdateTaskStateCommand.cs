
using Application.Models.Exceptions;
using Application.Models.Requests;
using Application.Models.Responses;
using Infrastructure.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Application.Features.Tasks.Commands;

public class UpdateTaskStateCommand: UpdateTaskStateRequest, IRequest<TaskResponse>
{
    public string? FromTokenEmail { get; set; } = default!;
    public bool CheckSameUser { get; set; }
}

public class UpdateTaskStateCommandHandler : IRequestHandler<UpdateTaskStateCommand, TaskResponse>
{
    private readonly ApplicationContext _context;
    public UpdateTaskStateCommandHandler(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<TaskResponse> Handle(UpdateTaskStateCommand request, CancellationToken cancellationToken)
    {

        var taskToModify = await (
            from task in _context.TaskJobs
            where task.Id == request.TaskId
            select task
        )
        .Include(t => t.User)
        .Include(t => t.State)
        .FirstOrDefaultAsync(cancellationToken) ?? throw new ApiException("Task not found");

        if(request.CheckSameUser && taskToModify.User.Email != request.FromTokenEmail) 
            throw new ApiException("User not authorized");

        var stateToChange = taskToModify.State;
        if (taskToModify.StateId != request.StateId)
        {
            stateToChange = await _context.States.FindAsync(new object?[] { request.StateId }, cancellationToken: cancellationToken) ?? throw new ApiException("State not found");
            taskToModify.StateId = request.StateId;
            await _context.SaveChangesAsync(cancellationToken);
        }
        
        return new TaskResponse {
            Name = taskToModify.Name,
            State = new StateResponse {
                Name = stateToChange.Name!,
                StateId = stateToChange.Id
            },
            TaskId = taskToModify.Id,
            User = new UserResponse {
                Email = taskToModify.User.Email,
                Name = taskToModify.User.Name,
                UserId = taskToModify.User.Id
            }
        };

    }
}