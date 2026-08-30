namespace StudentManagementApi.DTOs;

/// <summary>
/// Generic paginated response wrapper with pagination metadata.
/// </summary>
/// <typeparam name="T">Type of data items contained in the page.</typeparam>
public class PagedResponse<T>
{
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
    public IEnumerable<T> Items { get; set; } = new List<T>();

    public PagedResponse() { }

    public PagedResponse(IEnumerable<T> items, int totalCount, int pageNumber, int pageSize)
    {
        TotalCount = totalCount;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalPages = pageSize > 0 ? (int)Math.Ceiling((double)totalCount / pageSize) : 0;
        Items = items;
    }
}
