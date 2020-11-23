/* This is free and unencumbered software released into the public domain.
 *
 * Anyone is free to copy, modify, publish, use, compile, sell, or
 * distribute this software, either in source code form or as a compiled
 * binary, for any purpose, commercial or non-commercial, and by any
 * means.
 *
 * In jurisdictions that recognize copyright laws, the author or authors
 * of this software dedicate any and all copyright interest in the
 * software to the public domain. We make this dedication for the benefit
 * of the public at large and to the detriment of our heirs and
 * successors. We intend this dedication to be an overt act of
 * relinquishment in perpetuity of all present and future rights to this
 * software under copyright law.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
 * IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
 * OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
 * ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 *
 * For more information, please refer to <http://unlicense.org/>
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace Ubiety.Stringprep.Core
{
    internal class MappingTableBuilder : IMappingTableBuilder
    {
        private readonly List<IDictionary<int, int[]>> _baseTables;
        private readonly List<IDictionary<int, int[]>> _inclusions;
        private readonly List<int> _removals;
        private readonly List<Tuple<int[], int[]>> _valueRangeBaseTables;

        public MappingTableBuilder(params IDictionary<int, int[]>[] baseTables)
        {
            _baseTables = baseTables.ToList();
            _valueRangeBaseTables = new List<Tuple<int[], int[]>>();
            _inclusions = new List<IDictionary<int, int[]>>();
            _removals = new List<int>();
        }

        public IMappingTableBuilder WithValueRangeTable(int[] values, int replacement)
        {
            return WithValueRangeTable(values, new[] { replacement });
        }

        public IMappingTableBuilder WithValueRangeTable(int[] values, int[] replacement)
        {
            _valueRangeBaseTables.Add(new Tuple<int[], int[]>(values, replacement));
            return this;
        }

        public IMappingTableBuilder WithMappingTable(IDictionary<int, int[]> table)
        {
            _baseTables.Add(table);
            return this;
        }

        public IMappingTableBuilder Include(IDictionary<int, int[]> include)
        {
            _inclusions.Add(include);
            return this;
        }

        public IMappingTableBuilder Remove(int remove)
        {
            _removals.Add(remove);
            return this;
        }

        public IMappingTable Compile()
        {
            if (!_baseTables.Any() && !_inclusions.Any() && !_valueRangeBaseTables.Any())
            {
                throw new InvalidOperationException("At least one table must be provided");
            }

            var mappingTables = new List<IMappingTable>
            {
                new DictionaryMappingTable(MappingTableCompiler.Compile(
                    _baseTables.ToArray(),
                    _inclusions.ToArray(),
                    _removals.ToArray())),
            };

            mappingTables.AddRange(
                from t in _valueRangeBaseTables
                    let valueRangeTable = ValueRangeCompiler.Compile(new[] { t.Item1 }, Array.Empty<int>(), _removals.ToArray())
                    select new ValueRangeMappingTable(new ValueRangeTable(valueRangeTable), t.Item2));

            return new CompositeMappingTable(mappingTables);
        }
    }
}