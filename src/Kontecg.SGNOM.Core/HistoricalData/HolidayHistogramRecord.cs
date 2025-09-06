using System;
using NMoneys;

namespace Kontecg.HistoricalData
{
    public record HolidayHistogramRecord(long PersonId, Guid GroupId, decimal Hours, Money Amount);
}
