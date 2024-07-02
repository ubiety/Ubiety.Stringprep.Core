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

using System.Linq;
using Ubiety.Stringprep.Core.Exceptions;

namespace Ubiety.Stringprep.Core
{
    /// <summary>
    ///     Step that defines a prohibited value.
    /// </summary>
    /// <remarks>
    ///     Initializes a new instance of the <see cref="ProhibitedValueStep"/> class.
    /// </remarks>
    /// <param name="table">Table of prohibited values.</param>
    public class ProhibitedValueStep(IValueRangeTable table) : IPreparationProcess
    {
        /// <summary>
        ///     Executes the step.
        /// </summary>
        /// <param name="input">Data to compare against.</param>
        /// <returns>Data if not prohibited.</returns>
        /// <exception cref="ProhibitedValueException">Thrown when input is prohibited.</exception>
        public string Run(string input)
        {
            var item = input.Where(value => table.Contains(value)).FirstOrDefault();
            if (item != null)
            {
                throw new ProhibitedValueException(value);
            }

            return input;
        }
    }
}