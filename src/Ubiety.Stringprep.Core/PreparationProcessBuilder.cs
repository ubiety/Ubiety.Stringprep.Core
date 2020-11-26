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
    internal class PreparationProcessBuilder : IPreparationProcessBuilder
    {
        private readonly IList<IPreparationProcess> _steps = new List<IPreparationProcess>();

        public IPreparationProcessBuilder WithBidirectionalStep()
        {
            return WithBidirectionalStep(
                ValueRangeTable.Create(Prohibited.ChangeDisplayPropertiesOrDeprecated),
                ValueRangeTable.Create(BidirectionalTables.RorAL),
                ValueRangeTable.Create(BidirectionalTables.L));
        }

        public IPreparationProcessBuilder WithBidirectionalStep(
            IValueRangeTable prohibited,
            IValueRangeTable ral,
            IValueRangeTable l)
        {
            var step = new BidirectionalStep(prohibited, ral, l);
            _steps.Add(step);
            return this;
        }

        public IPreparationProcessBuilder WithMappingStep(IMappingTable mappingTable)
        {
            var step = new MappingStep(mappingTable);
            _steps.Add(step);
            return this;
        }

        public IPreparationProcessBuilder WithNormalizationStep()
        {
            return WithNormalizationStep(NormalizationForm.FormKC);
        }

        public IPreparationProcessBuilder WithNormalizationStep(NormalizationForm form)
        {
            var step = new NormalizationStep(form);
            _steps.Add(step);
            return this;
        }

        public IPreparationProcessBuilder WithProhibitedValueStep(IValueRangeTable prohibitedTable)
        {
            var step = new ProhibitedValueStep(prohibitedTable);
            _steps.Add(step);
            return this;
        }

        public IPreparationProcess Compile()
        {
            return new PreparationProcess(_steps);
        }
    }
}