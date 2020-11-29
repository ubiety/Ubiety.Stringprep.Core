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
    public static class ValueRangeCompiler
    {
        public static int[] Compile(IList<int>[] baseTables, IList<int> inclusions, IList<int> removals)
        {
            foreach (var table in baseTables)
            {
                Sort(table);
            }

            Sort(inclusions);
            Sort(removals);

            return DoRemove(DoReduce(DoInclude(DoCombine(baseTables), inclusions)), removals).ToArray();
        }

        private static void Sort(IList<int> table)
        {
            CheckSanity(table);

            var l = table.Count / 2;
            var starts = new int[l];
            var ends = new int[l];

            for (var i = 0; i < table.Count; i++)
            {
                if (i % 2 == 0)
                {
                    starts[i / 2] = table[i];
                }
                else
                {
                    ends[i / 2] = table[i];
                }
            }

            Array.Sort(starts, ends);
            for (var i = 0; i < table.Count; i++)
            {
                if (i % 2 == 0)
                {
                    table[i] = starts[i / 2];
                }
                else
                {
                    table[i] = ends[i / 2];
                }
            }
        }

        private static void CheckSanity(IList<int> table)
        {
            if (table.Count % 2 != 0)
            {
                throw new ArgumentException("Not a range table", nameof(table));
            }

            for (var i = 0; i < table.Count - 1; i += 2)
            {
                if (table[i + 1] < table[i])
                {
                    throw new ArgumentException("Not a range table", nameof(table));
                }
            }
        }

        private static List<int> DoCombine(IReadOnlyList<IList<int>> tables)
        {
            if (tables.Count == 1)
            {
                return tables[0].ToList();
            }

            var combined = new List<int>();
            var idx = new int[tables.Count];

            while (true)
            {
                var minIdx = -1;
                var min = -1;
                for (var i = 0; i < tables.Count; i++)
                {
                    if (tables[i].Count <= idx[i])
                    {
                        continue;
                    }

                    var current = tables[i][idx[i]];
                    if (min != -1 && current >= min)
                    {
                        continue;
                    }

                    min = current;
                    minIdx = i;
                }

                if (minIdx == -1)
                {
                    break;
                }

                combined.Add(tables[minIdx][idx[minIdx]]);
                combined.Add(tables[minIdx][idx[minIdx] + 1]);
                idx[minIdx] += 2;
            }

            return combined;
        }

        private static List<int> DoInclude(List<int> list, IList<int> inclusions)
        {
            for (var i = 0; i < inclusions.Count; i += 2)
            {
                if (inclusions[i] < list[0])
                {
                    list.InsertRange(0, new[] { inclusions[i], inclusions[i + 1] });
                }
                else
                {
                    var j = 0;
                    for (; j < list.Count; j += 2)
                    {
                        if (inclusions[i] > list[j])
                        {
                            continue;
                        }

                        list.InsertRange(j, new[] { inclusions[i], inclusions[i + 1] });
                        break;
                    }

                    list.Add(inclusions[i]);
                    list.Add(inclusions[i + 1]);
                }
            }

            return list;
        }

        private static List<int> DoRemove(List<int> list, IList<int> removals)
        {
            for (var i = 0; i < removals.Count; i += 2)
            {
                for (var j = 0; j < list.Count; j += 2)
                {
                    if (removals[i] == list[j])
                    {
                        list.RemoveAt(j--);
                        CloseRemove(list, removals, ref i, ref j);
                    }
                    else if (removals[i] > list[j] && removals[i] < list[j + 1])
                    {
                        list.Insert(++j, removals[i] - 1);
                        CloseRemove(list, removals, ref i, ref j);
                    }
                    else if (removals[i] < list[j] && (i == 0 || removals[i] > list[j - 1]))
                    {
                        list.RemoveAt(j--);
                        CloseRemove(list, removals, ref i, ref j);
                    }
                }
            }

            return list;
        }

        private static void CloseRemove(IList<int> list, IList<int> removals, ref int i, ref int j)
        {
            for (i++; i < removals.Count; i += 2)
            {
                for (j++; j < list.Count; j += 2)
                {
                    if (removals[i] == list[j])
                    {
                        list.RemoveAt(j);
                        return;
                    }

                    if (removals[i] < list[j])
                    {
                        list.Insert(j, removals[i] + 1);
                        return;
                    }

                    if (removals[i] <= list[j] || (j + 1 < list.Count && removals[i] >= list[j + 1]))
                    {
                        continue;
                    }

                    list.RemoveAt(j);
                    return;
                }
            }
        }

        private static List<int> DoReduce(List<int> list)
        {
            var i = 1;
            while (i < list.Count - 1)
            {
                for (; i < list.Count - 1; i += 2)
                {
                    if (list[i + 1] > list[i])
                    {
                        continue;
                    }

                    if (list[i + 2] > list[i])
                    {
                        list[i] = list[i + 2];
                    }

                    list.RemoveRange(i + 1, 2);
                    break;
                }
            }

            return list;
        }
    }
}