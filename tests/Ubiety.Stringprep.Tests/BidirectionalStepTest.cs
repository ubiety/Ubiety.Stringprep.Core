using System;
using Ubiety.Stringprep.Core;
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
                ValueRangeTable.Create(Bidirectional.D1),
                ValueRangeTable.Create(Bidirectional.D2));
        }
        
        [Fact]
        public void WillThrowForProhibitedValues()
        {
            var input = $"{Convert.ToChar(0x0340)}";
            Assert.Throws<ProhibitedValueException>(() => _step.Run(input));
        }

        [Fact]
        public void WillThrowForRALStringNotEndingWithRALCharacter()
        {
            var input = $"{Convert.ToChar(0x0627)}1";
            Assert.Throws<BidirectionalFormatException>(() => _step.Run(input));
        }

        [Fact]
        public void WillThrowForMixedRALAndLCharacters()
        {
            var input = $"{Convert.ToChar(0x05BE)}{Convert.ToChar(0x0041)}";
            Assert.Throws<BidirectionalFormatException>(() => _step.Run((input)));
        }

        [Fact]
        public void WillPassForRALStringEndingWithRALCharacter()
        {
            var input = $"{Convert.ToChar(0x0627)}{Convert.ToChar(0x0628)}";
            var output = _step.Run(input);
            Assert.Equal(input, output);
        }
    }
}