
namespace Application.Models.Responses;

public class UserResponse {
    public int UserId { get; set; }
    public string Email { get; set; } = default!;
    public string Name { get; set; } = default!;
}