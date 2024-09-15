
namespace Application.Models.Wrappers;


public class ResponseWrapper {
    public bool Succeeded { get; set; }
    public string Message { get; set; } = default!;
    public List<string> Errors { get; set; } = default!;

    public ResponseWrapper() {}

    public ResponseWrapper(string message)
    {
      Succeeded = true;
      Message = message; 
    }
}

public class ResponseWrapper<T>: ResponseWrapper
{
    public T? Data { get; set; } = default!;

    public ResponseWrapper(T? data, string? message = null)
    {
      Succeeded = true;
      Message = message ?? "" ;
      Data = data;
    }

}