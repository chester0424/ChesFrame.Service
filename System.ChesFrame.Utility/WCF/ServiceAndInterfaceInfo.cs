using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ChesFrame.Utility.WCF
{
    /// <summary>
    /// 保存服务和接口的对应关系 POCO
    /// </summary>
    public class ServiceAndInterfaceInfo
    {
        /// <summary>
        /// 实现接口的服务的类型
        /// </summary>
        public Type ServiceType { get; set; }

        /// <summary>
        /// 定义功能的接口类型
        /// </summary>
        public Type InterfaceType { get; set; }
    }
}
