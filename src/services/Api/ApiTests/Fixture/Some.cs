// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using System;

namespace Booliba.ApiTests.Fixture
{
    internal static class Some
    {
        public static DateOnly DayInCurrentMonth =>
            new(
                DateTimeOffset.UtcNow.Year,
                DateTimeOffset.UtcNow.Month,
                new Random(DateTimeOffset.UtcNow.Millisecond)
                .Next(
                    1,
                    DateTime.DaysInMonth(DateTimeOffset.UtcNow.Year, DateTimeOffset.UtcNow.Month)
                )
            );
    }
}
