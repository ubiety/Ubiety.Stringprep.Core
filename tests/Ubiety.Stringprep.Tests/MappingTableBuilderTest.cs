using System.Collections.Generic;
using Shouldly;
using Ubiety.Stringprep.Core;
using Xunit;

namespace Ubiety.Stringprep.Tests
{
    public class MappingTableBuilderTest
    {
        [Fact]
        public void WillRemoveValueFromDictionaryTable()
        {
            var table = MappingTable.Build(Mapping.B1)
                .Remove(0x00AD)
                .Compile();

            table.HasReplacement(0x00AD).ShouldBeFalse();
            table.HasReplacement(0x200B).ShouldBeTrue();
        }

        [Fact]
        public void WillRemoveValueFromValueRangeTable()
        {
            var table = MappingTable.Build(Mapping.B1)
                .WithValueRangeTable(Prohibited.C12, ' ')
                .Remove(0x2000)
                .Compile();

            table.HasReplacement(0x2000).ShouldBeFalse();
            table.HasReplacement(0x2001).ShouldBeTrue();
        }

        [Fact]
        public void WillRemoveMultipleValuesFromValueRangeTable()
        {
            // Removals are single code points, but the value range compiler works in start/end
            // pairs. An even count used to be read as ranges and an odd count threw outright.
            var table = MappingTable.Build(Mapping.B1)
                .WithValueRangeTable(Prohibited.C12, ' ')
                .Remove(0x2000)
                .Remove(0x2002)
                .Remove(0x2004)
                .Compile();

            table.HasReplacement(0x2000).ShouldBeFalse();
            table.HasReplacement(0x2002).ShouldBeFalse();
            table.HasReplacement(0x2004).ShouldBeFalse();
            table.HasReplacement(0x2001).ShouldBeTrue();
            table.HasReplacement(0x2003).ShouldBeTrue();
        }

        [Fact]
        public void WillRemoveFromBothTableKinds()
        {
            var table = MappingTable.Build(Mapping.B1)
                .WithValueRangeTable(Prohibited.C12, ' ')
                .Remove(0x00AD)
                .Compile();

            table.HasReplacement(0x00AD).ShouldBeFalse();
            table.HasReplacement(0x2000).ShouldBeTrue();
        }

        [Fact]
        public void WillMapValueRangeToReplacement()
        {
            var table = MappingTable.Build(Mapping.B1)
                .WithValueRangeTable(Prohibited.C12, ' ')
                .Compile();

            table.GetReplacement(0x2000).ShouldBe(new[] { 0x0020 });
            table.GetReplacement(0x00AD).ShouldBeEmpty();
        }

        [Fact]
        public void IncludeDoesNotOverwriteExistingEntries()
        {
            var table = MappingTable.Build(new Dictionary<int, int[]> { { 'a', [(int)'b'] } })
                .Include(new Dictionary<int, int[]> { { 'a', [(int)'c'] } })
                .Compile();

            table.GetReplacement('a').ShouldBe(new[] { (int)'b' });
        }
    }
}
