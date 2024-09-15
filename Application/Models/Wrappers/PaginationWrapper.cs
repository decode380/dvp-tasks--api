
namespace Application.Models.Wrappers;

public class PaginationWrapper<T>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public T Data { get; set; } = default!;
    public int TotalRecords { get; set; }
}

public class PaginationProperties {
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}