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
    /// <summary>
    ///     Mapping table builder.
    /// </summary>
    /// <remarks>
    ///     Initializes a new instance of the <see cref="MappingTableBuilder"/> class.
    /// </remarks>
    /// <param name="baseTables">Base value tables.</param>
    internal class MappingTableBuilder(params IDictionary<int, int[]>[] baseTables) : IMappingTableBuilder
    {
        private readonly List<IDictionary<int, int[]>> _baseTables = [.. baseTables];
        private readonly List<IDictionary<int, int[]>> _inclusions = [];
        private readonly List<int> _removals = [];
        private readonly List<(List<int> values, int[] replacements)> _valueRangeBaseTables = [];

        /// <summary>
        ///     Add value range table to mapping.
        /// </summary>
        /// <param name="values">Values to look for.</param>
        /// <param name="replacement">Replacement value.</param>
        /// <returns>Mapping table builder instance.</returns>
        public IMappingTableBuilder WithValueRangeTable(int[] values, int replacement)
        {
            return WithValueRangeTable(values, [replacement]);
        }

        /// <summary>
        ///     Add value range table to mapping.
        /// </summary>
        /// <param name="values">Values to look for.</param>
        /// <param name="replacement">Replacement values.</param>
        /// <returns>Mapping table builder instance.</returns>
        public IMappingTableBuilder WithValueRangeTable(int[] values, int[] replacement)
        {
            _valueRangeBaseTables.Add((values.ToList(), replacement));
            return this;
        }

        /// <summary>
        ///     Add mapping table.
        /// </summary>
        /// <param name="table">Table to add.</param>
        /// <returns>Mapping table builder instance.</returns>
        public IMappingTableBuilder WithMappingTable(IDictionary<int, int[]> table)
        {
            _baseTables.Add(table);
            return this;
        }

        /// <summary>
        ///     Add values to be included.
        /// </summary>
        /// <param name="table">Table of values to be included.</param>
        /// <returns>Mapping table builder instance.</returns>
        public IMappingTableBuilder Include(IDictionary<int, int[]> table)
        {
            _inclusions.Add(table);
            return this;
        }

        /// <summary>
        ///     Value to removed.
        /// </summary>
        /// <param name="removeValue">Value to remove.</param>
        /// <returns>Mapping table builder instance.</returns>
        public IMappingTableBuilder Remove(int removeValue)
        {
            _removals.Add(removeValue);
            return this;
        }

        /// <summary>
        ///     Compile mapping table.
        /// </summary>
        /// <returns>Mapping table instance.</returns>
        /// <exception cref="InvalidOperationException">Thrown when no tables are provided.</exception>
        public IMappingTable Compile()
        {
            if (_baseTables.Count == 0 && _inclusions.Count == 0 && _valueRangeBaseTables.Count == 0)
            {
                throw new InvalidOperationException("At least one table must be provided");
            }

            var mappingTables = new List<IMappingTable>
            {
                new DictionaryMappingTable(MappingTableCompiler.Compile(
                    [.. _baseTables],
                    [.. _inclusions],
                    [.. _removals])),
            };

            mappingTables.AddRange(
                from t in _valueRangeBaseTables
                let valueRangeTable = ValueRangeCompiler.Compile([t.values], [], _removals)
                select new ValueRangeMappingTable(new ValueRangeTable(valueRangeTable), t.replacements));

            return new CompositeMappingTable(mappingTables);
        }
    }
}