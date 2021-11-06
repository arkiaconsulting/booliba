// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using System;

namespace Booliba.Data
{
    public record WorkReport(Guid Id, string Name, DateOnly[] Days, string[] Recipients);
}
