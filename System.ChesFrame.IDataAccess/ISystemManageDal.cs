using System;
using System.ChesFrame.Entity.SystemManage;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ChesFrame.IDataAccess
{
   public interface ISystemManageDal
   {
       #region 菜单

       List<MenuEntity> MenuGetAll();

       #endregion
   }
}
