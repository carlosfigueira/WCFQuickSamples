using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Net;
using System.ServiceModel.Description;
using System.Runtime.Serialization;

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
    public class AsyncTemplate
    {
        static void Trace(string text, params object[] args)
        {
            if (args != null && args.Length > 0) text = string.Format(text, args);
            Console.WriteLine("[{0}] {1}", DateTime.UtcNow.ToString("HH:mm:ss.fff"), text);
        }

        [ServiceContract]
        public interface ITest
        {
            [OperationContract(AsyncPattern = true)]
            IAsyncResult BeginAdd(int x, int y, AsyncCallback callback, object state);
            int EndAdd(IAsyncResult ar);
        }
        [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Single, InstanceContextMode = InstanceContextMode.PerCall)]
        public class Service : ITest
        {
            delegate int IntDelegate(int x, int y);
            int AddDoWork(int x, int y)
            {
                Trace("Service.AddDoWork, before sleep");
                Thread.Sleep(2000);
                Trace("Service.AddDoWork, after sleep");
                return x + y;
            }
            public IAsyncResult BeginAdd(int x, int y, AsyncCallback callback, object state)
            {
                Trace("Service.BeginAdd");
                IntDelegate theDelegate = new IntDelegate(this.AddDoWork);
                return theDelegate.BeginInvoke(x, y, callback, state);
            }

            public int EndAdd(IAsyncResult ar)
            {
                Trace("Service.EndAdd");
                IntDelegate theDelegate = (IntDelegate)((System.Runtime.Remoting.Messaging.AsyncResult)ar).AsyncDelegate;
                return theDelegate.EndInvoke(ar);
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

            Thread[] threads = new Thread[5];
            for (var i = 0; i < threads.Length; i++)
            {
                threads[i] = new Thread(new ThreadStart(delegate
                {
                    ChannelFactory<ITest> factory = new ChannelFactory<ITest>(GetBinding(), new EndpointAddress(baseAddress));
                    Trace("{0} - Client before call", Thread.CurrentThread.ManagedThreadId);
                    ITest proxy = factory.CreateChannel();
                    proxy.BeginAdd(5, 8, (result) =>
                    {
                        Trace("{0} - Client result: {1}", Thread.CurrentThread.ManagedThreadId, proxy.EndAdd(result));
                        ((IClientChannel)proxy).Close();
                        factory.Close();
                    }, null);
                }));
            }

            Trace("Starting threads...");
            for (var i = 0; i < threads.Length; i++)
            {
                threads[i].Start();
            }

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
    public class WsdlTemplate
    {
        [DataContract]
        public class Person
        {
            [DataMember]
            public string Name { get; set; }
            [DataMember]
            public int Age { get; set; }
        }
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            int Add(int x, int y);
            [OperationContract]
            Person EchoPerson(Person p);
        }
        public class Service : ITest
        {
            public int Add(int x, int y)
            {
                return x + y;
            }

            public Person EchoPerson(Person p)
            {
                return p;
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
            host.Description.Behaviors.Add(new ServiceMetadataBehavior { HttpGetEnabled = true });
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(GetBinding(), new EndpointAddress(baseAddress));
            ITest proxy = factory.CreateChannel();
            Console.WriteLine(proxy.Add(5, 8));

            var p = new Person { Name = "John Doe", Age = 33 };
            var p2 = proxy.EchoPerson(p);
            Console.WriteLine("Person[Name={0}, Age={1}]", p2.Name, p2.Age);

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }
}
