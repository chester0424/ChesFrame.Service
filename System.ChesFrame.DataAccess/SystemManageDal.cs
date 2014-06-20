using System;
using System.ChesFrame.Entity.SystemManage;
using System.ChesFrame.IDataAccess;
using System.ChesFrame.Utility.DataAccess;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ChesFrame.DataAccess
{
    public class SystemManageDal : ISystemManageDal
    {
        #region 菜单
        public List<MenuEntity> MenuGetAll()
        {
            DbCommand dbcommand = DbCommandManager.GetDbCommand("SystemManage_GetAllMenu");
            var list = dbcommand.ExecuteEntityList<MenuEntity>();
            return list;
        }

        #endregion
    }
}
