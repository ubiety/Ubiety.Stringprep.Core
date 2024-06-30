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
using System.Text;
using Ubiety.Stringprep.Core.Bidirectional;

namespace Ubiety.Stringprep.Core
{
    /// <summary>
    ///     Preparation process builder.
    /// </summary>
    internal class PreparationProcessBuilder : IPreparationProcessBuilder
    {
        private readonly IList<IPreparationProcess> _steps = [];

        /// <summary>
        ///     Add bidirectional step.
        /// </summary>
        /// <returns>Preparation process builder instance.</returns>
        public IPreparationProcessBuilder WithBidirectionalStep()
        {
            return WithBidirectionalStep(
                ValueRangeTable.Create(Prohibited.ChangeDisplayPropertiesOrDeprecated),
                ValueRangeTable.Create(BidirectionalTables.RorAL),
                ValueRangeTable.Create(BidirectionalTables.L));
        }

        /// <summary>
        ///     Add bidirectional step.
        /// </summary>
        /// <param name="prohibited">Prohibited character table.</param>
        /// <param name="ral">R or AL character table.</param>
        /// <param name="l">L character table.</param>
        /// <returns>Preparation process builder.</returns>
        public IPreparationProcessBuilder WithBidirectionalStep(
            IValueRangeTable prohibited,
            IValueRangeTable ral,
            IValueRangeTable l)
        {
            var step = new BidirectionalStep(prohibited, ral, l);
            _steps.Add(step);
            return this;
        }

        /// <summary>
        ///     Add mapping step.
        /// </summary>
        /// <param name="mappingTable">Mapping table.</param>
        /// <returns>Preparation process builder.</returns>
        public IPreparationProcessBuilder WithMappingStep(IMappingTable mappingTable)
        {
            var step = new MappingStep(mappingTable);
            _steps.Add(step);
            return this;
        }

        /// <summary>
        ///     Add normalization step with default form KC.
        /// </summary>
        /// <returns>Preparation process builder.</returns>
        public IPreparationProcessBuilder WithNormalizationStep()
        {
            return WithNormalizationStep(NormalizationForm.FormKC);
        }

        /// <summary>
        ///     Add normalization step.
        /// </summary>
        /// <param name="form">Normalization form.</param>
        /// <returns>Preparation process builder.</returns>
        public IPreparationProcessBuilder WithNormalizationStep(NormalizationForm form)
        {
            var step = new NormalizationStep(form);
            _steps.Add(step);
            return this;
        }

        /// <summary>
        ///     Add prohibited value step.
        /// </summary>
        /// <param name="prohibitedTable">Prohibited character table.</param>
        /// <returns>Preparation process builder.</returns>
        public IPreparationProcessBuilder WithProhibitedValueStep(IValueRangeTable prohibitedTable)
        {
            var step = new ProhibitedValueStep(prohibitedTable);
            _steps.Add(step);
            return this;
        }

        /// <summary>
        ///     Compile preparation process.
        /// </summary>
        /// <returns>Preparation process.</returns>
        public IPreparationProcess Compile()
        {
            return new PreparationProcess(_steps);
        }
    }
}