namespace Erp.Sales.Application.Services;

public static class PaymentMethodCodeNormalizer
{
    public static string Normalize(string value)
    {
        return value
            .Trim()
            .Replace(" ", "_")
            .Replace("-", "_")
            .ToUpperInvariant();
    }
}
