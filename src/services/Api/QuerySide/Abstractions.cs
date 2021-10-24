// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

namespace Booliba.QuerySide
{
    public interface IWorkReportProjection
    {
        Task<WorkReportEntity[]> List(CancellationToken cancellationToken = default);
        Task<WorkReportEntity?> Get(Guid workReportId, CancellationToken cancellationToken = default);
    }
}
