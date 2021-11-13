// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

namespace Booliba.QuerySide
{
    public interface IWorkReportProjection
    {
        Task<WorkReportEntity[]> List(CancellationToken cancellationToken = default);
        Task<WorkReportEntity?> Get(Guid workReportId, CancellationToken cancellationToken = default);
        Task Add(WorkReportEntity entity, CancellationToken cancellationToken = default);
        Task Update(WorkReportEntity entity, CancellationToken cancellationToken = default);
        Task Delete(Guid workReportId, CancellationToken cancellationToken = default);
    }

    public interface ICustomerProjection
    {
        Task Add(CustomerEntity entity, CancellationToken cancellationToken = default);
        Task Delete(Guid customerId, CancellationToken cancellationToken);
        Task<CustomerEntity[]> List(CancellationToken cancellationToken);
    }
}
