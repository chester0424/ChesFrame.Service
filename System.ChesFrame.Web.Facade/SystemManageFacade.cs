using System;
using System.ChesFrame.Entity.SystemManage;
using System.ChesFrame.IService;
using System.ChesFrame.Utility.WCF;
using System.ChesFrame.Utility;
using System.ChesFrame.Web.Model.SystemManage;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ChesFrame.Web.Facade
{
    public class SystemManageFacade
    {
        #region 菜单

        public static List<MenuModel> MenuGetAll()
        {
            ISystemManageSvc proxy = ServiceFactory<ISystemManageSvc>.Instace;
            var allMenu = proxy.MenuGetAll();

            var modelList = allMenu.AssignValueToByAutoMapper<MenuEntity, MenuModel>();

            return modelList;
        }

        #endregion
    }
}
