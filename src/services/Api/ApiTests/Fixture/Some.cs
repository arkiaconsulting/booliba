using System;

namespace Booliba.ApiTests.Fixture
{
    internal static class Some
    {
        public static DateTimeOffset DayInCurrentMonth =>
            new(
                DateTimeOffset.UtcNow.Year,
                DateTimeOffset.UtcNow.Month,
                new Random(DateTimeOffset.UtcNow.Millisecond)
                .Next(
                    1,
                    DateTime.DaysInMonth(DateTimeOffset.UtcNow.Year, DateTimeOffset.UtcNow.Month)
                ), 0, 0, 0, DateTimeOffset.UtcNow.Offset
            );
    }
}
