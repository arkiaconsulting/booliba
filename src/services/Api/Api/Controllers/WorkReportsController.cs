﻿// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using Booliba.ApplicationCore.AddDaysToReport;
using Booliba.ApplicationCore.AddReport;
using Booliba.ApplicationCore.RemoveDaysFromReport;
using Booliba.ApplicationCore.RemoveWorkReport;
using Booliba.ApplicationCore.SendReport;
using Booliba.QuerySide;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Booliba.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkReportsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public WorkReportsController(IMediator mediator) => _mediator = mediator;

        [HttpPost("{id:guid}")]
        [Consumes("application/json")]
        public async Task<IActionResult> AddWorkReport(
            [FromRoute] Guid id,
            AddWorkReportRequest request,
            CancellationToken cancellationToken)
        {
            await _mediator.Send(
                new AddWorkReportCommand(id, request.Name, request.Days),
                cancellationToken
            );

            return Ok();
        }

        [HttpPost("{id:guid}/days")]
        public async Task<IActionResult> AddDays(
            [FromRoute] Guid id,
            [FromBody] AddWorkReportDaysRequest request,
            CancellationToken cancellationToken)
        {
            await _mediator.Send(
                new AddDaysCommand(id, request.Days),
                cancellationToken
            );

            return Ok();
        }

        [HttpDelete("{id:guid}/days")]
        public async Task<IActionResult> RemoveDays(
            [FromRoute] Guid id,
            [FromBody] RemoveWorkReportDaysRequest request,
            CancellationToken cancellationToken)
        {
            await _mediator.Send(
                new RemoveDaysCommand(id, request.Days),
                cancellationToken
            );

            return Ok();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> RemoveWorkReport(
            [FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            await _mediator.Send(
                new RemoveWorkReportCommand(id),
                cancellationToken
            );

            return Ok();
        }

        [HttpPost("{id:guid}/send")]
        public async Task<IActionResult> SendWorkReport(
            [FromRoute] Guid id,
            [FromBody] SendWorkReportRequest request,
            CancellationToken cancellationToken)
        {
            await _mediator.Send(
                new SendWorkReportCommand(id, request.EmailAddresses),
                cancellationToken
            );

            return Ok();
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetWorkReport(
            [FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(
                new GetWorkReportQuery(id),
                cancellationToken
            );

            if (response?.Result is null)
            {
                return NotFound();
            }

            return Ok(
                new WorkReportResponse(
                    response.Result.Id,
                    response.Result.Name,
                    response.Result.Days,
                    response.Result.RecipientEmails)
            );
        }

        [HttpGet()]
        public async Task<IActionResult> GetWorkReports(CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(
                new GetWorkReportsQuery(),
                cancellationToken
            );

            return Ok(
                response?.Results.Select(wr =>
                    new WorkReportResponse(
                        wr.Id,
                        wr.Name,
                        wr.Days,
                        wr.RecipientEmails)
                )
            );
        }
    }

    public record AddWorkReportRequest(string Name, DateOnly[] Days);
    public record AddWorkReportDaysRequest(DateOnly[] Days);
    public record RemoveWorkReportDaysRequest(DateOnly[] Days);
    public record SendWorkReportRequest(string[] EmailAddresses);
    public record WorkReportResponse(Guid Id, string Name, DateOnly[] Days, string[] Recipients);
}
