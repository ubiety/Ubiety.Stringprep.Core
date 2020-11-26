using System.Collections.Generic;
using Ubiety.Stringprep.Core.Generated;

namespace Ubiety.Stringprep.Core.Bidirectional
{
    /// <summary>
    ///     Bidirectional value tables.
    /// </summary>
    public static class BidirectionalTables
    {
        /// <summary>
        ///     Characters with bidirectional property 'R' or 'AL'.
        /// </summary>
        public static readonly List<int> D1 = Tables.D1;

        /// <summary>
        ///     Table D.1 Characters with bidirectional property 'R' or 'AL'.
        /// </summary>
        public static readonly List<int> RorAL = Tables.D1;

        /// <summary>
        ///     Characters with bidirectional property 'L'.
        /// </summary>
        public static readonly List<int> D2 = Tables.D2;

        /// <summary>
        ///     Table D.2 Characters with bidirectional property 'L'.
        /// </summary>
        public static readonly List<int> L = Tables.D2;
    }
}