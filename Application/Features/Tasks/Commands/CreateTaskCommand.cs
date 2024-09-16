
using Application.Models.Exceptions;
using Application.Models.Responses;
using Domain.Entities;
using Infrastructure.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Tasks.Commands;

public class CreateTaskCommand: IRequest<int>
{
    public string Name { get; set; } = default!;
    public string UserEmail { get; set; } = default!;
    public string StateId { get; set; } = default!;
}

public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, int>
{

    private readonly ApplicationContext _context;
    public CreateTaskCommandHandler(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        var userId = await _context.Users.Where(u => u.Email == request.UserEmail).Select(u => u.Id).FirstOrDefaultAsync(cancellationToken);
        if (userId == 0) throw new ApiException("User not found");

        var stateId = await _context.States.Where(s => s.Id == request.StateId).Select(s => s.Id).FirstOrDefaultAsync(cancellationToken)
            ?? throw new ApiException("State not found");

        var taskToCreate = new TaskJob {
            Name = request.Name,
            StateId = stateId,
            UserId = userId,
        };

        _context.TaskJobs.Add(taskToCreate);
        await _context.SaveChangesAsync(cancellationToken);

        return taskToCreate.Id;
    }
}