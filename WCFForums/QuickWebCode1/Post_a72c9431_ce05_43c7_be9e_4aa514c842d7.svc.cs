using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;

namespace Post_a72c9431_ce05_43c7_be9e_4aa514c842d7
{
    [ServiceContract]
    public interface IService1
    {
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        int Add(int x, int y);
    }

    public class Service1 : IService1
    {
        public int Add(int x, int y)
        {
            return x + y;
        }
    }

    public class MyJsonPEnabledWebServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            return base.CreateServiceHost(serviceType, baseAddresses);
        }
    }

    class MyJsonPEnabledWebServiceHost : WebServiceHost
    {
        public MyJsonPEnabledWebServiceHost(Type serviceType, Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }

        protected override void OnOpening()
        {
            base.OnOpening();
            foreach (var endpoint in this.Description.Endpoints)
            {
                WebHttpBinding webBinding = endpoint.Binding as WebHttpBinding;
                if (webBinding != null)
                {
                    webBinding.CrossDomainScriptAccessEnabled = true;
                }
            }
        }
    }
}
