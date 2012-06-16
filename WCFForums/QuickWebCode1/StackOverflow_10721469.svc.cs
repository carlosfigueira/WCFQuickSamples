using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Web;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Activation;

namespace StackOverflow_10721469
{
    public class ESBAPICreationAttribute : Attribute, IContractBehavior
    {
        public void AddBindingParameters(
            ContractDescription contractDescription,
            ServiceEndpoint endpoint,
            BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(
            ContractDescription contractDescription,
            ServiceEndpoint endpoint,
            ClientRuntime clientRuntime)
        {
        }

        public void ApplyDispatchBehavior(
            ContractDescription contractDescription,
            ServiceEndpoint endpoint,
            DispatchRuntime dispatchRuntime)
        {
            dispatchRuntime.InstanceContextInitializers.Add(new PSESBAPIInitializer());
            Logger.Log("Instance context initializer added");
        }

        public void Validate(
            ContractDescription contractDescription,
            ServiceEndpoint endpoint)
        {
        }
    }
    public class PSESBAPIInitializer : IInstanceContextInitializer
    {
        private static ESBAPIManager _manager = null;

        public PSESBAPIInitializer()
        {
            if (_manager == null)
            {
                _manager = new ESBAPIManager();
                Logger.Log("New instance of API manager initialized");
            }
        }

        public void Initialize(InstanceContext instanceContext, Message message)
        {
            Logger.Log("Extension added to the instance context");

            instanceContext.Extensions.Add(new PSESBAPIExtension(_manager));
        }
    }
    [DataContract]
    public class PSESBAPIExtension : IExtension<InstanceContext>
    {
        private ESBAPIManager _manager;

        [DataMember]
        public ESBAPIManager ESBAPIManager
        {
            get { return _manager; }
        }

        public PSESBAPIExtension(ESBAPIManager Manager)
        {
            Logger.Log("PSESBAPIExtension constructor called");
            _manager = Manager;
        }

        public void Attach(InstanceContext owner)
        {
        }

        public void Detach(InstanceContext owner)
        {
        }
    }
    public class ESBAPIManager { }
    public class PSRestServiceHost : ServiceHost
    {
        public PSRestServiceHost(Type t, Uri[] addresses)
            : base(t, addresses)
        {
            Logger.Log("PSRestServiceHost constructor");
        }
    }
    public class PSRestServiceFactory : ServiceHostFactoryBase
    {
        public override ServiceHostBase CreateServiceHost(string service, Uri[] baseAddresses)
        {
            // The service parameter is ignored here because we know our service. 
            PSRestServiceHost serviceHost = new PSRestServiceHost(typeof(PSRestService), baseAddresses);

            var endpoint = serviceHost.AddServiceEndpoint(typeof(IPSRestService), new WebHttpBinding(), "");
            endpoint.Behaviors.Add(new WebHttpBehavior());

            serviceHost.Opening += new EventHandler(serviceHost_Opening);
            serviceHost.Opened += new EventHandler(serviceHost_Opened);
            serviceHost.Closing += new EventHandler(serviceHost_Closing);
            serviceHost.Closed += new EventHandler(serviceHost_Closed);
            serviceHost.Faulted += new EventHandler(serviceHost_Faulted);
            serviceHost.UnknownMessageReceived += new EventHandler<UnknownMessageReceivedEventArgs>(serviceHost_UnknownMessageReceived);

            return serviceHost;
        }

        void serviceHost_UnknownMessageReceived(object sender, UnknownMessageReceivedEventArgs e)
        {
            Logger.Log("Unknown Message Received");
        }

        void serviceHost_Faulted(object sender, EventArgs e)
        {
            Logger.Log("service faulted");
        }

        void serviceHost_Closed(object sender, EventArgs e)
        {
            Logger.Log("service closed");
        }

        void serviceHost_Closing(object sender, EventArgs e)
        {
            Logger.Log("service closing by sender: {0}", sender.GetType().ToString());
        }

        void serviceHost_Opened(object sender, EventArgs e)
        {
            Logger.Log("service opened by sender: {0}", sender.GetType().ToString());
        }

        void serviceHost_Opening(object sender, EventArgs e)
        {
            Logger.Log("service opening by sender: {0}", sender.GetType().ToString());
        }
    }
    [ServiceContract]
    public interface IPSRestService
    {
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "blockActivityTrans", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        transAckResponseData PostBlockActivity(blockActivityData blockactivitydata);
    }
    [ESBAPICreation]
    public class PSRestService : IPSRestService
    {
        public transAckResponseData PostBlockActivity(blockActivityData blockactivitydata)
        {
            return new transAckResponseData { Logs = Logger.GetCurrentLog() };
        }
    }
    public class blockActivityData { }
    public class transAckResponseData
    {
        public string Logs { get; set; }
    }
    public static class Logger
    {
        static StringBuilder sb = new StringBuilder();
        public static void Log(string text, params object[] args)
        {
            if (args != null && args.Length > 0) text = string.Format(text, args);
            Console.WriteLine(text);
            sb.AppendLine(text);
        }
        public static string GetCurrentLog()
        {
            string result = sb.ToString();
            sb.Clear();
            return result;
        }
    }
}
