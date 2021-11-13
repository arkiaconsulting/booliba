// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

namespace Booliba.QuerySide
{
    public record WorkReportEntity(Guid Id, string Name, DateOnly[] Days, string[] RecipientEmails);
}
