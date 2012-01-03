using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Syndication;
using System.ServiceModel.Web;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Windows.Ink;
using System.Windows.Input;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Xml.Xsl;
using UtilCS;

namespace QuickCode3
{
    public class Post_f6284415_e9ec_454d_b075_8c4024999b0e
    {

        //[ServiceContract(Name="TestService")]
        //[ServiceKnownType(typeof(Y))]
        public class TestService : ITest
        {
            //[OperationContract]
            //[WebGet]
            [WebInvoke]
            public X getX()
            {
                return new Y();
            }
        }

        [ServiceContract(Name = "TestService")]
        //[ServiceKnownType(typeof(Y))]
        public interface ITest
        {
            [OperationContract]
            //[WebGet]
            [WebInvoke]
            X getX();
        }

        [KnownType(typeof(Y))]
        public class X
        {
        }

        public class Y : X
        {
        }

        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(TestService), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), new WebHttpBinding(), "").Behaviors.Add(new WebHttpBehavior());
            host.Open();
            Console.WriteLine("Host opened");

            //Util.SendRequest(baseAddress + "/getX", "GET", null, null);
            Util.SendRequest(baseAddress + "/getX", "POST", "text/xml", "");

            WebChannelFactory<ITest> factory = new WebChannelFactory<ITest>(new Uri(baseAddress));
            ITest proxy = factory.CreateChannel();
            Console.WriteLine(proxy.getX());

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_41992794_510e_467c_92b3_645d0329e20d
    {
        public class MyObject
        {
            public int ID;
        }
        [ServiceContract(Name = "MyService2", Namespace = "MyNamespace")]
        public interface IService2
        {
            [OperationContract]
            [WebGet]
            MyObject GetMyObject(int id);
        }
        [ServiceBehavior(Name = "MyService2", Namespace = "Mynamespace", InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
        public class MyService2 : IService2
        {
            public MyObject GetMyObject(int id)
            {
                MyObject mo = new MyObject();
                mo.ID = id;
                return mo;
            }
        }
        public static void Test()
        {
            string baseAddressTcp = "net.tcp://localhost:8000/MyServices/MyService2";
            string baseAddressHttp = "http://localhost:8001/MyServices/MyService2";
            ServiceHost host = new ServiceHost(typeof(MyService2), new Uri(baseAddressHttp), new Uri(baseAddressTcp));
            host.Open();
            Console.WriteLine("Host opened");

            foreach (ChannelDispatcher cd in host.ChannelDispatchers)
            {
                foreach (var ed in cd.Endpoints)
                {
                    Console.WriteLine("  {0}", ed.EndpointAddress.Uri);
                }
            }

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_a43c4cc1_16a4_47f1_8ac4_7f6803dcdb3f
    {
        public class MyServiceHostFactory : ServiceHostFactory
        {
            protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
            {
                return new MyServiceHost(serviceType, baseAddresses);
            }
        }
        public class MyServiceHost : ServiceHost
        {
            public MyServiceHost(Type serviceType, Uri[] baseAddresses)
                : base(serviceType, baseAddresses)
            {
            }

            protected override void InitializeRuntime()
            {
                this.AddServiceEndpoint(typeof(ITest), new BasicHttpBinding(), "");
                ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
                smb.HttpGetEnabled = true;
                this.Description.Behaviors.Add(smb);
                base.InitializeRuntime();
            }

            protected override void OnOpening()
            {
                this.Description.Behaviors.Add(new MyBehavior());
                base.OnOpening();
            }

            protected override void OnOpened()
            {
                base.OnOpened();
            }
        }
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            string GetServiceBehaviors();
        }
        public class Service : ITest
        {
            public string GetServiceBehaviors()
            {
                StringBuilder sb = new StringBuilder();
                foreach (IServiceBehavior isb in OperationContext.Current.Host.Description.Behaviors)
                {
                    sb.AppendLine(isb.GetType().FullName);
                }
                return sb.ToString();
            }
        }
        public class MyBehavior : IServiceBehavior
        {
            public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
            {
            }

            public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
            {
            }

            public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
            {
            }
        }
    }

    public class Post_356c015d_13eb_406a_b8bf_b23ecc7dc2d7
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            XElement Process(XElement data);
        }
        public class Service : ITest
        {
            public XElement Process(XElement data)
            {
                DoSqlMagic(data);
                return data;
            }

            void DoSqlMagic(XElement data)
            {
                XName toChange = "changeme";
                foreach (var node in data.Descendants(toChange))
                {
                    node.Name = "changed";
                }
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), new WebHttpBinding(), "").Behaviors.Add(new WebHttpBehavior());
            host.Open();
            Console.WriteLine("Host opened");

            string xml = "<root><changeme>hello</changeme><donotchangeme>hello world</donotchangeme><changeme>world</changeme></root>";

            Util.SendRequest(baseAddress + "/Process", "POST", "text/xml", xml);

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_356c015d_13eb_406a_b8bf_b23ecc7dc2d7_b
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            Stream Process(Stream data);
        }
        public class Service : ITest
        {
            public Stream Process(Stream data)
            {
                Stream newData = DoSqlMagic(data);
                return newData;
            }

            Stream DoSqlMagic(Stream data)
            {
                string dataStr = new StreamReader(data).ReadToEnd();
                dataStr = dataStr.Replace("changeme", "changed");
                return new MemoryStream(Encoding.UTF8.GetBytes(dataStr));
            }
        }
        class RawMapper : WebContentTypeMapper
        {
            public override WebContentFormat GetMessageFormatForContentType(string contentType)
            {
                return WebContentFormat.Raw;
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            WebHttpBinding binding = new WebHttpBinding();
            binding.ContentTypeMapper = new RawMapper();
            host.AddServiceEndpoint(typeof(ITest), binding, "").Behaviors.Add(new WebHttpBehavior());
            host.Open();
            Console.WriteLine("Host opened");

            string xml = "<changeme>hello</changeme><donotchangeme>hello world</donotchangeme><changeme>world</changeme>";

            Util.SendRequest(baseAddress + "/Process", "POST", "text/xml", xml);

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_f96a2d59_fd3e_41b2_ab45_600d544a89a1
    {
        [ServiceContract()]
        public interface IService
        {
            [OperationContract()]
            string Greet(string _s);

            [OperationContract()]
            int GetNo();

        }

        [Serializable]
        [DurableService()]
        public class Service : IService
        {
            public int iNo = default(int);

            public string Greet(string _s)
            {
                return "Hello " + _s;
            }

            [DurableOperation(CanCreateInstance = true)]
            public int GetNo()
            {
                return iNo;
            }
        }
        public static void Test()
        {
            ServiceHost host = new ServiceHost(typeof(Service));
            host.AddServiceEndpoint(typeof(IService), new NetTcpContextBinding(), "net.tcp://localhost:8050/Service");
            host.Open();
            Console.WriteLine("Service started. Press any key to exit...");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_764b32ef_2394_4cdb_90b6_46bd209b7cb3
    {
        public static void Test()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);
            ServiceModelSectionGroup configGroup = ServiceModelSectionGroup.GetSectionGroup(config);
            Console.WriteLine(configGroup);
            BindingsSection bindings = configGroup.Bindings;
            Console.WriteLine(bindings);
            foreach (BindingCollectionElement bindingCollection in bindings.BindingCollections)
            {
                if (bindingCollection is WebHttpBindingCollectionElement)
                {
                    WebHttpBindingElement binding = new WebHttpBindingElement("foo");
                    Console.WriteLine(bindingCollection);
                }
            }
        }
    }

    public class Post_c39cf01a_28ed_468e_b7e8_0af2a28703f2
    {
        [Serializable]
        [System.Xml.Serialization.XmlType(TypeName = "MyType")]
        public class MyType
        {
            [System.Xml.Serialization.XmlElement(DataType = "dateTime")]
            public DateTime dateTime { get; set; }
            [System.Xml.Serialization.XmlElement(DataType = "date")]
            public DateTime? date;
            [System.Xml.Serialization.XmlElement(DataType = "time")]
            public DateTime? time;
            [System.Xml.Serialization.XmlElement(DataType = "gDay")]
            public string gDay;
            [System.Xml.Serialization.XmlElement(DataType = "gMonth")]
            public string gMonth;
            [System.Xml.Serialization.XmlElement(DataType = "gMonthDay")]
            public string gMonthDay;
            [System.Xml.Serialization.XmlElement(DataType = "gYear")]
            public string gYear;
            [System.Xml.Serialization.XmlElement(DataType = "gYearMonth")]
            public string gYearMonth;
        }

        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            [XmlSerializerFormat]
            MyType Echo(MyType dc);
        }
        public class Service : ITest
        {
            public MyType Echo(MyType dc)
            {
                return dc;
            }
        }
        static Binding GetBinding()
        {
            BasicHttpBinding result = new BasicHttpBinding();
            //Change binding settings here
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
            //Console.WriteLine(proxy.Echo("Hello"));

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_c39cf01a_28ed_468e_b7e8_0af2a28703f2_b
    {
        [XmlSchemaProvider("ProvideSchema")]
        public class gMonth : IXmlSerializable
        {
            int theMonth;
            public gMonth() : this(0) { }
            public gMonth(int month) { this.theMonth = month; }

            public static XmlQualifiedName ProvideSchema(XmlSchemaSet xs)
            {
                return new XmlQualifiedName("gMonth", "http://www.w3.org/2001/XMLSchema");
            }

            public XmlSchema GetSchema()
            {
                return null;
            }

            public void ReadXml(XmlReader reader)
            {
                // additional parsing can be done here...
                this.theMonth = reader.ReadElementContentAsInt();
            }

            public void WriteXml(XmlWriter writer)
            {
                writer.WriteValue(String.Format("{0:00}", this.theMonth));
            }
        }

        [XmlSchemaProvider("ProvideSchema")]
        public class time : IXmlSerializable
        {
            DateTime theTime;
            public time() : this(new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc)) { }
            public time(DateTime dt) { this.theTime = dt; }

            public static XmlQualifiedName ProvideSchema(XmlSchemaSet xs)
            {
                return new XmlQualifiedName("time", "http://www.w3.org/2001/XMLSchema");
            }

            public XmlSchema GetSchema()
            {
                return null;
            }

            public void ReadXml(XmlReader reader)
            {
                // additional parsing can be done here...
                this.theTime = reader.ReadElementContentAsDateTime();
            }

            public void WriteXml(XmlWriter writer)
            {
                writer.WriteValue(this.theTime.ToString("hh:mm:ss.fff", CultureInfo.InvariantCulture));
            }
        }

        [DataContract]
        public class MyType
        {
            [DataMember]
            public gMonth month = new gMonth(11);
            [DataMember]
            public time time = new time(DateTime.Now);
        }

        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            MyType Echo(MyType type);
        }
        public class Service : ITest
        {
            public MyType Echo(MyType type) { return type; }
        }
        static Binding GetBinding() { return new BasicHttpBinding(); }
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
            //Console.WriteLine(proxy.Echo("Hello"));

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_09aacb16_565d_4850_b6c4_3a420d06ef9c
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            string EchoString(string text);
        }
        public class Service : ITest
        {
            public string EchoString(string text)
            {
                return text;
            }
        }
        static Binding GetBinding()
        {
            WSHttpBinding result = new WSHttpBinding();
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

            Console.WriteLine(proxy.EchoString("Hello\nworld"));

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_98dcc2ba_f44a_4038_8125_4abc456f75e9
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            string EchoString(string text);
        }
        public class Service : ITest
        {
            public string EchoString(string text)
            {
                HttpRequestMessageProperty prop = (HttpRequestMessageProperty)OperationContext.Current.IncomingMessageProperties[HttpRequestMessageProperty.Name];
                return prop.Headers[HttpRequestHeader.Host];
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), new BasicHttpBinding(), "");
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(new BasicHttpBinding(), new EndpointAddress(baseAddress));
            ITest proxy = factory.CreateChannel();

            Console.WriteLine(proxy.EchoString("Hello"));

            using (new OperationContextScope((IContextChannel)proxy))
            {
                HttpRequestMessageProperty prop = new HttpRequestMessageProperty();
                prop.Headers[HttpRequestHeader.Host] = "flaviaamaral2";
                OperationContext.Current.OutgoingMessageProperties.Add(HttpRequestMessageProperty.Name, prop);
                Console.WriteLine(proxy.EchoString("Hello"));
            }

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_a57dff69_f9cd_4471_8a2e_370786b3d419
    {
        const string SoapBindingName = "SoapBinding";
        const string RestBindingName = "RestBinding";
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            string EchoString(string text);
        }
        public class Service : ITest
        {
            public string EchoString(string text)
            {
                return text;
            }
        }
        class MySoapErrorHandler : IErrorHandler
        {
            public bool HandleError(Exception error) { throw new NotImplementedException(); }
            public void ProvideFault(Exception error, MessageVersion version, ref Message fault) { throw new NotImplementedException(); }
        }
        class MyRestErrorHandler : IErrorHandler
        {
            public bool HandleError(Exception error) { throw new NotImplementedException(); }
            public void ProvideFault(Exception error, MessageVersion version, ref Message fault) { throw new NotImplementedException(); }
        }
        class MyBehavior : IServiceBehavior
        {
            public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
            {
            }

            public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
            {
                foreach (ChannelDispatcher cd in serviceHostBase.ChannelDispatchers)
                {
                    if (cd.BindingName == RestBindingName)
                    {
                        cd.ErrorHandlers.Add(new MyRestErrorHandler());
                    }
                    else if (cd.BindingName == SoapBindingName)
                    {
                        cd.ErrorHandlers.Add(new MySoapErrorHandler());
                    }
                }
            }

            public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
            {
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            Binding soapBinding = new BasicHttpBinding() { Name = SoapBindingName };
            Binding restBinding = new WebHttpBinding() { Name = RestBindingName };
            host.AddServiceEndpoint(typeof(ITest), soapBinding, "SOAP");
            host.AddServiceEndpoint(typeof(ITest), restBinding, "REST").Behaviors.Add(new WebHttpBehavior());
            host.Description.Behaviors.Add(new MyBehavior());
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> soapFactory = new ChannelFactory<ITest>(new BasicHttpBinding(), new EndpointAddress(baseAddress + "/SOAP"));
            ITest proxy = soapFactory.CreateChannel();
            Console.WriteLine(proxy.EchoString("Hello"));
            ((IClientChannel)proxy).Close();
            soapFactory.Close();

            ChannelFactory<ITest> restFactory = new ChannelFactory<ITest>(new WebHttpBinding(), new EndpointAddress(baseAddress + "/REST"));
            restFactory.Endpoint.Behaviors.Add(new WebHttpBehavior());
            proxy = restFactory.CreateChannel();
            Console.WriteLine(proxy.EchoString("Hello"));
            ((IClientChannel)proxy).Close();
            restFactory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_f6459d87_a6c6_41c7_a6df_8f45c6789965
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            [WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
            string PostViaBase64(string base64);
            [OperationContract]
            [WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
            string PostViaBinary(Stream data);
            [OperationContract]
            [WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
            string PostViaByteArray(byte[] imgBytes);
        }
        public class Service : ITest
        {
            public string PostViaBase64(string base64)
            {
                byte[] imgBytes = Convert.FromBase64String(base64);
                return String.Format("First few bytes: {0:X2} {1:X2} {2:X2}", imgBytes[0], imgBytes[1], imgBytes[2]);
            }

            public string PostViaBinary(Stream data)
            {
                byte[] imgBytes = new byte[3];
                data.Read(imgBytes, 0, imgBytes.Length);
                return String.Format("First few bytes: {0:X2} {1:X2} {2:X2}", imgBytes[0], imgBytes[1], imgBytes[2]);
            }

            public string PostViaByteArray(byte[] imgBytes)
            {
                return String.Format("First few bytes: {0:X2} {1:X2} {2:X2}", imgBytes[0], imgBytes[1], imgBytes[2]);
            }
        }
        static void SendRequest(string uri, string contentType, byte[] body)
        {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(uri);
            req.Method = "POST";
            req.ContentType = contentType;
            req.GetRequestStream().Write(body, 0, body.Length);
            req.GetRequestStream().Close();

            HttpWebResponse resp;
            try
            {
                resp = (HttpWebResponse)req.GetResponse();
            }
            catch (WebException e)
            {
                resp = (HttpWebResponse)e.Response;
            }
            Console.WriteLine("HTTP/{0} {1} {2}", resp.ProtocolVersion, (int)resp.StatusCode, resp.StatusDescription);
            foreach (string headerName in resp.Headers.AllKeys)
            {
                Console.WriteLine("{0}: {1}", headerName, resp.Headers[headerName]);
            }
            Console.WriteLine();
            Stream respStream = resp.GetResponseStream();
            if (respStream != null)
            {
                string responseBody = new StreamReader(respStream).ReadToEnd();
                Console.WriteLine(responseBody);
            }
            else
            {
                Console.WriteLine("HttpWebResponse.GetResponseStream returned null");
            }
            Console.WriteLine();
            Console.WriteLine("  *-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*  ");
            Console.WriteLine();
        }
        static byte[] GetJsonArray(byte[] data)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('[');
            for (int i = 0; i < data.Length; i++)
            {
                if (i > 0) sb.Append(',');
                sb.Append((int)data[i]);
            }
            sb.Append(']');
            return Encoding.UTF8.GetBytes(sb.ToString());
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), new WebHttpBinding(), "").Behaviors.Add(new WebHttpBehavior());
            host.Open();
            Console.WriteLine("Host opened");

            byte[] image = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 }; // some image array
            string imageBase64 = Convert.ToBase64String(image);
            string jsonInput = "\"" + imageBase64 + "\"";
            byte[] jsonInputBytes = Encoding.UTF8.GetBytes(jsonInput);

            SendRequest(baseAddress + "/PostViaBase64", "application/json", jsonInputBytes);
            SendRequest(baseAddress + "/PostViaBinary", "image/jpeg", image);
            SendRequest(baseAddress + "/PostViaByteArray", "application/json", GetJsonArray(image));

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_25b586e1_04a4_4ac3_b3f8_f06391f4bfb4
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            [WebInvoke(Method = "PUT", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
            string Process(Stream input);
        }
        public class Service : ITest
        {
            public string Process(Stream input)
            {
                byte[] imgBytes = new byte[6];
                input.Read(imgBytes, 0, imgBytes.Length);
                return String.Format("First few bytes: {0:X2} {1:X2} {2:X2} {3:X2} {4:X2} {5:X2}",
                    imgBytes[0], imgBytes[1], imgBytes[2],
                    imgBytes[3], imgBytes[4], imgBytes[5]);
            }
        }
        class RawMapper : WebContentTypeMapper
        {
            public override WebContentFormat GetMessageFormatForContentType(string contentType)
            {
                return WebContentFormat.Raw;
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            WebHttpBinding binding = new WebHttpBinding();
            binding.ContentTypeMapper = new RawMapper();
            host.AddServiceEndpoint(typeof(ITest), binding, "").Behaviors.Add(new WebHttpBehavior());
            host.Open();
            Console.WriteLine("Host opened");

            Util.SendRequest(baseAddress + "/Process", "PUT", "application/xml; charset=utf-8", "<productId>abcdef</productId>");

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_e85a927c_9379_45ef_8bae_c7af01ba99b7
    {
        [ServiceContract]
        public interface ITestCallback
        {
            [OperationContract(IsOneWay = true)]
            void OnHello(string name);
        }
        [ServiceContract(CallbackContract = typeof(ITestCallback))]
        public interface ITest
        {
            [OperationContract]
            void Hello(string name);
        }
        public class Service : ITest
        {
            public void Hello(string name)
            {
                OperationContext.Current.GetCallbackChannel<ITestCallback>().OnHello(name);
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddDefaultEndpoints();
            host.AddServiceEndpoint(typeof(ITest), new PollingDuplexHttpBinding(), "pd");
            //host.AddServiceEndpoint(typeof(ITest), new WSDualHttpBinding(WSDualHttpSecurityMode.None), "ws");
            host.Open();
            Console.WriteLine("Host opened");

            foreach (ServiceEndpoint se in host.Description.Endpoints)
                Console.WriteLine("A: {0}, B: {1}, C: {2}",
                    se.Address, se.Binding.Name, se.Contract.Name);

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_2d31ee55_3d24_474f_a4bf_9dcba67b4c26
    {
        [XmlInclude(typeof(MyType))]
        public class Payload
        {
            public object CustomType { get; set; }
        }
        public class MyType
        {
            public string str { get; set; }
        }
        public class MyGenericSurrogate : ISerializationSurrogate
        {
            const string TypeKey = "__Object_Type__";
            const string ObjDataKey = "__Object_Data__";
            public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
            {
                MemoryStream ms = new MemoryStream();
                Type objType = obj.GetType();
                XmlSerializer xs = new XmlSerializer(objType);
                xs.Serialize(ms, obj);
                info.AddValue(TypeKey, objType.FullName);
                info.AddValue(ObjDataKey, ms.ToArray(), typeof(byte[]));
            }

            public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
            {
                Type objType = Type.GetType(info.GetString(TypeKey));
                byte[] objData = (byte[])info.GetValue(ObjDataKey, typeof(byte[]));
                XmlSerializer xs = new XmlSerializer(objType);
                return xs.Deserialize(new MemoryStream(objData));
            }
        }
        public static void Test()
        {
            XmlSerializer xs = new XmlSerializer(typeof(Payload));
            MemoryStream ms = new MemoryStream();
            XmlWriterSettings ws = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  ",
                Encoding = Encoding.UTF8,
                OmitXmlDeclaration = true,
            };
            XmlWriter w = XmlWriter.Create(ms, ws);
            Payload obj = new Payload
            {
                CustomType = new MyType
                {
                    str = "Hello world",
                }
            };

            SurrogateSelector ss = new SurrogateSelector();
            ss.AddSurrogate(typeof(object), new StreamingContext(StreamingContextStates.All), new MyGenericSurrogate());
            IFormatter f = (IFormatter)xs;
            f.SurrogateSelector = ss;
            xs.Serialize(w, obj);
            w.Flush();
            Console.WriteLine(Encoding.UTF8.GetString(ms.ToArray()));
        }
    }

    public class Post_34cbc899_5253_4148_81ea_4cc5722c869c
    {
        [DataContract]
        public class ReferenceItem
        {
            [DataMember]
            public int ItemId;
            [DataMember]
            public string ItemName;
        }
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            [WebInvoke(Method = "GET",
              UriTemplate = "/EntityData/{entity}",
              BodyStyle = WebMessageBodyStyle.Wrapped,
              ResponseFormat = WebMessageFormat.Json)]
            [return: MessageParameter(Name = "SomeOtherName")]
            List<ReferenceItem> GetSearchDropDown(string entity);
        }
        [ServiceContract]
        public interface ITest2
        {
            [OperationContract]
            [WebInvoke(Method = "GET",
              UriTemplate = "/EntityData/{entity}",
              BodyStyle = WebMessageBodyStyle.Wrapped,
              ResponseFormat = WebMessageFormat.Json)]
            List<ReferenceItem> GetSearchDropDown(string entity);
        }
        public class Service : ITest, ITest2
        {
            List<ReferenceItem> ITest.GetSearchDropDown(string entity)
            {
                return new List<ReferenceItem>
                {
                  new ReferenceItem { ItemId = 1, ItemName = entity },
                };
            }

            List<ReferenceItem> ITest2.GetSearchDropDown(string entity)
            {
                return ((ITest)this).GetSearchDropDown(entity);
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), new WebHttpBinding(), "NameChange").Behaviors.Add(new WebHttpBehavior());
            host.AddServiceEndpoint(typeof(ITest2), new WebHttpBinding(), "NoNameChange").Behaviors.Add(new WebHttpBehavior());
            host.Open();
            Console.WriteLine("Host opened");

            Util.SendRequest(baseAddress + "/NameChange/EntityData/MyEntity", "GET", null, null);
            Util.SendRequest(baseAddress + "/NoNameChange/EntityData/MyEntity", "GET", null, null);

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_6fbd8619_69d7_4931_9979_ec9f6649e2ec
    {
        public class MyGenericFactory<IContract> : ServiceHostFactory
        {
            protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
            {
                return base.CreateServiceHost(serviceType, baseAddresses);
            }
        }
        public class MyServiceHost<IContract> : ServiceHost
        {
            public MyServiceHost(Type serviceType, Uri[] baseAddresses) : base(serviceType, baseAddresses) { }

            protected override void InitializeRuntime()
            {
                this.AddServiceEndpoint(typeof(IContract), new BasicHttpBinding(), "");
                ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
                smb.HttpGetEnabled = true;
                this.Description.Behaviors.Add(smb);

                base.InitializeRuntime();
            }
        }
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            string Echo(string text);
        }
        public class Service : ITest
        {
            public string Echo(string text) { return text; }
        }
        public static void Test()
        {
            Console.WriteLine(typeof(MyGenericFactory<ITest>).FullName);
        }
    }

    public class Post_cc6873e9_6c1e_4333_bc8a_71e55b4d4f62
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            string EchoString(string text);
            [OperationContract]
            int Add(int x, int y);
        }
        [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
        public class Service : ITest
        {
            public string EchoString(string text)
            {
                ServiceBehaviorAttribute sba = OperationContext.Current.Host.Description.Behaviors.Find<ServiceBehaviorAttribute>();
                if (sba == null)
                {
                    Console.WriteLine("Cannot find SBA");
                }
                else
                {
                    Console.WriteLine("{0} - {1}", sba.ConcurrencyMode, sba.InstanceContextMode);
                }
                return text;
            }

            public int Add(int x, int y)
            {
                Console.WriteLine("[{0}] Inside Add", DateTime.Now.ToString("MM/dd hh:mm:ss.fff", CultureInfo.InvariantCulture));
                Thread.CurrentThread.Join(1000);
                return x + y;
            }
        }
        static Binding GetBinding()
        {
            BasicHttpBinding result = new BasicHttpBinding();
            return result;
        }
        static ITest proxy;
        static int proxyCalls = 10;
        static readonly string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
        public static void Test()
        {
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), GetBinding(), "");
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(GetBinding(), new EndpointAddress(baseAddress));
            proxy = factory.CreateChannel();

            System.Timers.Timer t = new System.Timers.Timer();
            t.Interval = 200;
            t.Elapsed += new System.Timers.ElapsedEventHandler(t_Elapsed);
            t.Start();

            Console.WriteLine("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }

        static void t_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            System.Timers.Timer t = (System.Timers.Timer)sender;
            //ChannelFactory<ITest> factory = new ChannelFactory<ITest>(GetBinding(), new EndpointAddress(baseAddress));
            //ITest proxy = factory.CreateChannel();
            int result = proxy.Add(proxyCalls, 4);
            Console.WriteLine("[{0}] Result of add: {1}", DateTime.Now.ToString("MM/dd hh:mm:ss.fff", CultureInfo.InvariantCulture), result);
            if (Interlocked.Decrement(ref proxyCalls) == 0)
            {
                t.Stop();
            }
        }
    }

    public class Post_a14dd9a5_edf3_464b_8e45_8001a0be8fcc
    {
        [ServiceContract]
        public interface IService1
        {
            [OperationContract]
            [WebGet]
            string EchoWithGet(string s);

            [OperationContract]
            [WebInvoke]
            string EchoWithPost(string s);
        }
        public class Service1 : IService1
        {
            public string EchoWithGet(string s)
            {
                return "You said GET " + s;
            }

            public string EchoWithPost(string s)
            {
                return "You said POST " + s;
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            WebServiceHost host = new WebServiceHost(typeof(Service1), new Uri(baseAddress));
            host.Open();
            Console.WriteLine("Host opened");

            WebChannelFactory<IService1> factory = new WebChannelFactory<IService1>(new Uri(baseAddress));
            IService1 proxy = factory.CreateChannel();

            Console.WriteLine(proxy.EchoWithGet("Hello"));
            Console.WriteLine(proxy.EchoWithPost("World"));

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_98ae5c3a_cd4f_47c9_9efe_b9a002fe04fd
    {
        [ServiceContract(Name = "ITest")]
        public interface ITest
        {
            [OperationContract]
            int Add(int x, int y);
        }
        [ServiceContract(Name = "ITest")]
        public interface ITestAsync
        {
            [OperationContract(AsyncPattern = true)]
            IAsyncResult BeginAdd(int x, int y, AsyncCallback callback, object userState);
            int EndAdd(IAsyncResult asyncResult);
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
            BasicHttpBinding result = new BasicHttpBinding();
            return result;
        }
        class MyState
        {
            public ITestAsync proxy;
            public int paramX;
            public int paramY;
        }
        static void AddCallback(IAsyncResult asyncResult)
        {
            MyState state = (MyState)asyncResult.AsyncState;
            Console.WriteLine("Parameters passed to the BeginAdd call: {0}, {1}", state.paramX, state.paramY);
            Console.WriteLine("Result: {0}", state.proxy.EndAdd(asyncResult));
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), GetBinding(), "");
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITestAsync> factory = new ChannelFactory<ITestAsync>(GetBinding(), new EndpointAddress(baseAddress));
            ITestAsync proxy = factory.CreateChannel();

            proxy.BeginAdd(3, 5, new AsyncCallback(AddCallback), new MyState { proxy = proxy, paramX = 3, paramY = 5 });

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_f614ee00_2bc4_4816_9c02_0dcdfe65d33a
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            string EchoString(string text);
        }
        public class Service : ITest
        {
            public string EchoString(string text)
            {
                Uri thisUri = OperationContext.Current.EndpointDispatcher.EndpointAddress.Uri;
                string whichEndpoint = null;
                foreach (var ep in OperationContext.Current.Host.Description.Endpoints)
                {
                    if (ep.Address.Uri == thisUri)
                    {
                        whichEndpoint = ep.Name;
                    }
                }
                return text + " from " + whichEndpoint;
            }
        }
        static void CallService(Binding binding, string address)
        {
            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(binding, new EndpointAddress(address));
            ITest proxy = factory.CreateChannel();

            Console.WriteLine(proxy.EchoString("Hello"));

            ((IClientChannel)proxy).Close();
            factory.Close();
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            string baseAddressTcp = "net.tcp://" + Environment.MachineName + ":8888/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress), new Uri(baseAddressTcp));
            ServiceEndpoint bhb = host.AddServiceEndpoint(typeof(ITest), new BasicHttpBinding(), "basic");
            bhb.Name = "BasicHttpBinding_endpoint";
            ServiceEndpoint ws = host.AddServiceEndpoint(typeof(ITest), new WSHttpBinding(SecurityMode.None), "ws");
            ws.Name = "WSHttpBinding_endpoint";
            ServiceEndpoint netTcp = host.AddServiceEndpoint(typeof(ITest), new NetTcpBinding(SecurityMode.None), "tcp");
            netTcp.Name = "NetTcpBinding_endpoint";
            host.Open();
            Console.WriteLine("Host opened");

            CallService(new BasicHttpBinding(), baseAddress + "/basic");
            CallService(new WSHttpBinding(SecurityMode.None), baseAddress + "/ws");
            CallService(new NetTcpBinding(SecurityMode.None), baseAddressTcp + "/tcp");

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_655958df_cce3_472b_99f1_2d3018e3d007
    {
        [ServiceContract(Namespace = "somenamespace")]
        [XmlSerializerFormat]
        public interface ITest
        {
            [OperationContract]
            string HelloRequest([XmlAttribute] string version);
        }
        public class Service : ITest
        {
            public string HelloRequest(string version)
            {
                return version;
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), new BasicHttpBinding(), "");
            host.Open();

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(new BasicHttpBinding(), new EndpointAddress(baseAddress));
            ITest proxy = factory.CreateChannel();
            Console.WriteLine(proxy.HelloRequest("1"));
        }
    }

    public class Post_b8513225_2b84_4913_8017_125edd55e4be
    {
        [ServiceContract]
        public interface ILibrary
        {
            [OperationContract]
            [WebGet(UriTemplate = "GetAuthors")]
            [PListBehavior]
            Person[] GetAuthors();

            [OperationContract]
            [WebGet(UriTemplate = "GetBooks")]
            [PListBehavior]
            Book[] GetBooks();

            [OperationContract]
            [WebGet(UriTemplate = "GetPublishers")]
            [PListBehavior]
            Publisher[] GetPublishers();

            [OperationContract]
            [WebGet(UriTemplate = "GetLibrary")]
            [PListBehavior]
            Library GetLibrary();

            [OperationContract]
            [WebGet(UriTemplate = "GetTags")]
            [PListBehavior]
            string[] GetTags();

            [OperationContract]
            [WebGet(UriTemplate = "GetBooksByTags/{tag}")]
            [PListBehavior]
            Book[] GetBooksByTags(string tag);
        }

        public class Person { }
        public class Book { }
        public class Publisher { }
        public class Library { }

        public class PListBehaviorAttribute : Attribute, IOperationBehavior
        {
            public void AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters) { }
            public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation) { }
            public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation) { }
            public void Validate(OperationDescription operationDescription) { }
        }

        public class DataContractPListSerializer : XmlObjectSerializer
        {
            DataContractSerializer dcs;
            public DataContractPListSerializer(Type type)
            {
                dcs = new DataContractSerializer(type);
            }

            public override bool IsStartObject(XmlDictionaryReader reader)
            {
                return this.dcs.IsStartObject(reader);
            }

            public override object ReadObject(XmlDictionaryReader reader, bool verifyObjectName)
            {
                return this.dcs.ReadObject(reader, verifyObjectName);
            }

            public override void WriteEndObject(XmlDictionaryWriter writer)
            {
                this.dcs.WriteEndObject(writer);
            }

            public override void WriteObjectContent(XmlDictionaryWriter writer, object graph)
            {
                writer.WriteAttributeString("__usingNewSerializer", "true");
                this.dcs.WriteObjectContent(writer, graph);
            }

            public override void WriteStartObject(XmlDictionaryWriter writer, object graph)
            {
                this.dcs.WriteStartObject(writer, graph);
            }
        }

        public class MyWebHttpBehavior : WebHttpBehavior
        {
            protected override IDispatchMessageFormatter GetReplyDispatchFormatter(OperationDescription operationDescription, ServiceEndpoint endpoint)
            {
                if (operationDescription.Behaviors.Find<PListBehaviorAttribute>() != null)
                {
                    if (operationDescription.Messages[1].Body.ReturnValue.Type != typeof(void))
                    {
                        return new MyReplyDispatchFormatter(operationDescription);
                    }
                }

                return base.GetReplyDispatchFormatter(operationDescription, endpoint);
            }

            class MyReplyDispatchFormatter : IDispatchMessageFormatter
            {
                private OperationDescription operationDescription;

                public MyReplyDispatchFormatter(OperationDescription operationDescription)
                {
                    this.operationDescription = operationDescription;
                }

                public void DeserializeRequest(Message message, object[] parameters)
                {
                    throw new NotSupportedException("This is a reply-only formatter");
                }

                public Message SerializeReply(MessageVersion messageVersion, object[] parameters, object result)
                {
                    string action = this.operationDescription.Messages[1].Action;
                    Type resultType = this.operationDescription.Messages[1].Body.ReturnValue.Type;
                    DataContractPListSerializer serializer = new DataContractPListSerializer(resultType);
                    return Message.CreateMessage(messageVersion, action, result, serializer);
                }
            }
        }

        public class LibraryService : ILibrary
        {
            public Person[] GetAuthors()
            {
                return new Person[] { new Person() };
            }

            public Book[] GetBooks()
            {
                return new Book[] { new Book() };
            }

            public Publisher[] GetPublishers()
            {
                return new Publisher[] { new Publisher() };
            }

            public Library GetLibrary()
            {
                return new Library();
            }

            public string[] GetTags()
            {
                return "hello world".Split(' ');
            }

            public Book[] GetBooksByTags(string tag)
            {
                return new Book[] { new Book() };
            }

            public void ReturnVoid() { }
        }

        public static void Test()
        {
            string baseAddress = "http://localhost:8000/Service";
            ServiceHost host = new ServiceHost(typeof(LibraryService), new Uri(baseAddress));

            WebHttpBinding webBinding = new WebHttpBinding(WebHttpSecurityMode.None);
            BasicHttpBinding basicBinding = new BasicHttpBinding(BasicHttpSecurityMode.None);

            ServiceMetadataBehavior metadataBehavior = new ServiceMetadataBehavior { HttpGetEnabled = true, HttpGetUrl = new Uri(host.BaseAddresses[0].AbsoluteUri + "/mex") };
            WebHttpBehavior httpBehavior = new MyWebHttpBehavior { DefaultOutgoingResponseFormat = System.ServiceModel.Web.WebMessageFormat.Xml };
            //WebHttpBehavior httpBehavior = new PListWebHttpBehavior { DefaultOutgoingResponseFormat = System.ServiceModel.Web.WebMessageFormat.Xml };

            host.AddServiceEndpoint(typeof(ILibrary), webBinding, string.Empty);
            host.AddServiceEndpoint(typeof(ILibrary), basicBinding, "basic");

            host.Description.Behaviors.Add(metadataBehavior);
            host.Description.Endpoints[0].Behaviors.Add(httpBehavior);

            host.Open();

            Util.SendRequest(baseAddress + "/GetAuthors", "GET", null, null);

            Console.WriteLine("Press ENTER.");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_35e0d7b9_9c7f_424d_87e8_0fa9c7336b67
    {
        [DataContract]
        public class MyDC
        {
            [DataMember]
            public string str = "The string";
            [DataMember]
            public int i = 1234;
        }
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            [WebGet]
            MyDC GetDC();
        }
        public class Service : ITest
        {
            public MyDC GetDC()
            {
                MyDC result = new MyDC { i = 123, str = "hello world" };
                string userAgent = WebOperationContext.Current.IncomingRequest.Headers[HttpRequestHeader.UserAgent];
                if (userAgent != null)
                {
                    if (userAgent.Contains("compatible; MSIE"))
                    {
                        // Internet Explorer, returning XML
                        WebOperationContext.Current.OutgoingResponse.Format = WebMessageFormat.Xml;
                    }
                    else if (userAgent.Contains("Firefox/"))
                    {
                        // Firefox, returning Json
                        WebOperationContext.Current.OutgoingResponse.Format = WebMessageFormat.Json;
                    }
                    else
                    {
                        // Others, return XML
                        WebOperationContext.Current.OutgoingResponse.Format = WebMessageFormat.Json;
                    }
                }
                else
                {
                    // No User-Agent, return default (XML)
                }
                return result;
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            WebServiceHost host = new WebServiceHost(typeof(Service), new Uri(baseAddress));
            host.Open();
            Console.WriteLine("Host opened");

            Util.SendRequest(baseAddress + "/GetDC", "GET", null, null);

            Console.WriteLine("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_bad614e9_9bf1_4062_b0b5_e5beba4539b5
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract, WebGet]
            string GetRequest();
        }

        public class Service : ITest
        {
            public string GetRequest()
            {
                WebOperationContext context = WebOperationContext.Current;
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Request URI: " + context.IncomingRequest.UriTemplateMatch.RequestUri.ToString());
                sb.AppendLine("Headers:");
                foreach (var header in context.IncomingRequest.Headers.AllKeys)
                {
                    sb.AppendLine(String.Format("  {0}: {1}", header, context.IncomingRequest.Headers[header]));
                }
                return sb.ToString();
            }
        }

        static void Test(string address, bool decompressionEnabled)
        {
            Console.WriteLine("Calling with DecompressionEnabled = {0}", decompressionEnabled);
            CustomBinding binding = new CustomBinding(new WebHttpBinding());
            binding.Elements.Find<HttpTransportBindingElement>().DecompressionEnabled = decompressionEnabled;
            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(binding, new EndpointAddress(address));
            factory.Endpoint.Behaviors.Add(new WebHttpBehavior());
            ITest proxy = factory.CreateChannel();
            Console.WriteLine(proxy.GetRequest());
            ((IClientChannel)proxy).Close();
            factory.Close();
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            WebServiceHost host = new WebServiceHost(typeof(Service), new Uri(baseAddress));
            host.Open();

            Test(baseAddress, true);
            Test(baseAddress, false);
        }
    }

    public class Post_9664913e_cb3d_4ac8_a73d_cd1d48107006
    {
        const string json = "{" +
"Summary:{pages:1,page:1,total:1,hqtime:\"20100621143525\"}," +
"Column:{id:0,code:1,name:2,hqtime:3,time:4,lcp:5,op:6,hp:7,lp:8,np:9,tm:10," +
    "pm:11,ta:12,pa:13,hlp:14,pl:15,avgp:16,bp1:17,ba1:18,bp2:19,ba2:20,bp3:21," +
    "ba3:22,bp4:23,ba4:24,bp5:25,ba5:26,sp1:27,sa1:28,sp2:29,sa2:30,sp3:31,sa3:32," +
    "sp4:33,sa4:34,sp5:35,sa5:36,bsa:37,inna:38,outa:39,hhp:40,llp:41,cot:42,cat:43," +
    "tr:44,pens:45,ape:46,sl:47,ceqt:48,teqt:49,cmv:50,tmv:51,apb:52,stp:53,apesh:54}," +
"HqData:[" +
    "[\"sz002318\",\"002318\",\"JiuLiTeCai\",\"20100621143525\",\"20100621143523\",20.25," +
        "20.08,20.78,19.2,20.72,3772.15,7.87,18768,38,0.47,2.32,20.1,20.72,1200,20.71,12400," +
        "20.7,4550,20.69,5770,20.68,500,20.73,2800,20.74,2700,20.75,3802,20.77,100,20.78,1400," +
        "1,7574,12219,22.28,18.23,38.66,0.55,3.61,2418,142.5,7.8,52000000,208000000,107744," +
        "430976,2.48,0,51.8]," +
    "[\"99sz002318\",\"99002318\",\"99JiuLiTeCai\",\"9920100621143525\",\"9920100621143523\"," +
    "9920.25,9920.08,9920.78,9919.2,9920.72,993772.15,997.87,9918768,9938,990.47,992.32,9920.1," +
    "9920.72,991200,9920.71,9912400,9920.7,994550,9920.69,995770,9920.68,99500,9920.73,992800," +
    "9920.74,992700,9920.75,993802,9920.77,99100,9920.78,991400,991,997574,9912219,9922.28," +
    "9918.23,9938.66,990.55,993.61,992418,99142.5,997.8,9952000000,99208000000,99107744," +
    "99430976,992.48,990,9951.8]" +
"]}";

        public class HQS
        {
            public Dictionary<string, object> Summary;
            public Dictionary<string, int> Column;
            public object[][] HqData;
        }

        public static void Test()
        {
            System.Web.Script.Serialization.JavaScriptSerializer ser = new System.Web.Script.Serialization.JavaScriptSerializer();
            HQS hqs1 = ser.Deserialize<HQS>(json);
            Console.WriteLine("Summary:");
            foreach (string summaryKey in hqs1.Summary.Keys)
            {
                Console.WriteLine("   {0}: {1}", summaryKey, hqs1.Summary[summaryKey]);
            }
            string[] columnNames = new string[hqs1.Column.Count];
            foreach (string colName in hqs1.Column.Keys)
            {
                columnNames[hqs1.Column[colName]] = colName;
            }
            for (int i = 0; i < hqs1.HqData.Length; i++)
            {
                Console.WriteLine("HqData[{0}]", i);
                for (int j = 0; j < hqs1.HqData[i].Length; j++)
                {
                    Console.WriteLine("  {0} ({1}): {2}", columnNames[j], j, hqs1.HqData[i][j]);
                }
            }
        }
    }

    public class Post_fb9efac5_8b57_417e_9f71_35d48d421eb4
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            [WebGet]
            Stream DownloadFile(string fileName);
            [OperationContract]
            [WebInvoke(UriTemplate = "/UploadFile/{fileName}")]
            void UploadFile(string fileName, Stream fileContents);
        }

        static long CountBytes(Stream stream)
        {
            byte[] buffer = new byte[100000];
            int bytesRead;
            long totalBytesRead = 0;
            do
            {
                bytesRead = stream.Read(buffer, 0, buffer.Length);
                totalBytesRead += bytesRead;
            } while (bytesRead > 0);
            return totalBytesRead;
        }

        class MyReadonlyStream : Stream
        {
            long length;
            long leftToRead;
            public MyReadonlyStream(long length)
            {
                this.length = length;
                this.leftToRead = length;
            }

            public override bool CanRead
            {
                get { return true; }
            }

            public override bool CanSeek
            {
                get { return false; }
            }

            public override bool CanWrite
            {
                get { return false; }
            }

            public override void Flush()
            {
            }

            public override long Length
            {
                get { return this.length; }
            }

            public override long Position
            {
                get { throw new NotSupportedException(); }
                set { throw new NotSupportedException(); }
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                int toReturn = (int)Math.Min(this.leftToRead, (long)count);
                this.leftToRead -= toReturn;
                return toReturn;
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new NotSupportedException();
            }

            public override void SetLength(long value)
            {
                throw new NotSupportedException();
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                throw new NotSupportedException();
            }
        }

        public class Service : ITest
        {
            public Stream DownloadFile(string fileName)
            {
                WebOperationContext.Current.OutgoingResponse.Headers["Content-Disposition"] = "attachment; filename=" + fileName;
                return new MyReadonlyStream(200000000); //200MB
            }

            public void UploadFile(string fileName, Stream fileContents)
            {
                long totalBytesRead = CountBytes(fileContents);
                Console.WriteLine("Total bytes read for file {0}: {1}", fileName, totalBytesRead);
            }
        }

        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            WebHttpBinding binding = new WebHttpBinding
            {
                TransferMode = TransferMode.Streamed,
                MaxReceivedMessageSize = int.MaxValue,
            };
            binding.ReaderQuotas.MaxArrayLength = int.MaxValue;
            host.AddServiceEndpoint(typeof(ITest), binding, "").Behaviors.Add(new WebHttpBehavior());
            host.Open();
            Console.WriteLine("Host opened");

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(baseAddress + "/DownloadFile?fileName=test.txt");
            req.Method = "GET";
            HttpWebResponse resp;
            try
            {
                resp = (HttpWebResponse)req.GetResponse();
            }
            catch (WebException e)
            {
                resp = (HttpWebResponse)e.Response;
            }

            Console.WriteLine("HTTP/{0} {1} {2}", resp.ProtocolVersion, (int)resp.StatusCode, resp.StatusDescription);
            foreach (string header in resp.Headers.AllKeys)
            {
                Console.WriteLine("{0}: {1}", header, resp.Headers[header]);
            }

            Stream respStream = resp.GetResponseStream();
            long size = CountBytes(respStream);
            Console.WriteLine("Response size: {0}", size);

            req = (HttpWebRequest)HttpWebRequest.Create(baseAddress + "/UploadFile/test.txt");
            req.Method = "POST";
            req.SendChunked = true;
            req.AllowWriteStreamBuffering = false;
            req.ContentType = "application/octet-stream";
            Stream reqStream = req.GetRequestStream();
            byte[] buffer = new byte[10000000];
            long bytesWritten = 0;
            for (int i = 0; i < 50; i++)
            {
                reqStream.Write(buffer, 0, buffer.Length);
                bytesWritten += buffer.Length;
                if ((i % 10) == 0)
                {
                    Console.WriteLine("Wrote {0} bytes", bytesWritten);
                }
            }
            reqStream.Close();
            resp = (HttpWebResponse)req.GetResponse();
            Console.WriteLine(resp.StatusCode);
        }
    }

    public class Post_003e95ed_88e4_4a41_b797_ed8bf730f81c
    {
        [DataContract(Name = "BaseDto", Namespace = "")]
        public class BaseDto
        {
            [DataMember]
            public string Str { set; get; }
        }

        //Option 1 - this does not work
        [DataContract(IsReference = true, Name = "NodeTypeDto", Namespace = "")]
        public class NodeTypeDto : BaseDto
        {
            [DataMember]
            public string Str2 { set; get; }
        }

        //Option 2
        [DataContract(IsReference = true, Name = "NodeTypeDto2", Namespace = "")]
        public class NodeTypeDto2
        {
            BaseDto baseDto = new BaseDto();

            [DataMember]
            public string Str
            {
                get { return this.baseDto.Str; }
                set { this.baseDto.Str = value; }
            }

            [DataMember]
            public string Str2 { set; get; }
        }

        //Option 3
        [DataContract(IsReference = true, Name = "NodeTypeDto3", Namespace = "")]
        public class NodeTypeDto3
        {
            [DataMember]
            public BaseDto BaseDto { get; set; }

            [DataMember]
            public string Str2 { set; get; }
        }

        static void Serialize(object toSerialize)
        {
            try
            {
                DataContractSerializer dcs = new DataContractSerializer(toSerialize.GetType());
                MemoryStream ms = new MemoryStream();
                XmlWriterSettings ws = new XmlWriterSettings
                {
                    Indent = true,
                    IndentChars = "  ",
                    OmitXmlDeclaration = true,
                    Encoding = new UTF8Encoding(false),
                };
                XmlWriter w = XmlWriter.Create(ms, ws);
                dcs.WriteObject(w, toSerialize);
                w.Flush();
                Console.WriteLine(Encoding.UTF8.GetString(ms.ToArray()));
            }
            catch (Exception e)
            {
                Console.WriteLine("Error serializing {0} - {1}: {2}", toSerialize.GetType().Name, e.GetType().FullName, e.Message);
            }
        }

        public static void Test()
        {
            NodeTypeDto first = new NodeTypeDto { Str = "first", Str2 = "first2" };
            NodeTypeDto[] firstOption = new NodeTypeDto[] { first, first, first };

            NodeTypeDto2 second = new NodeTypeDto2 { Str = "second", Str2 = "second2" };
            NodeTypeDto2[] secondOption = new NodeTypeDto2[] { second, second, second };

            NodeTypeDto3 third = new NodeTypeDto3
            {
                BaseDto = new BaseDto { Str = "third" },
                Str2 = "third2"
            };
            NodeTypeDto3[] thirdOption = new NodeTypeDto3[] { third, third, third };

            Serialize(firstOption);
            Serialize(secondOption);
            Serialize(thirdOption);
        }
    }

    public class Post_09b620e0_ea81_4a6c_8a10_02a032ccd821
    {
        static readonly string BaseAddress = "http://" + Environment.MachineName + ":8000/Service";
        static IChannelListener<IReplyChannel> listener;
        static void StartService()
        {
            WebHttpBinding binding = new WebHttpBinding();
            listener = binding.BuildChannelListener<IReplyChannel>(new Uri(BaseAddress), new BindingParameterCollection());
            listener.Open();
            listener.BeginAcceptChannel(new AsyncCallback(ChannelAccepted), listener);
            Console.WriteLine("Started listening...");
        }

        static void ChannelAccepted(IAsyncResult asyncResult)
        {
            IChannelListener<IReplyChannel> listener = (IChannelListener<IReplyChannel>)asyncResult.AsyncState;
            IReplyChannel replyChannel = listener.EndAcceptChannel(asyncResult);
            Console.WriteLine("Channel accepted, listening for messages");
            replyChannel.Open();
            RequestContext request = replyChannel.ReceiveRequest();
            Message incomingMessage = request.RequestMessage;
            Console.WriteLine("Incoming message: {0}", incomingMessage);
            Message reply = Message.CreateMessage(incomingMessage.Version, "http://the.reply.action", "Hello world");
            request.Reply(reply);
            incomingMessage.Close();
            request.Close();
            replyChannel.Close();

            listener.BeginAcceptChannel(new AsyncCallback(ChannelAccepted), null); // wait for next message
        }

        public static void Test()
        {
            StartService();

            Util.SendRequest(BaseAddress + "/Foo", "POST", "text/xml", "<string>hello world</string>");

            Console.WriteLine("Press ENTER to continue");
            Console.ReadLine();
        }
    }

    public class TestDynamic
    {
        public class JsonValue : DynamicObject
        {
            public override IEnumerable<string> GetDynamicMemberNames()
            {
                return "first second".Split();
            }

            public override bool TryConvert(ConvertBinder binder, out object result)
            {
                // Only explicit conversions
                if (binder.Explicit)
                {
                    switch (Type.GetTypeCode(binder.Type))
                    {
                        default:
                            break;
                    }
                }
                Console.WriteLine("TryConvert, binder.Explicit={0}", binder.Explicit);
                Console.WriteLine("TryConvert, binder.ReturnType={0}, .Type={1}", binder.ReturnType.FullName, binder.Type.FullName);
                if (binder.Type == typeof(int))
                {
                    result = 1;
                    return true;
                }
                else if (binder.Type == typeof(string))
                {
                    result = "hello";
                    return true;
                }
                return base.TryConvert(binder, out result);
            }

            public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
            {
                Console.WriteLine("In {0}, binder.{1} = {2}", MethodBase.GetCurrentMethod().Name, "ReturnType", binder.ReturnType);
                Console.WriteLine("In {0}, indexes={1}", MethodBase.GetCurrentMethod().Name, ArrayToString(indexes));
                result = null;
                if (indexes.Length == 1)
                {
                    if (indexes[0] is int && (int)indexes[0] == 0)
                    {
                        result = "hello";
                        return true;
                    }
                }

                return false;
            }

            private string ArrayToString(object[] indexes)
            {
                StringBuilder sb = new StringBuilder();
                if (indexes == null) return "<<null>>";
                sb.Append('[');
                for (int i = 0; i < indexes.Length; i++)
                {
                    if (i > 0) sb.Append(',');
                    sb.Append(indexes[i]);
                }
                sb.Append(']');
                return sb.ToString();
            }

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                Console.WriteLine("In {0}, binder.{1} = {2}", MethodBase.GetCurrentMethod().Name, "Name", binder.Name);
                Console.WriteLine("In {0}, binder.{1} = {2}", MethodBase.GetCurrentMethod().Name, "ReturnType", binder.ReturnType);
                result = null;
                if (binder.Name == "foo")
                {
                    result = 1;
                    return true;
                }
                return false;
            }
        }

        public static void Test()
        {
            JsonValue jv = new JsonValue();
            dynamic dyn = jv;
            string s = (string)dyn;
            int i = dyn;
            int a = dyn.foo;
            Console.WriteLine(a);
            s = dyn[0];
            Console.WriteLine(s);
        }
    }

    public class TestXElementWithJson
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract, WebGet(ResponseFormat = WebMessageFormat.Json)]
            XElement GetData();
        }
        public class Service : ITest
        {
            public XElement GetData()
            {
                return XElement.Parse(@"<root xmlns='http://tempuri.org/'>
  <person name='John Doe'>
    <a:address xmlns:a='http://address.namespace'>
      <a:number>1</a:number>
      <a:street>Microsoft Way</a:street>
      <a:city>Redmond</a:city>
      <a:zip>98052</a:zip>
    </a:address>
  </person>
</root>");
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            WebServiceHost host = new WebServiceHost(typeof(Service), new Uri(baseAddress));
            host.Open();
            Util.SendRequest(baseAddress + "/GetData", "GET", null, null);
        }
    }

    public class XmlToJson
    {
        public static void Test()
        {
            string xml = @"<root xmlns='http://tempuri.org/'>
  <person name='John Doe' age='23'>
    <a:address xmlns:a='http://address.namespace'>
      <a:number>1</a:number>
      <a:street>Microsoft Way</a:street>
      <a:city>Redmond</a:city>
      <a:zip>98052</a:zip>
    </a:address>
  </person>
</root>";
            string input = "input.xml";
            File.WriteAllText(input, xml);
            string output = "output.json";
            XslCompiledTransform transform = new XslCompiledTransform();
            transform.Load("http://www.xml.lt/static/files/xml2json.xsl");
            transform.Transform(input, output);
            string json = File.ReadAllText(output);
            Console.WriteLine(json);
        }
    }

    public class Post_79494d67_8886_4c8d_a656_76df3f462815
    {
        [DataContract]
        public class MyDC
        {
            [DataMember]
            public int data;
            [DataMember]
            public string str;
        }
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            long SumData(List<MyDC> list);
        }
        public class Service : ITest
        {
            public long SumData(List<MyDC> list)
            {
                if (OperationContext.Current.IncomingMessageProperties.ContainsKey(HttpRequestMessageProperty.Name))
                {
                    HttpRequestMessageProperty prop = (HttpRequestMessageProperty)OperationContext.Current.IncomingMessageProperties[HttpRequestMessageProperty.Name];
                    Console.WriteLine("Request length: {0}", prop.Headers[HttpRequestHeader.ContentLength]);
                }
                long result = 0;
                foreach (MyDC dc in list)
                {
                    result += dc.data;
                }
                return result;
            }
        }
        static Binding GetBinding()
        {
            BasicHttpBinding result = new BasicHttpBinding();
            result.MaxReceivedMessageSize = int.MaxValue;
            result.ReaderQuotas.MaxArrayLength = int.MaxValue;
            result.ReaderQuotas.MaxNameTableCharCount = int.MaxValue;
            return result;
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            ServiceEndpoint endpoint = host.AddServiceEndpoint(typeof(ITest), GetBinding(), "");
            foreach (OperationDescription od in endpoint.Contract.Operations)
            {
                DataContractSerializerOperationBehavior dcsob = od.Behaviors.Find<DataContractSerializerOperationBehavior>();
                dcsob.MaxItemsInObjectGraph = int.MaxValue;
            }
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(GetBinding(), new EndpointAddress(baseAddress));
            foreach (OperationDescription od in factory.Endpoint.Contract.Operations)
            {
                DataContractSerializerOperationBehavior dcsob = od.Behaviors.Find<DataContractSerializerOperationBehavior>();
                dcsob.MaxItemsInObjectGraph = int.MaxValue;
            }
            ITest proxy = factory.CreateChannel();

            Console.WriteLine("Creating the input");
            List<MyDC> input = new List<MyDC>();
            for (int i = 0; i < 500000; i++)
            {
                input.Add(new MyDC { data = i, str = "hello" });
            }
            Console.WriteLine("Finished creating the input with {0} elements.", input.Count);
            long expectedResult = input.Count;
            expectedResult = expectedResult * (expectedResult - 1) / 2;
            Console.WriteLine("Expected result: {0}", expectedResult);
            Console.WriteLine("Result: {0}", proxy.SumData(input));

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_d1c9bbc5_fc92_451a_8a54_283270b2c2d9
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            void Process(string text);
            [OperationContract(IsOneWay = true)]
            void ProcessOneWay(string text);
        }
        public class Service : ITest
        {
            public void Process(string text)
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.Accepted;
                Console.WriteLine("In Process({0})", text);
            }

            public void ProcessOneWay(string text)
            {
                Console.WriteLine("In ProcessOneWay({0})", text);
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            WebServiceHost host = new WebServiceHost(typeof(Service), new Uri(baseAddress));
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(new WebHttpBinding(), new EndpointAddress(baseAddress));
            factory.Endpoint.Behaviors.Add(new WebHttpBehavior());
            ITest proxy = factory.CreateChannel();

            proxy.Process("Hello");
            proxy.ProcessOneWay("World");

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_1115c3e2_3341_40da_b4ad_4bad54ab313e
    {
        [MessageContract]
        public class AddRequest
        {
            [MessageBodyMember]
            public int x;
            [MessageBodyMember]
            public int y;
        }
        [MessageContract]
        public class AddResponse
        {
            [MessageBodyMember]
            public int AddResult;
        }
        [MessageContract]
        public class AddFault
        {
            [MessageBodyMember]
            public string Message;
        }
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            int AddNormal(int x, int y);
            [OperationContract]
            [FaultContract(typeof(AddFault))]
            AddResponse AddMC(AddRequest request);
        }
        [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
        public class Service : ITest
        {
            public int AddNormal(int x, int y)
            {
                return x + y;
            }
            public AddResponse AddMC(AddRequest request)
            {
                return new AddResponse { AddResult = request.x + request.y };
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), new BasicHttpBinding(), "");
            host.Description.Behaviors.Add(new ServiceMetadataBehavior { HttpGetEnabled = true });
            host.Open();

            Console.WriteLine("Host opened");

            XmlReader r = XmlReader.Create(baseAddress + "?wsdl");
            XmlWriterSettings ws = new XmlWriterSettings();
            ws.Indent = true;
            ws.IndentChars = " ";
            ws.OmitXmlDeclaration = true;
            ws.Encoding = Encoding.UTF8;
            MemoryStream ms = new MemoryStream();
            XmlWriter w = XmlWriter.Create(ms, ws);
            w.WriteNode(r, false);
            w.Flush();
            Console.WriteLine(Encoding.UTF8.GetString(ms.ToArray()));
        }
    }

    public class Post_672de200_0613_4040_93e9_2a59fe56308f
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
            WSDualHttpBinding result = new WSDualHttpBinding(WSDualHttpSecurityMode.None);
            result.ReliableSession.Ordered = false;
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

            Console.WriteLine(proxy.Add(3, 5));

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_31301dd8_3a3b_403f_bd1c_85a51f643757
    {
        [DataContract]
        public class MailAddressSurrogated
        {
            [DataMember]
            public string Address;
            [DataMember]
            public string DisplayName;
        }
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            string GetEMail(MailAddress mailAddress);
        }
        [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
        public class Service : ITest
        {
            public string GetEMail(MailAddress mailAddress)
            {
                return mailAddress.Address;
            }
        }
        class MailSurrogate : IDataContractSurrogate
        {
            public object GetCustomDataToExport(Type clrType, Type dataContractType)
            {
                return null;
            }

            public object GetCustomDataToExport(MemberInfo memberInfo, Type dataContractType)
            {
                return null;
            }

            public Type GetDataContractType(Type type)
            {
                if (type == typeof(MailAddress))
                {
                    return typeof(MailAddressSurrogated);
                }
                return type;
            }

            public object GetDeserializedObject(object obj, Type targetType)
            {
                if (obj is MailAddressSurrogated)
                {
                    MailAddressSurrogated surrogate = (MailAddressSurrogated)obj;
                    MailAddress address = new MailAddress(surrogate.Address, surrogate.DisplayName);
                    return address;
                }
                return obj;
            }

            public void GetKnownCustomDataTypes(Collection<Type> customDataTypes)
            {
            }

            public object GetObjectToSerialize(object obj, Type targetType)
            {
                if (obj is MailAddress)
                {
                    MailAddress address = (MailAddress)obj;
                    MailAddressSurrogated surrogate = new MailAddressSurrogated
                    {
                        Address = address.Address,
                        DisplayName = address.DisplayName,
                    };
                    return surrogate;
                }
                return obj;
            }

            public Type GetReferencedTypeOnImport(string typeName, string typeNamespace, object customData)
            {
                return null;
            }

            public System.CodeDom.CodeTypeDeclaration ProcessImportedType(System.CodeDom.CodeTypeDeclaration typeDeclaration, System.CodeDom.CodeCompileUnit compileUnit)
            {
                return typeDeclaration;
            }
        }
        static Binding GetBinding()
        {
            BasicHttpBinding result = new BasicHttpBinding();
            return result;
        }
        static void UpdateDCSOB(ServiceEndpoint endpoint)
        {
            foreach (OperationDescription od in endpoint.Contract.Operations)
            {
                DataContractSerializerOperationBehavior dcsob = od.Behaviors.Find<DataContractSerializerOperationBehavior>();
                if (dcsob != null)
                {
                    dcsob.DataContractSurrogate = new MailSurrogate();
                }
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            ServiceEndpoint endpoint = host.AddServiceEndpoint(typeof(ITest), GetBinding(), "");
            UpdateDCSOB(endpoint);
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(GetBinding(), new EndpointAddress(baseAddress));
            UpdateDCSOB(factory.Endpoint);
            ITest proxy = factory.CreateChannel();

            MailAddress address = new MailAddress("a@b.com", "Mr A B", Encoding.UTF8);
            Console.WriteLine(proxy.GetEMail(address));

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class JsonpTest
    {
        [ServiceContract]
        public interface ITest
        {
            [WebGet(ResponseFormat = WebMessageFormat.Json)]
            int Add(int x, int y);
        }
        public class Service : ITest
        {
            public int Add(int x, int y) { return x + y; }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            WebHttpBinding binding = new WebHttpBinding();
            binding.CrossDomainScriptAccessEnabled = true;
            ServiceEndpoint endpoint = host.AddServiceEndpoint(typeof(ITest), binding, "");
            WebHttpBehavior behavior = new WebHttpBehavior();
            behavior.HelpEnabled = true;
            endpoint.Behaviors.Add(behavior);
            host.Open();

            Console.WriteLine("Host opened");

            Util.SendRequest(baseAddress + "/help", "GET", null, null);
            Util.SendRequest(baseAddress + "/Add?x=4&y=6", "GET", null, null);
            Util.SendRequest(baseAddress + "/Add?x=4&y=6&callback=myFunc", "GET", null, null);

            Console.WriteLine("Press ENTER to close");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_3612be9c_c501_442b_a174_0c069639eac1
    {
        [DataContract(Name = "MyDC", Namespace = "http://my.namespace.com")]
        public class MyDC
        {
            [DataMember]
            public string str = "The string";
        }
        [ServiceContract(Name = "ITest", Namespace = "http://my.namespace.com")]
        public interface ITest
        {
            [OperationContract]
            int Add(int x, int y);
            [OperationContract]
            MyDC EchoDC(MyDC input);
        }
        [ServiceContract(Name = "ITest", Namespace = "http://my.namespace.com")]
        public interface IUntypedTest
        {
            [OperationContract(Action = "*", ReplyAction = "*")]
            Message Process(Message input);
        }
        public class Service : ITest
        {
            public int Add(int x, int y)
            {
                return x + y;
            }

            public MyDC EchoDC(MyDC input)
            {
                return input;
            }
        }
        static Binding GetBinding()
        {
            CustomBinding result = new CustomBinding(new TextMessageEncodingBindingElement(), new HttpTransportBindingElement());
            return result;
        }
        static Binding GetTcpBinding()
        {
            return new NetTcpBinding();
        }
        static void SendMessagesUsingHttp(string baseAddress)
        {
            // easy way to see what the message should look like (look at it on Fiddler)
            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(GetBinding(), new EndpointAddress(baseAddress));
            ITest proxy = factory.CreateChannel();

            Console.WriteLine(proxy.EchoDC(new MyDC()));
            Console.WriteLine(proxy.Add(3, 5));

            ((IClientChannel)proxy).Close();
            factory.Close();
        }
        static Message CreateInput(string endpointAddress, string action)
        {
            string xml;
            if (action == "http://my.namespace.com/ITest/Add")
            {
                // add operation
                xml = @"<Add xmlns=""http://my.namespace.com"">
  <x>3</x>
  <y>5</y>
</Add>";
            }
            else
            {
                xml = @"<EchoDC xmlns=""http://my.namespace.com"">
  <input xmlns:b=""http://my.namespace.com"">
    <b:str>The string</b:str>
  </input>
</EchoDC>";
            }
            Message message = Message.CreateMessage(MessageVersion.Soap12WSAddressing10, action, new MyBodyWriter(xml));
            message.Headers.To = new Uri(endpointAddress);
            message.Headers.MessageId = new UniqueId();
            message.Headers.ReplyTo = new EndpointAddress("http://www.w3.org/2005/08/addressing/anonymous");
            return message;
        }
        class MyBodyWriter : BodyWriter
        {
            string xml;
            public MyBodyWriter(string xml)
                : base(true)
            {
                this.xml = xml;
            }

            protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
            {
                XmlReader r = XmlReader.Create(new MemoryStream(Encoding.UTF8.GetBytes(this.xml)));
                writer.WriteNode(r, false);
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            string baseAddressTcp = "net.tcp://" + Environment.MachineName + ":8888/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress), new Uri(baseAddressTcp));
            host.AddServiceEndpoint(typeof(ITest), GetBinding(), "");
            host.AddServiceEndpoint(typeof(ITest), GetTcpBinding(), "");
            host.Open();
            Console.WriteLine("Host opened");

            SendMessagesUsingHttp(baseAddress);

            Message input = CreateInput(baseAddressTcp, "http://my.namespace.com/ITest/Add");
            Console.WriteLine(input);
            ChannelFactory<IUntypedTest> factory = new ChannelFactory<IUntypedTest>(GetTcpBinding(), new EndpointAddress(baseAddressTcp));
            IUntypedTest proxy = factory.CreateChannel();
            Message result = proxy.Process(input);
            Console.WriteLine(result);

            Console.WriteLine();

            input = CreateInput(baseAddressTcp, "http://my.namespace.com/ITest/EchoDC");
            Console.WriteLine(input);
            result = proxy.Process(input);
            Console.WriteLine(result);

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_26f6a581_fcca_4f3a_a151_bdc959385db9
    {
        [ServiceContract]
        public interface IaMailServerWCF
        {
            [OperationContract]
            byte[] getImage();
        }
        public class aMail_Server : IaMailServerWCF
        {
            public byte[] getImage()
            {
                return new byte[100000];
            }
        }

        static ServiceHost host;
        static string baseAddress = "net.pipe://localhost";
        static void StartServer()
        {
            host = new ServiceHost(typeof(aMail_Server), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(IaMailServerWCF), new NetNamedPipeBinding(), "PipeReverse");
            host.Open();
            Console.WriteLine("Host opened");
        }
        static void StopServer()
        {
            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
        public static void Test()
        {
            StartServer();

            NetNamedPipeBinding binding = new NetNamedPipeBinding();
            binding.MaxReceivedMessageSize = 1000000;
            binding.ReaderQuotas.MaxArrayLength = 1000000;
            ChannelFactory<IaMailServerWCF> factory = new ChannelFactory<IaMailServerWCF>(binding, new EndpointAddress(baseAddress + "/PipeReverse"));
            IaMailServerWCF proxy = factory.CreateChannel();

            byte[] image = proxy.getImage();
            Console.WriteLine(image.Length);

            ((IClientChannel)proxy).Close();
            factory.Close();

            StopServer();
        }
    }

    public class Post_c24e3957_5440_4fd3_a2c7_97ef7c039d16
    {
        [ServiceContract(Namespace = "")]
        public interface ITest
        {
            [OperationContract]
            [WebInvoke(Method = "PUT", UriTemplate = "{clientId}/NetFriends", RequestFormat = WebMessageFormat.Xml, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
            [Description("Adds NetFriends to the user's account.")]
            string AddNetFriends(NetFriend[] _friends, string clientId, int applicationId);
        }

        [DataContract(Name = "NetFriend", Namespace = "http://my.namespace.com")]
        public class NetFriend
        {
            [DataMember(Order = 1)]
            public string Name { get; set; }
            [DataMember(Order = 2)]
            public string Image { get; set; }
            [DataMember(Order = 3)]
            public int FriendType { get; set; }
        }

        public class Service : ITest
        {
            public string AddNetFriends(NetFriend[] _friends, string clientId, int applicationId)
            {
                return _friends == null ? "<<null>>" : String.Format("_friends has {0} members", _friends.Length);
            }
        }

        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            WebServiceHost host = new WebServiceHost(typeof(Service), new Uri(baseAddress));
            host.Open();
            Console.WriteLine("Host opened");

            WebChannelFactory<ITest> factory = new WebChannelFactory<ITest>(new Uri(baseAddress));
            ITest proxy = factory.CreateChannel();
            NetFriend[] friends = new NetFriend[] { new NetFriend { FriendType = 1, Name = "Naam" } };

            // the line below will show what a "correct" request should look like
            Console.WriteLine(proxy.AddNetFriends(friends, "clientId", 1000));

            ((IClientChannel)proxy).Close();
            factory.Close();

            string goodRequest = @"<AddNetFriends>
  <_friends xmlns:a=""http://my.namespace.com"">
    <a:NetFriend>
      <a:Name>Naam</a:Name>
      <a:FriendType>1</a:FriendType>
    </a:NetFriend>
  </_friends>
  <applicationId>1000</applicationId>
</AddNetFriends>";

            string badRequest = @"<AddNetFriends>
  <_friends xmlns:a=""http://other.namespace.com"">
    <a:NetFriend>
      <a:Name>Naam</a:Name>
      <a:FriendType>1</a:FriendType>
    </a:NetFriend>
  </_friends>
  <applicationId>1000</applicationId>
</AddNetFriends>";

            Util.SendRequest(baseAddress + "/clientId/NetFriends", "PUT", "text/xml", goodRequest);
            Util.SendRequest(baseAddress + "/clientId/NetFriends", "PUT", "text/xml", badRequest);

            Console.WriteLine("Press ENTER to close host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_9785855d_7377_46cc_ae44_f361bcdb5b08
    {
        static readonly string soapAddress = "http://" + Environment.MachineName + ":8000/SOAP";
        static readonly string restAddress = "http://" + Environment.MachineName + ":8008/REST";

        [ServiceContract]
        public interface ITestSoap
        {
            [XmlSerializerFormat]
            [OperationContract]
            string getit(string version);
        }
        [ServiceContract]
        public interface ITestRest
        {
            [XmlSerializerFormat]
            [OperationContract]
            [WebGet(BodyStyle = WebMessageBodyStyle.Bare,
                    ResponseFormat = WebMessageFormat.Xml,
                    UriTemplate = "?Service=wfs&Request=GetCapabilities&Version={version}")]
            string getit(string version);
        }
        [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
        public class SoapService : ITestSoap
        {
            public string getit(string version)
            {
                Console.WriteLine("Inside SoapService.getit");
                WebChannelFactory<ITestRest> factory = new WebChannelFactory<ITestRest>(new Uri(restAddress));
                ITestRest proxy = factory.CreateChannel();
                string result;
                using (new OperationContextScope((IContextChannel)proxy))
                {
                    result = proxy.getit(version) + " from SOAP";
                }
                ((IClientChannel)proxy).Close();
                factory.Close();
                return result;
            }
        }
        public class RestService : ITestRest
        {
            public string getit(string version)
            {
                Console.WriteLine("Inside RestService.getit");
                return version + " from REST";
            }
        }

        public static void Test()
        {
            ServiceHost soapHost = new ServiceHost(typeof(SoapService), new Uri(soapAddress));
            ServiceHost restHost = new ServiceHost(typeof(RestService), new Uri(restAddress));
            soapHost.AddServiceEndpoint(typeof(ITestSoap), new BasicHttpBinding(), "");
            restHost.AddServiceEndpoint(typeof(ITestRest), new WebHttpBinding(), "").Behaviors.Add(new WebHttpBehavior());
            soapHost.Open();
            restHost.Open();
            Console.WriteLine("Hosts opened");

            ChannelFactory<ITestSoap> factory = new ChannelFactory<ITestSoap>(new BasicHttpBinding(), new EndpointAddress(soapAddress));
            ITestSoap proxy = factory.CreateChannel();

            Console.WriteLine(proxy.getit("hello"));

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            soapHost.Close();
            restHost.Close();
        }
    }

    public class Post_ddda0542_2c44_4f65_9ffe_6820df739394
    {
        [DataContract]
        public class MyBody
        {
            [DataMember]
            public string str = "The string";
            [DataMember]
            public double dbl = 123.456;
        }
        [XmlSerializerFormat]
        public class MyHeader
        {
            [XmlAttribute]
            public string strAttr = "hello";
            [XmlAttribute]
            public string strAttr2 = "world";
        }
        [MessageContract]
        public class MyMC
        {
            [MessageHeader]
            public MyHeader header;
            [MessageBodyMember]
            public MyBody body;
        }
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            MyMC EchoMC(MyMC mc);
        }
        public class Service : ITest
        {
            public MyMC EchoMC(MyMC mc)
            {
                return mc;
            }
        }
        static Binding GetBinding()
        {
            BasicHttpBinding result = new BasicHttpBinding();
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

            Console.WriteLine(proxy.EchoMC(new MyMC { body = new MyBody(), header = new MyHeader() }));

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_51d14a1e_607c_474b_b949_47f3f8706356
    {
        [Serializable]
        public class MyParameter : IXmlSerializable
        {
            public string Name { get; set; }
            public string Value { get; set; }
            public string Mytype { get; set; }

            #region IXmlSerializable Members

            public System.Xml.Schema.XmlSchema GetSchema()
            {
                return null;
            }

            public void ReadXml(System.Xml.XmlReader reader)
            {
                XElement e = XElement.Parse(reader.ReadOuterXml());
                IEnumerable<XElement> i = e.Elements();
                List<XElement> l = new List<XElement>(i);
                Name = l[0].Name.ToString();
                Value = l[0].Value.ToString();
                Mytype = l[0].Attribute(XName.Get("type", "http://www.w3.org/2001/XMLSchema-instance")).Value.ToString();
            }

            public void WriteXml(System.Xml.XmlWriter writer)
            {
                writer.WriteStartElement(Name);
                writer.WriteAttributeString("type", "http://www.w3.org/2001/XMLSchema-instance", Mytype);
                writer.WriteValue(Value);
                writer.WriteEndElement();
            }

            #endregion
        }

        [ServiceContract]
        public interface IOperation
        {
            [OperationContract]
            void Operation(List<Data> list);
        }

        [DataContract(Name = "Data", Namespace = "MyNS")]
        public class Data
        {
            [DataMember]
            public string Name { get; set; }
            [DataMember]
            public List<MyParameter> Parameters { get; set; }
        }

        [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
        public class Service : IOperation
        {
            public void Operation(List<Data> list)
            {
                if (list == null)
                {
                    Console.WriteLine("list is null");
                }
                else
                {
                    foreach (Data d in list)
                    {
                        Console.WriteLine("Data: Name={0}", d.Name);
                        if (d.Parameters == null)
                        {
                            Console.WriteLine("   Parameter is null");
                        }
                        else
                        {
                            foreach (MyParameter myp in d.Parameters)
                            {
                                Console.WriteLine("  Parameter: Name={0},MyType={1},Value={2}", myp.Name, myp.Mytype, myp.Value);
                            }
                        }
                    }
                }
            }
        }

        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(IOperation), new BasicHttpBinding(), "");
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<IOperation> factory = new ChannelFactory<IOperation>(new BasicHttpBinding(), new EndpointAddress(baseAddress));
            IOperation proxy = factory.CreateChannel();

            List<Data> input = new List<Data>
            {
                new Data { Name = "Data 1", Parameters = new List<MyParameter>
                {
                    new MyParameter { Name = "D1P1", Value = "V1P1", Mytype = "string" },
                    new MyParameter { Name = "D1P2", Value = "1234", Mytype = "int" },
                }},
                new Data { Name = "Data 2", Parameters = new List<MyParameter>
                {
                    new MyParameter { Name = "D2P1", Value = "V2P1", Mytype = "string" },
                }},
            };

            proxy.Operation(input);

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_97ddb118_fdfd_4651_9e61_4d822861325f
    {
        [ServiceContract]
        public interface IService
        {
            [Description("Simple echo operation over HTTP GET. The response is returned in XML or JSON based on the Accept header and on the format query string parameter.")]
            [WebGet]
            string EchoWithGet(string s);

            [Description("Simple echo operation over HTTP POST. The response is returned in the same format as the request.")]
            [WebInvoke]
            string EchoWithPost(string s);
        }

        [MyErrorHandler]
        public class Service : IService
        {
            public string EchoWithGet(string s)
            {
                // if a format query string parameter has been specified, set the response format to that. If no such
                // query string parameter exists the Accept header will be used
                string formatQueryStringValue = WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters["format"];
                if (!string.IsNullOrEmpty(formatQueryStringValue))
                {
                    if (formatQueryStringValue.Equals("xml", System.StringComparison.OrdinalIgnoreCase))
                    {
                        WebOperationContext.Current.OutgoingResponse.Format = WebMessageFormat.Xml;
                    }
                    else if (formatQueryStringValue.Equals("json", System.StringComparison.OrdinalIgnoreCase))
                    {
                        WebOperationContext.Current.OutgoingResponse.Format = WebMessageFormat.Json;
                    }
                    else
                    {
                        throw new WebFaultException<string>(string.Format("Unsupported format '{0}'", formatQueryStringValue), HttpStatusCode.BadRequest);
                    }
                }

                throw new Exception("Error expected type:" + WebOperationContext.Current.OutgoingResponse.Format.ToString());

                //return "You said " + s;
            }

            public string EchoWithPost(string s)
            {
                return "You said " + s;
            }
        }

        public class MyErrorHandler : Attribute, IErrorHandler, IServiceBehavior
        {
            public bool HandleError(Exception error)
            {
                return true;
            }

            public void ProvideFault(Exception error, MessageVersion version, ref System.ServiceModel.Channels.Message fault)
            {
                // check format
                var format = WebOperationContext.Current.OutgoingResponse.Format;
                bool isJson = format.HasValue && format.Value == WebMessageFormat.Json;

                MyFaultDetail faultDetails = new MyFaultDetail { Message = error.Message };
                fault = Message.CreateMessage(version, "http://mydefinedfault", new MyBodyWriter(faultDetails, isJson));

                HttpResponseMessageProperty prop = new HttpResponseMessageProperty();
                fault.Properties.Add(HttpResponseMessageProperty.Name, prop);
                prop.StatusCode = HttpStatusCode.InternalServerError;

                if (isJson)
                {
                    fault.Properties.Add(WebBodyFormatMessageProperty.Name, new WebBodyFormatMessageProperty(WebContentFormat.Json));
                    prop.Headers[HttpResponseHeader.ContentType] = "application/json";
                }
            }

            public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
            {
            }

            public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
            {
                // errorhandler
                foreach (ChannelDispatcher chanDisp in serviceHostBase.ChannelDispatchers)
                {
                    chanDisp.ErrorHandlers.Add(this);
                }
            }

            public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
            {
            }

            class MyBodyWriter : BodyWriter
            {
                MyFaultDetail faultDetails;
                XmlObjectSerializer serializer;
                public MyBodyWriter(MyFaultDetail faultDetails, bool isJson)
                    : base(true)
                {
                    this.faultDetails = faultDetails;
                    if (isJson)
                    {
                        this.serializer = new DataContractJsonSerializer(typeof(MyFaultDetail));
                    }
                    else
                    {
                        this.serializer = new DataContractSerializer(typeof(MyFaultDetail));
                    }
                }
                protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
                {
                    this.serializer.WriteObject(writer, this.faultDetails);
                }
            }
        }

        [DataContract(Name = "MyFaultDetail", Namespace = "")]
        public class MyFaultDetail
        {
            [DataMember]
            public string Message;
        }

        public static void Test()
        {
            Uri baseAddress = new Uri("http://" + Environment.MachineName + ":8000");
            Console.WriteLine("Service is hosted at: " + baseAddress.AbsoluteUri);
            Console.WriteLine("Service help page is at: " + baseAddress.AbsoluteUri + "help");

            using (ServiceHost host = new ServiceHost(typeof(Service), baseAddress))
            {
                WebHttpBehavior behavior = new WebHttpBehavior
                {
                    AutomaticFormatSelectionEnabled = true,
                    HelpEnabled = true,
                };
                host.AddServiceEndpoint(typeof(IService), new WebHttpBinding(), "").Behaviors.Add(behavior);
                host.Open();

                using (WebClient client = new WebClient())
                {
                    client.BaseAddress = baseAddress.AbsoluteUri;
                    client.QueryString["s"] = "hello";

                    // Specify response format for GET using Accept header
                    HttpWebResponse resp = null;
                    try
                    {
                        Console.WriteLine("Calling EchoWithGet via HTTP GET and Accept header application/xml: ");
                        client.Headers[HttpRequestHeader.Accept] = "application/xml";
                        Console.WriteLine(client.DownloadString("EchoWithGet"));
                    }
                    catch (WebException ex)
                    {
                        resp = (HttpWebResponse)ex.Response;
                        if (resp != null)
                        {
                            var reader = new StreamReader(resp.GetResponseStream());
                            Console.WriteLine(reader.ReadToEnd());
                        }
                    }

                    Console.WriteLine();

                    try
                    {
                        Console.WriteLine("Calling EchoWithGet via HTTP GET and Accept header application/json: ");
                        client.Headers[HttpRequestHeader.Accept] = "application/json";
                        Console.WriteLine(client.DownloadString("EchoWithGet"));
                    }
                    catch (WebException ex)
                    {
                        resp = (HttpWebResponse)ex.Response;
                        if (resp != null)
                        {
                            var reader = new StreamReader(resp.GetResponseStream());
                            Console.WriteLine(reader.ReadToEnd());
                        }
                    }

                    client.Headers[HttpRequestHeader.Accept] = null;

                    Console.WriteLine();

                    // Specify response format for GET using 'format' query string parameter
                    try
                    {
                        Console.WriteLine("Calling EchoWithGet via HTTP GET and format query string parameter set to xml: ");
                        client.QueryString["format"] = "xml";
                        Console.WriteLine(client.DownloadString("EchoWithGet"));
                    }
                    catch (WebException ex)
                    {
                        resp = (HttpWebResponse)ex.Response;
                        if (resp != null)
                        {
                            var reader = new StreamReader(resp.GetResponseStream());
                            Console.WriteLine(reader.ReadToEnd());
                        }
                    }

                    Console.WriteLine();

                    try
                    {
                        Console.WriteLine("Calling EchoWithGet via HTTP GET and format query string parameter set to json: ");
                        client.QueryString["format"] = "json";
                        Console.WriteLine(client.DownloadString("EchoWithGet"));
                    }
                    catch (WebException ex)
                    {
                        resp = (HttpWebResponse)ex.Response;
                        if (resp != null)
                        {
                            var reader = new StreamReader(resp.GetResponseStream());
                            Console.WriteLine(reader.ReadToEnd());
                        }
                    }

                    Console.WriteLine();

                    // Do POST in XML and JSON and get the response in the same format as the request
                    Console.WriteLine("Calling EchoWithPost via HTTP POST and request in XML format: ");
                    client.Headers[HttpRequestHeader.ContentType] = "application/xml";
                    Console.WriteLine(client.UploadString("EchoWithPost", "<string xmlns=\"http://schemas.microsoft.com/2003/10/Serialization/\">bye</string>"));

                    Console.WriteLine();

                    Console.WriteLine("Calling EchoWithPost via HTTP POST and request in JSON format: ");
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    Console.WriteLine(client.UploadString("EchoWithPost", "\"bye\""));

                    Console.WriteLine("Press any key to terminate");
                    Console.ReadLine();
                }
            }
        }
    }

    public class Post_c74d8a75_c32b_4801_86e4_096500539717
    {
        [ServiceContract]
        public interface IService1
        {
            [OperationContract]
            string GetData(int value);
            [OperationContract]
            Employee GetEmployee(string name);
            [OperationContract]
            Employee[] GetEmployees();
        }
        [DataContract]
        public class Employee
        {
            string name;
            public Employee(string n1)
            {
                name = n1;
            }
            [DataMember]
            public string Name
            {
                get { return name; }
                set { name = value; }
            }
        }
        public class Service1 : IService1
        {
            public string GetData(int value)
            {
                return string.Format("You entered: {0}", value);
            }
            public Employee GetEmployee(string name)
            {
                return new Employee("Hai " + name);
            }
            public Employee[] GetEmployees()
            {
                return new Employee[]
                {
                    new Employee("John Doe"),
                    new Employee("Jane Roe"),
                };
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service1), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(IService1), new BasicHttpBinding(), "");
            host.Open();

            ChannelFactory<IService1> factory = new ChannelFactory<IService1>(new BasicHttpBinding(), new EndpointAddress(baseAddress));
            IService1 proxy = factory.CreateChannel();

            Console.WriteLine(proxy.GetEmployees().Length);

            ((IClientChannel)proxy).Close();
            factory.Close();
            host.Close();
        }
    }

    public class Post_bc07fa75_84d7_4fd2_b56a_9192372aefb1
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
                object propValue;
                if (OperationContext.Current.IncomingMessageProperties.TryGetValue(MyInspector.MyPropertyName, out propValue))
                {
                    Console.WriteLine("AfterReceiveRequest set the following value: {0}", propValue);
                }

                OperationContext.Current.OutgoingMessageProperties.Add(MyInspector.MyPropertyName, "Set by the method to be used by the inspector");
                return x + y;
            }
        }
        static Binding GetBinding()
        {
            BasicHttpBinding result = new BasicHttpBinding();
            return result;
        }
        public class MyInspector : IDispatchMessageInspector, IEndpointBehavior
        {
            internal const string MyPropertyName = "ThePropertyName";
            public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
            {
            }

            public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
            {
            }

            public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
            {
                endpointDispatcher.DispatchRuntime.MessageInspectors.Add(this);
            }

            public void Validate(ServiceEndpoint endpoint)
            {
            }

            public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
            {
                request.Properties.Add(MyPropertyName, "This is available for the method");
                return null;
            }

            public void BeforeSendReply(ref Message reply, object correlationState)
            {
                object propValue;
                if (reply.Properties.TryGetValue(MyPropertyName, out propValue))
                {
                    Console.WriteLine("Method set this value: {0}", propValue);
                }
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            ServiceEndpoint endpoint = host.AddServiceEndpoint(typeof(ITest), GetBinding(), "");
            endpoint.Behaviors.Add(new MyInspector());
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(GetBinding(), new EndpointAddress(baseAddress));
            ITest proxy = factory.CreateChannel();

            Console.WriteLine(proxy.Add(3, 5));

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_c46ef367_f7b9_406b_8e52_bf1bf70c4d18
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            [WebInvoke(
             Method = "DELETE",
             ResponseFormat = WebMessageFormat.Json,
             BodyStyle = WebMessageBodyStyle.Bare,
             UriTemplate = "/myproject/persons")]
            void DeletePersons(PersonPool persons);
        }

        [DataContract]
        public class PersonPool
        {
            [DataMember(Name = "persons")]
            public Person[] persons { get; set; }
        }

        [DataContract]
        public class Person
        {
            [DataMember(Name = "fname")]
            public string fname { get; set; }
        }

        public class Service : ITest
        {
            public void DeletePersons(PersonPool persons)
            {
                Console.WriteLine("Persons to be deleted:");
                foreach (var person in persons.persons)
                {
                    Console.WriteLine(" {0}", person.fname);
                }

                WebOperationContext.Current.OutgoingResponse.Headers["X-MyAPP-Err0r-MSG"] = "My error";
            }
        }

        static string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
        static WebServiceHost host;
        static void StartService()
        {
            host = new WebServiceHost(typeof(Service), new Uri(baseAddress));
            host.Open();
            Console.WriteLine("Host opened");
        }

        static void MakeRequest()
        {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(baseAddress + "/myproject/persons");
            req.Method = "DELETE";
            req.ContentType = "text/json";
            byte[] jsonReq = Encoding.UTF8.GetBytes("{\"persons\": [{\"fname\":\"John\"}, {\"fname\":\"sheena\"}]}");
            req.GetRequestStream().Write(jsonReq, 0, jsonReq.Length);
            req.GetRequestStream().Close();
            HttpWebResponse resp;
            try
            {
                resp = (HttpWebResponse)req.GetResponse();
            }
            catch (WebException e)
            {
                resp = (HttpWebResponse)e.Response;
            }
            Console.WriteLine("HTTP/{0} {1} {2}", resp.ProtocolVersion, (int)resp.StatusCode, resp.StatusDescription);
            foreach (string headerName in resp.Headers.AllKeys)
            {
                Console.WriteLine("{0}: {1}", headerName, resp.Headers[headerName]);
            }
            Console.WriteLine();
            Stream respStream = resp.GetResponseStream();
            if (respStream != null)
            {
                Console.WriteLine(new StreamReader(respStream).ReadToEnd());
            }
            Console.WriteLine();
            Console.WriteLine(" *-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-* ");
            Console.WriteLine();
        }

        public static void Test()
        {
            StartService();
            MakeRequest();
        }
    }

    public class Post_da00e5d6_bdd9_4c32_8f36_ca2c60366aff
    {
        public class PocoPropertyData
        {
            public string N { get; set; }  // the property name
            public object V { get; set; }  // the property value

            static readonly DataContractJsonSerializer DateTimeDeserializer = new DataContractJsonSerializer(typeof(DateTime));
            [OnDeserialized]
            public void OnDeserialized(StreamingContext ctx)
            {
                string strV = V as string;
                if (strV != null)
                {
                    if (strV.StartsWith("/Date(") && strV.EndsWith(")/"))
                    {
                        string jsonDateTime = "\"\\" + strV.Substring(0, strV.Length - 1) + "\\/\""; // recreate the JSON object
                        V = (DateTime)DateTimeDeserializer.ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(jsonDateTime)));
                    }
                }
            }

            public override string ToString()
            {
                return string.Format("PocoPropertyData[N={0},V={1}]", N, V);
            }
        }
        public static void Test()
        {
            MemoryStream ms = new MemoryStream();
            PocoPropertyData poco = new PocoPropertyData { N = "DateTimeField", V = DateTime.Now };
            DataContractJsonSerializer dcjs = new DataContractJsonSerializer(typeof(PocoPropertyData));
            Console.WriteLine(poco);
            dcjs.WriteObject(ms, poco);
            Console.WriteLine(Encoding.UTF8.GetString(ms.ToArray()));
            ms.Position = 0;
            PocoPropertyData poco2 = (PocoPropertyData)dcjs.ReadObject(ms);
            Console.WriteLine(poco2);
        }
    }

    public class WebFaultExceptionTest
    {
        [ServiceContract]
        public interface ITest
        {
            [WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
            string Echo(string str);
        }
        public class Service : ITest
        {
            public string Echo(string str)
            {
                if (str == null)
                {
                    throw new WebFaultException(HttpStatusCode.Forbidden);
                }
                else
                {
                    throw new WebFaultException<string>(str, HttpStatusCode.ExpectationFailed);
                }
            }
        }
        public class MyBehavior : WebHttpBehavior
        {
            bool faultExceptionEnabled;
            public MyBehavior(bool faultExceptionEnabled)
            {
                this.faultExceptionEnabled = faultExceptionEnabled;
            }
            public override bool FaultExceptionEnabled
            {
                get
                {
                    return this.faultExceptionEnabled;
                }
                set
                {
                    this.faultExceptionEnabled = value;
                }
            }
            protected override void AddServerErrorHandlers(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
            {
                base.AddServerErrorHandlers(endpoint, endpointDispatcher);
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), new WebHttpBinding(), "").Behaviors.Add(new MyBehavior(false));
            host.Open();

            Util.SendRequest(baseAddress + "/Echo", "POST", "application/json", "\"hello\"");
            Util.SendRequest(baseAddress + "/Echo", "POST", "application/json", "");
        }
    }

    public class Post_e603865b_085b_4592_96da_d53ab65aa0ce
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            int Add(int x, int y);
        }
        [ServiceContract]
        public interface IHelpPageGenerator
        {
            [WebGet(UriTemplate = "")]
            Stream GetHelpPage();
        }
        public class Service : ITest, IHelpPageGenerator
        {
            public int Add(int x, int y) { return x + y; }
            public Stream GetHelpPage()
            {
                WebOperationContext.Current.OutgoingResponse.ContentType = "text/html";
                StringBuilder sb = new StringBuilder();
                sb.Append("<html><head><title>Service Help Page</title></head>");
                sb.Append("<body>");
                sb.Append("<h1>This is a custom help page, which can have anything you want</h1>");
                ServiceMetadataBehavior smb = OperationContext.Current.Host.Description.Behaviors.Find<ServiceMetadataBehavior>();
                if (smb == null || smb.HttpGetEnabled == false)
                {
                    sb.Append("<p>Service metadata is disabled...</p>");
                }
                else
                {
                    string metadataAddress = OperationContext.Current.Host.BaseAddresses[0].ToString() + "?wsdl";
                    sb.Append(string.Format("<p>You can create a client by pointing svcutil to <a href=\"{0}\">{0}</a></p>", metadataAddress));
                }
                sb.Append("</body></html>");
                return new MemoryStream(Encoding.UTF8.GetBytes(sb.ToString()));
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), new BasicHttpBinding(), "basic");
            ServiceEndpoint helpEndpoint = host.AddServiceEndpoint(typeof(IHelpPageGenerator), new WebHttpBinding(), "");
            helpEndpoint.Behaviors.Add(new WebHttpBehavior());

            ServiceDebugBehavior sdb = host.Description.Behaviors.Find<ServiceDebugBehavior>();
            if (sdb == null)
            {
                sdb = new ServiceDebugBehavior();
                host.Description.Behaviors.Add(sdb);
            }
            sdb.HttpHelpPageEnabled = false;
            sdb.HttpHelpPageUrl = new Uri(baseAddress + "/helpPage/GetHelpPage");

            host.Description.Behaviors.Add(new ServiceMetadataBehavior { HttpGetEnabled = true });

            host.Open();
            Console.WriteLine("Host opened");

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_c09e7c00_8a1e_42a5_8c6b_9eb2c7ffe918
    {
        [DataContract]
        public class Person
        {
            [DataMember]
            public string fname { get; set; }
        }
        [ServiceContract]
        public interface ITest
        {
            [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped)]
            Person[] Echo(Person[] persons);
        }
        [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
        public class Service : ITest
        {
            public Person[] Echo(Person[] persons) { return persons; }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), new WebHttpBinding(), "").Behaviors.Add(new WebHttpBehavior());

            host.Open();
            Console.WriteLine("Host opened");

            Util.SendRequest(baseAddress + "/Echo", "POST", "application/json", "{\"persons\": [{\"fname\":\"John\"}, {\"fname\":\"sheena\"}}");

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_b2984880_f984_4095_a4b4_8d60324490a1
    {
        [ServiceContract]
        public interface ITest
        {
            [WebGet]
            int Add(int x, int y);
        }
        public class Service : ITest
        {
            public int Add(int x, int y) { return x + y; }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), new WebHttpBinding { CrossDomainScriptAccessEnabled = true }, "").Behaviors.Add(new WebScriptEnablingBehavior());

            host.Open();
            Console.WriteLine("Host opened");

            WebClient c = new WebClient();
            Console.WriteLine(c.DownloadString(baseAddress + "/Add?x=123&y=333&callback=foo"));

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_4efa77d0_14fe_45cb_93b4_a9fee0e2884a
    {
        [DataContract(Name = "MyType", Namespace = "http://my.namespace.com")]
        public class MyTypeV1 : IExtensibleDataObject
        {
            public ExtensionDataObject ExtensionData { get; set; }
            [DataMember]
            public string str;
        }
        [DataContract(Name = "MyType", Namespace = "http://my.namespace.com")]
        public class MyTypeV2 : IExtensibleDataObject
        {
            public ExtensionDataObject ExtensionData { get; set; }
            [DataMember]
            public string str;
            [DataMember]
            public int dc;
        }
        public static void Test()
        {
            DataContractSerializer dcs2 = new DataContractSerializer(typeof(MyTypeV2));
            MyTypeV2 v2 = new MyTypeV2 { dc = 123, str = "hello" };
            MemoryStream ms = new MemoryStream();
            dcs2.WriteObject(ms, v2);
            Console.WriteLine(Encoding.UTF8.GetString(ms.ToArray()));
            ms.Position = 0;
            DataContractSerializer dcs1 = new DataContractSerializer(typeof(MyTypeV1));
            MyTypeV1 v1 = (MyTypeV1)dcs1.ReadObject(ms);
            Console.WriteLine(v1);
            ms.SetLength(0);
            dcs1.WriteObject(ms, v1);
            Console.WriteLine(Encoding.UTF8.GetString(ms.ToArray()));

            ms.SetLength(0);
            bool ignoreExtensionDataObject = true;
            DataContractJsonSerializer dcjs1;
            if (ignoreExtensionDataObject)
            {
                dcjs1 = new DataContractJsonSerializer(typeof(MyTypeV1), null, int.MaxValue, true, null, false);
            }
            else
            {
                dcjs1 = new DataContractJsonSerializer(typeof(MyTypeV1));
            }
            dcjs1.WriteObject(ms, v1);
            Console.WriteLine(Encoding.UTF8.GetString(ms.ToArray()));

            Console.WriteLine();

            DataContractJsonSerializer dcjs2 = new DataContractJsonSerializer(typeof(MyTypeV2));
            ms.SetLength(0);
            dcjs2.WriteObject(ms, v2);
            Console.WriteLine(Encoding.UTF8.GetString(ms.ToArray()));
            ms.Position = 0;
            v1 = (MyTypeV1)dcjs1.ReadObject(ms);
            Console.WriteLine(v1);
            ms.SetLength(0);
            dcjs1.WriteObject(ms, v1);
            Console.WriteLine(Encoding.UTF8.GetString(ms.ToArray()));

        }
    }

    public class Post_a30905e9_86c9_4117_b170_a8f1d72b478e
    {
        static class Arrays
        {
            internal static byte[] ServerArray;
            internal static byte[] ClientArray;

            internal static void PrintArray(string title, byte[] array)
            {
                Console.WriteLine(title);
                for (int i = 0; i < array.Length; i++)
                {
                    Console.Write(" {0}", (int)array[i]);
                }
                Console.WriteLine();
            }
        }
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            byte[] Echo(byte[] array);
        }
        public class Service : ITest
        {
            public byte[] Echo(byte[] array)
            {
                Arrays.ServerArray = array;
                Arrays.PrintArray("Inside service, array parameter", array);
                return array;
            }
        }
        public class MyMessageEncodingBindingElement : MessageEncodingBindingElement
        {
            public MessageEncodingBindingElement inner;
            bool serverEncoder;
            public MyMessageEncodingBindingElement(MessageEncodingBindingElement inner, bool serverEncoder)
            {
                this.inner = inner;
                this.serverEncoder = serverEncoder;
            }
            public override MessageEncoderFactory CreateMessageEncoderFactory()
            {
                return new MyMessageEncoderFactory(this.inner.CreateMessageEncoderFactory(), this.serverEncoder);
            }
            public override MessageVersion MessageVersion
            {
                get { return this.inner.MessageVersion; }
                set { this.inner.MessageVersion = value; }
            }
            public override BindingElement Clone()
            {
                return new MyMessageEncodingBindingElement((MessageEncodingBindingElement)this.inner.Clone(), this.serverEncoder);
            }
            public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
            {
                context.BindingParameters.Add(this);
                return context.BuildInnerChannelFactory<TChannel>();
            }
            public override IChannelListener<TChannel> BuildChannelListener<TChannel>(BindingContext context)
            {
                context.BindingParameters.Add(this);
                return context.BuildInnerChannelListener<TChannel>();
            }
            public override bool CanBuildChannelFactory<TChannel>(BindingContext context)
            {
                return context.CanBuildInnerChannelFactory<TChannel>();
            }
            public override bool CanBuildChannelListener<TChannel>(BindingContext context)
            {
                return context.CanBuildInnerChannelListener<TChannel>();
            }

            class MyMessageEncoderFactory : MessageEncoderFactory
            {
                MessageEncoderFactory inner;
                bool serverEncoder;
                public MyMessageEncoderFactory(MessageEncoderFactory inner, bool serverEncoder)
                {
                    this.inner = inner;
                    this.serverEncoder = serverEncoder;
                }
                public override MessageEncoder Encoder
                {
                    get { return new MyMessageEncoder(this.inner.Encoder, this.serverEncoder); }
                }
                public override MessageVersion MessageVersion
                {
                    get { return this.inner.MessageVersion; }
                }
            }

            class MyMessageEncoder : MessageEncoder
            {
                MessageEncoder inner;
                bool serverEncoder;
                public MyMessageEncoder(MessageEncoder inner, bool serverEncoder)
                {
                    this.inner = inner;
                    this.serverEncoder = serverEncoder;
                }
                public override string ContentType
                {
                    get { return this.inner.ContentType; }
                }
                public override string MediaType
                {
                    get { return this.inner.MediaType; }
                }
                public override MessageVersion MessageVersion
                {
                    get { return this.inner.MessageVersion; }
                }
                public override Message ReadMessage(ArraySegment<byte> buffer, BufferManager bufferManager, string contentType)
                {
                    return this.inner.ReadMessage(buffer, bufferManager, contentType);
                }
                public override Message ReadMessage(Stream stream, int maxSizeOfHeaders, string contentType)
                {
                    throw new NotImplementedException();
                }
                public override ArraySegment<byte> WriteMessage(Message message, int maxMessageSize, BufferManager bufferManager, int messageOffset)
                {
                    if (this.serverEncoder)
                    {
                        for (int i = 0; i < Arrays.ServerArray.Length; i++)
                        {
                            Arrays.ServerArray[i] = (byte)(Arrays.ServerArray[i] - 1);
                        }
                        Arrays.PrintArray("Modified server array prior to serializing the message", Arrays.ServerArray);
                    }
                    else
                    {
                        for (int i = 0; i < Arrays.ClientArray.Length; i++)
                        {
                            Arrays.ClientArray[i] = (byte)(Arrays.ClientArray[i] * 2);
                        }
                        Arrays.PrintArray("Modified client array prior to serializing the message", Arrays.ClientArray);
                    }

                    return this.inner.WriteMessage(message, maxMessageSize, bufferManager, messageOffset);
                }
                public override void WriteMessage(Message message, Stream stream)
                {
                    throw new NotImplementedException();
                }
            }
        }
        static Binding GetBinding(bool isService)
        {
            CustomBinding result = new CustomBinding(new BasicHttpBinding());
            for (int i = 0; i < result.Elements.Count; i++)
            {
                if (result.Elements[i] is MessageEncodingBindingElement)
                {
                    result.Elements[i] = new MyMessageEncodingBindingElement((MessageEncodingBindingElement)result.Elements[i], isService);
                }
            }
            return result;
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), GetBinding(true), "");
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(GetBinding(false), new EndpointAddress(baseAddress));
            ITest proxy = factory.CreateChannel();

            Arrays.ClientArray = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            Arrays.PrintArray("Client array, prior to calling service", Arrays.ClientArray);
            byte[] result = proxy.Echo(Arrays.ClientArray);
            Arrays.PrintArray("Result of service call", result);

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_53e625b9_756c_46fc_871c_f4454ffd035d
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            string EchoString(string text);
        }
        public class Service : ITest
        {
            public string EchoString(string text)
            {
                return text;
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.Open();
            Console.WriteLine("Host opened");

            Util.SendRequest(baseAddress + "?wsdl", "GET", null, null);

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_3a23a527_24f1_412e_b14e_75da842949a5
    {
        [DataContract]
        public class Contract
        {
            [DataMember(Order = 0)]
            public bool IsValid { get; set; }

            // not a data contract
            public string Messages { get; set; }

            [DataMember(Order = 1, EmitDefaultValue = false, IsRequired = false)]
            string ValidMessages { get; set; }

            [DataMember(Order = 1, EmitDefaultValue = false, IsRequired = false)]
            string InvalidMessages { get; set; }

            public override string ToString()
            {
                return string.Format("Contract[IsValid={0},Messages={1}]", this.IsValid, this.Messages);
            }

            [OnSerializing]
            void OnSerializing(StreamingContext ctx)
            {
                if (this.IsValid)
                {
                    this.ValidMessages = this.Messages;
                    this.InvalidMessages = null;
                }
                else
                {
                    this.InvalidMessages = this.Messages;
                    this.ValidMessages = null;
                }
            }

            [OnDeserialized]
            void OnDeserialized(StreamingContext ctx)
            {
                this.Messages = this.IsValid ? this.ValidMessages : this.InvalidMessages;
            }
        }

        [Serializable]
        public class Contract2 : ISerializable
        {
            public bool IsValid { get; set; }
            public string Messages { get; set; }

            public override string ToString()
            {
                return string.Format("Contract2[IsValid={0},Messages={1}]", this.IsValid, this.Messages);
            }

            public Contract2() { }

            public Contract2(SerializationInfo info, StreamingContext context)
            {
                this.IsValid = info.GetBoolean("IsValid");
                this.Messages = info.GetString(this.IsValid ? "ValidMessages" : "InvalidMessages");
            }

            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue("IsValid", this.IsValid);
                info.AddValue(this.IsValid ? "ValidMessages" : "InvalidMessages", this.Messages);
            }
        }

        static string SerializeToString(DataContractSerializer dcs, object instance)
        {
            MemoryStream ms = new MemoryStream();
            dcs.WriteObject(ms, instance);
            return Encoding.UTF8.GetString(ms.ToArray());
        }

        static object DeserializeFromString(DataContractSerializer dcs, string serialized)
        {
            return dcs.ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(serialized)));
        }

        public static void Test()
        {
            Console.WriteLine("Option 1 - multiple properties");
            DataContractSerializer dcs = new DataContractSerializer(typeof(Contract));
            string validSerialized = SerializeToString(dcs, new Contract { IsValid = true, Messages = "This is a valid contract" });
            Console.WriteLine("Valid serialized: {0}", validSerialized);
            string invalidSerialized = SerializeToString(dcs, new Contract { IsValid = false, Messages = "This is an invalid contract" });
            Console.WriteLine("Invalid serialized: {0}", invalidSerialized);
            Console.WriteLine();
            Console.WriteLine("Valid deserialized: {0}", DeserializeFromString(dcs, validSerialized));
            Console.WriteLine("Incalid deserialized: {0}", DeserializeFromString(dcs, invalidSerialized));

            Console.WriteLine();
            Console.WriteLine("Option 2 - ISerializable");
            dcs = new DataContractSerializer(typeof(Contract2));
            validSerialized = SerializeToString(dcs, new Contract2 { IsValid = true, Messages = "This is a valid contract" });
            Console.WriteLine("Valid serialized: {0}", validSerialized);
            invalidSerialized = SerializeToString(dcs, new Contract2 { IsValid = false, Messages = "This is an invalid contract" });
            Console.WriteLine("Invalid serialized: {0}", invalidSerialized);
            Console.WriteLine();
            Console.WriteLine("Valid deserialized: {0}", DeserializeFromString(dcs, validSerialized));
            Console.WriteLine("Incalid deserialized: {0}", DeserializeFromString(dcs, invalidSerialized));
        }
    }

    public class Post_1698968e_6608_485d_a8d5_d4c0138c1078
    {
        [DataContract]
        public class UserProfile
        {
            [DataMember(Name = "name", EmitDefaultValue = false)]
            public string name { get; set; }

            [DataMember(Name = "email", EmitDefaultValue = false)]
            public string email { get; set; }

            [DataMember(Name = "dataId", EmitDefaultValue = false)]
            public int dataId { get; set; }

            [DataMember(Name = "country", EmitDefaultValue = false)]
            public string country { get; set; }

            [DataMember(Name = "userId", EmitDefaultValue = false)]
            public int userId { get; set; }

            [DataMember(Name = "password", EmitDefaultValue = false)]
            public string tempPassword { get; set; }

            [DataMember(Name = "result", EmitDefaultValue = false)]
            public int result { get; set; }
        }

        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json)]
            [return: MessageParameter(Name = "userCreateResults")]
            UserProfile[] CreateUsers(UserProfile[] userList);

            [OperationContract]
            [WebInvoke(Method = "PUT", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json)]
            [return: MessageParameter(Name = "userUpdateResults")]
            UserProfile[] UpdateUsers(UserProfile[] userList);

            [OperationContract]
            [WebInvoke(Method = "DELETE", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json)]
            [return: MessageParameter(Name = "userDeleteResults")]
            UserProfile[] DeleteUsers(UserProfile[] userList);

            [OperationContract]
            [WebGet(BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json)]
            [return: MessageParameter(Name = "getUserResults")]
            UserProfile[] GetAllUsers();
        }

        public class Service : ITest
        {
            static List<UserProfile> users = new List<UserProfile>();
            static int nextUserId = 1;

            public UserProfile[] CreateUsers(UserProfile[] userList)
            {
                List<UserProfile> result = new List<UserProfile>();
                foreach (UserProfile toCreate in userList)
                {
                    UserProfile newUser = new UserProfile
                    {
                        name = toCreate.name,
                        email = toCreate.email,
                        dataId = toCreate.dataId,
                        country = toCreate.country,
                        userId = nextUserId++,
                        tempPassword = "changeme",
                        result = 200,
                    };

                    users.Add(newUser);
                    result.Add(new UserProfile
                    {
                        email = newUser.email,
                        userId = newUser.userId,
                        tempPassword = newUser.tempPassword,
                        result = newUser.result,
                    });
                }

                return result.ToArray();
            }

            public UserProfile[] UpdateUsers(UserProfile[] userList)
            {
                List<UserProfile> result = new List<UserProfile>();

                foreach (UserProfile toUpdate in userList)
                {
                    int index = FindUser(toUpdate.userId);
                    UserProfile toReturn = new UserProfile { userId = toUpdate.userId };
                    result.Add(toReturn);
                    if (index == -1)
                    {
                        toReturn.result = 404;
                    }
                    else
                    {
                        toReturn.result = 200;
                        users[index].name = toUpdate.name;
                        users[index].email = toUpdate.email;
                        users[index].dataId = toUpdate.dataId;
                        users[index].country = toUpdate.country;
                    }
                }

                return result.ToArray();
            }

            public UserProfile[] DeleteUsers(UserProfile[] userList)
            {
                List<UserProfile> result = new List<UserProfile>();
                List<int> indexesToDelete = new List<int>();
                bool allUsersDeletedSuccessfully = true;

                foreach (UserProfile toDelete in userList)
                {
                    int index = FindUser(toDelete.userId);
                    if (index == -1)
                    {
                        allUsersDeletedSuccessfully = false;
                        result.Add(new UserProfile { userId = toDelete.userId, result = 404 });
                    }
                    else
                    {
                        result.Add(new UserProfile { userId = toDelete.userId, result = 200 });
                        users.RemoveAt(index);
                    }
                }

                if (allUsersDeletedSuccessfully)
                {
                    WebOperationContext.Current.OutgoingResponse.SuppressEntityBody = true;
                    return null;
                }
                else
                {
                    return result.ToArray();
                }
            }

            public UserProfile[] GetAllUsers()
            {
                return users.ToArray();
            }

            private int FindUser(int userId)
            {
                for (int i = 0; i < users.Count; i++)
                {
                    if (users[i].userId == userId)
                    {
                        return i;
                    }
                }

                return -1;
            }
        }

        public static void Test()
        {
            string baseAddres = "http://" + Environment.MachineName + ":8000/Service";
            WebServiceHost host = new WebServiceHost(typeof(Service), new Uri(baseAddres));
            host.Open();

            Util.SendRequest(baseAddres + "/CreateUsers", "POST", "application/json", @"{
  ""userList"" :
   [
    {
       ""name"" : ""Biff Tanner"",
       ""email"" : ""biff@tanner.com"",
       ""dataId"" : 111,
       ""country"" : ""US""
    },
    {
       ""name"" : ""Doc Brown"",
       ""email"" : ""emmett@brown.com"",
       ""dataId"" : 222,
       ""country"" : ""UK""
    },
    {
       ""name"" : ""Marty McFly"",
       ""email"" : ""marty@mcfly.com"",
       ""dataId"" : 333,
       ""country"" : ""US""
    }
   ]
}");

            Util.SendRequest(baseAddres + "/GetAllUsers", "GET", null, null);

            Util.SendRequest(baseAddres + "/UpdateUsers", "PUT", "application/json", @"{
  ""userList"" :
   [
    {
       ""userId"" : 2,
       ""name"" : ""Emmett Brown"",
       ""email"" : ""doc@brown.com"",
       ""dataId"" : 234,
       ""country"" : ""US""
    },
    {
       ""userId"" : 66,
       ""name"" : ""Not exist"",
       ""email"" : ""not@exist.com"",
       ""dataId"" : 1,
       ""country"" : ""US""
    }
   ]
}");

            Util.SendRequest(baseAddres + "/GetAllUsers", "GET", null, null);

            Util.SendRequest(baseAddres + "/DeleteUsers", "DELETE", "application/json", @"{
    ""userList"" :
     [
         {""userId"" : 1},
         {""userId"" : 23}
     ]
}");

            Util.SendRequest(baseAddres + "/GetAllUsers", "GET", null, null);

            Util.SendRequest(baseAddres + "/DeleteUsers", "DELETE", "application/json", @"{
    ""userList"" :
     [
         {""userId"" : 2}
     ]
}");

            Util.SendRequest(baseAddres + "/GetAllUsers", "GET", null, null);
        }
    }

    public class XElementTest
    {
        public static void Test()
        {
            XElement xe = XElement.Parse("<root><foo><bar>hello</bar></foo></root>");
            xe.Changing += new EventHandler<XObjectChangeEventArgs>(xe_Changing);
            xe.Changed += new EventHandler<XObjectChangeEventArgs>(xe_Changed);
            xe.Element("foo").Element("bar").SetValue("world");
            xe.Element("foo").Add(new XAttribute("attr", "value"));
            Console.WriteLine(xe);
        }

        static void xe_Changed(object sender, XObjectChangeEventArgs e)
        {
            Console.WriteLine("{0}: {1} {2}", MethodBase.GetCurrentMethod().Name, e.ObjectChange, sender);
        }

        static void xe_Changing(object sender, XObjectChangeEventArgs e)
        {
            Console.WriteLine("{0}: {1} {2}", MethodBase.GetCurrentMethod().Name, e.ObjectChange, sender);
        }
    }

    public class Post_691871fd_41bf_42e4_8ac9_4fd033d2e4c1
    {
        [ServiceContract]
        public interface ITest
        {
            [WebGet(UriTemplate = "/items/{itemId}/elements", ResponseFormat = WebMessageFormat.Json)]
            string GetUri(string itemId);
            [WebGet(UriTemplate = "/items/{*end}", ResponseFormat = WebMessageFormat.Json)]
            string GetUri2(string end);
        }
        public class Service : ITest
        {
            public string GetUri(string itemId)
            {
                return itemId + " - " + WebOperationContext.Current.IncomingRequest.UriTemplateMatch.RequestUri.ToString();
            }
            public string GetUri2(string end)
            {
                if (end.EndsWith("/elements"))
                {
                    string itemId = end.Substring(0, end.Length - "/elements".Length);
                    return "[from GetUri2] " + GetUri(itemId);
                }
                else
                {
                    throw new WebFaultException(HttpStatusCode.NotFound);
                }
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), new WebHttpBinding(), "").Behaviors.Add(new WebHttpBehavior());
            host.Open();

            Util.SendRequest(baseAddress + "/items/fooBar/elements", "GET", null, null);
            Util.SendRequest(baseAddress + "/items/foo/Bar/elements", "GET", null, null);
        }
    }

    public class Post_04d37c1a_2b7d_45fd_b696_349459403c0c
    {
        [ServiceContract(Name = "ITest", Namespace = "http://tempuri.org/")]
        public interface ITestServer
        {
            [OperationContract]
            int GetValue(int myValue);
        }
        [ServiceContract(Name = "ITest", Namespace = "http://tempuri.org/")]
        public interface ITestClient
        {
            [OperationContract]
            int GetValue(int myValue);
            [OperationContract(AsyncPattern = true)]
            IAsyncResult BeginGetValue(int myValue, AsyncCallback callback, object asyncState);
            int EndGetValue(IAsyncResult r);
        }
        public class Service : ITestServer
        {
            public int GetValue(int myValue)
            {
                return myValue;
            }
        }
        public static void Test()
        {
            string baseAddress = "net.tcp://localhost:8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITestServer), new NetTcpBinding(), "");
            host.Open();

            ChannelFactory<ITestClient> factory = new ChannelFactory<ITestClient>(new NetTcpBinding(), new EndpointAddress(baseAddress));
            ITestClient proxy = factory.CreateChannel();

            // Sync version
            Console.WriteLine(proxy.GetValue(123));

            // Async version
            proxy.BeginGetValue(444, new AsyncCallback(AddCallback), proxy);

            Console.WriteLine("Press ENTER to close");
            Console.ReadLine();
            ((IClientChannel)proxy).Close();
            factory.Close();
            host.Close();
        }
        static void AddCallback(IAsyncResult asyncResult)
        {
            ITestClient proxy = (ITestClient)asyncResult.AsyncState;
            Console.WriteLine("[in callback] {0}", proxy.EndGetValue(asyncResult));
        }
    }

    public class Post_1f9f0ddb_7f42_432e_bef3_b4ae97c2a592
    {
        [ServiceContract]
        public interface IImages
        {
            [WebInvoke(UriTemplate = "UploadFile/{fileName}")]
            void UploadFile(string fileName, Stream fileContents);
        }
        [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
        public class ImageService : IImages
        {
            public void UploadFile(string fileName, Stream fileContents)
            {
                Console.WriteLine("Received upload request for {0}, with {1} bytes.", fileName, CountBytes(fileContents));
            }

            private int CountBytes(Stream fileContents)
            {
                int result = 0;
                int bytesRead;
                byte[] buffer = new byte[1000];
                do
                {
                    bytesRead = fileContents.Read(buffer, 0, buffer.Length);
                    result += bytesRead;
                } while (bytesRead > 0);
                return result;
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(ImageService), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(IImages), new WebHttpBinding(), "").Behaviors.Add(new WebHttpBehavior());
            host.Description.Behaviors.Add(new ServiceMetadataBehavior { HttpGetEnabled = true });
            host.Open();

            // This will show the error
            Util.SendRequest(baseAddress, "GET", null, null);

            // This will show the error
            Util.SendRequest(baseAddress + "/UploadFile/myname", "GET", null, null);

            // This will succeed
            Util.SendRequest(baseAddress + "/UploadFile/myname", "POST", "text/plain", "This is the file content");
        }
    }

    public class Post_7f0e9957_48b6_4c2d_80b9_433989a92167
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            StrokeCollection EchoCollection(Stroke[] strokes);
        }
        [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
        public class Service : ITest
        {
            public StrokeCollection EchoCollection(Stroke[] strokes)
            {
                return new StrokeCollection(strokes);
            }
        }
        public class NewStroke
        {
            public StylusPointCollection points;
            public DrawingAttributes attributes;
        }
        class StrokeSurrogate : IDataContractSurrogate
        {
            public object GetCustomDataToExport(Type clrType, Type dataContractType)
            {
                return null;
            }

            public object GetCustomDataToExport(MemberInfo memberInfo, Type dataContractType)
            {
                return null;
            }

            public Type GetDataContractType(Type type)
            {
                if (type == typeof(Stroke))
                {
                    return typeof(NewStroke);
                }

                return type;
            }

            public object GetDeserializedObject(object obj, Type targetType)
            {
                if (obj is NewStroke)
                {
                    NewStroke ns = (NewStroke)obj;
                    Stroke s = new Stroke(ns.points, ns.attributes);
                    return s;
                }

                return obj;
            }

            public void GetKnownCustomDataTypes(Collection<Type> customDataTypes)
            {
            }

            public object GetObjectToSerialize(object obj, Type targetType)
            {
                if (obj is Stroke)
                {
                    Stroke s = (Stroke)obj;
                    NewStroke ns = new NewStroke { attributes = s.DrawingAttributes, points = s.StylusPoints };
                    return ns;
                }

                return obj;
            }

            public Type GetReferencedTypeOnImport(string typeName, string typeNamespace, object customData)
            {
                return null;
            }

            public System.CodeDom.CodeTypeDeclaration ProcessImportedType(System.CodeDom.CodeTypeDeclaration typeDeclaration, System.CodeDom.CodeCompileUnit compileUnit)
            {
                return null;
            }
        }
        class MyInspector : IDispatchMessageInspector, IEndpointBehavior
        {
            public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
            {
            }

            public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
            {
            }

            public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
            {
                endpointDispatcher.DispatchRuntime.MessageInspectors.Add(this);
            }

            public void Validate(ServiceEndpoint endpoint)
            {
            }

            public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
            {
                Console.WriteLine("AfterReceiveRequest, req = {0}", request);
                return null;
            }

            public void BeforeSendReply(ref Message reply, object correlationState)
            {
                Console.WriteLine("BeforeSendReply, reply = {0}", reply);
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            ServiceEndpoint endpoint = host.AddServiceEndpoint(typeof(ITest), new BasicHttpBinding(), "");
            endpoint.Behaviors.Add(new MyInspector());
            foreach (var od in endpoint.Contract.Operations)
            {
                DataContractSerializerOperationBehavior dcsob = od.Behaviors.Find<DataContractSerializerOperationBehavior>();
                dcsob.DataContractSurrogate = new StrokeSurrogate();
            }
            host.Open();

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(new BasicHttpBinding(), new EndpointAddress(baseAddress));
            foreach (var od in factory.Endpoint.Contract.Operations)
            {
                DataContractSerializerOperationBehavior dcsob = od.Behaviors.Find<DataContractSerializerOperationBehavior>();
                dcsob.DataContractSurrogate = new StrokeSurrogate();
            }
            ITest proxy = factory.CreateChannel();

            StylusPointCollection spc = new StylusPointCollection
            {
                new StylusPoint(1, 1),
                new StylusPoint(1, 2),
            };
            Stroke s1 = new Stroke(spc, new DrawingAttributes { Color = System.Windows.Media.Colors.Red, FitToCurve = true, StylusTip = StylusTip.Rectangle });
            var result = proxy.EchoCollection(new Stroke[] { s1 });
            Console.WriteLine(result);
        }
    }

    public class Post_cab05f3d_ec18_4f0e_85b0_a52b18bc732f
    {
        static readonly string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
        static void LogWithThreadAndTimestamp(string text, params object[] args)
        {
            if (args != null && args.Length > 0)
            {
                text = string.Format(text, args);
            }
            Console.WriteLine("[{0:00} - {1}] {2}", Thread.CurrentThread.ManagedThreadId, DateTime.Now.ToString("hh:mm:ss.fff"), text);
        }
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            string EchoString(string text);
        }
        public class Service : ITest
        {
            public string EchoString(string text)
            {
                LogWithThreadAndTimestamp("Service {0}, Entering {1}", this.GetHashCode(), MethodBase.GetCurrentMethod().Name);
                Thread.Sleep(2000);
                LogWithThreadAndTimestamp("Service {0}, Leaving {1}", this.GetHashCode(), MethodBase.GetCurrentMethod().Name);
                return text;
            }
        }
        static void CallService()
        {
            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(new BasicHttpBinding(), new EndpointAddress(baseAddress));
            ITest proxy = factory.CreateChannel();
            LogWithThreadAndTimestamp("Calling service");
            string result = proxy.EchoString("Echo from client thread " + Thread.CurrentThread.ManagedThreadId);
            LogWithThreadAndTimestamp("Called service, result = {0}", result);
        }
        static void CallServiceWithSameProxy(object proxyObj)
        {
            ITest proxy = proxyObj as ITest;
            LogWithThreadAndTimestamp("Calling service with same proxy");
            string result = proxy.EchoString("Echo from client thread " + Thread.CurrentThread.ManagedThreadId);
            LogWithThreadAndTimestamp("Called service with same proxy, result = {0}", result);
        }
        public static void Test()
        {
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), new BasicHttpBinding(), "");
            ServiceBehaviorAttribute sba = host.Description.Behaviors.Find<ServiceBehaviorAttribute>();
            sba.InstanceContextMode = InstanceContextMode.PerCall;
            sba.ConcurrencyMode = ConcurrencyMode.Multiple;
            host.Open();
            LogWithThreadAndTimestamp("Host opened");

            Thread t1 = new Thread(CallService);
            Thread t2 = new Thread(CallService);

            t1.Start();
            t2.Start();

            t1.Join();
            t2.Join();

            LogWithThreadAndTimestamp("Calls with different clients finished");
            Console.WriteLine();

            t1 = new Thread(CallServiceWithSameProxy);
            t2 = new Thread(CallServiceWithSameProxy);

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(new BasicHttpBinding(), new EndpointAddress(baseAddress));
            ITest proxy = factory.CreateChannel();

            t1.Start(proxy);
            t2.Start(proxy);

            t1.Join();
            t2.Join();

            LogWithThreadAndTimestamp("Calls with same client finished");
            Console.WriteLine();

            Console.WriteLine("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_12293ed1_e89d_4376_8888_3d89896b5582
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            void Process(Stream data);
        }
        public class Service : ITest
        {
            public void Process(Stream data)
            {
                string strData = new StreamReader(data).ReadToEnd();
                NameValueCollection nvc = HttpUtility.ParseQueryString(strData);
                var username = nvc["username"];
                var password = nvc["password"];
                Console.WriteLine("User: {0}; password: {1}", username, password);
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            WebHttpBinding binding = new WebHttpBinding();
            host.AddServiceEndpoint(typeof(ITest), binding, "").Behaviors.Add(new WebHttpBehavior());
            host.Open();
            Console.WriteLine("Host opened");

            string request = "username=John%20Doe&password=MySecretWord";

            WebClient client = new WebClient();
            client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            client.UploadString(baseAddress + "/Process", "POST", request);

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_3ce8c1ba_7f37_4314_ad2c_2130a53f9b10
    {
        public static void Test()
        {
            string file = @"c:\temp\deleteme\a.txt";
            XmlReaderSettings rs = new XmlReaderSettings();
            rs.DtdProcessing = DtdProcessing.Parse;
            XmlReader r = XmlReader.Create(file, rs);
            while (r.Read())
            {
                Console.WriteLine("{0}NodeType={1},Name={2},Value={3}", new string(' ', r.Depth), r.NodeType, r.Name, r.Value);
            }
        }
    }

    public class Post_91e1da2d_61f4_44cc_a228_4bd304683bbb
    {
        [DataContract]
        public class MyResponse
        {
            [DataMember]
            public string itemid;
            [DataMember]
            public string requestBody;
            [DataMember]
            public string[] requestHeaders;
        }
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            [WebInvoke(Method = "POST", UriTemplate = "/{itemid}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
            MyResponse doit(string itemid, Stream body);
        }
        public class Service : ITest
        {
            public MyResponse doit(string itemid, Stream body)
            {
                MyResponse result = new MyResponse();
                result.itemid = itemid;
                result.requestBody = new StreamReader(body).ReadToEnd();
                List<string> headers = new List<string>();
                foreach (string headerName in WebOperationContext.Current.IncomingRequest.Headers.AllKeys)
                {
                    headers.Add(string.Format("{0}: {1}", headerName, WebOperationContext.Current.IncomingRequest.Headers[headerName]));
                }
                result.requestHeaders = headers.ToArray();
                return result;
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            WebServiceHost host = new WebServiceHost(typeof(Service), new Uri(baseAddress));
            host.Open();

            Util.SendRequest(baseAddress + "/myItem", "POST", "text/plain", "This is the request body");
            Util.SendRequest(baseAddress + "/myItem", "POST", "text/xml", "<root>a xml request body</root>");
        }
    }

    public class EnumSerializationTest
    {
        public enum HouseSize1
        {
            OneStory,
            TwoStory,
            ThreeStory
        }
        public enum HouseSize2
        {
            [EnumMember(Value = "one story")]
            OneStory,
            [EnumMember(Value = "two story")]
            TwoStory,
            [EnumMember(Value = "three story")]
            ThreeStory
        }
        [DataContract]
        public enum HouseSize3
        {
            [EnumMember(Value = "one story")]
            OneStory,
            [EnumMember(Value = "two story")]
            TwoStory,
            [EnumMember(Value = "three story")]
            ThreeStory
        }
        static void SerializeMembers(Type enumType)
        {
            Array values = Enum.GetValues(enumType);
            DataContractSerializer dcs = new DataContractSerializer(values.GetType());
            MemoryStream ms = new MemoryStream();
            XmlWriterSettings ws = new XmlWriterSettings { Indent = true, IndentChars = "  ", OmitXmlDeclaration = true, Encoding = Encoding.UTF8 };
            XmlWriter w = XmlWriter.Create(ms, ws);
            dcs.WriteObject(w, values);
            w.Flush();
            Console.WriteLine("{0}: {1}", enumType.Name, Encoding.UTF8.GetString(ms.ToArray()));
        }
        public static void Test()
        {
            SerializeMembers(typeof(HouseSize1));
            SerializeMembers(typeof(HouseSize2));
            SerializeMembers(typeof(HouseSize3));
        }
    }

    public class Post_6a9fac43_2107_424d_9147_69b92a987c16
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            string Echo(string text);
        }
        public class Service : ITest
        {
            public string Echo(string text)
            {
                int index = OperationContext.Current.IncomingMessageHeaders.FindHeader("headerName", "headerNS");
                if (index < 0)
                {
                    Console.WriteLine("No header");
                }
                else
                {
                    Console.WriteLine(OperationContext.Current.IncomingMessageHeaders.GetHeader<string>(index));
                }
                return text;
            }
        }
        static Binding GetBinding()
        {
            BasicHttpBinding result = new BasicHttpBinding();
            return result;
        }
        class MyInspector : IEndpointBehavior, IClientMessageInspector
        {
            public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
            {
            }

            public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
            {
                clientRuntime.MessageInspectors.Add(this);
            }

            public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
            {
            }

            public void Validate(ServiceEndpoint endpoint)
            {
            }

            public void AfterReceiveReply(ref Message reply, object correlationState)
            {
            }

            public object BeforeSendRequest(ref Message request, IClientChannel channel)
            {
                Console.WriteLine("In BeforeSendRequest, OC.Current == null: {0}", OperationContext.Current == null);

                if (OperationContext.Current == null)
                {
                    using (new OperationContextScope(channel))
                    {
                        OperationContext.Current.OutgoingMessageHeaders.Add(MessageHeader.CreateHeader("headerName", "headerNS", "The header value added inside the inspector"));
                        Console.WriteLine("In BeforeSendRequest, inside scope, OC.Current == null: {0}", OperationContext.Current == null);
                    }
                }
                return null;
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), GetBinding(), "");
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(GetBinding(), new EndpointAddress(baseAddress));
            factory.Endpoint.Behaviors.Add(new MyInspector());
            ITest proxy = factory.CreateChannel();
            Console.WriteLine("Calling Echo outside context scope");
            Console.WriteLine(proxy.Echo("Hello outside context scope"));
            using (new OperationContextScope((IContextChannel)proxy))
            {
                OperationContext.Current.OutgoingMessageHeaders.Add(MessageHeader.CreateHeader("headerName", "headerNS", "The header value"));
                Console.WriteLine("Calling Echo inside context scope");
                Console.WriteLine(proxy.Echo("Hello within context scope"));
            }

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_12e09a40_df41_4bf7_8462_c40df68af3d5
    {
        [DataContract]
        public class MyDC
        {
            [DataMember]
            public int[] IntArray { get; set; }
        }

        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            MyDC EchoDC(MyDC input);
        }
        public class Service : ITest
        {
            public MyDC EchoDC(MyDC input)
            {
                return input;
            }
        }
        static Binding GetBinding()
        {
            BasicHttpBinding result = new BasicHttpBinding();
            return result;
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            ServiceEndpoint endpoint = host.AddServiceEndpoint(typeof(ITest), GetBinding(), "");
            foreach (OperationDescription od in endpoint.Contract.Operations)
            {
                DataContractSerializerOperationBehavior dcsob = od.Behaviors.Find<DataContractSerializerOperationBehavior>();
                dcsob.MaxItemsInObjectGraph = 102;
            }
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(GetBinding(), new EndpointAddress(baseAddress));
            ITest proxy = factory.CreateChannel();

            Console.WriteLine(proxy.EchoDC(new MyDC { IntArray = new int[100] }));

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_ba74bcf3_2c5b_4cca_a602_2f3760dbcc84
    {
        [DataContract]
        public class MyDC
        {
            [DataMember]
            public string str = "Hello";
            [DataMember]
            public int i = 1234;
        }
        [ServiceContract]
        public interface ITest
        {
            [WebGet(ResponseFormat = WebMessageFormat.Json)]
            MyDC GetDC();
        }
        public class Service : ITest
        {
            public MyDC GetDC()
            {
                return new MyDC();
            }
        }
        public class WrappedJsonDataContractSerializer : XmlObjectSerializer
        {
            public override bool IsStartObject(XmlDictionaryReader reader)
            {
                return false;
            }

            public override object ReadObject(XmlDictionaryReader reader, bool verifyObjectName)
            {
                return null;
            }

            public override void WriteEndObject(XmlDictionaryWriter writer)
            {
                writer.WriteEndElement();
            }

            public override void WriteObjectContent(XmlDictionaryWriter writer, object graph)
            {
                var resultString = "{\"str\":\"hello world\",\"i\":8888}";
                var result = resultString.ToCharArray();
                writer.WriteString(resultString);
            }

            public override void WriteStartObject(System.Xml.XmlDictionaryWriter writer, object graph)
            {
                writer.WriteStartElement("root");
            }
        }
        class MyDCSOB : DataContractSerializerOperationBehavior
        {
            public MyDCSOB(OperationDescription od) : base(od) { }
            public override XmlObjectSerializer CreateSerializer(Type type, string name, string ns, IList<Type> knownTypes)
            {
                return new WrappedJsonDataContractSerializer();
            }
            public override XmlObjectSerializer CreateSerializer(Type type, XmlDictionaryString name, XmlDictionaryString ns, IList<Type> knownTypes)
            {
                return new WrappedJsonDataContractSerializer();
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            ServiceEndpoint endpoint = host.AddServiceEndpoint(typeof(ITest), new WebHttpBinding(), "");
            endpoint.Behaviors.Add(new WebHttpBehavior());
            foreach (OperationDescription od in endpoint.Contract.Operations)
            {
                for (int i = 0; i < od.Behaviors.Count; i++)
                {
                    if (od.Behaviors[i] is DataContractSerializerOperationBehavior)
                    {
                        od.Behaviors[i] = new MyDCSOB(od);
                    }
                }
            }

            host.Open();
            Console.WriteLine("Host opened");

            Util.SendRequest(baseAddress + "/GetDC", "GET", null, null);

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_605526ee_2181_41a2_8d0e_ced1f5e2b825
    {
        public class FaqOptionCollection : List<FaqOption>
        {
            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                sb.Append('[');
                for (int i = 0; i < this.Count; i++)
                {
                    if (i > 0) sb.Append(',');
                    sb.Append(this[i].ToString());
                }
                sb.Append(']');
                return sb.ToString();
            }
        }
        [DataContract]
        public class FaqOption
        {
            [DataMember]
            public string topic;
            [DataMember]
            public string keyword;
            public override string ToString()
            {
                return string.Format("FaqOption[topic={0},keyword={1}]", topic, keyword);
            }
        }
        [ServiceContract]
        public interface ITest
        {
            [WebGet(UriTemplate = "Send?collection={collection}", RequestFormat = WebMessageFormat.Json)]
            void Send(FaqOptionCollection collection);
        }
        public class Service : ITest
        {
            public void Send(FaqOptionCollection collection)
            {
                Console.WriteLine("Input: {0}", collection);
            }
        }
        public class MyWebHttpBehavior : WebHttpBehavior
        {
            protected override QueryStringConverter GetQueryStringConverter(OperationDescription operationDescription)
            {
                return new JsonQueryStringConverter();
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            ServiceEndpoint endpoint = host.AddServiceEndpoint(typeof(ITest), new WebHttpBinding(), "");
            WebHttpBehavior behavior = new MyWebHttpBehavior();
            endpoint.Behaviors.Add(behavior);
            host.Open();
            Console.WriteLine("Host opened");

            Util.SendRequest(baseAddress + "/Send?collection=[{\"topic\":\"foo\",\"keyword\":\"foo2\"},{\"topic\":\"bar\",\"keyword\":\"bar2\"}]", "GET", null, null);
            //ChannelFactory<ITest> factory = new ChannelFactory<ITest>(GetBinding(), new EndpointAddress(baseAddress));
            //ITest proxy = factory.CreateChannel();

            //Console.WriteLine(proxy.EchoString("Hello"));
            //Console.WriteLine(proxy.EchoDC(new MyDC()));
            //Console.WriteLine(proxy.Add(3, 5));

            //((IClientChannel)proxy).Close();
            //factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_b69da75f_b597_4b32_b367_2ce3eec78160
    {
        [ServiceContract]
        public interface IOne
        {
            [OperationContract]
            string Op1();
        }

        [ServiceContract]
        public interface ITwo
        {
            [OperationContract]
            string Op2();
        }

        [ServiceContract]
        public interface IAll : IOne, ITwo
        {
        }

        public class AClass : IAll
        {
            public string Op1()
            {
                return "From Op1";
            }

            public string Op2()
            {
                return "From Op2";
            }
        }

        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(AClass), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(IAll), new BasicHttpBinding(), "");
            host.Description.Behaviors.Add(new ServiceMetadataBehavior { HttpGetEnabled = true });
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<IAll> factory = new ChannelFactory<IAll>(new BasicHttpBinding(), new EndpointAddress(baseAddress));
            IAll proxy = factory.CreateChannel();

            Console.WriteLine(proxy.Op1());
            Console.WriteLine(proxy.Op2());

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_71127320_ab87_46bf_8d21_0607e0fe71a4
    {
        public class MessageHdr
        {
            [XmlElement(ElementName = "messagecount", Order = 0)]
            public int MessageCnt { get; set; }

            [XmlElement(ElementName = "pagecnt", Order = 1)]
            public int PageCnt { get; set; }

            [XmlElement(ElementName = "message", Order = 2)]
            public SingleMessage[] Msgs { get; set; }
        }
        public class SingleMessage
        {
            [XmlElement(ElementName = "id")]
            public int Id { get; set; }
            [XmlElement(ElementName = "text")]
            public string Text { get; set; }
            [XmlElement(ElementName = "sender")]
            public string Sender { get; set; }
        }
        [ServiceContract]
        public interface ITest
        {
            [XmlSerializerFormat]
            [WebGet]
            MessageHdr GetData();
        }
        public class Service : ITest
        {
            public MessageHdr GetData()
            {
                return new MessageHdr
                {
                    MessageCnt = 20,
                    PageCnt = 5,
                    Msgs = new SingleMessage[]
                    {
                        new SingleMessage { Id = 122, Text = "a sample message", Sender = "AndyHamp" },
                        new SingleMessage { Id = 124, Text = "another sample message", Sender = "FredBloggs" },
                    }
                };
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            WebServiceHost host = new WebServiceHost(typeof(Service), new Uri(baseAddress));
            host.Open();
            Console.WriteLine("Host opened");

            WebClient webClient = new WebClient();
            Console.WriteLine(webClient.DownloadString(baseAddress + "/GetData"));

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_018620e6_b25f_469d_b256_570d34f658d6
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            string GetRuntimeBinding();
        }
        public class Service : ITest
        {
            public string GetRuntimeBinding()
            {
                Uri currentEndpointAddress = OperationContext.Current.EndpointDispatcher.EndpointAddress.Uri;
                foreach (var endpoint in OperationContext.Current.Host.Description.Endpoints)
                {
                    if (endpoint.Address.Uri == currentEndpointAddress)
                    {
                        Console.WriteLine("Current endpoint name: {0}", endpoint.Name);
                        return endpoint.Binding.Name;
                    }
                }

                return null;
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), new BasicHttpBinding(), "basic");
            host.AddServiceEndpoint(typeof(ITest), new WebHttpBinding(), "web").Behaviors.Add(new WebHttpBehavior());
            host.AddServiceEndpoint(typeof(ITest), new WSHttpBinding(), "ws");
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(new BasicHttpBinding(), new EndpointAddress(baseAddress + "/basic"));
            ITest proxy = factory.CreateChannel();
            Console.WriteLine("Calling basic: {0}", proxy.GetRuntimeBinding());
            ((IClientChannel)proxy).Close();
            factory.Close();

            factory = new ChannelFactory<ITest>(new WebHttpBinding(), new EndpointAddress(baseAddress + "/web"));
            factory.Endpoint.Behaviors.Add(new WebHttpBehavior());
            proxy = factory.CreateChannel();
            Console.WriteLine("Calling web (REST): {0}", proxy.GetRuntimeBinding());
            ((IClientChannel)proxy).Close();
            factory.Close();

            factory = new ChannelFactory<ITest>(new WSHttpBinding(), new EndpointAddress(baseAddress + "/ws"));
            proxy = factory.CreateChannel();
            Console.WriteLine("Calling WS: {0}", proxy.GetRuntimeBinding());
            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_6b1d2656_1e6d_4be5_966d_5708d31682aa
    {
        const string JSON = @"{
          ""SearchResponse"":
            {
                ""Version"":""2.2"",
                ""Query"":
                {
                    ""SearchTerms"":""Hello""
                },
                ""Translation"":
                {
                    ""Results"":
                    [
                        {""TranslatedTerm"":""Ciao""}
                    ]
                }
            }
        }";

        [DataContract(Name = "translateResponse")]
        public class MicrosoftTranslationResponse
        {
            [DataMember(Name = "SearchResponse")]
            public MicrosoftSearchResponse SearchResponse { get; set; }
        }

        [DataContract(Name = "SearchResponse")]
        public class MicrosoftSearchResponse
        {
            [DataMember(Name = "Version")]
            public string Version { get; set; }

            [DataMember(Name = "Query")]
            public MicrosoftSearchQuery Query { get; set; }

            [DataMember(Name = "Translation")]
            public MicrosoftSearchTranslation Translation { get; set; }
        }

        [DataContract(Name = "Query")]
        public class MicrosoftSearchQuery
        {
            [DataMember(Name = "SearchTerms")]
            public string SearchTerms { get; set; }
        }

        //[DataContract(Name = "Translation")]
        //public class MicrosoftSearchTranslation
        //{
        //    [DataMember]
        //    public MicrosoftSearchResults Results { get; set; }
        //}

        //[CollectionDataContract(ItemName = "TranslatedTerm")]
        //public class MicrosoftSearchResults : List<string>
        //{
        //    [DataMember(Name = "TranslatedTerm")]
        //    public string TranslatedTerm { get; set; }
        //}

        [DataContract(Name = "Translation")]
        public class MicrosoftSearchTranslation
        {
            [DataMember]
            public List<MicrosoftSearchResult> Results { get; set; }
        }

        [DataContract(Name = "TranslatedTerm")]
        public class MicrosoftSearchResult
        {
            [DataMember(Name = "TranslatedTerm")]
            public string TranslatedTerm { get; set; }
        }

        public static void Test()
        {
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(JSON));
            DataContractJsonSerializer dcjs = new DataContractJsonSerializer(typeof(MicrosoftTranslationResponse));
            try
            {
                object obj = dcjs.ReadObject(ms);
                Console.WriteLine(obj);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    public class Post_6a7ae6fd_e15f_4e75_ae3f_47b601f0707e
    {
        [MessageContract]
        public class UploadFileRequest
        {
            [MessageHeader]
            public string fileName;
            [MessageBodyMember]
            public Stream content;
        }
        [MessageContract]
        public class UploadFileResponse
        {
            [MessageBodyMember]
            public string UploadFileResult;
        }
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            UploadFileResponse UploadFile(UploadFileRequest input);
        }
        public class Service : ITest
        {
            static long CountBytes(Stream stream)
            {
                byte[] buffer = new byte[100000];
                int bytesRead;
                long totalBytesRead = 0;
                do
                {
                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    totalBytesRead += bytesRead;
                } while (bytesRead > 0);
                return totalBytesRead;
            }
            public UploadFileResponse UploadFile(UploadFileRequest input)
            {
                long byteCount = CountBytes(input.content);
                string result = string.Format("File {0} has {1} bytes", input.fileName, byteCount);
                return new UploadFileResponse { UploadFileResult = result };
            }
        }

        class MyReadonlyStream : Stream
        {
            long length;
            long leftToRead;
            public MyReadonlyStream(long length)
            {
                this.length = length;
                this.leftToRead = length;
            }

            public override bool CanRead
            {
                get { return true; }
            }

            public override bool CanSeek
            {
                get { return false; }
            }

            public override bool CanWrite
            {
                get { return false; }
            }

            public override void Flush()
            {
            }

            public override long Length
            {
                get { return this.length; }
            }

            public override long Position
            {
                get { throw new NotSupportedException(); }
                set { throw new NotSupportedException(); }
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                int toReturn = (int)Math.Min(this.leftToRead, (long)count);
                this.leftToRead -= toReturn;
                return toReturn;
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new NotSupportedException();
            }

            public override void SetLength(long value)
            {
                throw new NotSupportedException();
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                throw new NotSupportedException();
            }
        }

        static Binding GetBinding()
        {
            NetTcpBinding result = new NetTcpBinding();
            result.TransferMode = TransferMode.Streamed;
            result.MaxReceivedMessageSize = int.MaxValue;
            return result;
        }

        public static void Test()
        {
            string baseAddress = "net.tcp://localhost:8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), GetBinding(), "");
            host.Open();

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(GetBinding(), new EndpointAddress(baseAddress));
            ITest proxy = factory.CreateChannel();

            Stream fileToBeUploaded = new MyReadonlyStream(1234567890);
            var result = proxy.UploadFile(new UploadFileRequest { fileName = "TheFile.bin", content = fileToBeUploaded });
            Console.WriteLine(result.UploadFileResult);

            Console.WriteLine("Press ENTER to close");
            Console.ReadLine();
            ((IClientChannel)proxy).Close();
            factory.Close();
            host.Close();
        }
    }

    public class Post_b3f154d8_4ed7_4d7c_b281_999eebe90157
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            int CountBytes(byte[] file);
        }

        [ServiceContract]
        public interface ITestMessage
        {
            [OperationContract(Action = "*", ReplyAction = "*")]
            Message Process(Message input);
        }

        [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
        public class Service : ITest
        {
            public int CountBytes(byte[] file)
            {
                HttpRequestMessageProperty prop = (HttpRequestMessageProperty)OperationContext.Current.IncomingMessageProperties[HttpRequestMessageProperty.Name];
                var contentLength = prop.Headers[HttpRequestHeader.ContentLength];
                // with MTOM, the Content-Length is around ~11000 for a 10000-byte file; without it, it's around 13900.
                Console.WriteLine("In Service.CountBytes, file.Length = {0}, request length = {1}", file.Length, contentLength);
                return file.Length;
            }
        }

        static Binding GetBinding()
        {
            WSHttpBinding binding = new WSHttpBinding(SecurityMode.None);
            binding.MessageEncoding = WSMessageEncoding.Mtom;
            return binding;
        }

        // Simple [DC] class to create the expected body
        [DataContract(Name = "CountBytes", Namespace = "http://tempuri.org/")]
        public class RequestMessageClass
        {
            [DataMember]
            public byte[] file;

            public RequestMessageClass(int fileLen)
            {
                this.file = new byte[fileLen];
                for (int i = 0; i < fileLen; i++) this.file[i] = 0x33;
            }
        }

        static Message CreateRequestMessage(string address)
        {
            Message result = Message.CreateMessage(MessageVersion.Soap12WSAddressing10, "http://tempuri.org/ITest/CountBytes", new RequestMessageClass(10000));
            result.Headers.MessageId = new UniqueId();
            result.Headers.To = new Uri(address);
            return result;
        }

        class RequestMessageBodyWriter : BodyWriter
        {
            int fileSize;
            public RequestMessageBodyWriter(int fileSize)
                : base(true)
            {
                this.fileSize = fileSize;
            }

            public Message CreateRequestMessage(string address)
            {
                Message result = Message.CreateMessage(MessageVersion.Soap12WSAddressing10, "http://tempuri.org/ITest/CountBytes", this);
                result.Headers.MessageId = new UniqueId();
                result.Headers.To = new Uri(address);
                return result;
            }

            protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
            {
                byte[] buffer = new byte[this.fileSize];
                for (int i = 0; i < fileSize; i++) buffer[i] = 0x41;
                writer.WriteStartElement("CountBytes", "http://tempuri.org/");
                writer.WriteStartElement("file", "http://tempuri.org/");
                writer.WriteBase64(buffer, 0, buffer.Length);
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
        }

        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), GetBinding(), "");
            host.Open();
            Console.WriteLine("Host opened");

            byte[] file = new byte[10000];
            for (int i = 0; i < file.Length; i++) file[i] = 0x30;

            // Just testing that the service is ok...
            ChannelFactory<ITest> typedFactory = new ChannelFactory<ITest>(GetBinding(), new EndpointAddress(baseAddress));
            ITest typedProxy = typedFactory.CreateChannel();
            Console.WriteLine(typedProxy.CountBytes(file));

            // Now trying with an untyped message contract
            ChannelFactory<ITestMessage> msgFactory = new ChannelFactory<ITestMessage>(GetBinding(), new EndpointAddress(baseAddress));
            ITestMessage msgProxy = msgFactory.CreateChannel();
            Message input = CreateRequestMessage(baseAddress);
            Console.WriteLine(msgProxy.Process(input));
            Console.WriteLine();

            // Now trying with an IRequestChannel
            ChannelFactory<IRequestChannel> reqChannelFactory = new ChannelFactory<IRequestChannel>(GetBinding(), new EndpointAddress(baseAddress));
            IRequestChannel channel = reqChannelFactory.CreateChannel();
            input = CreateRequestMessage(baseAddress);
            Console.WriteLine("From IRequestChannel, message creation option 1");
            Console.WriteLine(channel.Request(input));

            Console.WriteLine();
            RequestMessageBodyWriter myBodyWriter = new RequestMessageBodyWriter(10000);
            input = myBodyWriter.CreateRequestMessage(baseAddress);
            Console.WriteLine("From IRequestChannel, message creation option 2");
            Console.WriteLine(channel.Request(input));

            ((IClientChannel)typedProxy).Close();
            typedFactory.Close();
            ((IClientChannel)msgProxy).Close();
            msgFactory.Close();
            channel.Close();
            reqChannelFactory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_239cdde9_0490_4bdf_976f_d14fc1569566
    {
        [DataContract(Name = "B", Namespace = "")]
        public class B1
        {
            [DataMember(Order = 1)]
            public int b;

            [DataMember(Order = 2)]
            public int c;

            [DataMember(Order = 3)]
            public int d;
        }

        [DataContract(Name = "B", Namespace = "")]
        public class B2
        {
            [DataMember(Order = 2, IsRequired = true)]
            public int b;

            [DataMember(Order = 3, IsRequired = true)]
            public int c;

            [DataMember(Order = 1, IsRequired = true)]
            public int d;
        }

        static string GenericToString(object obj)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(obj.GetType().Name);
            sb.Append('[');
            FieldInfo[] fields = obj.GetType().GetFields();
            for (int i = 0; i < fields.Length; i++)
            {
                if (i > 0) sb.Append(',');
                sb.AppendFormat("{0}={1}", fields[i].Name, fields[i].GetValue(obj));
            }
            sb.Append(']');
            return sb.ToString();
        }
        static string Serialize(object obj)
        {
            MemoryStream ms = new MemoryStream();
            DataContractSerializer dcs = new DataContractSerializer(obj.GetType());
            dcs.WriteObject(ms, obj);
            string uselessNamespaceDecl = " xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\"";
            string result = Encoding.UTF8.GetString(ms.ToArray());
            result = result.Replace(uselessNamespaceDecl, "");
            return result;
        }
        static T Deserialize<T>(string serialized)
        {
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(serialized));
            DataContractSerializer dcs = new DataContractSerializer(typeof(T));
            return (T)dcs.ReadObject(ms);
        }
        public static void Test()
        {
            string serialized1 = Serialize(new B1 { b = 1, c = 2, d = 3 });
            Console.WriteLine("B1: {0}", serialized1);
            B1 b1 = Deserialize<B1>(serialized1);
            Console.WriteLine(GenericToString(b1));
            Console.WriteLine();
            Console.WriteLine("Deserializing B1 out of order:");
            serialized1 = "<B><c>2</c><d>3</d><b>1</b></B>";
            Console.WriteLine("{0}: {1}", serialized1, GenericToString(Deserialize<B1>(serialized1)));
            serialized1 = "<B><d>3</d><c>2</c><b>1</b></B>";
            Console.WriteLine("{0}: {1}", serialized1, GenericToString(Deserialize<B1>(serialized1)));
            serialized1 = "<B><a>111</a><b>1</b><c>2</c><d>3</d></B>";
            Console.WriteLine("{0}: {1}", serialized1, GenericToString(Deserialize<B1>(serialized1)));
        }
    }

    public class Post_19500d14_78b7_4356_b817_fcc9abc2afcf
    {
        [DataContract]
        public class MyDC
        {
            [DataMember]
            public string str = "The string";
        }
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            string EchoString(string text);
            [OperationContract]
            int Add(int x, int y);
            [OperationContract]
            MyDC EchoDC(MyDC input);
        }
        public class Service : ITest
        {
            public string EchoString(string text)
            {
                return text;
            }

            public int Add(int x, int y)
            {
                return x + y;
            }

            public MyDC EchoDC(MyDC input)
            {
                return input;
            }
        }
        class MyInspector : IClientMessageInspector, IEndpointBehavior
        {
            public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
            {
            }

            public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
            {
                clientRuntime.MessageInspectors.Add(this);
            }

            public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
            {
            }

            public void Validate(ServiceEndpoint endpoint)
            {
            }

            public void AfterReceiveReply(ref Message reply, object correlationState)
            {
                Console.WriteLine("AfterReceiveReply, Action = {0}", reply.Headers.Action);
            }

            public object BeforeSendRequest(ref Message request, IClientChannel channel)
            {
                Console.WriteLine("BeforeSendRequest, Action = {0}", request.Headers.Action);
                return request.Headers.Action;
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), new BasicHttpBinding(), "basic");
            host.AddServiceEndpoint(typeof(ITest), new WSHttpBinding(SecurityMode.None), "ws");
            host.Open();
            Console.WriteLine("Host opened");

            for (int i = 0; i < 2; i++)
            {
                Binding binding;
                string relativeAddress;
                if (i == 0)
                {
                    binding = new BasicHttpBinding();
                    relativeAddress = "basic";
                }
                else
                {
                    binding = new WSHttpBinding(SecurityMode.None);
                    relativeAddress = "ws";
                }

                Console.WriteLine("Calling endpoint with SOAP version: {0}", binding.MessageVersion.Envelope);

                ChannelFactory<ITest> factory = new ChannelFactory<ITest>(binding, new EndpointAddress(baseAddress + "/" + relativeAddress));
                factory.Endpoint.Behaviors.Add(new MyInspector());
                ITest proxy = factory.CreateChannel();

                Console.WriteLine(proxy.EchoString("Hello"));
                Console.WriteLine(proxy.EchoDC(new MyDC()));
                Console.WriteLine(proxy.Add(3, 5));

                ((IClientChannel)proxy).Close();
                factory.Close();

                Console.WriteLine();
            }

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    [DataContract]
    public class Post_fe25607c_1b1c_4bd7_8c13_d303ff75e0e8
    {
        [DataContract]
        public class M
        {
            public M()
            {
                m2 = "m2"; //during deserialization this won't execute
            }

            [DataMember]
            public string m = "m"; //during deserializtion m won't be initialized to "m"

            [DataMember]
            public string m1 = "m1"; //during deserialization m1 won't be initialized to "m1"

            [DataMember]
            public string m2;

            [OnDeserialized]
            public void SerializationInitializer(StreamingContext ctx)
            {
                this.m = "m";
                this.m1 = "m1";
                this.m2 = "m2";
            }
        }

        [DataContract(Name = "M2", Namespace = "")]
        public class M2
        {
            public M2(string valueOfM2)
            {
                m2 = valueOfM2; //during deserialization this won't execute
            }

            [DataMember]
            public string m = "m"; //during deserializtion m won't be initialized to "m"

            [DataMember]
            public string m1 = "m1"; //during deserialization m1 won't be initialized to "m1"

            [DataMember]
            public string m2;

            [OnDeserializing]
            public void SerializationInitializer(StreamingContext ctx)
            {
                this.m = "m";
                this.m1 = "m1";
                this.m2 = "m2";
            }
        }

        public static string Serialize(object instance)
        {
            MemoryStream ms = new MemoryStream();
            DataContractSerializer dcs = new DataContractSerializer(instance.GetType());
            dcs.WriteObject(ms, instance);
            return Encoding.UTF8.GetString(ms.ToArray());
        }

        public static T Deserialize<T>(string serialized)
        {
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(serialized));
            DataContractSerializer dcs = new DataContractSerializer(typeof(T));
            return (T)dcs.ReadObject(ms);
        }

        public static void Test()
        {
            string serialized = Serialize(new M2("myM2") { m1 = "myM1" });
            Console.WriteLine(serialized);
            string serializedWithoutM1 = serialized.Replace("<m1>myM1</m1>", "");
            M2 deserialized = Deserialize<M2>(serializedWithoutM1);
            Console.WriteLine("M2[m={0},m1={1},m2={2}]", deserialized.m, deserialized.m1, deserialized.m2);
        }
    }

    public class Post_e9087687_d96f_455b_9ca0_5c175aa7e2ba
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            int UploadFileToServer(Stream fileStream);
            [OperationContract]
            Stream DownloadFileFromServer();
        }
        public class Service : ITest
        {
            public int UploadFileToServer(Stream fileStream)
            {
                int i, result = 0;
                while ((i = fileStream.ReadByte()) >= 0)
                {
                    result++;
                }

                return result;
            }
            public Stream DownloadFileFromServer()
            {
                return new MyReadOnlyStream(123);
            }
            class MyReadOnlyStream : Stream
            {
                int streamSize;
                public MyReadOnlyStream(int streamSize)
                {
                    this.streamSize = streamSize;
                }
                public override bool CanRead
                {
                    get { return true; }
                }

                public override bool CanSeek
                {
                    get { return false; }
                }

                public override bool CanWrite
                {
                    get { return false; }
                }

                public override void Flush()
                {
                }

                public override long Length
                {
                    get { throw new NotImplementedException(); }
                }

                public override long Position
                {
                    get { throw new NotImplementedException(); }
                    set { throw new NotImplementedException(); }
                }

                public override void Close()
                {
                    Console.WriteLine("Stream has been closed.");
                    Console.WriteLine(Environment.StackTrace);
                    base.Close();
                }

                public override int Read(byte[] buffer, int offset, int count)
                {
                    int toReturn = Math.Min(count, this.streamSize);
                    this.streamSize -= toReturn;
                    return toReturn;
                }

                public override long Seek(long offset, SeekOrigin origin)
                {
                    throw new NotImplementedException();
                }

                public override void SetLength(long value)
                {
                    throw new NotImplementedException();
                }

                public override void Write(byte[] buffer, int offset, int count)
                {
                    throw new NotImplementedException();
                }
            }
        }
        static Binding GetBinding()
        {
            BasicHttpBinding result = new BasicHttpBinding();
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

            string originalFile = Path.GetTempFileName();
            File.WriteAllText(originalFile, "those are the file contents");
            Console.WriteLine("Uploading file {0}", originalFile);

            FileStream fs = File.OpenRead(originalFile);
            int result = proxy.UploadFileToServer(fs);
            Console.WriteLine("Uploaded file had {0} bytes", result);
            fs.Close();

            try
            {
                File.Delete(originalFile);
                Console.WriteLine("Temp file deleted.");
            }
            catch (Exception e)
            {
                Console.WriteLine("{0}: {1}", e.GetType().FullName, e.Message);
                Exception inner = e.InnerException;
                while (inner != null)
                {
                    Console.WriteLine("    {0}: {1}", inner.GetType().FullName, inner.Message);
                    inner = inner.InnerException;
                }
            }

            Stream downloadedStream = proxy.DownloadFileFromServer();
            int byteCount = 0;
            int b;
            while ((b = downloadedStream.ReadByte()) >= 0)
            {
                byteCount++;
            }

            Console.WriteLine("Downloaded stream has {0} bytes.");
            downloadedStream.Close();
            Console.WriteLine("Closed the donwloaded stream.");

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.WriteLine("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_042f46ad_ac46_41cd_8fdf_628c1d77bb2f
    {
        [DataContract(Name = "Person", Namespace = "MyNamespace")]
        public class Person
        {
            [DataMember]
            public string Name { get; set; }
            [DataMember]
            public int Age { get; set; }
            [DataMember]
            public DateTime DateOfBirth { get; set; }
            [DataMember]
            public XmlDictionaryString dicString { get; set; }
        }

        class MyDictionary : XmlDictionary
        {
            static MyDictionary instance;
            private MyDictionary()
            {
                base.Add("DicStringValue1");
                base.Add("DicStringValue2");
            }

            public static MyDictionary Instance
            {
                get
                {
                    if (instance == null)
                    {
                        instance = new MyDictionary();
                    }
                    return instance;
                }
            }

            public XmlDictionaryString GetDicString(string value)
            {
                XmlDictionaryString result;
                this.TryLookup(value, out result);
                return result;
            }

            public override bool TryLookup(int key, out XmlDictionaryString result)
            {
                bool temp = base.TryLookup(key, out result);
                return temp;
            }
            public override bool TryLookup(string value, out XmlDictionaryString result)
            {
                bool temp = base.TryLookup(value, out result);
                return temp;
            }
            public override bool TryLookup(XmlDictionaryString value, out XmlDictionaryString result)
            {
                bool temp = base.TryLookup(value, out result);
                return temp;
            }
        }

        class MyWriterSession : XmlBinaryWriterSession
        {
            public Dictionary<int, string> items = new Dictionary<int, string>();
            public override bool TryAdd(XmlDictionaryString value, out int key)
            {
                bool result = base.TryAdd(value, out key);
                if (result)
                {
                    items.Add(key, value.Value);
                }

                return result;
            }
        }

        public static void Test()
        {
            Console.WriteLine("Not using any dictionaries");
            Test(false, false);
            Console.WriteLine();
            Console.WriteLine("Using static dictionary only");
            Test(true, false);
            Console.WriteLine();
            Console.WriteLine("Using session dictionary only");
            Test(false, true);
        }

        static byte[] Serialize(bool useStaticDictionary, bool useSessionDictionary, object instance)
        {
            DataContractSerializer dcs = new DataContractSerializer(instance.GetType());
            MemoryStream ms = new MemoryStream();
            MyWriterSession writerSession = null;
            if (useSessionDictionary)
            {
                writerSession = new MyWriterSession();
            }

            IXmlDictionary staticDictionary = MyDictionary.Instance;
            XmlDictionaryWriter writer = XmlDictionaryWriter.CreateBinaryWriter(ms, staticDictionary, writerSession);
            dcs.WriteObject(writer, instance);
            writer.Flush();
            if (useSessionDictionary)
            {
                MemoryStream dicStream = new MemoryStream();
                dicStream.WriteByte((byte)writerSession.items.Count);
                for (int i = 0; i < writerSession.items.Count; i++)
                {
                    string dicString = writerSession.items[i];
                    byte[] dicStringBytes = Encoding.UTF8.GetBytes(dicString);
                    dicStream.WriteByte((byte)dicStringBytes.Length);
                    dicStream.Write(dicStringBytes, 0, dicStringBytes.Length);
                }

                ms.Position = 0;
                ms.CopyTo(dicStream);
                return dicStream.ToArray();
            }
            else
            {
                return ms.ToArray();
            }
        }

        static T Deserialize<T>(bool useStaticDictionary, bool useSessionDictionary, byte[] serialized)
        {
            XmlBinaryReaderSession readerSession = null;
            int index = 0;
            if (useSessionDictionary)
            {
                readerSession = new XmlBinaryReaderSession();
                int dicStringCount = (int)serialized[index++];
                for (int i = 0; i < dicStringCount; i++)
                {
                    int stringLen = (int)serialized[index++];
                    string dicString = Encoding.UTF8.GetString(serialized, index, stringLen);
                    index += stringLen;
                    readerSession.Add(i, dicString);
                }
            }

            IXmlDictionary staticDictionary = MyDictionary.Instance;
            using (XmlDictionaryReader reader = XmlDictionaryReader.CreateBinaryReader(serialized, index, serialized.Length - index, staticDictionary, XmlDictionaryReaderQuotas.Max, readerSession))
            {
                DataContractSerializer dcs = new DataContractSerializer(typeof(T));
                return (T)dcs.ReadObject(reader);
            }
        }
        public static void Test(bool useStaticDictionary, bool useSessionDictionary)
        {
            var data = new List<Person> { 
                new Person { Name = "Scooby Doo", Age = 10, DateOfBirth = new DateTime(2000, 12, 12), dicString = MyDictionary.Instance.GetDicString("DicStringValue1") },
                new Person { Name = "Shaggy", Age = 19, DateOfBirth = new DateTime(1991, 10, 2), dicString = MyDictionary.Instance.GetDicString("DicStringValue2") },
                new Person { Name = "Fred", Age = 22, DateOfBirth = new DateTime(1988, 8, 5), dicString = MyDictionary.Instance.GetDicString("DicStringValue1") },
                new Person { Name = "Daphne", Age = 21, DateOfBirth = new DateTime(1989, 4, 7), dicString = MyDictionary.Instance.GetDicString("DicStringValue2") },
                new Person { Name = "Velma", Age = 20, DateOfBirth = new DateTime(1990, 6, 30), dicString = MyDictionary.Instance.GetDicString("DicStringValue1") },
            };

            var sw = new Stopwatch();
            sw.Start();
            byte[] serialized = Serialize(useStaticDictionary, useSessionDictionary, data);
            sw.Stop();
            Console.WriteLine("To {0} bytes by binary DCS\tin {1:00:000}ms", serialized.Length, sw.Elapsed.TotalMilliseconds);

            sw.Reset();
            List<Person> res;
            sw.Start();
            res = Deserialize<List<Person>>(useStaticDictionary, useSessionDictionary
                , serialized);
            sw.Stop();
            Console.WriteLine("From bytes to object by DCS\tin {0:00:000}ms", sw.Elapsed.TotalMilliseconds);
        }
    }

    public class Post_fb906fa1_8ce9_412e_a16a_5d4a2a0c2ac5
    {
        [DataContract(Name = "StringExample")]
        public class StringExample
        {
            public StringExample(string str1, string str2)
            {
                String1 = str1;
                String2 = str2;
            }
            [DataMember]
            public string String1 { get; private set; }
            [DataMember]
            public string String2 { get; private set; }
        }

        [ServiceContract]
        public interface IStringExampleService
        {
            [WebGet(ResponseFormat = WebMessageFormat.Json)]
            StringExample GetStringExample(int i);
        }

        public class StringExampleService : IStringExampleService
        {
            public StringExample GetStringExample(int i)
            {
                if (i == 0) //The contents of this method are unimportant...
                {
                    return new StringExample("Yes", "(s)he can");
                }
                else if (i == 1)
                {
                    throw new Exception("NO STRING FOR YOU");
                }
                else
                {
                    throw new FaultException<int>(234, "No String For You Either");
                }
            }
        }

        [DataContract(Name = "JsonError")]
        public class JsonError
        {
            [DataMember]
            public string Message { get; set; }
            [DataMember]
            public int FaultCode { get; set; }
        }

        public class MyErrorHandler : IErrorHandler
        {
            public bool HandleError(Exception error)
            {
                //Tell the system that we handle all errors here.
                return true;
            }

            public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
            {
                FaultException<int> fe = error as FaultException<int>;
                JsonError msErrObject = new JsonError { FaultCode = fe == null ? -1 : fe.Detail, Message = error.Message };

                //The fault to be returned
                fault = Message.CreateMessage(version, "", msErrObject, new DataContractJsonSerializer(msErrObject.GetType()));

                // tell WCF to use JSON encoding rather than default XML
                WebBodyFormatMessageProperty wbf = new WebBodyFormatMessageProperty(WebContentFormat.Json);

                // Add the formatter to the fault
                fault.Properties.Add(WebBodyFormatMessageProperty.Name, wbf);

                //Modify response
                HttpResponseMessageProperty rmp = new HttpResponseMessageProperty();

                // return custom error code, 400.
                rmp.StatusCode = System.Net.HttpStatusCode.BadRequest;
                rmp.StatusDescription = "Bad request";

                //Mark the jsonerror and json content
                rmp.Headers[HttpResponseHeader.ContentType] = "application/json";
                rmp.Headers["jsonerror"] = "true";

                //Add to fault
                fault.Properties.Add(HttpResponseMessageProperty.Name, rmp);
            }
        }

        public class MyEndpointBehavior : IEndpointBehavior
        {
            public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
            {
            }

            public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
            {
            }

            public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
            {
                endpointDispatcher.ChannelDispatcher.ErrorHandlers.Clear();
                endpointDispatcher.ChannelDispatcher.ErrorHandlers.Add(new MyErrorHandler());
            }

            public void Validate(ServiceEndpoint endpoint)
            {
            }
        }

        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(StringExampleService), new Uri(baseAddress));
            ServiceEndpoint endpoint = host.AddServiceEndpoint(typeof(IStringExampleService), new WebHttpBinding(), "");
            endpoint.Behaviors.Add(new WebScriptEnablingBehavior());
            endpoint.Behaviors.Add(new MyEndpointBehavior());
            host.Open();
            Console.WriteLine("Host opened");

            Util.SendRequest(baseAddress + "/GetStringExample?i=0", "GET", null, null);
            Util.SendRequest(baseAddress + "/GetStringExample?i=1", "GET", null, null);
            Util.SendRequest(baseAddress + "/GetStringExample?i=2", "GET", null, null);

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_0a264ab7_96ad_43e5_b7c4_5eeb7ea45dad
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            string Echo(string text);
        }
        public class Service : ITest
        {
            public string Echo(string text) { return text; }
        }
        [ServiceContract]
        public class RestService
        {
            [WebInvoke(UriTemplate = "*")]
            public Stream Process(Stream input)
            {
                string response = @"<s:Envelope
        xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">
    <s:Body>
        <EchoResponse xmlns=""http://tempuri.org/"">
            <EchoResult>Hello</EchoResult>
        </EchoResponse>
    </s:Body>
</s:Envelope>";
                WebOperationContext.Current.OutgoingResponse.ContentType = "text/xml";
                WebOperationContext.Current.OutgoingResponse.Headers["X-BadHeader"] = "ab⌂cd";
                return new MemoryStream(Encoding.UTF8.GetBytes(response));
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            WebServiceHost host = new WebServiceHost(typeof(RestService), new Uri(baseAddress));
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(new BasicHttpBinding(), new EndpointAddress(baseAddress + "/"));
            ITest proxy = factory.CreateChannel();

            Console.WriteLine(proxy.Echo("Hello"));

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_1c5c31c4_fc2b_43d4_9593_48f48d2f157f
    {
        [ServiceContract]
        public interface ITest
        {
            [WebInvoke]
            XElement ProcessXml(XElement input);
        }
        public class Service : ITest
        {
            public XElement ProcessXml(XElement input)
            {
                int productId = 0;
                string productName = null;
                List<XElement> children = new List<XElement>(input.Elements());
                foreach (XElement child in children)
                {
                    if (child.Name == "ProductId")
                    {
                        productId = int.Parse(child.DescendantNodes().Where(x => x is XText).Select(x => (XText)x).First().Value);
                    }
                    else if (child.Name == "ProductName")
                    {
                        productName = child.DescendantNodes().Where(x => x is XText).Select(x => (XText)x).First().Value;
                        if (DoesProductExist(productId, productName))
                        {
                            child.AddAfterSelf(XElement.Parse("<IsAvailable>true</IsAvailable>"));
                        }
                        else
                        {
                            child.AddAfterSelf(XElement.Parse("<IsAvailable>false</IsAvailable>"));
                        }
                    }
                }

                return input;
            }

            private bool DoesProductExist(int productId, string productName)
            {
                return productId == 1;
            }
        }

        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), new WebHttpBinding(), "").Behaviors.Add(new WebHttpBehavior());
            host.Open();
            Console.WriteLine("Host opened");

            string xml = "<Products><ProductId>1</ProductId><ProductName>Test</ProductName><ProductId>2</ProductId><ProductName>Test2</ProductName></Products>";

            Util.SendRequest(baseAddress + "/ProcessXml", "POST", "text/xml", xml);

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_0c7571eb_2a37_4325_b9d5_4dfa3228c9e7
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
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), new BasicHttpBinding(), "");
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(new WSHttpBinding(SecurityMode.None), new EndpointAddress(baseAddress));
            ITest proxy = factory.CreateChannel();

            Console.WriteLine(proxy.Add(3, 5));

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_b5cc0b7b_4cd8_4b0b_9d82_491acec6295a
    {
        [DataContract]
        public class QueryColumn
        {
            [DataMember]
            public string Name;
        }

        [KnownType(typeof(QueryColumnCollection))]
        [CollectionDataContract]
        public abstract class QueryColumnCollectionBase : List<QueryColumn>
        {
            //...
        }

        [CollectionDataContract]
        public class QueryColumnCollection : QueryColumnCollectionBase
        {
            //...
        }

        public class WcfUtil
        {
            public static Type[] GetKnownTypes(ICustomAttributeProvider provider)
            {
                return new Type[] { };
            }
        }

        [DataContract]
        public class FaultDetail
        {
            [DataMember]
            public string Description;
        }

        [ServiceContract]
        public interface ICrudService
        {
            [OperationContract]
            [FaultContract(typeof(FaultDetail))]
            [ServiceKnownType("GetKnownTypes", typeof(WcfUtil))]
            ICollection<object> GetAll(string targetTypeName, QueryColumnCollectionBase queryColumnCollection);
        }

        public class CrudService : ICrudService
        {
            public ICollection<object> GetAll(string targetTypeName, QueryColumnCollectionBase queryColumnCollection)
            {
                var colNames = from col in queryColumnCollection
                               select col.Name;
                return new List<object>(colNames);
            }
        }

        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(CrudService), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ICrudService), new BasicHttpBinding(), "");
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ICrudService> factory = new ChannelFactory<ICrudService>(new BasicHttpBinding(), new EndpointAddress(baseAddress));
            ICrudService proxy = factory.CreateChannel();

            QueryColumnCollection input = new QueryColumnCollection
            {
                new QueryColumn { Name = "Col1" },
                new QueryColumn { Name = "Col2" },
            };

            try
            {
                var result = proxy.GetAll("target", input);
                foreach (var item in result)
                {
                    Console.WriteLine(item);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_7d930220_95a2_48f1_8b08_ddabb0ee317e
    {
        public static void Test()
        {
            Configuration currentConfig = ConfigurationManager.OpenExeConfiguration(Assembly.GetEntryAssembly().Location);
            ServiceModelSectionGroup smsg = (ServiceModelSectionGroup)currentConfig.GetSectionGroup("system.serviceModel");
            ServiceBehaviorElement sbe = smsg.Behaviors.ServiceBehaviors["Throttling"];
            ServiceThrottlingElement ste = sbe[typeof(ServiceThrottlingElement)] as ServiceThrottlingElement;
            object serviceThrottlingBehavior = typeof(ServiceThrottlingElement).GetMethod("CreateBehavior", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(ste, null);
            Console.WriteLine(serviceThrottlingBehavior);
        }
    }

    public class Post_c6678f2e_0234_4d47_86d9_3efdc2e6c682
    {
        public static ServiceThrottlingBehavior CreateBehavior()
        {
            ServiceThrottlingBehavior stb = new ServiceThrottlingBehavior
            {
                MaxConcurrentCalls = 100,
                MaxConcurrentInstances = 100,
                MaxConcurrentSessions = 100,
            };

            return stb;
        }
    }

    public class Post_ec4b69b8_6dc7_4091_8f84_4e85174c7fdf
    {
        [DataContract]
        class Contact
        {
            [DataMember]
            internal Guid contactId;
            [DataMember]
            internal bool isFavorite;
            [DataMember(EmitDefaultValue = false)]
            internal string comment;
        }
        public static void Test()
        {
            MemoryStream ms = new MemoryStream();
            DataContractJsonSerializer dcjs = new DataContractJsonSerializer(typeof(Contact));
            Contact instance = new Contact
            {
                isFavorite = true,
                contactId = new Guid("00000000-0000-0000-0006-000080D6D01E"),
                comment = null,
            };
            dcjs.WriteObject(ms, instance);
            Console.WriteLine(Encoding.UTF8.GetString(ms.ToArray()));
        }
    }

    public class Post_f758f5fa_3c68_47ef_b390_e7cbccecf209
    {
        [DataContract]
        public class MyDC
        {
            [DataMember]
            public string str = "The string";
        }
        [MessageContract]
        public class MyMC
        {
            [MessageHeader]
            public MyDC header;
            [MessageBodyMember]
            public MyDC body;
        }
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            MyDC EchoDC(MyDC input);
            [OperationContract]
            MyMC EchoMC(MyMC input);
        }
        [DataContract(Namespace = "", Name = "DC")]
        public class DCReplacement
        {
            [DataMember]
            public string s;
        }
        public class MySurrogate : IDataContractSurrogate
        {
            public object GetCustomDataToExport(Type clrType, Type dataContractType)
            {
                return null;
            }

            public object GetCustomDataToExport(MemberInfo memberInfo, Type dataContractType)
            {
                return null;
            }

            public Type GetDataContractType(Type type)
            {
                if (type == typeof(MyDC))
                {
                    return typeof(DCReplacement);
                }
                else
                {
                    return type;
                }
            }

            public object GetDeserializedObject(object obj, Type targetType)
            {
                DCReplacement replacement = obj as DCReplacement;
                if (replacement != null)
                {
                    Console.WriteLine("On deserialization, replacing replacement with MyDC object");
                    return new MyDC { str = replacement.s };
                }

                return obj;
            }

            public void GetKnownCustomDataTypes(Collection<Type> customDataTypes)
            {
            }

            public object GetObjectToSerialize(object obj, Type targetType)
            {
                MyDC dc = obj as MyDC;
                if (dc != null)
                {
                    Console.WriteLine("Replacing MyDC object with DCReplacement");
                    return new DCReplacement { s = dc.str };
                }
                else
                {
                    return obj;
                }
            }

            public Type GetReferencedTypeOnImport(string typeName, string typeNamespace, object customData)
            {
                return null;
            }

            public System.CodeDom.CodeTypeDeclaration ProcessImportedType(System.CodeDom.CodeTypeDeclaration typeDeclaration, System.CodeDom.CodeCompileUnit compileUnit)
            {
                return null;
            }
        }
        public class Service : ITest
        {
            public MyDC EchoDC(MyDC input)
            {
                return input;
            }
            public MyMC EchoMC(MyMC input)
            {
                return input;
            }
        }
        static void AddSurrogate(ServiceEndpoint endpoint)
        {
            foreach (OperationDescription od in endpoint.Contract.Operations)
            {
                DataContractSerializerOperationBehavior dcsob = od.Behaviors.Find<DataContractSerializerOperationBehavior>();
                if (dcsob != null)
                {
                    dcsob.DataContractSurrogate = new MySurrogate();
                }
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            ServiceEndpoint endpoint = host.AddServiceEndpoint(typeof(ITest), new BasicHttpBinding(), "");
            AddSurrogate(endpoint);

            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(new BasicHttpBinding(), new EndpointAddress(baseAddress));
            AddSurrogate(factory.Endpoint);
            ITest proxy = factory.CreateChannel();

            Console.WriteLine("Calling EchoDC (will cause serialization/deserialization of MyDC by itself)");
            Console.WriteLine(proxy.EchoDC(new MyDC { str = "EchoDC" }));
            Console.WriteLine();
            Console.WriteLine(proxy.EchoMC(new MyMC { header = new MyDC { str = "Header" }, body = new MyDC { str = "Body" } }));
            Console.WriteLine("Calling EchoDC (will cause serialization/deserialization of MyDC within a message contract)");

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.WriteLine("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_ce3c69b6_ccb0_4455_8cb0_96523fa3dc20
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            string Process(string param);
            [OperationContract(Name = "ProcessWithDefault")]
            string Process();
        }
        public class Service : ITest
        {
            public string Process(string param)
            {
                return param;
            }

            public string Process()
            {
                return Process("Default parameter");
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), new BasicHttpBinding(), "");
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(new BasicHttpBinding(), new EndpointAddress(baseAddress));
            ITest proxy = factory.CreateChannel();

            Console.WriteLine(proxy.Process());
            Console.WriteLine(proxy.Process("hello world"));

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_68bf71a7_c121_4da9_aeb8_87f4359e8a37
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            int Add(int x, int y);
        }
        [ServiceContract]
        public interface ITest2
        {
            [OperationContract]
            int Multiply(int x, int y);
        }
        public class Service : ITest
        {
            public int Add(int x, int y) { return x + y; }
        }
        [ServiceContract]
        public class ServiceWithServiceContract
        {
            [OperationContract]
            public string Echo(string text) { return text; }
        }
        public class Service2 : IDisposable, ITest
        {
            public void Dispose() { }
            public int Add(int x, int y) { return x + y; }
        }
        public class ServiceWithTwoContracts : ITest, ITest2
        {
            public int Add(int x, int y) { return x + y; }
            public int Multiply(int x, int y) { return x * y; }
        }
        public class NonService { }
        static bool ContainsServiceContractAttribute(Type type)
        {
            ServiceContractAttribute[] sca = (ServiceContractAttribute[])type.GetCustomAttributes(typeof(ServiceContractAttribute), true);
            return (sca != null && sca.Length > 0);
        }
        static IEnumerable<Type> GetServiceContract(Type serviceType)
        {
            if (ContainsServiceContractAttribute(serviceType))
            {
                yield return serviceType;
            }

            foreach (Type implementedInterface in serviceType.GetInterfaces())
            {
                if (ContainsServiceContractAttribute(implementedInterface))
                {
                    yield return implementedInterface;
                }
            }
        }
        public static void Test()
        {
            Type[] serviceTypes = new Type[] { typeof(Service), typeof(ServiceWithServiceContract), typeof(Service2), typeof(ServiceWithTwoContracts), typeof(NonService) };
            foreach (Type type in serviceTypes)
            {
                List<Type> scTypes = new List<Type>(GetServiceContract(type));
                Console.Write("[SC] type(s) for {0}:", type.Name);
                for (int i = 0; i < scTypes.Count; i++)
                {
                    Console.Write(" " + scTypes[i].Name);
                }
                Console.WriteLine();
            }
        }
    }

    public class Post_a8861e3b_7b12_4882_939e_645ccc067c46
    {
        [DataContract]
        public class QueueTest
        {
            [DataMember]
            public QueueTestCommands Command;

            public QueueTest()
            {
                Command = new QueueTestCommands();
            }
        }

        [DataContract]
        public class QueueTestCommands
        {
            public enum Commands
            {
                None,
            }

            [DataMember]
            public Queue<Commands> Command;

            public QueueTestCommands()
            {
                Command = new Queue<Commands>();
            }
        }
        [ServiceContract]
        public interface IVTService
        {

            [OperationContract]
            QueueTest CreateQueueTest();
        }
        [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
        public class VTService : IVTService
        {

            public QueueTest CreateQueueTest()
            {
                QueueTest queueTest = new QueueTest();
                queueTest.Command = new QueueTestCommands();
                queueTest.Command.Command.Enqueue(QueueTestCommands.Commands.None);

                return queueTest;
            }
        }

        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(VTService), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(IVTService), new BasicHttpBinding(), "");
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<IVTService> factory = new ChannelFactory<IVTService>(new BasicHttpBinding(), new EndpointAddress(baseAddress));
            IVTService proxy = factory.CreateChannel();

            var result = proxy.CreateQueueTest();
            Console.WriteLine(result);
            Console.WriteLine(result.Command);
            Console.WriteLine(result.Command.Command);
            Console.WriteLine(result.Command.Command.Count);

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_4be9c7e0_67d8_4ab6_9e0f_7dd3fb31bd2a
    {
        [DataContract]
        public class MyDC
        {
            [DataMember]
            public string str = "The string";
        }
        [ServiceContract]
        public interface ITest
        {
            [WebGet(ResponseFormat = WebMessageFormat.Json)]
            MyDC GetDC(string str);
        }
        public class Service : ITest
        {
            public MyDC GetDC(string str)
            {
                return new MyDC { str = str };
            }
        }
        public class MyInspector : IDispatchMessageInspector, IEndpointBehavior
        {
            public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
            {
            }

            public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
            {
            }

            public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
            {
                endpointDispatcher.DispatchRuntime.MessageInspectors.Add(this);
            }

            public void Validate(ServiceEndpoint endpoint)
            {
            }

            public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
            {
                return null;
            }

            public void BeforeSendReply(ref Message reply, object correlationState)
            {
                XmlDictionaryReader originalReplyBodyReader = reply.GetReaderAtBodyContents();
                MemoryStream ms = new MemoryStream();
                XmlDictionaryWriter jsonWriter = JsonReaderWriterFactory.CreateJsonWriter(ms);
                jsonWriter.WriteNode(originalReplyBodyReader, true);
                jsonWriter.Flush();
                string jsonReply = Encoding.UTF8.GetString(ms.ToArray());
                Console.WriteLine("Original reply: {0}", jsonReply);

                // change jsonReply
                jsonReply = jsonReply.Replace("foo bar", "i was modified");

                byte[] jsonReplyBytes = Encoding.UTF8.GetBytes(jsonReply);
                XmlDictionaryReader newReplyBodyReader = JsonReaderWriterFactory.CreateJsonReader(jsonReplyBytes, XmlDictionaryReaderQuotas.Max);
                Message newReply = Message.CreateMessage(reply.Version, null, newReplyBodyReader);
                newReply.Properties.CopyProperties(reply.Properties);
                newReply.Headers.CopyHeadersFrom(reply);
                reply = newReply;
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            ServiceEndpoint endpoint = host.AddServiceEndpoint(typeof(ITest), new WebHttpBinding(), "");
            endpoint.Behaviors.Add(new WebHttpBehavior());
            endpoint.Behaviors.Add(new MyInspector());
            host.Open();
            Console.WriteLine("Host opened");

            WebClient c = new WebClient();
            Console.WriteLine(c.DownloadString(baseAddress + "/GetDC?str=foo%20bar"));

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_9526c85b_93ac_4f89_bcc8_2d917a184d91
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            string EchoString(string text);
            [OperationContract]
            int Add(int x, int y);
        }
        public class Service : ITest
        {
            public string EchoString(string text)
            {
                return text;
            }

            public int Add(int x, int y)
            {
                return x + y;
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), new BasicHttpBinding(), "");
            host.AddServiceEndpoint(typeof(ITest), new WSHttpBinding(), "ws");
            host.Open();
            Console.WriteLine("Host opened");

            foreach (ServiceEndpoint endpoint in host.Description.Endpoints)
            {
                Console.WriteLine("Endpoint address: {0}", endpoint.Address.Uri);
            }

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_6bd86eb5_808a_4119_8c18_dae0ffae94b8
    {
        [DataContract]
        public class GenericItem
        {
            [DataMember]
            public object ObjectProperty { get; set; }
            [DataMember]
            public string ModelTypename { get; set; }
        }

        [DataContract]
        public class Customer
        {
            [DataMember]
            public string Name;
        }

        public static void Test()
        {
            Console.WriteLine(typeof(GenericItem).AssemblyQualifiedName);
            Console.WriteLine(typeof(Customer).AssemblyQualifiedName);

            MemoryStream ms = new MemoryStream();
            DataContractSerializer dcs = new DataContractSerializer(typeof(GenericItem));
            GenericItem item = new GenericItem
            {
                ModelTypename = "Customer",
                ObjectProperty = new Customer { Name = "John" },
            };
            try
            {
                dcs.WriteObject(ms, item);
                Console.WriteLine(Encoding.UTF8.GetString(ms.ToArray()));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    public class Post_449229f5_e04f_4d65_8fc7_2486941bd8aa
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            string[] GetAllHttpHeaders();
        }
        public class Service : ITest
        {
            public string[] GetAllHttpHeaders()
            {
                HttpRequestMessageProperty prop = (HttpRequestMessageProperty)OperationContext.Current.IncomingMessageProperties[HttpRequestMessageProperty.Name];
                List<string> headers = new List<string>();
                foreach (var header in prop.Headers.AllKeys)
                {
                    headers.Add(string.Format("{0}: {1}", header, prop.Headers[header]));
                }

                return headers.ToArray();
            }
        }
        public class MyClient : ClientBase<ITest>, ITest
        {
            public MyClient(Binding binding, EndpointAddress address) : base(binding, address) { }

            public string[] GetAllHttpHeaders()
            {
                return this.Channel.GetAllHttpHeaders();
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), new BasicHttpBinding(), "");
            host.Open();
            Console.WriteLine("Host opened");

            MyClient client = new MyClient(new BasicHttpBinding(), new EndpointAddress(baseAddress));

            var headers = client.GetAllHttpHeaders();
            foreach (var header in headers)
            {
                Console.WriteLine(header);
            }

            Console.WriteLine();
            Console.WriteLine("Now setting the headers before the request");
            Console.WriteLine();

            using (new OperationContextScope(client.InnerChannel))
            {
                HttpRequestMessageProperty prop = new HttpRequestMessageProperty();
                OperationContext.Current.OutgoingMessageProperties.Add(HttpRequestMessageProperty.Name, prop);

                prop.Headers["Version"] = "1.0";
                prop.Headers["OnBehalfOf"] = "";
                prop.Headers["Role"] = "1";
                prop.Headers["EndPoint"] = "001";
                prop.Headers["ServiceId"] = "001";
                prop.Headers["DateTime"] = "09/25/07";
                prop.Headers["ClientApplication"] = "XYZ001";
                prop.Headers["TraceWebMethod"] = "false";
                prop.Headers["ClientTouchPoint"] = "ClientTouchPoint";
                prop.Headers["ChannelInfo"] = "ChannelInfo";
                prop.Headers["Environment"] = "WINOS";
                prop.Headers["http_tmsamlsessionticket"] = "";

                headers = client.GetAllHttpHeaders();
                foreach (var header in headers)
                {
                    Console.WriteLine(header);
                }
            }

            client.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_7c35fe3f_969c_4658_ac25_6f8b51a124a1
    {
        [DataContract(Name = "ApplicationInfo", Namespace = "http://schemas.datacontract.org/2004/07/ConnectListenManager")]
        public class ApplicationInfo
        {
            [DataMember]
            public string Dba
            {
                get;
                set;
            }

            [DataMember]
            public string BusinessName
            {
                get;
                set;
            }
        }

        [ServiceContract]
        public interface IConnectListenService
        {
            [OperationContract]
            [WebGet]
            int Add();

            [OperationContract]
            [WebInvoke(Method = "POST",
                UriTemplate = "ProcessPostRequest",
                ResponseFormat = WebMessageFormat.Xml,
                RequestFormat = WebMessageFormat.Xml,
                BodyStyle = WebMessageBodyStyle.Wrapped
                )]
            string ProcessPostRequest();

            [OperationContract]
            [WebInvoke(Method = "POST",
                UriTemplate = "InvokeSignEvent",
                ResponseFormat = WebMessageFormat.Xml,
                RequestFormat = WebMessageFormat.Xml,
                BodyStyle = WebMessageBodyStyle.Wrapped
                )]
            string InvokeSignEvent();

            [OperationContract]
            [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
            void SubmitApplication(ApplicationInfo ApplicationInfo, int accessCode);
        }

        public class Service : IConnectListenService
        {
            public int Add()
            {
                return 1;
            }

            public string ProcessPostRequest()
            {
                return "";
            }

            public string InvokeSignEvent()
            {
                return "";
            }

            public void SubmitApplication(ApplicationInfo ApplicationInfo, int accessCode)
            {
                Console.WriteLine("appInfo[bn={0},dba={1}] - accessCode={2}", ApplicationInfo.BusinessName, ApplicationInfo.Dba, accessCode);
            }
        }

        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));

            WSHttpBinding wsBinding = new WSHttpBinding();
            wsBinding.Security.Mode = SecurityMode.None;
            wsBinding.ReaderQuotas.MaxArrayLength = 50000000;

            WebHttpBinding webBinding = new WebHttpBinding();
            webBinding.MaxBufferPoolSize = 15000000;
            webBinding.MaxBufferSize = 15000000;
            webBinding.MaxReceivedMessageSize = 15000000;
            webBinding.ReaderQuotas.MaxArrayLength = 15000000;
            webBinding.ReaderQuotas.MaxBytesPerRead = 15000000;
            webBinding.ReaderQuotas.MaxDepth = 15000000;
            webBinding.ReaderQuotas.MaxStringContentLength = 15000000;
            webBinding.ReaderQuotas.MaxNameTableCharCount = 10000000;

            WebHttpBehavior webHttp = new WebHttpBehavior();

            host.AddServiceEndpoint(typeof(IConnectListenService), wsBinding, "soap");
            host.AddServiceEndpoint(typeof(IConnectListenService), webBinding, "").Behaviors.Add(webHttp);

            host.Description.Behaviors.Add(new ServiceMetadataBehavior { HttpGetEnabled = true });
            ServiceDebugBehavior sdb = host.Description.Behaviors.Find<ServiceDebugBehavior>();
            if (sdb == null)
            {
                sdb = new ServiceDebugBehavior();
                host.Description.Behaviors.Add(sdb);
            }
            sdb.IncludeExceptionDetailInFaults = true;

            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<IConnectListenService> factory = new ChannelFactory<IConnectListenService>(wsBinding, new EndpointAddress(baseAddress + "/soap"));
            IConnectListenService proxy = factory.CreateChannel();
            proxy.SubmitApplication(new ApplicationInfo { BusinessName = "TEST BusinessName", Dba = "TEST DBA" }, 1234);

            string requestFromPost = @"<?xml version=""1.0"" encoding=""utf-8""?>
<s:Envelope xmlns:s=""http://www.w3.org/2003/05/soap-envelope"" xmlns:a=""http://www.w3.org/2005/08/addressing"">
 <s:Header>
  <a:Action s:mustUnderstand=""1"">http://tempuri.org/IConnectListenService/SubmitApplication</a:Action>
  <a:To s:mustUnderstand=""1"">BASEADDRESS/soap</a:To>
 </s:Header>
 <s:Body>
  <SubmitApplication xmlns=""http://tempuri.org/"">
   <ApplicationInfo xmlns:b=""http://schemas.datacontract.org/2004/07/ConnectListenManager"" xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"">
    <b:Dba>Test DBA</b:Dba>
    <b:BusinessName>TEST BusinessName</b:BusinessName>
   </ApplicationInfo>
   <accessCode>1234</accessCode>
  </SubmitApplication>
 </s:Body>
</s:Envelope>";
            requestFromPost = requestFromPost.Replace("BASEADDRESS", baseAddress);
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(baseAddress + "/soap");
            req.Method = "POST";
            req.ContentType = "application/soap+xml; charset=utf-8";
            Stream reqStream = req.GetRequestStream();
            byte[] reqBytes = Encoding.UTF8.GetBytes(requestFromPost);
            reqStream.Write(reqBytes, 0, reqBytes.Length);
            reqStream.Close();

            HttpWebResponse resp;
            try
            {
                resp = (HttpWebResponse)req.GetResponse();
            }
            catch (WebException e)
            {
                resp = (HttpWebResponse)e.Response;
            }

            Console.WriteLine("HTTP/{0} {1} {2}", resp.ProtocolVersion, (int)resp.StatusCode, resp.StatusDescription);
            foreach (var header in resp.Headers.AllKeys)
            {
                Console.WriteLine("{0}: {1}", header, resp.Headers[header]);
            }
            Console.WriteLine();
            Console.WriteLine(new StreamReader(resp.GetResponseStream()).ReadToEnd());

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_b989ac84_0366_4be5_808b_98b61f2d915b
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            string GenerateQuotaExceededException();
            [OperationContract]
            string GenerateTimeoutException();
        }
        [ServiceContract]
        public interface ITestRest
        {
            [WebInvoke(UriTemplate = "*")]
            string Return503();
        }
        public class Service : ITest, ITestRest
        {
            public string GenerateQuotaExceededException()
            {
                return new string('r', 70000);
            }

            public string GenerateTimeoutException()
            {
                Thread.Sleep(30000);
                return "hello";
            }

            public string Return503()
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.ServiceUnavailable;
                return "";
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), new BasicHttpBinding(), "soap");
            host.AddServiceEndpoint(typeof(ITestRest), new WebHttpBinding(), "rest").Behaviors.Add(new WebHttpBehavior());
            host.Open();
            Console.WriteLine("Host opened");

            BasicHttpBinding clientBinding = new BasicHttpBinding();
            clientBinding.SendTimeout = TimeSpan.FromSeconds(10);
            clientBinding.ReceiveTimeout = TimeSpan.FromSeconds(10);

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(clientBinding, new EndpointAddress(baseAddress + "/soap"));
            ITest proxy = factory.CreateChannel();

            try
            {
                proxy.GenerateQuotaExceededException();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            ((IClientChannel)proxy).Close();
            factory.Close();

            factory = new ChannelFactory<ITest>(clientBinding, new EndpointAddress(baseAddress + "/rest"));
            proxy = factory.CreateChannel();

            try
            {
                proxy.GenerateQuotaExceededException(); // any operation will do
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_55d9c995_c857_4ae4_b1d2_55cd1039bc35
    {
        [ServiceContract]
        public interface ITest
        {
            [WebInvoke(ResponseFormat = WebMessageFormat.Json)]
            string EchoJson(string text);
            [WebInvoke(ResponseFormat = WebMessageFormat.Xml)]
            string EchoXml(string text);
        }
        public class Service : ITest
        {
            public string EchoJson(string text) { return text; }
            public string EchoXml(string text) { return text; }
        }
        class MyInspector : IEndpointBehavior, IClientMessageInspector
        {
            public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
            {
            }

            public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
            {
                clientRuntime.MessageInspectors.Add(this);
            }

            public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
            {
            }

            public void Validate(ServiceEndpoint endpoint)
            {
            }

            public void AfterReceiveReply(ref Message reply, object correlationState)
            {
                if (reply.Properties.ContainsKey(WebBodyFormatMessageProperty.Name))
                {
                    WebBodyFormatMessageProperty prop = (WebBodyFormatMessageProperty)reply.Properties[WebBodyFormatMessageProperty.Name];
                    Console.WriteLine("Reply format: {0}", prop.Format);
                }
            }

            public object BeforeSendRequest(ref Message request, IClientChannel channel)
            {
                return null;
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), new WebHttpBinding(), "").Behaviors.Add(new WebHttpBehavior());
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(new WebHttpBinding(), new EndpointAddress(baseAddress));
            factory.Endpoint.Behaviors.Add(new WebHttpBehavior());
            factory.Endpoint.Behaviors.Add(new MyInspector());
            ITest proxy = factory.CreateChannel();

            Console.WriteLine("Calling EchoJson");
            proxy.EchoJson("EchoJson");

            Console.WriteLine();
            Console.WriteLine("Calling EchoXml");
            proxy.EchoXml("EchoXml");

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_2d18fcd4_1b0e_4b8d_b479_924ba580721
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            [MyParamInspector]
            int Add(int x, int y);
            [OperationContract]
            [MyParamInspector]
            void AddWithOut(int x, int y, out int result);
        }
        public class Service : ITest
        {
            public int Add(int x, int y)
            {
                return x + y;
            }

            public void AddWithOut(int x, int y, out int result)
            {
                result = x + y;
            }
        }
        public class MyParamInspectorAttribute : Attribute, IOperationBehavior, IParameterInspector
        {
            public void AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters)
            {
            }

            public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
            {
                clientOperation.ParameterInspectors.Add(this);
            }

            public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
            {
            }

            public void Validate(OperationDescription operationDescription)
            {
            }

            public void AfterCall(string operationName, object[] outputs, object returnValue, object correlationState)
            {
                Console.WriteLine("Operation {0}, return={1}, outputs.Length={2}", operationName, returnValue, outputs.Length);
                for (int i = 0; i < outputs.Length; i++)
                {
                    if (outputs[i] is int)
                    {
                        outputs[i] = 100 + (int)outputs[i];
                    }
                }
            }

            public object BeforeCall(string operationName, object[] inputs)
            {
                return null;
            }
        }

        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), new BasicHttpBinding(), "");
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(new BasicHttpBinding(), new EndpointAddress(baseAddress));
            ITest proxy = factory.CreateChannel();

            Console.WriteLine(proxy.Add(3, 5));
            int result;
            proxy.AddWithOut(3, 5, out result);
            Console.WriteLine(result);

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_11c8efcf_d0ed_4333_9c8e_45e4edaa717c
    {
        [DataContract(IsReference = true, Namespace = "")]
        public class Category
        {
            [DataMember]
            public string CategoryName;
            [DataMember]
            public List<Article> Articles = new List<Article>();
        }
        [DataContract(IsReference = true, Namespace = "")]
        public class Article
        {
            [DataMember]
            public string ArticleName;
            [DataMember]
            public Category Category;
        }
        [ServiceContract(Namespace = "")]
        public interface ITest
        {
            [OperationContract]
            Category MyFunction(Category category);
        }
        public class Service : ITest
        {
            public Category MyFunction(Category category)
            {
                return category;
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), new BasicHttpBinding(), "");
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(new BasicHttpBinding(), new EndpointAddress(baseAddress));
            ITest proxy = factory.CreateChannel();

            Article article = new Article { ArticleName = "AName" };
            Category category = new Category { CategoryName = "cname" };
            category.Articles.Add(article);
            article.Category = category;

            Console.WriteLine(proxy.MyFunction(category));

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_19b65bd3_7cd7_4b90_9fa8_b410bc641e09
    {
        [XmlRoot(ElementName = "MyXmlType", Namespace = "")]
        public class MyXmlType
        {
            [XmlAttribute]
            public string str = "The string";
            [XmlAttribute]
            public string str2 = "Other value";
        }
        [MessageContract(IsWrapped = false)]
        public class RequestContract
        {
            [MessageBodyMember]
            public MyXmlType value;
        }
        [MessageContract(IsWrapped = false)]
        public class ResponseContract
        {
            [MessageBodyMember]
            public MyXmlType value;
        }
        public class Constants
        {
            public const string OperationAction = "http://tempuri.org/MyOperation";
            public const string OperationReplyAction = "http://tempuri.org/MyOperationResponse";
        }
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            [XmlSerializerFormat]
            ResponseContract Process(RequestContract input);
            [OperationContract(Action = Constants.OperationAction, ReplyAction = Constants.OperationReplyAction)]
            Message UntypedMessage(Message input);
        }
        public class Service : ITest
        {
            public ResponseContract Process(RequestContract input)
            {
                return new ResponseContract { value = input.value };
            }
            public Message UntypedMessage(Message input)
            {
                MessageBuffer buffer = input.CreateBufferedCopy(int.MaxValue);
                Message result = buffer.CreateMessage();
                result.Headers.Action = Constants.OperationReplyAction;
                return result;
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), new BasicHttpBinding(), "");
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(new BasicHttpBinding(), new EndpointAddress(baseAddress));
            ITest proxy = factory.CreateChannel();

            Console.WriteLine(proxy.Process(new RequestContract { value = new MyXmlType() }));

            string messageXml = @"<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">
  <s:Body myAttribute=""value"">
    <foo>bar</foo>
  </s:Body>
</s:Envelope>";
            Message input = Message.CreateMessage(XmlReader.Create(new MemoryStream(Encoding.UTF8.GetBytes(messageXml))), int.MaxValue, MessageVersion.Soap11);
            input.Headers.Action = Constants.OperationAction;
            Console.WriteLine(proxy.UntypedMessage(input));

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_d964c49e_9fb6_47a7_88f6_4adfe025bf8a
    {
        [ServiceContract]
        public interface INotification
        {
            [OperationContract(IsOneWay = true)]
            void NewMessage(string message);
        }
        [ServiceContract(CallbackContract = typeof(INotification))]
        public interface ISubscription
        {
            [OperationContract]
            void Connect(string name);
            [OperationContract]
            void Disconnect(string name);
            [OperationContract]
            void SendMessage(string name, string message);
        }
        [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
        public class SubscriptionService : ISubscription
        {
            Dictionary<string, INotification> connectedClients = new Dictionary<string, INotification>();
            public void Connect(string name)
            {
                connectedClients[name] = OperationContext.Current.GetCallbackChannel<INotification>();
                SendMessage(name, name + " connected");
            }

            public void Disconnect(string name)
            {
                connectedClients.Remove(name);
                SendMessage(name, name + " disconnected");
            }

            public void SendMessage(string name, string message)
            {
                foreach (var clientName in connectedClients.Keys)
                {
                    if (clientName != name)
                    {
                        INotification client = connectedClients[clientName];
                        client.NewMessage("[" + name + "]: " + message);
                    }
                }
            }
        }
        public class CallbackHandler : INotification
        {
            string name;
            public CallbackHandler(string name)
            {
                this.name = name;
            }
            public void NewMessage(string message)
            {
                Console.WriteLine("[On client {0}]: {1}", this.name, message);
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(SubscriptionService), new Uri(baseAddress));
            WSDualHttpBinding binding = new WSDualHttpBinding();
            binding.ClientBaseAddress = new Uri("http://" + Environment.MachineName + ":8008/Client");
            host.AddServiceEndpoint(typeof(ISubscription), binding, "");
            host.Open();
            Console.WriteLine("Host opened");

            CallbackHandler handler1 = new CallbackHandler("Client1");
            CallbackHandler handler2 = new CallbackHandler("Client2");
            CallbackHandler handler3 = new CallbackHandler("Client3");

            DuplexChannelFactory<ISubscription> factory1 = new DuplexChannelFactory<ISubscription>(new InstanceContext(handler1), binding, new EndpointAddress(baseAddress));
            DuplexChannelFactory<ISubscription> factory2 = new DuplexChannelFactory<ISubscription>(new InstanceContext(handler2), binding, new EndpointAddress(baseAddress));
            DuplexChannelFactory<ISubscription> factory3 = new DuplexChannelFactory<ISubscription>(new InstanceContext(handler3), binding, new EndpointAddress(baseAddress));

            ISubscription client1 = factory1.CreateChannel();
            ISubscription client2 = factory2.CreateChannel();
            ISubscription client3 = factory3.CreateChannel();

            client1.Connect("Client1");
            client2.Connect("Client2");
            client1.SendMessage("Client1", "I'm sending a message");
            client3.Connect("Client3");
            client2.Disconnect("Client2");

            Console.WriteLine("Press ENTER to close...");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_046edadf_1a94_47c6_af4a_ffa5c3f5f7c1
    {
        [ServiceContract]
        [ServiceKnownType(typeof(Cat))]
        [ServiceKnownType(typeof(Dog))]
        public interface ISampleService
        {
            [OperationContract]
            [WebGet]
            string GetData(ServiceRequest serviceRequest);
        }

        public class Service : ISampleService
        {
            public string GetData(ServiceRequest serviceRequest)
            {
                return string.Format("request.Name={0}, animal={1}", serviceRequest.Name, serviceRequest.Animal);
            }
        }

        [DataContract(Name = "ServiceRequest", Namespace = "#SampleWcfService.Contracts")]
        public class ServiceRequest
        {
            [DataMember]
            public string Name { get; set; }

            [DataMember]
            public Animal Animal { get; set; }
        }

        [KnownType(typeof(Cat))]
        [KnownType(typeof(Dog))]
        [DataContract(Name = "Animal", Namespace = "#SampleWcfService.Contracts")]
        public class Animal
        {

        }

        [DataContract(Name = "Cat", Namespace = "#SampleWcfService.Contracts")]
        public class Cat : Animal
        {
            [DataMember]
            public string Color { get; set; }
        }

        [DataContract(Name = "Dog", Namespace = "#SampleWcfService.Contracts")]
        public class Dog : Animal
        {
            [DataMember]
            public string Breed { get; set; }
        }

        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ISampleService), new WebHttpBinding(), "").Behaviors.Add(new WebScriptEnablingBehavior());
            host.Open();
            Console.WriteLine("Host opened");

            DataContractJsonSerializer dcjs = new DataContractJsonSerializer(typeof(ServiceRequest));
            MemoryStream ms = new MemoryStream();
            dcjs.WriteObject(ms, new ServiceRequest { Name = "Abc", Animal = new Cat { Color = "Black" } });
            string json = Encoding.UTF8.GetString(ms.ToArray());
            Console.WriteLine(json);
            Console.WriteLine(Uri.EscapeDataString(json));

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(baseAddress + "/GetData?serviceRequest=" + Uri.EscapeDataString(json));
            HttpWebResponse resp;
            try
            {
                resp = (HttpWebResponse)req.GetResponse();
            }
            catch (WebException e)
            {
                resp = (HttpWebResponse)e.Response;
            }
            Console.WriteLine("HTTP/{0} {1} {2}", resp.ProtocolVersion, (int)resp.StatusCode, resp.StatusDescription);
            foreach (var header in resp.Headers.AllKeys)
            {
                Console.WriteLine("{0}: {1}", header, resp.Headers[header]);
            }
            if (resp.ContentLength > 0)
            {
                Console.WriteLine(new StreamReader(resp.GetResponseStream()).ReadToEnd());
            }

            Console.WriteLine("Press ENTER to close...");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_0ba45516_e89a_4bb8_9692_2037927c6259
    {
        [DataContract]
        public class MyTypeWithBinaryProperties
        {
            [DataMember]
            public byte[] binary1;
            [DataMember]
            public byte[] binary2;
            [DataMember]
            public byte[] binary3;
        }
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            MyTypeWithBinaryProperties Echo(MyTypeWithBinaryProperties input);
        }
        public class Service : ITest
        {
            public MyTypeWithBinaryProperties Echo(MyTypeWithBinaryProperties input)
            {
                return input;
            }
        }
        static Binding GetBinding()
        {
            BasicHttpBinding result = new BasicHttpBinding();
            result.MessageEncoding = WSMessageEncoding.Mtom;
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

            MyTypeWithBinaryProperties input = new MyTypeWithBinaryProperties();
            input.binary1 = new byte[1000];
            input.binary2 = new byte[1000];
            input.binary3 = new byte[1000];
            for (int i = 0; i < 1000; i++)
            {
                if ((i % 80) == 79)
                {
                    input.binary1[i] = (byte)'\n';
                    input.binary2[i] = (byte)'\n';
                    input.binary3[i] = (byte)'\n';
                }
                else
                {
                    input.binary1[i] = (byte)'1';
                    input.binary2[i] = (byte)'2';
                    input.binary3[i] = (byte)'3';
                }
            }

            Console.WriteLine(proxy.Echo(input));

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_0da33309_ec07_47d6_8ddb_15290a80977f
    {
        [DataContract]
        public class MyDC
        {
            [DataMember]
            public string str = "The string";
            [OnSerializing]
            public void OnSerializing(StreamingContext ctx)
            {
                Console.WriteLine("MyDC, Before serialization");
            }
            [OnSerialized]
            public void OnSerialized(StreamingContext ctx)
            {
                Console.WriteLine("MyDC, After serialization");
            }
        }
        [DataContract]
        public class WrapperDC
        {
            [DataMember]
            public MyDC dc = new MyDC();
            [OnSerializing]
            public void OnSerializing(StreamingContext ctx)
            {
                Console.WriteLine("WrapperDC, Before serialization");
            }
            [OnSerialized]
            public void OnSerialized(StreamingContext ctx)
            {
                Console.WriteLine("WrapperDC, After serialization");
            }
        }
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            WrapperDC GetValue();
        }
        public class Service : ITest
        {
            public WrapperDC GetValue()
            {
                return new WrapperDC();
            }
        }
        public class MyMessageEncodingBindingElement : MessageEncodingBindingElement
        {
            public MessageEncodingBindingElement inner;
            public MyMessageEncodingBindingElement(MessageEncodingBindingElement inner)
            {
                this.inner = inner;
            }
            public override MessageEncoderFactory CreateMessageEncoderFactory()
            {
                return new MyMessageEncoderFactory(this.inner.CreateMessageEncoderFactory());
            }
            public override MessageVersion MessageVersion
            {
                get { return this.inner.MessageVersion; }
                set { this.inner.MessageVersion = value; }
            }
            public override BindingElement Clone()
            {
                return new MyMessageEncodingBindingElement((MessageEncodingBindingElement)this.inner.Clone());
            }
            public override IChannelListener<TChannel> BuildChannelListener<TChannel>(BindingContext context)
            {
                context.BindingParameters.Add(this);
                return context.BuildInnerChannelListener<TChannel>();
            }
            public override bool CanBuildChannelListener<TChannel>(BindingContext context)
            {
                return context.CanBuildInnerChannelListener<TChannel>();
            }

            class MyMessageEncoderFactory : MessageEncoderFactory
            {
                MessageEncoderFactory inner;
                public MyMessageEncoderFactory(MessageEncoderFactory inner)
                {
                    this.inner = inner;
                }
                public override MessageEncoder Encoder
                {
                    get { return new MyMessageEncoder(this.inner.Encoder); }
                }
                public override MessageVersion MessageVersion
                {
                    get { return this.inner.MessageVersion; }
                }
            }

            class MyMessageEncoder : MessageEncoder
            {
                MessageEncoder inner;
                public MyMessageEncoder(MessageEncoder inner)
                {
                    this.inner = inner;
                }
                public override string ContentType
                {
                    get { return this.inner.ContentType; }
                }
                public override string MediaType
                {
                    get { return this.inner.MediaType; }
                }
                public override MessageVersion MessageVersion
                {
                    get { return this.inner.MessageVersion; }
                }
                public override bool IsContentTypeSupported(string contentType)
                {
                    return this.inner.IsContentTypeSupported(contentType);
                }
                public override Message ReadMessage(ArraySegment<byte> buffer, BufferManager bufferManager, string contentType)
                {
                    return this.inner.ReadMessage(buffer, bufferManager, contentType);
                }
                public override Message ReadMessage(Stream stream, int maxSizeOfHeaders, string contentType)
                {
                    throw new NotImplementedException();
                }
                public override ArraySegment<byte> WriteMessage(Message message, int maxMessageSize, BufferManager bufferManager, int messageOffset)
                {
                    Console.WriteLine("Writing a message; serialization has yet to happen");
                    ArraySegment<byte> result = this.inner.WriteMessage(message, maxMessageSize, bufferManager, messageOffset);
                    Console.WriteLine("Finished writing the message; serialization is finished");
                    return result;
                }
                public override void WriteMessage(Message message, Stream stream)
                {
                    throw new NotImplementedException();
                }
            }
        }
        static Binding GetBinding(bool isService)
        {
            BasicHttpBinding binding = new BasicHttpBinding();
            if (!isService)
            {
                return binding;
            }
            else
            {
                CustomBinding result = new CustomBinding(new BasicHttpBinding());
                for (int i = 0; i < result.Elements.Count; i++)
                {
                    if (result.Elements[i] is MessageEncodingBindingElement)
                    {
                        result.Elements[i] = new MyMessageEncodingBindingElement((MessageEncodingBindingElement)result.Elements[i]);
                    }
                }
                return result;
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), GetBinding(true), "");
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(GetBinding(false), new EndpointAddress(baseAddress));
            ITest proxy = factory.CreateChannel();

            Console.WriteLine(proxy.GetValue());

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_d8bb921a_091a_4fba_bb5c_1e4f099653cc
    {
        public class User
        {
            public string Name { get; set; }
        }
        [ServiceContract]
        public class Service
        {
            [WebGet(UriTemplate = "dummy", BodyStyle = WebMessageBodyStyle.WrappedResponse)]
            [Description("dummy")]
            [OperationContract]
            [return: MessageParameter(Name = "foo")]
            public String dummy()
            {
                User user = new User();
                user.Name = "Ronen";
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.OK;
                return user.Name;
            }
        }

        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service/";
            WebServiceHost host = new WebServiceHost(typeof(Service), new Uri(baseAddress));
            host.Open();
            Console.WriteLine("Host opened");

            WebClient c = new WebClient();
            string result = c.DownloadString(baseAddress + "/dummy");
            Console.WriteLine(result);

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_5871b5df_7b07_459d_a8dd_8c16d8f68ca0
    {
        const string DataServicesNamespace = "http://schemas.microsoft.com/ado/2007/08/dataservices";
        const string MetadataNamespace = "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata";
        [XmlRoot(Namespace = MetadataNamespace, ElementName = "properties")]
        public class ItemContentProperties
        {
            [XmlElement(ElementName = "EntityID", Namespace = DataServicesNamespace)]
            public ItemContentProperty EntityID;
            [XmlElement(ElementName = "Latitude", Namespace = DataServicesNamespace)]
            public ItemContentProperty Latitude;
            [XmlElement(ElementName = "Longitude", Namespace = DataServicesNamespace)]
            public ItemContentProperty Longitude;
            [XmlElement(ElementName = "__Distance", Namespace = DataServicesNamespace)]
            public ItemContentProperty Distance;

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Properties:");
                sb.AppendLine("  EntityID: " + this.EntityID);
                sb.AppendLine("  Latitude: " + this.Latitude);
                sb.AppendLine("  Longitude: " + this.Longitude);
                sb.AppendLine("  Distance: " + this.Distance);

                return sb.ToString();
            }
        }
        public class ItemContentProperty
        {
            [XmlAttribute(AttributeName = "type", Namespace = MetadataNamespace)]
            public string PropertyType;
            [XmlText]
            public string PropertyValue;

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                if (!string.IsNullOrEmpty(this.PropertyType))
                {
                    sb.Append("(type = ");
                    sb.Append(this.PropertyType);
                    sb.Append(") ");
                }

                sb.Append(this.PropertyValue);
                return sb.ToString();
            }
        }
        [ServiceContract]
        public interface ISimulatedBingService
        {
            [WebGet(UriTemplate = "/data/{accessId}/{dataSourceName}/{entityTypeName}")]
            Stream GetBingData(string accessId, string dataSourceName, string entityTypeName);
        }
        public class SimulatedBingService : ISimulatedBingService
        {
            const string BingResponse = @"<?xml version=""1.0"" encoding=""utf-8""?>
<feed xmlns:d=""http://schemas.microsoft.com/ado/2007/08/dataservices"" xmlns:m=""http://schemas.microsoft.com/ado/2007/08/dataservices/metadata"" xmlns=""http://www.w3.org/2005/Atom"">
  <title type=""text""></title>
  <id>uuid:9cbbfa3c-06be-4436-9d4d-13e30cc9f3a1;id=6370</id>
  <rights type=""text"">© 2011 Microsoft and its suppliers.  This API and any results cannot be used or accessed without Microsoft's express written permission.</rights>
  <updated>2011-04-07T15:04:30Z</updated>
  <entry>
    <id>https://spatial.virtualearth.net/REST/v1/data/20181f26d9e94c81acdf9496133d4f23/FourthCoffeeSample/FourthCoffeeShops('-8051')</id>
    <title type=""text""></title>
    <updated>2011-04-07T15:04:30Z</updated>
    <content type=""application/xml"">
      <m:properties>
        <d:EntityID>-8051</d:EntityID>
        <d:Latitude m:type=""Edm.Double"">40.820685</d:Latitude>
        <d:Longitude m:type=""Edm.Double"">-74.295682</d:Longitude>
        <d:__Distance m:type=""Edm.Double"">2.19740735412509</d:__Distance>
      </m:properties>
    </content>
  </entry>
  <entry>
    <id>https://spatial.virtualearth.net/REST/v1/data/20181f26d9e94c81acdf9496133d4f23/FourthCoffeeSample/FourthCoffeeShops('-8174')</id>
    <title type=""text""></title>
    <updated>2011-04-07T15:04:30Z</updated>
    <content type=""application/xml"">
      <m:properties>
        <d:EntityID>-8174</d:EntityID>
        <d:Latitude m:type=""Edm.Double"">40.849513</d:Latitude>
        <d:Longitude m:type=""Edm.Double"">-74.292831</d:Longitude>
        <d:__Distance m:type=""Edm.Double"">2.72010344668636</d:__Distance>
      </m:properties>
    </content>
  </entry>
  <entry>
    <id>https://spatial.virtualearth.net/REST/v1/data/20181f26d9e94c81acdf9496133d4f23/FourthCoffeeSample/FourthCoffeeShops('-8181')</id>
    <title type=""text""></title>
    <updated>2011-04-07T15:04:30Z</updated>
    <content type=""application/xml"">
      <m:properties>
        <d:EntityID>-8181</d:EntityID>
        <d:Latitude m:type=""Edm.Double"">40.851404</d:Latitude>
        <d:Longitude m:type=""Edm.Double"">-74.293720</d:Longitude>
        <d:__Distance m:type=""Edm.Double"">2.81746736218219</d:__Distance>
      </m:properties>
    </content>
  </entry>
</feed>";
            Encoding UTF8 = new UTF8Encoding(false);

            public Stream GetBingData(string accessId, string dataSourceName, string entityTypeName)
            {
                WebOperationContext.Current.OutgoingResponse.ContentType = "application/xml; charset=utf-8";
                MemoryStream ms = new MemoryStream(UTF8.GetBytes(BingResponse));
                return ms;
            }
        }

        [ServiceContract(Namespace = "http://www.w3.org/2005/Atom")]
        [ServiceKnownType(typeof(Atom10FeedFormatter))]
        public interface IBingMapsData
        {
            [OperationContract]
            [WebGet(UriTemplate = "/data/{accessId}/{dataSourceName}/{entityTypeName}?spatialFilter={spatialFilter}&$select={select}&$top={top}&key={bingMapsKey}")]
            SyndicationFeedFormatter QueryByArea(
                string accessId,
                string dataSourceName,
                string entityTypeName,
                string spatialFilter,
                string select,
                string top,
                string bingMapsKey);
        }

        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            WebServiceHost host = new WebServiceHost(typeof(SimulatedBingService), new Uri(baseAddress));
            host.Open();
            Console.WriteLine("Host opened");

            WebChannelFactory<IBingMapsData> factory = new WebChannelFactory<IBingMapsData>(new Uri(baseAddress));
            IBingMapsData bingMapsData = factory.CreateChannel();
            SyndicationFeedFormatter formatter = bingMapsData.QueryByArea("a", "d", "e", "s", "s", "t", "b");

            XmlSerializer propertiesSerializer = new XmlSerializer(typeof(ItemContentProperties));

            foreach (var item in formatter.Feed.Items)
            {
                XmlSyndicationContent xmlContent = item.Content as XmlSyndicationContent;
                if (xmlContent != null)
                {
                    ItemContentProperties itemProperties = xmlContent.ReadContent<ItemContentProperties>(propertiesSerializer);
                    Console.WriteLine(itemProperties);
                }
            }

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_53e8a6fe_ba35_4a07_866e_6b1cdb5dcbaf
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
                if (MyEvent != null)
                {
                    this.MyEvent(this, new EventArgs());
                }

                return x + y;
            }

            public event EventHandler<EventArgs> MyEvent;
        }
        static Binding GetBinding()
        {
            BasicHttpBinding result = new BasicHttpBinding();
            return result;
        }
        public class MyInstanceProvider : IEndpointBehavior, IInstanceProvider
        {
            public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
            {
            }

            public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
            {
            }

            public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
            {
                endpointDispatcher.DispatchRuntime.InstanceProvider = this;
            }

            public void Validate(ServiceEndpoint endpoint)
            {
            }

            public object GetInstance(InstanceContext instanceContext, Message message)
            {
                Service service = new Service();
                service.MyEvent += new EventHandler<EventArgs>(service_MyEvent);
                return service;
            }

            void service_MyEvent(object sender, EventArgs e)
            {
                Console.WriteLine("In service_MyEvent");
            }

            public object GetInstance(InstanceContext instanceContext)
            {
                return this.GetInstance(instanceContext, null);
            }

            public void ReleaseInstance(InstanceContext instanceContext, object instance)
            {
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            ServiceEndpoint endpoint = host.AddServiceEndpoint(typeof(ITest), GetBinding(), "");
            endpoint.Behaviors.Add(new MyInstanceProvider());
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(GetBinding(), new EndpointAddress(baseAddress));
            ITest proxy = factory.CreateChannel();

            Console.WriteLine(proxy.Add(3, 5));

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_ad3a3415_19ee_4a12_b25f_45f6406f873e
    {
        [ServiceContract]
        public interface ITest
        {
            [WebGet]
            [MyInspector]
            string EchoString(string text);
            [WebGet]
            [MyInspector]
            int Add(int x, int y);
        }
        public class Service : ITest
        {
            public string EchoString(string text)
            {
                return text;
            }

            public int Add(int x, int y)
            {
                return x + y;
            }
        }
        public class MyInspectorAttribute : Attribute, IOperationBehavior, IParameterInspector
        {
            public void AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters)
            {
            }

            public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
            {
            }

            public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
            {
                dispatchOperation.ParameterInspectors.Add(this);
            }

            public void Validate(OperationDescription operationDescription)
            {
            }

            public void AfterCall(string operationName, object[] outputs, object returnValue, object correlationState)
            {
                Console.WriteLine("Operation {0} returned: result = {1}", operationName, returnValue);
            }

            public object BeforeCall(string operationName, object[] inputs)
            {
                Console.WriteLine("Calling {0} with the following parameters:", operationName);
                for (int i = 0; i < inputs.Length; i++)
                {
                    Console.WriteLine("  [{0}] = {1}", i, inputs[i]);
                }

                return null;
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            WebServiceHost host = new WebServiceHost(typeof(Service), new Uri(baseAddress));
            host.Open();
            Console.WriteLine("Host opened");

            WebClient c = new WebClient();
            Console.WriteLine(c.DownloadString(baseAddress + "/Add?x=4&y=9"));
            Console.WriteLine(c.DownloadString(baseAddress + "/EchoString?text=hello%20world"));

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_6c5d0fb4_a2ac_4c0a_ad02_782d34cf5f46
    {
        [DataContract]
        public class Resource
        {
            [DataMember]
            public string Description { get; set; }
        }
        [DataContract]
        public class GetResourcesFaultContract
        {
            [DataMember]
            public string Description { get; set; }
        }
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            [FaultContract(typeof(GetResourcesFaultContract))]
            [WebGet(BodyStyle = WebMessageBodyStyle.WrappedRequest,
              ResponseFormat = WebMessageFormat.Json)]
            List<Resource> getResources();
        }
        public class Service : ITest
        {
            public List<Resource> getResources()
            {
                throw new WebFaultException<GetResourcesFaultContract>(new GetResourcesFaultContract { Description = "This is my error" }, HttpStatusCode.BadRequest);
            }
        }
        public static void SendRequest(string uri)
        {
            string responseBody = null;

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(uri);
            req.Method = "GET";
            HttpWebResponse resp;
            try
            {
                resp = (HttpWebResponse)req.GetResponse();
            }
            catch (WebException e)
            {
                resp = (HttpWebResponse)e.Response;
            }

            Console.WriteLine("HTTP/{0} {1} {2}", resp.ProtocolVersion, (int)resp.StatusCode, resp.StatusDescription);
            foreach (string headerName in resp.Headers.AllKeys)
            {
                Console.WriteLine("{0}: {1}", headerName, resp.Headers[headerName]);
            }

            Console.WriteLine();
            Stream respStream = resp.GetResponseStream();
            responseBody = new StreamReader(respStream).ReadToEnd();
            Console.WriteLine(responseBody);
            Console.WriteLine();
            Console.WriteLine("  *-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*  ");
            Console.WriteLine();
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), new WebHttpBinding(), "").Behaviors.Add(new WebHttpBehavior());
            host.Open();
            Console.WriteLine("Host opened");

            SendRequest(baseAddress + "/getResources");

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    // http://stackoverflow.com/q/5999249/751090
    public class StackOverflow_5999249_751090
    {
        [ServiceContract(Name = "ITest", Namespace = "")]
        public interface ITest
        {
            [OperationContract]
            string Echo(string path);
        }

        public class Service : ITest
        {
            public string Echo(string path) { return path; }
        }

        [ServiceContract(Name = "ITest", Namespace = "")]
        public interface ITestClient_Wrong
        {
            [OperationContract]
            IAsyncResult BeginEcho(string path, AsyncCallback callback, object state);
            string EndEcho(IAsyncResult asyncResult);
        }

        [ServiceContract(Name = "ITest", Namespace = "")]
        public interface ITestClient_Correct
        {
            [OperationContract(AsyncPattern = true)]
            IAsyncResult BeginEcho(string path, AsyncCallback callback, object state);
            string EndEcho(IAsyncResult asyncResult);
        }

        static void PrintException(Exception e)
        {
            int indent = 2;
            while (e != null)
            {
                for (int i = 0; i < indent; i++)
                {
                    Console.Write(' ');
                }

                Console.WriteLine("{0}: {1}", e.GetType().FullName, e.Message);
                indent += 2;
                e = e.InnerException;
            }
        }

        public static void Test()
        {
            string baseAddress = "net.pipe://localhost/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), new NetNamedPipeBinding(), "");
            host.Open();
            Console.WriteLine("Host opened");

            AutoResetEvent evt = new AutoResetEvent(false);

            Console.WriteLine("Correct");
            ChannelFactory<ITestClient_Correct> factory1 = new ChannelFactory<ITestClient_Correct>(new NetNamedPipeBinding(), new EndpointAddress(baseAddress));
            ITestClient_Correct proxy1 = factory1.CreateChannel();
            proxy1.BeginEcho("Hello", delegate(IAsyncResult ar)
            {
                Console.WriteLine("Result from correct: {0}", proxy1.EndEcho(ar));
                evt.Set();
            }, null);
            evt.WaitOne();

            Console.WriteLine("Wrong");
            ChannelFactory<ITestClient_Wrong> factory2 = new ChannelFactory<ITestClient_Wrong>(new NetNamedPipeBinding(), new EndpointAddress(baseAddress));
            ITestClient_Wrong proxy2 = factory2.CreateChannel();
            try
            {
                proxy2.BeginEcho("Hello", delegate(IAsyncResult ar)
                {
                    try
                    {
                        Console.WriteLine("Result from wrong: {0}", proxy2.EndEcho(ar));
                    }
                    catch (Exception e)
                    {
                        PrintException(e);
                    }
                    evt.Set();
                }, null);
                evt.WaitOne();
            }
            catch (Exception e2)
            {
                PrintException(e2);
            }

            Console.WriteLine("Press ENTER to close");
            Console.ReadLine();
            host.Close();
        }
    }

    //http://stackoverflow.com/q/6009099/751090
    public class StackOverflow_6009099_751090
    {
        const string XmlFile = @"<?xml version=""1.0"" encoding=""utf-8""?>
<locations>
  <location id=""0"">
    <name>Mariborsko Pohorje</name>
    <temperature>5</temperature>
    <wind>26</wind>
    <weather_text_SI>oblačno</weather_text_SI>
    <visibility></visibility>
    <latitude>46.4527</latitude>
    <longitude>15.334</longitude>
    <elevation>1517</elevation>
  </location>
</locations>";

        [XmlRoot("locations")]
        public class SnowParkList
        {
            [XmlElement("location")]
            public List<SnowPark> Locations { get; set; }
        }

        [XmlRootAttribute("locations")]
        public class SnowPark
        {
            public SnowPark() { }
            private int id;
            [XmlAttribute("id")]
            public int Id { get { return id; } set { id = value; } }
            private string name;
            [XmlElement("name")]
            public string Name { get { return name; } set { name = value; } }
            private int temperature;
            [XmlElement("temperature")]
            public int Temperature { get { return temperature; } set { temperature = value; } }
            private int wind;
            [XmlElement("wind")]
            public int Wind { get { return wind; } set { wind = value; } }
            private string weatherText;
            [XmlElement("weather_text_SI")]
            public string WeatherText { get { return weatherText; } set { weatherText = value; } }
            private double latitude;
            [XmlElement("latitude")]
            public double Latitude { get { return latitude; } set { latitude = value; } }
            private double longitude;
            [XmlElement("longitude")]
            public double Longitude { get { return longitude; } set { longitude = value; } }
            private int elevation;
            [XmlElement("elevation")]
            public int Elevation { get { return elevation; } set { elevation = value; } }
        }

        public static void Test()
        {
            File.WriteAllText(@"file.xml", XmlFile);
            XmlSerializer deserializer = new XmlSerializer(typeof(SnowParkList));
            TextReader textReader = new StreamReader(@"file.xml");
            SnowParkList movies;
            movies = (SnowParkList)deserializer.Deserialize(textReader);
            textReader.Close();
            Console.WriteLine(movies.Locations.Count);
        }
    }

    public class Post_33631bf7_ebc1_46f1_bc92_868dc3cfb0ec
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
                var sba = OperationContext.Current.Host.Description.Behaviors.Find<ServiceBehaviorAttribute>();
                Console.WriteLine("instace context mode of this service is = " + sba.InstanceContextMode);
                Console.WriteLine("concurrency mode of this service is = " + sba.ConcurrencyMode);

                return x + y;
            }
        }
        static Binding GetBinding()
        {
            BasicHttpBinding result = new BasicHttpBinding();
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

            Console.WriteLine(proxy.Add(3, 5));

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    //http://stackoverflow.com/q/6045852/751090
    public class StackOverflow_6045852_751090
    {
        public class MyRequest
        {
            public string Name { get; set; }
        }

        public static void Test()
        {
            Dictionary<int, MyRequest> request = new Dictionary<int, MyRequest>();
            for (int i = 0; i < 1000; i++) { request.Add(i, new MyRequest() { Name = i.ToString() }); }
            var ids = request.Keys.ToList();
            Parallel.For(
                0,
                ids.Count,
                (t) =>
                {
                    var id = ids[t];
                    var b = request[id];
                    lock (b)
                    {
                        if (b.Name == 4.ToString()) { Thread.Sleep(10000); }

                        Console.WriteLine(b.Name);
                    }
                });

            Console.WriteLine("done");
            Console.Read();
        }
    }

    // http://stackoverflow.com/q/6057279/751090
    public class StackOverflow_6057279_751090
    {
        [MessageContract(WrapperName = "TestResponse", WrapperNamespace = "http://tempuri.org/")]
        public class TestResponse
        {
            [MessageBodyMember(Namespace = "")]
            public string TestResult;
        }
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            string Test(string text);
            [OperationContract]
            TestResponse GetTest();
        }
        public class Service : ITest
        {
            public string Test(string text)
            {
                return text;
            }

            public TestResponse GetTest()
            {
                return new TestResponse { TestResult = "hi Test" };
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), new BasicHttpBinding(), "");
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(new BasicHttpBinding(), new EndpointAddress(baseAddress));
            ITest proxy = factory.CreateChannel();

            Console.WriteLine(proxy.Test("hi Test"));
            Console.WriteLine(proxy.GetTest());

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    // http://stackoverflow.com/q/6072850/751090
    public class StackOverflow_6072850_751090
    {
        public struct Point
        {
            public int X { get; set; }
            public int Y { get; set; }

            public override string ToString()
            {
                return string.Format("Point[X={0},Y={1}]", this.X, this.Y);
            }
        }

        public class Map
        {
            public int ID { get; set; }
            public Point? PointA { get; set; }

            public override string ToString()
            {
                return string.Format("Map[ID={0}, PointA={1}]", this.ID, this.PointA == null ? "<<null>>" : this.PointA.ToString());
            }
        }

        public static void Test()
        {
            Map map = new Map { ID = 1, PointA = new Point { X = 2, Y = 3 } };
            JavaScriptSerializer jss = new JavaScriptSerializer();
            string json = jss.Serialize(map);
            Console.WriteLine(json);

            json = "{ \"ID\":1, \"PointA\": { \"__type\": \"" + typeof(Point).FullName + "\", \"X\": 2, \"Y\": 3} }";
            Console.WriteLine(json);

            try
            {
                object obj = jss.Deserialize(json, typeof(Map));
                Console.WriteLine(obj);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    // http://stackoverflow.com/q/6073581/751090
    public class StackOverflow_6073581_751090
    {
        [ServiceContract]
        public interface ITest
        {
            [WebGet(UriTemplate = "/API/{id}")]
            string Get(string id);
        }
        public class Service : ITest
        {
            public string Get(string id)
            {
                return id;
            }
        }
        public class MyBehavior : WebHttpBehavior
        {
            protected override WebHttpDispatchOperationSelector GetOperationSelector(ServiceEndpoint endpoint)
            {
                return new MySelector(endpoint);
            }

            class MySelector : WebHttpDispatchOperationSelector
            {
                public MySelector(ServiceEndpoint endpoint) : base(endpoint) { }

                public override UriTemplate GetUriTemplate(string operationName)
                {
                    UriTemplate result = base.GetUriTemplate(operationName);
                    result = new UriTemplate(result.ToString(), true, result.Defaults);
                    //result.IgnoreTrailingSlash = true;
                    return result;
                }
                //protected override string SelectOperation(ref Message message, out bool uriMatched)
                //{
                //    string result = base.SelectOperation(ref message, out uriMatched);
                //    if (!uriMatched)
                //    {
                //        string address = message.Headers.To.AbsoluteUri;
                //        if (address.EndsWith("/"))
                //        {
                //            message.Headers.To = new Uri(address.Substring(0, address.Length - 1));
                //        }

                //        result = base.SelectOperation(ref message, out uriMatched);
                //    }

                //    return result;
                //}
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), new WebHttpBinding(), "").Behaviors.Add(new MyBehavior());
            host.Open();
            Console.WriteLine("Host opened");

            WebClient c = new WebClient();
            Console.WriteLine(c.DownloadString(baseAddress + "/API/2"));
            Console.WriteLine(c.DownloadString(baseAddress + "/API/2/"));

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    // http://codereview.stackexchange.com/q/2533/4430
    public class CodeReview_StackExchange_2533_4430
    {
        static bool isInside(int pointX, int pointY, int rectX, int rectY, int rectWidth, int rectHeight)
        {
            return
                (rectX <= pointX && pointX <= rectX + rectWidth) &&
                (rectY <= pointY && pointY <= rectY + rectHeight);
        }

        static bool checkCollide(int x, int y, int oWidth, int oHeight, int x2, int y2, int o2Width, int o2Height)
        {
            bool collide =
                isInside(x, y, x2, y2, x2 + o2Width, y2 + o2Height) ||
                isInside(x + oWidth, y, x2, y2, x2 + o2Width, y2 + o2Height) ||
                isInside(x, y + oHeight, x2, y2, x2 + o2Width, y2 + o2Height) ||
                isInside(x + oWidth, y + oHeight, x2, y2, x2 + o2Width, y2 + o2Height);

            return collide;
        }

        public static void Test()
        {
            Console.WriteLine(checkCollide(2, 2, 4, 4, 1, 3, 2, 2));
            Console.WriteLine(checkCollide(1, 3, 2, 2, 2, 2, 4, 4));
        }
    }

    public class Post_a866abd2_bdc2_4d30_8bbc_2ce46df38dc4
    {
        public static void Test()
        {
            string xml = @"<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">
  <s:Body xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
    <Header xmlns=""http://www.test1.com"">
      <applicationID>1234</applicationID>
    </Header>
    <GetMatchRequest xmlns=""http://www.tempuri.org"">test</GetMatchRequest>
  </s:Body>
</s:Envelope>";
            Message message = Message.CreateMessage(XmlReader.Create(new StringReader(xml)), int.MaxValue, MessageVersion.Soap11);
            Console.WriteLine(message);
            XmlDocument bodyDoc = new XmlDocument();
            MemoryStream ms = new MemoryStream();
            XmlWriter w = XmlWriter.Create(ms, new XmlWriterSettings { Indent = true, IndentChars = "  ", OmitXmlDeclaration = true });
            XmlDictionaryReader bodyReader = message.GetReaderAtBodyContents();
            w.WriteStartElement("s", "Body", "http://schemas.xmlsoap.org/soap/envelope/");
            while (bodyReader.NodeType != XmlNodeType.EndElement && bodyReader.LocalName != "Body" && bodyReader.NamespaceURI != "http://schemas.xmlsoap.org/soap/envelope/")
            {
                if (bodyReader.NodeType != XmlNodeType.Whitespace)
                {
                    w.WriteNode(bodyReader, true);
                }
                else
                {
                    bodyReader.Read(); // ignore whitespace; maintain if you want
                }
            }
            w.WriteEndElement();
            w.Flush();
            Console.WriteLine(Encoding.UTF8.GetString(ms.ToArray()));
            ms.Position = 0;
            XmlDocument doc = new XmlDocument();
            doc.Load(ms);
            Console.WriteLine(doc.DocumentElement.OuterXml);
        }
    }

    // http://stackoverflow.com/q/6092243/751090
    public class StackOverflow_6092243_751090
    {
        static decimal CountDecimalPlaces(decimal dec)
        {
            int[] bits = Decimal.GetBits(dec);
            int exponent = bits[3] >> 16;
            int result = exponent;
            long lowDecimal = bits[0] | (bits[1] >> 8);
            while ((lowDecimal % 10) == 0)
            {
                result--;
                lowDecimal /= 10;
            }

            return result;
        }

        public static void Test()
        {
            Console.WriteLine(CountDecimalPlaces(1.6m));
            Console.WriteLine(CountDecimalPlaces(1.600m));
            Console.WriteLine(CountDecimalPlaces(decimal.MaxValue));
            Console.WriteLine(CountDecimalPlaces(1m * 0.00025m));
            Console.WriteLine(CountDecimalPlaces(4000m * 0.00025m));
        }
    }

    // http://stackoverflow.com/q/6045579/751090
    public class StackOverflow_6045579_751090
    {
        static AutoResetEvent serverStarted = new AutoResetEvent(false);
        public static void Test()
        {
            new Thread(new ThreadStart(StartServer)).Start();
            serverStarted.WaitOne();
            SendRequest();
        }
        static void StartServer()
        {
            TcpListener serverSocket = new TcpListener(IPAddress.Any, 8000);
            TcpClient clientSocket = null;
            serverSocket.Start();
            serverStarted.Set();
            clientSocket = serverSocket.AcceptTcpClient();
            var clientStream = clientSocket.GetStream();
            byte[] bytesFrom = new byte[10025];
            int bytesRead = clientStream.Read(bytesFrom, 0, (int)clientSocket.ReceiveBufferSize);
            string dataFromClient = Encoding.UTF8.GetString(bytesFrom, 0, bytesRead);
            Console.WriteLine(" >> Data from client - " + dataFromClient);
            string serverResponse = @"HTTP/1.1 200 OK
Content-Length: 0
Set-Cookie: x-main=Yvi4723B?Nk5zuPPCZ0?66eGEI5kVnOM; path=/; domain=.amazon.com; expires=Wed May 25 13:20:05 2011 GMT
Set-Cookie:atmain=4|WzNBbofyvkTvUaivgd9aS0Tzy0V2MJoUy+gJVcZ4szMhpt+gEyrRyLfUEodtZBKdn9vyJQJ3szoLgO12SUrB5XYqJ/tggOPurUepz5qsd6eg2V6vfbE0X1HQKp03xVkYLPEH5MDhmkMevQgkv36FyY+zA6HN5LzlM92+4kGu6wHeIILZ5+y/dtoYU/uORs1hC9hEo5iwP8Mljg4hDx7b/g==; path=/; domain=.amazon.com; expires=Wed May 25 13:20:05 2011 GMT; secure
Set-Cookie: ubid-main=182-3549292-6045052; path=/; domain=.amazon.com; expires=Wed May 25 13:20:05 2011 GMT
Connection: close
Date: Mon, 23 May 2011 06:56:18 GMT

";
            Byte[] sendBytes = Encoding.UTF8.GetBytes(serverResponse);
            clientStream.Write(sendBytes, 0, sendBytes.Length);
            clientStream.Flush();
            clientStream.Close();
            clientSocket.Close();
        }
        public static void SendRequest()
        {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create("http://" + Environment.MachineName + ":8000/foo");
            req.Method = "GET";
            HttpWebResponse resp;
            try
            {
                resp = (HttpWebResponse)req.GetResponse();
            }
            catch (WebException e)
            {
                resp = (HttpWebResponse)e.Response;
            }

            if (resp != null)
            {
                Console.WriteLine("HTTP/{0} {1} {2}", resp.ProtocolVersion, (int)resp.StatusCode, resp.StatusDescription);
                foreach (var headerName in resp.Headers.AllKeys)
                {
                    foreach (var values in resp.Headers.GetValues(headerName))
                    {
                        Console.WriteLine("{0}: {1}", headerName, values);
                    }
                }
            }
        }
    }

    public class Post_b5efb12c_3920_41f7_a478_349fd4607875
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            [WebGet(UriTemplate = "sites?limit={limit}")]
            string DoSomething(int limit);
        }
        public class Service : ITest
        {
            public string DoSomething(int limit)
            {
                return "Limit is " + limit;
            }
        }
        public static void SendGet(string uri)
        {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(uri);
            req.Method = "GET";

            HttpWebResponse resp;
            try
            {
                resp = (HttpWebResponse)req.GetResponse();
            }
            catch (WebException e)
            {
                resp = (HttpWebResponse)e.Response;
            }

            Console.WriteLine("HTTP/{0} {1} {2}", resp.ProtocolVersion, (int)resp.StatusCode, resp.StatusDescription);
            foreach (string headerName in resp.Headers.AllKeys)
            {
                Console.WriteLine("{0}: {1}", headerName, resp.Headers[headerName]);
            }
            Console.WriteLine();
            Stream respStream = resp.GetResponseStream();
            Console.WriteLine(new StreamReader(respStream).ReadToEnd());

            Console.WriteLine();
            Console.WriteLine(" *-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-* ");
            Console.WriteLine();
        }
        class MyQueryStringConverter : QueryStringConverter
        {
            QueryStringConverter originalConverter;
            public MyQueryStringConverter(QueryStringConverter originalConverter)
            {
                this.originalConverter = originalConverter;
            }
            public override object ConvertStringToValue(string parameter, Type parameterType)
            {
                if (parameterType == typeof(int))
                {
                    int result;
                    if (int.TryParse(parameter, out result))
                    {
                        return result;
                    }
                    else
                    {
                        throw new WebFaultException<string>("limit parameter must be an integer", HttpStatusCode.BadRequest);
                    }
                }
                else
                {
                    return base.ConvertStringToValue(parameter, parameterType);
                }
            }
        }
        public class MyWebHttpBehavior : WebHttpBehavior
        {
            protected override QueryStringConverter GetQueryStringConverter(OperationDescription operationDescription)
            {
                return new MyQueryStringConverter(base.GetQueryStringConverter(operationDescription));
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), new BasicHttpBinding(), "soap");
            host.AddServiceEndpoint(typeof(ITest), new WebHttpBinding(), "rest").Behaviors.Add(new MyWebHttpBehavior());
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(new BasicHttpBinding(), new EndpointAddress(baseAddress + "/soap"));
            ITest proxy = factory.CreateChannel();
            Console.WriteLine("Using SOAP: {0}", proxy.DoSomething(1234));
            Console.WriteLine();

            SendGet(baseAddress + "/rest/sites?limit=123");
            SendGet(baseAddress + "/rest/sites?limit=abc");

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    // http://stackoverflow.com/q/5867304/751090
    public class StackOverflow_5867304_751090
    {
        public class Product
        {
            public string Name { get; set; }
            public int Price { get; set; }
        }
        [ServiceContract]
        public interface ITest
        {
            [WebGet(ResponseFormat = WebMessageFormat.Json)]
            List<Product> GetProducts(int size);
        }
        public class Service : ITest
        {
            public List<Product> GetProducts(int size)
            {
                List<Product> result = new List<Product>();
                for (int i = 0; i < size; i++)
                {
                    result.Add(new Product { Name = "Prod " + i, Price = i });
                }
                return result;
            }
        }
        static Binding GetBinding()
        {
            return new WebHttpBinding() { MaxReceivedMessageSize = int.MaxValue };
        }
        static void AddBehavior(ServiceEndpoint endpoint)
        {
            endpoint.Behaviors.Add(new WebHttpBehavior());
            foreach (var operation in endpoint.Contract.Operations)
            {
                DataContractSerializerOperationBehavior dcsob = operation.Behaviors.Find<DataContractSerializerOperationBehavior>();
                if (dcsob != null)
                {
                    dcsob.MaxItemsInObjectGraph = 1000000;
                }
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            ServiceEndpoint endpoint = host.AddServiceEndpoint(typeof(ITest), GetBinding(), "");
            AddBehavior(endpoint);
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(GetBinding(), new EndpointAddress(baseAddress));
            AddBehavior(factory.Endpoint);
            ITest proxy = factory.CreateChannel();
            Console.WriteLine(proxy.GetProducts(100000).Count);

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    // http://stackoverflow.com/q/6203895/751090
    public class StackOverflow_6203895_751090
    {
        [ServiceContract]
        public interface IMySubService1
        {
            [OperationContract]
            int MyOp11(string opnd);
            [OperationContract]
            int MyOp12(string opnd);
        }
        [ServiceContract]
        public interface IMySubService2
        {
            [OperationContract]
            int MyOp21(string opnd);
            [OperationContract]
            int MyOp22(string opnd);
        }
        [ServiceContract]
        public interface IMySubService3
        {
            [OperationContract]
            int MyOp31(string opnd);
            [OperationContract]
            int MyOp32(string opnd);
        }
        [ServiceContract]
        public interface IMyService : IMySubService1, IMySubService2, IMySubService3 { }
        public class Service : IMyService
        {
            public int MyOp11(string opnd)
            {
                return 11;
            }

            public int MyOp12(string opnd)
            {
                return 12;
            }

            public int MyOp21(string opnd)
            {
                return 21;
            }

            public int MyOp22(string opnd)
            {
                return 22;
            }

            public int MyOp31(string opnd)
            {
                return 31;
            }

            public int MyOp32(string opnd)
            {
                return 32;
            }
        }
        static Binding GetBinding()
        {
            return new WSHttpBinding();
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(IMyService), GetBinding(), "");
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<IMySubService1> factory = new ChannelFactory<IMySubService1>(GetBinding(), new EndpointAddress(baseAddress));
            IMySubService1 proxy = factory.CreateChannel();

            Console.WriteLine(proxy.MyOp12("Hello"));

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    // http://stackoverflow.com/q/6237996/751090
    public class StackOverflow_6237996
    {
        [ServiceContract(Name = "ITest")]
        public interface ITest
        {
            [OperationContract]
            int Add(int x, int y);
        }
        [ServiceContract(Name = "ITest")]
        public interface ITestAsync
        {
            [OperationContract(AsyncPattern = true)]
            IAsyncResult BeginAdd(int x, int y, AsyncCallback callback, object userState);
            int EndAdd(IAsyncResult asyncResult);
        }
        public class Service : ITest
        {
            public int Add(int x, int y)
            {
                Thread.Sleep(100);
                return x + y;
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), new BasicHttpBinding(), "");
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITestAsync> factory = new ChannelFactory<ITestAsync>(new BasicHttpBinding(), new EndpointAddress(baseAddress));
            ITestAsync proxy = factory.CreateChannel();

            var tsk1 = Task.Factory.FromAsync<int>(
                proxy.BeginAdd(4, 5, null, null),
                (ar) => proxy.EndAdd(ar));
            var tsk2 = Task.Factory.FromAsync<int>(
                proxy.BeginAdd(7, 8, null, null),
                (ar) => proxy.EndAdd(ar));

            Task.WaitAll(tsk1, tsk2);

            Console.WriteLine("Result 1: {0}", tsk1.Result);
            Console.WriteLine("Result 2: {0}", tsk2.Result);

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    // http://stackoverflow.com/q/6267866/751090
    public class StackOverflow_6267866
    {
        [ServiceContract]
        public interface ITest1
        {
            [WebInvoke(UriTemplate = "")]
            string EchoString(string text);
        }
        [ServiceContract]
        public interface ITest2
        {
            [WebInvoke(UriTemplate = "", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
            int Add(int x, int y);
        }
        public class Service : ITest1, ITest2
        {
            public string EchoString(string text)
            {
                return text;
            }

            public int Add(int x, int y)
            {
                return x + y;
            }
        }
        static void SendPostRequest(string uri, string contentType, string body)
        {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(uri);
            req.Method = "POST";
            req.ContentType = contentType;
            byte[] bodyBytes = Encoding.UTF8.GetBytes(body);
            req.GetRequestStream().Write(bodyBytes, 0, bodyBytes.Length);
            req.GetRequestStream().Close();

            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Console.WriteLine("HTTP/{0} {1} {2}", resp.ProtocolVersion, (int)resp.StatusCode, resp.StatusDescription);
            foreach (string headerName in resp.Headers.AllKeys)
            {
                Console.WriteLine("{0}: {1}", headerName, resp.Headers[headerName]);
            }
            Console.WriteLine();
            Stream respStream = resp.GetResponseStream();
            Console.WriteLine(new StreamReader(respStream).ReadToEnd());
            Console.WriteLine();
            Console.WriteLine("  *-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*  ");
            Console.WriteLine();
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest1), new WebHttpBinding(), "ITest1").Behaviors.Add(new WebHttpBehavior());
            host.AddServiceEndpoint(typeof(ITest2), new WebHttpBinding(), "ITest2").Behaviors.Add(new WebHttpBehavior());
            host.Open();
            Console.WriteLine("Host opened");

            SendPostRequest(baseAddress + "/ITest1", "application/json", "\"hello world\"");
            SendPostRequest(baseAddress + "/ITest1/", "application/json", "\"hello world\"");
            SendPostRequest(baseAddress + "/ITest2", "application/json", "{\"x\":123,\"y\":456}");
            SendPostRequest(baseAddress + "/ITest2/", "application/json", "{\"x\":123,\"y\":456}");

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    // http://stackoverflow.com/q/4968244/751090
    public class StackOverflow_4968244
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
                Console.WriteLine("Request at service: {0}", OperationContext.Current.Host.BaseAddresses[0]);
                if (new Random().Next(3) == 0)
                {
                    throw new InvalidOperationException("Random error");
                }

                return x + y;
            }
        }
        public class Client : ClientBase<ITest>, ITest
        {
            public Client(string address) : base(new BasicHttpBinding(), new EndpointAddress(address)) { }

            public int Add(int x, int y)
            {
                return this.Channel.Add(x, y);
            }
        }
        public class SafeClient : ITest
        {
            List<Client> clients;
            public SafeClient(params Client[] clients)
            {
                this.clients = new List<Client>(clients);
            }

            public int Add(int x, int y)
            {
                foreach (var client in this.clients)
                {
                    try
                    {
                        return client.Add(x, y);
                    }
                    catch (CommunicationException)
                    {
                        Console.WriteLine("Error calling client {0}, retrying with next one", client.Endpoint.Address.Uri);
                    }
                }

                throw new InvalidOperationException("All services seem to be down");
            }
        }

        public static void Test()
        {
            string baseAddress1 = "http://" + Environment.MachineName + ":8000/Service";
            string baseAddress2 = "http://" + Environment.MachineName + ":8001/Service";
            string baseAddress3 = "http://" + Environment.MachineName + ":8002/Service";
            ServiceHost host1 = new ServiceHost(typeof(Service), new Uri(baseAddress1));
            ServiceHost host2 = new ServiceHost(typeof(Service), new Uri(baseAddress2));
            ServiceHost host3 = new ServiceHost(typeof(Service), new Uri(baseAddress3));
            host1.AddServiceEndpoint(typeof(ITest), new BasicHttpBinding(), "");
            host2.AddServiceEndpoint(typeof(ITest), new BasicHttpBinding(), "");
            host3.AddServiceEndpoint(typeof(ITest), new BasicHttpBinding(), "");
            host1.Open();
            host2.Open();
            host3.Open();
            Console.WriteLine("Hosts opened");

            SafeClient safeClient = new SafeClient(
                new Client(baseAddress1),
                new Client(baseAddress2),
                new Client(baseAddress3));
            for (int i = 0; i < 20; i++)
            {
                Console.WriteLine(safeClient.Add(i, 10));
            }

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host1.Close();
            host2.Close();
            host3.Close();
        }
    }

    // http://stackoverflow.com/q/6331246/751090
    public class StackOverflow_6331246
    {
        public static bool IsValid(string propertyName, object propertyValue)
        {
            Console.Write(propertyName + " - ");
            if (propertyValue == null)
            {
                Console.WriteLine("Value is null");
                return false;
            }

            Type propType = propertyValue.GetType();
            if (propType.IsGenericType)
            {
                if (propType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    Type genericParameter = propType.GetGenericArguments()[0];
                    if (genericParameter == typeof(bool))
                    {
                        bool value = (bool)propType.GetProperty("Value").GetValue(propertyValue, null);
                        Console.WriteLine("It's a bool?, returning its value");
                        return value;
                    }
                    if (genericParameter == typeof(bool))
                    {
                        int value = (int)propType.GetProperty("Value").GetValue(propertyValue, null);
                        Console.WriteLine("It's an int? with value = {0}; returning value > 0");
                        return value > 0;
                    }
                    else
                    {
                        Console.WriteLine("It's a Nullable<T> where T is not bool or int, returning false");
                        return false;
                    }
                }
                else
                {
                    Console.WriteLine("A generic type, not Nullable<T>, returning false");
                    return false;
                }
            }
            else
            {
                Console.WriteLine("Not a generic type, returning false");
                return false;
            }
        }
        public static void Test()
        {
            IsValid("bool? true", new bool?(true));
            IsValid("bool? false", new bool?(false));
            IsValid("bool? null", (bool?)null);

            IsValid("int? 1", new int?(1));
            IsValid("int? 0", new int?(0));
            IsValid("int? null", (int?)null);
        }
    }

    // http://stackoverflow.com/q/6332575/751090
    public class StackOverflow_6332575
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
            BasicHttpBinding result = new BasicHttpBinding();
            return result;
        }
        class MyExtension : IExtension<IContextChannel>
        {
            public void Attach(IContextChannel owner)
            {
            }

            public void Detach(IContextChannel owner)
            {
            }

            public Binding Binding { get; set; }
        }
        static void CallProxy(ITest proxy)
        {
            Console.WriteLine(proxy.Add(3, 5));
            MyExtension extension = ((IClientChannel)proxy).Extensions.Find<MyExtension>();
            if (extension != null)
            {
                Console.WriteLine("Binding: {0}", extension.Binding);
            }
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

            ((IClientChannel)proxy).Extensions.Add(new MyExtension { Binding = factory.Endpoint.Binding });

            CallProxy(proxy);

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    // http://stackoverflow.com/q/6339286/751090
    public class StackOverflow_6339286
    {
        [CollectionDataContract(Namespace = "http://tempuri.org/", ItemName = "int")]
        public class MyIntCollection : Collection<int> { }
        [ServiceContract]
        public interface ITest
        {
            [WebInvoke(Method = "PUT", UriTemplate = "users/role/{userID}", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
            [OperationContract]
            bool AssignUserRole(string userID, MyIntCollection roleIDs);
        }
        public class Service : ITest
        {
            public bool AssignUserRole(string userID, MyIntCollection roleIDs)
            {
                return true;
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            WebServiceHost host = new WebServiceHost(typeof(Service), new Uri(baseAddress));
            host.Open();
            Console.WriteLine("Host opened");

            WebChannelFactory<ITest> factory = new WebChannelFactory<ITest>(new Uri(baseAddress));
            ITest proxy = factory.CreateChannel();

            proxy.AssignUserRole("1234", new MyIntCollection { 1, 2, 3, 4 });

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    // http://stackoverflow.com/q/6346646/751090
    public class StackOverflow_6346646
    {
        static void SerializeGuid()
        {
            Console.WriteLine("Serializing Guid[]");
            var guids = new Guid[20000];
            Random rndGen = new Random();
            for (int i = 0; i < guids.Length; i++)
            {
                guids[i] = Guid.NewGuid();
            }
            MemoryStream ms = new MemoryStream();
            Stopwatch watch = new Stopwatch();
            DataContractSerializer dcs = new DataContractSerializer(guids.GetType());
            XmlDictionaryWriter binaryWriter = XmlDictionaryWriter.CreateBinaryWriter(ms);
            watch.Start();
            dcs.WriteObject(binaryWriter, guids);
            binaryWriter.Flush();
            watch.Stop();
            Console.WriteLine("Serialized in {0}ms, total size = {1} bytes", watch.ElapsedMilliseconds, ms.Position);
        }
        static void SerializeInt()
        {
            Console.WriteLine("Serializing int[]");
            var guids = new int[80000]; // new Guid[20000];
            Random rndGen = new Random();
            for (int i = 0; i < guids.Length; i++)
            {
                guids[i] = rndGen.Next(); // Guid.NewGuid();
            }
            MemoryStream ms = new MemoryStream();
            Stopwatch watch = new Stopwatch();
            DataContractSerializer dcs = new DataContractSerializer(guids.GetType());
            XmlDictionaryWriter binaryWriter = XmlDictionaryWriter.CreateBinaryWriter(ms);
            watch.Start();
            dcs.WriteObject(binaryWriter, guids);
            binaryWriter.Flush();
            watch.Stop();
            Console.WriteLine("Serialized in {0}ms, total size = {1} bytes", watch.ElapsedMilliseconds, ms.Position);
        }
        static void SerializeGuidAsByteArray(bool useLinq)
        {
            Console.WriteLine("Serializing Guid[] as byte[], {0}", useLinq ? "using LINQ" : "not using LINQ");
            var guids = new Guid[20000];
            Random rndGen = new Random();
            for (int i = 0; i < guids.Length; i++)
            {
                guids[i] = Guid.NewGuid();
            }

            MemoryStream ms = new MemoryStream();
            Stopwatch watch = new Stopwatch();
            DataContractSerializer dcs = new DataContractSerializer(typeof(byte[]));
            XmlDictionaryWriter binaryWriter = XmlDictionaryWriter.CreateBinaryWriter(ms);
            watch.Start();
            byte[] bytes;
            if (useLinq)
            {
                bytes = guids.SelectMany(x => x.ToByteArray()).ToArray();
            }
            else
            {
                bytes = new byte[guids.Length * 16];
                for (int i = 0; i < guids.Length; i++)
                {
                    byte[] guidBytes = guids[i].ToByteArray();
                    Buffer.BlockCopy(guidBytes, 0, bytes, 16 * i, 16);
                }
            }
            dcs.WriteObject(binaryWriter, bytes);
            binaryWriter.Flush();
            watch.Stop();
            Console.WriteLine("Serialized in {0}ms, total size = {1} bytes", watch.ElapsedMilliseconds, ms.Position);
        }
        public static void Test()
        {
            SerializeGuid();
            SerializeInt();
            SerializeGuidAsByteArray(true);
            SerializeGuidAsByteArray(false);
        }
    }

    // http://stackoverflow.com/q/6399085/751090
    public class StackOverflow_6399085
    {
        [XmlRoot(ElementName = "ServerName", Namespace = "http://schemas.microsoft.com/sqlazure/2010/12/")]
        public class ServerName
        {
            [XmlText]
            public string Name { get; set; }

            public override string ToString()
            {
                return string.Format("ServerName[Name={0}]", this.Name);
            }
        }

        const string XML = "<?xml version=\"1.0\" encoding=\"utf-8\"?><ServerName xmlns=\"http://schemas.microsoft.com/sqlazure/2010/12/\">zpc0fbxur0</ServerName>";

        static void RunWithXmlSerializer()
        {
            XmlSerializer xs = new XmlSerializer(typeof(ServerName));
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(XML));
            ServerName obj = (ServerName)xs.Deserialize(ms);
            Console.WriteLine("Using XML serializer: {0}", obj);
        }

        static void RunWithDataContractSerializer()
        {
            DataContractSerializer dcs = new DataContractSerializer(typeof(string), "ServerName", "http://schemas.microsoft.com/sqlazure/2010/12/");
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(XML));
            string name = (string)dcs.ReadObject(ms);
            Console.WriteLine("Using DataContractSerializer (different name): {0}", name);
        }

        [ServiceContract(Namespace = "http://schemas.microsoft.com/sqlazure/2010/12/")]
        public class SqlAzureRestService
        {
            [WebGet]
            public Stream GetServerName()
            {
                MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(XML));
                WebOperationContext.Current.OutgoingResponse.ContentType = "text/xml";
                return ms;
            }
            [WebGet(BodyStyle = WebMessageBodyStyle.Bare)]
            [return: MessageParameter(Name = "ServerName")]
            public string GetServerName2()
            {
                return "zpc0fbxur0";
            }
        }

        [ServiceContract(Namespace = "http://schemas.microsoft.com/sqlazure/2010/12/")]
        public interface IServerNameClient
        {
            [WebGet(BodyStyle = WebMessageBodyStyle.Bare)]
            [XmlSerializerFormat]
            ServerName GetServerName();
        }

        static void RunWithWCFRestClient()
        {
            // Setting up the mock service
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            WebServiceHost host = new WebServiceHost(typeof(SqlAzureRestService), new Uri(baseAddress));
            host.Open();

            WebChannelFactory<IServerNameClient> factory = new WebChannelFactory<IServerNameClient>(new Uri(baseAddress));
            IServerNameClient proxy = factory.CreateChannel();
            var name = proxy.GetServerName();
            Console.WriteLine("Using WCF REST client: {0}", name);
        }

        public static void Test()
        {
            RunWithXmlSerializer();
            RunWithDataContractSerializer();
            RunWithWCFRestClient();
        }
    }

    class MyTest
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            string Echo(string text);
            [OperationContract]
            bool InOutRef(string value, ref int i, out double dbl);
            [OperationContract]
            void Process();
        }
        public class Service : ITest
        {
            public string Echo(string text)
            {
                return text;
            }

            public bool InOutRef(string value, ref int i, out double dbl)
            {
                i++;
                return double.TryParse(value, out dbl);
            }

            public void Process() { }
        }
        static Binding GetBinding()
        {
            BasicHttpBinding result = new BasicHttpBinding();
            return result;
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            ServiceEndpoint endpoint = host.AddServiceEndpoint(typeof(ITest), GetBinding(), "");
            host.Open();
            Console.WriteLine("Host opened");

            Console.WriteLine(endpoint.Contract.Operations[2].Messages[1].Body.ReturnValue);

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(GetBinding(), new EndpointAddress(baseAddress));
            ITest proxy = factory.CreateChannel();
            Console.WriteLine(proxy.Echo("foo"));

            int i = 1;
            double dbl;
            Console.WriteLine("{0} - {1} - {2}", proxy.InOutRef("123.45", ref i, out dbl), i, dbl);

            proxy.Process();

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    // http://stackoverflow.com/q/6426553/751090
    public class StackOverflow_6426553
    {
        [ServiceContract]
        public class Service
        {
            [WebGet]
            public string GetString()
            {
                return "hello";
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            WebServiceHost host = new WebServiceHost(typeof(Service), new Uri(baseAddress));
            host.Open();
            Console.WriteLine("Host opened");

            WebClient c = new WebClient();
            Console.WriteLine(c.DownloadString(baseAddress + "/GetString"));

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    // http://stackoverflow.com/q/6449723/751090
    public class StackOverflow_6449723
    {
        [ServiceContract]
        public class Service
        {
            [WebGet(UriTemplate = "*", ResponseFormat = WebMessageFormat.Json)]
            public Stream GetHeaders()
            {
                StringBuilder sb = new StringBuilder();
                foreach (var header in WebOperationContext.Current.IncomingRequest.Headers.AllKeys)
                {
                    sb.AppendLine(string.Format("{0}: {1}", header, Uri.UnescapeDataString(WebOperationContext.Current.IncomingRequest.Headers[header])));
                }
                WebOperationContext.Current.OutgoingResponse.ContentType = "text/plain; charset=utf-8";
                return new MemoryStream(Encoding.UTF8.GetBytes(sb.ToString()));
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            WebServiceHost host = new WebServiceHost(typeof(Service), new Uri(baseAddress));
            host.Open();
            Console.WriteLine("Host opened");

            WebRequest req = WebRequest.Create(baseAddress + "/foo");
            req.Headers.Add("s", Uri.EscapeDataString("АБВ12"));
            req.Headers.Add("username", "user");
            req.Headers.Add("password", "pass");
            WebResponse resp = req.GetResponse();
            StreamReader sr = new StreamReader(resp.GetResponseStream());
            Console.WriteLine(sr.ReadToEnd());

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_e928530d_b161_41b6_90c6_533198b7671f
    {
        public class ExtendedServiceHostFactory : ServiceHostFactory
        {
            protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
            {
                return new ExtendedWebServiceHost(serviceType, baseAddresses);
            }
        }
        public class RequestInspector : IEndpointBehavior, IDispatchMessageInspector
        {
            public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
            {
            }

            public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
            {
            }

            public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
            {
                endpointDispatcher.DispatchRuntime.MessageInspectors.Add(this);
            }

            public void Validate(ServiceEndpoint endpoint)
            {
            }

            public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
            {
                Console.WriteLine("Inside inspector");
                return null;
            }

            public void BeforeSendReply(ref Message reply, object correlationState)
            {
            }
        }
        public class ExtendedWebServiceHost : WebServiceHost
        {
            public ExtendedWebServiceHost(Type serviceType, params Uri[] baseAddresses)
                : base(serviceType, baseAddresses)
            {
            }

            protected override void OnOpening()
            {
                base.OnOpening();
                foreach (ServiceEndpoint endpoint in this.Description.Endpoints)
                {
                    if (!endpoint.IsSystemEndpoint)
                    {
                        endpoint.Behaviors.Add(new RequestInspector());
                    }
                }
            }
        }
        [ServiceContract]
        public interface ITest
        {
            [WebGet]
            int Add(int x, int y);
        }
        public class Service : ITest
        {
            public int Add(int x, int y)
            {
                return x + y;
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ExtendedWebServiceHost host = new ExtendedWebServiceHost(typeof(Service), new Uri(baseAddress));
            host.Open();
            Console.WriteLine("Host opened");

            WebClient c = new WebClient();
            Console.WriteLine(c.DownloadString(baseAddress + "/Add?x=5&y=8"));

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    // http://stackoverflow.com/q/6515309/751090
    public class StackOverflow_6515309
    {
        [DataContract]
        public class Person
        {
            [DataMember(Order = 1)]
            public string Name;
            [DataMember(Order = 2)]
            public int Age;
            [DataMember(Order = 3)]
            public Address Address;
        }
        [DataContract]
        public class Address
        {
            [DataMember(Order = 1)]
            public string Street;
            [DataMember(Order = 2)]
            public string City;
            [DataMember(Order = 3)]
            public string State;
        }
        public static void Test()
        {
            MemoryStream ms = new MemoryStream();
            XmlWriterSettings ws = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  ",
                Encoding = Encoding.UTF8,
            };
            XmlWriter w = XmlWriter.Create(ms, ws);
            DataContractSerializer dcs = new DataContractSerializer(typeof(Person));
            Person person = new Person
            {
                Name = "John",
                Age = 22,
                Address = new Address
                {
                    Street = "1 Main St.",
                    City = "Springfield",
                    State = "ZZ",
                }
            };
            dcs.WriteObject(w, person);
            w.Flush();
            Console.WriteLine("Serialized:");
            Console.WriteLine(Encoding.UTF8.GetString(ms.ToArray()));
            ms.Position = 0;
            XmlReader r = XmlReader.Create(ms);
            Person deserialized = (Person)dcs.ReadObject(r);
            Console.WriteLine(deserialized);
        }
    }

    public class Post_06ae6369_2303_4485_94f6_59d9411309de
    {
        [ServiceContract]
        public class Service
        {
            [WebGet]
            public XElement GetDocument()
            {
                XDocument doc = XDocument.Parse(@"<orders>
  <order id='1'>
    <items>
      <item id='123' quantity='2' price='23.45' />
      <item id='332' quantity='5' price='3.33' />
      <item id='888' quantity='1' price='5.00' />
    </items>
  </order>
</orders>");
                return doc.Root;
            }
            [WebGet]
            public string GetDocumentAsString()
            {
                return this.GetDocument().ToString();
            }
            [WebGet]
            public Stream GetDocumentAsStream()
            {
                WebOperationContext.Current.OutgoingResponse.ContentType = "text/xml";
                return new MemoryStream(Encoding.UTF8.GetBytes(this.GetDocument().ToString()));
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            WebServiceHost host = new WebServiceHost(typeof(Service), new Uri(baseAddress));
            host.Open();
            Console.WriteLine("Host opened");

            WebClient c = new WebClient();
            Console.WriteLine(c.DownloadString(baseAddress + "/GetDocument"));
            Console.WriteLine();
            Console.WriteLine(c.DownloadString(baseAddress + "/GetDocumentAsString"));
            Console.WriteLine();
            Console.WriteLine(c.DownloadString(baseAddress + "/GetDocumentAsStream"));

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class FaultOfException
    {
        [Serializable]
        public class MyException : Exception
        {
            public MyException() : base() { }
            public MyException(string message) : base(message) { }
            public MyException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        }
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            [FaultContract(typeof(MyException))]
            int Add(int x, int y);
        }
        public class Service : ITest
        {
            public int Add(int x, int y)
            {
                if (x < 0 && y < 0)
                {
                    throw new FaultException<MyException>(new MyException("Both parameters are negative"));
                }

                return x + y;
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), new BasicHttpBinding(), "");
            host.Description.Behaviors.Add(new ServiceMetadataBehavior { HttpGetEnabled = true });
            host.Open();
            Console.WriteLine("Host opened");

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    // http://stackoverflow.com/q/6529793/751090
    public class StackOverflow_6529793
    {
        public static void Test()
        {
            XmlDocument doc = new XmlDocument();
            string xml = "<root><item1></item1><item2></item2><item3/><item4 a='b'></item4><a:item5 xmlns:a='ns'></a:item5></root>";
            doc.LoadXml(xml);
            MemoryStream ms = new MemoryStream();
            doc.Save(ms);
            Console.WriteLine(Encoding.UTF8.GetString(ms.ToArray()));

            CollapseEmptyElements(doc.DocumentElement);
            ms = new MemoryStream();
            doc.Save(ms);
            Console.WriteLine(Encoding.UTF8.GetString(ms.ToArray()));
        }
        static void CollapseEmptyElements(XmlElement node)
        {
            if (!node.IsEmpty && node.ChildNodes.Count == 0)
            {
                node.IsEmpty = true;
            }

            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.NodeType == XmlNodeType.Element)
                {
                    CollapseEmptyElements((XmlElement)child);
                }
            }
        }
    }

    // http://stackoverflow.com/q/6548562/751090
    public class StackOverflow_6548562
    {
        public class WebserviceMessage
        {
            public string Data;
        }

        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            [WebGet(ResponseFormat = WebMessageFormat.Xml,
                BodyStyle = WebMessageBodyStyle.Wrapped,
                UriTemplate = "MAC/{input}")]
            string MAC_Get(string input);

            [OperationContract]
            [WebInvoke(Method = "POST",
                ResponseFormat = WebMessageFormat.Xml,
                BodyStyle = WebMessageBodyStyle.Wrapped,
                UriTemplate = "MAC/{input}")]
            WebserviceMessage MAC_Post(string input);
        }

        public class Service : ITest
        {
            public string MAC_Get(string input)
            {
                return input;
            }

            public WebserviceMessage MAC_Post(string input)
            {
                return new WebserviceMessage { Data = input };
            }
        }

        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service/";
            WebServiceHost host = new WebServiceHost(typeof(Service), new Uri(baseAddress));
            host.Open();
            Console.WriteLine("Host opened");

            WebClient c = new WebClient();
            Console.WriteLine(c.DownloadString(baseAddress + "/MAC/fromGET"));
            Console.WriteLine(c.UploadString(baseAddress + "/MAC/frompost", "POST", ""));

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    // http://stackoverflow.com/questions/6550019
    public class StackOverflow_6550019
    {
        [ServiceContract]
        public interface IfldtWholesaleService
        {
            [OperationContract]
            [WebInvoke(Method = "POST",
                ResponseFormat = WebMessageFormat.Xml,
                BodyStyle = WebMessageBodyStyle.Wrapped,
                UriTemplate = "MAC")]
            string MAC(string input);
        }

        public class Service : IfldtWholesaleService
        {
            public string MAC(string input)
            {
                return input;
            }
        }

        private static void postToWebsite(string url)
        {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
            req.Method = "POST";
            req.ContentType = "text/xml";

            string input = @"<MAC xmlns=""http://tempuri.org/""><input>hello</input></MAC>";

            StreamWriter writer = new StreamWriter(req.GetRequestStream());
            writer.Write(input);
            writer.Close();
            var rsp = req.GetResponse().GetResponseStream();

            Console.WriteLine(new StreamReader(rsp).ReadToEnd());
        }

        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service/";
            WebServiceHost host = new WebServiceHost(typeof(Service), new Uri(baseAddress));
            host.Open();
            Console.WriteLine("Host opened");

            // To find out the expected request, using a WCF client. Look at what it sends in Fiddler
            var factory = new WebChannelFactory<IfldtWholesaleService>(new Uri(baseAddress));
            var proxy = factory.CreateChannel();
            proxy.MAC("Hello world");

            postToWebsite(baseAddress + "/MAC");

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    // http://stackoverflow.com/q/6552443/751090
    public class StackOverflow_6552443
    {
        [DataContract]
        [KnownType("GetKnownTypes")]
        public class MyDCWithException
        {
            [DataMember]
            public Exception myException;

            public static MyDCWithException GetInstance()
            {
                MyDCWithException result = new MyDCWithException();
                result.myException = new ArgumentException("Invalid value");
                result.myException.Data["someData"] = new Dictionary<string, object>
                {
                    { "One", 1 },
                    { "Two", 2 },
                    { "Three", 3 },
                };
                return result;
            }

            public static Type[] GetKnownTypes()
            {
                List<Type> result = new List<Type>();
                result.Add(typeof(ArgumentException));
                result.Add(typeof(Dictionary<string, object>));
                result.Add(typeof(IDictionary).Assembly.GetType("System.Collections.ListDictionaryInternal"));
                return result.ToArray();
            }
        }
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            MyDCWithException GetDCWithException();
        }
        public class Service : ITest
        {
            public MyDCWithException GetDCWithException()
            {
                return MyDCWithException.GetInstance();
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), new BasicHttpBinding(), "");
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(new BasicHttpBinding(), new EndpointAddress(baseAddress));
            ITest proxy = factory.CreateChannel();

            Console.WriteLine(proxy.GetDCWithException());

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    // http://stackoverflow.com/q/6660689/751090
    public class StackOverflow_6660689
    {
        public static void Test()
        {
            WebClient wc = new WebClient();
            Encoding encoding = Encoding.GetEncoding("windows-1251");
            byte[] data = wc.DownloadData("http://demonscity.combats.com/zayavka.pl?logs=08_07_11&filter=Alex");
            GZipStream gzip = new GZipStream(new MemoryStream(data), CompressionMode.Decompress);
            MemoryStream decompressed = new MemoryStream();
            gzip.CopyTo(decompressed);
            string str = encoding.GetString(decompressed.GetBuffer(), 0, (int)decompressed.Length);
            Console.WriteLine(str);
        }
    }

    // http://stackoverflow.com/q/6666697/751090
    public class StackOverflow_6666697
    {
        [ServiceContract]
        public interface IService
        {
            [OperationContract]
            void Change(TypeA a);
        }
        [DataContract]
        public class TypeA { [DataMember] public TypeB b = new TypeB(); }
        [DataContract]
        public class TypeB { [DataMember] public TypeC c = new TypeC(); }
        [DataContract]
        public class TypeC { [DataMember] public string S1; }
        public class Service : IService
        {
            public void Change(TypeA a) { }
        }
        public class MyLoggingBehavior : IEndpointBehavior, IDispatchMessageInspector
        {
            public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
            {
            }

            public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
            {
            }

            public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
            {
                endpointDispatcher.DispatchRuntime.MessageInspectors.Add(this);
            }

            public void Validate(ServiceEndpoint endpoint)
            {
            }

            public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
            {
                Console.WriteLine("Log request:");
                Console.WriteLine(request);
                return null;
            }

            public void BeforeSendReply(ref Message reply, object correlationState)
            {
            }
        }
        public static void TestSerialization()
        {
            MemoryStream ms = new MemoryStream();
            XmlWriterSettings ws = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  ",
                OmitXmlDeclaration = true,
                Encoding = Encoding.UTF8
            };
            TypeA instance = new TypeA { b = new TypeB { c = new TypeC { S1 = "Hello world" } } };

            XmlWriter w = XmlWriter.Create(ms, ws);
            new XmlSerializer(typeof(TypeA), new Type[] { typeof(TypeB), typeof(TypeC) }).Serialize(w, instance);
            w.Flush();
            Console.WriteLine("XmlSerializer:");
            Console.WriteLine(Encoding.UTF8.GetString(ms.ToArray()));

            ms.SetLength(0);
            w = XmlWriter.Create(ms, ws);
            new DataContractSerializer(typeof(TypeA), new Type[] { typeof(TypeB), typeof(TypeC) }).WriteObject(w, instance);
            w.Flush();
            Console.WriteLine("DataContractSerializer:");
            Console.WriteLine(Encoding.UTF8.GetString(ms.ToArray()));

            ms.SetLength(0);
            w = XmlWriter.Create(ms, ws);
            new NetDataContractSerializer().WriteObject(w, instance);
            w.Flush();
            Console.WriteLine("NetDataContractSerializer:");
            Console.WriteLine(Encoding.UTF8.GetString(ms.ToArray()));

            ms.SetLength(0);
            w = JsonReaderWriterFactory.CreateJsonWriter(ms, Encoding.UTF8);
            new DataContractJsonSerializer(typeof(TypeA), new Type[] { typeof(TypeB), typeof(TypeC) }, 65536, false, null, true).WriteObject(w, instance);
            w.Flush();
            Console.WriteLine("DataContractJsonSerializer:");
            Console.WriteLine(Encoding.UTF8.GetString(ms.ToArray()));
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(IService), new BasicHttpBinding(), "").Behaviors.Add(new MyLoggingBehavior());
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<IService> factory = new ChannelFactory<IService>(new BasicHttpBinding(), new EndpointAddress(baseAddress));
            IService proxy = factory.CreateChannel();

            TypeA instance = new TypeA { b = new TypeB { c = new TypeC { S1 = "Hello world" } } };
            proxy.Change(instance);

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_9b68e890_cae0_4a65_b0ef_53e936d46d0d
    {
        [DataContract(Name = "Employee", Namespace = "http://schemas.datacontract.org/2004/07/Cy.Wpf.Test.Serialize")]
        public class Employee
        {
            [DataMember]
            public int EmployeeId { get; set; }
            [DataMember]
            public string FirstName { get; set; }
            [DataMember]
            public string LastName { get; set; }
        }

        public static void Test()
        {
            XmlWriterSettings ws = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  ",
                OmitXmlDeclaration = true,
                Encoding = new UTF8Encoding(false),
            };
            MemoryStream ms = new MemoryStream();
            XmlWriter w = XmlWriter.Create(ms, ws);
            DataContractSerializer dcs = new DataContractSerializer(typeof(Employee));
            Employee employee = new Employee
            {
                EmployeeId = 101,
                FirstName = "John",
                LastName = "Doe"
            };
            dcs.WriteObject(w, employee);
            w.Flush();
            Console.WriteLine(Encoding.UTF8.GetString(ms.ToArray()));
        }
    }

    public class Post_2d66e289_11ff_47f9_a3d1_bad18c60d341
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            byte[] GetData(int size);
        }
        public class Service : ITest
        {
            public byte[] GetData(int size)
            {
                return new byte[size];
            }
        }
        public class MyMessageEncodingBindingElement : MessageEncodingBindingElement
        {
            public MessageEncodingBindingElement inner;
            long maxSentMessageSize;
            public MyMessageEncodingBindingElement(MessageEncodingBindingElement inner, long maxSentMessageSize)
            {
                this.inner = inner;
                this.maxSentMessageSize = maxSentMessageSize;
            }
            public override MessageEncoderFactory CreateMessageEncoderFactory()
            {
                return new MyMessageEncoderFactory(this.inner.CreateMessageEncoderFactory(), this.maxSentMessageSize);
            }
            public override MessageVersion MessageVersion
            {
                get { return this.inner.MessageVersion; }
                set { this.inner.MessageVersion = value; }
            }
            public override BindingElement Clone()
            {
                return new MyMessageEncodingBindingElement((MessageEncodingBindingElement)this.inner.Clone(), this.maxSentMessageSize);
            }
            public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
            {
                context.BindingParameters.Add(this);
                return context.BuildInnerChannelFactory<TChannel>();
            }
            public override IChannelListener<TChannel> BuildChannelListener<TChannel>(BindingContext context)
            {
                context.BindingParameters.Add(this);
                return context.BuildInnerChannelListener<TChannel>();
            }
            public override bool CanBuildChannelFactory<TChannel>(BindingContext context)
            {
                return context.CanBuildInnerChannelFactory<TChannel>();
            }
            public override bool CanBuildChannelListener<TChannel>(BindingContext context)
            {
                return context.CanBuildInnerChannelListener<TChannel>();
            }

            class MyMessageEncoderFactory : MessageEncoderFactory
            {
                MessageEncoderFactory inner;
                long maxSentMessageSize;
                public MyMessageEncoderFactory(MessageEncoderFactory inner, long maxSentMessageSize)
                {
                    this.inner = inner;
                    this.maxSentMessageSize = maxSentMessageSize;
                }
                public override MessageEncoder Encoder
                {
                    get { return new MyMessageEncoder(this.inner.Encoder, this.maxSentMessageSize); }
                }
                public override MessageVersion MessageVersion
                {
                    get { return this.inner.MessageVersion; }
                }
            }

            class MyMessageEncoder : MessageEncoder
            {
                MessageEncoder inner;
                long maxSentMessageSize;
                public MyMessageEncoder(MessageEncoder inner, long maxSentMessageSize)
                {
                    this.inner = inner;
                    this.maxSentMessageSize = maxSentMessageSize;
                }
                public override string ContentType
                {
                    get { return this.inner.ContentType; }
                }
                public override string MediaType
                {
                    get { return this.inner.MediaType; }
                }
                public override MessageVersion MessageVersion
                {
                    get { return this.inner.MessageVersion; }
                }
                public override bool IsContentTypeSupported(string contentType)
                {
                    return this.inner.IsContentTypeSupported(contentType);
                }
                public override Message ReadMessage(ArraySegment<byte> buffer, BufferManager bufferManager, string contentType)
                {
                    return this.inner.ReadMessage(buffer, bufferManager, contentType);
                }
                public override Message ReadMessage(Stream stream, int maxSizeOfHeaders, string contentType)
                {
                    return this.inner.ReadMessage(stream, maxSizeOfHeaders, contentType);
                }
                public override ArraySegment<byte> WriteMessage(Message message, int maxMessageSize, BufferManager bufferManager, int messageOffset)
                {
                    ArraySegment<byte> result = this.inner.WriteMessage(message, maxMessageSize, bufferManager, messageOffset);
                    if (result.Count > this.maxSentMessageSize)
                    {
                        throw new InvalidOperationException("MaxSentMessageSize exceeded!");
                    }

                    return result;
                }
                public override void WriteMessage(Message message, Stream stream)
                {
                    CountingStream countingStream = new CountingStream(stream, this.maxSentMessageSize);
                    this.inner.WriteMessage(message, countingStream);
                }
            }
            class CountingStream : Stream
            {
                Stream innerStream;
                long maxSentMessageSize;
                long bytesSent;
                public CountingStream(Stream innerStream, long maxSentMessageSize)
                {
                    this.innerStream = innerStream;
                    this.maxSentMessageSize = maxSentMessageSize;
                    this.bytesSent = 0;
                }

                public override bool CanRead
                {
                    get { return false; }
                }

                public override bool CanSeek
                {
                    get { return false; }
                }

                public override bool CanWrite
                {
                    get { return true; }
                }

                public override void Flush()
                {
                }

                public override long Length
                {
                    get { throw new NotSupportedException(); }
                }

                public override long Position
                {
                    get { throw new NotSupportedException(); }
                    set { throw new NotSupportedException(); }
                }

                public override int Read(byte[] buffer, int offset, int count)
                {
                    throw new NotSupportedException();
                }

                public override long Seek(long offset, SeekOrigin origin)
                {
                    throw new NotSupportedException();
                }

                public override void SetLength(long value)
                {
                    throw new NotSupportedException();
                }

                public override void Write(byte[] buffer, int offset, int count)
                {
                    this.bytesSent -= count;
                    if (this.bytesSent < 0)
                    {
                        throw new InvalidOperationException("MaxSentMessageSize exceeded!");
                    }

                    this.innerStream.Write(buffer, offset, count);
                }
            }
        }
        static Binding GetBinding(long maxSentMessageSize)
        {
            CustomBinding result = new CustomBinding(new BasicHttpBinding());
            for (int i = 0; i < result.Elements.Count; i++)
            {
                if (result.Elements[i] is MessageEncodingBindingElement)
                {
                    result.Elements[i] = new MyMessageEncodingBindingElement((MessageEncodingBindingElement)result.Elements[i], maxSentMessageSize);
                }
            }
            return result;
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), GetBinding(1000), "");
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(GetBinding(1000), new EndpointAddress(baseAddress));
            ITest proxy = factory.CreateChannel();

            Console.WriteLine(proxy.GetData(100).Length);
            try
            {
                Console.WriteLine(proxy.GetData(1000).Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    // http://stackoverflow.com/q/6783264/751090
    public class StackOverflow_6783264
    {
        public class InputData
        {
            public string FirstName;
            public string LastName;
        }
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            [WebGet(UriTemplate = "/InsertData?param1={param1}")]
            string saveDataGet(InputData param1);
            [OperationContract]
            [WebInvoke(UriTemplate = "/InsertData")]
            string saveDataPost(InputData param1);
        }
        public class Service : ITest
        {
            public string saveDataGet(InputData param1)
            {
                return "Via GET: " + param1.FirstName + " " + param1.LastName;
            }
            public string saveDataPost(InputData param1)
            {
                return "Via POST: " + param1.FirstName + " " + param1.LastName;
            }
        }
        public class MyQueryStringConverter : QueryStringConverter
        {
            public override bool CanConvert(Type type)
            {
                return (type == typeof(InputData)) || base.CanConvert(type);
            }
            public override object ConvertStringToValue(string parameter, Type parameterType)
            {
                if (parameterType == typeof(InputData))
                {
                    string[] parts = parameter.Split(',');
                    return new InputData { FirstName = parts[0], LastName = parts[1] };
                }
                else
                {
                    return base.ConvertStringToValue(parameter, parameterType);
                }
            }
        }
        public class MyWebHttpBehavior : WebHttpBehavior
        {
            protected override QueryStringConverter GetQueryStringConverter(OperationDescription operationDescription)
            {
                return new MyQueryStringConverter();
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), new WebHttpBinding(), "").Behaviors.Add(new MyWebHttpBehavior());
            host.Open();
            Console.WriteLine("Host opened");

            WebClient client = new WebClient();
            Console.WriteLine(client.DownloadString(baseAddress + "/InsertData?param1=John,Doe"));

            client = new WebClient();
            client.Headers[HttpRequestHeader.ContentType] = "application/json";
            Console.WriteLine(client.UploadString(baseAddress + "/InsertData", "{\"FirstName\":\"John\",\"LastName\":\"Doe\"}"));

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_16872bec_5817_410b_83bd_f4e9c5feaa61
    {
        [ServiceContract]
        public interface ITest
        {
            [WebGet(UriTemplate = "/sys/name")]
            SysNameResponse SysName();
            [WebGet(UriTemplate = "/tstat/temp")]
            TStatTempResponse TstatTemp();
            [WebGet(UriTemplate = "/tstat/tmode")]
            TStatTModeResponse TstatTMode();
            [WebGet(UriTemplate = "/tstat")]
            TStatResponse Tstat();
            [WebGet(UriTemplate = "/tstat/program/heat/day")]
            TStatProgramHeatDayResponse TstatProgramHeatDay();
        }
        [DataContract]
        public class SysNameResponse
        {
            [DataMember(Name = "name")]
            public string Name { get; set; }
        }
        [DataContract]
        public class TStatTempResponse
        {
            [DataMember(Name = "temp")]
            public double Temp { get; set; }
        }
        [DataContract]
        public class TStatTModeResponse
        {
            [DataMember(Name = "tmode")]
            public int TMode { get; set; }
        }
        [DataContract]
        public class TStatResponse
        {
            [DataMember(Name = "temp")]
            public double Temp { get; set; }
            [DataMember(Name = "tmode")]
            public int TMode { get; set; }
            [DataMember(Name = "fmode")]
            public int FMode { get; set; }
            [DataMember(Name = "override")]
            public int Override { get; set; }
            [DataMember(Name = "hold")]
            public int Hold { get; set; }
            [DataMember(Name = "tcool")]
            public double TCool { get; set; }
            [DataMember(Name = "tstate")]
            public int TState { get; set; }
            [DataMember(Name = "fstate")]
            public int FState { get; set; }
            [DataMember(Name = "time")]
            public TStatTime Time { get; set; }
            [DataMember(Name = "t_type_post")]
            public int TTypePost { get; set; }
        }
        [DataContract]
        public class TStatTime
        {
            [DataMember(Name = "day")]
            public int Day { get; set; }
            [DataMember(Name = "hour")]
            public int Hour { get; set; }
            [DataMember(Name = "minute")]
            public int Minute { get; set; }
        }
        [DataContract]
        public class TStatProgramHeatDayResponse
        {
            // property names can be changed - it won't affect the behavior
            // i.e., Sunday --> Day0, Monday --> Day1, etc.
            [DataMember(Name = "0")]
            public int[] Sunday { get; set; }
            [DataMember(Name = "1")]
            public int[] Monday { get; set; }
            [DataMember(Name = "2")]
            public int[] Tuesday { get; set; }
            [DataMember(Name = "3")]
            public int[] Wednesday { get; set; }
            [DataMember(Name = "4")]
            public int[] Thursday { get; set; }
            [DataMember(Name = "5")]
            public int[] Friday { get; set; }
            [DataMember(Name = "6")]
            public int[] Saturday { get; set; }
        }
        public class Service : ITest
        {
            public SysNameResponse SysName()
            {
                return new SysNameResponse { Name = "Bob" };
            }

            public TStatTempResponse TstatTemp()
            {
                return new TStatTempResponse { Temp = 71.5 };
            }

            public TStatTModeResponse TstatTMode()
            {
                return new TStatTModeResponse { TMode = 2 };
            }

            public TStatResponse Tstat()
            {
                return new TStatResponse
                {
                    Temp = 74.5,
                    TMode = 2,
                    FMode = 0,
                    Override = 1,
                    Hold = 0,
                    TCool = 79.0,
                    TState = 0,
                    FState = 0,
                    TTypePost = 0,
                    Time = new TStatTime { Day = 3, Hour = 17, Minute = 7 }
                };
            }

            public TStatProgramHeatDayResponse TstatProgramHeatDay()
            {
                return new TStatProgramHeatDayResponse
                {
                    Sunday = new int[] { 360, 70, 480, 62, 1080, 70, 1320, 62 },
                    Monday = new int[] { 360, 70, 480, 62, 1080, 70, 1320, 62 },
                    Tuesday = new int[] { 360, 70, 480, 62, 1080, 70, 1320, 62 },
                    Wednesday = new int[] { 360, 70, 480, 62, 1080, 70, 1320, 62 },
                    Thursday = new int[] { 360, 70, 480, 62, 1080, 70, 1320, 62 },
                    Friday = new int[] { 360, 70, 480, 62, 1080, 70, 1320, 62 },
                    Saturday = new int[] { 360, 70, 480, 62, 1080, 70, 1320, 62 },
                };
            }
        }

        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            WebServiceHost host = new WebServiceHost(typeof(Service), new Uri(baseAddress));
            host.Open();
            Console.WriteLine("Host opened");

            WebChannelFactory<ITest> factory = new WebChannelFactory<ITest>(new Uri(baseAddress));
            ITest proxy = factory.CreateChannel();
            Console.WriteLine(proxy.SysName().Name);
            Console.WriteLine(proxy.TstatTemp().Temp);
            Console.WriteLine(proxy.TstatTMode().TMode);
            Console.WriteLine(proxy.Tstat().Time.Day);
            Console.WriteLine(proxy.TstatProgramHeatDay().Sunday[2]);

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_16872bec_5817_410b_83bd_f4e9c5feaa61_b
    {
        [ServiceContract]
        public interface ITest
        {
            [WebInvoke(UriTemplate = "/tstat", RequestFormat = WebMessageFormat.Json)]
            TStatPostResponse Operation(TStatPostRequest input);

            [WebInvoke(UriTemplate = "/tstat", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
            [return: MessageParameter(Name = "success")]
            int Operation(string t_heat);
        }
        [DataContract]
        public class TStatPostResponse
        {
            [DataMember(Name = "success")]
            public int Success { get; set; }
        }
        [DataContract]
        public class TStatPostRequest
        {
            [DataMember(Name = "t_heat")]
            public double THeat { get; set; }
        }
    }

    public class RedirectTest
    {
        [ServiceContract]
        public interface ITest
        {
            [WebGet]
            void Redirect();
        }
        public class Service : ITest
        {
            public void Redirect()
            {
                WebOperationContext.Current.OutgoingResponse.Location = "http://www.google.com";
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.Redirect;
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            WebServiceHost host = new WebServiceHost(typeof(Service), new Uri(baseAddress));
            host.Open();
            Console.WriteLine("Host opened");

            Util.SendRequest(baseAddress + "/Redirect", "GET", null, null);

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    // http://stackoverflow.com/questions/6846012
    public class StackOverflow_6846012
    {
        class MyWriter : XmlDictionaryWriter
        {
            XmlDictionaryWriter inner;
            public MyWriter(XmlDictionaryWriter inner)
            {
                this.inner = inner;
            }

            public override void Close()
            {
                this.inner.Close();
            }

            public override void Flush()
            {
                this.inner.Flush();
            }

            public override string LookupPrefix(string ns)
            {
                return this.inner.LookupPrefix(ns);
            }

            public override void WriteBase64(byte[] buffer, int index, int count)
            {
                this.inner.WriteBase64(buffer, index, count);
            }

            public override void WriteCData(string text)
            {
                this.inner.WriteCData(text);
            }

            public override void WriteCharEntity(char ch)
            {
                this.inner.WriteCharEntity(ch);
            }

            public override void WriteChars(char[] buffer, int index, int count)
            {
                this.inner.WriteChars(buffer, index, count);
            }

            public override void WriteComment(string text)
            {
                this.inner.WriteComment(text);
            }

            public override void WriteDocType(string name, string pubid, string sysid, string subset)
            {
                this.inner.WriteDocType(name, pubid, sysid, subset);
            }

            public override void WriteEndAttribute()
            {
                this.inner.WriteEndAttribute();
            }

            public override void WriteEndDocument()
            {
                this.inner.WriteEndDocument();
            }

            public override void WriteEndElement()
            {
                this.inner.WriteFullEndElement();
            }

            public override void WriteEntityRef(string name)
            {
                this.inner.WriteEntityRef(name);
            }

            public override void WriteFullEndElement()
            {
                this.inner.WriteFullEndElement();
            }

            public override void WriteProcessingInstruction(string name, string text)
            {
                this.inner.WriteProcessingInstruction(name, text);
            }

            public override void WriteRaw(string data)
            {
                this.inner.WriteRaw(data);
            }

            public override void WriteRaw(char[] buffer, int index, int count)
            {
                this.inner.WriteRaw(buffer, index, count);
            }

            public override void WriteStartAttribute(string prefix, string localName, string ns)
            {
                this.inner.WriteStartAttribute(prefix, localName, ns);
            }

            public override void WriteStartDocument(bool standalone)
            {
                this.inner.WriteStartDocument(standalone);
            }

            public override void WriteStartDocument()
            {
                this.inner.WriteStartDocument();
            }

            public override void WriteStartElement(string prefix, string localName, string ns)
            {
                this.inner.WriteStartElement(prefix, localName, ns);
            }

            public override WriteState WriteState
            {
                get { return this.inner.WriteState; }
            }

            public override void WriteString(string text)
            {
                this.inner.WriteString(text);
            }

            public override void WriteSurrogateCharEntity(char lowChar, char highChar)
            {
                this.inner.WriteSurrogateCharEntity(lowChar, highChar);
            }

            public override void WriteWhitespace(string ws)
            {
                this.inner.WriteWhitespace(ws);
            }
        }
        public static void Test()
        {
            DataContractSerializer dcs = new DataContractSerializer(typeof(List<string>));
            MemoryStream ms = new MemoryStream();
            List<string> list = new List<string> { "Hello", "", "world" };
            dcs.WriteObject(ms, list);
            Console.WriteLine(Encoding.UTF8.GetString(ms.ToArray()));

            Console.WriteLine();

            ms.SetLength(0);
            XmlDictionaryWriter myWriter = new MyWriter(XmlDictionaryWriter.CreateTextWriter(ms));
            dcs.WriteObject(myWriter, list);
            myWriter.Flush();
            Console.WriteLine(Encoding.UTF8.GetString(ms.ToArray()));
        }
    }

    public class Post_54770c31_51df_4ba2_bf7a_56fe2b76834c
    {
        static readonly string BaseAddressB = "http://" + Environment.MachineName + ":8000/Service";
        static readonly string BaseAddressC = "http://" + Environment.MachineName + ":8008/Service";
        static Binding GetBinding()
        {
            return new BasicHttpBinding();
        }
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            string Echo(string text);
        }
        [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
        public class ServiceC : ITest
        {
            public string Echo(string text)
            {
                int headerIndex = OperationContext.Current.IncomingMessageHeaders.FindHeader("ServerEndpointAddress", "MyNamespace");
                if (headerIndex < 0)
                {
                    return text + " - no server endpoint address";
                }
                else
                {
                    Uri headerValue = OperationContext.Current.IncomingMessageHeaders.GetHeader<Uri>(headerIndex);
                    return text + " - server endpoint address:" + headerValue.ToString();
                }
            }
        }
        [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
        public class ServiceB : ITest
        {
            public string Echo(string text)
            {
                Uri myAddress = OperationContext.Current.EndpointDispatcher.EndpointAddress.Uri;
                TestClient client = new TestClient(BaseAddressC);
                using (new OperationContextScope(client.InnerChannel))
                {
                    OperationContext.Current.OutgoingMessageHeaders.Add(
                        MessageHeader.CreateHeader(
                            "ServerEndpointAddress",
                            "MyNamespace",
                            myAddress));
                    return client.Echo(text);
                }
            }
        }
        class TestClient : ClientBase<ITest>, ITest
        {
            public TestClient(string address) : base(GetBinding(), new EndpointAddress(address)) { }
            public string Echo(string text)
            {
                return this.Channel.Echo(text);
            }
        }
        public static void Test()
        {
            ServiceHost hostB = new ServiceHost(typeof(ServiceB), new Uri(BaseAddressB));
            ServiceHost hostC = new ServiceHost(typeof(ServiceC), new Uri(BaseAddressC));
            hostB.AddServiceEndpoint(typeof(ITest), GetBinding(), "");
            hostC.AddServiceEndpoint(typeof(ITest), GetBinding(), "");
            hostB.Open();
            hostC.Open();
            Console.WriteLine("Hosts opened");

            TestClient client = new TestClient(BaseAddressB);
            Console.WriteLine("Calling via B");
            Console.WriteLine(client.Echo("Via B"));

            client = new TestClient(BaseAddressC);
            Console.WriteLine("Calling C directly");
            Console.WriteLine(client.Echo("Direct call"));

            Console.WriteLine("Press ENTER to close the host");
            Console.ReadLine();
            hostB.Close();
            hostC.Close();
        }
    }

    public class Post_18451bc5_86e5_4189_af61_c240597413dc
    {
        [ServiceContract]
        public interface ITest
        {
            [WebInvoke(Method = "OPTIONS", UriTemplate = "*")]
            void Options();
            [WebGet]
            int Add(int x, int y);
        }
        public class Service : ITest
        {
            public void Options()
            {
                WebOperationContext.Current.OutgoingResponse.Headers.Add("X-MyHeader", "value");
                WebOperationContext.Current.OutgoingResponse.Headers.Add("Public", "OPTIONS, GET");
            }

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

            Util.SendRequest(baseAddress, "OPTIONS", null, null);
            Util.SendRequest(baseAddress + "/Add?x=5&y=8", "GET", null, null);

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_37650ab4_f3cb_4f2f_aa65_d94f218ea992
    {
        [ServiceContract]
        public interface ITest
        {
            [WebInvoke(Method = "OPTIONS", UriTemplate = "*")]
            void Options();
            [WebGet]
            int Add(int x, int y);
        }
        public class Service : ITest
        {
            public void Options()
            {
                WebOperationContext.Current.OutgoingResponse.Headers.Add("X-MyHeader", "value");
                WebOperationContext.Current.OutgoingResponse.Headers.Add("Public", "OPTIONS, GET");
            }

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

            Util.SendRequest(baseAddress, "OPTIONS", null, null);
            Util.SendRequest(baseAddress + "/Add?x=5&y=8", "GET", null, null);

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    // http://stackoverflow.com/q/6905108/751090
    public class StackOverflow_6905108
    {
        [ServiceContract]
        public class Service
        {
            [OperationContract]
            [WebInvoke(Method = "POST", UriTemplate = "UploadImages/{fileName}?array={array}")]
            public void UploadImages(int[] array, string fileName, Stream image)
            {
                Console.WriteLine("Array:");
                foreach (var item in array) Console.Write("{0} ", item);
                Console.WriteLine();
            }
        }
        public static void SendPost(string uri, string contentType, string body)
        {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(uri);
            req.Method = "POST";
            req.ContentType = contentType;
            Stream reqStream = req.GetRequestStream();
            byte[] reqBytes = Encoding.UTF8.GetBytes(body);
            reqStream.Write(reqBytes, 0, reqBytes.Length);
            reqStream.Close();

            HttpWebResponse resp;
            try
            {
                resp = (HttpWebResponse)req.GetResponse();
            }
            catch (WebException e)
            {
                resp = (HttpWebResponse)e.Response;
            }

            Console.WriteLine("HTTP/{0} {1} {2}", resp.ProtocolVersion, (int)resp.StatusCode, resp.StatusDescription);
            foreach (string headerName in resp.Headers.AllKeys)
            {
                Console.WriteLine("{0}: {1}", headerName, resp.Headers[headerName]);
            }
            Console.WriteLine();
            Stream respStream = resp.GetResponseStream();
            Console.WriteLine(new StreamReader(respStream).ReadToEnd());

            Console.WriteLine();
            Console.WriteLine(" *-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-* ");
            Console.WriteLine();
        }
        class MyQueryStringConverter : QueryStringConverter
        {
            QueryStringConverter originalConverter;
            public MyQueryStringConverter(QueryStringConverter originalConverter)
            {
                this.originalConverter = originalConverter;
            }
            public override bool CanConvert(Type type)
            {
                return type == typeof(int[]) || base.CanConvert(type);
            }
            public override object ConvertStringToValue(string parameter, Type parameterType)
            {
                if (parameterType == typeof(int[]))
                {
                    return parameter.Split(',').Select(x => int.Parse(x)).ToArray();
                }
                else
                {
                    return base.ConvertStringToValue(parameter, parameterType);
                }
            }
        }
        public class MyWebHttpBehavior : WebHttpBehavior
        {
            protected override QueryStringConverter GetQueryStringConverter(OperationDescription operationDescription)
            {
                return new MyQueryStringConverter(base.GetQueryStringConverter(operationDescription));
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(Service), new WebHttpBinding(), "").Behaviors.Add(new MyWebHttpBehavior());
            host.Description.Behaviors.Add(new ServiceMetadataBehavior { HttpGetEnabled = true });
            host.Open();
            Console.WriteLine("Host opened");

            SendPost(baseAddress + "/UploadImages/a.txt?array=1,2,3,4", "application/octet-stream", "The file contents");

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_80c25f31_a62a_41ce_b494_932e74321800
    {
        [DataContract(Name = "User", Namespace = "")]
        public class User
        {
            [DataMember]
            public int Id { get; set; }
            [DataMember]
            public string Name { get; set; }
            [DataMember]
            public string password { get; set; }
        }

        [ServiceContract]
        public class Service
        {
            [OperationContract]
            [WebInvoke(
                UriTemplate = "User2/purchase",
                RequestFormat = WebMessageFormat.Xml,
                ResponseFormat = WebMessageFormat.Xml,
                Method = "POST")]
            public User SearchUser(User name)
            {
                User u = new User();
                if (name == null)
                {
                    u.Id = 1;
                }
                else
                {
                    u.Id = name.Id + 1;
                    u.Name = name.Name;
                    u.password = name.password;
                }

                return u;
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            WebServiceHost host = new WebServiceHost(typeof(Service), new Uri(baseAddress));
            host.Open();
            Console.WriteLine("Host opened");

            try
            {
                var client = new WebClient();
                client.Headers[HttpRequestHeader.ContentType] = "text/xml";
                string userxml = "<User><Id>2</Id><Name>harry</Name><password>123</password></User>";
                string usern = client.UploadString(baseAddress + "/User2/purchase", "POST", userxml);
                Console.WriteLine(usern);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            Console.WriteLine();

            try
            {
                var client = new WebClient();
                string userxml = "";
                string usern = client.UploadString(baseAddress + "/User2/purchase", "POST", userxml);
                Console.WriteLine(usern);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    // http://stackoverflow.com/q/6926166/751090
    public class StackOverflow_6926166
    {
        [DataContract(Name = "Face", Namespace = "")]
        public class Face
        {
            [DataMember]
            public string Eyes { get; set; }
            [DataMember]
            public string Hair { get; set; }
        }

        [CollectionDataContract(Name = "MyDifferentName", Namespace = "")]
        public class MyListOfFace : List<Face> { }

        [ServiceContract]
        public class Service
        {
            [OperationContract]
            [WebGet]
            public MyListOfFace DetectedFaces()
            {
                return new MyListOfFace
                {
                    new Face { Eyes = "Brown", Hair = "Black" },
                    new Face { Eyes = "Green", Hair = "Brown" },
                };
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            WebServiceHost host = new WebServiceHost(typeof(Service), new Uri(baseAddress));
            host.Open();
            Console.WriteLine("Host opened");

            WebClient c = new WebClient();
            Console.WriteLine(c.DownloadString(baseAddress + "/DetectedFaces"));

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    // http://stackoverflow.com/q/6966835/751090
    public class StackOverflow_6966835
    {
        [DataContract()]
        public class MyClass
        {
            [DataMember()]
            public Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();
        }
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            MyClass GetMyClass();
        }
        public class Service : ITest
        {
            public MyClass GetMyClass()
            {
                return new MyClass
                {
                    dictionary = new Dictionary<string, List<string>>
                    {
                        { "one", "uno un eins".Split().ToList() },
                        { "two", "dos un zwei".Split().ToList() },
                        { "three", "tres trois drei".Split().ToList() }
                    }
                };
            }
        }
        public static void Test()
        {
            Console.WriteLine("Stand-alone serialization");
            MemoryStream ms = new MemoryStream();
            MyClass c = new Service().GetMyClass();
            DataContractSerializer dcs = new DataContractSerializer(typeof(MyClass));
            dcs.WriteObject(ms, c);
            Console.WriteLine("Serialized: {0}", Encoding.UTF8.GetString(ms.ToArray()));

            Console.WriteLine();
            Console.WriteLine("Now using in a service");

            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), new BasicHttpBinding(), "");
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(new BasicHttpBinding(), new EndpointAddress(baseAddress));
            ITest proxy = factory.CreateChannel();

            Console.WriteLine(proxy.GetMyClass());

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    // http://stackoverflow.com/q/7005187/751090
    public class StackOverflow_7005187
    {
        [DataContract]
        public class MyDC
        {
            [DataMember]
            public string str = "The string";
        }
        [ServiceContract]
        public interface ITest
        {
            [WebInvoke]
            string EchoString(string text);
            [WebGet]
            int Add(int x, int y);
            [WebInvoke]
            MyDC EchoDC(MyDC input);
            [WebGet(UriTemplate = "/help")]
            Message GetMainHelpPage();
            [WebGet(UriTemplate = "/help/operations/EchoDC")]
            Message GetOperationsEchoDCHelpPage();
            // help for other operations not implemented
        }
        public class Service : ITest
        {
            public string EchoString(string text)
            {
                return text;
            }

            public int Add(int x, int y)
            {
                return x + y;
            }

            public MyDC EchoDC(MyDC input)
            {
                return input;
            }

            public Message GetMainHelpPage()
            {
                string page = @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
    <html xmlns=""http://www.w3.org/1999/xhtml"">
      <head>
        <title>Operations at http://localhost:8000/Service</title>
        <style>BODY { color: #000000; background-color: white; font-family: Verdana; margin-left: 0px; margin-top: 0px; } #content { margin-left: 30px; font-size: .70em; padding-bottom: 2em; } A:link { color: #336699; font-weight: bold; text-decoration: underline; } A:visited { color: #6699cc; font-weight: bold; text-decoration: underline; } A:active { color: #336699; font-weight: bold; text-decoration: underline; } .heading1 { background-color: #003366; border-bottom: #336699 6px solid; color: #ffffff; font-family: Tahoma; font-size: 26px; font-weight: normal;margin: 0em 0em 10px -20px; padding-bottom: 8px; padding-left: 30px;padding-top: 16px;} pre { font-size:small; background-color: #e5e5cc; padding: 5px; font-family: Courier New; margin-top: 0px; border: 1px #f0f0e0 solid; white-space: pre-wrap; white-space: -pre-wrap; word-wrap: break-word; } table { border-collapse: collapse; border-spacing: 0px; font-family: Verdana;} table th { border-right: 2px white solid; border-bottom: 2px white solid; font-weight: bold; background-color: #cecf9c;} table td { border-right: 2px white solid; border-bottom: 2px white solid; background-color: #e5e5cc;}</style>
      </head>
      <body>
        <div id=""content"">
          <p class=""heading1"">Operations at http://localhost:8000/Service</p>
          <p>This page describes the service operations at this endpoint.</p>
          <table>
            <tr>
              <th>Uri</th>
              <th>Method</th>
              <th>Description</th>
            </tr>
            <tr>
              <td>Add</td>
              <td title=""http://localhost:8000/Service/Add?x={X}&amp;y={Y}"">
                <a rel=""operation"" href=""help/operations/Add"">GET</a>
              </td>
              <td>Service at http://localhost:8000/Service/Add?x={X}&amp;y={Y}</td>
            </tr>
            <tr>
              <td>EchoDC</td>
              <td title=""http://localhost:8000/Service/EchoDC"">
                <a rel=""operation"" href=""help/operations/EchoDC"">POST</a>
              </td>
              <td>Service at http://localhost:8000/Service/EchoDC</td>
            </tr>
            <tr>
              <td>EchoString</td>
              <td title=""http://localhost:8000/Service/EchoString"">
                <a rel=""operation"" href=""help/operations/EchoString"">POST</a>
              </td>
              <td>Service at http://localhost:8000/Service/EchoString</td>
            </tr>
          </table>
        </div>
      </body>
    </html>";
                return WebOperationContext.Current.CreateStreamResponse(
                    new MemoryStream(Encoding.UTF8.GetBytes(page)),
                    "text/html");
            }


            public Message GetOperationsEchoDCHelpPage()
            {
                string page = @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
    <html xmlns=""http://www.w3.org/1999/xhtml"">
      <head>
        <title>Reference for  http://localhost:8000/Service/EchoDC</title>
        <style>BODY { color: #000000; background-color: white; font-family: Verdana; margin-left: 0px; margin-top: 0px; } #content { margin-left: 30px; font-size: .70em; padding-bottom: 2em; } A:link { color: #336699; font-weight: bold; text-decoration: underline; } A:visited { color: #6699cc; font-weight: bold; text-decoration: underline; } A:active { color: #336699; font-weight: bold; text-decoration: underline; } .heading1 { background-color: #003366; border-bottom: #336699 6px solid; color: #ffffff; font-family: Tahoma; font-size: 26px; font-weight: normal;margin: 0em 0em 10px -20px; padding-bottom: 8px; padding-left: 30px;padding-top: 16px;} pre { font-size:small; background-color: #e5e5cc; padding: 5px; font-family: Courier New; margin-top: 0px; border: 1px #f0f0e0 solid; white-space: pre-wrap; white-space: -pre-wrap; word-wrap: break-word; } table { border-collapse: collapse; border-spacing: 0px; font-family: Verdana;} table th { border-right: 2px white solid; border-bottom: 2px white solid; font-weight: bold; background-color: #cecf9c;} table td { border-right: 2px white solid; border-bottom: 2px white solid; background-color: #e5e5cc;}</style>
      </head>
      <body>
        <div id=""content"">
          <p class=""heading1"">Reference for  http://localhost:8000/Service/EchoDC</p>
          <p></p>
          <p xmlns=""http://www.w3.org/1999/xhtml"">
            <b>Url: </b>
            <span class=""uri-template"">http://localhost:8000/Service/EchoDC</span>
          </p>
          <p xmlns=""http://www.w3.org/1999/xhtml"">
            <b>HTTP Method: </b>
            <span class=""method"">POST</span>
          </p>
          <table>
            <tr>
              <th>Message direction</th>
              <th>Format</th>
              <th>Body</th>
            </tr>
            <tr>
              <td>Request</td>
              <td>Xml</td>
              <td>
                <a href=""#request-xml"">Example</a></td>
            </tr>
            <tr>
              <td>Request</td>
              <td>Json</td>
              <td>
                <a href=""#request-json"">Example</a>
              </td>
            </tr>
            <tr>
              <td>Response</td>
              <td>Xml</td>
              <td>
                <a href=""#response-xml"">Example</a></td>
            </tr>
          </table>
          <p>
            <a name=""#request-xml"">The following is an example request Xml body:</a>
            <pre class=""request-xml"">&lt;StackOverflow_7005187.MyDC xmlns=""http://schemas.datacontract.org/2004/07/WcfForums""&gt;
      &lt;str&gt;This is a modified string content&lt;/str&gt;
    &lt;/StackOverflow_7005187.MyDC&gt;</pre>
          </p>
          <p>
            <a name=""#request-json"">The following is an example request Json body:</a>
            <pre class=""request-json"">{
	    ""str"":""Some content in JSON""
    }</pre>
          </p>
          <p>
            <a name=""#response-xml"">The following is an example response Xml body:</a>
            <pre class=""response-xml"">&lt;StackOverflow_7005187.MyDC xmlns=""http://schemas.datacontract.org/2004/07/WcfForums""&gt;
      &lt;str&gt;Another modified XML content&lt;/str&gt;
    &lt;/StackOverflow_7005187.MyDC&gt;</pre>
          </p>
        </div>
      </body>
    </html>";
                return WebOperationContext.Current.CreateStreamResponse(
                    new MemoryStream(Encoding.UTF8.GetBytes(page)),
                    "text/html");
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), new WebHttpBinding(), "").Behaviors.Add(new WebHttpBehavior { HelpEnabled = false });
            host.Open();
            Console.WriteLine("Host opened");

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    // http://stackoverflow.com/q/7033442/751090
    public class StackOverflow_7033442
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            string Echo(string text);
        }

        public class Service : ITest
        {
            public string Echo(string text)
            {
                return text;
            }
        }

        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            CustomBinding binding = new CustomBinding(
                new CustomTextMessageBindingElement("iso-8859-1", "text/xml", MessageVersion.Soap11),
                new HttpTransportBindingElement());
            host.AddServiceEndpoint(typeof(ITest), binding, "");
            host.Open();
            Console.WriteLine("Host opened");

            string request = @"<?xml version=""1.0"" encoding=""iso-8859-1""?>
    <s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">
      <s:Body>
        <Echo xmlns=""http://tempuri.org/"">
          <text>Hello áéíóú</text>
        </Echo>
      </s:Body>
    </s:Envelope>";
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(baseAddress);
            req.Method = "POST";
            req.ContentType = "text/xml; charset=iso-8859-1";
            req.Headers["SOAPAction"] = "http://tempuri.org/ITest/Echo";
            Stream reqStream = req.GetRequestStream();
            Encoding encoding = Encoding.GetEncoding("iso-8859-1");
            byte[] reqBytes = encoding.GetBytes(request);
            reqStream.Write(reqBytes, 0, reqBytes.Length);
            reqStream.Close();

            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Console.WriteLine("HTTP/{0} {1} {2}", resp.ProtocolVersion, (int)resp.StatusCode, resp.StatusDescription);
            foreach (var header in resp.Headers.AllKeys)
            {
                Console.WriteLine("{0}: {1}", header, resp.Headers[header]);
            }

            if (resp.ContentLength > 0)
            {
                Console.WriteLine(new StreamReader(resp.GetResponseStream(), encoding).ReadToEnd());
            }

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }

        #region Custom Text Message Encoder sample, verbatim
        public class CustomTextMessageEncoder : MessageEncoder
        {
            private CustomTextMessageEncoderFactory factory;
            private XmlWriterSettings writerSettings;
            private string contentType;

            public CustomTextMessageEncoder(CustomTextMessageEncoderFactory factory)
            {
                this.factory = factory;

                this.writerSettings = new XmlWriterSettings();
                this.writerSettings.Encoding = Encoding.GetEncoding(factory.CharSet);
                this.contentType = string.Format("{0}; charset={1}",
                    this.factory.MediaType, this.writerSettings.Encoding.HeaderName);
            }

            public override string ContentType
            {
                get
                {
                    return this.contentType;
                }
            }

            public override string MediaType
            {
                get
                {
                    return factory.MediaType;
                }
            }

            public override MessageVersion MessageVersion
            {
                get
                {
                    return this.factory.MessageVersion;
                }
            }

            public override Message ReadMessage(ArraySegment<byte> buffer, BufferManager bufferManager, string contentType)
            {
                byte[] msgContents = new byte[buffer.Count];
                Array.Copy(buffer.Array, buffer.Offset, msgContents, 0, msgContents.Length);
                bufferManager.ReturnBuffer(buffer.Array);

                MemoryStream stream = new MemoryStream(msgContents);
                return ReadMessage(stream, int.MaxValue);
            }

            public override Message ReadMessage(Stream stream, int maxSizeOfHeaders, string contentType)
            {
                XmlReader reader = XmlReader.Create(stream);
                return Message.CreateMessage(reader, maxSizeOfHeaders, this.MessageVersion);
            }

            public override ArraySegment<byte> WriteMessage(Message message, int maxMessageSize, BufferManager bufferManager, int messageOffset)
            {
                MemoryStream stream = new MemoryStream();
                XmlWriter writer = XmlWriter.Create(stream, this.writerSettings);
                message.WriteMessage(writer);
                writer.Close();

                byte[] messageBytes = stream.GetBuffer();
                int messageLength = (int)stream.Position;
                stream.Close();

                int totalLength = messageLength + messageOffset;
                byte[] totalBytes = bufferManager.TakeBuffer(totalLength);
                Array.Copy(messageBytes, 0, totalBytes, messageOffset, messageLength);

                ArraySegment<byte> byteArray = new ArraySegment<byte>(totalBytes, messageOffset, messageLength);
                return byteArray;
            }

            public override void WriteMessage(Message message, Stream stream)
            {
                XmlWriter writer = XmlWriter.Create(stream, this.writerSettings);
                message.WriteMessage(writer);
                writer.Close();
            }
        }
        public class CustomTextMessageEncoderFactory : MessageEncoderFactory
        {
            private MessageEncoder encoder;
            private MessageVersion version;
            private string mediaType;
            private string charSet;

            internal CustomTextMessageEncoderFactory(string mediaType, string charSet,
                MessageVersion version)
            {
                this.version = version;
                this.mediaType = mediaType;
                this.charSet = charSet;
                this.encoder = new CustomTextMessageEncoder(this);
            }

            public override MessageEncoder Encoder
            {
                get
                {
                    return this.encoder;
                }
            }

            public override MessageVersion MessageVersion
            {
                get
                {
                    return this.version;
                }
            }

            internal string MediaType
            {
                get
                {
                    return this.mediaType;
                }
            }

            internal string CharSet
            {
                get
                {
                    return this.charSet;
                }
            }
        }
        public class CustomTextMessageBindingElement : MessageEncodingBindingElement, IWsdlExportExtension
        {
            private MessageVersion msgVersion;
            private string mediaType;
            private string encoding;
            private XmlDictionaryReaderQuotas readerQuotas;

            CustomTextMessageBindingElement(CustomTextMessageBindingElement binding)
                : this(binding.Encoding, binding.MediaType, binding.MessageVersion)
            {
                this.readerQuotas = new XmlDictionaryReaderQuotas();
                binding.ReaderQuotas.CopyTo(this.readerQuotas);
            }

            public CustomTextMessageBindingElement(string encoding, string mediaType,
                MessageVersion msgVersion)
            {
                if (encoding == null)
                    throw new ArgumentNullException("encoding");

                if (mediaType == null)
                    throw new ArgumentNullException("mediaType");

                if (msgVersion == null)
                    throw new ArgumentNullException("msgVersion");

                this.msgVersion = msgVersion;
                this.mediaType = mediaType;
                this.encoding = encoding;
                this.readerQuotas = new XmlDictionaryReaderQuotas();
            }

            public CustomTextMessageBindingElement(string encoding, string mediaType)
                : this(encoding, mediaType, MessageVersion.Soap11WSAddressing10)
            {
            }

            public CustomTextMessageBindingElement(string encoding)
                : this(encoding, "text/xml")
            {

            }

            public CustomTextMessageBindingElement()
                : this("UTF-8")
            {
            }

            public override MessageVersion MessageVersion
            {
                get
                {
                    return this.msgVersion;
                }

                set
                {
                    if (value == null)
                        throw new ArgumentNullException("value");
                    this.msgVersion = value;
                }
            }


            public string MediaType
            {
                get
                {
                    return this.mediaType;
                }

                set
                {
                    if (value == null)
                        throw new ArgumentNullException("value");
                    this.mediaType = value;
                }
            }

            public string Encoding
            {
                get
                {
                    return this.encoding;
                }

                set
                {
                    if (value == null)
                        throw new ArgumentNullException("value");
                    this.encoding = value;
                }
            }

            // This encoder does not enforces any quotas for the unsecure messages. The 
            // quotas are enforced for the secure portions of messages when this encoder
            // is used in a binding that is configured with security. 
            public XmlDictionaryReaderQuotas ReaderQuotas
            {
                get
                {
                    return this.readerQuotas;
                }
            }

            #region IMessageEncodingBindingElement Members

            public override MessageEncoderFactory CreateMessageEncoderFactory()
            {
                return new CustomTextMessageEncoderFactory(this.MediaType,
                    this.Encoding, this.MessageVersion);
            }

            #endregion


            public override BindingElement Clone()
            {
                return new CustomTextMessageBindingElement(this);
            }

            public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
            {
                if (context == null)
                    throw new ArgumentNullException("context");

                context.BindingParameters.Add(this);
                return context.BuildInnerChannelFactory<TChannel>();
            }

            public override bool CanBuildChannelFactory<TChannel>(BindingContext context)
            {
                if (context == null)
                    throw new ArgumentNullException("context");

                return context.CanBuildInnerChannelFactory<TChannel>();
            }

            public override IChannelListener<TChannel> BuildChannelListener<TChannel>(BindingContext context)
            {
                if (context == null)
                    throw new ArgumentNullException("context");

                context.BindingParameters.Add(this);
                return context.BuildInnerChannelListener<TChannel>();
            }

            public override bool CanBuildChannelListener<TChannel>(BindingContext context)
            {
                if (context == null)
                    throw new ArgumentNullException("context");

                context.BindingParameters.Add(this);
                return context.CanBuildInnerChannelListener<TChannel>();
            }

            public override T GetProperty<T>(BindingContext context)
            {
                if (typeof(T) == typeof(XmlDictionaryReaderQuotas))
                {
                    return (T)(object)this.readerQuotas;
                }
                else
                {
                    return base.GetProperty<T>(context);
                }
            }

            #region IWsdlExportExtension Members

            void IWsdlExportExtension.ExportContract(WsdlExporter exporter, WsdlContractConversionContext context)
            {
            }

            void IWsdlExportExtension.ExportEndpoint(WsdlExporter exporter, WsdlEndpointConversionContext context)
            {
                // The MessageEncodingBindingElement is responsible for ensuring that the WSDL has the correct
                // SOAP version. We can delegate to the WCF implementation of TextMessageEncodingBindingElement for this.
                TextMessageEncodingBindingElement mebe = new TextMessageEncodingBindingElement();
                mebe.MessageVersion = this.msgVersion;
                ((IWsdlExportExtension)mebe).ExportEndpoint(exporter, context);
            }

            #endregion
        }
        #endregion
    }

    // http://stackoverflow.com/q/7058942/751090
    public class StackOverflow_7058942
    {
        //[Serializable]
        public class Number
        {
            public int Number1 { get; set; }
            public int Number2 { get; set; }
        }
        [ServiceContract]
        public class Service
        {
            [OperationContract(Name = "Add")]
            [WebInvoke(UriTemplate = "test/", Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
            public int Add(Number n1)
            {
                int res = Convert.ToInt32(n1.Number1) + Convert.ToInt32(n1.Number2);
                return res;
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(Service), new WebHttpBinding(), "").Behaviors.Add(new WebHttpBehavior());
            host.Open();
            Console.WriteLine("Host opened");

            WebClient c = new WebClient();
            c.Headers[HttpRequestHeader.ContentType] = "application/json; charset=utf-8";
            c.Encoding = Encoding.UTF8;
            Console.WriteLine(c.UploadString(baseAddress + "/test/", "{\"Number1\":\"7\",\"Number2\":\"7\"}"));

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class TestRest
    {
        [ServiceContract]
        public interface ITest
        {
            [WebGet]
            int Add(int x, int y);
        }
        public class Service : ITest
        {
            public int Add(int x, int y)
            {
                //if (x == 0) throw new WebFaultException<string>("Hello here", HttpStatusCode.Conflict);
                return x + y;
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), new WebHttpBinding(), "").Behaviors.Add(new WebHttpBehavior
            {
                DefaultOutgoingResponseFormat = WebMessageFormat.Xml,
                FaultExceptionEnabled = false,
                AutomaticFormatSelectionEnabled = false
            });
            host.Open();
            Console.WriteLine("Host opened");

            Util.SendRequest(baseAddress + "/Add?x=0&y=1", "GET", null, null);
            Util.SendRequest(baseAddress + "/Add?x=0&y=1", "GET", null, null, new Dictionary<string, string> { { "Accept", "application/json" } });

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    // http://stackoverflow.com/q/7085678/751090
    public class StackOverflow_7085678
    {
        [ServiceContract]
        [XmlSerializerFormat]
        public class Service
        {
            [WebGet]
            public string GetData(string format)
            {
                WebMessageFormat responseFormat;
                if (Enum.TryParse(format, out responseFormat))
                {
                    WebOperationContext.Current.OutgoingResponse.Format = responseFormat;
                }

                return "Some string";
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            WebServiceHost host = new WebServiceHost(typeof(Service), new Uri(baseAddress));
            host.Open();
            Console.WriteLine("Host opened");

            WebClient c = new WebClient();
            Console.WriteLine(c.DownloadString(baseAddress + "/GetData"));

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_76644d43_cbaf_4817_87b1_cd014199035c
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            [WebInvoke(Method = "POST", UriTemplate = "/SampleService", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
            string SampleService(Person personObj);
        }
        public class Service : ITest
        {
            public string SampleService(Person personObj)
            {
                return string.Format("First={0},Middle={1},Last={2},Age={3}", personObj.FirstName, personObj.MiddleName, personObj.LastName, personObj.Age);
            }
        }
        [DataContract(Name = "Person", Namespace = "http://www.test.com")]
        public class Person
        {
            [DataMember(Name = "MiddleName")]
            public string MiddleName { get; set; }
            [DataMember(Name = "FirstName")]
            public string FirstName { get; set; }
            [DataMember(Name = "LastName")]
            public string LastName { get; set; }
            [DataMember(Name = "Age")]
            public string Age { get; set; }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), new WebHttpBinding(), "").Behaviors.Add(new WebHttpBehavior());
            host.Open();
            Console.WriteLine("Host opened");

            string outOfOrderXml = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Person xmlns=""http://www.test.com"">
  <MiddleName>mName</MiddleName>
  <FirstName>fName</FirstName>
  <LastName>lname</LastName>
  <Age>23</Age>
</Person>";
            string orderedXml = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Person xmlns=""http://www.test.com"">
  <Age>23</Age>
  <FirstName>fName</FirstName>
  <LastName>lname</LastName>
  <MiddleName>mName</MiddleName>
</Person>";

            WebChannelFactory<ITest> factory = new WebChannelFactory<ITest>(new Uri(baseAddress));
            Console.WriteLine(factory.CreateChannel().SampleService(new Person { Age = "23", FirstName = "f", MiddleName = "m", LastName = "l" }));

            WebClient c = new WebClient();
            c.Headers[HttpRequestHeader.ContentType] = "text/xml";
            Console.WriteLine(c.UploadString(baseAddress + "/SampleService", outOfOrderXml));
            Console.WriteLine();

            c = new WebClient();
            c.Headers[HttpRequestHeader.ContentType] = "text/xml";
            Console.WriteLine(c.UploadString(baseAddress + "/SampleService", orderedXml));

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    // http://stackoverflow.com/q/7141998/751090
    // public class StackOverflow_7141998
    public class Post_2ee8f228_b01b_4e80_9335_8c7fa23c9dd0
    {
        [MessageContract]
        public class MyMC
        {
            [MessageHeader(Name = "MyHeader", Namespace = "http://my.namespace.com")]
            public string HeaderValue { get; set; }
            [MessageBodyMember(Name = "MyBody", Namespace = "http://my.namespace.com")]
            public string BodyValue { get; set; }
        }
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            void Process(MyMC mc);
        }
        public class Service : ITest
        {
            public void Process(MyMC mc)
            {
                Console.WriteLine("Header value: {0}", mc.HeaderValue);
            }
        }
        public class MyInspector : IEndpointBehavior, IClientMessageInspector
        {
            public string NewHeaderValue { get; set; }

            public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
            {
            }

            public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
            {
                clientRuntime.MessageInspectors.Add(this);
            }

            public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
            {
            }

            public void Validate(ServiceEndpoint endpoint)
            {
            }

            public void AfterReceiveReply(ref Message reply, object correlationState)
            {
            }

            public object BeforeSendRequest(ref Message request, IClientChannel channel)
            {
                int originalIndex = request.Headers.FindHeader("MyHeader", "http://my.namespace.com");
                if (originalIndex >= 0)
                {
                    request.Headers.Insert(originalIndex, MessageHeader.CreateHeader("MyHeader", "http://my.namespace.com", this.NewHeaderValue));
                    request.Headers.RemoveAt(originalIndex + 1);
                }

                return null;
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            ServiceEndpoint endpoint = host.AddServiceEndpoint(typeof(ITest), new WSHttpBinding(), "");
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(new WSHttpBinding(), new EndpointAddress(baseAddress));
            MyInspector inspector = new MyInspector { NewHeaderValue = "Modified header value" };
            factory.Endpoint.Behaviors.Add(inspector);
            ITest proxy = factory.CreateChannel();

            proxy.Process(new MyMC { HeaderValue = "Original header value", BodyValue = "The body" });

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    // http://stackoverflow.com/q/7155154/751090
    public class StackOverflow_7155154
    {
        [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.1")]
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
        public partial class Orders
        {
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 0)]
            public int OrderID { get; set; }
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 1)]
            public string Description { get; set; }
        }
        [ServiceContract]
        [XmlSerializerFormat]
        public interface ITest
        {
            [OperationContract]
            Orders GetOrders();
        }
        public class Service : ITest
        {
            public Orders GetOrders()
            {
                return new Orders { Description = "My order", OrderID = 1 };
            }
        }
        public static void Test()
        {
            //MemoryStream ms = new MemoryStream();
            //XmlSerializer xs = new XmlSerializer(typeof(Orders));
            //Orders o = new Orders { OrderID = 1, Description = "My order" };
            //xs.Serialize(ms, o);
            //Console.WriteLine(Encoding.UTF8.GetString(ms.ToArray()));

            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), new BasicHttpBinding(), "");
            host.Description.Behaviors.Add(new ServiceMetadataBehavior { HttpGetEnabled = true });
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(new BasicHttpBinding(), new EndpointAddress(baseAddress));
            ITest proxy = factory.CreateChannel();

            Console.WriteLine(proxy.GetOrders());

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_e0545037_8a8b_47ea_8a9c_c0adaf9f7736
    {
        [DataContract(Name = "MyDC", Namespace = "http://my.namespace/dc")]
        public class MyDC
        {
            [DataMember(Order = 0)]
            public string str = "The string";
            [DataMember(Order = 1)]
            public int i = 1234;

            public override string ToString()
            {
                return string.Format("MyDC[str={0},i={1}]", str, i);
            }
        }
        [ServiceContract(Name = "ITest", Namespace = "http://my.namespace/sc")]
        public interface ITest
        {
            [OperationContract]
            MyDC GetDC(string str, int i);
        }
        [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
        public class Service : ITest
        {
            public MyDC GetDC(string str, int i)
            {
                return new MyDC { str = str, i = i };
            }
        }
        class MyInspector : IEndpointBehavior, IDispatchMessageInspector
        {
            public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
            {
            }

            public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
            {
            }

            public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
            {
                endpointDispatcher.DispatchRuntime.MessageInspectors.Add(this);
            }

            public void Validate(ServiceEndpoint endpoint)
            {
            }

            public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
            {
                int changeResponseIndex = request.Headers.FindHeader("ChangeResponse", "");
                if (changeResponseIndex >= 0)
                {
                    return request.Headers.GetHeader<bool>(changeResponseIndex);
                }

                return false;
            }

            public void BeforeSendReply(ref Message reply, object correlationState)
            {
                bool changeResponse = (bool)correlationState;
                if (changeResponse)
                {
                    Console.WriteLine("Original reply:");
                    Console.WriteLine(reply);
                    Console.WriteLine();
                    MemoryStream ms = new MemoryStream();
                    XmlWriter w = XmlWriter.Create(ms);
                    reply.WriteMessage(w);
                    w.Flush();
                    XmlDocument doc = new XmlDocument();
                    ms.Position = 0;
                    doc.Load(ms);
                    XmlNamespaceManager nsManager = new XmlNamespaceManager(doc.NameTable);
                    nsManager.AddNamespace("sc", "http://my.namespace/sc");
                    nsManager.AddNamespace("dc", "http://my.namespace/dc");
                    XmlElement iNode = (XmlElement)doc.SelectSingleNode("//sc:GetDCResponse/sc:GetDCResult/dc:i", nsManager);
                    iNode.FirstChild.Value = "not-an-integer";
                    Console.WriteLine("XmlDocument:");
                    Console.WriteLine(doc.OuterXml);
                    Console.WriteLine();
                    ms.SetLength(0);
                    w = XmlWriter.Create(ms);
                    doc.WriteTo(w);
                    w.Flush();
                    ms.Position = 0;
                    XmlReader r = XmlReader.Create(ms);
                    Message newReply = Message.CreateMessage(r, int.MaxValue, reply.Version);
                    newReply.Properties.CopyProperties(reply.Properties);
                    reply = newReply;
                    Console.WriteLine("New reply:");
                    Console.WriteLine(reply);
                    Console.WriteLine();
                }
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            ServiceEndpoint endpoint = host.AddServiceEndpoint(typeof(ITest), new BasicHttpBinding(), "");
            endpoint.Behaviors.Add(new MyInspector());
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(new BasicHttpBinding(), new EndpointAddress(baseAddress));
            ITest proxy = factory.CreateChannel();

            Console.WriteLine(proxy.GetDC("hello", 1));

            using (new OperationContextScope((IContextChannel)proxy))
            {
                try
                {
                    OperationContext.Current.OutgoingMessageHeaders.Add(MessageHeader.CreateHeader("ChangeResponse", "", true));
                    Console.WriteLine(proxy.GetDC("world", 2));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    // http://stackoverflow.com/q/7278569/751090
    public class StackOverflow_7278569
    {
        [DataContract(Namespace = "", Name = "OperationResultOf{0}")]
        public class OperationResult<T>
        {
            [DataMember]
            public T Data { get; set; }
        }
        public class FacebookData
        {
            public string token;
        }
        [ServiceContract]
        public class Service
        {
            [OperationContract]
            [WebGet(UriTemplate = "FacebookData/?accessToken={accessToken}")]
            public OperationResult<FacebookData> GetFacebookData(string accessToken)
            {
                return new OperationResult<FacebookData>
                {
                    Data = new FacebookData { token = accessToken }
                };
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            WebServiceHost host = new WebServiceHost(typeof(Service), new Uri(baseAddress));
            host.Open();
            Console.WriteLine("Host opened");

            WebClient c = new WebClient();
            Console.WriteLine(c.DownloadString(baseAddress + "/FacebookData/?accessToken=abcd"));

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_e70439f9_fa99_4f88_a93d_c8ecd4a257f8
    {
        [DataContract(Name = "FileInfo", Namespace = "http://schemas.datacontract.org/2004/07/System.IO")]
        public class FileInfoEquivalent
        {
            [DataMember(Order = 1, Name = "OriginalPath")]
            public string OriginalPath;
            [DataMember(Order = 2, Name = "FullPath")]
            public string FullPath;
        }
        //[ServiceContract]
        //public interface ITest
        //{
        //    [OperationContract]
        //    string EchoString(string text);
        //    [OperationContract]
        //    int Add(int x, int y);
        //    [OperationContract]
        //    MyDC EchoDC(MyDC input);
        //}
        //public class Service : ITest
        //{
        //    public string EchoString(string text)
        //    {
        //        return text;
        //    }

        //    public int Add(int x, int y)
        //    {
        //        return x + y;
        //    }

        //    public MyDC EchoDC(MyDC input)
        //    {
        //        return input;
        //    }
        //}
        //static Binding GetBinding()
        //{
        //    BasicHttpBinding result = new BasicHttpBinding();
        //    return result;
        //}
        public static void Test()
        {
            MemoryStream ms = new MemoryStream();
            string path = @"C:\temp\a.txt";
            FileInfo fi = new FileInfo(path);
            new DataContractSerializer(typeof(FileInfo)).WriteObject(ms, fi);
            Console.WriteLine(Encoding.UTF8.GetString(ms.ToArray()));

            Console.WriteLine();

            ms.SetLength(0);
            new DataContractSerializer(typeof(FileInfoEquivalent)).WriteObject(ms, new FileInfoEquivalent { OriginalPath = @"C:\temp\a.txt", FullPath = path });
            Console.WriteLine(Encoding.UTF8.GetString(ms.ToArray()));
            ms.Position = 0;

            fi = (FileInfo)(new DataContractSerializer(typeof(FileInfo)).ReadObject(ms));
            Console.WriteLine(fi);
            //string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            //ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            //host.AddServiceEndpoint(typeof(ITest), GetBinding(), "");
            //host.Open();
            //Console.WriteLine("Host opened");

            //ChannelFactory<ITest> factory = new ChannelFactory<ITest>(GetBinding(), new EndpointAddress(baseAddress));
            //ITest proxy = factory.CreateChannel();

            //Console.WriteLine(proxy.EchoString("Hello"));
            //Console.WriteLine(proxy.EchoDC(new MyDC()));
            //Console.WriteLine(proxy.Add(3, 5));

            //((IClientChannel)proxy).Close();
            //factory.Close();

            //Console.Write("Press ENTER to close the host");
            //Console.ReadLine();
            //host.Close();
        }
    }

    public class Post_743d0eea_1e80_4cc7_8ec7_e0923d256b05
    {
        const string ClientNamespace = "www.shepherdcolor.com";
        const string ServerNamespace = "shco-appsrv1.us.shepherd.ad";

        [DataContract(Name = "MyDC", Namespace = ClientNamespace)]
        public class MyDCClient
        {
            [DataMember]
            public int Value;
        }

        [DataContract(Name = "MyDC", Namespace = ServerNamespace)]
        public class MyDCServer
        {
            [DataMember]
            public int Value;
        }

        [ServiceContract(Name = "ITest")]
        public interface ITest
        {
            [OperationContract]
            MyDCServer Add(MyDCServer x, MyDCServer y);
        }
        [ServiceContract(Name = "ITest")]
        public interface ITestClient
        {
            [OperationContract]
            MyDCClient Add(MyDCClient x, MyDCClient y);
        }
        public class Service : ITest
        {
            public MyDCServer Add(MyDCServer x, MyDCServer y)
            {
                return new MyDCServer { Value = x.Value + y.Value };
            }
        }
        public class MyInspector : IDispatchMessageInspector, IEndpointBehavior
        {
            public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
            {
            }

            public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
            {
            }

            public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
            {
                endpointDispatcher.DispatchRuntime.MessageInspectors.Add(this);
            }

            public void Validate(ServiceEndpoint endpoint)
            {
            }

            public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
            {
                request = ChangeString(request, ClientNamespace, ServerNamespace);
                return null;
            }

            public void BeforeSendReply(ref Message reply, object correlationState)
            {
                reply = ChangeString(reply, ServerNamespace, ClientNamespace);
            }

            public Message ChangeString(Message oldMessage, string from, string to)
            {
                MemoryStream ms = new MemoryStream();
                XmlWriter xw = XmlWriter.Create(ms);
                oldMessage.WriteMessage(xw);
                xw.Flush();
                string body = Encoding.UTF8.GetString(ms.ToArray());
                xw.Close();

                body = body.Replace(from, to);

                ms = new MemoryStream(Encoding.UTF8.GetBytes(body));
                XmlDictionaryReader xdr = XmlDictionaryReader.CreateTextReader(ms, new XmlDictionaryReaderQuotas());
                Message newMessage = Message.CreateMessage(xdr, int.MaxValue, oldMessage.Version);
                newMessage.Properties.CopyProperties(oldMessage.Properties);
                return newMessage;

                //Message newMessage = null;
                //var msgBuf = oldMessage.CreateBufferedCopy(int.MaxValue);
                //var nav = msgBuf.CreateNavigator();

                //var ms = new MemoryStream();
                //var xw = XmlWriter.Create(ms);
                //nav.WriteSubtree(xw);
                //xw.Flush();
                //xw.Close();

                //ms.Position = 0;
                //var xdoc = XDocument.Load(XmlReader.Create(ms));
                //var body = xdoc.ToString();
                //body = body.Replace(from, to);
                //xdoc = XDocument.Parse(body);

                //xw = XmlWriter.Create(ms);
                //ms.Position = 0;
                //xdoc.Save(xw);
                //xw.Flush();
                //xw.Close();

                //ms.Position = 0;
                //XmlDictionaryReader xdr = XmlDictionaryReader.CreateTextReader(
                //    ms, new XmlDictionaryReaderQuotas()
                //    );
                //newMessage = Message.CreateMessage(xdr, int.MaxValue, oldMessage.Version);
                ////newMessage.Headers.CopyHeadersFrom(oldMessage.Headers);
                //newMessage.Properties.CopyProperties(oldMessage.Properties);
                //return newMessage;
            }
        }
        static Binding GetBinding()
        {
            return new BasicHttpBinding();
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            ServiceEndpoint endpoint = host.AddServiceEndpoint(typeof(ITest), GetBinding(), "");
            endpoint.Behaviors.Add(new MyInspector());
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITestClient> factory = new ChannelFactory<ITestClient>(GetBinding(), new EndpointAddress(baseAddress));
            ITestClient proxy = factory.CreateChannel();

            Console.WriteLine(proxy.Add(new MyDCClient { Value = 3 }, new MyDCClient { Value = 5 }).Value);

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    // http://stackoverflow.com/q/7348240/751090
    public class StackOverflow_7348240
    {
        [Serializable]
        [DataContract(Name = "myRoot", Namespace = "")]
        [XmlRoot(ElementName = "myRoot", Namespace = "")]
        public class MyType
        {
            private string dataField;

            [XmlElement(ElementName = "data")]
            [DataMember(Name = "data")]
            public string Data
            {
                get { return this.dataField; }
                set { this.dataField = value; }
            }
        }

        public static void Test()
        {
            MyType obj = new MyType { Data = "hello world" };

            MemoryStream ms = new MemoryStream();
            new DataContractSerializer(obj.GetType()).WriteObject(ms, obj);
            Console.WriteLine(Encoding.UTF8.GetString(ms.ToArray()));

            ms.SetLength(0);
            new XmlSerializer(obj.GetType()).Serialize(ms, obj);
            Console.WriteLine(Encoding.UTF8.GetString(ms.ToArray()));
        }
    }

    // http://stackoverflow.com/q/7348240/751090
    public class StackOverflow_7348240_b
    {
        [DataContract(Name = "myRoot", Namespace = "")]
        [XmlRoot(ElementName = "myRoot", Namespace = "")]
        public class MyType
        {
            [DataMember(Name = "myArray")]
            [XmlArray(ElementName = "myArray")]
            [XmlArrayItem(ElementName = "url")]
            public MyArray myArray;
        }

        [CollectionDataContract(Name = "myArray", Namespace = "", ItemName = "url")]
        [XmlType(Namespace = "")]
        [XmlRoot(ElementName = "myArray", Namespace = "")]
        public class MyArray : List<string>
        {
        }

        public static void Test()
        {
            MyType obj = new MyType { myArray = new MyArray { "one", "two" } };

            MemoryStream ms = new MemoryStream();
            new DataContractSerializer(obj.GetType()).WriteObject(ms, obj);
            Console.WriteLine(Encoding.UTF8.GetString(ms.ToArray()));

            ms.SetLength(0);
            new XmlSerializer(obj.GetType()).Serialize(ms, obj);
            Console.WriteLine(Encoding.UTF8.GetString(ms.ToArray()));
        }
    }

    // http://stackoverflow.com/q/7360920/751090
    public class StackOverflow_7360920
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            int Add(int x, int y);
        }
        //[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
        public class Service : ITest
        {
            public Service()
            {
                Console.WriteLine(OperationContext.Current.Host.Description.Behaviors.Find<ServiceBehaviorAttribute>().InstanceContextMode);
            }
            public int Add(int x, int y)
            {
                return x + y;
            }
        }
        static Binding GetBinding()
        {
            BasicHttpBinding result = new BasicHttpBinding();
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

            Console.WriteLine(proxy.Add(3, 5));

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    // http://stackoverflow.com/q/7374609/751090
    public class StackOverflow_7374609
    {
        [XmlRoot(ElementName = "MyType", Namespace = "")]
        public class MyType
        {
            [XmlText]
            public string Value;
        }
        static void PrintChars(string str)
        {
            string toEscape = "\r\n\t\b";
            string escapeChar = "rntb";
            foreach (char c in str)
            {
                if (' ' <= c && c <= '~')
                {
                    Console.WriteLine(c);
                }
                else
                {
                    int escapeIndex = toEscape.IndexOf(c);
                    if (escapeIndex >= 0)
                    {
                        Console.WriteLine("\\{0}", escapeChar[escapeIndex]);
                    }
                    else
                    {
                        Console.WriteLine("\\u{0:X4}", (int)c);
                    }
                }
            }

            Console.WriteLine();
        }
        public static void Test()
        {
            string serialized = "<MyType>Hello\r\nworld</MyType>";
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(serialized));
            XmlSerializer xs = new XmlSerializer(typeof(MyType));
            MyType obj = (MyType)xs.Deserialize(ms);
            Console.WriteLine("Without the replacement");
            PrintChars(obj.Value);

            serialized = serialized.Replace("\r", "&#xD;");
            ms = new MemoryStream(Encoding.UTF8.GetBytes(serialized));
            obj = (MyType)xs.Deserialize(ms);
            Console.WriteLine("With the replacement");
            PrintChars(obj.Value);
        }
    }

    public class Post_79533008_cb7f_478d_9f34_24ed85f5cd51
    {
        [DataContract]
        public class MyHolder
        {
            [DataMember]
            public object Value;

            public override string ToString()
            {
                return string.Format("MyHolder[Value={0}]", this.Value);
            }
        }
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            MyHolder EchoHolder(MyHolder holder);
        }
        public class Service : ITest
        {
            public MyHolder EchoHolder(MyHolder holder)
            {
                return holder;
            }
        }
        private static void SetResolver(ServiceEndpoint endpoint)
        {
            foreach (var operation in endpoint.Contract.Operations)
            {
                DataContractSerializerOperationBehavior dcsob = operation.Behaviors.Find<DataContractSerializerOperationBehavior>();
                if (dcsob != null)
                {
                    dcsob.DataContractResolver = new DynamicTypeResolver(ReflectionEmitHelper.CreatePersonType());
                }
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            ServiceEndpoint endpoint = host.AddServiceEndpoint(typeof(ITest), new BasicHttpBinding(), "");
            SetResolver(endpoint);
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(new BasicHttpBinding(), new EndpointAddress(baseAddress));
            SetResolver(factory.Endpoint);
            ITest proxy = factory.CreateChannel();

            Type personType = ReflectionEmitHelper.CreatePersonType();
            object person = Activator.CreateInstance(personType);
            personType.GetProperty("Name").SetValue(person, "John Doe", null);
            personType.GetProperty("Age").SetValue(person, 22, null);
            Console.WriteLine("Person: {0}", person);

            MyHolder holder = new MyHolder { Value = person };
            Console.WriteLine("Sending holder over the wire: {0}", holder);
            var newHolder = proxy.EchoHolder(holder);
            Console.WriteLine("Received holder over the wire: {0}", newHolder);

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }

        class DynamicTypeResolver : DataContractResolver
        {
            Type dynamicType;
            public DynamicTypeResolver(Type dynamicType)
            {
                this.dynamicType = dynamicType;
            }

            public override Type ResolveName(string typeName, string typeNamespace, Type declaredType, DataContractResolver knownTypeResolver)
            {
                if (typeNamespace == "http://dynamic/" + ReflectionEmitHelper.AssemblyName && typeName == this.dynamicType.Name)
                {
                    return this.dynamicType;
                }
                else
                {
                    return knownTypeResolver.ResolveName(typeName, typeNamespace, declaredType, null);
                }
            }

            public override bool TryResolveType(Type type, Type declaredType, DataContractResolver knownTypeResolver, out XmlDictionaryString typeName, out XmlDictionaryString typeNamespace)
            {
                if (type.Assembly.GetName().Name == ReflectionEmitHelper.AssemblyName)
                {
                    XmlDictionary dic = new XmlDictionary();
                    typeName = dic.Add(type.Name);
                    typeNamespace = dic.Add("http://dynamic/" + ReflectionEmitHelper.AssemblyName);
                    return true;
                }
                else
                {
                    return knownTypeResolver.TryResolveType(type, declaredType, null, out typeName, out typeNamespace);
                }
            }
        }

        static class ReflectionEmitHelper
        {
            public const string AssemblyName = "MyReflectionEmitAssembly";
            static Type personType;

            internal static Type CreatePersonType()
            {
                if (personType == null)
                {
                    AppDomain myDomain = AppDomain.CurrentDomain;
                    AssemblyName myAsmName = new AssemblyName(AssemblyName);
                    AssemblyBuilder myAssembly =
                        myDomain.DefineDynamicAssembly(myAsmName,
                            AssemblyBuilderAccess.RunAndSave);

                    ModuleBuilder myModule =
                        myAssembly.DefineDynamicModule(myAsmName.Name,
                           myAsmName.Name + ".dll");

                    // public class Person
                    TypeBuilder personTypeBuilder =
                        myModule.DefineType("Person", TypeAttributes.Public);

                    // Add [DataContract] to the type
                    Type dcaType = typeof(DataContractAttribute);
                    CustomAttributeBuilder dataContractBuilder = new CustomAttributeBuilder(
                        dcaType.GetConstructor(Type.EmptyTypes),
                        new object[0],
                        new PropertyInfo[] { dcaType.GetProperty("Name"), dcaType.GetProperty("Namespace") },
                        new object[] { "Person", "http://my.dynamic.namespace" });
                    personTypeBuilder.SetCustomAttribute(dataContractBuilder);

                    // Define some fields
                    FieldBuilder nameField =
                        personTypeBuilder.DefineField(
                            "name",
                            typeof(string),
                            FieldAttributes.Private);
                    FieldBuilder ageField =
                        personTypeBuilder.DefineField(
                            "age",
                            typeof(int),
                            FieldAttributes.Private);

                    // Define the public properties
                    MethodBuilder get_Name;
                    MethodBuilder get_Age;
                    PropertyBuilder nameProperty = CreateProperty(personTypeBuilder, "Name", nameField, out get_Name);
                    PropertyBuilder ageProperty = CreateProperty(personTypeBuilder, "Age", ageField, out get_Age);

                    // Override ToString
                    MethodBuilder toString = personTypeBuilder.DefineMethod(
                        "ToString",
                        MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual,
                        typeof(string),
                        Type.EmptyTypes);

                    MethodInfo stringFormat = typeof(string).GetMethod(
                        "Format",
                        BindingFlags.Static | BindingFlags.Public,
                        null,
                        new Type[] { typeof(string), typeof(object), typeof(object) },
                        null);

                    ILGenerator toStringIL = toString.GetILGenerator();
                    toStringIL.Emit(OpCodes.Ldstr, "Person[Name={0},Age={1}]");
                    toStringIL.Emit(OpCodes.Ldarg_0);
                    toStringIL.Emit(OpCodes.Call, get_Name);
                    toStringIL.Emit(OpCodes.Ldarg_0);
                    toStringIL.Emit(OpCodes.Call, get_Age);
                    toStringIL.Emit(OpCodes.Box, typeof(int));
                    toStringIL.Emit(OpCodes.Call, stringFormat);
                    toStringIL.Emit(OpCodes.Ret);

                    personType = personTypeBuilder.CreateType();
                }

                return personType;
            }

            private static PropertyBuilder CreateProperty(TypeBuilder typeBuilder, string propertyName, FieldBuilder backingField, out MethodBuilder getter)
            {
                Type propertyType = backingField.FieldType;
                PropertyBuilder property = typeBuilder.DefineProperty(
                    propertyName,
                    PropertyAttributes.None,
                    propertyType,
                    Type.EmptyTypes);

                Type dmaType = typeof(DataMemberAttribute);
                CustomAttributeBuilder dataMemberAttribute = new CustomAttributeBuilder(
                    dmaType.GetConstructor(Type.EmptyTypes),
                    new object[0],
                    new PropertyInfo[] { dmaType.GetProperty("Name") },
                    new object[] { propertyName });
                property.SetCustomAttribute(dataMemberAttribute);

                MethodAttributes getSetAttr = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;

                MethodBuilder getMethod = typeBuilder.DefineMethod("get_" + propertyName, getSetAttr, propertyType, Type.EmptyTypes);
                MethodBuilder setMethod = typeBuilder.DefineMethod("set_" + propertyName, getSetAttr, null, new Type[] { propertyType });

                getter = getMethod;

                ILGenerator getIL = getMethod.GetILGenerator();
                getIL.Emit(OpCodes.Ldarg_0);
                getIL.Emit(OpCodes.Ldfld, backingField);
                getIL.Emit(OpCodes.Ret);

                ILGenerator setIL = setMethod.GetILGenerator();
                setIL.Emit(OpCodes.Ldarg_0);
                setIL.Emit(OpCodes.Ldarg_1);
                setIL.Emit(OpCodes.Stfld, backingField);
                setIL.Emit(OpCodes.Ret);

                property.SetSetMethod(setMethod);
                property.SetGetMethod(getMethod);

                return property;
            }
        }
    }

    // http://stackoverflow.com/q/7479187/751090
    public class StackOverflow_7479187
    {
        [DataContract]
        public class MyDC
        {
            [DataMember]
            public string str = "The string";
        }
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            string EchoString(string text);
            [OperationContract]
            int Add(int x, int y);
            [OperationContract]
            MyDC EchoDC(MyDC input);
        }
        public class Service : ITest
        {
            public string EchoString(string text)
            {
                return text;
            }

            public int Add(int x, int y)
            {
                return x + y;
            }

            public MyDC EchoDC(MyDC input)
            {
                return input;
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), new BasicHttpBinding(), "basic");
            host.AddServiceEndpoint(typeof(ITest), new WSHttpBinding(), "ws");
            host.AddServiceEndpoint(typeof(ITest), new WebHttpBinding(), "web");
            host.Description.Behaviors.Add(new ServiceMetadataBehavior { HttpGetEnabled = true });
            host.Open();
            Console.WriteLine("Host opened");

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    // http://stackoverflow.com/q/7974435/751090
    public class StackOverflow_7974435
    {
        [ServiceContract]
        public class Service
        {
            [WebGet(UriTemplate = "/Sum?x={x}&y={y}")]
            public int Add(int x, int y)
            {
                return x + y;
            }
            [WebGet(UriTemplate = "/Data?isNull={isNull}")]
            public string GetData(bool isNull)
            {
                return isNull ? null : "Hello world";
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            WebHttpBinding binding = new WebHttpBinding { CrossDomainScriptAccessEnabled = true };
            WebHttpBehavior behavior = new WebHttpBehavior { DefaultOutgoingResponseFormat = WebMessageFormat.Json };
            host.AddServiceEndpoint(typeof(Service), binding, "").Behaviors.Add(behavior);
            host.Open();
            Console.WriteLine("Host opened");

            WebClient c = new WebClient();
            Console.WriteLine("Not a JSONP call");
            Console.WriteLine(c.DownloadString(baseAddress + "/Sum?x=6&y=8"));

            Console.WriteLine("A JSONP call");
            Console.WriteLine(c.DownloadString(baseAddress + "/Sum?x=6&y=8&callback=MyFunction"));

            Console.WriteLine("A JSONP call returning string");
            Console.WriteLine(c.DownloadString(baseAddress + "/Data?isNull=false&callback=MyFunction"));

            Console.WriteLine("A JSONP call returning null");
            Console.WriteLine(c.DownloadString(baseAddress + "/Data?isNull=true&callback=MyFunction"));

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public static class Post_55b943ea_a99b_46bc_948a_8c06d8b6605f
    {
        public static XmlDocument SerializeToXmlDoc<T>(this List<T> list)
        {
            DataContractSerializer dcs = new DataContractSerializer(typeof(List<T>));
            MemoryStream ms = new MemoryStream();
            dcs.WriteObject(ms, list);
            ms.Position = 0;
            XmlDocument doc = new XmlDocument();
            doc.Load(ms);
            return doc;
        }
        public static void Test()
        {
            List<int> l1 = new List<int> { 1, 2, 3, 4 };
            Console.WriteLine(l1.SerializeToXmlDoc().OuterXml);
            List<string> l2 = new List<string> { "hello", "world" };
            Console.WriteLine(l2.SerializeToXmlDoc().OuterXml);
        }
    }

    public class Post_4e1e619a_bbfd_43c2_86c1_ace3af8c0f91
    {
        [KnownType(typeof(Child))]
        public class Base
        {
            public bool IsNew { get; set; }
        }
        public class Child : Base
        {
            public string Name { get; set; }
            public override string ToString()
            {
                return string.Format("Child[IsNew={0},Name={1}]", IsNew, Name);
            }
        }
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            Base[] Echo(Base[] input);
        }
        public class Service : ITest
        {
            public Base[] Echo(Base[] input)
            {
                return input;
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

            Child[] cClass = new Child[] 
            { 
                new Child { IsNew = true, Name = "C1" },
                new Child { IsNew = true, Name = "C2" }
            };
            Base[] bClass = cClass;
            Child[] cClass2 = (Child[])bClass;

            Child[] cClass3 = (Child[])proxy.Echo(cClass);

            Console.WriteLine(cClass3[0]);
            Console.WriteLine(cClass3[1]);

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    // http://stackoverflow.com/q/8472985/751090
    public class StackOverflow_8472985
    {
        [ServiceContract]
        public class Service
        {
            [WebGet]
            public int DiffDates(DateTime startDate, DateTime endDate)
            {
                Console.WriteLine("[service] startDate: {0}", startDate);
                Console.WriteLine("[service] endDate: {0}", endDate);
                return (int)endDate.Subtract(startDate).TotalDays;
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            WebServiceHost host = new WebServiceHost(typeof(Service), new Uri(baseAddress));
            host.Open();
            Console.WriteLine("Host opened");

            WebClient c = new WebClient();
            DateTime startDate = new DateTime(2011, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime endDate = new DateTime(2011, 12, 12, 0, 0, 0, DateTimeKind.Utc);

            string uri = string.Format(baseAddress + "/DiffDates?startdate={0}&endDate={1}",
                startDate.ToString("yyyy-MM-dd"),
                endDate.ToString("yyyy-MM-dd"));

            Console.WriteLine("URI: {0}", uri);
            Console.WriteLine(c.DownloadString(uri));

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_73c1b1c2_8085_46d0_9635_efcef68bcc70
    {
        public class Person
        {
            public string Name;
            public int Age;
            public Address Address;
        }
        public class Address
        {
            public string Street;
            public string City;
            public string State;
        }
        public static string SerializeToYaml(object obj)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("---");
            SerializeToYaml(sb, "", obj);
            sb.AppendLine("...");
            return sb.ToString();
        }
        private static void SerializeToYaml(StringBuilder sb, string indent, object obj)
        {
            Type type = obj.GetType();
            foreach (FieldInfo field in type.GetFields())
            {
                string name = field.Name;
                object value = field.GetValue(obj);
                switch (Type.GetTypeCode(value.GetType()))
                {
                    case TypeCode.String:
                    case TypeCode.Int32:
                        sb.Append(indent);
                        sb.Append(name);
                        sb.Append(": ");
                        sb.Append(value);
                        sb.AppendLine();
                        break;
                    case TypeCode.Object:
                        sb.Append(indent);
                        sb.Append(name);
                        sb.AppendLine(":");
                        SerializeToYaml(sb, indent + "    ", value);
                        break;
                    default:
                        throw new ArgumentException("Not implemented yet");
                }
            }
        }
        [ServiceContract]
        public class Service
        {
            [WebGet]
            public Stream GetYaml()
            {
                Person p = new Person
                {
                    Name = "John Doe",
                    Age = 25,
                    Address = new Address { Street = "123 Tornado Alley", City = "East Centerville", State = "KS" },
                };
                WebOperationContext.Current.OutgoingResponse.ContentType = "text/plain";
                return new MemoryStream(Encoding.UTF8.GetBytes(SerializeToYaml(p)));
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            WebServiceHost host = new WebServiceHost(typeof(Service), new Uri(baseAddress));
            host.Open();
            Console.WriteLine("Host opened");

            WebClient c = new WebClient();
            Console.WriteLine(c.DownloadString(baseAddress + "/GetYaml"));

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Post_73c1b1c2_8085_46d0_9635_efcef68bcc70.Test();
        }
    }
}
