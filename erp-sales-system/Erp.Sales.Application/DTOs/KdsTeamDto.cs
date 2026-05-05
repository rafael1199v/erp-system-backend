namespace Erp.Sales.Application.DTOs;

public record KdsTeamDto(
    int Id,
    string Name,
    List<int> CategoryIds
);