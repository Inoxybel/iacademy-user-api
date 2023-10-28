namespace CrossCutting.Helpers;

public class PaginatedResult<T>
{
    public long TotalRecords { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public List<T> Data { get; set; }
}
