using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ChesFrame.Utility.DataAccess
{
    /// <summary>
    /// Sql相关的只读变量
    /// </summary>
    public class SqlConst
    {
        #region About Page
        
        public static readonly string PageSize = "@PageSize";

        public static readonly string RowFrom = "@RowFrom";

        public static readonly string RowTo = "@RowTo";

        public static readonly string OrderBy = "@OrderBy";

        public static readonly string TotalCount = "@TotalCount";

        #endregion
    }
}
