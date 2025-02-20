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
    ///     Mapping table compiler.
    /// </summary>
    internal static class MappingTableCompiler
    {
        /// <summary>
        ///     Compiles the mapping table.
        /// </summary>
        /// <param name="baseTables">Base value tables.</param>
        /// <param name="inclusions">Table of values to include.</param>
        /// <param name="removals">Values to remove.</param>
        /// <returns>Table of values.</returns>
        public static IDictionary<int, int[]> Compile(
            IDictionary<int, int[]>[] baseTables,
            IEnumerable<IDictionary<int, int[]>> inclusions,
            IEnumerable<int> removals)
        {
            return DoRemove(DoInclude(DoCombine(baseTables), inclusions), removals);
        }

        private static SortedDictionary<int, int[]> DoRemove(SortedDictionary<int, int[]> dict, IEnumerable<int> removals)
        {
            foreach (var key in removals)
            {
                if (!dict.ContainsKey(key))
                {
                    continue;
                }

                dict.Remove(key);
            }

            return dict;
        }

        private static SortedDictionary<int, int[]> DoInclude(
            SortedDictionary<int, int[]> dict,
            IEnumerable<IDictionary<int, int[]>> inclusions)
        {
            foreach (var t in inclusions)
            {
                foreach (var (key, value) in t)
                {
                    if (dict.ContainsKey(key))
                    {
                        continue;
                    }

                    dict.Add(key, value);
                }
            }

            return dict;
        }

        private static SortedDictionary<int, int[]> DoCombine(IReadOnlyList<IDictionary<int, int[]>> baseTables)
        {
            if (baseTables.Count == 0)
            {
                return [];
            }

            var combined = new SortedDictionary<int, int[]>(baseTables[0]);
            for (var i = 1; i < baseTables.Count; i++)
            {
                foreach (var (key, value) in baseTables[i])
                {
                    if (combined.ContainsKey(key))
                    {
                        continue;
                    }

                    combined.Add(key, value);
                }
            }

            return combined;
        }
    }
}