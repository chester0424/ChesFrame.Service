using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace System.ChesFrame.Utility.WCF
{
    public class ServiceHostContainer
    {
        #region Field

        private static List<ServiceHost> _ServiceHostList = new List<ServiceHost>();

        #endregion

        #region Contructor
        private ServiceHostContainer()
        {

        }

        #endregion

        public static ServiceHostContainer Instance
        {
            get { return new ServiceHostContainer(); }
        }

        #region ContainnerOperator

        public void ServiceHostAdd(ServiceHost serviceHost)
        {
            _ServiceHostList.Add(serviceHost);
        }

        public void ServiceHostAdd(params ServiceHost[] serviceHosts)
        {
            _ServiceHostList.AddRange(serviceHosts);
        }

        public void ServiceHostAdd(List<ServiceHost> serviceHostList)
        {
            _ServiceHostList.ForEach((sub) => { serviceHostList.Add(sub); });
        }

        public void ServiceHostClear()
        {
            ServiceHostClose();
            _ServiceHostList.Clear();
        }

        public List<ServiceHost> ServiceHosts {
            get {
                return _ServiceHostList;
            }
        }

        public void ServiceHostOpen()
        {
            _ServiceHostList.ForEach((sub) =>
            {
                if (sub != null && sub.State != CommunicationState.Opened)
                {
                    sub.Open();
                }
            });
        }

        public void ServiceHostClose()
        {
            _ServiceHostList.ForEach((sub) =>
            {
                if (sub != null && sub.State != CommunicationState.Closed)
                {
                    sub.Close();
                }
            });
        }

        #endregion

        #region LoadServiceHost


        private List<ServiceAndInterfaceInfo> GetService(string assemblyFilePath)
        {
            List<ServiceAndInterfaceInfo> typeList = new List<ServiceAndInterfaceInfo>();
            var serviceAssembly = Assembly.LoadFile(string.Format(assemblyFilePath));
            foreach (Type oType in serviceAssembly.GetTypes())
            {
                //判断类的接口附加了ServiceContractAttribute属性
                var serviceInterfaces = oType.GetInterfaces();
                for (int i = 0; i < serviceInterfaces.Count(); i++)
                {
                    var customerAttributes = serviceInterfaces[i].GetCustomAttributes<ServiceContractAttribute>();
                    if (customerAttributes != null && customerAttributes.Count() > 0)
                    {
                        typeList.Add(new ServiceAndInterfaceInfo()
                        {
                            ServiceType = oType,
                            InterfaceType = serviceInterfaces[i]
                        });
                    }
                }
            }
            return typeList;

        }

        private List<ServiceHost> GetServiceHosts(List<ServiceAndInterfaceInfo> serviceInfo, string baseAddress, Binding binding,
            List<IContractBehavior> contractBehaviors = null, List<IOperationBehavior> operationBehaviors = null, bool metaDataPushable = false)
        {
            List<ServiceHost> serviceHostList = new List<ServiceHost>();

            foreach (var service in serviceInfo)
            {

                baseAddress +=  service.InterfaceType.Name;
                //ServiceHost serviceHost = new ServiceHost(service.ServiceType, new Uri(baseAddress));
                //ServiceEndpoint endpoint = serviceHost.AddServiceEndpoint(service.InterfaceType, binding, "");

                ServiceHost serviceHost = new ServiceHost(service.ServiceType);//初始化的过程中从配置文件中读取服务配置
                if (serviceHost.Description.Endpoints.Count <= 0)
                {
                    ServiceEndpoint endpoint = serviceHost.AddServiceEndpoint(service.InterfaceType, binding, baseAddress);

                    //服务行为
                    if (contractBehaviors != null && contractBehaviors.Count() > 0)
                    {
                        contractBehaviors.ForEach((sub) => { endpoint.Contract.Behaviors.Add(sub); });

                    }

                    //操作行为
                    if (operationBehaviors != null && operationBehaviors.Count() > 0)
                    {
                        foreach (OperationDescription operation in endpoint.Contract.Operations)
                        {
                            operationBehaviors.ForEach((sub) => { operation.Behaviors.Add(sub); });
                        }
                    }

                    //元数据
                    if (metaDataPushable)
                    {
                        ServiceMetadataBehavior smb = serviceHost.Description.Behaviors.Find<ServiceMetadataBehavior>();
                        if (smb == null)
                            smb = new ServiceMetadataBehavior();
                        smb.HttpGetEnabled = true;

                        smb.HttpGetUrl = new Uri(baseAddress);//元数据

                        smb.MetadataExporter.PolicyVersion = PolicyVersion.Default;
                        serviceHost.Description.Behaviors.Add(smb);

                        //serviceHost.AddServiceEndpoint(
                        //    ServiceMetadataBehavior.MexContractName,
                        //    MetadataExchangeBindings.CreateMexHttpBinding(),
                        //    mex"
                        //);
                       // serviceHost.AddServiceEndpoint(
                       //    ServiceMetadataBehavior.MexContractName,
                       //    MetadataExchangeBindings.CreateMexHttpBinding(),
                       //   baseAddress + "/" + "mex"
                       //);
                    }
                }
                serviceHostList.Add(serviceHost);
            }

            return serviceHostList;
        }

        public void LoadServieHost()
        {
            #region IOC

            //string assemblyFilePath = ServiceContainnerConst.AssemblyFilePath;

            var baseAddress = ServiceContainnerConst.BaseAddress;
            Binding binding = ServiceContainnerConst.Binding;
            List<IContractBehavior> contractBehaviors = ServiceContainnerConst.ContractBehaviorList;
            List<IOperationBehavior> operationBehaviors = ServiceContainnerConst.OperationBehaviorList;
            bool metaDataPushable = ServiceContainnerConst.MetaDataPushable;

            #endregion

            var serviceAndInterfaceInfoList = ServiceDiscoverer.Instance.ServiceAndInterfaceInfos;//GetService(assemblyFilePath);

            var serviceHostList = GetServiceHosts(serviceAndInterfaceInfoList, baseAddress,
                binding, contractBehaviors, operationBehaviors, metaDataPushable);

            _ServiceHostList.Clear();
            _ServiceHostList.AddRange(serviceHostList);

        }

        #endregion
    }


    public class ServiceContainnerConst
    {
        public static string AssemblyFilePath
        {
            get
            {
                return string.Format("{0}bin\\{1}", System.AppDomain.CurrentDomain.BaseDirectory,
                    "System.ChesFrame.Service.dll");
            }
        }

        /// <summary>
        ///绑定
        /// </summary>
        public static Binding Binding
        {
            get { return new BasicHttpBinding(); }
            //get
            //{
            //    NetTcpBinding myBinding = new NetTcpBinding();

            //    myBinding.Security.Mode = SecurityMode.None;

            //    myBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;
            //    return myBinding;
            //}
        }

        /// <summary>
        /// 基地址
        /// </summary>
        public static string BaseAddress
        {
            get { return "http://localhost" + ":80/Service/"; }
            //get { return "net.tcp://localhost" + ":824/Service/"; }
        }

        /// <summary>
        /// 服务行为
        /// </summary>
        public static List<IContractBehavior> ContractBehaviorList
        {
            get { return null; }
        }

        /// <summary>
        /// 操作行为
        /// </summary>
        public static List<IOperationBehavior> OperationBehaviorList
        {
            get { return null; }
        }

        /// <summary>
        /// 是否发布元数据
        /// </summary>
        public static bool MetaDataPushable
        {
            get
            {
                return true;
            }
        }
    }

}
