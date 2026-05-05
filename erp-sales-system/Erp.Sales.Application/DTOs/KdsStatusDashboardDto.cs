namespace Erp.Sales.Application.DTOs;

public record KdsStatusDashboardDto(
    int PendingCount,
    int PreparingCount,
    int ReadyCount
);