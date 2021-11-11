// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using Booliba.Data;
using Booliba.ViewModels;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booliba.Pages
{
    public partial class WorkReports
    {
        [Inject]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private BoolibaService BoolibaService { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        private List<DisplayWorkReportViewModel>? _workReports = null;
        private bool _addRecipientDialogOpen;
        private Guid _workReportId = Guid.Empty;
        private string _newWorkReportName = string.Empty;
        private int _newYear = DateTime.Now.Year;
        private int _newMonth = DateTime.Now.Month;

        private readonly int[] _availableYears = Enumerable.Range(DateTime.Now.Year - 1, 1).Reverse().ToArray();

        protected override async Task OnInitializedAsync()
        {
            _workReports = (await BoolibaService.GetWorkReports())
            .Select(wr => new DisplayWorkReportViewModel(BoolibaService, wr.Id, wr.Name, wr.Days, wr.Recipients))
            .ToList();
        }

        private async Task Remove(DisplayWorkReportViewModel workReport)
        {
            await workReport.Remove();
            _workReports?.Remove(workReport);
        }

        private async Task Fill(DisplayWorkReportViewModel workReport, int year, int month)
        {
            await workReport.Fill(year, month);
        }

        private async Task Empty(DisplayWorkReportViewModel workReport, int year, int month)
        {
            await workReport.Empty(year, month);
        }

        private async Task AddMonth(DisplayWorkReportViewModel workReport)
        {
            if(workReport.DaysPerYear!.Any(y => y.Key == _newYear)
                && workReport.DaysPerYear!.Single(y => y.Key == _newYear).Any(m => m.Month == _newMonth))
            {
                return;
            }
            
            await workReport.AddMonth(_newYear, _newMonth);
        }

        private void ChangeNewYear(int year) => _newYear = year;

        private void ChangeNewMonth(int month) => _newMonth = month;

        private void OpenAddRecipient(DisplayWorkReportViewModel workReport)
        {
            _workReportId = workReport.Id;
            _addRecipientDialogOpen = true;
        }

        private void OnEmailAdded(string? email)
        {
            if (email != null)
            {
                var workReport = _workReports!.Single(wr => wr.Id == _workReportId);
                workReport.AddRecipient(email);
            }

            _addRecipientDialogOpen = false;
        }

        private async Task CreateWorkReport()
        {
            var newGuid = Guid.NewGuid();
            var initialDays = new[] { DateOnly.FromDateTime(DateTime.Now) };
            await BoolibaService.CreateWorkReport(newGuid, _newWorkReportName, initialDays);
            _workReports?.Insert(0, new DisplayWorkReportViewModel(BoolibaService, newGuid, _newWorkReportName, initialDays, Array.Empty<string>()));
            _newWorkReportName = string.Empty;
        }        

        private static string OnWeekEnd(DayOfWeek dayOfWeek) =>
            new[] { DayOfWeek.Saturday, DayOfWeek.Sunday }.Contains(dayOfWeek) ? "text-light bg-secondary" : string.Empty;

    }
}
