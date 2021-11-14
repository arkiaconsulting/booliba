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
        public BoolibaInlineAutoDataAttribute(bool addWorkReportWithCustomer = false)
            : base(new BoolibaAutoDataAttribute(addWorkReportWithCustomer))
        {
        }
    }

    internal class BoolibaAutoDataAttribute : AutoDataAttribute
    {
        public BoolibaAutoDataAttribute(bool addWorkReportWithCustomer = false)
            : base(() => new Fixture().Customize(new BoolibaDefaultConventions(addWorkReportWithCustomer)))
        {
        }
    }

    internal class BoolibaDefaultConventions : ICustomization
    {
        private readonly bool _addWorkReportWithCustomer;

        public BoolibaDefaultConventions(bool addWorkReportWithCustomer) =>
            _addWorkReportWithCustomer = addWorkReportWithCustomer;

        public void Customize(IFixture fixture)
        {
            fixture.Customizations.Add(new DateOnlyBuilder());
            if (!_addWorkReportWithCustomer)
            {
                fixture.Customizations.Add(new AddWorkReportWithoutCustomerBuilder());
            }
        }
    }

    internal class AddWorkReportWithoutCustomerBuilder : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return IsNotAddWorkReportCommandRequest(request)
                       ? new NoSpecimen()
                       : new AddWorkReportCommand(
                           context.Create<Guid>(),
                           context.Create<string>(),
                           context.Create<DateOnly[]>(),
                           default
                        );
        }

        private static bool IsNotAddWorkReportCommandRequest(object request) =>
            !typeof(AddWorkReportCommand).GetTypeInfo().IsAssignableFrom(request as Type);
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
