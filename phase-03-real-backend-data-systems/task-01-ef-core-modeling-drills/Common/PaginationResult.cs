namespace EFCoreModelingDrills.Common;

/// <summary>
/// Generic pagination container with computed metadata and validation.
/// </summary>
public class PaginationResult<T>
{
    public IReadOnlyList<T> Items { get; }
    public int PageNumber { get; }
    public int PageSize { get; }
    public int TotalCount { get; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    public PaginationResult(IEnumerable<T> items, int totalCount, int pageNumber, int pageSize)
    {
        if (pageNumber < 1)
            throw new ArgumentOutOfRangeException(nameof(pageNumber), "Page number must be at least 1.");

        if (pageSize < 1 || pageSize > 50)
            throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be between 1 and 50.");

        Items = items.ToList().AsReadOnly();
        TotalCount = totalCount;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
}
