﻿// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using System;
using System.Collections.Generic;
using System.Linq;

namespace Booliba.ApiTests.Fixture
{
    internal static class TestExtensions
    {
        public static T PickRandom<T>(this IEnumerable<T> source) =>
            source.OrderBy(_ => Guid.NewGuid())
            .First();
    }
}
