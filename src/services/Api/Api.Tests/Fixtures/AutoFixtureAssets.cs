// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using AutoFixture;
using AutoFixture.Kernel;
using AutoFixture.Xunit2;
using Booliba.ApplicationCore.WorkReports;
using System;
using System.Reflection;

namespace Booliba.Tests.Fixtures
{
    internal class BoolibaInlineAutoDataAttribute : InlineAutoDataAttribute
    {
        public BoolibaInlineAutoDataAttribute(bool workReportWithCustomer = false)
            : base(new BoolibaAutoDataAttribute(workReportWithCustomer))
        {
        }
    }

    internal class BoolibaAutoDataAttribute : AutoDataAttribute
    {
        public BoolibaAutoDataAttribute(bool workReportWithCustomer = false)
            : base(() => new Fixture().Customize(new BoolibaDefaultConventions(workReportWithCustomer)))
        {
        }
    }

    internal class BoolibaDefaultConventions : ICustomization
    {
        private readonly bool _workReportWithCustomer;

        public BoolibaDefaultConventions(bool workReportWithCustomer) =>
            _workReportWithCustomer = workReportWithCustomer;

        public void Customize(IFixture fixture)
        {
            fixture.Customizations.Add(new DateOnlyBuilder());
            if (!_workReportWithCustomer)
            {
                fixture.Customizations.Add(new WorkReportWithoutCustomerBuilder());
            }
        }
    }

    internal class WorkReportWithoutCustomerBuilder : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (typeof(AddWorkReportCommand).GetTypeInfo().IsAssignableFrom(request as Type))
            {
                return new AddWorkReportCommand(
                           context.Create<Guid>(),
                           context.Create<string>(),
                           context.Create<DateOnly[]>(),
                           default);
            }
            else if (typeof(SetWorkReportCustomerCommand).GetTypeInfo().IsAssignableFrom(request as Type))
            {
                return new SetWorkReportCustomerCommand(
                           context.Create<Guid>(),
                           default);
            }
            else
            {
                return new NoSpecimen();
            }
        }
    }

    internal class DateOnlyBuilder : ISpecimenBuilder
    {
        private readonly RandomNumericSequenceGenerator _randomizer;

        public DateOnlyBuilder() => _randomizer = new RandomNumericSequenceGenerator(DateTime.MinValue.Ticks, DateTime.MaxValue.Ticks);

        public object Create(object request, ISpecimenContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return IsNotDateOnlyRequest(request)
                       ? new NoSpecimen()
                       : this.CreateRandomDate(context);
        }

        private object CreateRandomDate(ISpecimenContext context) =>
            DateOnly.FromDateTime(new DateTime((long)_randomizer.Create(typeof(long), context)));

        private static bool IsNotDateOnlyRequest(object request) => !typeof(DateOnly).GetTypeInfo().IsAssignableFrom(request as Type);
    }
}
