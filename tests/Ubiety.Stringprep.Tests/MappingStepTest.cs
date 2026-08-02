using System;
using System.Collections.Generic;
using Shouldly;
using Ubiety.Stringprep.Core;
using Xunit;

namespace Ubiety.Stringprep.Tests
{
    public class MappingStepTest
    {
        [Fact]
        public void WillNotReplaceValuesNotMapped()
        {
            const string input = "this will not be replaced";
            var step = new MappingStep(MappingTable.Create(Mapping.B1));
            var output = step.Run(input);

            output.ShouldBe(input);
        }

        [Fact]
        public void WillReplaceValuesMappedToNothing()
        {
            var input = $"this value: {Convert.ToChar(0x180B)} will be replaced";
            const string expected = "this value:  will be replaced";
            var step = new MappingStep(MappingTable.Create(Mapping.B1));
            var output = step.Run(input);
            
            output.ShouldBe(expected);
        }

        [Fact]
        public void WillReplaceValues()
        {
            var input = $"this value: {Convert.ToChar(0x0041)} will be replaced";
            var expected = $"this value: {Convert.ToChar(0x0061)} will be replaced";
            var step = new MappingStep(MappingTable.Create(Mapping.B2));
            var output = step.Run(input);
            
            output.ShouldBe(expected);
        }

        [Fact]
        public void WillReplaceWithMultipleCharacters()
        {
            var input = $"this value: {Convert.ToChar(0x00DF)} will be replaced";
            var expected = $"this value: {Convert.ToChar(0x0073)}{Convert.ToChar(0x0073)} will be replaced";
            var step = new MappingStep(MappingTable.Create(Mapping.B2));
            var output = step.Run(input);
            
            output.ShouldBe(expected);
        }

        [Fact]
        public void WillReplaceNonBmpValues()
        {
            // B.2 case-folds U+10400 (DESERET CAPITAL LONG I) to U+10428.
            var input = $"deseret {char.ConvertFromUtf32(0x10400)} letter";
            var expected = $"deseret {char.ConvertFromUtf32(0x10428)} letter";
            var step = new MappingStep(MappingTable.Create(Mapping.B2));
            var output = step.Run(input);

            output.ShouldBe(expected);
        }

        [Fact]
        public void WillPreserveUnmappedNonBmpValues()
        {
            var input = $"emoji {char.ConvertFromUtf32(0x1F600)} here";
            var step = new MappingStep(MappingTable.Create(Mapping.B2));
            var output = step.Run(input);

            output.ShouldBe(input);
        }

        [Fact]
        public void WillEmitNonBmpReplacement()
        {
            var table = MappingTable.Create(new Dictionary<int, int[]> { { 'a', [0x10428] } });
            var step = new MappingStep(table);

            step.Run("a").ShouldBe(char.ConvertFromUtf32(0x10428));
        }
    }
}