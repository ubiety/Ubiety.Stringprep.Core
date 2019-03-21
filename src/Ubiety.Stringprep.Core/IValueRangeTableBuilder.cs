﻿/* This is free and unencumbered software released into the public domain.
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

namespace Ubiety.Stringprep.Core
{
    /// <summary>
    ///     Value range table builder.
    /// </summary>
    public interface IValueRangeTableBuilder
    {
        /// <summary>
        ///     Include a value in the table.
        /// </summary>
        /// <param name="include">Value to include.</param>
        /// <returns>Builder instance.</returns>
        IValueRangeTableBuilder Include(int include);

        /// <summary>
        ///     Include a range of values in the table.
        /// </summary>
        /// <param name="start">Range start value.</param>
        /// <param name="end">Range end value.</param>
        /// <returns>Builder instance.</returns>
        IValueRangeTableBuilder IncludeRange(int start, int end);

        /// <summary>
        ///     Remove a value from the table.
        /// </summary>
        /// <param name="remove">Value to remove.</param>
        /// <returns>Builder instance.</returns>
        IValueRangeTableBuilder Remove(int remove);

        /// <summary>
        ///     Remove a range of values from the table.
        /// </summary>
        /// <param name="start">Range start value.</param>
        /// <param name="end">Range end value.</param>
        /// <returns>Builder instance.</returns>
        IValueRangeTableBuilder RemoveRange(int start, int end);

        /// <summary>
        ///     Compile the value table.
        /// </summary>
        /// <returns>Compiled value table.</returns>
        IValueRangeTable Compile();
    }
}