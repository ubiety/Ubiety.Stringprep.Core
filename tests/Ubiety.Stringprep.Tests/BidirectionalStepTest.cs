using System;
using Shouldly;
using Ubiety.Stringprep.Core;
using Ubiety.Stringprep.Core.Bidirectional;
using Ubiety.Stringprep.Core.Exceptions;
using Xunit;

namespace Ubiety.Stringprep.Tests
{
    public class BidirectionalStepTest
    {
        private readonly BidirectionalStep _step;

        public BidirectionalStepTest()
        {
            _step = new BidirectionalStep(
                ValueRangeTable.Create(Prohibited.C8),
                ValueRangeTable.Create(BidirectionalTables.D1),
                ValueRangeTable.Create(BidirectionalTables.D2));
        }
        
        [Fact]
        public void WillThrowForProhibitedValues()
        {
            var input = $"{Convert.ToChar(0x0340)}";
            Should.Throw<ProhibitedValueException>(() => _step.Run(input));
        }

        [Fact]
        public void WillThrowForRALStringNotEndingWithRALCharacter()
        {
            var input = $"{Convert.ToChar(0x0627)}1";
            Should.Throw<BidirectionalFormatException>(() => _step.Run(input));
        }

        [Fact]
        public void WillThrowForMixedRALAndLCharacters()
        {
            var input = $"{Convert.ToChar(0x05BE)}{Convert.ToChar(0x0041)}";
            Should.Throw<BidirectionalFormatException>(() => _step.Run((input)));
        }

        [Fact]
        public void WillPassForRALStringEndingWithRALCharacter()
        {
            var input = $"{Convert.ToChar(0x0627)}{Convert.ToChar(0x0628)}";
            var output = _step.Run(input);
            
            output.ShouldBe(input);
        }

        [Fact]
        public void WillThrowForStringNotStartingWithRALCharacter()
        {
            var input = $"1{Convert.ToChar(0x0627)}";
            Should.Throw<BidirectionalFormatException>(() => _step.Run(input));
        }
    }
}