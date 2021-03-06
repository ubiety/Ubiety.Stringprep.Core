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

using System.Text;

namespace Ubiety.Stringprep.Core
{
    /// <summary>
    ///     Preparation process builder interface.
    /// </summary>
    public interface IPreparationProcessBuilder
    {
        /// <summary>
        ///     Add a normalization step to the process.
        /// </summary>
        /// <returns>Builder instance.</returns>
        IPreparationProcessBuilder WithNormalizationStep();

        /// <summary>
        ///     Add a normalization step to the process.
        /// </summary>
        /// <param name="form">Normalization to use.</param>
        /// <returns>Builder instance.</returns>
        IPreparationProcessBuilder WithNormalizationStep(NormalizationForm form);

        /// <summary>
        ///     Add a mapping step to the process.
        /// </summary>
        /// <param name="mappingTable">Mapping table to add.</param>
        /// <returns>Builder instance.</returns>
        IPreparationProcessBuilder WithMappingStep(IMappingTable mappingTable);

        /// <summary>
        ///     Add a prohibited value processing step.
        /// </summary>
        /// <param name="prohibitedTable">Prohibited value table.</param>
        /// <returns>Builder instance.</returns>
        IPreparationProcessBuilder WithProhibitedValueStep(IValueRangeTable prohibitedTable);

        /// <summary>
        ///     Add a bidirectional processing step.
        /// </summary>
        /// <returns>Builder instance.</returns>
        IPreparationProcessBuilder WithBidirectionalStep();

        /// <summary>
        ///     Add a bidirectional processing step.
        /// </summary>
        /// <param name="prohibited">Prohibited value table.</param>
        /// <param name="ral">RAL table.</param>
        /// <param name="l">L Table.</param>
        /// <returns>Builder instance.</returns>
        IPreparationProcessBuilder WithBidirectionalStep(
            IValueRangeTable prohibited,
            IValueRangeTable ral,
            IValueRangeTable l);

        /// <summary>
        ///     Compile the process.
        /// </summary>
        /// <returns>Compiled process instance.</returns>
        IPreparationProcess Compile();
    }
}