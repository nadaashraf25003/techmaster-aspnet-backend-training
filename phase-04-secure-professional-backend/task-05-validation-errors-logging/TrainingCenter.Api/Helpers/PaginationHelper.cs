using Microsoft.EntityFrameworkCore;
using TrainingCenter.Api.DTOs.Common;

namespace TrainingCenter.Api.Helpers;

public static class PaginationHelper
{
    public static async Task<PagedResult<T>> CreatePagedResultAsync<T>(
        IQueryable<T> query,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        pageNumber = pageNumber > 0 ? pageNumber : 1;
        pageSize = pageSize > 0 ? pageSize : 10;

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<T>(items, totalCount, pageNumber, pageSize);
    }
}
