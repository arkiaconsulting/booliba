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


    [Serializable]
    public class MissingEmailRecipientsException : Exception
    {
        public MissingEmailRecipientsException(Guid workReportId)
            : this($"Email recipients are missing when trying to send the work report with Id '{workReportId}'")
        { }

        public MissingEmailRecipientsException(string message) : base(message) { }

        protected MissingEmailRecipientsException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    public class WorkReportRemovedException : Exception
    {
        public WorkReportRemovedException(Guid workReportId)
            : base($"The work report with Id '{workReportId}' has already been removed")
        {
        }
    }
}
