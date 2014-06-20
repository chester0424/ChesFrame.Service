using System;
using System.ChesFrame.Utility.DataAccess.DataConfig;
using System.ChesFrame.Utility.WCF.Config;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace System.ChesFrame.Utility.WCF
{
    /// <summary>
    /// 通过配置，找出配置的DLL中所有的继承的Interface被[ServiceContract]属性修饰的Class
    /// </summary>
    public class ServiceDiscoverer
    {
        /*
         * 1.加载配置，得到DLL name
         * 2.从DLL中找到需要的服务class和interface
         * 3.组合返回
         */
        bool haveLoad = false;

        private List<ServiceAndInterfaceInfo> serviceAndInterfaceInfos = new List<ServiceAndInterfaceInfo>();

        /// <summary>
        /// 加载配置（反序列化）
        /// </summary>
        /// <returns></returns>
        private ServiceAssemblyConfig LoadConfig()
        {
            string directorStr = "";
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            directorStr = string.Format(@"{0}\Config\WCF", baseDirectory.TrimEnd(new char[] { '\\' }));

            var filePath = string.Format("{0}\\{1}", directorStr, "ServiceAssembly.Config");

            var mappingConfig = SerializeHelper.XmlDeserialize<ServiceAssemblyConfig>(filePath);

            return mappingConfig;
        }

        private List<ServiceAndInterfaceInfo> LoadServiceAndInterfaceInfo()
        {
            List<ServiceAndInterfaceInfo> serviceAndInterfaceInfoList = new List<ServiceAndInterfaceInfo>();
            ServiceAssemblyConfig config = LoadConfig();

            foreach (var sub in config.Items)
            {
                Func<string, string> dllPath = (p) =>
                {
                    return string.Format("{0}bin\\{1}", System.AppDomain.CurrentDomain.BaseDirectory, p);
                };

                Type[] types = Assembly.LoadFrom(dllPath(sub.DllName)).GetTypes();//并自定加载程序集的依赖

                foreach (Type oType in types)
                {
                    //判断类的接口附加了ServiceContractAttribute属性
                    var serviceInterfaces = oType.GetInterfaces();
                    for (int i = 0; i < serviceInterfaces.Count(); i++)
                    {
                        var customerAttributes = serviceInterfaces[i].GetCustomAttributes<ServiceContractAttribute>();
                        if (customerAttributes != null && customerAttributes.Count() > 0)
                        {
                            serviceAndInterfaceInfoList.Add(new ServiceAndInterfaceInfo()
                            {
                                ServiceType = oType,
                                InterfaceType = serviceInterfaces[i]
                            });
                        }
                    }
                }
            }
            return serviceAndInterfaceInfoList;
        }

        public List<ServiceAndInterfaceInfo> ServiceAndInterfaceInfos
        {
            get 
            {
                if (haveLoad == false)
                {
                    serviceAndInterfaceInfos = LoadServiceAndInterfaceInfo();
                    haveLoad = true;
                }
                return serviceAndInterfaceInfos;
            }
        }

        #region Single Instance

        private static ServiceDiscoverer _ServiceDiscoverer = null;
        private static object objLock = new object();
        public static ServiceDiscoverer Instance
        {
            get
            {
                if (_ServiceDiscoverer == null)
                {
                    lock (objLock)
                    {
                        if (_ServiceDiscoverer == null)
                        {
                            _ServiceDiscoverer = new ServiceDiscoverer();
                        }
                    }
                }
                return _ServiceDiscoverer;
            }
        }

        #endregion
    }
}
