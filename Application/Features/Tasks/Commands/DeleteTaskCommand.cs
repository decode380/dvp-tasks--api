
using Application.Models.Exceptions;
using Infrastructure.Contexts;
using MediatR;

namespace Application.Features.Tasks.Commands;

public class DeleteTaskCommand: IRequest<int>
{
    public int TaskId { get; set; }
}

public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand, int>
{
    private readonly ApplicationContext _context;
    public DeleteTaskCommandHandler(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
    {
        var taskToRemove = await _context.TaskJobs.FindAsync(request.TaskId) ?? throw new ApiException("Task not found");
        _context.TaskJobs.Remove(taskToRemove);
        await _context.SaveChangesAsync();

        return taskToRemove.Id;
    }
}