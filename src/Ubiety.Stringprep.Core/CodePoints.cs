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
    ///     Helpers for walking a string as a sequence of Unicode code points.
    /// </summary>
    /// <remarks>
    ///     RFC3454 tables are defined over code points, not UTF-16 code units. Several tables
    ///     (B.2, B.3, C.3, C.4, C.9, D.2) contain values above U+FFFF, so a <see cref="char" />
    ///     based walk can never match them.
    /// </remarks>
    internal static class CodePoints
    {
        /// <summary>
        ///     Enumerates the code points of a string.
        /// </summary>
        /// <param name="input">String to enumerate.</param>
        /// <returns>
        ///     The code points of <paramref name="input" />. A well formed surrogate pair yields the
        ///     single supplementary code point it encodes; an unpaired surrogate yields its own value
        ///     so that it can still be matched against table C.5.
        /// </returns>
        public static IEnumerable<int> Enumerate(string input)
        {
            for (var i = 0; i < input.Length; i++)
            {
                var c = input[i];

                if (char.IsHighSurrogate(c) && i + 1 < input.Length && char.IsLowSurrogate(input[i + 1]))
                {
                    yield return char.ConvertToUtf32(c, input[i + 1]);
                    i++;
                }
                else
                {
                    yield return c;
                }
            }
        }
    }
}
