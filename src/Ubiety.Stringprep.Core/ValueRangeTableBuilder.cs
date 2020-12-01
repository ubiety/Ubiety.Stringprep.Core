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
    internal class ValueRangeTableBuilder : IValueRangeTableBuilder
    {
        private readonly IList<List<int>> _baseTables;
        private readonly IList<int> _inclusions;
        private readonly IList<int> _removals;

        public ValueRangeTableBuilder(params List<int>[] baseTables)
        {
            _baseTables = baseTables.ToList();
            _inclusions = new List<int>();
            _removals = new List<int>();
        }

        public IValueRangeTableBuilder Include(int includeValue)
        {
            IncludeRange(includeValue, includeValue);
            return this;
        }

        public IValueRangeTableBuilder IncludeRange(int start, int end)
        {
            _inclusions.Add(start);
            _inclusions.Add(end);
            return this;
        }

        public IValueRangeTableBuilder Remove(int removeValue)
        {
            RemoveRange(removeValue, removeValue);
            return this;
        }

        public IValueRangeTableBuilder RemoveRange(int start, int end)
        {
            _removals.Add(start);
            _removals.Add(end);
            return this;
        }

        public IValueRangeTable Compile()
        {
            if (!_baseTables.Any())
            {
                throw new InvalidOperationException("At least one base table must be provided");
            }

            var ranges = ValueRangeCompiler.Compile(_baseTables.ToArray(), _inclusions, _removals.ToArray());
            return new ValueRangeTable(ranges);
        }
    }
}