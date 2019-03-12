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

using System.Collections.Generic;

namespace Ubiety.Stringprep.Core
{
    /// <summary>
    ///     Mapping table builder interface
    /// </summary>
    public interface IMappingTableBuilder
    {
        /// <summary>
        ///     Builds a mapping with a value range
        /// </summary>
        /// <param name="values">Values array</param>
        /// <param name="replacement">Replacement value</param>
        /// <returns>Mapping builder</returns>
        IMappingTableBuilder WithValueRangeTable(int[] values, int replacement);

        /// <summary>
        ///     Builds a mapping with a value range
        /// </summary>
        /// <param name="values">Values array</param>
        /// <param name="replacement">Replacement array</param>
        /// <returns>Mapping builder</returns>
        IMappingTableBuilder WithValueRangeTable(int[] values, int[] replacement);

        /// <summary>
        ///     Builds a mapping with a mapping table
        /// </summary>
        /// <param name="table">Mapping table</param>
        /// <returns>Mapping builder</returns>
        IMappingTableBuilder WithMappingTable(IDictionary<int, int[]> table);

        /// <summary>
        ///     Includes a table in the current mapping
        /// </summary>
        /// <param name="include">Table to include</param>
        /// <returns>Mapping builder</returns>
        IMappingTableBuilder Include(IDictionary<int, int[]> include);

        /// <summary>
        ///     Removes a table from the mapping
        /// </summary>
        /// <param name="remove">Integer position of the table</param>
        /// <returns>Mapping builder</returns>
        IMappingTableBuilder Remove(int remove);

        /// <summary>
        ///     Compile the mapping
        /// </summary>
        /// <returns>Mapping table</returns>
        IMappingTable Compile();
    }
}