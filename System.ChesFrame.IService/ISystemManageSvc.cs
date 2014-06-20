using System;
using System.ChesFrame.Entity.SystemManage;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace System.ChesFrame.IService
{
    [ServiceContract]
    public interface ISystemManageSvc
    {
        #region 菜单管理
       
        /// <summary>
        /// 获取所有菜单
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        List<MenuEntity> MenuGetAll();

        #endregion
    }
}
