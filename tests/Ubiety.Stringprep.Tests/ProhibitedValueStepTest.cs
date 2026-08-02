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

        [Fact]
        public void WillProhibitNullCharacter()
        {
            // C.2.1 prohibits U+0000. FirstOrDefault-based detection could not
            // distinguish a matched NUL from "no match".
            var input = $"ab{Convert.ToChar(0x00)}cd";
            var step = new ProhibitedValueStep(ValueRangeTable.Build(Prohibited.C21).Compile());
            Action run = () => { step.Run(input); };
            run.ShouldThrow<ProhibitedValueException>();
        }

        [Fact]
        public void WillProhibitNonBmpValuesInTable()
        {
            // C.4 prohibits the non-character U+1FFFE, which is encoded as a surrogate pair.
            var input = $"abc{char.ConvertFromUtf32(0x1FFFE)}def";
            var step = new ProhibitedValueStep(ValueRangeTable.Build(Prohibited.C4).Compile());
            Action run = () => { step.Run(input); };
            run.ShouldThrow<ProhibitedValueException>().CodePoint.ShouldBe(0x1FFFE);
        }

        [Fact]
        public void WillNotProhibitWellFormedSurrogatePairs()
        {
            // C.5 prohibits surrogate *code points*; a valid pair encodes a legal
            // supplementary character and must pass through.
            var input = $"emoji {char.ConvertFromUtf32(0x1F600)} here";
            var step = new ProhibitedValueStep(ValueRangeTable.Build(Prohibited.C5).Compile());
            var output = step.Run(input);

            output.ShouldBe(input);
        }

        [Fact]
        public void WillProhibitUnpairedSurrogate()
        {
            var input = $"bad {Convert.ToChar(0xD800)} pair";
            var step = new ProhibitedValueStep(ValueRangeTable.Build(Prohibited.C5).Compile());
            Action run = () => { step.Run(input); };
            run.ShouldThrow<ProhibitedValueException>();
        }
    }
}