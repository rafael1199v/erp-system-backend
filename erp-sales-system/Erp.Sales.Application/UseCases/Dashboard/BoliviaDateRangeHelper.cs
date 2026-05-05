namespace Erp.Sales.Application.UseCases.Dashboard;

internal static class BoliviaDateRangeHelper
{
    private const string BoliviaIanaTimeZoneId = "America/La_Paz";
    private const string BoliviaWindowsTimeZoneId = "SA Western Standard Time";

    public static (DateTime StartUtc, DateTime EndUtc) GetCurrentDayUtcRange()
    {
        TimeZoneInfo boliviaTimeZone = ResolveBoliviaTimeZone();
        DateTime boliviaNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, boliviaTimeZone);
        DateTime boliviaDayStart = boliviaNow.Date;
        DateTime boliviaNextDayStart = boliviaDayStart.AddDays(1);

        DateTime dayStartUtc = TimeZoneInfo.ConvertTimeToUtc(boliviaDayStart, boliviaTimeZone);
        DateTime nextDayStartUtc = TimeZoneInfo.ConvertTimeToUtc(boliviaNextDayStart, boliviaTimeZone);

        return (dayStartUtc, nextDayStartUtc);
    }

    private static TimeZoneInfo ResolveBoliviaTimeZone()
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById(BoliviaIanaTimeZoneId);
        }
        catch (TimeZoneNotFoundException)
        {
            return TimeZoneInfo.FindSystemTimeZoneById(BoliviaWindowsTimeZoneId);
        }
    }
}