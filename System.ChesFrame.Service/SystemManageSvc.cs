using System;
using System.ChesFrame.Entity.SystemManage;
using System.ChesFrame.IService;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ChesFrame.Service
{
    public class SystemManageSvc : ISystemManageSvc
    {
        private BizProcessor.SystemManageProcessor systemManageProcessor = new BizProcessor.SystemManageProcessor();

        #region 菜单Menu

        public List<MenuEntity> MenuGetAll()
        {
            return systemManageProcessor.MenuGetAll();
        }

        #endregion

        #region 权限 Limite



        #endregion
    }
}