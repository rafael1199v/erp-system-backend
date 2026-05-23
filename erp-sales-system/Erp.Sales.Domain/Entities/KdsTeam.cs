namespace Erp.Sales.Domain.Entities;

public class KdsTeam
{
    public int Id { get; set; }
    public string Cen { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public List<int> CategoryIds { get; set; } = [];
    public List<string> CategoryCens { get; set; } = [];
}
