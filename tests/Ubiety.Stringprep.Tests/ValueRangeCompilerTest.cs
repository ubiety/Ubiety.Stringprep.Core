using System;
using Shouldly;
using Ubiety.Stringprep.Core;
using Xunit;

namespace Ubiety.Stringprep.Tests
{
    public class ValueRangeCompilerTest
    {
        [Fact]
        public void WillCompileSingleTable()
        {
            var a = new[] {1, 1};
            var result = ValueRangeCompiler.Compile(new[] {a}, new int[0], new int[0]);

            result.ShouldBe(new[] {1, 1});
        }

        [Fact]
        public void WillSortSingleTable()
        {
            var a = new[] {3, 7, 18, 22, 1, 1, 9, 13};
            var result = ValueRangeCompiler.Compile(new[] {a}, new int[0], new int[0]);

            result.ShouldBe(new[] {1, 1, 3, 7, 9, 13, 18, 22});
        }

        [Fact]
        public void WillCompileTwoTables()
        {
            var a = new[] {3, 7, 22, 25};
            var b = new[] {9, 15, 18, 20};

            var result = ValueRangeCompiler.Compile(new[] {a, b}, new int[0], new int[0]);
            result.ShouldBe(new[] {3, 7, 9, 15, 18, 20, 22, 25});
        }

        [Fact]
        public void WillCombineTablesOfDifferentLengths()
        {
            var a = new[] {1, 2};
            var b = new[] {75, 75, 42, 46, 13, 15};
            var c = new[] {33, 35, 77, 79};
            var d = new[] {4, 10, 99, 99, 101, 105, 303, 307};

            var result = ValueRangeCompiler.Compile(new[] {a, b, c, d}, new int[0], new int[0]);
            result.ShouldBe(new[] {1, 2, 4, 10, 13, 15, 33, 35, 42, 46, 75, 75, 77, 79, 99, 99, 101, 105, 303, 307});
        }

        [Fact]
        public void WillReduceOverlappingValueRanges()
        {
            var a = new[]
            {
                1, 10,
                9, 12,
                14, 17,
                15, 18
            };

            var result = ValueRangeCompiler.Compile(new[] {a}, new int[0], new int[0]);
            result.ShouldBe(
                new[] {1, 12, 14, 18});
        }

        [Fact]
        public void WillReduceInclusiveValueRanges()
        {
            var a = new[]
            {
                1, 100,
                9, 15,
                18, 22,
                33, 75
            };

            var result = ValueRangeCompiler.Compile(new[] {a}, new int[0], new int[0]);
            result.ShouldBe(new[] {1, 100});
        }

        [Fact]
        public void WillReduceAdjacentValueRanges()
        {
            var a = new[]
            {
                1, 20,
                20, 35,
                35, 48,
                48, 50
            };

            var result = ValueRangeCompiler.Compile(new[] {a}, new int[0], new int[0]);
            result.ShouldBe(new[] {1, 50});
        }

        [Fact]
        public void WillIncludeValueRangesAfterBaseTable()
        {
            var a = new[]
            {
                1, 5
            };

            var result = ValueRangeCompiler.Compile(new[] {a}, new[] {10, 10}, new int[0]);
            result.ShouldBe(new[] {1, 5, 10, 10});
        }

        [Fact]
        public void WillIncludeMultipleValueRangesAfterBaseTable()
        {
            var a = new[]
            {
                1, 5
            };

            var result = ValueRangeCompiler.Compile(new[] {a}, new[] {10, 10, 15, 20}, new int[0]);
            result.ShouldBe(new[] {1, 5, 10, 10, 15, 20});
        }

        [Fact]
        public void WillIncludeValueRangesBeforeBaseTable()
        {
            var a = new[]
            {
                15, 20
            };

            var result = ValueRangeCompiler.Compile(new[] {a}, new[] {4, 7}, new int[0]);
            result.ShouldBe(new[] {4, 7, 15, 20});
        }

        [Fact]
        public void WillIncludeMultipleValueRangesBeforeBaseTable()
        {
            var a = new[]
            {
                15, 20
            };

            var result = ValueRangeCompiler.Compile(new[] {a}, new[] {1, 1, 4, 7}, new int[0]);
            result.ShouldBe(new[] {1, 1, 4, 7, 15, 20});
        }

        [Fact]
        public void WillRemoveValueRangeEqualToStartValue()
        {
            var a = new[]
            {
                10, 20
            };

            var result = ValueRangeCompiler.Compile(new[] {a}, new int[0], new[] {10, 15});
            result.ShouldBe(new[] {16, 20});
        }

        [Fact]
        public void WillRemoveValueRangeEqualToEndValue()
        {
            var a = new[]
            {
                10, 20
            };

            var result = ValueRangeCompiler.Compile(new[] {a}, new int[0], new[] {15, 20});
            result.ShouldBe(new[] {10, 14});
        }

        [Fact]
        public void WillRemoveValueRangeOverlappingStartValue()
        {
            var a = new[]
            {
                10, 20
            };

            var result = ValueRangeCompiler.Compile(new[] {a}, new int[0], new[] {5, 15});
            result.ShouldBe(new[] {16, 20});
        }

        [Fact]
        public void WillRemoveValueRangeOverlappingStartValueOnLaterRanges()
        {
            var a = new[]
            {
                10, 20,
                30, 40
            };

            var result = ValueRangeCompiler.Compile(new[] {a}, new int[0], new[] {25, 35});
            result.ShouldBe(new[] {10, 20, 36, 40});
        }

        [Fact]
        public void WillRemoveValueRangeOverlappingEndValue()
        {
            var a = new[]
            {
                10, 20
            };

            var result = ValueRangeCompiler.Compile(new[] {a}, new int[0], new[] {15, 25});
            result.ShouldBe(new[] {10, 14});
        }

        [Fact]
        public void WillRemoveValueRangeOverlappingEndValueOnEarlierRanges()
        {
            var a = new[]
            {
                10, 20,
                30, 40
            };

            var result = ValueRangeCompiler.Compile(new[] {a}, new int[0], new[] {15, 25});
            result.ShouldBe(new[] {10, 14, 30, 40});
        }

        [Fact]
        public void ThrowsForInvalidRange()
        {
            var a = new[]
            {
                7, 5
            };

            Should.Throw<ArgumentException>(() => { ValueRangeCompiler.Compile(new[] {a}, new int[0], new int[0]); });
        }

        [Fact]
        public void ThrowsForOddLengthArray()
        {
            var a = new[]
            {
                5, 7,
                4
            };

            Should.Throw<ArgumentException>(() => { ValueRangeCompiler.Compile(new[] {a}, new int[0], new int[0]); });
        }
    }
}