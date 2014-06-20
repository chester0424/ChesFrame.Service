using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace System.ChesFrame.Utility.WCF
{
   public  class AuthCheckServiceBehavior : IServiceBehavior 
    {
        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceModel.ServiceHostBase serviceHostBase, Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
            throw new NotImplementedException();
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceModel.ServiceHostBase serviceHostBase)
        {
            //throw new NotImplementedException();
            var headers = OperationContext.Current.IncomingMessageHeaders;
            var index = headers.FindHeader("Name", "http://tempuri.org");
            var name = "";
             if (index > -1)
             {
                name= headers.GetHeader<string>(index);  
             }
        }

        public void Validate(ServiceDescription serviceDescription, ServiceModel.ServiceHostBase serviceHostBase)
        {
            throw new NotImplementedException();
        }
    }
}
