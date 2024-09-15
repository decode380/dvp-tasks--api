
using Application.Models.Exceptions;
using Application.Models.Responses;
using Infrastructure.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace Application.Features.Tasks.Commands;

public class UpdateTaskNameOrAssignationCommand: IRequest<TaskResponse>
{
    public int TaskId { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; } = default!;
}

public class UpdateTaskNameOrAssignationCommandHandler : IRequestHandler<UpdateTaskNameOrAssignationCommand, TaskResponse>
{
    private readonly ApplicationContext _context;

    public UpdateTaskNameOrAssignationCommandHandler(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<TaskResponse> Handle(UpdateTaskNameOrAssignationCommand request, CancellationToken cancellationToken)
    {
        var taskToModify = await (
            from task in _context.TaskJobs
            where task.Id == request.TaskId
            select task
        ).Include(t => t.State)
        .Include(t => t.User)
        .FirstOrDefaultAsync(cancellationToken) ?? throw new ApiException("Task not found");
        
        var userToAssign = taskToModify.User;
        if(taskToModify.UserId != request.UserId) {
            userToAssign = await _context.Users.FindAsync(request.UserId) ?? throw new ApiException("User not found");
            taskToModify.UserId = userToAssign.Id;
        }

        taskToModify.Name = request.Name;
        await _context.SaveChangesAsync();

        return new TaskResponse {
            Name = taskToModify.Name,
            State = new StateResponse {
                Name = taskToModify.State.Name!,
                StateId = taskToModify.State.Id
            },
            TaskId = taskToModify.Id,
            User = new UserResponse {
                Email = userToAssign.Email,
                Name = userToAssign.Name,
                UserId = userToAssign.Id
            }
        };
        
    }
}