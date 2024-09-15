
namespace Application.Models.Requests;

public class UpdateTaskStateRequest
{
    public int TaskId { get; set; }
    public string StateId { get; set; } = default!;
}