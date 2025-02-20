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
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NON-INFRINGEMENT.
 * IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
 * OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
 * ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 *
 * For more information, please refer to <http://unlicense.org/>
 */

using System.Collections.Generic;

namespace Ubiety.Stringprep.Core
{
    /// <summary>
    ///     Mapping table.
    /// </summary>
    public abstract class MappingTable : IMappingTable
    {
        /// <summary>
        ///     Create a mapping table.
        /// </summary>
        /// <param name="valueTable">Array of values to replace.</param>
        /// <param name="replacement">Replacement value.</param>
        /// <returns>Mapping table instance.</returns>
        public static IMappingTable Create(int[] valueTable, int replacement)
        {
            return Create(valueTable, [replacement]);
        }

        /// <summary>
        ///     Create a mapping table.
        /// </summary>
        /// <param name="valueTable">Integer array of values to replace.</param>
        /// <param name="replacement">Integer array of replacement values.</param>
        /// <returns>Mapping table instance.</returns>
        public static IMappingTable Create(int[] valueTable, int[] replacement)
        {
            return Build(valueTable, replacement).Compile();
        }

        /// <summary>
        ///     Create a mapping table.
        /// </summary>
        /// <param name="baseTables">Tables to base off of.</param>
        /// <returns>Mapping table instance.</returns>
        public static IMappingTable Create(params IDictionary<int, int[]>[] baseTables)
        {
            return Build(baseTables).Compile();
        }

        /// <summary>
        ///     Build mapping table.
        /// </summary>
        /// <param name="valueTable">Table of values to replace.</param>
        /// <param name="replacement">Replacement value.</param>
        /// <returns>Mapping table builder instance.</returns>
        public static IMappingTableBuilder Build(int[] valueTable, int replacement)
        {
            return Build(valueTable, [replacement]);
        }

        /// <summary>
        ///     Build a mapping table.
        /// </summary>
        /// <param name="valueTable">Table of values to replace.</param>
        /// <param name="replacement">Table of replacement values.</param>
        /// <returns>Mapping table builder.</returns>
        public static IMappingTableBuilder Build(int[] valueTable, int[] replacement)
        {
            return Build().WithValueRangeTable(valueTable, replacement);
        }

        /// <summary>
        ///     Build a mapping table.
        /// </summary>
        /// <param name="baseTables">Value and replacement tables.</param>
        /// <returns>Mapping table builder.</returns>
        public static IMappingTableBuilder Build(params IDictionary<int, int[]>[] baseTables)
        {
            return new MappingTableBuilder(baseTables);
        }

        /// <summary>
        ///     Does the value have a replacement.
        /// </summary>
        /// <param name="value">Value to search for.</param>
        /// <returns>true if replacement is available; otherwise false.</returns>
        public abstract bool HasReplacement(int value);

        /// <summary>
        ///     Get the replacement value.
        /// </summary>
        /// <param name="value">Value to replace.</param>
        /// <returns>integer array of the replacement values.</returns>
        public abstract int[] GetReplacement(int value);
    }
}