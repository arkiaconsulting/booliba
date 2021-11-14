// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using System;

namespace Booliba.ApiTests.Fixture
{
    internal record WorkReportDto(Guid Id, string Name, DateOnly[] Days, string[] Recipients, Guid? CustomerId);
}
