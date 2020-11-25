using System;
using Shouldly;
using Ubiety.Stringprep.Core;
using Ubiety.Stringprep.Core.Exceptions;
using Xunit;

namespace Ubiety.Stringprep.Tests
{
    public class ProhibitedValueStepTest
    {
        [Fact]
        public void WillProhibitValuesInTable()
        {
            var input = $"{Convert.ToChar(0x20)}";
            var step = new ProhibitedValueStep(ValueRangeTable.Build(Prohibited.C11).Compile());
            Action run = () => { step.Run(input); };
            run.ShouldThrow<ProhibitedValueException>();
        }

        [Fact]
        public void WillNotProhibitValuesNotInTable()
        {
            const string input = "ThisIsAStringWithoutSpaces";
            var step = new ProhibitedValueStep(ValueRangeTable.Build(Prohibited.C11).Compile());
            var output = step.Run(input);
            
            output.ShouldBe(input);
        }
    }
}