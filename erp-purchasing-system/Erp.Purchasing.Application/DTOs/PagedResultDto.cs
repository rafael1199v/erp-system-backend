namespace Erp.Purchasing.Application.DTOs;

public sealed record PagedResultDto<T>(
    IReadOnlyCollection<T> Items,
    int TotalCount,
    int TotalPages,
    int CurrentPage);
