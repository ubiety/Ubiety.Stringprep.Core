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

namespace Ubiety.Stringprep.Core
{
    /// <summary>
    ///     Value range table.
    /// </summary>
    public class ValueRangeTable : IValueRangeTable
    {
        private readonly int _length;

        private readonly int[] _valueRanges;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ValueRangeTable"/> class.
        /// </summary>
        /// <param name="valueRanges">Value range table.</param>
        /// <exception cref="ArgumentException">Thrown if not a valid table.</exception>
        public ValueRangeTable(int[] valueRanges)
        {
            if (valueRanges.Length % 2 != 0)
            {
                throw new ArgumentException("Not a value range table", nameof(valueRanges));
            }

            _valueRanges = valueRanges;
            _length = valueRanges.Length / 2;
        }

        /// <summary>
        ///     Create a range table.
        /// </summary>
        /// <param name="baseTables">Value tables.</param>
        /// <returns>Value range table.</returns>
        public static IValueRangeTable Create(params List<int>[] baseTables)
        {
            return Build(baseTables).Compile();
        }

        /// <summary>
        ///     Build range table.
        /// </summary>
        /// <param name="baseTables">Value tables.</param>
        /// <returns>Value range table builder instance.</returns>
        public static IValueRangeTableBuilder Build(params List<int>[] baseTables)
        {
            return new ValueRangeTableBuilder(baseTables);
        }

        /// <summary>
        ///     Does the table contain the value.
        /// </summary>
        /// <param name="value">Value to look up.</param>
        /// <returns>True if the value is in the table, otherwise false.</returns>
        public bool Contains(int value)
        {
            var l = 0;
            var r = _length - 1;
            while (l <= r)
            {
                var m = (int)Math.Floor((double)(l + r) / 2);

                var lowValue = _valueRanges[m * 2];
                var highValue = _valueRanges[(m * 2) + 1];

                if (lowValue == value || highValue == value || (lowValue < value && highValue > value))
                {
                    return true;
                }

                if (lowValue < value)
                {
                    l = m + 1;
                }
                else
                {
                    r = m - 1;
                }
            }

            return false;
        }
    }
}