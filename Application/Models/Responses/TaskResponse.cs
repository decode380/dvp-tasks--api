
namespace Application.Models.Responses;

public class TaskResponse
{
    public int TaskId { get; set; }
    public UserResponse User { get; set; } = default!;
    public string Name { get; set; } = default!;
    public StateResponse State { get; set; } = default!;
}