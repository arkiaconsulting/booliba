// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

namespace Booliba.ApplicationCore
{
    public class WorkReportNotFoundException : Exception
    {
        public WorkReportNotFoundException(Guid workReportId)
            : base($"The work report with Id '{workReportId}' was not found")
        {
        }
    }
}
