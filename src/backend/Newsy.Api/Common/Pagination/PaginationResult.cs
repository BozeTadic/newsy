namespace Newsy.Api.Common.Pagination;

public class PaginationResult<T> where T : class
{
    public PaginationResult(List<T> items, int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        Items = items;
    }

    public int PageNumber { get; private set; }
    public int PageSize { get; private set; }
    public IReadOnlyList<T> Items { get; private set; }
}