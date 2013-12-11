using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Net;

namespace QuickCode1
{
    public class PostXXXXXXX
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            int Add(int x, int y);
        }
        public class Service : ITest
        {
            public int Add(int x, int y)
            {
                return x + y;
            }
        }
        static Binding GetBinding()
        {
            var result = new BasicHttpBinding();
            return result;
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), GetBinding(), "");
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(GetBinding(), new EndpointAddress(baseAddress));
            ITest proxy = factory.CreateChannel();
            Console.WriteLine(proxy.Add(5, 8));

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }
    public class RestTemplate
    {
        [ServiceContract]
        public class Service
        {
            [WebGet]
            public int Add(int x, int y)
            {
                return x + y;
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            WebServiceHost host = new WebServiceHost(typeof(Service), new Uri(baseAddress));
            host.Open();
            Console.WriteLine("Host opened");

            WebClient c = new WebClient();
            Console.WriteLine(c.DownloadString(baseAddress + "/Add?x=6&y=8"));

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }
    public class DuplexTemplate
    {
        [ServiceContract(CallbackContract = typeof(ICallback))]
        public interface ITest
        {
            [OperationContract]
            string Hello(string text);
        }
        [ServiceContract(Name = "IReallyWantCallback")]
        public interface ICallback
        {
            [OperationContract(IsOneWay = true)]
            void OnHello(string text);
        }
        public class Service : ITest
        {
            public string Hello(string text)
            {
                ICallback callback = OperationContext.Current.GetCallbackChannel<ICallback>();
                ThreadPool.QueueUserWorkItem(delegate
                {
                    callback.OnHello(text);
                });

                return text;
            }
        }
        class MyCallback : ICallback
        {
            AutoResetEvent evt;
            public MyCallback(AutoResetEvent evt)
            {
                this.evt = evt;
            }

            public void OnHello(string text)
            {
                Console.WriteLine("[callback] OnHello({0})", text);
                evt.Set();
            }
        }
        public static void Test()
        {
            string baseAddress = "net.tcp://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), new NetTcpBinding(SecurityMode.None), "");
            host.Open();
            Console.WriteLine("Host opened");

            AutoResetEvent evt = new AutoResetEvent(false);
            MyCallback callback = new MyCallback(evt);
            DuplexChannelFactory<ITest> factory = new DuplexChannelFactory<ITest>(
                new InstanceContext(callback),
                new NetTcpBinding(SecurityMode.None),
                new EndpointAddress(baseAddress));
            ITest proxy = factory.CreateChannel();

            Console.WriteLine(proxy.Hello("foo bar"));
            evt.WaitOne();

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }
}
