// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using AutoFixture;
using AutoFixture.Kernel;
using AutoFixture.Xunit2;
using System;
using System.Reflection;

namespace Booliba.Tests.Fixtures
{
    internal class BoolibaInlineAutoDataAttribute : InlineAutoDataAttribute
    {
        public BoolibaInlineAutoDataAttribute()
            : base(new BoolibaAutoDataAttribute())
        {
        }
    }

    internal class BoolibaAutoDataAttribute : AutoDataAttribute
    {
        public BoolibaAutoDataAttribute()
            : base(() => new Fixture().Customize(new BoolibaConventions()))
        {
        }
    }

    internal class BoolibaConventions : ICustomization
    {
        public void Customize(IFixture fixture) => fixture.Customizations.Add(new DateOnlyBuilder());
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
