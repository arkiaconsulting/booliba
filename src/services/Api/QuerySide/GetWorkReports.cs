// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using MediatR;

namespace Booliba.QuerySide
{
    public record GetWorkReportsQuery : IRequest<GetWorkReportsResponse>;
    public record GetWorkReportsResponse(WorkReportEntity[] Results);
    public record WorkReportEntity(Guid Id, string Name, DateOnly[] Days, string[] RecipientEmails);

    internal class GetWorkReportsQueryHandler : IRequestHandler<GetWorkReportsQuery, GetWorkReportsResponse>
    {
        private readonly IWorkReportProjection _workReportProjection;

        public GetWorkReportsQueryHandler(IWorkReportProjection workReportProjection) =>
            _workReportProjection = workReportProjection;

        async Task<GetWorkReportsResponse> IRequestHandler<GetWorkReportsQuery, GetWorkReportsResponse>.Handle(GetWorkReportsQuery request, CancellationToken cancellationToken)
        {
            var workReports = await _workReportProjection.List(cancellationToken);

            return new GetWorkReportsResponse(workReports);
        }
    }
}
