// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using System;

namespace Booliba.ApiTests.Fixture
{
    internal static class Some
    {
        private static readonly Random _random = new(DateTime.Now.Millisecond);

        public static DateOnly DayInCurrentMonth =>
            new(
                DateTimeOffset.UtcNow.Year,
                DateTimeOffset.UtcNow.Month,
                _random
                .Next(
                    1,
                    DateTime.DaysInMonth(DateTimeOffset.UtcNow.Year, DateTimeOffset.UtcNow.Month)
                )
            );
    }
}
