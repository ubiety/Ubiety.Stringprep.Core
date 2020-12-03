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
using Ubiety.Stringprep.Core.Generated;

namespace Ubiety.Stringprep.Core
{
    /// <summary>
    ///     Prohibited value tables.
    /// </summary>
    public static class Prohibited
    {
        /// <summary>
        ///     Table C1.1 ASCII space characters.
        /// </summary>
        public static readonly List<int> C11 = Tables.C11;

        /// <summary>
        ///     Table C1.1 ASCII space characters.
        /// </summary>
        public static readonly List<int> ASCIISpaceCharacters = Tables.C11;

        /// <summary>
        ///     Table C1.2 Non ASCII space characters.
        /// </summary>
        public static readonly List<int> C12 = Tables.C12;

        /// <summary>
        ///     Table C1.2 Non ASCII space characters.
        /// </summary>
        public static readonly List<int> NonASCIISpaceCharacters = Tables.C12;

        /// <summary>
        ///     Table C2.1 ASCII control characters.
        /// </summary>
        public static readonly List<int> C21 = Tables.C21;

        /// <summary>
        ///     Table C2.1 ASCII control characters.
        /// </summary>
        public static readonly List<int> ASCIIControlCharacters = Tables.C21;

        /// <summary>
        ///     Table C2.2 Non ASCII control characters.
        /// </summary>
        public static readonly List<int> C22 = Tables.C22;

        /// <summary>
        ///     Table C2.2 Non ASCII control characters.
        /// </summary>
        public static readonly List<int> NonASCIIControlCharacters = Tables.C22;

        /// <summary>
        ///     Table C3 Private use characters.
        /// </summary>
        public static readonly List<int> C3 = Tables.C3;

        /// <summary>
        ///     Table C3 Private use characters.
        /// </summary>
        public static readonly List<int> PrivateUseCharacters = Tables.C3;

        /// <summary>
        ///     Table C4 Non character code points.
        /// </summary>
        public static readonly List<int> C4 = Tables.C4;

        /// <summary>
        ///     Table C4 Non character code points.
        /// </summary>
        public static readonly List<int> NonCharacterCodePoints = Tables.C4;

        /// <summary>
        ///     Table C5 Surrogate code points.
        /// </summary>
        public static readonly List<int> C5 = Tables.C5;

        /// <summary>
        ///     Table C5 Surrogate code points.
        /// </summary>
        public static readonly List<int> SurrogateCodePoints = Tables.C5;

        /// <summary>
        ///     Table C6 Inappropriate for plain text.
        /// </summary>
        public static readonly List<int> C6 = Tables.C6;

        /// <summary>
        ///     Table C6 Inappropriate for plain text.
        /// </summary>
        public static readonly List<int> InappropriateForPlainText = Tables.C6;

        /// <summary>
        ///     Table C7 Inappropriate for canonical representation.
        /// </summary>
        public static readonly List<int> C7 = Tables.C7;

        /// <summary>
        ///     Table C7 Inappropriate for canonical representation.
        /// </summary>
        public static readonly List<int> InappropriateForCanonicalRepresentation = Tables.C7;

        /// <summary>
        ///     Table C8 Change display properties or deprecated.
        /// </summary>
        public static readonly List<int> C8 = Tables.C8;

        /// <summary>
        ///     Table C8 Change display properties or deprecated.
        /// </summary>
        public static readonly List<int> ChangeDisplayPropertiesOrDeprecated = Tables.C8;

        /// <summary>
        ///     Table C9 Tagging characters.
        /// </summary>
        public static readonly List<int> C9 = Tables.C9;

        /// <summary>
        ///     Table C9 Tagging characters.
        /// </summary>
        public static readonly List<int> TaggingCharacters = Tables.C9;
    }
}