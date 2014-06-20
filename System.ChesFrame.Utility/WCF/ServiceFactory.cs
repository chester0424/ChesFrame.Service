using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace System.ChesFrame.Utility.WCF
{

    public class CustomizeChannelFactory<T> : ChannelFactory<T>
    {
        public CustomizeChannelFactory(Type type)
            : base("*")//通配符方式//默认指定方式 base(type.Name)
        {
            //base.ApplyConfiguration(type.Name);
        }
    }

    public class ServiceFactory<T> : RealProxy where T : class
    {
        public ServiceFactory() : base(typeof(T)) { }
        public override IMessage Invoke(IMessage msg)
        {
            var baseAddress = ServiceFactoryConst.BaseAddress + typeof(T).Name;

            IMethodReturnMessage methodReturn = null;
            IMethodCallMessage methodCall = (IMethodCallMessage)msg;

            //var binding = ServiceFactoryConst.Binding;
            //var factory = new ChannelFactory<T>(binding, new EndpointAddress(baseAddress));
            var factory = new CustomizeChannelFactory<T>(typeof(T));


            //服务行为
            var contractBehaviors = ServiceFactoryConst.ContractBehaviorList;
            if (contractBehaviors != null && contractBehaviors.Count() > 0)
            {
                contractBehaviors.ForEach((sub) => { factory.Endpoint.Contract.Behaviors.Add(sub); });

            }

            //操作行为
            var operationBehaviors = ServiceFactoryConst.OperationBehaviorList;
            if (operationBehaviors != null && operationBehaviors.Count() > 0)
            {
                foreach (OperationDescription operation in factory.Endpoint.Contract.Operations)
                {
                    operationBehaviors.ForEach((sub) => { operation.Behaviors.Add(sub); });
                }
            }


            var channel = factory.CreateChannel();
            try
            {
                object[] copiedArgs = Array.CreateInstance(typeof(object), methodCall.Args.Length) as object[];
                methodCall.Args.CopyTo(copiedArgs, 0);
                object returnValue = methodCall.MethodBase.Invoke(channel, copiedArgs);
                methodReturn = new ReturnMessage(returnValue,
                                                copiedArgs,
                                                copiedArgs.Length,
                                                methodCall.LogicalCallContext,
                                                methodCall);
                //TODO:Write log 
            }
            catch (Exception ex)
            {
                var exception = ex; if (ex.InnerException != null)
                    exception = ex.InnerException;
                methodReturn = new ReturnMessage(exception, methodCall);
            }
            finally
            {
                var commObj = channel as ICommunicationObject; if (commObj != null)
                {
                    try
                    {
                        commObj.Close();
                    }
                    catch (CommunicationException)
                    {
                        commObj.Abort();
                    }
                    catch (TimeoutException)
                    {
                        commObj.Abort();
                    }
                    catch (Exception)
                    {
                        commObj.Abort(); //TODO:Logging exception throw;
                    }
                }
            } return methodReturn;
        }

        public static T Instace
        {
            get
            {
                return (T)new ServiceFactory<T>().GetTransparentProxy();
            }
        }
    }

    public class ServiceFactoryConst
    {
        /// <summary>
        /// 基地址
        /// </summary>
        public static string BaseAddress
        {
            get { return "http://localhost" + ":80/Service/"; }

           // get { return "net.tcp://localhost" + ":824/Service/"; }
        }

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
    }
}
