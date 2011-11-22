using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using UtilCS;
using System.ServiceModel.Activation;
using System.Xml.Linq;
using System.IO;
using System.Xml;
using System.Net;
using System.Collections.Specialized;
using System.ServiceModel.Dispatcher;
using System.Runtime.Serialization;
using System.Threading;
using System.Xml.Serialization;
using System.Dynamic;
using System.Reflection;
using System.ServiceModel.Configuration;
using System.Runtime.Serialization.Json;
using System.CodeDom;
using System.Configuration;
using System.Collections.ObjectModel;
using System.CodeDom.Compiler;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Drawing;
using System.ComponentModel;
using System.IO.Compression;
using System.ServiceModel.Discovery;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Web;
using System.Threading.Tasks;
using System.Xml.Schema;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Globalization;

namespace QuickCode1
{
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
            byte[] buffer = new byte[1000000];
            int bytesRead;
            long totalBytesRead = 0;
            int i = 0;
            bool debug = false;
            do
            {
                bytesRead = stream.Read(buffer, 0, buffer.Length);
                totalBytesRead += bytesRead;
                if (debug)
                {
                    i++;
                    if ((i % 100) == 0)
                    {
                        Console.WriteLine("Already counted {0} ({1:.000}) bytes", totalBytesRead, Math.Log10(totalBytesRead));
                    }
                }
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
                MaxReceivedMessageSize = long.MaxValue,
                SendTimeout = TimeSpan.FromHours(1),
                ReceiveTimeout = TimeSpan.FromHours(1),
                OpenTimeout = TimeSpan.FromHours(1),
                CloseTimeout = TimeSpan.FromHours(1),
            };
            binding.ReaderQuotas.MaxArrayLength = int.MaxValue;
            host.AddServiceEndpoint(typeof(ITest), binding, "").Behaviors.Add(new WebHttpBehavior());
            host.Open();
            Console.WriteLine("Host opened");

            //ChannelFactory<ITest> factory = new ChannelFactory<ITest>(binding, new EndpointAddress(baseAddress));
            //factory.Endpoint.Behaviors.Add(new WebHttpBehavior());
            //ITest proxy = factory.CreateChannel();

            //long length = 10000000;
            //length *= 500;
            //proxy.UploadFile("test.txt", new MyReadonlyStream(length));

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
            req.ReadWriteTimeout = int.MaxValue;
            req.Method = "POST";
            req.SendChunked = true;
            req.AllowWriteStreamBuffering = false;
            req.ServicePoint.UseNagleAlgorithm = false;
            req.ContentType = "application/octet-stream";
            Stream reqStream = req.GetRequestStream();
            byte[] buffer = new byte[10000000];
            long bytesWritten = 0;
            for (int i = 0; i < 500; i++)
            {
                reqStream.Write(buffer, 0, buffer.Length);
                bytesWritten += buffer.Length;
                if ((i % 10) == 0)
                {
                    Console.WriteLine("Wrote {0} bytes ({1:.000})", bytesWritten, Math.Log10(bytesWritten));
                }
            }
            reqStream.Close();
            resp = (HttpWebResponse)req.GetResponse();
            Console.WriteLine(resp.StatusCode);
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
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            int AddNormal(int x, int y);
            [OperationContract]
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
            ws.IndentChars = "  ";
            ws.OmitXmlDeclaration = true;
            ws.Encoding = Encoding.UTF8;
            MemoryStream ms = new MemoryStream();
            XmlWriter w = XmlWriter.Create(ms, ws);
            w.WriteNode(r, false);
            w.Flush();
            Console.WriteLine(Encoding.UTF8.GetString(ms.ToArray()));
        }
    }

    public class JsonpTest
    {
        [ServiceContract]
        public interface ITest
        {
            [WebGet(ResponseFormat = WebMessageFormat.Json)]
            int Add(int x, int y);
            [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
            int AddPost(int x, int y);
        }
        public class Service : ITest
        {
            public int Add(int x, int y) { return x + y; }
            public int AddPost(int x, int y) { return x + y; }
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

            Util.SendRequest(baseAddress + "/AddPost", "POST", "application/json", "{\"x\":3, \"y\":8}");
            Util.SendRequest(baseAddress + "/AddPost?callback=myFunc", "POST", "application/json", "{\"x\":3, \"y\":8}");

            Console.WriteLine("Press ENTER to close");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_3ed11dd3_f8cb_4b27_a1b3_bece4123e2af
    {
        [ServiceContract]
        public interface ITest
        {
            [System.ComponentModel.Description("Get a list of active appointments")]
            [WebGet(UriTemplate = "GetAppointments")]
            string GetAppointments();

            [System.ComponentModel.Description("Get a list of active appointments")]
            [WebGet(UriTemplate = "GetAppointmentsXml")]
            XElement GetAppointmentsXml();

            [System.ComponentModel.Description("Get a list of active appointments")]
            [WebGet(UriTemplate = "GetAppointmentsXmlDoc")]
            Stream GetAppointmentsXmlDoc();
        }
        public class Service : ITest
        {
            class AppointmentType
            {
                public int AppointmentID { get; set; }
                public DateTime AppointmentDateTime { get; set; }
                public int ProviderID { get; set; }
                public int Appointment { get; set; }
            }
            public XElement GetAppointmentsXml()
            {
                XDocument doc = CreateXDocument();
                return doc.Root;
            }
            public string GetAppointments()
            {
                XDocument doc = CreateXDocument();
                return doc.ToString(SaveOptions.None);
            }
            public Stream GetAppointmentsXmlDoc()
            {
                XDocument doc = CreateXDocument();
                string result = doc.ToString(SaveOptions.None);
                WebOperationContext.Current.OutgoingResponse.ContentType = "text/xml; charset=utf-8";
                return new MemoryStream(Encoding.UTF8.GetBytes(result));
            }
            XDocument CreateXDocument()
            {
                List<AppointmentType> appointments = new List<AppointmentType>
                {
                    new AppointmentType { AppointmentID = 1, AppointmentDateTime = new DateTime(2010, 7, 26, 15, 0, 0), ProviderID = 333, Appointment = 1 },
                    new AppointmentType { AppointmentID = 2, AppointmentDateTime = new DateTime(2010, 7, 26, 15, 30, 0), ProviderID = 444, Appointment = 2 },
                    new AppointmentType { AppointmentID = 3, AppointmentDateTime = new DateTime(2010, 7, 26, 16, 0, 0), ProviderID = 333, Appointment = 3 },
                };
                XDocument doc = new XDocument(
                            new XDeclaration("1.0", "utf-8", "yes"),
                            new XComment("Get Appointments service response"),
                            new XElement("getAppointmentsResp",
                                from appointment in appointments
                                select new XElement("appointment",
                                    new XElement("appointmentId", appointment.AppointmentID.ToString()),
                                    new XElement("appointmentDateTime", appointment.AppointmentDateTime.ToShortDateString()),
                                    new XElement("providerId", appointment.ProviderID.ToString()),
                                    new XElement("appointment", appointment.Appointment.ToString())
                                )
                            ));
                return doc;
            }
        }

        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            WebHttpBinding binding = new WebHttpBinding();
            ServiceEndpoint endpoint = host.AddServiceEndpoint(typeof(ITest), binding, "");
            WebHttpBehavior behavior = new WebHttpBehavior();
            behavior.HelpEnabled = true;
            endpoint.Behaviors.Add(behavior);
            host.Open();

            Console.WriteLine("Host opened at {0}", baseAddress);

            Console.WriteLine("Press ENTER to close");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_93991bf1_02f1_4328_a648_9596cb9ca849
    {
        public class User
        {
            public string Name;
            public int Age;
            public override string ToString()
            {
                return String.Format("User[Name={0},Age={1}]", Name, Age);
            }
        }
        [ServiceContract]
        public interface IUsersService
        {
            [WebGet(UriTemplate = "")]
            List<User> GetCollection();

            [WebInvoke(UriTemplate = "", Method = "POST")]
            User Create(User instance);

            [WebGet(UriTemplate = "{id}")]
            User Get(string id);
        }

        [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
        [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
        public class UsersService : IUsersService
        {
            public List<User> GetCollection()
            {
                Console.WriteLine("In GetCollection");
                return new List<User>() { get_user() };
            }

            public User Create(User instance)
            {
                Console.WriteLine("In Create, instance = {0}", instance);
                return get_user();
            }

            public User Get(string id)
            {
                Console.WriteLine("In Get, id = {0}", id);
                return get_user();
            }

            User get_user()
            {
                return new User { Name = "John Doe", Age = 22 };
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(UsersService), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(IUsersService), new WebHttpBinding(), "").Behaviors.Add(new WebHttpBehavior());
            host.Open();

            Util.SendRequest(baseAddress + "/Users.xml", "GET", null, null);
        }
    }

    public class AutoFormatSelectionTest
    {
        public class MyWebHttpBehavior : WebHttpBehavior { }
        [ServiceContract]
        public interface ITest
        {
            [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
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
            WebHttpBehavior behavior = new MyWebHttpBehavior();
            behavior.AutomaticFormatSelectionEnabled = true;
            host.AddServiceEndpoint(typeof(ITest), new WebHttpBinding(), "").Behaviors.Add(behavior);
            host.Open();

            Util.SendRequest(baseAddress + "/Add", "POST", "application/xml", "<Add xmlns=\"http://tempuri.org/\"><x>123</x><y>456</y></Add>");
            Util.SendRequest(baseAddress + "/Add", "POST", "application/json", "{\"x\":333,\"y\":666}");
        }
    }

    public class Post_efd33d5c_9374_486e_aa0a_5b4dda412cf0
    {
        [ServiceContract(Name = "ITest", Namespace = "http://tempuri.org")]
        public interface ITestServer
        {
            [OperationContract]
            string Echo(string input);
            [OperationContract]
            int Add(int x, int y);

            [OperationContract(AsyncPattern = true)]
            IAsyncResult BeginEcho(string input, AsyncCallback callback, object state);
            string EndEcho(IAsyncResult asyncResult);
            [OperationContract(AsyncPattern = true)]
            IAsyncResult BeginAdd(int x, int y, AsyncCallback callback, object state);
            int EndAdd(IAsyncResult asyncResult);
        }
        public class Service : ITestServer
        {
            public string Echo(string input)
            {
                return input;
            }

            public int Add(int x, int y)
            {
                return x + y;
            }

            public IAsyncResult BeginEcho(string input, AsyncCallback callback, object state)
            {
                throw new NotImplementedException();
            }

            public string EndEcho(IAsyncResult asyncResult)
            {
                throw new NotImplementedException();
            }

            public IAsyncResult BeginAdd(int x, int y, AsyncCallback callback, object state)
            {
                throw new NotImplementedException();
            }

            public int EndAdd(IAsyncResult asyncResult)
            {
                throw new NotImplementedException();
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITestServer), new BasicHttpBinding(), "");
            host.Open();

            ChannelFactory<ITestServer> factory = new ChannelFactory<ITestServer>(new BasicHttpBinding(), new EndpointAddress(baseAddress));
            ITestServer proxy = factory.CreateChannel();

            proxy.BeginAdd(34, 44, delegate(IAsyncResult asyncResult)
            {
                int result = proxy.EndAdd(asyncResult);
                Console.WriteLine("Add result: {0}", result);
            }, null);

            proxy.BeginEcho("Hello world", delegate(IAsyncResult asyncResult)
            {
                string result = proxy.EndEcho(asyncResult);
                Console.WriteLine("Echo result: {0}", result);
            }, null);

            Console.WriteLine("Press ENTER to close");
            Console.ReadLine();
            ((IClientChannel)proxy).Close();
            factory.Close();
            host.Close();
        }
    }

    public class Post_4d5cce94_83c5_4ff0_9bad_2d3ffde24dc0
    {
        [DataContract(Name = "ItemCollectionOf{0}")]
        public class ItemCollection<T>
        {
            [DataMember]
            public List<T> Items;

            public ItemCollection()
            {
                LastUpdate = DateTime.Now;
            }

            [DataMember]
            public DateTime LastUpdate { get; set; }
        }

        public class Site { }
        public class Person { }

        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            ItemCollection<Site> GetSites();
            [OperationContract]
            ItemCollection<Person> GetPeople();
        }
        public class Service : ITest
        {
            public ItemCollection<Site> GetSites()
            {
                throw new NotImplementedException();
            }

            public ItemCollection<Person> GetPeople()
            {
                throw new NotImplementedException();
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

    public class Post_9db6793b_8db9_479b_825c_e781d023f6c1
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare)]
            string Echo(string text);

            [OperationContract]
            [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped)]
            string EchoWrapped(string text);

            [OperationContract]
            [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped)]
            int Divide(int dividend, int divisor, out int reminder);
        }
        public class Service : ITest
        {
            public string Echo(string text)
            {
                return text;
            }

            public string EchoWrapped(string text)
            {
                return text;
            }

            public int Divide(int dividend, int divisor, out int reminder)
            {
                int result = dividend / divisor;
                reminder = dividend % divisor;
                return result;
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
            Console.WriteLine(proxy.Echo("Hello"));
            Console.WriteLine(proxy.EchoWrapped("Hello wrapped"));
            int reminder;
            Console.WriteLine("53 / 3 = {0} (reminder = {1})", proxy.Divide(53, 3, out reminder), reminder);

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_58752d65_06bf_4b93_849d_9ae4c318937e
    {
        [ServiceContract(Namespace = "http://my.namespace.com/")]
        public interface ICalculadora
        {
            [OperationContract(Action = "http:/my.namespace.com/Subtrair", ReplyAction = "http://my.namespace.com/SubtrairResponse")]
            int Subtrair(int numA, int numB);
        }
        public class Service : ICalculadora
        {
            public int Subtrair(int numA, int numB)
            {
                return numA - numB;
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ICalculadora), new WSHttpBinding(SecurityMode.None), "");
            host.Description.Behaviors.Add(new ServiceMetadataBehavior { HttpGetEnabled = true });
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ICalculadora> factory = new ChannelFactory<ICalculadora>(new WSHttpBinding(SecurityMode.None), new EndpointAddress(baseAddress));
            ICalculadora proxy = factory.CreateChannel();
            Console.WriteLine(proxy.Subtrair(543, 234));

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_97ddb118_fdfd_4651_9e61_4d822861325f
    {
        [DataContract(Name = "MyDC", Namespace = "")]
        public class MyDC
        {
            [DataMember]
            public string str;
            [DataMember]
            public int i;
        }
        [ServiceContract(Namespace = "")]
        public interface ITest
        {
            [WebInvoke]
            MyDC EchoDC(MyDC dc);
            [WebGet(ResponseFormat = WebMessageFormat.Json)]
            MyDC CreateDC(string str, int i);
        }
        public class Service : ITest
        {
            public MyDC EchoDC(MyDC dc)
            {
                if (dc.i == -1)
                {
                    throw new WebFaultException<string>("dc.i cannot be -1", HttpStatusCode.InternalServerError);
                }
                else
                {
                    return dc;
                }
            }

            public MyDC CreateDC(string str, int i)
            {
                if (i == -1)
                {
                    throw new WebFaultException<string>("i cannot be -1", HttpStatusCode.InternalServerError);
                }
                else
                {
                    return new MyDC { str = str, i = i };
                }
            }
        }
        public class MyInspector : IEndpointBehavior, IDispatchMessageInspector, IErrorHandler
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
                endpointDispatcher.ChannelDispatcher.ErrorHandlers.Add(this);
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
                NameValueCollection queryParams = WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters;
                foreach (string key in queryParams.Keys)
                {
                    if (key == "format")
                    {
                        string format = queryParams[key];
                        if (format == "json")
                        {
                            WebOperationContext.Current.OutgoingResponse.Format = WebMessageFormat.Json;
                        }
                    }
                }
            }

            public bool HandleError(Exception error)
            {
                throw new NotImplementedException();
            }

            public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
            {
                throw new NotImplementedException();
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            WebHttpBehavior behavior = new WebHttpBehavior();
            behavior.FaultExceptionEnabled = true;
            ServiceEndpoint endpoint = host.AddServiceEndpoint(typeof(ITest), new WebHttpBinding(), "");
            endpoint.Behaviors.Add(behavior);
            endpoint.Behaviors.Add(new MyInspector());
            host.Open();
            Console.WriteLine("Host opened");

            //Util.SendRequest(baseAddress + "/CreateDC?str=hello&i=123", "GET", null, null);
            Util.SendRequest(baseAddress + "/CreateDC?str=hello&i=-1", "GET", null, null);
            //Util.SendRequest(baseAddress + "/CreateDC?str=hello&i=123&format=json", "GET", null, null);
            //Util.SendRequest(baseAddress + "/CreateDC?str=hello&i=-1&format=json", "GET", null, null);

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class MyCode_1_Dynamic
    {
        [DataContract(Namespace = "myns")]
        public class Foo
        {
            [DataMember]
            public int Bar;
            [DataMember]
            public byte[] Baz;
        }

        public class MySession : XmlBinaryWriterSession
        {
            internal Dictionary<int, string> sessionStrings = new Dictionary<int, string>();

            public override bool TryAdd(XmlDictionaryString value, out int key)
            {
                if (base.TryAdd(value, out key))
                {
                    sessionStrings.Add(key, value.Value);
                    return true;
                }

                return false;
            }
        }

        public static void Test()
        {
            var foos = new Foo[10];
            for (int i = 0; i < foos.Length; i++)
                foos[i] = new Foo { Bar = i, Baz = new byte[i] };

            DataContractSerializer ser = new DataContractSerializer(typeof(Foo[]));
            MemoryStream stm = new MemoryStream();

            XmlDictionaryWriter writer;
            MySession writerSession = new MySession();
            writer = XmlDictionaryWriter.CreateBinaryWriter(stm, null, writerSession);

            ser.WriteObject(writer, foos);
            writer.Flush();

            Dump(stm.GetBuffer(), (int)stm.Length);

            // reading
            stm.Position = 0;
            XmlDictionaryReaderQuotas quotas = new XmlDictionaryReaderQuotas();
            XmlBinaryReaderSession readerSession = new XmlBinaryReaderSession();
            InitializeFromWriterSession(readerSession, writerSession.sessionStrings);
            XmlDictionaryReader reader = XmlDictionaryReader.CreateBinaryReader(stm, null, quotas, readerSession);
            object x = ser.ReadObject(reader);
            Console.WriteLine(x);
        }

        private static void InitializeFromWriterSession(XmlBinaryReaderSession readerSession, Dictionary<int, string> dictionary)
        {
            foreach (int key in dictionary.Keys)
            {
                readerSession.Add(key, dictionary[key]);
            }
        }

        static void Dump(byte[] bytes, int length)
        {
            for (int i = 0; i < length; i++)
            {
                byte val = bytes[i];
                Console.Write("{0}", (val < 0x20 || val > 0x7E) ? '.' : (char)val);
            }
        }
    }

    public class MyCode_1_Static
    {
        [DataContract(Name = "Foo", Namespace = "myns")]
        public class Foo
        {
            [DataMember]
            public int Bar;
            [DataMember]
            public byte[] Baz;
        }

        public class MyDict : XmlDictionary
        {
            public override bool TryLookup(XmlDictionaryString value, out XmlDictionaryString result)
            {
                return base.TryLookup(value.Value, out result);
            }
        }

        public static void Test()
        {
            var foos = new Foo[10];
            for (int i = 0; i < foos.Length; i++)
                foos[i] = new Foo { Bar = i, Baz = new byte[i] };

            DataContractSerializer ser = new DataContractSerializer(typeof(Foo[]));
            MemoryStream stm = new MemoryStream();

            XmlDictionaryWriter writer;
            XmlDictionary xmldict = new MyDict();
            xmldict.Add("ArrayOfFoo");
            xmldict.Add("http://www.w3.org/2001/XMLSchema-instance");
            xmldict.Add("Foo");
            xmldict.Add("Bar");
            xmldict.Add("Baz");

            writer = XmlDictionaryWriter.CreateBinaryWriter(stm, xmldict, null);

            ser.WriteObject(writer, foos);
            writer.Flush();

            File.WriteAllBytes(@"c:\temp\a.bin", stm.ToArray());
            Dump(stm.GetBuffer(), (int)stm.Length);

            // reading
            stm.Position = 0;
            XmlDictionaryReaderQuotas quotas = new XmlDictionaryReaderQuotas();
            XmlDictionaryReader reader = XmlDictionaryReader.CreateBinaryReader(stm, xmldict, quotas, null);
            object x = ser.ReadObject(reader);
            Console.WriteLine(x);
        }

        private static void InitializeFromWriterSession(XmlBinaryReaderSession readerSession, Dictionary<int, string> dictionary)
        {
            foreach (int key in dictionary.Keys)
            {
                readerSession.Add(key, dictionary[key]);
            }
        }

        static void Dump(byte[] bytes, int length)
        {
            for (int i = 0; i < length; i++)
            {
                byte val = bytes[i];
                Console.Write("{0}", (val < 0x20 || val > 0x7E) ? '.' : (char)val);
            }
        }
    }

    public class Post_cfcee9d6_f091_4431_a82d_1105093c21df
    {
        [ServiceContract]
        public interface ITest
        {
            [WebInvoke(Method = "POST", UriTemplate = "Products")]
            void Operation();
            [WebInvoke(Method = "POST", UriTemplate = "Products/")]
            void Operation2();
        }
        public class Service : ITest
        {
            public void Operation()
            {
                Console.WriteLine("Inside operation");
            }
            public void Operation2()
            {
                Console.Write("Via Operation2... ");
                this.Operation();
            }
        }
        public class MyServiceAuthorizationManager : ServiceAuthorizationManager
        {
            protected override bool CheckAccessCore(OperationContext operationContext)
            {
                Message requestMessage = operationContext.RequestContext.RequestMessage;
                if (requestMessage.Properties.ContainsKey(HttpRequestMessageProperty.Name))
                {
                    HttpRequestMessageProperty prop = (HttpRequestMessageProperty)requestMessage.Properties[HttpRequestMessageProperty.Name];
                    string myAuth = prop.Headers["X-MyAuth"];
                    if (myAuth != null)
                    {
                        Console.WriteLine("Authorized");
                        return true;
                    }
                }

                Console.WriteLine("Not Authorized");
                return false;
            }
        }
        static void Send(string uri, bool addAuthHeader)
        {
            Console.WriteLine("Request to {0}, {1} auth header", uri, addAuthHeader ? "with" : "without");
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(uri);
            req.Method = "POST";
            req.ContentType = "text/xml";
            req.GetRequestStream().Close();
            if (addAuthHeader)
            {
                req.Headers["X-MyAuth"] = "foo";
            }
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
            Console.WriteLine("  *-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*  ");
            Console.WriteLine();
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), new WebHttpBinding(), "").Behaviors.Add(new WebHttpBehavior());
            host.Authorization.ServiceAuthorizationManager = new MyServiceAuthorizationManager();
            host.Open();
            Console.WriteLine("Host opened");

            Send(baseAddress + "/Products/", false);
            Send(baseAddress + "/Products/", true);
            Send(baseAddress + "/Products", false);
            Send(baseAddress + "/Products", true);

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
                    Console.WriteLine("  {0}", person.fname);
                }
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
            Console.WriteLine("  *-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*  ");
            Console.WriteLine();
        }

        public static void Test()
        {
            StartService();
            MakeRequest();
        }
    }

    public class Post_c46ef367_f7b9_406b_8e52_bf1bf70c4d18_b
    {
        [ServiceContract]
        public interface ITest
        {
            [WebInvoke(Method = "DELETE", ResponseFormat = WebMessageFormat.Json, UriTemplate = "/myproject/persons")]
            void DeletePersons(int[] ids);
            [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, UriTemplate = "/myproject/person")]
            int AddPerson(Person person);
            [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/myproject/person?id={id}")]
            Person GetPerson(int id);
        }

        [DataContract]
        public class Person
        {
            [DataMember(Name = "fname")]
            public string fname { get; set; }
        }

        public class Service : ITest
        {
            static int nextId = 0;
            static Dictionary<int, Person> allPeople = new Dictionary<int, Person>();

            public void DeletePersons(int[] ids)
            {
                foreach (int id in ids)
                {
                    if (allPeople.Remove(id))
                    {
                        Console.WriteLine("Removed person with id {0}", id);
                    }
                }
            }

            public int AddPerson(Person person)
            {
                int id = nextId++;
                allPeople[id] = person;
                return id;
            }

            public Person GetPerson(int id)
            {
                if (allPeople.ContainsKey(id))
                {
                    return allPeople[id];
                }
                return null;
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

        static string MakeRequest(string method, string uri, string body)
        {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(uri);
            req.Method = method;
            if (body != null)
            {
                req.ContentType = "text/json";
                byte[] jsonReq = Encoding.UTF8.GetBytes(body);
                req.GetRequestStream().Write(jsonReq, 0, jsonReq.Length);
                req.GetRequestStream().Close();
            }
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
            string responseBody = null;
            if (respStream != null)
            {
                responseBody = new StreamReader(respStream).ReadToEnd();
                Console.WriteLine(responseBody);
            }
            Console.WriteLine();
            Console.WriteLine("  *-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*  ");
            Console.WriteLine();

            return responseBody;
        }

        static void MakeRequests()
        {
            string id1 = MakeRequest("POST", baseAddress + "/myproject/person", "{\"fname\":\"John\"}");
            string id2 = MakeRequest("POST", baseAddress + "/myproject/person", "{\"fname\":\"Jane\"}");
            string id3 = MakeRequest("POST", baseAddress + "/myproject/person", "{\"fname\":\"sheena\"}");

            Console.WriteLine("People ids: {0}, {1}, {2}", id1, id2, id3);
            MakeRequest("GET", baseAddress + "/myproject/person?id=" + id2, null);
            MakeRequest("GET", baseAddress + "/myproject/person?id=" + id3, null);

            Console.WriteLine("Deleting some people");
            MakeRequest("DELETE", baseAddress + "/myproject/persons", "[" + id1 + "," + id2 + "]");

            MakeRequest("GET", baseAddress + "/myproject/person?id=" + id2, null);
            MakeRequest("GET", baseAddress + "/myproject/person?id=" + id3, null);
        }

        public static void Test()
        {
            StartService();
            MakeRequests();
        }
    }

    public class Post_498eea4f_1625_450b_b94f_914b0ae4d5f1
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract(IsOneWay = true)]
            void OneWay(string text);
            [OperationContract(IsOneWay = false)]
            void ReqReply(string text);
        }
        public class Service : ITest
        {
            public void OneWay(string text)
            {
                Console.WriteLine("In OneWay: {0}", text);
                Thread.Sleep(20000);
                Console.WriteLine("Exiting OneWay");
            }

            public void ReqReply(string text)
            {
                Console.WriteLine("In ReqReply: {0}", text);
                Thread.Sleep(20000);
                Console.WriteLine("Exiting ReqReply");
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            BasicHttpBinding binding = new BasicHttpBinding();
            binding.SendTimeout = TimeSpan.FromSeconds(10);
            binding.ReceiveTimeout = TimeSpan.FromSeconds(10);
            binding.OpenTimeout = TimeSpan.FromSeconds(10);
            binding.CloseTimeout = TimeSpan.FromSeconds(10);
            host.AddServiceEndpoint(typeof(ITest), binding, "");
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(binding, new EndpointAddress(baseAddress));
            ITest proxy = factory.CreateChannel();
            proxy.OneWay("hello");
            Thread.Sleep(25000);
            proxy.ReqReply("world");

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class WPProblem_20100824
    {
        static readonly string BaseAddress = "http://" + Environment.MachineName + ":8000/Service";
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            string Echo(string str);
        }
        public class Service : ITest
        {
            public string Echo(string str)
            {
                Console.WriteLine("In server; message headers:");
                foreach (var header in OperationContext.Current.IncomingMessageHeaders)
                {
                    Console.WriteLine("  {0} ({1})", header.Name, header.Namespace);
                }
                return str;
            }
        }
        public class WPSC : ClientBase<ITest>, ITest, IDisposable
        {
            public WPSC()
                : base(new BasicHttpBinding(), new EndpointAddress(BaseAddress))
            {
            }

            #region ITest Members

            public string Echo(string str)
            {
                return base.Channel.Echo(str);
            }

            #endregion

            protected override ITest CreateChannel()
            {
                return base.CreateChannel();
            }

            void IDisposable.Dispose()
            {
            }
        }
        public class Client
        {
            WPSC client;
            OperationContextScope contextScope;
            public Client()
            {
                this.client = new WPSC();
                contextScope = new OperationContextScope(client.InnerChannel);
                OperationContext.Current.OutgoingMessageHeaders.Add(MessageHeader.CreateHeader("mobile_security_token", "", "my token"));
            }
            public void CallService()
            {
                this.client.Echo("hello");
            }
        }
        public static void Test()
        {
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(BaseAddress));
            host.AddServiceEndpoint(typeof(ITest), new BasicHttpBinding(), "");
            host.Open();

            Client c = new Client();
            ThreadPool.QueueUserWorkItem(delegate { c.CallService(); });
            ThreadPool.QueueUserWorkItem(delegate { c.CallService(); });
            Console.WriteLine("Press enter to continue");
            Console.ReadLine();
        }
    }

    public class Post_cb63dc13_8925_4cc4_be68_8ed8dd554672
    {
        [ServiceContract]
        public interface ITest
        {
            [WebGet]
            string WhoAmI();
        }
        public class Service : ITest
        {
            public string WhoAmI()
            {
                string auth = WebOperationContext.Current.IncomingRequest.Headers[HttpRequestHeader.Authorization];
                if (auth != null)
                {
                    if (auth.StartsWith("Basic "))
                    {
                        string cred = Encoding.UTF8.GetString(Convert.FromBase64String(auth.Substring("Basic ".Length)));
                        string[] parts = cred.Split(':');
                        string userName = parts[0];
                        string password = parts[1];
                        return string.Format("User: {0}, password: {1}", userName, password);
                    }
                }

                return "Nobody";
            }
        }
        static void SendRequest(string uri, string authHeader)
        {
            WebClient c = new WebClient();
            if (authHeader != null)
            {
                c.Headers[HttpRequestHeader.Authorization] = authHeader;
            }
            string result = c.DownloadString(uri);
            Console.WriteLine(result);
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            WebServiceHost host = new WebServiceHost(typeof(Service), new Uri(baseAddress));
            host.Open();
            Console.WriteLine("Host opened");

            SendRequest(baseAddress + "/WhoAmI", null);
            SendRequest(baseAddress + "/WhoAmI", "Basic QWxhZGRpbjpvcGVuIHNlc2FtZQ==");

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_02b518aa_13b7_4630_a1af_8250743f9af6
    {
        [ServiceContract]
        public interface IPPInfo
        {
            // expecting RESULT = 0 and RESPMSG = APPROVED
            [OperationContract]
            [WebGet(UriTemplate = "ReadResponse?RESULT={result}&AUTHCODE={authcode}&RESPMSG={respmsg}&AVSDATA={avsdata}&PNREF={pnref}",
                BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Xml)]
            void ReadResponse(string result, string authcode, string respmsg, string avsdata, string pnref);
        }

        [ServiceBehavior(
            InstanceContextMode = InstanceContextMode.PerCall,
            AddressFilterMode = AddressFilterMode.Any,
            IncludeExceptionDetailInFaults = true)]
        public class PPInfo : IPPInfo
        {
            public void ReadResponse(string result, string authcode, string respmsg, string avsdata, string pnref)
            {
                Console.WriteLine("result: {0}", result ?? "<<null>>");
                Console.WriteLine("authcode: {0}", authcode ?? "<<null>>");
                Console.WriteLine("respmsg: {0}", respmsg ?? "<<null>>");
                Console.WriteLine("avsdata: {0}", avsdata ?? "<<null>>");
                Console.WriteLine("pnref: {0}", pnref ?? "<<null>>");
            }
        }

        static void SendGet(string uri)
        {
            WebClient c = new WebClient();
            c.DownloadString(uri);
            Console.WriteLine();
        }

        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(PPInfo), new Uri(baseAddress));
            ServiceEndpoint endpoint = host.AddServiceEndpoint(typeof(IPPInfo), new WebHttpBinding(), "");
            endpoint.Behaviors.Add(new WebHttpBehavior());
            host.Open();
            Console.WriteLine("Host opened");

            SendGet(baseAddress + "/ReadResponse?RESULT=0&AUTHCODE=10001&RESPMSG=APPROVED&PNREF=12345");

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_02b518aa_13b7_4630_a1af_8250743f9af6_b
    {
        [ServiceContract]
        public interface IPPInfo
        {
            // expecting RESULT = 0 and RESPMSG = APPROVED
            [OperationContract]
            [WebGet(UriTemplate = "ReadResponse")]
            void ReadResponse();
        }

        [ServiceBehavior(
            InstanceContextMode = InstanceContextMode.PerCall,
            AddressFilterMode = AddressFilterMode.Any,
            IncludeExceptionDetailInFaults = true)]
        public class PPInfo : IPPInfo
        {
            public void ReadResponse()
            {
                var qString = WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters;
                foreach (string s in qString)
                {
                    Console.WriteLine("{0}: {1}", s, qString[s]);
                }
            }
        }

        static void SendGet(string uri)
        {
            WebClient c = new WebClient();
            c.DownloadString(uri);
            Console.WriteLine();
        }

        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(PPInfo), new Uri(baseAddress));
            ServiceEndpoint endpoint = host.AddServiceEndpoint(typeof(IPPInfo), new WebHttpBinding(), "");
            endpoint.Behaviors.Add(new WebHttpBehavior());
            host.Open();
            Console.WriteLine("Host opened");

            SendGet(baseAddress + "/ReadResponse?RESULT=0&AUTHCODE=10001&RESPMSG=APPROVED&PNREF=12345");

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class FaultExceptionTests
    {
        public class MyNonSerializableType
        {
            public int i;
            public MyNonSerializableType(int i) { this.i = i; }
        }
        [ServiceContract]
        public class Service
        {
            [WebInvoke(ResponseFormat = WebMessageFormat.Json)]
            public string Echo(string text)
            {
                if (text == "MyNonSerializableType")
                {
                    throw new WebFaultException<MyNonSerializableType>(new MyNonSerializableType(1), HttpStatusCode.Conflict);
                }
                else if (text == null)
                {
                    throw new WebFaultException(HttpStatusCode.BadGateway);
                }
                else
                {
                    throw new WebFaultException<string>(text, HttpStatusCode.ExpectationFailed);
                }
            }
        }
        public class MyWebHttpBehavior : WebHttpBehavior
        {
            protected override void AddServerErrorHandlers(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
            {
                int errorHandlerCount = endpointDispatcher.DispatchRuntime.ChannelDispatcher.ErrorHandlers.Count;
                base.AddServerErrorHandlers(endpoint, endpointDispatcher);
                IErrorHandler baseHandler = endpointDispatcher.DispatchRuntime.ChannelDispatcher.ErrorHandlers[errorHandlerCount];
                endpointDispatcher.DispatchRuntime.ChannelDispatcher.ErrorHandlers.RemoveAt(errorHandlerCount);
                MyErrorHandler handler = new MyErrorHandler(baseHandler, this, endpoint.Contract, endpointDispatcher.DispatchRuntime.ChannelDispatcher.IncludeExceptionDetailInFaults);
                endpointDispatcher.DispatchRuntime.ChannelDispatcher.ErrorHandlers.Add(handler);

            }

            class MyErrorHandler : IErrorHandler
            {
                IErrorHandler baseHandler;
                MyWebHttpBehavior behavior;
                bool includeExceptionDetailInFaults;
                ContractDescription contractDescription;

                public MyErrorHandler(IErrorHandler baseHandler, MyWebHttpBehavior behavior, ContractDescription contractDescription, bool includeExceptionDetailInFaults)
                {
                    this.baseHandler = baseHandler;
                    this.behavior = behavior;
                    this.contractDescription = contractDescription;
                    this.includeExceptionDetailInFaults = includeExceptionDetailInFaults;
                }

                public bool HandleError(Exception error)
                {
                    return true;
                }

                public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
                {
                    if (error is WebFaultException<MyNonSerializableType>)
                    {
                        WebFaultException<MyNonSerializableType> exception = (WebFaultException<MyNonSerializableType>)error;
                        string operation = (string)OperationContext.Current.IncomingMessageProperties[WebHttpDispatchOperationSelector.HttpOperationNamePropertyName];
                        WebOperationContext.Current.OutgoingResponse.StatusCode = exception.StatusCode;
                        string resp = string.Format("MyNonSerializableType[i={0}]", exception.Detail.i);
                        MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(resp));
                        fault = WebOperationContext.Current.CreateStreamResponse(ms, "text/plain");
                    }
                    else
                    {
                        this.baseHandler.ProvideFault(error, version, ref fault);
                    }
                }
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(Service), new WebHttpBinding(), "").Behaviors.Add(new MyWebHttpBehavior { FaultExceptionEnabled = false });
            host.Open();
            Console.WriteLine("Host opened");

            Util.SendRequest(baseAddress + "/Echo", "POST", "text/json", "\"MyNonSerializableType\"");
            Util.SendRequest(baseAddress + "/Echo", "POST", "text/json", "");
            Util.SendRequest(baseAddress + "/Echo", "POST", "text/json", "\"hello\"");

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_e18c4171_61d3_49da_9b24_84871dd6ae80
    {
        [DataContract]
        public class CurrentJob
        {
            [DataMember]
            public string JobName { get; set; }
        }
        public class AsyncExecInstance { }
        public class AsyncRequest { }

        [ServiceContract(Name = "IAsyncInterface")]
        public interface IAsyncInterface
        {
            ///Async Call
            [OperationContract(IsOneWay = true, AsyncPattern = true)]
            IAsyncResult BeginSubmit(CurrentJob currentJob, AsyncCallback callback, object state);
            void EndSubmit(IAsyncResult ar);

            [OperationContract(IsOneWay = true, AsyncPattern = true)]
            IAsyncResult BeginExecute(AsyncExecInstance exec, AsyncRequest request, AsyncCallback callback, object state);
            void EndExecute(IAsyncResult ar);
        }

        [ServiceContract(Name = "IAsyncInterface")]
        public interface ISyncInterface
        {
            [OperationContract(IsOneWay = true)]
            void Submit(CurrentJob currentJob);

            [OperationContract(IsOneWay = true)]
            void Execute(AsyncExecInstance exec, AsyncRequest request);
        }

        [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
        public class Service : IAsyncInterface
        {
            static int beginSubmitCount = 0;
            static int endSubmitCount = 0;
            static int beginExecuteCount = 0;
            static int endExecuteCount = 0;

            delegate void SubmitDelegate(CurrentJob currentJob);
            void SubmitDoWork(CurrentJob currentJob)
            {
                Thread.Sleep(5000);
            }
            delegate void ExecuteDelegate(AsyncExecInstance exec, AsyncRequest request);
            void ExecuteDoWork(AsyncExecInstance exec, AsyncRequest request)
            {
                Thread.Sleep(5000);
            }

            public IAsyncResult BeginSubmit(CurrentJob currentJob, AsyncCallback callback, object state)
            {
                Console.WriteLine("In BeginSubmit - {0}", Interlocked.Increment(ref beginSubmitCount));
                Thread.Sleep(100);
                SubmitDelegate theDelegate = new SubmitDelegate(SubmitDoWork);
                return theDelegate.BeginInvoke(currentJob, callback, state);
            }

            public void EndSubmit(IAsyncResult ar)
            {
                Console.WriteLine("In EndSubmit - {0}", Interlocked.Increment(ref endSubmitCount));
                SubmitDelegate theDelegate = (SubmitDelegate)((System.Runtime.Remoting.Messaging.AsyncResult)ar).AsyncDelegate;
                theDelegate.EndInvoke(ar);
            }

            public IAsyncResult BeginExecute(AsyncExecInstance exec, AsyncRequest request, AsyncCallback callback, object state)
            {
                Console.WriteLine("In BeginExecute - {0}", Interlocked.Increment(ref beginExecuteCount));
                Thread.Sleep(100);
                ExecuteDelegate theDelegate = new ExecuteDelegate(ExecuteDoWork);
                return theDelegate.BeginInvoke(exec, request, callback, state);
            }

            public void EndExecute(IAsyncResult ar)
            {
                Console.WriteLine("In EndExecute - {0}", Interlocked.Increment(ref endExecuteCount));
                ExecuteDelegate theDelegate = (ExecuteDelegate)((System.Runtime.Remoting.Messaging.AsyncResult)ar).AsyncDelegate;
                theDelegate.EndInvoke(ar);
            }
        }

        static void ThreadProc(object param)
        {
            Tuple<string, int, ISyncInterface> typedParam = (Tuple<string, int, ISyncInterface>)param;
            string methodToCall = typedParam.Item1;
            int numberOfCalls = typedParam.Item2;
            ISyncInterface proxy = typedParam.Item3;

            for (int i = 0; i < numberOfCalls; i++)
            {
                if (methodToCall == "Submit")
                {
                    proxy.Submit(new CurrentJob { JobName = "Job " + i });
                }
                else
                {
                    proxy.Execute(new AsyncExecInstance(), new AsyncRequest());
                }
            }
        }

        public static void Test()
        {
            string baseAddress = "net.tcp://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(IAsyncInterface), new NetTcpBinding(), "");
            ServiceThrottlingBehavior stb = host.Description.Behaviors.Find<ServiceThrottlingBehavior>();
            if (stb == null)
            {
                stb = new ServiceThrottlingBehavior();
                host.Description.Behaviors.Add(stb);
            }
            stb.MaxConcurrentCalls = 50;
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ISyncInterface> factory = new ChannelFactory<ISyncInterface>(new NetTcpBinding(), new EndpointAddress(baseAddress));
            ISyncInterface proxy = factory.CreateChannel();

            Thread submitThread = new Thread(new ParameterizedThreadStart(ThreadProc));
            Thread executeThread = new Thread(new ParameterizedThreadStart(ThreadProc));

            submitThread.Start(new Tuple<string, int, ISyncInterface>("Submit", 100, proxy));
            executeThread.Start(new Tuple<string, int, ISyncInterface>("Execute", 100, proxy));

            Console.WriteLine("Press ENTER to close");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_6f14b940_28e2_464a_800d_99f190a22867
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
        public class MyInspector : IEndpointBehavior, IDispatchMessageInspector
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
                object prop;
                string myHeader;
                Console.WriteLine("In {0}", MethodBase.GetCurrentMethod().Name);
                if (!OperationContext.Current.IncomingMessageProperties.TryGetValue(HttpRequestMessageProperty.Name, out prop))
                {
                    myHeader = "Cannot read header from request";
                }
                else
                {
                    HttpRequestMessageProperty reqProp = (HttpRequestMessageProperty)prop;
                    myHeader = reqProp.Headers["X-MyHeader"];
                    Console.WriteLine("X-MyHeader: {0}", myHeader);
                }

                if (!OperationContext.Current.OutgoingMessageProperties.TryGetValue(HttpResponseMessageProperty.Name, out prop))
                {
                    prop = new HttpResponseMessageProperty();
                    OperationContext.Current.OutgoingMessageProperties.Add(HttpResponseMessageProperty.Name, prop);
                }
                HttpResponseMessageProperty respProp = (HttpResponseMessageProperty)prop;
                respProp.Headers["X-MyResponseHeader"] = "Added from AfterReceiveRequest - " + myHeader;
                Console.WriteLine("Leaving {0}", MethodBase.GetCurrentMethod().Name);
                Console.WriteLine();
                return null;
            }

            public void BeforeSendReply(ref Message reply, object correlationState)
            {
                object prop;
                string requestHeader = null;
                Console.WriteLine("In {0}", MethodBase.GetCurrentMethod().Name);
                if (OperationContext.Current.IncomingMessageProperties.TryGetValue(HttpRequestMessageProperty.Name, out prop))
                {
                    HttpRequestMessageProperty reqProp = (HttpRequestMessageProperty)prop;
                    requestHeader = reqProp.Headers["X-MyHeader"];
                    Console.WriteLine("Got the request header: {0}", requestHeader);
                }

                if (!OperationContext.Current.OutgoingMessageProperties.TryGetValue(HttpResponseMessageProperty.Name, out prop))
                {
                    prop = new HttpResponseMessageProperty();
                    OperationContext.Current.OutgoingMessageProperties.Add(HttpResponseMessageProperty.Name, prop);
                }
                HttpResponseMessageProperty respProp = (HttpResponseMessageProperty)prop;
                respProp.Headers["X-MyResponseHeader2"] = "Added from BeforeSendReply - " + requestHeader;
                Console.WriteLine("Leaving {0}", MethodBase.GetCurrentMethod().Name);
                Console.WriteLine();
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), new BasicHttpBinding(), "").Behaviors.Add(new MyInspector());
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(new BasicHttpBinding(), new EndpointAddress(baseAddress));
            ITest proxy = factory.CreateChannel();

            using (new OperationContextScope((IContextChannel)proxy))
            {
                HttpRequestMessageProperty reqProp = new HttpRequestMessageProperty();
                reqProp.Headers["X-MyHeader"] = "The value of my header";
                OperationContext.Current.OutgoingMessageProperties.Add(HttpRequestMessageProperty.Name, reqProp);
                Console.WriteLine(proxy.Echo("Hello"));
                HttpResponseMessageProperty respProp = (HttpResponseMessageProperty)OperationContext.Current.IncomingMessageProperties[HttpResponseMessageProperty.Name];
                Console.WriteLine("HTTP headers in response:");
                foreach (string headerName in respProp.Headers.AllKeys)
                {
                    Console.WriteLine("  {0}: {1}", headerName, respProp.Headers[headerName]);
                }
            }

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_2283f5f7_c36a_4e34_bce8_a2d7ddfa5067
    {
        [XmlType(TypeName = "EmailAddress", Namespace = "http://www.company.com/xml/xsd/ValidationTypes")]
        public class EmailAddress
        {
            [XmlText]
            public string value;
        }
        [XmlType(TypeName = "ZipCode", Namespace = "http://www.company.com/xml/xsd/ValidationTypes")]
        public class ZipCode
        {
            [XmlText]
            public string value;
        }
        public class EmailAddressList
        {
            [XmlElement(ElementName = "EmailAddress", Namespace = "http://www.company.com/xml/xsd/ValidationTypes")]
            public EmailAddress[] addresses;
        }
        public class ZipCodeList
        {
            [XmlElement(ElementName = "ZipCode", Namespace = "http://www.company.com/xml/xsd/ValidationTypes")]
            public ZipCode[] zipCodes;
        }

        [MessageContract(WrapperName = "ValidateRequest", WrapperNamespace = "http://www.company.com/xml/xsd/ValidationServiceMessages")]
        public class ValidateRequest
        {
            [MessageBodyMember(Name = "EmailList")]
            public EmailAddressList emails;
            [MessageBodyMember(Name = "ZipCodeList")]
            public ZipCodeList zipCodes;
        }

        [ServiceContract(Namespace = "http://www.company.com/xml/xsd/ValidationServiceMessages")]
        [XmlSerializerFormat]
        public interface ITest
        {
            [OperationContract]
            void ValidateRequest(ValidateRequest request);
        }
        public class Service : ITest
        {
            public void ValidateRequest(ValidateRequest request) { }
        }

        #region From blog post
        public class AddBodyNamespacesMessageEncodingBindingElement : MessageEncodingBindingElement
        {
            MessageEncodingBindingElement inner;
            Dictionary<string, string> prefixToNamespaceMapping = new Dictionary<string, string>();
            public AddBodyNamespacesMessageEncodingBindingElement(MessageEncodingBindingElement inner)
            {
                this.inner = inner;
            }

            private AddBodyNamespacesMessageEncodingBindingElement(AddBodyNamespacesMessageEncodingBindingElement other)
            {
                this.inner = other.inner;
                this.prefixToNamespaceMapping = new Dictionary<string, string>(other.prefixToNamespaceMapping);
            }

            public void AddNamespaceMapping(string prefix, string namespaceUri)
            {
                this.prefixToNamespaceMapping.Add(prefix, namespaceUri);
            }

            public override MessageEncoderFactory CreateMessageEncoderFactory()
            {
                return new AddBodyNamespacesMessageEncoderFactory(this.inner.CreateMessageEncoderFactory(), this.prefixToNamespaceMapping);
            }

            public override MessageVersion MessageVersion
            {
                get { return this.inner.MessageVersion; }
                set { this.inner.MessageVersion = value; }
            }

            public override BindingElement Clone()
            {
                return new AddBodyNamespacesMessageEncodingBindingElement(this);
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

            public override bool CanBuildChannelFactory<TChannel>(BindingContext context)
            {
                return context.CanBuildInnerChannelFactory<TChannel>();
            }

            public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
            {
                context.BindingParameters.Add(this);
                return context.BuildInnerChannelFactory<TChannel>();
            }

            public static CustomBinding ReplaceEncodingBindingElement(Binding originalBinding, Dictionary<string, string> prefixToNamespaceMapping)
            {
                CustomBinding custom = originalBinding as CustomBinding;
                if (custom == null)
                {
                    custom = new CustomBinding(originalBinding);
                }

                for (int i = 0; i < custom.Elements.Count; i++)
                {
                    if (custom.Elements[i] is MessageEncodingBindingElement)
                    {
                        AddBodyNamespacesMessageEncodingBindingElement element = new AddBodyNamespacesMessageEncodingBindingElement((MessageEncodingBindingElement)custom.Elements[i]);
                        foreach (var mapping in prefixToNamespaceMapping)
                        {
                            element.AddNamespaceMapping(mapping.Key, mapping.Value);
                        }

                        custom.Elements[i] = element;
                    }
                }

                return custom;
            }

            class AddBodyNamespacesMessageEncoderFactory : MessageEncoderFactory
            {
                private MessageEncoderFactory messageEncoderFactory;
                private Dictionary<string, string> prefixToNamespaceMapping;

                public AddBodyNamespacesMessageEncoderFactory(MessageEncoderFactory messageEncoderFactory, Dictionary<string, string> prefixToNamespaceMapping)
                {
                    this.messageEncoderFactory = messageEncoderFactory;
                    this.prefixToNamespaceMapping = prefixToNamespaceMapping;
                }

                public override MessageEncoder Encoder
                {
                    get { return new AddBodyNamespacesMessageEncoder(this.messageEncoderFactory.Encoder, this.prefixToNamespaceMapping); }
                }

                public override MessageVersion MessageVersion
                {
                    get { return this.messageEncoderFactory.MessageVersion; }
                }

                public override MessageEncoder CreateSessionEncoder()
                {
                    return new AddBodyNamespacesMessageEncoder(this.messageEncoderFactory.CreateSessionEncoder(), this.prefixToNamespaceMapping);
                }
            }

            class AddBodyNamespacesMessageEncoder : MessageEncoder
            {
                private MessageEncoder messageEncoder;
                private Dictionary<string, string> prefixToNamespaceMapping;

                public AddBodyNamespacesMessageEncoder(MessageEncoder messageEncoder, Dictionary<string, string> prefixToNamespaceMapping)
                {
                    this.messageEncoder = messageEncoder;
                    this.prefixToNamespaceMapping = prefixToNamespaceMapping;
                }

                public override string ContentType
                {
                    get { return this.messageEncoder.ContentType; }
                }

                public override string MediaType
                {
                    get { return this.messageEncoder.MediaType; }
                }

                public override MessageVersion MessageVersion
                {
                    get { return this.messageEncoder.MessageVersion; }
                }

                public override bool IsContentTypeSupported(string contentType)
                {
                    return this.messageEncoder.IsContentTypeSupported(contentType);
                }

                public override Message ReadMessage(ArraySegment<byte> buffer, BufferManager bufferManager, string contentType)
                {
                    return this.messageEncoder.ReadMessage(buffer, bufferManager, contentType);
                }

                public override Message ReadMessage(Stream stream, int maxSizeOfHeaders, string contentType)
                {
                    throw new NotSupportedException("Streamed not supported");
                }

                public override ArraySegment<byte> WriteMessage(Message message, int maxMessageSize, BufferManager bufferManager, int messageOffset)
                {
                    MemoryStream ms = new MemoryStream();
                    XmlDictionaryWriter w = XmlDictionaryWriter.CreateBinaryWriter(ms);
                    XmlDictionaryReader bodyReader = message.GetReaderAtBodyContents();

                    AddBodyNamespacesBodyWriter bodyWriter = new AddBodyNamespacesBodyWriter(this.prefixToNamespaceMapping, bodyReader);

                    Message newMessage = Message.CreateMessage(message.Version, null, bodyWriter);
                    newMessage.Properties.CopyProperties(message.Properties);
                    newMessage.Headers.CopyHeadersFrom(message);
                    return this.messageEncoder.WriteMessage(newMessage, maxMessageSize, bufferManager, messageOffset);
                }

                public override void WriteMessage(Message message, Stream stream)
                {
                    throw new NotSupportedException("Streamed not supported");
                }
            }

            class AddBodyNamespacesBodyWriter : BodyWriter
            {
                Dictionary<string, string> prefixToNamespaceMapping;
                XmlDictionaryReader bodyReader;
                public AddBodyNamespacesBodyWriter(Dictionary<string, string> prefixToNamespaceMapping, XmlDictionaryReader bodyReader)
                    : base(true)
                {
                    this.prefixToNamespaceMapping = prefixToNamespaceMapping;
                    this.bodyReader = bodyReader;
                }

                protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
                {
                    foreach (var prefix in this.prefixToNamespaceMapping.Keys)
                    {
                        writer.WriteAttributeString("xmlns", prefix, null, this.prefixToNamespaceMapping[prefix]);
                    }

                    writer.WriteNode(this.bodyReader, false);
                }
            }
        }
        #endregion

        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            Binding serverBinding = new BasicHttpBinding();
            host.AddServiceEndpoint(typeof(ITest), serverBinding, "");
            host.Open();
            Console.WriteLine("Host opened");

            Dictionary<string, string> prefixes = new Dictionary<string, string>
            {
                { "valTypes", "http://www.company.com/xml/xsd/ValidationTypes" }
            };
            Binding clientBinding = AddBodyNamespacesMessageEncodingBindingElement.ReplaceEncodingBindingElement(new BasicHttpBinding(), prefixes);

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(clientBinding, new EndpointAddress(baseAddress));
            ITest proxy = factory.CreateChannel();
            List<EmailAddress> emails = new List<EmailAddress>
            {
                new EmailAddress { value = "michael.logan@vita.virginia.gov" },
                new EmailAddress { value = "mike.logan@vita.com" },
                new EmailAddress { value = "mike" },
            };
            List<ZipCode> zipCodes = new List<ZipCode>
            {
                new ZipCode { value = "90210" },
                new ZipCode { value = "23838" },
                new ZipCode { value = "abcde" },
            };
            ValidateRequest request = new ValidateRequest
            {
                emails = new EmailAddressList { addresses = emails.ToArray() },
                zipCodes = new ZipCodeList { zipCodes = zipCodes.ToArray() },
            };

            proxy.ValidateRequest(request);

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_44c2f094_0cee_4990_95a4_0995a49bbea4
    {
        [DataContract]
        public class MyDC
        {
            [DataMember]
            public string Data { get; set; }
        }
        [DataContract]
        public class MyErrorDC
        {
            [DataMember]
            public string Message { get; set; }
        }
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            [FaultContract(typeof(MyErrorDC))]
            MyDC Process(string[] input);
        }
        public class Service : ITest
        {
            public MyDC Process(string[] input)
            {
                if (input == null || input.Length == 0)
                {
                    throw new FaultException<MyErrorDC>(new MyErrorDC { Message = "Collection cannot be empty" });
                }

                return new MyDC { Data = "Collection has " + input.Length + " elements" };
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

            // Success
            Console.WriteLine(proxy.Process(new string[] { "hello", "world" }).Data);

            // Error
            try
            {
                Console.WriteLine(proxy.Process(new string[0]));
            }
            catch (FaultException<MyErrorDC> e)
            {
                Console.WriteLine("Error from server: {0}", e.Detail.Message);
            }

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_42071894_8fd9_413e_adae_524f859b3953
    {
        [ServiceContract]
        public interface ITestWithXmlElement
        {
            [OperationContract]
            bool Validate(XmlElement input);
        }
        [ServiceContract]
        public interface ITestWithStream
        {
            [OperationContract]
            bool Validate(Stream input);
        }
        public class Service : ITestWithStream, ITestWithXmlElement
        {
            bool ITestWithStream.Validate(Stream input)
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(input);
                Console.WriteLine("Inside ITestWithStream.Validate, doc = {0}", doc.OuterXml);
                return Validate(doc);
            }

            bool ITestWithXmlElement.Validate(XmlElement input)
            {
                Console.WriteLine("Inside ITestWithXmlElement.Validate, input = {0}", input.OuterXml);
                return true;
            }

            private bool Validate(XmlDocument doc)
            {
                return true;
            }
        }
        static void ValidateUsingXmlElement(XmlDocument doc, string endpointAddress)
        {
            ChannelFactory<ITestWithXmlElement> factory = new ChannelFactory<ITestWithXmlElement>(new BasicHttpBinding(), new EndpointAddress(endpointAddress));
            ITestWithXmlElement proxy = factory.CreateChannel();

            Console.WriteLine(proxy.Validate(doc.DocumentElement));

            ((IClientChannel)proxy).Close();
            factory.Close();
        }
        static void ValidateUsingStream(XmlDocument doc, string endpointAddress)
        {
            ChannelFactory<ITestWithStream> factory = new ChannelFactory<ITestWithStream>(new WebHttpBinding(), new EndpointAddress(endpointAddress));
            factory.Endpoint.Behaviors.Add(new WebHttpBehavior());
            ITestWithStream proxy = factory.CreateChannel();

            MemoryStream ms = new MemoryStream();
            XmlWriter w = XmlWriter.Create(ms);
            doc.WriteTo(w);
            w.Flush();
            ms.Position = 0;
            Console.WriteLine(proxy.Validate(ms));

            ((IClientChannel)proxy).Close();
            factory.Close();
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));

            host.AddServiceEndpoint(typeof(ITestWithXmlElement), new BasicHttpBinding(), "basic");
            host.AddServiceEndpoint(typeof(ITestWithStream), new WebHttpBinding(), "web").Behaviors.Add(new WebHttpBehavior());

            host.Open();
            Console.WriteLine("Host opened");

            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<root><item>1</item><item>2</item></root>");

            ValidateUsingXmlElement(doc, baseAddress + "/basic");
            ValidateUsingStream(doc, baseAddress + "/web");

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class DynamicTestClass : DynamicObject
    {
        Dictionary<string, string> dict = new Dictionary<string, string>();
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (dict.ContainsKey(binder.Name))
            {
                result = this.dict[binder.Name];
                return true;
            }

            return base.TryGetMember(binder, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (value is string)
            {
                this.dict[binder.Name] = (string)value;
                return true;
            }

            return base.TrySetMember(binder, value);
        }
    }

    public class Post_59025b10_b2c7_4dad_b5ca_eebaf085a84a
    {
        [ServiceContract]
        public interface ITest
        {
            [WebGet(ResponseFormat = WebMessageFormat.Json)]
            string Echo(string text);
            [WebGet(ResponseFormat = WebMessageFormat.Xml)]
            int Add(int x, int y);
            [WebGet(UriTemplate = "/help")]
            Stream help();
        }
        public class Service : ITest
        {
            public string Echo(string text) { return text; }
            public int Add(int x, int y) { return x + y; }
            public Stream help()
            {
                WebOperationContext.Current.OutgoingResponse.ContentType = "text/plain";
                return new MemoryStream(Encoding.UTF8.GetBytes("The response"));
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            WebHttpBinding binding = new WebHttpBinding();
            WebHttpBehavior behavior = new WebHttpBehavior { HelpEnabled = true };
            host.AddServiceEndpoint(typeof(ITest), binding, "").Behaviors.Add(behavior);
            host.Open();
            Console.WriteLine("Host opened");

            Util.SendRequest(baseAddress + "/help", "GET", null, null);

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_394dfbb3_99cc_45e4_a6d1_bfa75ca4913b
    {
        public static void Test()
        {
            string configFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            System.Configuration.ExeConfigurationFileMap fileMap = new System.Configuration.ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = configFile;
            System.Configuration.Configuration config = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(fileMap, System.Configuration.ConfigurationUserLevel.None);
            System.ServiceModel.Configuration.ServiceModelSectionGroup smsg = config.SectionGroups["system.serviceModel"] as System.ServiceModel.Configuration.ServiceModelSectionGroup;
            System.ServiceModel.Configuration.BehaviorsSection bs = (System.ServiceModel.Configuration.BehaviorsSection)smsg.Sections["behaviors"];
            ServiceBehaviorElement sbe = bs.ServiceBehaviors["MyServiceBehavior"];
            foreach (BehaviorExtensionElement bee in sbe)
            {
                Type behaviorElementType = bee.GetType();
                MethodInfo createBehaviorMethod = behaviorElementType.GetMethod("CreateBehavior", BindingFlags.Instance | BindingFlags.NonPublic);
                if (createBehaviorMethod != null)
                {
                    IServiceBehavior behavior = createBehaviorMethod.Invoke(bee, null) as IServiceBehavior;
                    Console.WriteLine("{0}: {1}", bee, behavior == null ? "<<null>>" : behavior.ToString());
                }
            }
        }
    }

    public class Post_1dc19d08_71b0_4f4d_9d8a_698d7af877d8
    {
        const string HttpMethodOverrideHeaderName = "X-HTTP-Method-Override";
        const string OriginalHttpMethodPropertyName = "OriginalHttpMethod";

        [ServiceContract]
        public interface ITest
        {
            [WebInvoke(Method = "PUT", UriTemplate = "/Operation", ResponseFormat = WebMessageFormat.Json)]
            string Put(string text);
            [WebInvoke(Method = "POST", UriTemplate = "/Operation", ResponseFormat = WebMessageFormat.Json)]
            string Post(string text);
            [WebInvoke(Method = "DELETE", UriTemplate = "/Operation", ResponseFormat = WebMessageFormat.Json)]
            string Delete(string text);
        }
        public class Service : ITest
        {
            public string Put(string text)
            {
                return Operation("PUT", text);
            }
            public string Post(string text)
            {
                return Operation("POST", text);
            }
            public string Delete(string text)
            {
                return Operation("DELETE", text);
            }
            private string Operation(string method, string text)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Method: " + method);
                sb.AppendLine("Method (from WOC): " + WebOperationContext.Current.IncomingRequest.Method);

                if (OperationContext.Current.IncomingMessageProperties.ContainsKey(OriginalHttpMethodPropertyName))
                {
                    sb.AppendLine("Original HTTP request method: " + (string)OperationContext.Current.IncomingMessageProperties[OriginalHttpMethodPropertyName]);
                }

                sb.AppendLine("Headers:");
                foreach (string headerName in WebOperationContext.Current.IncomingRequest.Headers.AllKeys)
                {
                    string headerValue = WebOperationContext.Current.IncomingRequest.Headers[headerName];
                    sb.AppendLine(string.Format("  {0}: {1}", headerName, headerValue));
                }
                sb.AppendLine("Text: " + text);
                return sb.ToString();
            }
        }
        static void SendRequest(string uri, string method, string body, string httpOverrideHeader)
        {
            string contentType = "application/json";
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(uri);
            req.Method = method;
            req.ContentType = contentType;
            if (httpOverrideHeader != null)
            {
                req.Headers[HttpMethodOverrideHeaderName] = httpOverrideHeader;
            }

            byte[] bodyBytes = Encoding.UTF8.GetBytes(body);
            req.GetRequestStream().Write(bodyBytes, 0, bodyBytes.Length);
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

            if (resp == null)
            {
                Console.WriteLine("Response is null");
            }
            else
            {
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
                    responseBody = responseBody.Replace("\\u000d", "\r").Replace("\\u000a", "\n").Replace("\\/", "/");
                    Console.WriteLine(responseBody);
                }
                else
                {
                    Console.WriteLine("HttpWebResponse.GetResponseStream returned null");
                }
            }

            Console.WriteLine();
            Console.WriteLine("  *-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*  ");
            Console.WriteLine();
        }
        public class HttpOverrideInspector : IEndpointBehavior
        {
            public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
            {
            }

            public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
            {
            }

            public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
            {
                endpointDispatcher.DispatchRuntime.OperationSelector = new HttpOverrideOperationSelector(endpointDispatcher.DispatchRuntime.OperationSelector);
            }

            public void Validate(ServiceEndpoint endpoint)
            {
            }
        }
        public class HttpOverrideOperationSelector : IDispatchOperationSelector
        {
            IDispatchOperationSelector originalSelector;
            public HttpOverrideOperationSelector(IDispatchOperationSelector originalSelector)
            {
                this.originalSelector = originalSelector;
            }

            public string SelectOperation(ref Message message)
            {
                if (message.Properties.ContainsKey(HttpRequestMessageProperty.Name))
                {
                    HttpRequestMessageProperty reqProp = (HttpRequestMessageProperty)message.Properties[HttpRequestMessageProperty.Name];
                    string httpMethodOverride = reqProp.Headers["X-HTTP-Method-Override"];
                    if (!string.IsNullOrEmpty(httpMethodOverride))
                    {
                        message.Properties[OriginalHttpMethodPropertyName] = reqProp.Method;
                        reqProp.Method = httpMethodOverride;
                    }
                }

                return this.originalSelector.SelectOperation(ref message);
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            ServiceEndpoint endpoint = host.AddServiceEndpoint(typeof(ITest), new WebHttpBinding(), "");
            endpoint.Behaviors.Add(new WebHttpBehavior());
            endpoint.Behaviors.Add(new HttpOverrideInspector());
            host.Open();
            Console.WriteLine("Host opened");

            SendRequest(baseAddress + "/Operation", "POST", "\"Method POST, override PUT\"", "PUT");
            SendRequest(baseAddress + "/Operation", "POST", "\"Method POST, no override\"", null);
            SendRequest(baseAddress + "/Operation", "POST", "\"Method POST, override DELETE\"", "DELETE");

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_9b60bf01_2867_4256_a060_bd9a25f0a86c
    {
        const string UpdateLinkPropertyName = "Unique_Name_For_A_Property_With_UpdateLink";
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
                int updateLink = (int)OperationContext.Current.IncomingMessageProperties[UpdateLinkPropertyName];
                Console.WriteLine("Inside service, update link: {0}", updateLink);
                return text;
            }
        }
        public class MessageLoggerInspector : IEndpointBehavior, IDispatchMessageInspector
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
                string requestText = request.ToString();
                int updateLink = requestText.GetHashCode();
                request.Properties.Add(UpdateLinkPropertyName, updateLink);
                return null;
            }

            public void BeforeSendReply(ref Message reply, object correlationState)
            {
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            ServiceEndpoint endpoint = host.AddServiceEndpoint(typeof(ITest), new BasicHttpBinding(), "");
            endpoint.Behaviors.Add(new MessageLoggerInspector());
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(new BasicHttpBinding(), new EndpointAddress(baseAddress));
            ITest proxy = factory.CreateChannel();
            Console.WriteLine(proxy.Echo("Hello"));

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_8de089f4_890b_402d_ae16_09e1b951e62b
    {
        [ServiceContract]
        interface IBaseBallContract
        {
            [OperationContract]
            [WebGet(UriTemplate = "/baseball/{id}")]
            TeamDocument GetDocument(string id);
        }

        [DataContract()]
        class TeamDocument : IExtensibleDataObject
        {
            [DataMember(Name = "TeamName")]
            public string TeamName { get; set; }

            [DataMember(Name = "State")]
            public string State { get; set; }

            public ExtensionDataObject ExtensionData { get; set; }
        }

        class TextToJsonMapper : WebContentTypeMapper
        {
            public override WebContentFormat GetMessageFormatForContentType(string contentType)
            {
                if (contentType.StartsWith("text/plain"))
                {
                    return WebContentFormat.Json;
                }

                return WebContentFormat.Default;
            }
        }

        public static void Test()
        {
            ChannelFactory<IBaseBallContract> factory = new ChannelFactory<IBaseBallContract>("BaseballRest");
            WebHttpBinding binding = factory.Endpoint.Binding as WebHttpBinding;
            if (binding != null)
            {
                binding.ContentTypeMapper = new TextToJsonMapper();
            }
            var proxy = factory.CreateChannel();
            var response = proxy.GetDocument("4830b9a8752c50b644304071fb0031c2");
            ((IDisposable)proxy).Dispose();
        }
    }

    public class TestJsonEmptyBody
    {
        [DataContract(Name = "Person")]
        public class Person { }
        [ServiceContract]
        public interface ITest
        {
            [WebInvoke(ResponseFormat = WebMessageFormat.Json)]
            Person EchoPerson(Person p);
        }
        public class Service : ITest
        {
            public Person EchoPerson(Person p)
            {
                Console.WriteLine("p is {0}", p == null ? "null" : "not null");
                return p;
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8008/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), new WebHttpBinding(), "").Behaviors.Add(new WebHttpBehavior());
            host.Open();

            Util.SendRequest(baseAddress + "/EchoPerson", "POST", "application/json", "");
            Util.SendRequest(baseAddress + "/EchoPerson", "POST", "application/json", "{}");
        }
    }

    public class Post_aaeea6c5_872c_4600_8b58_3fa0465d8740
    {
        [ServiceContract(Name = "ITest", Namespace = "http://tempuri.org")]
        public interface ITest
        {
            [OperationContract]
            int Add(int x, int y);
            [OperationContract]
            int Subtract(int x, int y);
        }
        [ServiceContract(Name = "ITest", Namespace = "http://tempuri.org")]
        public interface ITestOld
        {
            [OperationContract]
            int Add(int x, int y);
            [OperationContract]
            int Subtrat(int x, int y);
        }
        [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
        public class Service : ITest
        {
            public int Add(int x, int y)
            {
                return x + y;
            }
            public int Subtract(int x, int y)
            {
                return x + y;
            }
        }
        static void ShowWsdlOperations(string wsdlAddress)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(new MemoryStream(new WebClient().DownloadData(wsdlAddress)));
            XmlNamespaceManager nsManager = new XmlNamespaceManager(doc.NameTable);
            nsManager.AddNamespace("wsdl", "http://schemas.xmlsoap.org/wsdl/");
            Console.WriteLine("Operations:");
            foreach (XmlNode node in doc.SelectNodes("wsdl:definitions/wsdl:binding/wsdl:operation", nsManager))
            {
                Console.WriteLine("  {0}", node.Attributes["name"].Value);
            }
        }
        class DuplicateOperationBehavior : IEndpointBehavior
        {
            private string existingOperationName;
            private string operationAlias;

            public DuplicateOperationBehavior(string existingOperationName, string operationAlias)
            {
                this.existingOperationName = existingOperationName;
                this.operationAlias = operationAlias;
            }
            public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
            {
            }

            public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
            {
            }

            public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
            {
                endpointDispatcher.DispatchRuntime.OperationSelector = new DuplicateOperationSelector(endpoint.Contract, this.existingOperationName, this.operationAlias);
            }

            public void Validate(ServiceEndpoint endpoint)
            {
            }

            class DuplicateOperationSelector : IDispatchOperationSelector
            {
                Dictionary<string, string> actionToOperation;
                string existingAction;
                string actionAlias;

                public DuplicateOperationSelector(ContractDescription contract, string existingOperationName, string operationAlias)
                {
                    this.actionToOperation = new Dictionary<string, string>();
                    foreach (OperationDescription od in contract.Operations)
                    {
                        this.actionToOperation.Add(od.Messages[0].Action, od.Name);
                        if (od.Name == existingOperationName)
                        {
                            this.existingAction = od.Messages[0].Action;
                            this.actionAlias = existingAction.Replace(existingOperationName, operationAlias);
                            this.actionToOperation.Add(this.actionAlias, od.Name);
                        }
                    }
                }

                public string SelectOperation(ref Message message)
                {
                    if (message.Headers.Action == this.actionAlias)
                    {
                        message.Headers.Action = this.existingAction;
                    }

                    return this.actionToOperation[message.Headers.Action];
                }
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            ServiceEndpoint endpoint = host.AddServiceEndpoint(typeof(ITest), new BasicHttpBinding(), "");
            endpoint.Behaviors.Add(new DuplicateOperationBehavior("Subtract", "Subtrat"));
            host.Description.Behaviors.Add(new ServiceMetadataBehavior { HttpGetEnabled = true });
            host.Open();
            Console.WriteLine("Host opened");

            ShowWsdlOperations(baseAddress + "?wsdl");

            ChannelFactory<ITestOld> factory = new ChannelFactory<ITestOld>(new BasicHttpBinding(), new EndpointAddress(baseAddress));
            ITestOld proxy = factory.CreateChannel();
            Console.WriteLine(proxy.Add(3, 4));
            Console.WriteLine(proxy.Subtrat(3, 4));

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_aca60ca7_03c7_4dc0_b671_61682bae2ea8
    {
        [DataContract(Name = "Person", Namespace = "")]
        public class Person
        {
            [DataMember]
            public string Name;
            [DataMember]
            public int Age;
        }
        [ServiceContract]
        public interface ITest
        {
            [WebGet(ResponseFormat = WebMessageFormat.Json)]
            Person GetPersonWithMyQSC(Person p);
            [WebGet(ResponseFormat = WebMessageFormat.Json)]
            Person GetPersonWithJsonQSC(Person p);
        }
        public class Service : ITest
        {
            public Person GetPersonWithMyQSC(Person p) { return p; }
            public Person GetPersonWithJsonQSC(Person p) { return p; }
        }
        class MyQueryStringConverter : QueryStringConverter
        {
            public override bool CanConvert(Type type)
            {
                return type == typeof(Person) || base.CanConvert(type);
            }
            public override object ConvertStringToValue(string parameter, Type parameterType)
            {
                if (parameterType == typeof(Person))
                {
                    string separator = "---";
                    int separatorIndex = parameter.IndexOf(separator);
                    string name = parameter.Substring(0, separatorIndex);
                    int age = int.Parse(parameter.Substring(separatorIndex + separator.Length));
                    return new Person { Name = name, Age = age };
                }
                else
                {
                    return base.ConvertStringToValue(parameter, parameterType);
                }
            }
        }
        class MyWebHttpBehavior : WebHttpBehavior
        {
            protected override QueryStringConverter GetQueryStringConverter(OperationDescription operationDescription)
            {
                if (operationDescription.Name == "GetPersonWithMyQSC")
                {
                    return new MyQueryStringConverter();
                }
                else
                {
                    return new JsonQueryStringConverter();
                }
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            ServiceEndpoint endpoint = host.AddServiceEndpoint(typeof(ITest), new WebHttpBinding(), "");
            MyWebHttpBehavior behavior = new MyWebHttpBehavior();
            endpoint.Behaviors.Add(behavior);
            host.Open();

            Console.WriteLine("Host opened");

            Util.SendRequest(baseAddress + "/GetPersonWithMyQSC?p=John%20Doe---23", "GET", null, null);
            Util.SendRequest(baseAddress + "/GetPersonWithJsonQSC?p=%7B%22Name%22%3A%22John%20Doe%22%2C%22Age%22%3A23%7D", "GET", null, null);

            Console.WriteLine("Press ENTER to close");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_c2672206_f255_4b14_b45e_7e3d057f4ffc
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            [WebInvoke(Method = "HEAD")]
            Stream Process();
            [OperationContract]
            [WebInvoke(Method = "HEAD")]
            void Process2();
        }
        public class Service : ITest
        {
            public class MyLengthOnlyStream : Stream
            {
                int length;
                public MyLengthOnlyStream(int length)
                {
                    this.length = length;
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

                public override void Flush() { }

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
                    int result = 0;
                    if (this.length > 0)
                    {
                        result = Math.Min(this.length, count);
                        this.length -= count;
                    }
                    return result;
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
            public Stream Process()
            {
                WebOperationContext.Current.OutgoingResponse.ContentType = "the/actual/content/type";
                return new MyLengthOnlyStream(1234);
            }
            public void Process2()
            {
                WebOperationContext.Current.OutgoingResponse.ContentType = "the/actual/content/type";
                WebOperationContext.Current.OutgoingResponse.ContentLength = 1234567;
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.Accepted;
            }
        }
        static void SendRequest(string uri, string method)
        {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(uri);
            req.Method = method;
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
                byte[] buffer = new byte[1000];
                int bytesRead, totalBytesRead = 0;
                do
                {
                    bytesRead = respStream.Read(buffer, 0, buffer.Length);
                    totalBytesRead += bytesRead;
                } while (bytesRead > 0);
                Console.WriteLine("Response body has {0} bytes", totalBytesRead);
            }
            else
            {
                Console.WriteLine("HttpWebResponse.GetResponseStream returned null");
            }
        }

        public static void Test()
        {
            string baseAddress = "http://localhost:8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), new WebHttpBinding(), "").Behaviors.Add(new WebHttpBehavior());
            host.Open();
            Console.WriteLine("Host opened");

            SendRequest(baseAddress + "/Process", "HEAD");
            SendRequest(baseAddress + "/Process2", "HEAD");

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_1e95243c_e98b_4d64_9874_f97ee6abe2e4
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
        class ServerInspectorToAddMustUnderstandHeader : IEndpointBehavior, IDispatchMessageInspector
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
                MessageHeader header = MessageHeader.CreateHeader("foo", "bar", "value", true);
                reply.Headers.Add(header);
            }
        }
        class ClientInspectorToRemoveMustUnderstandHeader : IEndpointBehavior, IClientMessageInspector
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
                int headerIndex = reply.Headers.FindHeader("foo", "bar");
                if (headerIndex >= 0)
                {
                    Console.WriteLine("Removing \"mustUnderstand=1\" header");
                    reply.Headers.RemoveAt(headerIndex);
                }
            }

            public object BeforeSendRequest(ref Message request, IClientChannel channel)
            {
                return null;
            }
        }
        static Binding GetBinding()
        {
            TextMessageEncodingBindingElement messageElement = new TextMessageEncodingBindingElement();
            messageElement.MessageVersion = MessageVersion.CreateVersion(EnvelopeVersion.Soap11);
            return new CustomBinding(
                new TextMessageEncodingBindingElement(MessageVersion.Soap11WSAddressing10, Encoding.UTF8),
                new HttpTransportBindingElement());
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            ServiceEndpoint endpoint = host.AddServiceEndpoint(typeof(ITest), GetBinding(), "");
            endpoint.Behaviors.Add(new ServerInspectorToAddMustUnderstandHeader());
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(GetBinding(), new EndpointAddress(baseAddress));
            factory.Endpoint.Behaviors.Add(new ClientInspectorToRemoveMustUnderstandHeader());
            ITest proxy = factory.CreateChannel();
            Console.WriteLine(proxy.Echo("Hello"));

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class MyCode_2
    {
        [DataContract(Namespace = "")]
        [Serializable]
        public class BothModes
        {
            private string str;
            private int i;

            //[DataMember(Name = "IntMember")]
            public int I { get { return this.i; } set { this.i = value; } }
            //[DataMember(Name = "StrMember")]
            public string Str { get { return this.str; } set { this.str = value; } }
        }

        public static void Test()
        {
            MemoryStream ms = new MemoryStream();
            BothModes instance = new BothModes { I = 123, Str = "hello" };
            DataContractSerializer dcs = new DataContractSerializer(typeof(BothModes));
            dcs.WriteObject(ms, instance);
            Console.WriteLine(Encoding.UTF8.GetString(ms.ToArray()));
            ms.SetLength(0);

            Console.WriteLine();
            XmlSerializer xs = new XmlSerializer(typeof(BothModes));
            xs.Serialize(ms, instance);
            Console.WriteLine(Encoding.UTF8.GetString(ms.ToArray()));
        }
    }

    public class Post_7ae16af6_42f3_45d0_9bef_d61a5f2797af_does_not_work
    {
        public class Group1
        {
            public void G1_Op1() { }
            public void G1_Op2() { }
        }
        public class Group2
        {
            public void G2_Op1() { }
            public void G2_Op2() { }
        }
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            Group1 GetGroup1();
            [OperationContract]
            Group2 GetGroup2();
        }
        public class Service : ITest
        {
            public Group1 GetGroup1()
            {
                return new Group1();
            }

            public Group2 GetGroup2()
            {
                return new Group2();
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
        }
    }

    public class Post_7ae16af6_42f3_45d0_9bef_d61a5f2797af_client_facade
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            void G1_Op1();
            [OperationContract]
            void G1_Op2();
            [OperationContract]
            void G2_Op1();
            [OperationContract]
            void G2_Op2();
        }
        public class Service : ITest
        {
            public void G1_Op1()
            {
                Console.WriteLine("In {0}", MethodBase.GetCurrentMethod().Name);
            }
            public void G1_Op2()
            {
                Console.WriteLine("In {0}", MethodBase.GetCurrentMethod().Name);
            }
            public void G2_Op1()
            {
                Console.WriteLine("In {0}", MethodBase.GetCurrentMethod().Name);
            }
            public void G2_Op2()
            {
                Console.WriteLine("In {0}", MethodBase.GetCurrentMethod().Name);
            }
        }

        public class ClientWrapper : ClientBase<ITest>
        {
            Group1Class group1;
            Group2Class group2;
            public ClientWrapper(Binding binding, string endpointAddress)
                : base(binding, new EndpointAddress(endpointAddress))
            {
                this.group1 = new Group1Class(this);
                this.group2 = new Group2Class(this);
            }

            public Group1Class Group1 { get { return this.group1; } }
            public Group2Class Group2 { get { return this.group2; } }

            public class Group1Class
            {
                ClientWrapper parent;
                public Group1Class(ClientWrapper parent) { this.parent = parent; }
                public void Op1()
                {
                    this.parent.Channel.G1_Op1();
                }
                public void Op2()
                {
                    this.parent.Channel.G1_Op2();
                }
            }

            public class Group2Class
            {
                ClientWrapper parent;
                public Group2Class(ClientWrapper parent) { this.parent = parent; }
                public void Op1()
                {
                    this.parent.Channel.G2_Op1();
                }
                public void Op2()
                {
                    this.parent.Channel.G2_Op2();
                }
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

            ClientWrapper client = new ClientWrapper(new BasicHttpBinding(), baseAddress);
            client.Group1.Op1();
            client.Group1.Op2();
            client.Group2.Op1();
            client.Group2.Op2();
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

    public class Post_c412b16d_df90_498d_aff3_104e2039cdc6
    {
        [DataContract(Name = "PurchaseOrder", Namespace = "http://myNameSpace.com/CreatePurchaseOrder")]
        public class PurchaseOrderHeader : IExtensibleDataObject
        {
            private ExtensionDataObject _ExtensionData;
            public virtual ExtensionDataObject ExtensionData
            {
                get
                {
                    return _ExtensionData;
                }
                set
                {
                    _ExtensionData = value;
                }
            }
            [DataMember]
            public string ExternalRef;
            [DataMember]
            public string Contract;
            [DataMember]
            public string VendorNo;
            [DataMember]
            public string AutorizeCode;
            [DataMember]
            public string BuyerCode;
            [DataMember]
            public string OrderCode;
            [DataMember]
            public string TechnicalCoordinatorID;
            [DataMember]
            public string CurrencyCode;
            [DataMember]
            public DateTime OrderDate;
            [DataMember]
            public DateTime WantedReceiptDate;
            [DataMember]
            public string VendorCoNo;
            [DataMember]
            public string CustomerPoNo;
            [DataMember]
            public string LabelNote;
            [DataMember]
            public PurchaseOrderLine[] PurchaseOrderLines;
        }

        [DataContract(Name = "PurchaseOrderLine", Namespace = "http://myNameSpace.com/CreatePurchaseOrder")]
        public class PurchaseOrderLine : IExtensibleDataObject
        {
            private ExtensionDataObject _ExtensionData;
            public virtual ExtensionDataObject ExtensionData
            {
                get
                {
                    return _ExtensionData;
                }
                set
                {
                    _ExtensionData = value;
                }
            }
            [DataMember]
            public int LineNo;
            [DataMember]
            public string PartNo;
            [DataMember]
            public string PartDescription;
            [DataMember]
            public string BuyUnitPrice;
            [DataMember]
            public string BuyUnitMeasure;
            [DataMember]
            public int Quantity;
            [DataMember]
            public string UnitMeasure;
            [DataMember]
            public string CurrencyCode;
            [DataMember]
            public string PartOwnership;
            [DataMember]
            public string Owner;
            [DataMember]
            public DateTime WantedDeliveryDate;
            [DataMember]
            public DateTime PlannedDeliveryDate;
            [DataMember]
            public DateTime PlannedReceiptDate;
        }

        [XmlSerializerFormat]
        [ServiceContract(Namespace = "http://myNameSpace.com/CreatePurchaseOrder")]
        public interface IPurchaseOrderOperations
        {
            [OperationContract(
              Name = "CreatePurchaseOrder",
              Action = "http://myNameSpace.com/CreatePurchaseOrderRequest",
              ReplyAction = "http://myNameSpace.com/CreatePurchaseOrderResponse")]
            bool CreatePurchaseOrders(List<PurchaseOrderHeader> PurchaseOrders);
        }

        public class Service : IPurchaseOrderOperations
        {
            public bool CreatePurchaseOrders(List<PurchaseOrderHeader> PurchaseOrders)
            {
                if (PurchaseOrders == null)
                {
                    Console.WriteLine("PurchaseOrders == null");
                }
                else
                {
                    if (PurchaseOrders.Count == 0)
                    {
                        Console.WriteLine("No purchase orders");
                    }
                    else
                    {
                        for (int i = 0; i < PurchaseOrders.Count; i++)
                        {
                            Console.WriteLine(PurchaseOrders[i].PurchaseOrderLines);
                        }
                    }
                }

                return true;
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
            host.AddServiceEndpoint(typeof(IPurchaseOrderOperations), GetBinding(), "");
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<IPurchaseOrderOperations> factory = new ChannelFactory<IPurchaseOrderOperations>(GetBinding(), new EndpointAddress(baseAddress));
            IPurchaseOrderOperations proxy = factory.CreateChannel();
            Console.WriteLine(proxy.CreatePurchaseOrders(new List<PurchaseOrderHeader> {
                new PurchaseOrderHeader {
                    AutorizeCode = "ac",
                    BuyerCode = "bc" ,
                    Contract = "c",
                    CurrencyCode = "cc",
                    CustomerPoNo = "cpn",
                    ExternalRef = "er",
                    LabelNote = "ln",
                    OrderCode = "oc",
                    OrderDate = DateTime.Now,
                    PurchaseOrderLines = new PurchaseOrderLine[] {
                        new PurchaseOrderLine {
                            BuyUnitMeasure = "bum",
                            BuyUnitPrice = "bup",
                            CurrencyCode = "cc",
                            LineNo = 1,
                            Owner = "o",
                            PartDescription = "pd",
                            PartNo = "pn",
                            PartOwnership = "po",
                            PlannedDeliveryDate = DateTime.Now,
                            PlannedReceiptDate = DateTime.Now,
                            Quantity = 1,
                            UnitMeasure = "um",
                            WantedDeliveryDate = DateTime.Now,
                        },
                    },
                    TechnicalCoordinatorID = "tcid",
                    VendorCoNo = "vcn",
                    VendorNo = "vn",
                    WantedReceiptDate = DateTime.Now,
                }
            }));

            string request = @"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
               xmlns:xsd=""http://www.w3.org/2001/XMLSchema""
               xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
 <soap:Body>
 <CreatePurchaseOrder xmlns=""http://myNameSpace.com/CreatePurchaseOrder"">
  <PurchaseOrders>
  <PurchaseOrder>
   <AutorizeCode>A</AutorizeCode>
   <BuyerCode>B</BuyerCode>
   <Contract>DK101</Contract>
   <CurrencyCode>DKK</CurrencyCode>
   <CustomerPoNo>0000</CustomerPoNo>
   <ExternalRef>26</ExternalRef>
   <LabelNote>01-201841-227</LabelNote>
   <OrderCode>1</OrderCode>
   <OrderDate>2010-11-18T15:30:25</OrderDate>
   <PurchaseOrderLines>
   <PurchaseOrderLine>
    <BuyUnitMeasure>PCS</BuyUnitMeasure>
    <BuyUnitPrice>0</BuyUnitPrice>
    <CurrencyCode>DKK</CurrencyCode>
    <LineNo>1</LineNo>
    <Owner>110381845201</Owner>
    <PartDescription>Cisco GE SFP,LC connector - 773589</PartDescription>
    <PartNo>73.004.459</PartNo>
    <PartOwnership>Customer Owned</PartOwnership>
    <PlannedDeliveryDate>2010-11-18T01:00:00</PlannedDeliveryDate>
    <PlannedReceiptDate>2010-11-18T01:00:00</PlannedReceiptDate>
    <Quantity>9</Quantity>
    <UnitMeasure>PCS</UnitMeasure>
    <WantedDeliveryDate>2010-11-18T01:00:00</WantedDeliveryDate>
   </PurchaseOrderLine>
   </PurchaseOrderLines>
   <TechnicalCoordinatorID>C</TechnicalCoordinatorID>
   <VendorCoNo>PO</VendorCoNo>
   <VendorNo>11845201</VendorNo>
   <WantedReceiptDate>2010-11-18T00:00:00</WantedReceiptDate>
  </PurchaseOrder>
  </PurchaseOrders>
 </CreatePurchaseOrder>
 </soap:Body>
</soap:Envelope>";

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(baseAddress);
            req.Method = "POST";
            req.ContentType = "text/xml";
            req.Headers["SOAPAction"] = "http://myNameSpace.com/CreatePurchaseOrderRequest";
            Stream reqStream = req.GetRequestStream();
            byte[] reqBytes = Encoding.UTF8.GetBytes(request);
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

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_33861d74_9037_4d4f_836b_efe715de5af3
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
        public class MyInspector : IEndpointBehavior, IDispatchMessageInspector
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
                Console.WriteLine("In AfterReceiveRequest, WOC.C.IR.UTM.RU = {0}", WebOperationContext.Current.IncomingRequest.UriTemplateMatch.RequestUri);
                return null;
            }

            public void BeforeSendReply(ref Message reply, object correlationState)
            {
                Console.WriteLine("In BeforeSendReply, WOC.C.IR.UTM.RU = {0}", WebOperationContext.Current.IncomingRequest.UriTemplateMatch.RequestUri);
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

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(new WebHttpBinding(), new EndpointAddress(baseAddress));
            factory.Endpoint.Behaviors.Add(new WebHttpBehavior());
            ITest proxy = factory.CreateChannel();
            Console.WriteLine(proxy.Echo("Hello"));

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class BlogPostComment
    {
        [ServiceContract]
        public class Service
        {
            [WebGet(UriTemplate = "/products.xml")]
            public Stream GetXml()
            {
                string xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<?xml-stylesheet type=""text/xsl"" href=""products.xslt""?>
<products>
  <product>
    <name>Chocolate</name>
    <price>1.30</price>
  </product>
  <product>
    <name>Soda</name>
    <price>2.45</price>
  </product>
  <product>
    <name>Cookies</name>
    <price>7.50</price>
  </product>
</products>";
                WebOperationContext.Current.OutgoingResponse.ContentType = "application/xml";
                return new MemoryStream(Encoding.UTF8.GetBytes(xml));
            }

            [WebGet(UriTemplate = "/products.xslt")]
            public Stream GetXslt()
            {
                string xslt = @"<xsl:stylesheet xmlns:xsl=""http://www.w3.org/1999/XSL/Transform"" version=""1.0"">
<xsl:output method=""html""/>
<xsl:template match=""/"">
<html>
  <head>
    <title>Product directories</title>
  </head>
  <body>
    <xsl:apply-templates />
  </body>
</html>
</xsl:template>

<xsl:template match=""products"">
    <h3>Products in stock</h3>
    <table>
        <tr>
            <th>Name</th>
            <th>Price</th>
        </tr>
        <xsl:for-each select=""product"">
            <tr>
                <td><xsl:value-of select=""name""/></td>
                <td><xsl:value-of select=""price""/></td>
            </tr>
        </xsl:for-each>
    </table>
</xsl:template>
</xsl:stylesheet>
";
                WebOperationContext.Current.OutgoingResponse.ContentType = "application/xml";
                return new MemoryStream(Encoding.UTF8.GetBytes(xslt));
            }
        }

        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            WebServiceHost host = new WebServiceHost(typeof(Service), new Uri(baseAddress));
            host.Open();
            Console.WriteLine("Host opened");
            Console.WriteLine("Press ENTER to close");
            Console.ReadLine();
            host.Close();
        }
    }

    public class IsOneWayTest
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract(IsOneWay = true)]
            void DoWork(DateTime clientTime);
        }
        [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerCall)]
        public class Service : ITest
        {
            public void DoWork(DateTime clientTime)
            {
                Console.WriteLine("[{0}] - client: {1}", DateTime.Now.ToString("HH:mm:ss.fff"), clientTime.ToString("HH:mm:ss.fff"));
                Thread.CurrentThread.Join(500);
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
            for (int i = 0; i < 20; i++)
            {
                proxy.DoWork(DateTime.Now);
            }
        }
    }

    public class Post_ef5f9f03_9312_468e_ae8d_a5b1ac8adaad
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            string Echo(string text);
        }
        [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
        public class Service : ITest
        {
            public string Echo(string text)
            {
                return text;
            }
        }
        public class MyMessageInspector : IDispatchMessageInspector
        {
            public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
            {
                Console.WriteLine("Request:");
                Console.WriteLine(request);
                return null;
            }

            public void BeforeSendReply(ref Message reply, object correlationState)
            {
                Console.WriteLine("Reply:");
                Console.WriteLine(reply);
            }
        }
        public class MyServiceBehavior : IServiceBehavior
        {
            public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
            {
            }

            public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
            {
                foreach (ChannelDispatcher chDisp in serviceHostBase.ChannelDispatchers)
                {
                    foreach (EndpointDispatcher epDisp in chDisp.Endpoints)
                    {
                        epDisp.DispatchRuntime.MessageInspectors.Add(new MyMessageInspector());
                    }
                }
            }

            public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
            {
            }
        }
        public static void Test()
        {
            string baseAddress = "net.tcp://" + Environment.MachineName + ":8000/Service";
            Service service = new Service();
            ServiceHost host = new ServiceHost(service, new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), new NetTcpBinding(), "");
            host.Description.Behaviors.Add(new MyServiceBehavior());
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(new NetTcpBinding(), new EndpointAddress(baseAddress));
            ITest proxy = factory.CreateChannel();
            Console.WriteLine(proxy.Echo("Hello"));

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_dd520d9b_334f_4e78_9b08_bf809b887e4f
    {
        public class MySurrogate : IDataContractSurrogate
        {
            Type originalType;
            IEnumerable<string> fieldsToOmit;
            public MySurrogate(Type originalType, IEnumerable<string> fieldsToOmit)
            {
                this.originalType = originalType;
                this.fieldsToOmit = fieldsToOmit;
            }
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
                if (type == this.originalType)
                {
                    return typeof(MyGenericSurrogate);
                }
                else
                {
                    return type;
                }
            }

            public object GetDeserializedObject(object obj, Type targetType)
            {
                if (targetType == this.originalType && obj is MyGenericSurrogate)
                {
                    DataContractJsonSerializer dcjs = new DataContractJsonSerializer(typeof(MyGenericSurrogate));
                    MemoryStream ms = new MemoryStream();
                    dcjs.WriteObject(ms, obj);
                    ms.Position = 0;
                    dcjs = new DataContractJsonSerializer(targetType);
                    return dcjs.ReadObject(ms);
                }

                return obj;
            }

            public void GetKnownCustomDataTypes(System.Collections.ObjectModel.Collection<Type> customDataTypes)
            {
            }

            public object GetObjectToSerialize(object obj, Type targetType)
            {
                if (this.originalType == obj.GetType())
                {
                    return new MyGenericSurrogate(obj, this.fieldsToOmit);
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
        [Serializable]
        public class MyGenericSurrogate : ISerializable
        {
            object originalInput;
            List<string> fieldsToOmit;
            public MyGenericSurrogate() { }
            public MyGenericSurrogate(SerializationInfo info, StreamingContext context)
            {
                throw new InvalidOperationException("This surrogate class should be used for serialization only");
            }
            public MyGenericSurrogate(object originalInput, IEnumerable<string> fieldsToOmit)
            {
                this.originalInput = originalInput;
                this.fieldsToOmit = new List<string>(fieldsToOmit);
            }

            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                Type originalType = this.originalInput.GetType();
                var dataMembers = from field in originalType.GetFields()
                                  where ContainsAttribute(typeof(DataMemberAttribute), field)
                                    && !this.fieldsToOmit.Contains(field.Name)
                                  select field;
                foreach (var dataMember in dataMembers)
                {
                    info.AddValue(dataMember.Name, dataMember.GetValue(this.originalInput));
                }
            }

            private static bool ContainsAttribute(Type attributeType, MemberInfo member)
            {
                object[] attrs = member.GetCustomAttributes(attributeType, false);
                return attrs != null && attrs.Length > 0;
            }
        }
        [DataContract(Name = "ScalarWrapper", Namespace = "")]
        public class ScalarWrapper
        {
            [DataMember]
            public ScalarOnly scalar = new ScalarOnly();

            public override string ToString()
            {
                return string.Format("ScalarWrapper[scalar={0}]", scalar);
            }
        }
        [DataContract(Name = "ScalarOnly", Namespace = "")]
        public class ScalarOnly
        {
            [DataMember]
            public string str = "hello";
            [DataMember]
            public int i = 123;
            [DataMember]
            public bool b = true;
            [DataMember]
            public double dbl = 1.234;

            public override string ToString()
            {
                return string.Format("ScalarOnly[str={0},i={1},b={2},dbl={3}]", str, i, b, dbl);
            }
        }
        public class Point
        {
            public int x;
            public int y;

            public override string ToString()
            {
                return string.Format("Point[x={0},y={1}]", x, y);
            }
        }
        [DataContract(Name = "ComplexType", Namespace = "")]
        public class ComplexType
        {
            [DataMember]
            public string str = "hello";
            [DataMember]
            public ScalarOnly scalar = new ScalarOnly();
            [DataMember]
            public Point point = new Point { x = 12, y = -32 };
            [DataMember]
            public Point point2 = new Point { x = 22, y = -22 };

            public override string ToString()
            {
                return string.Format("ComplexType[str={0},scalar={1},point={2},point2={3}", str, scalar, point, point2);
            }
        }
        static string Serialize(object instance, Type typeToOmitFields, IEnumerable<string> fieldsToOmit, Type[] knownTypes)
        {
            MySurrogate surrogate = new MySurrogate(typeToOmitFields, fieldsToOmit);
            DataContractJsonSerializer dcjs = new DataContractJsonSerializer(instance.GetType(), knownTypes, int.MaxValue, false, surrogate, false);
            MemoryStream ms = new MemoryStream();
            dcjs.WriteObject(ms, instance);
            return Encoding.UTF8.GetString(ms.ToArray());
        }
        static T Deserialize<T>(string serialized)
        {
            DataContractJsonSerializer dcjs = new DataContractJsonSerializer(typeof(T));
            return (T)dcjs.ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(serialized)));
        }
        public static void Test()
        {
            string serialized = Serialize(new ScalarWrapper(), typeof(ScalarOnly), "str".Split(), null);
            Console.WriteLine(serialized);
            ScalarWrapper deserialized = Deserialize<ScalarWrapper>(serialized);
            Console.WriteLine(deserialized);
            Console.WriteLine();

            serialized = Serialize(new ScalarOnly(), typeof(ScalarOnly), "str".Split(), null);
            Console.WriteLine(serialized);
            ScalarOnly deserialized2 = Deserialize<ScalarOnly>(serialized);
            Console.WriteLine(deserialized2);
            Console.WriteLine();

            serialized = Serialize(new ComplexType(), typeof(ComplexType), "str point2".Split(), new Type[] { typeof(ScalarOnly), typeof(Point) });
            Console.WriteLine(serialized);
            ComplexType deserialized3 = Deserialize<ComplexType>(serialized);
            Console.WriteLine(deserialized3);
        }
    }

    public class BinarySerialization
    {
        public class MyDictionary : IXmlDictionary
        {
            List<XmlDictionaryString> dict = new List<XmlDictionaryString>();
            public MyDictionary(IEnumerable<string> knownStrings)
            {
                foreach (var str in knownStrings)
                {
                    int key = this.dict.Count;
                    dict.Add(new XmlDictionaryString(this, str, key));
                }
            }

            public bool TryLookup(XmlDictionaryString value, out XmlDictionaryString result)
            {
                result = null;
                if (value.Dictionary != this) return false;
                if (value.Key >= this.dict.Count) return false;
                XmlDictionaryString candidate = this.dict[value.Key];
                if (candidate.Value == value.Value)
                {
                    result = candidate;
                    return true;
                }

                return false;
            }

            public bool TryLookup(int key, out XmlDictionaryString result)
            {
                if (key < this.dict.Count)
                {
                    result = this.dict[key];
                    return true;
                }
                else
                {
                    result = null;
                    return false;
                }
            }

            public bool TryLookup(string value, out XmlDictionaryString result)
            {
                foreach (var candidate in this.dict)
                {
                    if (candidate.Value == value)
                    {
                        result = candidate;
                        return true;
                    }
                }

                result = null;
                return false;
            }
        }
        static MyDictionary dictionary = null;
        public static void SerializeToBin<T>(Stream stream, T obj)
        {
            DataContractSerializer dcs = new DataContractSerializer(typeof(T));
            XmlDictionaryWriter xdw =
                dictionary != null ?
                XmlDictionaryWriter.CreateBinaryWriter(stream, dictionary) :
                XmlDictionaryWriter.CreateBinaryWriter(stream);
            dcs.WriteObject(xdw, obj);
            xdw.Flush();
        }
        public static T DeserializeFromBin<T>(Stream stream)
        {
            DataContractSerializer dcs = new DataContractSerializer(typeof(T));
            XmlDictionaryReader xdr = dictionary != null ?
                XmlDictionaryReader.CreateBinaryReader(stream, dictionary, XmlDictionaryReaderQuotas.Max) :
                XmlDictionaryReader.CreateBinaryReader(stream, XmlDictionaryReaderQuotas.Max);
            return (T)dcs.ReadObject(xdr);
        }
        [DataContract]
        public class MyDC
        {
            [DataMember]
            public string member1 = "hello";
            [DataMember]
            public int member2 = 123;
            [DataMember]
            public bool member3 = false;
            [DataMember]
            public double member4 = 12.34;
            [DataMember]
            public string member5 = "world";
            [DataMember]
            public int member6 = -23;
            [DataMember]
            public long member7 = 0;
            [DataMember]
            public decimal member8 = 1;
        }
        public static void Test()
        {
            MemoryStream ms = new MemoryStream();
            MyDC dc = new MyDC();
            SerializeToBin<MyDC>(ms, dc);
            Console.WriteLine("Without dictionary, size = {0}", ms.Position);
            ms.Position = 0;
            MyDC other = DeserializeFromBin<MyDC>(ms);
            Console.WriteLine("Deserialized: {0}", other);
            Console.WriteLine();
            Console.WriteLine("Now with dictionary");
            dictionary = new MyDictionary("member1 member2 member3 member4 member5 member6 member7 member8".Split());
            ms.SetLength(0);
            SerializeToBin<MyDC>(ms, dc);
            Console.WriteLine("With dictionary, size = {0}", ms.Position);
            ms.Position = 0;
            other = DeserializeFromBin<MyDC>(ms);
            Console.WriteLine("Deserialized: {0}", other);
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
        class MyBehavior : IEndpointBehavior
        {
            public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
            {
            }

            public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
            {
                clientRuntime.MessageInspectors.Add(new MyInspector(endpoint));
            }

            public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
            {
            }

            public void Validate(ServiceEndpoint endpoint)
            {
            }
        }
        class MyInspector : IClientMessageInspector
        {
            Dictionary<string, string> actionToReplyAction;
            public MyInspector(ServiceEndpoint endpoint)
            {
                this.actionToReplyAction = new Dictionary<string, string>();
                foreach (var operation in endpoint.Contract.Operations)
                {
                    if (!operation.IsOneWay)
                    {
                        this.actionToReplyAction.Add(operation.Messages[0].Action, operation.Messages[1].Action);
                    }
                }
            }

            public void AfterReceiveReply(ref Message reply, object correlationState)
            {
                string requestAction = (string)correlationState;
                string replyAction = this.actionToReplyAction[requestAction];
                Console.WriteLine("AfterReceiveReply, Action = {0}", reply.Headers.Action);
                Console.WriteLine("   -> ReplyAction: {0}", replyAction);
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
                factory.Endpoint.Behaviors.Add(new MyBehavior());
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

    public class Post_6569a13e_c2a0_4440_ab97_c5c6b753737a
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            string Echo(string input);
        }
        public class Service : ITest
        {
            public string Echo(string input)
            {
                return input;
            }
        }

        class MyResponseSizeLimitingEncodingBindingElement : MessageEncodingBindingElement
        {
            int maxSentMessageSize;
            MessageEncodingBindingElement inner;
            public MyResponseSizeLimitingEncodingBindingElement(MessageEncodingBindingElement inner, int maxSentMessageSize)
            {
                this.maxSentMessageSize = maxSentMessageSize;
                this.inner = inner;
            }
            public override MessageEncoderFactory CreateMessageEncoderFactory()
            {
                return new MyResponseSizeLimitingEncoderFactory(this.inner.CreateMessageEncoderFactory(), this.maxSentMessageSize);
            }

            public override MessageVersion MessageVersion
            {
                get { return this.inner.MessageVersion; }
                set { this.inner.MessageVersion = value; }
            }

            public override BindingElement Clone()
            {
                return new MyResponseSizeLimitingEncodingBindingElement((MessageEncodingBindingElement)this.inner.Clone(), this.maxSentMessageSize);
            }

            public override bool CanBuildChannelListener<TChannel>(BindingContext context)
            {
                return this.inner.CanBuildChannelListener<TChannel>(context);
            }

            public override IChannelListener<TChannel> BuildChannelListener<TChannel>(BindingContext context)
            {
                context.BindingParameters.Add(this);
                return context.BuildInnerChannelListener<TChannel>();
            }

            class MyResponseSizeLimitingEncoderFactory : MessageEncoderFactory
            {
                int maxSentMessageSize;
                MessageEncoderFactory inner;
                public MyResponseSizeLimitingEncoderFactory(MessageEncoderFactory inner, int maxSentMessageSize)
                {
                    this.maxSentMessageSize = maxSentMessageSize;
                    this.inner = inner;
                }

                public override MessageEncoder Encoder
                {
                    get { return new MyResponseSizeLimitingEncoder(this.inner.Encoder, this.maxSentMessageSize); }
                }

                public override MessageVersion MessageVersion
                {
                    get { return this.inner.MessageVersion; }
                }
            }

            class MyResponseSizeLimitingEncoder : MessageEncoder
            {
                int maxSentMessageSize;
                MessageEncoder inner;

                public MyResponseSizeLimitingEncoder(MessageEncoder inner, int maxSentMessageSize)
                {
                    this.maxSentMessageSize = maxSentMessageSize;
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
                    return this.inner.ReadMessage(stream, maxSizeOfHeaders, contentType);
                }

                public override ArraySegment<byte> WriteMessage(Message message, int maxMessageSize, BufferManager bufferManager, int messageOffset)
                {
                    ArraySegment<byte> result = this.inner.WriteMessage(message, maxMessageSize, bufferManager, messageOffset);
                    int messageSize = result.Offset + result.Count;
                    Console.WriteLine("Return message size: {0}", messageSize);
                    if (messageSize > this.maxSentMessageSize)
                    {
                        // This is a quick/dirty solution. This will simply cause the server to abort the socket,
                        // and the client will receive a generic "socket abort exception"
                        //
                        // Another solution would be for this encoder to produce a fault or some
                        // response which the client can understand.
                        Console.WriteLine("Message size ({0}) is greater than MaxSentMessageSize ({1})", messageSize, this.maxSentMessageSize);
                        throw new InvalidOperationException("MaxSentMessageSize exceeded");
                    }

                    return result;
                }

                public override void WriteMessage(Message message, Stream stream)
                {
                    // To implement this, wrap the stream with a "CountingStream", which would throw if
                    // its size grew past MaxSentMessageSize.
                    throw new NotImplementedException();
                }
            }
        }

        static Binding GetBinding(bool server)
        {
            BasicHttpBinding result = new BasicHttpBinding();
            if (server)
            {
                CustomBinding custom = new CustomBinding(result);
                for (int i = 0; i < custom.Elements.Count; i++)
                {
                    MessageEncodingBindingElement mebe = custom.Elements[i] as MessageEncodingBindingElement;
                    if (mebe != null)
                    {
                        custom.Elements[i] = new MyResponseSizeLimitingEncodingBindingElement(mebe, 1000);
                        break;
                    }
                }

                return custom;
            }
            else
            {
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
            Console.WriteLine(proxy.Echo("Hello"));
            Console.WriteLine(proxy.Echo(new string('r', 800)));
            try
            {
                Console.WriteLine(proxy.Echo(new string('r', 1000)));
            }
            catch (Exception e)
            {
                Console.WriteLine("{0}: {1}", e.GetType().FullName, e.Message);
                Exception inner = e.InnerException;
                while (inner != null)
                {
                    Console.WriteLine("   {0}: {1}", inner.GetType().FullName, inner.Message);
                    inner = inner.InnerException;
                }
            }

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_59bf0215_6b27_4fb2_8009_b7a24c7e74c6
    {
        const string serialized = @"<GetItemResponse xmlns=""http://testsite.testapp/testService"">
    <GetItemResult xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"">
        <Id>e0eb114e-b9dd-480c-911d-dd0fa16cd203</Id>
        <Status>0</Status>
        <LastModifiedDate>2011-01-24T09:32:31.8149146-08:00</LastModifiedDate>
        <Name>Item Name</Name>
        <SortIndex>1</SortIndex>
    </GetItemResult>
</GetItemResponse>";

        [DataContract(Name = "Item", Namespace = "http://testsite.testapp/testService")]
        public class Item
        {
            [DataMember(Order = 0)]
            public Guid Id;
            [DataMember(Order = 1)]
            public int Status;
            [DataMember(Order = 2)]
            public DateTime LastModifiedDate;
            [DataMember(Order = 3)]
            public string Name;
            [DataMember(Order = 4)]
            public int SortIndex;
        }

        [DataContract(Name = "GetItemResponse", Namespace = "http://testsite.testapp/testService")]
        public class GetItemResponse
        {
            [DataMember]
            public Item GetItemResult;
        }

        public static void Test()
        {
            DataContractSerializer dcs = new DataContractSerializer(typeof(GetItemResponse));
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(serialized));
            GetItemResponse resp = (GetItemResponse)dcs.ReadObject(ms);
            Console.WriteLine(resp.GetItemResult);
        }
    }

    public class Post_59bf0215_6b27_4fb2_8009_b7a24c7e74c6_b
    {
        [DataContract(Name = "Item", Namespace = "http://testsite.testapp/testService")]
        public class Item
        {
            [DataMember(Order = 0)]
            public Guid Id;
            [DataMember(Order = 1)]
            public int Status;
            [DataMember(Order = 2)]
            public DateTime LastModifiedDate;
            [DataMember(Order = 3)]
            public string Name;
            [DataMember(Order = 4)]
            public int SortIndex;
        }

        [ServiceContract(Namespace = "http://testsite.testapp/testService")]
        public interface ITest
        {
            [WebGet(BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Xml)]
            Item GetItem();
        }

        public class Service : ITest
        {
            public Item GetItem()
            {
                return new Item
                {
                    Id = new Guid("e0eb114e-b9dd-480c-911d-dd0fa16cd203"),
                    Status = 0,
                    LastModifiedDate = new DateTime(2011, 1, 24, 09, 32, 31, 814, DateTimeKind.Local),
                    Name = "Item Name",
                    SortIndex = 1,
                };
            }
        }

        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), new WebHttpBinding(), "").Behaviors.Add(new WebHttpBehavior());
            host.Open();
            Console.WriteLine("Host opened");

            WebClient c = new WebClient();
            string serialized = c.DownloadString(baseAddress + "/GetItem");

            DataContractSerializer dcs = new DataContractSerializer(typeof(Item));
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(serialized));
            Item item = (Item)dcs.ReadObject(ms);
            Console.WriteLine(item);

            host.Close();
        }
    }

    public class Post_379cc5c4_1613_42b1_8342_2456a68f2f48
    {
        [DataContract(Name = "MyDC")]
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
            host.Description.Behaviors.Add(new ServiceMetadataBehavior { HttpGetEnabled = true });
            host.Open();
            Console.WriteLine("Host opened");

            MetadataExchangeClient cli = new MetadataExchangeClient(new Uri(baseAddress + "?wsdl"), MetadataExchangeClientMode.HttpGet);
            WsdlImporter wsdlImporter = new WsdlImporter(cli.GetMetadata());
            System.Collections.ObjectModel.Collection<ContractDescription> allContracts = wsdlImporter.ImportAllContracts();
            System.Collections.ObjectModel.Collection<Binding> allBindings = wsdlImporter.ImportAllBindings();
            ServiceEndpointCollection allEndpoints = wsdlImporter.ImportAllEndpoints();
            CodeCompileUnit unit = new CodeCompileUnit();
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.MachineConfigFilename = ConfigurationManager.OpenMachineConfiguration().FilePath;
            fileMap.ExeConfigFilename = @"c:\temp\deleteme\myoutput.config";
            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            ServiceContractGenerator gen = new ServiceContractGenerator(unit, config);
            gen.GenerateServiceContractType(allContracts[0]);
            string bindingSectionName, configurationName;
            ChannelEndpointElement channelElement;
            gen.GenerateServiceEndpoint(allEndpoints[0], out channelElement);
            gen.GenerateBinding(allBindings[0], out bindingSectionName, out configurationName);
            gen.Configuration.Save();

            foreach (CodeNamespace ns in unit.Namespaces)
            {
                if (ns.Name == "")
                {
                    foreach (CodeTypeDeclaration typeDecl in ns.Types)
                    {
                        if (typeDecl.Name == "TestClient")
                        {
                            typeDecl.Name = "MyOtherTestClient";
                            break;
                        }
                    }
                    break;
                }
            }

            CodeDomProvider provider = CodeDomProvider.CreateProvider("C#");
            System.CodeDom.Compiler.CodeGeneratorOptions options = new CodeGeneratorOptions();
            options.BracingStyle = "C";
            using (StreamWriter sw = File.CreateText(@"c:\temp\deleteme\myproxy.cs"))
            {
                provider.GenerateCodeFromCompileUnit(unit, sw, options);
            }

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_30beaf07_db86_4a0e_a693_0176340fa41f
    {
        [XmlRoot]
        public class Submission
        {
            [XmlElement]
            public string enrolment { get; set; }
            [XmlElement]
            public Student_Serializable student { get; set; }
            [XmlElement]
            public Programme_Serializable programme { get; set; }
            [XmlElement]
            public Module_Serializable module { get; set; }
            [XmlElement]
            public Assessment_Serializable assessment { get; set; }
            [XmlArray]
            public List<element> elements { get; set; }
        }

        public class Student_Serializable
        {
            [XmlAttribute]
            public int id { get; set; }
            [XmlElement]
            public string name { get; set; }
            [XmlElement]
            public int stage { get; set; }
            [XmlElement]
            public int year { get; set; }
        }

        public class Module_Serializable
        {
            [XmlAttribute]
            public int id { get; set; }
            [XmlElement]
            public string code { get; set; }
            [XmlElement]
            public string name { get; set; }
        }

        public class Programme_Serializable
        {
            [XmlAttribute]
            public int id;
            [XmlElement]
            public string Name;
        }

        public class Assessment_Serializable
        {
            [XmlAttribute]
            public int id { get; set; }
            [XmlElement]
            public string type { get; set; }
            [XmlElement]
            public string code { get; set; }
            [XmlElement]
            public string name { get; set; }
            [XmlElement]
            public string theme { get; set; }
            [XmlElement]
            public string block { get; set; }
            [XmlElement]
            public string xsl { get; set; }
            [XmlElement]
            public string submit { get; set; }
        }

        public class element
        {
            [XmlAttribute]
            public int id { get; set; }
            [DataMemberAttribute]
            public bool readOnly { get; set; }
            [XmlElement]
            public int type { get; set; }
            [XmlElement]
            public string code { get; set; }
            [XmlElement]
            public string name { get; set; }
            [XmlElement]
            public string answer { get; set; }
            [XmlArray]
            public List<Option> options;
        }

        public class Option
        {
            [XmlAttribute]
            public string value { get; set; }
            [XmlElement]
            public string text { get; set; }
            [DataMemberAttribute]
            public decimal score { get; set; }
        }

        [ServiceContract]
        public interface ITest
        {
            [OperationContract(Name = "getter")]
            [XmlSerializerFormat]
            [WebGet(BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Xml, UriTemplate = "{id}")]
            Submission GetAssessmentSubmission(string ID);

            [OperationContract(Name = "subber")]
            [XmlSerializerFormat]
            [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Xml, UriTemplate = "sub")]
            string SaveAssessmentSubmission(Submission sub);
        }

        public class Service : ITest
        {
            public Submission GetAssessmentSubmission(string id)
            {
                Submission sub = new Submission
                {
                    student = new Student_Serializable
                    {
                        stage = 51,
                        year = 1255,
                        id = 1,
                        name = "John Doe",
                    },
                    programme = new Programme_Serializable
                    {
                        id = 1,
                        Name = "Bachelor",
                    },
                    module = new Module_Serializable
                    {
                        id = 12,
                        code = "A47RYHGBN40IIZ9JWYXH48EZC2AMJN4ZY29B621L0Z5FS2",
                        name = "Jeff519",
                    },
                    assessment = new Assessment_Serializable
                    {
                        id = 100,
                        type = "PT",
                        code = "GO",
                        theme = "TXQCEXT",
                        xsl = "Danielle",
                        submit = "",
                    },
                    elements = new List<element>
                    {
                        new element
                        {
                            id = 68,
                            options = new List<Option>
                            {
                                new Option
                                {
                                    value = "N",
                                    text = "Not present",
                                    score = 0,
                                },
                                new Option
                                {
                                    value = "U",
                                    text = "Unsatisfactory",
                                    score = 0,
                                },
                                new Option
                                {
                                    value = "B",
                                    text = "Borderline",
                                    score = 0,
                                },
                                new Option
                                {
                                    value = "E",
                                    text = "Excellent",
                                    score = 0,
                                },
                            },
                            readOnly = false,
                            type = 0,
                            code = "06YBTGFISD27H0S5T8",
                            name = "Mr",
                        },
                        new element
                        {
                            id = 73,
                            readOnly = false,
                            type = 0,
                            code = "C1UKJTXRB76L",
                            name = "Mr",
                        },
                    }
                };

                return sub;
            }

            public string SaveAssessmentSubmission(Submission sub)
            {
                string strout = "NONE";
                return strout;
            }
        }

        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), new WebHttpBinding(), "").Behaviors.Add(new WebHttpBehavior());
            host.Open();
            Console.WriteLine("Host opened");

            string returned = Util.SendRequest(baseAddress + "/123", "GET", null, null);
            Util.SendRequest(baseAddress + "/sub", "POST", "text/xml", returned);

            string requestAsInPost = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Submission xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <enrolment>100</enrolment>
  <student id=""1"">
    <stage>51</stage>
    <year>1255</year>
  </student>
  <programme id=""1"">
    <Name>Bachelor</Name>
  </programme>
  <module id=""12"">
    <code>A47RYHGBN40IIZ9JWYXH48EZC2AMJN4ZY29B621L0Z5FS2</code>
    <name>Jeff519</name>
  </module>
  <assessment id=""100"">
    <type>PT</type>
    <code>GO</code>
    <theme>TXQCEXT   </theme>
    <xsl>Danielle</xsl>
    <submit/>
  </assessment>
  <elements>
    <element id=""68"">
      <options>
        <Option value=""N"">
          <text>Not present</text>
          <score>0</score>
        </Option>
        <Option value=""U"">
          <text>Unsatisfactory</text>
          <score>0</score>
        </Option>
        <Option value=""B"">
          <text>Borderline</text>
          <score>0</score>
        </Option>
        <Option value=""E"">
          <text>Excellent</text>
          <score>0</score>
        </Option>
      </options>
      <readOnly>false</readOnly>
      <type>0</type>
      <code>06YBTGFISD27H0S5T8</code>
      <name>Mr</name>
    </element>
    <element id=""73"">
      <readOnly>false</readOnly>
      <type>0</type>
      <code>C1UKJTXRB76L</code>
      <name>Mr</name>
    </element>
  </elements>
</Submission>";

            Util.SendRequest(baseAddress + "/sub", "POST", "text/xml", requestAsInPost);
            host.Close();
        }
    }

    public class Post_3c715630_0b80_4180_94f2_f71e8ddc2e09
    {
        [DataContract]
        public class TcpPerson
        {
            [DataMember]
            public string tcpName;
            [DataMember]
            public string tcpAddress;
        }
        [DataContract]
        public class Person
        {
            [DataMember]
            public string name;
            [DataMember]
            public string address;
        }
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            Person Echo(Person person);
        }
        [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
        public class Service : ITest
        {
            public Person Echo(Person person)
            {
                return person;
            }
        }
        class PersonSurrogate : IDataContractSurrogate
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
                Console.WriteLine("  {0}.{1}", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                if (type == typeof(Person))
                {
                    return typeof(TcpPerson);
                }
                return type;
            }

            public object GetDeserializedObject(object obj, Type targetType)
            {
                Console.WriteLine("  {0}.{1}", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                if (obj is TcpPerson)
                {
                    TcpPerson surrogate = (TcpPerson)obj;
                    Person person = new Person { name = surrogate.tcpName, address = surrogate.tcpAddress };
                    return person;
                }
                return obj;
            }

            public void GetKnownCustomDataTypes(Collection<Type> customDataTypes)
            {
            }

            public object GetObjectToSerialize(object obj, Type targetType)
            {
                Console.WriteLine("  {0}.{1}", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                if (obj is Person)
                {
                    Person person = (Person)obj;
                    TcpPerson surrogate = new TcpPerson
                    {
                        tcpName = person.name,
                        tcpAddress = person.address,
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
        class MyEndpointBehavior : IEndpointBehavior
        {
            internal static List<DataContractSerializerOperationBehavior> allDCSOBInstances = new List<DataContractSerializerOperationBehavior>();
            public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
            {
            }

            public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
            {
                ReplaceDCSOB(endpoint);
            }

            public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
            {
                SaveDCSOB(endpoint);
                ReplaceDCSOB(endpoint);
            }

            private void SaveDCSOB(ServiceEndpoint endpoint)
            {
                foreach (OperationDescription od in endpoint.Contract.Operations)
                {
                    DataContractSerializerOperationBehavior dcsob = od.Behaviors.Find<DataContractSerializerOperationBehavior>();
                    if (dcsob != null)
                    {
                        allDCSOBInstances.Add(dcsob);
                    }
                }
            }

            private static void ReplaceDCSOB(ServiceEndpoint endpoint)
            {
                if (endpoint.Binding.Scheme == "net.tcp")
                {
                    foreach (OperationDescription od in endpoint.Contract.Operations)
                    {
                        for (int i = 0; i < od.Behaviors.Count; i++)
                        {
                            DataContractSerializerOperationBehavior dcsob = od.Behaviors[i] as DataContractSerializerOperationBehavior;
                            if (dcsob != null)
                            {
                                DataContractSerializerOperationBehavior newDCSOB = new DataContractSerializerOperationBehavior(od, dcsob.DataContractFormatAttribute);
                                od.Behaviors[i] = newDCSOB;
                                newDCSOB.DataContractSurrogate = new PersonSurrogate();
                            }
                        }
                    }
                }
            }

            public void Validate(ServiceEndpoint endpoint)
            {
            }
        }
        static void CallService(Binding binding, string endpointAddress)
        {
            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(binding, new EndpointAddress(endpointAddress));
            factory.Endpoint.Behaviors.Add(new MyEndpointBehavior());
            ITest proxy = factory.CreateChannel();
            Person p = new Person { name = "Sherlock Holmes", address = "221B Baker St., London, UK" };
            Person result = proxy.Echo(p);
            Console.WriteLine(result);
            ((IClientChannel)proxy).Close();
            factory.Close();
        }
        public static void Test()
        {
            string baseAddressHttp = "http://" + Environment.MachineName + ":8000/Service";
            string baseAddressTcp = "net.tcp://" + Environment.MachineName + ":8008/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddressHttp), new Uri(baseAddressTcp));
            host.AddServiceEndpoint(typeof(ITest), new BasicHttpBinding(), "basic").Behaviors.Add(new MyEndpointBehavior());
            host.AddServiceEndpoint(typeof(ITest), new NetTcpBinding(SecurityMode.None), "tcp").Behaviors.Add(new MyEndpointBehavior());
            host.Open();
            Console.WriteLine("Host opened");

            Console.WriteLine("{0} DCSOB instances", MyEndpointBehavior.allDCSOBInstances.Count);
            for (int i = 1; i < MyEndpointBehavior.allDCSOBInstances.Count; i++)
            {
                Console.WriteLine("instance[0] is {0} to instance[{1}]",
                    Object.ReferenceEquals(MyEndpointBehavior.allDCSOBInstances[0], MyEndpointBehavior.allDCSOBInstances[i]) ? "equal" : "different",
                    i);
            }

            Console.WriteLine("Calling via HTTP");
            CallService(new BasicHttpBinding(), baseAddressHttp + "/basic");
            Console.WriteLine();
            Console.WriteLine("Calling via TCP");
            CallService(new NetTcpBinding(SecurityMode.None), baseAddressTcp + "/tcp");

            host.Close();
        }
    }

    public class Post_1c5c31c4_fc2b_43d4_9593_48f48d2f157f
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            void ReceiveXElement(XElement input);
            [OperationContract]
            void ReceiveXElementList(XElement[] input);
            [OperationContract]
            void ReceiveXmlNodeArray(XmlNode[] input);
        }
        public class Service : ITest
        {
            public void ReceiveXElement(XElement input)
            {
                Console.WriteLine("in ReceiveXElement, input = ");
                Console.WriteLine(input.ToString(SaveOptions.None));
            }

            public void ReceiveXElementList(XElement[] input)
            {
                Console.WriteLine("in ReceiveXElementList, input.Length = {0}", input == null ? "<<null>>" : input.Length.ToString());
                if (input != null)
                {
                    for (int i = 0; i < input.Length; i++)
                    {
                        Console.WriteLine(input[i]);
                    }
                }
            }

            public void ReceiveXmlNodeArray(XmlNode[] input)
            {
                Console.WriteLine("in ReceiveXmlNodeArray, input.Length = {0}", input == null ? "<<null>>" : input.Length.ToString());
                if (input != null)
                {
                    for (int i = 0; i < input.Length; i++)
                    {
                        Console.WriteLine(input[i].OuterXml);
                    }
                }
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

            string xml = @"<Products>
  <ProductId>1</ProductId>
  <ProductName>Test</ProductName>
  <ProductId>2</ProductId>
  <ProductName>Test2</ProductName>
</Products>";
            XElement products = XElement.Parse(xml);
            proxy.ReceiveXElement(products);

            XElement[] products2 = new XElement[]
            {
                XElement.Parse("<Product><ProductId>1</ProductId><ProductName>Test</ProductName></Product>"),
                XElement.Parse("<Product><ProductId>2</ProductId><ProductName>Test2</ProductName></Product>"),

            };
            proxy.ReceiveXElementList(products2);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            List<XmlNode> nodes = new List<XmlNode>();
            foreach (XmlNode node in doc.DocumentElement.ChildNodes)
            {
                nodes.Add(node);
            }
            proxy.ReceiveXmlNodeArray(nodes.ToArray());

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_ed9a9191_acc1_4c20_85d3_3bf78e53ea4b
    {
        [DataContract]
        public class Resource
        {
            [DataMember]
            public string name;
        }
        [DataContract]
        public class MyDC
        {
            private IList<Resource> resources;
            [DataMember]
            public virtual IList<Resource> WCFResources
            {
                get { return resources; }
                set
                {
                    if (value != null)
                    {
                        resources = new List<Resource>(value);
                    }
                    else
                    {
                        resources = value;
                    }
                }
            }
        }
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            MyDC GetAllResources();
        }
        public class Service : ITest
        {
            public MyDC GetAllResources()
            {
                return new MyDC
                {
                    WCFResources = new List<Resource>
                    {
                        new Resource { name = "resource 1" },
                        new Resource { name = "resource 2" },
                    }
                };
            }
        }
        class MyNetDCSOperationBehavior : DataContractSerializerOperationBehavior
        {
            public MyNetDCSOperationBehavior(OperationDescription operationDescription) : base(operationDescription) { }
            public override XmlObjectSerializer CreateSerializer(Type type, string name, string ns, IList<Type> knownTypes)
            {
                return new NetDataContractSerializer(name, ns);
            }
            public override XmlObjectSerializer CreateSerializer(Type type, XmlDictionaryString name, XmlDictionaryString ns, IList<Type> knownTypes)
            {
                return new NetDataContractSerializer(name, ns);
            }
        }
        static void ReplaceDCSOB(ServiceEndpoint endpoint)
        {
            foreach (OperationDescription od in endpoint.Contract.Operations)
            {
                for (int i = 0; i < od.Behaviors.Count; i++)
                {
                    if (od.Behaviors[i] is DataContractSerializerOperationBehavior)
                    {
                        od.Behaviors[i] = new MyNetDCSOperationBehavior(od);
                    }
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
            ServiceEndpoint endpoint = host.AddServiceEndpoint(typeof(ITest), GetBinding(), "");
            ReplaceDCSOB(endpoint);
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(GetBinding(), new EndpointAddress(baseAddress));
            ReplaceDCSOB(factory.Endpoint);
            ITest proxy = factory.CreateChannel();

            Console.WriteLine(proxy.GetAllResources());

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_14209010_a40a_429c_bc96_8c0fb9dc9e57
    {
        [DataContract]
        public class MyDC
        {
            [DataMember]
            public int i;
            [DataMember]
            public string str;

            public static MyDC[] CreateArray(int size)
            {
                MyDC[] result = new MyDC[size];
                for (int i = 0; i < size; i++)
                {
                    result[i] = new MyDC { i = i, str = "str_" + i };
                }
                return result;
            }
        }
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            int GetLength(MyDC[] array);
        }
        public class Service : ITest
        {
            public int GetLength(MyDC[] array)
            {
                return array.Length;
            }
        }
        static Binding GetBinding()
        {
            BasicHttpBinding result = new BasicHttpBinding();
            result.MaxReceivedMessageSize = int.MaxValue;
            result.ReaderQuotas.MaxArrayLength = int.MaxValue;
            return result;
        }
        static void UpdateMIIOG(ServiceEndpoint endpoint, int newMIIOG)
        {
            foreach (OperationDescription od in endpoint.Contract.Operations)
            {
                DataContractSerializerOperationBehavior dcsob = od.Behaviors.Find<DataContractSerializerOperationBehavior>();
                if (dcsob != null)
                {
                    dcsob.MaxItemsInObjectGraph = newMIIOG;
                }
            }
        }
        public static void Test()
        {
            const int newMIIOG = 1000000;
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            ServiceEndpoint endpoint = host.AddServiceEndpoint(typeof(ITest), GetBinding(), "");
            UpdateMIIOG(endpoint, newMIIOG);
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(GetBinding(), new EndpointAddress(baseAddress));
            UpdateMIIOG(factory.Endpoint, newMIIOG);

            ITest proxy = factory.CreateChannel();

            Console.WriteLine(proxy.GetLength(MyDC.CreateArray(10000)));
            // With the default value for MIIOG, this call fails.
            Console.WriteLine(proxy.GetLength(MyDC.CreateArray(50000)));

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_f778370c_19bb_45a5_afbc_efe0df6b1795
    {
        [DataContract(Name = "MyDC", Namespace = "")]
        public class MyDC
        {
            [DataMember]
            public string Name;
            [DataMember]
            public DateTime DOJ;
        }
        [ServiceContract]
        public interface ITest
        {
            [WebInvoke]
            string Process(MyDC dc);
        }
        public class Service : ITest
        {
            public string Process(MyDC dc)
            {
                return string.Format("{0}: {1} days", dc.Name, DateTime.Now.Subtract(dc.DOJ).Days);
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            WebServiceHost host = new WebServiceHost(typeof(Service), new Uri(baseAddress));
            host.Open();
            Console.WriteLine("Host opened");

            string xml = "<MyDC><DOJ>2011-01-01T00:00:00</DOJ><Name>John Doe</Name></MyDC>";
            byte[] requestBytes = Encoding.UTF8.GetBytes(xml);
            WebClient c = new WebClient();
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(baseAddress + "/Process");
            req.Method = "POST";
            req.ContentType = "text/xml";
            Stream reqStream = req.GetRequestStream();
            reqStream.Write(requestBytes, 0, requestBytes.Length);
            reqStream.Close();
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Console.WriteLine("HTTP/{0} {1} {2}", resp.ProtocolVersion, (int)resp.StatusCode, resp.StatusDescription);
            foreach (var header in resp.Headers.AllKeys)
            {
                Console.WriteLine("{0}: {1}", header, resp.Headers[header]);
            }

            Console.WriteLine(new StreamReader(resp.GetResponseStream()).ReadToEnd());

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_a2968717_2aae_4ae4_bfe1_f80a7b0a0c17
    {
        [DataContract]
        public class OrderHeader
        {
            [DataMember]
            public string Name;
        }
        [DataContract]
        public class OrderItem
        {
            [DataMember]
            public string Name;
            [DataMember]
            public double Price;
        }
        [DataContract]
        public class Order
        {
            [DataMember]
            public OrderHeader orderHeader;
            [DataMember]
            public List<OrderItem> items;
        }
        [ServiceContract]
        public interface ITest
        {
            [WebGet(ResponseFormat = WebMessageFormat.Json)]
            Order GetOrderAsJson();
        }
        public class Service : ITest
        {
            public Order GetOrderAsJson()
            {
                return new Order
                {
                    orderHeader = new OrderHeader
                    {
                        Name = "Main Order",
                    },
                    items = new List<OrderItem>
                    {
                        new OrderItem { Name = "Bread", Price = 1.66 },
                        new OrderItem { Name = "Milk", Price = 2.50 },
                        new OrderItem { Name = "Eggs", Price = 2.43 },
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

            WebClient c = new WebClient();
            Console.WriteLine(c.DownloadString(baseAddress + "/GetOrderAsJson"));

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class MyCode_3
    {
        [DataContract]
        public class MyDC
        {
            [DataMember]
            public string Name;
            [DataMember]
            public byte[] Contents;
        }
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            MyDC Echo(MyDC input);
        }
        public class Service : ITest
        {
            public MyDC Echo(MyDC input)
            {
                return input;
            }
        }

        class MyReadMtomWriteXmlEncodingBindingElement : MessageEncodingBindingElement
        {
            public MtomMessageEncodingBindingElement mtomBE;
            public TextMessageEncodingBindingElement textBE;

            public MyReadMtomWriteXmlEncodingBindingElement()
            {
                this.mtomBE = new MtomMessageEncodingBindingElement();
                this.textBE = new TextMessageEncodingBindingElement();
            }

            public MyReadMtomWriteXmlEncodingBindingElement(MtomMessageEncodingBindingElement mtomBE, TextMessageEncodingBindingElement textBE)
            {
                this.mtomBE = mtomBE;
                this.textBE = textBE;
            }

            public override MessageEncoderFactory CreateMessageEncoderFactory()
            {
                return new MyReadMtomWriteXmlEncoderFactory(this.mtomBE.CreateMessageEncoderFactory(), this.textBE.CreateMessageEncoderFactory());
            }

            public override MessageVersion MessageVersion
            {
                get { return this.mtomBE.MessageVersion; }
                set
                {
                    this.textBE.MessageVersion = value;
                    this.mtomBE.MessageVersion = value;
                }
            }

            public override BindingElement Clone()
            {
                return new MyReadMtomWriteXmlEncodingBindingElement((MtomMessageEncodingBindingElement)this.mtomBE.Clone(), (TextMessageEncodingBindingElement)this.textBE.Clone());
            }

            public override bool CanBuildChannelListener<TChannel>(BindingContext context)
            {
                return this.mtomBE.CanBuildChannelListener<TChannel>(context);
            }

            public override IChannelListener<TChannel> BuildChannelListener<TChannel>(BindingContext context)
            {
                context.BindingParameters.Add(this);
                return context.BuildInnerChannelListener<TChannel>();
            }

            class MyReadMtomWriteXmlEncoderFactory : MessageEncoderFactory
            {
                MessageEncoderFactory mtom;
                MessageEncoderFactory text;

                public MyReadMtomWriteXmlEncoderFactory(MessageEncoderFactory mtom, MessageEncoderFactory text)
                {
                    this.mtom = mtom;
                    this.text = text;
                }

                public override MessageEncoder Encoder
                {
                    get { return new MyReadMtomWriteXmlEncoder(this.mtom.Encoder, this.text.Encoder); }
                }

                public override MessageVersion MessageVersion
                {
                    get { return this.mtom.MessageVersion; }
                }
            }

            class MyReadMtomWriteXmlEncoder : MessageEncoder
            {
                MessageEncoder mtom;
                MessageEncoder text;

                public MyReadMtomWriteXmlEncoder(MessageEncoder mtom, MessageEncoder text)
                {
                    this.mtom = mtom;
                    this.text = text;
                }

                public override string ContentType
                {
                    get { return this.text.ContentType; }
                }

                public override string MediaType
                {
                    get { return this.text.MediaType; }
                }

                public override MessageVersion MessageVersion
                {
                    get { return this.text.MessageVersion; }
                }

                public override bool IsContentTypeSupported(string contentType)
                {
                    return this.mtom.IsContentTypeSupported(contentType);
                }

                public override Message ReadMessage(ArraySegment<byte> buffer, BufferManager bufferManager, string contentType)
                {
                    return this.mtom.ReadMessage(buffer, bufferManager, contentType);
                }

                public override Message ReadMessage(Stream stream, int maxSizeOfHeaders, string contentType)
                {
                    return this.mtom.ReadMessage(stream, maxSizeOfHeaders, contentType);
                }

                public override ArraySegment<byte> WriteMessage(Message message, int maxMessageSize, BufferManager bufferManager, int messageOffset)
                {
                    return this.text.WriteMessage(message, maxMessageSize, bufferManager, messageOffset);
                }

                public override void WriteMessage(Message message, Stream stream)
                {
                    this.text.WriteMessage(message, stream);
                }
            }
        }

        static Binding GetBinding(bool server)
        {
            if (server)
            {
                return new CustomBinding(new MyReadMtomWriteXmlEncodingBindingElement(), new HttpTransportBindingElement());
            }
            else
            {
                return new CustomBinding(new MtomMessageEncodingBindingElement(), new HttpTransportBindingElement());
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

            byte[] fileContents = new byte[10000];
            for (int i = 0; i < fileContents.Length; i++)
            {
                fileContents[i] = (byte)('a' + (i % 26));
            }
            proxy.Echo(new MyDC { Name = "FileName.bin", Contents = fileContents });

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_a3105b84_84ea_4d22_8da9_b78ced9c8d36
    {
        [ServiceContract]
        public interface ITestCallback
        {
            [OperationContract]
            string EchoAtClient(string text);
        }
        [ServiceContract(CallbackContract = typeof(ITestCallback))]
        public interface ITest
        {
            [OperationContract]
            void Connect(string clientName);
        }
        [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
        public class Service : ITest
        {
            public void Connect(string clientName)
            {
                ITestCallback callback = OperationContext.Current.GetCallbackChannel<ITestCallback>();
                ThreadPool.QueueUserWorkItem(delegate
                {
                    Console.WriteLine("[{0}] At server, calling operation on client {1}", Thread.CurrentThread.ManagedThreadId, clientName);
                    string result = callback.EchoAtClient("Hello " + clientName);
                    Console.WriteLine("[{0}] At server, client {1} replied: {2}", Thread.CurrentThread.ManagedThreadId, clientName, result);
                });
            }
        }
        public class MyCallback : ITestCallback
        {
            public string EchoAtClient(string text)
            {
                Thread.Sleep(500);
                Console.WriteLine("[{0}] At client, received {1}", Thread.CurrentThread.ManagedThreadId, text);
                return text;
            }
        }
        public static void Test()
        {
            string baseAddressHttp = "http://" + Environment.MachineName + ":8000/Service";
            string baseAddressTcp = "net.tcp://" + Environment.MachineName + ":8008/Service";
            string baseAddressHttpClient = "http://" + Environment.MachineName + ":8009/Client";

            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddressHttp), new Uri(baseAddressTcp));
            host.AddServiceEndpoint(typeof(ITest), new NetTcpBinding(SecurityMode.None), "tcp");
            WSDualHttpBinding dualBinding = new WSDualHttpBinding(WSDualHttpSecurityMode.None);
            dualBinding.ClientBaseAddress = new Uri(baseAddressHttpClient);
            host.AddServiceEndpoint(typeof(ITest), dualBinding, "wsDual");
            host.Open();
            Console.WriteLine("Host opened");

            MyCallback clientCallback = new MyCallback();

            DuplexChannelFactory<ITest> factory1 = new DuplexChannelFactory<ITest>(new InstanceContext(clientCallback), new NetTcpBinding(SecurityMode.None), baseAddressTcp + "/tcp");
            ITest proxy1 = factory1.CreateChannel();
            proxy1.Connect("TcpClient");

            DuplexChannelFactory<ITest> factory2 = new DuplexChannelFactory<ITest>(new InstanceContext(clientCallback), dualBinding, baseAddressHttp + "/wsDual");
            ITest proxy2 = factory2.CreateChannel();
            proxy2.Connect("HttpClient");

            Console.WriteLine("Press ENTER to close");
            Console.ReadLine();

            ((IClientChannel)proxy1).Close();
            ((IClientChannel)proxy2).Close();
            factory1.Close();
            factory2.Close();
            host.Close();
        }
    }

    public class Post_730e53e1_8284_4717_b6c9_81838b35fa2f
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            string Echo(string text);
        }
        public class Service : ITest
        {
            string name;
            public Service(string name = "Default")
            {
                this.name = name;
            }
            public string Echo(string text)
            {
                return "Hello " + text + " from " + this.name;
            }
        }
        class MyProvider : IInstanceProvider, IEndpointBehavior
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
                return new Service();
            }

            public object GetInstance(InstanceContext instanceContext)
            {
                return new Service();
            }

            public void ReleaseInstance(InstanceContext instanceContext, object instance)
            {
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), new BasicHttpBinding(), "").Behaviors.Add(new MyProvider());
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(new BasicHttpBinding(), new EndpointAddress(baseAddress));
            ITest proxy = factory.CreateChannel();
            Console.WriteLine(proxy.Echo("foo"));

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_2a74b569_89d1_4164_9640_1e2e511f7efe
    {
        [ServiceContract]
        public interface ITestCallback1
        {
            [OperationContract(IsOneWay = true)]
            void Callback1(string text);
        }
        [ServiceContract]
        public interface ITestCallback2 : ITestCallback1
        {
            [OperationContract(IsOneWay = true)]
            void Callback2(string text);
        }
        [ServiceContract(CallbackContract = typeof(ITestCallback1))]
        public interface ITestBase
        {
            [OperationContract]
            void TestBaseOp(string text);
        }
        [ServiceContract(CallbackContract = typeof(ITestCallback2))]
        public interface ITestDerived : ITestBase
        {
            [OperationContract]
            void TestDerivedOp(string text);
        }
        [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
        public class Service : ITestDerived, ITestBase
        {
            public void TestDerivedOp(string text)
            {
                Console.WriteLine("[Service] TestDerivedOp: {0}", text);
                OperationContext.Current.GetCallbackChannel<ITestCallback2>().Callback1(text);
                OperationContext.Current.GetCallbackChannel<ITestCallback2>().Callback2(text);
            }

            void ITestBase.TestBaseOp(string text)
            {
                Console.WriteLine("[Service] ITestBase.TestBaseOp: {0}", text);
                OperationContext.Current.GetCallbackChannel<ITestCallback1>().Callback1(text);
            }

            public void TestBaseOp(string text)
            {
                Console.WriteLine("[Service] ITestDerived.TestBaseOp: {0}", text);
                OperationContext.Current.GetCallbackChannel<ITestCallback2>().Callback1(text);
                OperationContext.Current.GetCallbackChannel<ITestCallback2>().Callback2(text);
            }
        }
        public class MyCallback : ITestCallback1, ITestCallback2
        {
            public void Callback1(string text)
            {
                Console.WriteLine("[Client] Callback1: {0}", text);
            }

            public void Callback2(string text)
            {
                Console.WriteLine("[Client] Callback2: {0}", text);
            }
        }
        public static void Test()
        {
            string baseAddressTcp = "net.tcp://" + Environment.MachineName + ":8008/Service";

            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddressTcp));
            host.AddServiceEndpoint(typeof(ITestBase), new NetTcpBinding(SecurityMode.None), "base");
            host.AddServiceEndpoint(typeof(ITestDerived), new NetTcpBinding(SecurityMode.None), "derived");
            host.Description.Behaviors.Add(new ServiceMetadataBehavior());
            host.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexTcpBinding(), "mex");
            host.Open();
            Console.WriteLine("Host opened");

            MyCallback clientCallback = new MyCallback();

            DuplexChannelFactory<ITestDerived> factory1 = new DuplexChannelFactory<ITestDerived>(new InstanceContext(clientCallback), new NetTcpBinding(SecurityMode.None), baseAddressTcp + "/derived");
            ITestDerived proxy1 = factory1.CreateChannel();
            proxy1.TestBaseOp("calling base");
            proxy1.TestDerivedOp("calling derived");

            DuplexChannelFactory<ITestBase> factory2 = new DuplexChannelFactory<ITestBase>(new InstanceContext(clientCallback), new NetTcpBinding(SecurityMode.None), baseAddressTcp + "/base");
            ITestBase proxy2 = factory2.CreateChannel();
            proxy2.TestBaseOp("calling base");

            Console.WriteLine("Press ENTER to close");
            Console.ReadLine();

            ((IClientChannel)proxy1).Close();
            ((IClientChannel)proxy2).Close();
            factory1.Close();
            factory2.Close();
            host.Close();
        }
    }

    public class Post_0662dc2e_6468_47c5_9676_fa14a56121cb
    {
        [ServiceContract]
        public interface IServ
        {
            [OperationContract, WebGet(UriTemplate = "/")]
            Stream MyStream();
        }

        [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
        class Serv : IServ
        {
            public Stream MyStream()
            {
                var resp = WebOperationContext.Current.OutgoingResponse;
                resp.StatusCode = System.Net.HttpStatusCode.OK;
                return new MyStream();
            }
        }
        public class MyStream : Stream
        {
            int bytesToReturn = 100000;

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
                get { throw new NotSupportedException(); }
            }

            public override long Position
            {
                get { throw new NotSupportedException(); }
                set { throw new NotSupportedException(); }
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                Thread.Sleep(500);
                int toReturn = Math.Min(500, count);
                this.bytesToReturn -= toReturn;
                Console.WriteLine("[{0}] returning {1} bytes", DateTime.Now.ToString("HH:mm:ss.fff"), toReturn);
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
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Serv), new Uri(baseAddress));
            WebHttpBinding binding = new WebHttpBinding();
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.MaxBufferSize = 100;
            binding.TransferMode = TransferMode.Streamed;
            ServiceEndpoint endpoint = host.AddServiceEndpoint(typeof(IServ), binding, "");
            endpoint.Behaviors.Add(new WebHttpBehavior());
            host.Open();
            Console.WriteLine("Host opened");

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(baseAddress + "/");
            req.Method = "GET";
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Stream respStream = resp.GetResponseStream();

            byte[] buffer = new byte[100000];
            int bytesRead;
            while ((bytesRead = respStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                Console.WriteLine("[{0}] {1} bytes", DateTime.Now.ToString("HH:mm:ss.fff"), bytesRead);
            }

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_0f199848_7ae8_4517_a126_d2b633a17882
    {
        [DataContract(Name = "Medication", Namespace = "")]
        public class Medication
        {
            [DataMember]
            public string Name { get; set; }
        }
        [MessageContract(IsWrapped = false)]
        public class MedicationListResponseContract
        {
            [MessageBodyMember(Name = "MedicationListResponse")]
            public Medication[] MedicationList;
        }
        [ServiceContract(Namespace = "")]
        public interface ITest
        {
            [OperationContract]
            MedicationListResponseContract MedicationList();
        }
        public class Service : ITest
        {
            public MedicationListResponseContract MedicationList()
            {
                return new MedicationListResponseContract
                {
                    MedicationList = new Medication[] {
                        new Medication { Name = "aspirin" },
                        new Medication { Name = "tylenol" },
                        new Medication { Name = "advil" },
                    },
                };
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

            Console.WriteLine(proxy.MedicationList());

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_5dddcdad_3c74_43f6_9c04_fb69cf2c6f77
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            string Echo(string text);
        }
        [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
        public class Service : ITest
        {
            public string Echo(string text)
            {
                return text;
            }
        }
        public class MyInspector : IEndpointBehavior, IDispatchMessageInspector
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
                foreach (var header in request.Headers)
                {
                    Console.WriteLine("{0}: {1}", header.Name, header.Namespace);
                    if (header.Namespace == "ns" && header.Name == "token")
                    {
                        IntegrationHeader typed = request.Headers.GetHeader<IntegrationHeader>(header.Name, header.Namespace);
                        Console.WriteLine("  {0}", typed);
                    }
                }
                return null;
            }

            public void BeforeSendReply(ref Message reply, object correlationState)
            {
            }
        }
        [DataContract]
        public class IntegrationHeader
        {
            [DataMember]
            public string LoginName { get; set; }
            [DataMember]
            public string Password { get; set; }
            public override string ToString()
            {
                return string.Format("IntegrationHeader[LoginName={0},Password={1}]", LoginName, Password);
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

            using (new OperationContextScope((IContextChannel)proxy))
            {
                IntegrationHeader ih = new IntegrationHeader { LoginName = "john doe", Password = "******" };
                //MessageHeader<IntegrationHeader> mhg = new MessageHeader<IntegrationHeader>();
                //mhg.Content = new IntegrationHeader { LoginName = "john doe", Password = "******" };
                //mhg.Actor = "";
                //MessageHeader untyped = mhg.GetUntypedHeader("token", "ns");
                MessageHeader untyped = MessageHeader.CreateHeader("token", "ns", ih);
                OperationContext.Current.OutgoingMessageHeaders.Add(untyped);
                Console.WriteLine(proxy.Echo("Hello"));
            }

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_69ebfa7c_8a73_4fd1_adf3_d9deb863efad
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
                HttpRequestMessageProperty prop = (HttpRequestMessageProperty)OperationContext.Current.IncomingMessageProperties[HttpRequestMessageProperty.Name];
                Console.WriteLine("In server, HTTP headers:");
                foreach (string headerName in prop.Headers.AllKeys)
                {
                    Console.WriteLine("  {0}: {1}", headerName, prop.Headers[headerName]);
                }

                return text;
            }
        }
        class MyClientInspector : IEndpointBehavior, IClientMessageInspector
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
                HttpRequestMessageProperty prop;
                if (request.Properties.ContainsKey(HttpRequestMessageProperty.Name))
                {
                    prop = (HttpRequestMessageProperty)request.Properties[HttpRequestMessageProperty.Name];
                }
                else
                {
                    prop = new HttpRequestMessageProperty();
                    request.Properties.Add(HttpRequestMessageProperty.Name, prop);
                }

                prop.Headers.Add("X-MyCustom", "HeaderValue");
                return null;
            }
        }
        static Binding GetBinding()
        {
            WSHttpBinding binding = new WSHttpBinding(SecurityMode.TransportWithMessageCredential);
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
            binding.Security.Message.ClientCredentialType = MessageCredentialType.UserName;
            return binding;
        }
        public static void Test()
        {
            ServicePointManager.ServerCertificateValidationCallback = delegate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
            {
                Console.WriteLine("Removing {0}", certificate.Subject);
                return true;
            };
            string baseAddress = "https://" + Environment.MachineName + ":8009/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            ServiceEndpoint endpoint = host.AddServiceEndpoint(typeof(ITest), GetBinding(), "");
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(GetBinding(), new EndpointAddress(baseAddress));
            factory.Endpoint.Behaviors.Add(new MyClientInspector());
            factory.Credentials.UserName.UserName = "john";
            factory.Credentials.UserName.Password = "doe";
            ITest proxy = factory.CreateChannel();
            Console.WriteLine(proxy.Echo("Hello"));

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_9d3bf0b6_8958_4731_ba43_8ee10c720e31
    {
        [DataContract(Name = "Customer", Namespace = "")]
        public class Customer
        {
            [DataMember(Order = 0)]
            public string Name { get; set; }
            [DataMember(Order = 1)]
            public Address Address { get; set; }
            public string Id { get; set; }
        }
        [DataContract(Name = "Address", Namespace = "")]
        public class Address
        {
            [DataMember(Order = 0)]
            public string Street { get; set; }
            [DataMember(Order = 1)]
            public string City { get; set; }
            [DataMember(Order = 2)]
            public string Zip { get; set; }
        }
        [ServiceContract]
        public class Repository
        {
            public static List<Customer> customers = new List<Customer>();

            [WebGet(UriTemplate = "/Customer/{id}", ResponseFormat = WebMessageFormat.Json)]
            public Customer GetCustomer(string id)
            {
                foreach (Customer customer in customers)
                {
                    if (customer.Id == id)
                    {
                        return customer;
                    }
                }

                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.NotFound;
                return null;
            }

            [WebInvoke(Method = "PUT", UriTemplate = "/Customer", ResponseFormat = WebMessageFormat.Json)]
            public int CreateCustomer(Customer customer)
            {
                int newId = customers.Count;
                customer.Id = newId.ToString();
                customers.Add(customer);
                return newId;
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            WebServiceHost host = new WebServiceHost(typeof(Repository), new Uri(baseAddress));
            host.Open();
            Console.WriteLine("Host opened");

            string newCustomer = "<Customer><Name>John Doe</Name><Address><Street>1234 56th St</Street><City>Springfield</City><Zip>11111</Zip></Address></Customer>";
            string id = Util.SendRequest(baseAddress + "/Customer", "PUT", "text/xml", newCustomer);

            Util.SendRequest(baseAddress + "/Customer/" + id, "GET", null, null);

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_271a589a_8c19_4ca7_92b3_90bf1b3fb484
    {
        [ServiceContract]
        public interface ITest
        {
            [WebGet(ResponseFormat = WebMessageFormat.Json)]
            string GetAsJson(string userid, string password);
            [WebGet]
            Stream GetAsRaw(string userid, string password);
        }
        public class Service : ITest
        {
            public string GetAsJson(string userid, string password)
            {
                string result = "IvejqSWrZLDm7VHPjVh/PhzyEwh0oYnhWPi1QYnuZkCYkJ8W3sDwXPXRJv8s4nv2";
                return result;
            }

            public Stream GetAsRaw(string userid, string password)
            {
                string result = this.GetAsJson(userid, password);
                WebOperationContext.Current.OutgoingResponse.ContentType = "text/plain; charset=utf-8";
                return new MemoryStream(Encoding.UTF8.GetBytes(result));
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            WebServiceHost host = new WebServiceHost(typeof(Service), new Uri(baseAddress));
            host.Open();
            Console.WriteLine("Host opened");

            WebClient c = new WebClient();
            Console.WriteLine(c.DownloadString(baseAddress + "/GetAsJson"));
            Console.WriteLine(c.DownloadString(baseAddress + "/GetAsRaw"));
        }
    }

    public class MyCode_4
    {
        public class MyMessageEncodingBindingElement : MessageEncodingBindingElement
        {
            MessageEncodingBindingElement inner;
            public MyMessageEncodingBindingElement(MessageEncodingBindingElement inner)
            {
                this.inner = inner;
            }

            private MyMessageEncodingBindingElement(MyMessageEncodingBindingElement other)
            {
                this.inner = other.inner;
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
                return new MyMessageEncodingBindingElement(this);
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

            public override bool CanBuildChannelFactory<TChannel>(BindingContext context)
            {
                return context.CanBuildInnerChannelFactory<TChannel>();
            }

            public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
            {
                context.BindingParameters.Add(this);
                return context.BuildInnerChannelFactory<TChannel>();
            }

            public static CustomBinding ReplaceEncodingBindingElement(Binding originalBinding)
            {
                CustomBinding custom = originalBinding as CustomBinding;
                if (custom == null)
                {
                    custom = new CustomBinding(originalBinding);
                }

                for (int i = 0; i < custom.Elements.Count; i++)
                {
                    if (custom.Elements[i] is MessageEncodingBindingElement)
                    {
                        MyMessageEncodingBindingElement element = new MyMessageEncodingBindingElement((MessageEncodingBindingElement)custom.Elements[i]);
                        custom.Elements[i] = element;
                    }
                }

                return custom;
            }

            class MyMessageEncoderFactory : MessageEncoderFactory
            {
                private MessageEncoderFactory messageEncoderFactory;

                public MyMessageEncoderFactory(MessageEncoderFactory messageEncoderFactory)
                {
                    this.messageEncoderFactory = messageEncoderFactory;
                }

                public override MessageEncoder Encoder
                {
                    get { return new MyMessageEncoder(this.messageEncoderFactory.Encoder); }
                }

                public override MessageVersion MessageVersion
                {
                    get { return this.messageEncoderFactory.MessageVersion; }
                }

                public override MessageEncoder CreateSessionEncoder()
                {
                    return new MyMessageEncoder(this.messageEncoderFactory.CreateSessionEncoder());
                }
            }

            class MyMessageEncoder : MessageEncoder
            {
                private MessageEncoder messageEncoder;

                public MyMessageEncoder(MessageEncoder messageEncoder)
                {
                    this.messageEncoder = messageEncoder;
                }

                public override string ContentType
                {
                    get { return this.messageEncoder.ContentType; }
                }

                public override string MediaType
                {
                    get { return this.messageEncoder.MediaType; }
                }

                public override MessageVersion MessageVersion
                {
                    get { return this.messageEncoder.MessageVersion; }
                }

                public override bool IsContentTypeSupported(string contentType)
                {
                    return this.messageEncoder.IsContentTypeSupported(contentType);
                }

                public override Message ReadMessage(ArraySegment<byte> buffer, BufferManager bufferManager, string contentType)
                {
                    Console.WriteLine("Raw request: {0}", Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count));
                    return this.messageEncoder.ReadMessage(buffer, bufferManager, contentType);
                }

                public override Message ReadMessage(Stream stream, int maxSizeOfHeaders, string contentType)
                {
                    throw new NotSupportedException("Streamed not supported");
                }

                public override ArraySegment<byte> WriteMessage(Message message, int maxMessageSize, BufferManager bufferManager, int messageOffset)
                {
                    return this.messageEncoder.WriteMessage(message, maxMessageSize, bufferManager, messageOffset);
                }

                public override void WriteMessage(Message message, Stream stream)
                {
                    throw new NotSupportedException("Streamed not supported");
                }
            }
        }

        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
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
            ServiceHost host = new WebServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), MyMessageEncodingBindingElement.ReplaceEncodingBindingElement(new BasicHttpBinding()), "basic");
            host.AddServiceEndpoint(typeof(ITest), MyMessageEncodingBindingElement.ReplaceEncodingBindingElement(new WebHttpBinding()), "rest").Behaviors.Add(new WebHttpBehavior());
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(new BasicHttpBinding(), new EndpointAddress(baseAddress + "/basic"));
            ITest proxy = factory.CreateChannel();
            Console.WriteLine(proxy.Add(4, 5));
            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.WriteLine();

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(baseAddress + "/rest/Add");
            req.Method = "POST";
            req.ContentType = "application/json";
            byte[] reqBytes = Encoding.UTF8.GetBytes("{\"x\":4,\"y\":8}");
            req.GetRequestStream().Write(reqBytes, 0, reqBytes.Length);
            req.GetRequestStream().Close();
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Console.WriteLine(new StreamReader(resp.GetResponseStream()).ReadToEnd());

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_ff233917_eabf_47a3_8127_55fac4188b94
    {
        [DataContract(Name = "MyType")]
        public class MyType
        {
            [DataMember(Name = "Array")]
            private double[][] surrogateArray;

            //Non-data member
            public double[,] ArrayMember;

            [DataMember]
            public string StringMember;

            [OnSerializing]
            public void BeforeSerializing(StreamingContext ctx)
            {
                int dimension0 = this.ArrayMember.GetLength(0);
                int dimension1 = this.ArrayMember.GetLength(1);
                this.surrogateArray = new double[dimension0][];
                for (int i = 0; i < dimension0; i++)
                {
                    this.surrogateArray[i] = new double[dimension1];
                    for (int j = 0; j < dimension1; j++)
                    {
                        this.surrogateArray[i][j] = this.ArrayMember[i, j];
                    }
                }
            }

            [OnDeserialized]
            public void AfterDeserializing(StreamingContext ctx)
            {
                if (this.surrogateArray == null)
                {
                    this.ArrayMember = null;
                }
                else
                {
                    int dimension0 = this.surrogateArray.Length;
                    if (dimension0 == 0)
                    {
                        this.ArrayMember = new double[0, 0];
                    }
                    else
                    {
                        int dimension1 = this.surrogateArray[0].Length;
                        for (int i = 1; i < dimension0; i++)
                        {
                            if (this.surrogateArray[i].Length != dimension1)
                            {
                                throw new InvalidOperationException("Surrogate (jagged) array does not correspond to a rectangular one");
                            }
                        }

                        this.ArrayMember = new double[dimension0, dimension1];
                        for (int i = 0; i < dimension0; i++)
                        {
                            for (int j = 0; j < dimension1; j++)
                            {
                                this.ArrayMember[i, j] = this.surrogateArray[i][j];
                            }
                        }
                    }
                }
            }
        }

        public static void Test()
        {
            DataContractSerializer dcs = new DataContractSerializer(typeof(MyType));
            MemoryStream ms = new MemoryStream();
            MyType instance = new MyType();
            instance.StringMember = "Hello";
            instance.ArrayMember = new double[2, 3];
            instance.ArrayMember[0, 0] = 0;
            instance.ArrayMember[0, 1] = 1;
            instance.ArrayMember[0, 2] = 2;
            instance.ArrayMember[1, 0] = 3;
            instance.ArrayMember[1, 1] = 4;
            instance.ArrayMember[1, 2] = 5;
            dcs.WriteObject(ms, instance);
            Console.WriteLine("Serialized: {0}", Encoding.UTF8.GetString(ms.ToArray()));

            ms.Position = 0;
            MyType newInstance = (MyType)dcs.ReadObject(ms);
            Console.WriteLine(newInstance.StringMember);
            Console.WriteLine(newInstance.ArrayMember[1, 2]);
        }
    }

    public class Post_9fc5c26f_e2ee_410f_8490_a046e6d35770
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
                return OperationContext.Current.IncomingMessageHeaders.MessageId.ToString();
            }
        }
        class ClientMessageInspector : IEndpointBehavior, IClientMessageInspector
        {
            public string RequestMessage { get; set; }
            public string ResponseMessage { get; set; }
            public UniqueId MessageId { get; set; }

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
                this.ResponseMessage = reply.ToString();
            }

            public object BeforeSendRequest(ref Message request, IClientChannel channel)
            {
                request.Headers.MessageId = this.MessageId;
                this.RequestMessage = request.ToString();

                return null;
            }
        }
        static Binding GetBinding()
        {
            return new WSHttpBinding(SecurityMode.None);
        }
        class MyClient : ClientBase<ITest>, ITest
        {
            public MyClient(Binding binding, EndpointAddress endpointAddress)
                : base(binding, endpointAddress)
            {
            }

            public string Echo(string text)
            {
                return this.Channel.Echo(text);
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            ServiceEndpoint endpoint = host.AddServiceEndpoint(typeof(ITest), GetBinding(), "");
            host.Open();
            Console.WriteLine("Host opened");

            ClientMessageInspector inspector1 = new ClientMessageInspector();
            byte[] id = new byte[16]; for (int i = 0; i < id.Length; i++) id[i] = (byte)1;
            inspector1.MessageId = new UniqueId(id);
            MyClient client = new MyClient(GetBinding(), new EndpointAddress(baseAddress));
            client.Endpoint.Behaviors.Add(inspector1);

            ChannelFactory<ITest> factory1 = new ChannelFactory<ITest>(client.Endpoint);
            ITest proxy1 = factory1.CreateChannel();
            using (new OperationContextScope((IContextChannel)proxy1))
            {
                OperationContext.Current.OutgoingMessageHeaders.Add(MessageHeader.CreateHeader("name", "ns", "value"));
                Console.WriteLine(proxy1.Echo("foo"));
            }

            Console.WriteLine(inspector1.RequestMessage);
            Console.WriteLine(inspector1.ResponseMessage);

            Console.Write("Press ENTER to close the host");
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
            public IStreamProvider stream1;
            [DataMember]
            public IStreamProvider stream2;
        }
        public class MyStreamProvider : IStreamProvider
        {
            private Stream stream;
            public MyStreamProvider(Stream stream)
            {
                this.stream = stream;
            }
            public Stream GetStream()
            {
                return this.stream;
            }

            public void ReleaseStream(Stream stream)
            {
            }
        }
        public static void Test()
        {
            DataContractSerializer dcs = new DataContractSerializer(typeof(MyTypeWithBinaryProperties));
            MemoryStream ms = new MemoryStream();
            MyTypeWithBinaryProperties instance = new MyTypeWithBinaryProperties();
            byte[] array1 = new byte[1000];
            byte[] array2 = new byte[1000];
            for (int i = 0; i < 1000; i++)
            {
                if ((i % 80) == 79)
                {
                    array1[i] = (byte)'\n';
                    array2[i] = (byte)'\n';
                }
                else
                {
                    array1[i] = (byte)'1';
                    array2[i] = (byte)'2';
                }
            }
            instance.stream1 = new MyStreamProvider(new MemoryStream(array1));
            instance.stream2 = new MyStreamProvider(new MemoryStream(array2));
            XmlDictionaryWriter mtomWriter = XmlDictionaryWriter.CreateMtomWriter(ms, Encoding.UTF8, int.MaxValue, "text/xml");
            dcs.WriteObject(mtomWriter, instance);
            mtomWriter.Flush();
            Console.WriteLine(Encoding.UTF8.GetString(ms.ToArray()));
        }
    }

    public class Post_526bdc2f_9639_4f50_8409_be37a6137654
    {
        [ServiceContract(CallbackContract = typeof(ICallBack))]
        public interface ITestService
        {
            [OperationContract(IsOneWay = false)]
            void AddValues(int a, int b, out int c);
        }
        public interface ICallBack
        {
            [OperationContract(IsOneWay = true)]
            void ResultConfirmation(string message);
        }
        public class Service : ITestService
        {
            public void AddValues(int a, int b, out int c)
            {
                c = a + b;
                OperationContext.Current.GetCallbackChannel<ICallBack>().ResultConfirmation("c = " + c);
            }
        }
        static Binding GetBinding()
        {
            WSDualHttpBinding result = new WSDualHttpBinding();
            //Change binding settings here
            return result;
        }
        public class MyCallback : ICallBack
        {
            public void ResultConfirmation(string message)
            {
                Console.WriteLine("[From server] {0}", message);
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITestService), GetBinding(), "");
            host.Open();
            Console.WriteLine("Host opened");

            InstanceContext instanceContext = new InstanceContext(new MyCallback());
            DuplexChannelFactory<ITestService> factory = new DuplexChannelFactory<ITestService>(instanceContext, GetBinding(), new EndpointAddress(baseAddress));
            ITestService proxy = factory.CreateChannel();
            int c;
            proxy.AddValues(33, 55, out c);
            Console.WriteLine("c = {0}", c);

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class MyCode_5
    {
        [DataContract(Name = "response", Namespace = "")]
        public sealed class Response<T> where T : class
        {
            [DataMember(Name = "status_code")]
            public int StatusCode { get; set; }

            [DataMember(Name = "status_txt")]
            public string StatusText { get; set; }

            [DataMember(Name = "data")]
            public T Data;

            public override string ToString()
            {
                return string.Format("Response<{0}>[StatusCode={1},StatusText={2},Data={3}]",
                    typeof(T).Name, StatusCode, StatusText, Data);
            }
        }

        [DataContract(Name = "data", Namespace = "")]
        public sealed class ShortenResponse
        {
            [DataMember(Name = "url")]
            public string Url { get; set; }

            [DataMember(Name = "hash")]
            public string Hash { get; set; }

            [DataMember(Name = "global_hash")]
            public string GlobalHash { get; set; }

            [DataMember(Name = "long_url")]
            public string LongUrl { get; set; }

            [DataMember(Name = "new_hash")]
            public int NewHash { get; set; }

            public override string ToString()
            {
                return string.Format("ShortenResponse[Url={0},Hash={1},GlobalHash={2},LongUrl={3},NewHash={4}",
                    Url, Hash, GlobalHash, LongUrl, NewHash);
            }
        }

        [ServiceContract]
        internal interface IBitlyService
        {
            [OperationContract]
            [WebGet(UriTemplate = "/v3/shorten?login=user&apikey=key&longurl={url}&domain=bit.ly&x_login={user}&x_apikey={key}&format=xml", ResponseFormat = WebMessageFormat.Xml)]
            Response<ShortenResponse> Shorten(string url, string user, string key);
        }

        public class Service : IBitlyService
        {
            public Response<ShortenResponse> Shorten(string url, string user, string key)
            {
                Response<ShortenResponse> resp = new Response<ShortenResponse>
                {
                    StatusCode = 200,
                    StatusText = "OK",
                    Data = new ShortenResponse
                    {
                        GlobalHash = "globalhash",
                        Hash = "hash",
                        LongUrl = "longurl",
                        NewHash = 1234,
                        Url = "url",
                    }
                };

                return resp;
            }
        }

        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            WebServiceHost host = new WebServiceHost(typeof(Service), new Uri(baseAddress));
            host.Open();
            Console.WriteLine("Host opened");

            WebClient c = new WebClient();
            string downloaded = c.DownloadString(baseAddress + "/v3/shorten?login=user&apikey=key&longurl=the_long_url&domain=bit.ly&x_login=john_doe&x_apikey=abcdef0123456789&format=xml");
            Console.WriteLine("Via WebClient");
            Console.WriteLine(downloaded);
            Console.WriteLine();

            DataContractSerializer dcs = new DataContractSerializer(typeof(Response<ShortenResponse>));
            Response<ShortenResponse> resp = (Response<ShortenResponse>)dcs.ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(downloaded)));
            Console.WriteLine("Deserialization");
            Console.WriteLine(resp);
            Console.WriteLine();

            ChannelFactory<IBitlyService> factory = new ChannelFactory<IBitlyService>(new WebHttpBinding(), new EndpointAddress(baseAddress));
            factory.Endpoint.Behaviors.Add(new WebHttpBehavior());
            IBitlyService proxy = factory.CreateChannel();
            resp = proxy.Shorten("myurl", "username", "userkey");
            Console.WriteLine("Via proxy");
            Console.WriteLine(resp);
            Console.WriteLine();
        }
    }

    public class Post_0ec30e3c_ece9_48f7_a7b6_e036070b30a0
    {
        [DataContract]
        public class ABC
        {
            private AnotherClass _intabc;

            public AnotherClass IntAbc
            {
                get { return _intabc; }
                set { _intabc = value; }
            }

            public ABC()
            {
                this.Initialize();
            }

            private void Initialize()
            {
                _intabc = new AnotherClass();
            }

            [OnDeserializing]
            private void DeserializationInitializer(StreamingContext ctx)
            {
                this.Initialize();
            }
        }

        public class AnotherClass { }

        public static void Test()
        {
            MemoryStream ms = new MemoryStream();
            DataContractSerializer dcs = new DataContractSerializer(typeof(ABC));
            dcs.WriteObject(ms, new ABC());
            Console.WriteLine("Serialized: {0}", Encoding.UTF8.GetString(ms.ToArray()));
            ms.Position = 0;
            ABC newInstance = (ABC)dcs.ReadObject(ms);
            Console.WriteLine(newInstance);
            Console.WriteLine(newInstance.IntAbc);
        }
    }

    public class Post_09851985_ee54_4627_9af7_6a9505c2067f
    {
        [DataContract]
        public class Person
        {
            [DataMember]
            public string name;
            [DataMember]
            public string address;
        }
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            Person Echo(Person person);
        }
        [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
        public class Service : ITest
        {
            public Person Echo(Person person)
            {
                Console.WriteLine("Request from {0}", OperationContext.Current.EndpointDispatcher.EndpointAddress.Uri);
                return person;
            }
        }
        public static void SingleHost()
        {
            string baseAddressHttp = "http://" + Environment.MachineName + ":8000/Service";
            string baseAddressTcp = "net.tcp://" + Environment.MachineName + ":8008/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddressHttp), new Uri(baseAddressTcp));
            host.AddServiceEndpoint(typeof(ITest), new BasicHttpBinding(), "basic");
            host.AddServiceEndpoint(typeof(ITest), new NetTcpBinding(SecurityMode.None), "tcp");

            host.Description.Behaviors.Add(new ServiceMetadataBehavior());
            host.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexHttpBinding(), "mex");
            host.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexTcpBinding(), "mex");

            host.Open();
            Console.WriteLine("Host opened");

            Console.WriteLine("Press ENTER to close");
            Console.ReadLine();

            host.Close();
        }
        public static void TwoHosts()
        {
            string baseAddressHttp = "http://" + Environment.MachineName + ":8000/Service";
            string baseAddressTcp = "net.tcp://" + Environment.MachineName + ":8008/Service";
            ServiceHost httpHost = new ServiceHost(typeof(Service), new Uri(baseAddressHttp));
            ServiceHost tcpHost = new ServiceHost(typeof(Service), new Uri(baseAddressTcp));
            httpHost.AddServiceEndpoint(typeof(ITest), new BasicHttpBinding(), "basic");
            tcpHost.AddServiceEndpoint(typeof(ITest), new NetTcpBinding(SecurityMode.None), "tcp");

            httpHost.Description.Behaviors.Add(new ServiceMetadataBehavior());
            tcpHost.Description.Behaviors.Add(new ServiceMetadataBehavior());
            httpHost.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexHttpBinding(), "mex");
            tcpHost.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexTcpBinding(), "mex");

            httpHost.Open();
            tcpHost.Open();
            Console.WriteLine("Host opened");

            Console.WriteLine("Press ENTER to close");
            Console.ReadLine();

            httpHost.Close();
            tcpHost.Close();
        }
        public static void Test()
        {
            SingleHost();
        }
    }

    public class Post_d70daa13_6b41_4f65_b292_d1473465b5ab
    {
        [DataContract(Name = "HELDRequest", Namespace = "")]
        public class HELDRequest
        {
            [DataMember]
            public string LocationRequest;
        }
        [DataContract(Name = "HELDResponse", Namespace = "")]
        public class HELDResponse
        {
            [DataMember]
            public string LocationResponse;

            public HELDResponse(string location)
            {
                this.LocationResponse = location;
            }

            internal void Build() { }
        }
        [ServiceContract]
        public interface ITest
        {
            [WebInvoke(Method = "POST", UriTemplate = "held",
            RequestFormat = WebMessageFormat.Xml,
            ResponseFormat = WebMessageFormat.Xml,
            BodyStyle = WebMessageBodyStyle.Bare)]
            HELDResponse LocationRequest(HELDRequest enrData);
        }
        public class Service : ITest
        {
            public HELDResponse LocationRequest(HELDRequest request)
            {
                WebOperationContext.Current.OutgoingResponse.ContentType = "application/held+xml";
                HELDResponse response = new HELDResponse(request.LocationRequest);
                response.Build();
                return response;
            }
        }
        static void SendRequest(string uri, string method, string contentType, string body)
        {
            string responseBody = null;

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(uri);
            req.Method = method;

            if (!String.IsNullOrEmpty(contentType))
            {
                req.ContentType = contentType;
            }

            if (body != null)
            {
                byte[] bodyBytes = Encoding.UTF8.GetBytes(body);
                req.GetRequestStream().Write(bodyBytes, 0, bodyBytes.Length);
                req.GetRequestStream().Close();
            }

            HttpWebResponse resp;
            try
            {
                resp = (HttpWebResponse)req.GetResponse();
            }
            catch (WebException e)
            {
                resp = (HttpWebResponse)e.Response;
            }

            if (resp == null)
            {
                responseBody = null;
                Console.WriteLine("Response is null");
            }
            else
            {
                Console.WriteLine("HTTP/{0} {1} {2}", resp.ProtocolVersion, (int)resp.StatusCode, resp.StatusDescription);
                foreach (string headerName in resp.Headers.AllKeys)
                {
                    Console.WriteLine("{0}: {1}", headerName, resp.Headers[headerName]);
                }
                Console.WriteLine();
                Stream respStream = resp.GetResponseStream();
                if (respStream != null)
                {
                    responseBody = new StreamReader(respStream).ReadToEnd();
                    Console.WriteLine(responseBody);
                }
                else
                {
                    Console.WriteLine("HttpWebResponse.GetResponseStream returned null");
                }
            }

            Console.WriteLine();
            Console.WriteLine("  *-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*  ");
            Console.WriteLine();
        }
        class MyContentTypeMapper : WebContentTypeMapper
        {
            public override WebContentFormat GetMessageFormatForContentType(string contentType)
            {
                if (contentType.StartsWith("application/held+xml", StringComparison.OrdinalIgnoreCase))
                {
                    return WebContentFormat.Xml;
                }
                else
                {
                    return WebContentFormat.Default;
                }
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            WebHttpBinding binding = new WebHttpBinding();
            binding.ContentTypeMapper = new MyContentTypeMapper();
            ServiceEndpoint endpoint = host.AddServiceEndpoint(typeof(ITest), binding, "");
            endpoint.Behaviors.Add(new WebHttpBehavior());
            host.Open();
            Console.WriteLine("Host opened");

            string request = @"<HELDRequest><LocationRequest>here</LocationRequest></HELDRequest>";
            SendRequest(baseAddress + "/held", "POST", "application/xml; charset=utf-8", request);
            SendRequest(baseAddress + "/held", "POST", "application/held+xml; charset=utf-8", request);

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_f2b07891_a347_4f1d_8526_8c7644522450
    {
        class InterestingFieldProperty
        {
            public const string Name = "InterestingFieldProperty";
            public string dataMemberName;
            public object value;
        }
        [DataContract]
        public class MyDC
        {
            [DataMember]
            public string str;
            [DataMember(Name = "int")]
            public int i;

            [OnSerializing]
            public void BeforeSerialization(StreamingContext ctx)
            {
                Console.WriteLine("[{0}] {1}.{2}", Thread.CurrentThread.ManagedThreadId, GetType().Name, MethodBase.GetCurrentMethod().Name);
                var dataMembers = from m in this.GetType().GetMembers()
                                  where Attribute.IsDefined(m, typeof(DataMemberAttribute))
                                  select new KeyValuePair<FieldInfo, DataMemberAttribute>(
                                      (FieldInfo)m, m.GetCustomAttributes(typeof(DataMemberAttribute), false)[0] as DataMemberAttribute);
                foreach (var dataMember in dataMembers)
                {
                    var value = dataMember.Key.GetValue(this);
                    var name = dataMember.Value.Name ?? dataMember.Key.Name;
                    Console.WriteLine("[{0}] {1}: {2}", Thread.CurrentThread.ManagedThreadId, name, value);
                    if (dataMember.Value.Name == "int")
                    {
                        InterestingFieldProperty prop = new InterestingFieldProperty
                        {
                            dataMemberName = dataMember.Value.Name,
                            value = value
                        };

                        OperationContext.Current.OutgoingMessageProperties.Add(InterestingFieldProperty.Name, prop);
                    }
                }
            }
        }

        [ServiceContract]
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
                return null;
            }

            public void BeforeSendReply(ref Message reply, object correlationState)
            {
                // at this point the serialization has yet to happen...
                MemoryStream ms = new MemoryStream();
                XmlDictionaryWriter writer = XmlDictionaryWriter.CreateBinaryWriter(ms);
                reply.WriteMessage(writer);
                writer.Flush();
                // now the serialization happened
                ms.Position = 0;
                XmlDictionaryReader reader = XmlDictionaryReader.CreateBinaryReader(ms, XmlDictionaryReaderQuotas.Max);
                Message newReply = Message.CreateMessage(reader, int.MaxValue, reply.Version);
                newReply.Properties.CopyProperties(reply.Properties);
                reply = newReply;

                InterestingFieldProperty prop = (InterestingFieldProperty)OperationContext.Current.OutgoingMessageProperties[InterestingFieldProperty.Name];

                Console.WriteLine("[{0}] BeforeSendReply, prop={1}: {2}", Thread.CurrentThread.ManagedThreadId, prop.dataMemberName, prop.value);
            }
        }

        static void CallService(object param)
        {
            string baseAddress = (string)param;
            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(new BasicHttpBinding(), new EndpointAddress(baseAddress));
            ITest proxy = factory.CreateChannel();

            int threadId = Thread.CurrentThread.ManagedThreadId;
            Console.WriteLine("Thread id: {0}", threadId);
            for (int i = 0; i < 10; i++)
            {
                proxy.GetDC("hello", threadId * 100 + i);
            }

            ((IClientChannel)proxy).Close();
            factory.Close();
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
            Console.WriteLine(proxy.GetDC("hello", 123));

            ((IClientChannel)proxy).Close();
            factory.Close();

            Thread[] threads = new Thread[5];
            for (int i = 0; i < threads.Length; i++)
            {
                threads[i] = new Thread(new ParameterizedThreadStart(CallService));
                threads[i].Start(baseAddress);
            }

            for (int i = 0; i < threads.Length; i++)
            {
                threads[i].Join();
            }

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_65005638_33a8_4564_a02c_d3681a65ce46
    {
        [DataContract]
        public class Rate
        {
            [DataMember]
            public double value;
            [DataMember]
            public DateTime expiration;
        }
        [DataContract]
        public class Product
        {
            [DataMember]
            public string name;
        }
        [ServiceContract]
        [ServiceKnownType(typeof(Rate))]
        [ServiceKnownType(typeof(Product))]
        public interface ITest
        {
            [OperationContract]
            object GetResponse(int i);
        }
        public class Service : ITest
        {
            public object GetResponse(int i)
            {
                if (i == 0)
                {
                    return new Rate { value = 4.75, expiration = DateTime.Today.AddDays(7) };
                }
                else
                {
                    return new Product { name = "Bread" };
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

            Console.WriteLine(proxy.GetResponse(0));
            Console.WriteLine(proxy.GetResponse(1));

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_ac135bef_6ddd_4ee6_978b_d3a6b6782218
    {
        [MessageContract]
        public class UploadImageRequest
        {
            [MessageHeader]
            public int imageId;
            [MessageBodyMember]
            public Stream imageBytes;
        }

        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            void UploadImage(UploadImageRequest request);
            [OperationContract]
            string Echo(string text);
        }
        public class Service : ITest
        {
            public string Echo(string text)
            {
                return text;
            }
            public void UploadImage(UploadImageRequest request)
            {
                int totalBytes = 0, b;
                while ((b = request.imageBytes.ReadByte()) >= 0)
                {
                    totalBytes++;
                }

                Console.WriteLine("Image {0} uploaded with {1} bytes", request.imageId, totalBytes);
            }
        }
        static Binding GetBinding()
        {
            BasicHttpBinding result = new BasicHttpBinding();
            result.TransferMode = TransferMode.Streamed;
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
            Console.WriteLine(proxy.Echo("Hello"));

            byte[] bytes = new byte[100];
            for (int i = 0; i < bytes.Length; i++) bytes[i] = (byte)i;
            UploadImageRequest request = new UploadImageRequest
            {
                imageId = 1,
                imageBytes = new MemoryStream(bytes),
            };
            proxy.UploadImage(request);

            Console.WriteLine("Sending a \"normal\" (non-chunked) request to the service");
            string requestBody = @"<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">
  <s:Body>
    <Echo xmlns=""http://tempuri.org/"">
      <text>world</text>
    </Echo>
  </s:Body>
</s:Envelope>";
            byte[] requestBodyBytes = Encoding.UTF8.GetBytes(requestBody);
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(baseAddress);
            req.Method = "POST";
            req.ContentType = "text/xml; charset=utf-8";
            req.Headers["SOAPAction"] = "http://tempuri.org/ITest/Echo";
            Stream reqStream = req.GetRequestStream();
            reqStream.Write(requestBodyBytes, 0, requestBodyBytes.Length);
            reqStream.Close();
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Console.WriteLine("HTTP/{0} {1} {2}", resp.ProtocolVersion, (int)resp.StatusCode, resp.StatusDescription);
            foreach (var header in resp.Headers.AllKeys)
            {
                Console.WriteLine("{0}: {1}", header, resp.Headers[header]);
            }

            Stream respStream = resp.GetResponseStream();
            if (respStream != null)
            {
                Console.WriteLine(new StreamReader(resp.GetResponseStream()).ReadToEnd());
            }

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class ReplaceEncodingInContractBehavior
    {
        [ServiceContract]
        [MyReplaceEncoderContractBehavior]
        public interface ITest
        {
            [OperationContract]
            string Echo(string text);
        }
        public class Service : ITest
        {
            public string Echo(string text)
            {
                HttpRequestMessageProperty reqProp = OperationContext.Current.IncomingMessageProperties[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty;
                Console.WriteLine(reqProp.Headers[HttpRequestHeader.ContentType]);
                return text;
            }
        }
        static Binding GetBinding()
        {
            CustomBinding result = new CustomBinding(new HttpTransportBindingElement());
            return result;
        }
        public class MyMessageEncodingBindingElement : MessageEncodingBindingElement
        {
            MessageEncodingBindingElement inner;
            public MyMessageEncodingBindingElement(MessageEncodingBindingElement inner)
            {
                this.inner = inner;
            }

            private MyMessageEncodingBindingElement(MyMessageEncodingBindingElement other)
            {
                this.inner = other.inner;
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
                return new MyMessageEncodingBindingElement(this);
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

            public override bool CanBuildChannelFactory<TChannel>(BindingContext context)
            {
                return context.CanBuildInnerChannelFactory<TChannel>();
            }

            public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
            {
                context.BindingParameters.Add(this);
                return context.BuildInnerChannelFactory<TChannel>();
            }

            public static CustomBinding ReplaceEncodingBindingElement(Binding originalBinding)
            {
                CustomBinding custom = originalBinding as CustomBinding;
                if (custom == null)
                {
                    custom = new CustomBinding(originalBinding);
                }

                for (int i = 0; i < custom.Elements.Count; i++)
                {
                    if (custom.Elements[i] is MessageEncodingBindingElement)
                    {
                        MyMessageEncodingBindingElement element = new MyMessageEncodingBindingElement((MessageEncodingBindingElement)custom.Elements[i]);
                        custom.Elements[i] = element;
                    }
                }

                return custom;
            }

            class MyMessageEncoderFactory : MessageEncoderFactory
            {
                private MessageEncoderFactory messageEncoderFactory;

                public MyMessageEncoderFactory(MessageEncoderFactory messageEncoderFactory)
                {
                    this.messageEncoderFactory = messageEncoderFactory;
                }

                public override MessageEncoder Encoder
                {
                    get { return new MyMessageEncoder(this.messageEncoderFactory.Encoder); }
                }

                public override MessageVersion MessageVersion
                {
                    get { return this.messageEncoderFactory.MessageVersion; }
                }

                public override MessageEncoder CreateSessionEncoder()
                {
                    return new MyMessageEncoder(this.messageEncoderFactory.CreateSessionEncoder());
                }
            }

            class MyMessageEncoder : MessageEncoder
            {
                private MessageEncoder messageEncoder;

                public MyMessageEncoder(MessageEncoder messageEncoder)
                {
                    this.messageEncoder = messageEncoder;
                }

                public override string ContentType
                {
                    get { return this.messageEncoder.ContentType; }
                }

                public override string MediaType
                {
                    get { return this.messageEncoder.MediaType; }
                }

                public override MessageVersion MessageVersion
                {
                    get { return this.messageEncoder.MessageVersion; }
                }

                public override bool IsContentTypeSupported(string contentType)
                {
                    return this.messageEncoder.IsContentTypeSupported(contentType);
                }

                public override Message ReadMessage(ArraySegment<byte> buffer, BufferManager bufferManager, string contentType)
                {
                    Console.WriteLine("Message read: {0}", Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count));
                    return this.messageEncoder.ReadMessage(buffer, bufferManager, contentType);
                }

                public override Message ReadMessage(Stream stream, int maxSizeOfHeaders, string contentType)
                {
                    throw new NotSupportedException("Streamed not supported");
                }

                public override ArraySegment<byte> WriteMessage(Message message, int maxMessageSize, BufferManager bufferManager, int messageOffset)
                {
                    return this.messageEncoder.WriteMessage(message, maxMessageSize, bufferManager, messageOffset);
                }

                public override void WriteMessage(Message message, Stream stream)
                {
                    throw new NotSupportedException("Streamed not supported");
                }
            }
        }
        public class MyReplaceEncoderContractBehaviorAttribute : Attribute, IContractBehavior
        {
            public void AddBindingParameters(ContractDescription contractDescription, ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
            {
                bindingParameters.Add(new BinaryMessageEncodingBindingElement());
                //for (int i = 0; i < bindingParameters.Count; i++)
                //{
                //    if (bindingParameters[i] is MessageEncodingBindingElement)
                //    {
                //        bindingParameters[i] = new MyMessageEncodingBindingElement((MessageEncodingBindingElement)bindingParameters[i]);
                //    }
                //}
            }

            public void ApplyClientBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, ClientRuntime clientRuntime)
            {
            }

            public void ApplyDispatchBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, DispatchRuntime dispatchRuntime)
            {
            }

            public void Validate(ContractDescription contractDescription, ServiceEndpoint endpoint)
            {
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            ServiceEndpoint endpoint = host.AddServiceEndpoint(typeof(ITest), GetBinding(), "");
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(GetBinding(), new EndpointAddress(baseAddress));
            ITest proxy = factory.CreateChannel();
            Console.WriteLine(proxy.Echo("Hello"));

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_0e653d10_4294_4113_98cd_44d0e40d36d0
    {
        [DataContract]
        public class Employee
        {
            [DataMember]
            public string Name;
            [DataMember]
            public DateTime DOB;
            [DataMember]
            public string ID;

            private AddressDetails _addressDetails;
            [DataMember]
            public AddressDetails addressDetails
            {
                get { return _addressDetails; }
                set { _addressDetails = value; }
            }

            public Employee()
            {
                this.Initialize();
            }

            private void Initialize()
            {
                this._addressDetails = new AddressDetails();
                Console.WriteLine("Employee.Initialize called, _addressDetails = {0}", _addressDetails);
            }

            [OnDeserializing]
            private void DeserializationInitializer(StreamingContext ctx)
            {
                this.Initialize();
            }
        }

        [DataContract]
        public class Address
        {
            [DataMember]
            public string Unit;

            [DataMember]
            public string StreetNo;

            [DataMember]
            public string StreetName;

            [DataMember]
            public string StreetType;

            [DataMember]
            public string PostCode;

            [DataMember]
            public string State;
        }

        [DataContract]
        public class AddressDetails
        {
            private Address _postalAddress;
            [DataMember]
            public Address postalAddress
            {
                get { return _postalAddress; }
                set { _postalAddress = value; }
            }

            private Address _homeAddress;
            [DataMember]
            public Address homeAddress
            {
                get { return _homeAddress; }
                set { _homeAddress = value; }
            }

            public AddressDetails()
            {
                this.Initialize();
            }

            private void Initialize()
            {
                this._postalAddress = new Address();
                this._homeAddress = new Address();
                Console.WriteLine("AddressDetails.Initialize called, _homeAddress = {0}", _homeAddress);
            }

            [OnDeserializing]
            private void DeserializationInitializer(StreamingContext ctx)
            {
                this.Initialize();
            }
        }

        public static void Test()
        {
            Employee emp = new Employee();
            emp.addressDetails.homeAddress.StreetNo = "demo st";
            Console.WriteLine(emp);
        }
    }

    public class Post_3b996393_ca12_45ad_a2c2_17936a288b0a
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            string Echo(string text);
        }
        [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
        public class Service : ITest
        {
            public string Echo(string text)
            {
                return text;
            }
        }
        static Binding GetBinding()
        {
            SecurityBindingElement userPwdOverTransport = SecurityBindingElement.CreateUserNameOverTransportBindingElement();
            userPwdOverTransport.AllowInsecureTransport = true;
            TextMessageEncodingBindingElement textBE = new TextMessageEncodingBindingElement(MessageVersion.Soap11, Encoding.UTF8);
            HttpTransportBindingElement httpBE = new HttpTransportBindingElement();
            CustomBinding result = new CustomBinding(userPwdOverTransport, textBE, httpBE);
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
            factory.Credentials.UserName.UserName = "me";
            factory.Credentials.UserName.Password = "pwd";
            ITest proxy = factory.CreateChannel();
            Console.WriteLine(proxy.Echo("Hello"));

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class TestIStreamProviderInType
    {
        [XmlRoot(Namespace = "")]
        public class MyType : IXmlSerializable
        {
            [XmlElement]
            public string Str;
            [XmlElement]
            public IStreamProvider StreamProvider;

            public MyType() { }

            public System.Xml.Schema.XmlSchema GetSchema()
            {
                throw new NotSupportedException("This is never called on the client");
            }

            public void ReadXml(XmlReader reader)
            {
                Console.WriteLine("{0} - {1} - {2}", reader.NodeType, reader.Name, reader.Value);
                reader.ReadStartElement("Str");
                Console.WriteLine("{0} - {1} - {2}", reader.NodeType, reader.Name, reader.Value);
            }

            public void WriteXml(XmlWriter writer)
            {
                XmlDictionaryWriter dicWriter = writer as XmlDictionaryWriter;
                if (dicWriter == null)
                {
                    dicWriter = XmlDictionaryWriter.CreateDictionaryWriter(writer);
                }

                dicWriter.WriteElementString("Str", this.Str);
                dicWriter.WriteStartElement("StreamProvider");
                dicWriter.WriteValue(this.StreamProvider);
                dicWriter.WriteEndElement();
            }
        }

        class MyStreamProvider : IStreamProvider
        {
            Stream stream;
            public MyStreamProvider(Stream stream)
            {
                this.stream = stream;
            }
            public Stream GetStream()
            {
                return this.stream;
            }

            public void ReleaseStream(Stream stream)
            {
            }
        }
        public static void Test()
        {
            byte[] bytes = new byte[1000];
            for (int i = 0; i < bytes.Length; i++) bytes[i] = (byte)'r';
            MemoryStream bytesStream = new MemoryStream(bytes);
            XmlSerializer xs = new XmlSerializer(typeof(MyType));
            MemoryStream ms = new MemoryStream();
            MyType instance = new MyType { Str = "hello", StreamProvider = new MyStreamProvider(bytesStream) };
            XmlDictionaryWriter w = XmlDictionaryWriter.CreateMtomWriter(ms, Encoding.UTF8, int.MaxValue, "text/xml");
            xs.Serialize(w, instance);
            w.Flush();
            Console.WriteLine(Encoding.UTF8.GetString(ms.ToArray()));
        }
    }

    public class MyCode_6
    {
        [DataContract]
        [KnownType(typeof(Derived1))]
        [KnownType(typeof(Derived2))]
        public class BaseClass
        {
        }

        public class Derived1 : BaseClass { }
        public class Derived2 : BaseClass { }
        public class Derived3 : BaseClass { }

        class MyResolver : DataContractResolver
        {
            public override Type ResolveName(string typeName, string typeNamespace, Type declaredType, DataContractResolver knownTypeResolver)
            {
                throw new NotImplementedException();
            }

            public override bool TryResolveType(Type type, Type declaredType, DataContractResolver knownTypeResolver, out XmlDictionaryString typeName, out XmlDictionaryString typeNamespace)
            {
                throw new NotImplementedException();
            }
        }

        public static void Test()
        {
            DataContractResolver dcr = new MyResolver();
        }
    }

    public class MyCode_7
    {
        public class Constants
        {
            public const string CheckpointRequestElementName = "CheckpointRequest";
            public const string CheckpointResponseElementName = "CheckpointResponse";
            public const string ServiceXmlNamespace = "http://service.xml.namespace";
            public const string ServiceContractName = "ITest";
        }
        [MessageContract(IsWrapped = true, WrapperName = Constants.CheckpointResponseElementName, WrapperNamespace = Constants.ServiceXmlNamespace)]
        public class CheckpointResponse
        {
            [MessageBodyMember]
            public string str2 = "Hello world";
        }
        [MessageContract(IsWrapped = true, WrapperName = Constants.CheckpointRequestElementName, WrapperNamespace = Constants.ServiceXmlNamespace)]
        public class CheckpointRequest
        {
            [MessageBodyMember]
            public string str1 = "Hello world";
        }
        [ServiceContract(Name = Constants.ServiceContractName, Namespace = Constants.ServiceXmlNamespace)]
        public interface ITest
        {
            [OperationContract(AsyncPattern = true)]
            IAsyncResult BeginCheckpoint(CheckpointRequest request, AsyncCallback callback, object asyncState);
            CheckpointResponse EndCheckpoint(IAsyncResult asyncResult);
        }
        [ServiceContract(Name = Constants.ServiceContractName, Namespace = Constants.ServiceXmlNamespace)]
        public interface ITestClient
        {
            [OperationContract]
            CheckpointResponse Checkpoint(CheckpointRequest request);
        }
        public class Service : ITest
        {
            public IAsyncResult BeginCheckpoint(CheckpointRequest request, AsyncCallback callback, object asyncState)
            {
                Func<string, string> checkpointDoWork = x => x;
                return checkpointDoWork.BeginInvoke(request.str1, callback, asyncState);
            }

            public CheckpointResponse EndCheckpoint(IAsyncResult asyncResult)
            {
                Func<string, string> func = ((System.Runtime.Remoting.Messaging.AsyncResult)asyncResult).AsyncDelegate as Func<string, string>;
                return new CheckpointResponse { str2 = func.EndInvoke(asyncResult) };
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), new BasicHttpBinding(), "");
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITestClient> factory = new ChannelFactory<ITestClient>(new BasicHttpBinding(), new EndpointAddress(baseAddress));
            ITestClient proxy = factory.CreateChannel();

            CheckpointRequest req = new CheckpointRequest { str1 = "hello world" };
            var resp = proxy.Checkpoint(req);
            Console.WriteLine("Response: {0}", resp.str2);

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.WriteLine("Now using the async version on the client");
            AutoResetEvent evt = new AutoResetEvent(false);

            ChannelFactory<ITest> asyncFactory = new ChannelFactory<ITest>(new BasicHttpBinding(), new EndpointAddress(baseAddress));
            ITest asyncProxy = asyncFactory.CreateChannel();
            asyncProxy.BeginCheckpoint(req, delegate(IAsyncResult asyncResult)
            {
                var asyncResp = asyncProxy.EndCheckpoint(asyncResult);
                Console.WriteLine("Response (from async client): {0}", asyncResp.str2);
                evt.Set();
            }, null);
            evt.WaitOne();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_26935056_c4dc_4764_b1d3_38893ea53026
    {
        [ServiceContract]
        public interface ITest
        {
            [WebGet(ResponseFormat = WebMessageFormat.Json)]
            int Get();
        }
        public class Service : ITest
        {
            public int Get()
            {
                WebOperationContext.Current.OutgoingResponse.Headers.Add(HttpResponseHeader.SetCookie, "cookie1");
                WebOperationContext.Current.OutgoingResponse.Headers.Add(HttpResponseHeader.SetCookie, "cookie2");
                return 1;
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            WebServiceHost host = new WebServiceHost(typeof(Service), new Uri(baseAddress));
            host.Open();
            Console.WriteLine("Host opened");

            WebClient c = new WebClient();
            string result = c.DownloadString(baseAddress + "/Get");
            Console.WriteLine("Response");
            foreach (var headerName in c.ResponseHeaders.AllKeys)
            {
                Console.WriteLine("{0}: {1}", headerName, c.ResponseHeaders[headerName]);
            }

            Console.WriteLine(result);

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_a872b5f5_c83a_4894_8ad3_3928837eab14
    {
        [DataContract]
        public class MyApplicationSettings
        {
            [DataMember]
            public int UserID { get; set; }
        }
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
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(GetBinding(), new EndpointAddress(baseAddress));
            ITest proxy = factory.CreateChannel();
            Console.WriteLine(proxy.Echo("Hello"));

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class SL_Post_227776
    {
        public static void Test()
        {
            Bitmap bitmap = new Bitmap(20, 20);
            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    bitmap.SetPixel(i, j, (Math.Abs(i - j) < 2) ? Color.Blue : Color.Yellow);
                }
            }
            MemoryStream ms = new MemoryStream();
            DataContractSerializer dcs = new DataContractSerializer(typeof(Bitmap));
            XmlWriter w = XmlWriter.Create(ms, new XmlWriterSettings { Indent = true, OmitXmlDeclaration = true, IndentChars = "  " });
            dcs.WriteObject(w, bitmap);
            w.Flush();
            Console.WriteLine(Encoding.UTF8.GetString(ms.ToArray()));
        }
    }

    public class Post_f6427047_e770_47d7_ad83_8b1a29dafb15
    {
        [ServiceContract]
        public class Service
        {
            [Description("Submits the user entered data, and returns the stream")]
            [WebInvoke(Method = "POST", UriTemplate = "/SubmitData/{fileName}")]
            public Stream Submit(string fileName, Stream contents)
            {
                string input = new StreamReader(contents).ReadToEnd();
                Console.WriteLine("In service, input = {0}", input);

                string response = "This is the response";
                WebOperationContext.Current.OutgoingResponse.ContentType = "text/plain";
                return new MemoryStream(Encoding.UTF8.GetBytes(response));
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            WebServiceHost host = new WebServiceHost(typeof(Service), new Uri(baseAddress));
            host.Open();
            Console.WriteLine("Host opened");

            Util.SendRequest(baseAddress + "/SubmitData/Test.pdf", "POST", "text/plain", "This is the request data");

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class StackOverflow_5979252
    {
        [ServiceContract(Name = "IMessageCallback")]
        public interface IAsyncMessageCallback
        {
            [OperationContract(AsyncPattern = true)]
            IAsyncResult BeginOnMessageAdded(string msg, DateTime timestamp, AsyncCallback callback, object asyncState);
            void EndOnMessageAdded(IAsyncResult result);
        }
        [ServiceContract(CallbackContract = typeof(IAsyncMessageCallback))]
        public interface IMessage
        {
            [OperationContract]
            void AddMessage(string message);
        }
        [ServiceBehavior(IncludeExceptionDetailInFaults = true, ConcurrencyMode = ConcurrencyMode.Multiple)]
        public class Service : IMessage
        {
            public void AddMessage(string message)
            {
                IAsyncMessageCallback callback = OperationContext.Current.GetCallbackChannel<IAsyncMessageCallback>();
                callback.BeginOnMessageAdded(message, DateTime.Now, delegate(IAsyncResult ar)
                {
                    callback.EndOnMessageAdded(ar);
                }, null);
            }
        }
        [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
        class MyClientCallback : IAsyncMessageCallback
        {
            public IAsyncResult BeginOnMessageAdded(string msg, DateTime timestamp, AsyncCallback callback, object asyncState)
            {
                Action<string, DateTime> act = (txt, time) => { Console.WriteLine("[{0}] {1}", time, txt); };
                return act.BeginInvoke(msg, timestamp, callback, asyncState);
            }

            public void EndOnMessageAdded(IAsyncResult result)
            {
                Action<string, DateTime> act = (Action<string, DateTime>)((System.Runtime.Remoting.Messaging.AsyncResult)result).AsyncDelegate;
                act.EndInvoke(result);
            }
        }
        static Binding GetBinding()
        {
            return new NetTcpBinding(SecurityMode.None);
        }
        public static void Test()
        {
            string baseAddress = "net.tcp://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(IMessage), GetBinding(), "");
            host.Open();
            Console.WriteLine("Host opened");

            for (int i = 0; i < 10; i++)
            {
                ThreadPool.QueueUserWorkItem(delegate
                {
                    InstanceContext instanceContext = new InstanceContext(new MyClientCallback());
                    DuplexChannelFactory<IMessage> factory = new DuplexChannelFactory<IMessage>(instanceContext, GetBinding(), new EndpointAddress(baseAddress));
                    IMessage proxy = factory.CreateChannel();
                    proxy.AddMessage("Hello world");
                    Console.WriteLine("AddMessage callled");
                });
            }

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            //((IClientChannel)proxy).Close();
            //factory.Close();
            host.Close();
        }
    }

    // http://stackoverflow.com/q/5937029/751090
    public class StackOverflow_5937029
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
                return x + y;
            }
        }
        static void SendRequest(string address)
        {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(address);
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
            Console.WriteLine("  *-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*  ");
            Console.WriteLine();
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            WebHttpBehavior behavior = new WebHttpBehavior
            {
                HelpEnabled = true
            };
            WebHttpBinding binding = new WebHttpBinding
            {
                TransferMode = TransferMode.Streamed
            };
            host.AddServiceEndpoint(typeof(ITest), binding, "").Behaviors.Add(behavior);
            host.Open();
            Console.WriteLine("Host opened");

            SendRequest(baseAddress + "/Add?x=4&y=8");
            SendRequest(baseAddress + "/help");

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_e668f3a1_2eab_4e50_b8cd_138663a0bfb1
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
                RemoteEndpointMessageProperty remp = (RemoteEndpointMessageProperty)OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name];
                Console.WriteLine("Request from {0}:{1}", remp.Address, remp.Port);
                return text;
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
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(GetBinding(), new EndpointAddress(baseAddress));
            ITest proxy = factory.CreateChannel();
            Console.WriteLine(proxy.Echo("Hello"));

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    // http://stackoverflow.com/q/5984689/751090
    public class StackOverflow_5984689
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
            host.AddServiceEndpoint(typeof(ITest), new BasicHttpBinding(), "/GoInto/Svc");
            host.Open();
            Console.WriteLine("Host opened");

            foreach (ServiceEndpoint endpoint in host.Description.Endpoints)
            {
                Console.WriteLine("Endpoint at {0}", endpoint.Address.Uri);
            }

            foreach (ChannelDispatcher cd in host.ChannelDispatchers)
            {
                foreach (EndpointDispatcher ed in cd.Endpoints)
                {
                    Console.WriteLine("Dispatcher at {0}", ed.EndpointAddress.Uri);
                }
            }

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(new BasicHttpBinding(), new EndpointAddress(baseAddress + "/GoInto/Svc"));
            ITest proxy = factory.CreateChannel();
            Console.WriteLine(proxy.Echo("Hello"));

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    // http://stackoverflow.com/q/6049454/751090
    public class StackOverflow_6049454_751090
    {
        [ServiceContract]
        public interface IService1
        {
            [OperationContract]
            [WebInvoke(Method = "GET",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json,
                BodyStyle = WebMessageBodyStyle.Wrapped,
                UriTemplate = "NullTestPost")]
            [return: MessageParameter(Name = "NullTestType")]
            NullTestType GettMethod();

            [OperationContract]
            [WebInvoke(Method = "POST",
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json,
                BodyStyle = WebMessageBodyStyle.Wrapped,
                UriTemplate = "NullTestPost22")]
            [return: MessageParameter(Name = "NullTestType")]
            NullTestType PostMethod(NullTestType NullTestTypeObject);
        }

        [DataContract]
        public class NullTestType
        {
            [DataMember]
            public string NullTestString { get; set; }
            [DataMember]
            public int NullTestInt { get; set; }
        }

        public class Service1 : IService1
        {
            public NullTestType PostMethod(NullTestType NullTestTypeObject)
            {
                return NullTestTypeObject;
            }

            public NullTestType GettMethod()
            {
                return new NullTestType { NullTestString = "Returned String", NullTestInt = 25 };
            }
        }

        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            WebServiceHost host = new WebServiceHost(typeof(Service1), new Uri(baseAddress));
            host.Open();

            WebChannelFactory<IService1> factory = new WebChannelFactory<IService1>(new Uri(baseAddress));
            IService1 proxy = factory.CreateChannel();
            Console.WriteLine(proxy.PostMethod(new NullTestType { NullTestInt = 123, NullTestString = "hello" }));
        }
    }

    // http://stackoverflow.com/q/6077549/751090
    public class StackOverflow_6077549_751090
    {
        internal static class KnownTypesProvider
        {
            public static IEnumerable<Type> GetKnownTypes(ICustomAttributeProvider provider)
            {
                // collect and pass back the list of known types    
                List<Type> types = new List<Type>();
                types.Add(typeof(EmployeeDTO));
                return types;
            }
        }

        [DataContract(Name = "response")]
        public class Response
        {
            [DataMember(Order = 1)]
            public int totalResultCount { get; set; }
            [DataMember(Order = 2)]
            public IEnumerable<EmployeeDTO> results { get; set; }
        }

        [DataContract(Name = "result")]
        public class EmployeeDTO
        {
            [DataMember]
            public string EmployeeCode { get; set; }
            [DataMember]
            public string EmployeeName { get; set; }
        }

        public static void Test()
        {
            Response response = new Response
            {
                totalResultCount = 10,
                results = new List<EmployeeDTO>
                {
                    new EmployeeDTO { EmployeeName = "John", EmployeeCode = "123" },
                    new EmployeeDTO { EmployeeName = "Jane", EmployeeCode = "321" },
                }
            };

            DataContractSerializer dcs = new DataContractSerializer(typeof(Response));
            MemoryStream ms = new MemoryStream();
            XmlWriter w = XmlWriter.Create(ms, new XmlWriterSettings { OmitXmlDeclaration = true, IndentChars = "  ", Indent = true });
            dcs.WriteObject(w, response);
            w.Flush();
            Console.WriteLine(Encoding.UTF8.GetString(ms.ToArray()));
        }
    }

    // http://stackoverflow.com/q/6100587/751090
    public class StackOverflow_6100587_751090
    {
        public class MyType
        {
            public MyTypeWithDates d1;
            public MyTypeWithDates d2;
        }
        public class MyTypeWithDates
        {
            public DateTime Start;
            public DateTime End;
        }
        public class MySurrogate : IDataContractSurrogate
        {
            public object GetCustomDataToExport(Type clrType, Type dataContractType)
            {
                throw new NotImplementedException();
            }

            public object GetCustomDataToExport(MemberInfo memberInfo, Type dataContractType)
            {
                throw new NotImplementedException();
            }

            public Type GetDataContractType(Type type)
            {
                return type;
            }

            public object GetDeserializedObject(object obj, Type targetType)
            {
                return obj;
            }

            public void GetKnownCustomDataTypes(Collection<Type> customDataTypes)
            {
            }

            public object GetObjectToSerialize(object obj, Type targetType)
            {
                return ReplaceLocalDateWithUTC(obj);
            }

            public Type GetReferencedTypeOnImport(string typeName, string typeNamespace, object customData)
            {
                throw new NotImplementedException();
            }

            public CodeTypeDeclaration ProcessImportedType(CodeTypeDeclaration typeDeclaration, CodeCompileUnit compileUnit)
            {
                throw new NotImplementedException();
            }

            private object ReplaceLocalDateWithUTC(object obj)
            {
                if (obj == null) return null;
                Type objType = obj.GetType();
                foreach (var field in objType.GetFields())
                {
                    if (field.FieldType == typeof(DateTime))
                    {
                        DateTime fieldValue = (DateTime)field.GetValue(obj);
                        if (fieldValue.Kind != DateTimeKind.Utc)
                        {
                            field.SetValue(obj, fieldValue.ToUniversalTime());
                        }
                    }
                }

                return obj;
            }
        }
        public static void Test()
        {
            MemoryStream ms = new MemoryStream();
            DataContractJsonSerializer dcjs = new DataContractJsonSerializer(typeof(MyType), null, int.MaxValue, true, new MySurrogate(), false);
            MyType t = new MyType
            {
                d1 = new MyTypeWithDates { Start = DateTime.Now, End = DateTime.Now.AddMinutes(1) },
                d2 = new MyTypeWithDates { Start = DateTime.Now.AddHours(1), End = DateTime.Now.AddHours(2) },
            };
            dcjs.WriteObject(ms, t);
            Console.WriteLine(Encoding.UTF8.GetString(ms.ToArray()));
        }
    }

    public class Post_b5efb12c_3920_41f7_a478_349fd4607875
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            [WebGet(UriTemplate = "sites?limit={limitStr}")]
            string DoSomething(string limitStr);
        }
        public class Service : ITest
        {
            public string DoSomething(string limitStr)
            {
                int limit;
                if (!int.TryParse(limitStr, out limit))
                {
                    throw new WebFaultException<string>("limit parameter must be an integer", HttpStatusCode.BadRequest);
                }

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
            Console.WriteLine("  *-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*  ");
            Console.WriteLine();
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            WebServiceHost host = new WebServiceHost(typeof(Service), new Uri(baseAddress));
            host.Open();
            Console.WriteLine("Host opened");

            SendGet(baseAddress + "/sites?limit=123");
            SendGet(baseAddress + "/sites?limit=abc");

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    // http://stackoverflow.com/q/5189844/751090
    public class StackOverflow_5189844_751090
    {
        [ServiceContract]
        public interface IService1
        {
            [OperationContract]
            TestObject Load();
        }
        [DataContract]
        [KnownType(typeof(Collection<object>))]
        public class TestObject
        {
            [DataMember]
            public Collection<object> Properties = new Collection<object>();
        }
        public class Service1 : IService1
        {
            public TestObject Load()
            {
                var obj = new TestObject();

                // bad line
                obj.Properties.Add(new Collection<object>());

                return obj;
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
            ServiceHost host = new ServiceHost(typeof(Service1), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(IService1), GetBinding(), "");
            host.Open();
            Console.WriteLine("Host opened");

            var factory = new ChannelFactory<IService1>(GetBinding(), new EndpointAddress(baseAddress));
            var proxy = factory.CreateChannel();
            Console.WriteLine(proxy.Load());

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_2341c11e_92b3_4da4_aba5_858054f46c80
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract, WebGet]
            int Sum(int[] ids);
            [OperationContract, WebGet(UriTemplate = "/invoice/{ids}")]
            int Invoices(string ids);
            [OperationContract, WebGet]
            string GetValue(Nullable<int> ni, Nullable<double> nd);
        }
        public class Service : ITest
        {
            public int Sum(int[] ids)
            {
                if (ids == null) return 0;
                int result = 0;
                foreach (int id in ids) result += id;
                return result;
            }
            public int Invoices(string ids)
            {
                string[] parts = ids.Split(',');
                int result = 0;
                foreach (string part in parts) result += int.Parse(part);
                return result;
            }
            public string GetValue(Nullable<int> ni, Nullable<double> nd)
            {
                return string.Format("ni={0},nd={1}",
                    ni == null ? "<null>" : ni.Value.ToString(),
                    nd == null ? "<null>" : nd.Value.ToString());
            }
        }
        public class MyBehavior : WebHttpBehavior
        {
            protected override QueryStringConverter GetQueryStringConverter(OperationDescription operationDescription)
            {
                return new MyConverter(base.GetQueryStringConverter(operationDescription));
            }
        }
        public class MyConverter : QueryStringConverter
        {
            QueryStringConverter inner;
            public MyConverter(QueryStringConverter inner)
            {
                this.inner = inner;
            }
            public override bool CanConvert(Type type)
            {
                return type == typeof(int[]) || type == typeof(int?) || type == typeof(double?) || inner.CanConvert(type);
            }
            public override object ConvertStringToValue(string parameter, Type parameterType)
            {
                if (parameterType == typeof(int[]))
                {
                    string[] parts = parameter.Split(',');
                    int[] result = new int[parts.Length];
                    for (int i = 0; i < parts.Length; i++) result[i] = int.Parse(parts[i]);
                    return result;
                }
                else if (parameterType == typeof(int?))
                {
                    int temp;
                    if (int.TryParse(parameter, out temp))
                    {
                        return temp;
                    }
                    else
                    {
                        return null;
                    }
                }
                else if (parameterType == typeof(double?))
                {
                    double temp;
                    if (double.TryParse(parameter, out temp))
                    {
                        return temp;
                    }
                    else
                    {
                        return null;
                    }
                }

                return inner.ConvertStringToValue(parameter, parameterType);
            }
            // The override below is not needed if used only at the server
            public override string ConvertValueToString(object parameter, Type parameterType)
            {
                if (parameterType == typeof(int[]))
                {
                    int[] intArray = (int[])parameter;
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < intArray.Length; i++)
                    {
                        if (i > 0) sb.Append(',');
                        sb.Append(intArray[i]);
                    }

                    return sb.ToString();
                }

                return inner.ConvertValueToString(parameter, parameterType);
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), new WebHttpBinding(), "").Behaviors.Add(new MyBehavior());
            host.Open();
            Console.WriteLine("Host opened");

            Util.SendRequest(baseAddress + "/Sum?ids=123,345,121", "GET", null, null);
            Util.SendRequest(baseAddress + "/invoice/123,345,121", "GET", null, null);
            Util.SendRequest(baseAddress + "/GetValue?ni=null&nd=123.45", "GET", null, null);
            Util.SendRequest(baseAddress + "/GetValue?ni=222&nd=123.45", "GET", null, null);

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    // http://stackoverflow.com/q/6157636/751090
    public class StackOverflow_6157636_751090
    {
        public static void Test()
        {
            string xml = @"  <SOAP-ENV:Envelope xmlns:SOAP-ENV=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xs=""http://www.w3.org/2001/XMLSchema""> 
 <SOAP-ENV:Body>
 <ZbcInsertRequest xmlns:ns1=""urn:sap-com:document:sap:soap:functions:mc-style"">
  <Destino>qas</Destino> 
 <IClrQst>
  <Request>foo</Request> 
  <Proyecto>foofoo</Proyecto> 
  <Modulocq /> 
  <Titureq>foofoofoo</Titureq> 
  <Tiporeq /> 
  <Prioridad>0</Prioridad> 
  <Clasific /> 
  <Asignadoa>foofoofoofoo</Asignadoa> 
  <Destino>qas</Destino> 
  <Solicita>foo</Solicita> 
  <Autoriza>c.foo</Autoriza> 
  <ModTransp /> 
  <ReqSox>F</ReqSox> 
  <Notfylst /> 
  <Tipodeploy>Normal</Tipodeploy> 
  <Paqdeploy>CDVQA00000000</Paqdeploy> 
  </IClrQst>
 <TRequest>
 <item>
  <Trkorr>DESK9A0ZJT</Trkorr> 
  <Secuencia>0</Secuencia> 
  <Solicita>c.foo</Solicita> 
  <Type /> 
  <Id /> 
  <Numero /> 
  <Message /> 
  </item>
  </TRequest>
  </ZbcInsertRequest>
  </SOAP-ENV:Body>
</SOAP-ENV:Envelope>";
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            MemoryStream ms = new MemoryStream();
            Encoding encoding = new UTF8Encoding(false);
            XmlWriter w = XmlWriter.Create(ms, new XmlWriterSettings { Indent = true, OmitXmlDeclaration = false, Encoding = encoding, IndentChars = "  " });
            doc.Save(w);
            w.Flush();
            Console.WriteLine(encoding.GetString(ms.ToArray()));
        }
    }

    // http://stackoverflow.com/q/6158263/751090
    public class StackOverflow_6158263_751090
    {
        public int MyLongOperation(int x, int y)
        {
            Thread.Sleep(10000);
            return x + y;
        }
        public void CallLongOperation()
        {
            Func<int, int, int> func = MyLongOperation;
            func.BeginInvoke(5, 7, MyCallback, "Expected result: " + 12);
            Console.WriteLine("Called BeginInvoke");
            func.BeginInvoke(11, 22, MyCallback, "Expected result: " + 33);
            Console.WriteLine("Press ENTER to continue");
            Console.ReadLine();
        }
        void MyCallback(IAsyncResult asyncResult)
        {
            Func<int, int, int> func = (Func<int, int, int>)((System.Runtime.Remoting.Messaging.AsyncResult)asyncResult).AsyncDelegate;
            string expectedResult = (string)asyncResult.AsyncState;
            int result = func.EndInvoke(asyncResult);
            Console.WriteLine("Result: {0} - {1}", result, expectedResult);
        }
        public static void Test()
        {
            new StackOverflow_6158263_751090().CallLongOperation();
        }
    }

    // http://stackoverflow.com/q/6216858/751090
    public class StackOverflow_6216858_751090
    {
        public class MyClass { }
        [ServiceContract]
        public interface ITest<T> where T : MyClass
        {
            [OperationContract]
            string Echo(string text);
        }
        public class Service : ITest<MyClass>
        {
            public string Echo(string text)
            {
                return text;
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
            host.AddServiceEndpoint(typeof(ITest<MyClass>), GetBinding(), "");
            host.Description.Behaviors.Add(new ServiceMetadataBehavior { HttpGetEnabled = true });
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest<MyClass>> factory = new ChannelFactory<ITest<MyClass>>(GetBinding(), new EndpointAddress(baseAddress));
            ITest<MyClass> proxy = factory.CreateChannel();
            Console.WriteLine(proxy.Echo("Hello"));

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    // http://stackoverflow.com/q/6216605_751090
    public class StackOverflow_6216605_751090
    {
        [ServiceContract(CallbackContract = typeof(ITestCallback))]
        public interface ITest
        {
            [OperationContract]
            int Add(int x, int y);
            [OperationContract]
            void CallMe(int numberOfTimes);
        }
        [ServiceContract]
        public interface ITestCallback
        {
            [OperationContract]
            string Hello(string name);
        }
        [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
        public class Service : ITest
        {
            public int Add(int x, int y)
            {
                Console.WriteLine("In a Request/Reply operation on server: {0} + {1}", x, y);
                return x + y;
            }

            public void CallMe(int numberOfTimes)
            {
                Console.WriteLine("In another request/reply operation on server, which will call the client.");
                ITestCallback callback = OperationContext.Current.GetCallbackChannel<ITestCallback>();
                ThreadPool.QueueUserWorkItem(delegate
                {
                    for (int i = 0; i < numberOfTimes; i++)
                    {
                        Console.WriteLine("Received from client: {0}", callback.Hello("Server"));
                    }
                });
            }
        }
        [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
        public class ClientCallback : ITestCallback
        {
            static int count = 0;
            public string Hello(string name)
            {
                Console.WriteLine("In a client operation, name = {0}", name);
                return string.Format("[{0}] Hello, {1}", ++count, name);
            }
        }
        static void PrintUsage()
        {
            string programName = Path.GetFileName(Assembly.GetEntryAssembly().CodeBase);
            Console.WriteLine("Usage: {0} <options>", programName);
            Console.WriteLine("Examples:");
            Console.WriteLine("  Starting the server: {0} -server", programName);
            Console.WriteLine("  Starting the client: {0} -client <serverMachineName>", programName);
        }
        public static void Test(string[] args)
        {
            if (args.Length < 1)
            {
                PrintUsage();
                return;
            }

            if (args[0].Equals("-server", StringComparison.OrdinalIgnoreCase))
            {
                string serviceAddress = "http://" + Environment.MachineName + ":8000/Service";
                ServiceHost host = new ServiceHost(typeof(Service), new Uri(serviceAddress));
                host.AddServiceEndpoint(typeof(ITest), new WSDualHttpBinding(WSDualHttpSecurityMode.None), "");
                host.Open();
                Console.WriteLine("Host opened, press ENTER to close");
                Console.ReadLine();
                host.Close();
            }
            else if (args.Length > 1 && args[0].Equals("-client", StringComparison.OrdinalIgnoreCase))
            {
                string serviceAddress = "http://" + args[1] + ":8000/Service";
                ClientCallback clientCallback = new ClientCallback();
                DuplexChannelFactory<ITest> factory = new DuplexChannelFactory<ITest>(
                    clientCallback,
                    new WSDualHttpBinding(WSDualHttpSecurityMode.None),
                    new EndpointAddress(serviceAddress));
                ITest proxy = factory.CreateChannel();
                Console.WriteLine("Simple Request/Reply: {0}", proxy.Add(3, 4));
                Console.WriteLine("Now calling an operation on the server which will cause callbacks");
                proxy.CallMe(10);
                Console.WriteLine("Press ENTER to close");
                Console.ReadLine();
            }
            else
            {
                PrintUsage();
            }
        }
    }

    // http://stackoverflow.com/q/6209650/751090
    public class StackOverflow_6209650_751090
    {
        [DataContract]
        public class MyDC
        {
            [DataMember]
            public DateTime SerializedTime
            {
                get { return DateTime.Now; }
                set { }
            }
        }
        public static void Test()
        {
            Message message = Message.CreateMessage(MessageVersion.None, "foo", new MyDC());
            var buffer = message.CreateBufferedCopy(int.MaxValue);
            Console.WriteLine(buffer.CreateMessage());
            Console.WriteLine();
            Console.WriteLine(buffer.CreateMessage());
        }
    }

    // http://stackoverflow.com/q/6231864/751090
    public class StackOverflow_6231864_751090
    {
        [ServiceContract(Name = "ITest")]
        public interface ITest
        {
            [OperationContract]
            string Echo(string text);
            [OperationContract]
            int Add(int x, int y);
        }
        [ServiceContract(Name = "ITest")]
        public interface ITestAsync
        {
            [OperationContract(AsyncPattern = true)]
            IAsyncResult BeginEcho(string text, AsyncCallback callback, object state);
            string EndEcho(IAsyncResult ar);
            [OperationContract(AsyncPattern = true)]
            IAsyncResult BeginAdd(int x, int y, AsyncCallback callback, object state);
            int EndAdd(IAsyncResult ar);
        }
        public class Service : ITest
        {
            public string Echo(string text) { return text; }
            public int Add(int x, int y) { return x + y; }
        }
        public class ServiceAsync : ITestAsync
        {
            string Echo(string text) { return text; }
            int Add(int x, int y) { return x + y; }
            public IAsyncResult BeginEcho(string text, AsyncCallback callback, object state)
            {
                Func<string, string> func = Echo;
                return func.BeginInvoke(text, callback, state);
            }

            public string EndEcho(IAsyncResult ar)
            {
                Func<string, string> func = (Func<string, string>)((System.Runtime.Remoting.Messaging.AsyncResult)ar).AsyncDelegate;
                return func.EndInvoke(ar);
            }

            public IAsyncResult BeginAdd(int x, int y, AsyncCallback callback, object state)
            {
                Func<int, int, int> func = Add;
                return func.BeginInvoke(x, y, callback, state);
            }

            public int EndAdd(IAsyncResult ar)
            {
                Func<int, int, int> func = (Func<int, int, int>)((System.Runtime.Remoting.Messaging.AsyncResult)ar).AsyncDelegate;
                return func.EndInvoke(ar);
            }
        }
        public static void Test()
        {
            foreach (bool useAsync in new bool[] { false, true })
            {
                Type contractType = useAsync ? typeof(ITestAsync) : typeof(ITest);
                Type serviceType = useAsync ? typeof(ServiceAsync) : typeof(Service);
                Console.WriteLine("Using {0} service implementation", useAsync ? "Asynchronous" : "Synchronous");
                string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
                ServiceHost host = new ServiceHost(serviceType, new Uri(baseAddress));
                host.AddServiceEndpoint(contractType, new BasicHttpBinding(), "");
                host.Open();
                Console.WriteLine("Host opened");

                Console.WriteLine("Using the same client for both services...");
                ChannelFactory<ITest> factory = new ChannelFactory<ITest>(new BasicHttpBinding(), new EndpointAddress(baseAddress));
                ITest proxy = factory.CreateChannel();
                Console.WriteLine(proxy.Echo("Hello"));
                Console.WriteLine(proxy.Add(3, 4));

                ((IClientChannel)proxy).Close();
                factory.Close();

                host.Close();

                Console.WriteLine("Done");
                Console.WriteLine();
            }
        }
    }

    public class StackOverflow_6267866
    {
        [ServiceContract]
        public interface ITest
        {
            [WebGet(UriTemplate = "/{x}/{y}?z={z}")]
            string EchoString(string x, string y, string z);
        }
        public class Service : ITest
        {
            public string EchoString(string x, string y, string z)
            {
                const string nullStr = "<<null>>";
                return string.Format("{0}-{1}-{2}", x ?? nullStr, y ?? nullStr, z ?? nullStr);
            }
        }
        static void SendRequest(string uri)
        {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(uri);
            req.Method = "GET";

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
            host.AddServiceEndpoint(typeof(ITest), new WebHttpBinding(), "ITest").Behaviors.Add(new WebHttpBehavior());
            host.Open();
            Console.WriteLine("Host opened");

            SendRequest(baseAddress + "/ITest///");

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    // http://stackoverflow.com/q/6272819/751090
    public class StackOverflow_6272819
    {
        public static void Test()
        {
            string xml = @"<hash>
  <result>
    <properties type=""array"">
      <property>
        <registers type=""array"">
          <register>
            <dials type=""integer"">6</dials>
          </register>
          <register>
            <dials type=""integer"">6</dials>
          </register>
        </registers>
        <unit-balance type=""integer"">-104</unit-balance>
      </property>
    </properties>
    <account-number>9001234</account-number>
  </result>
  <version>1.0</version>
</hash>";

            XElement customerXml = XElement.Parse(xml);
            var rawProperties = from property in customerXml.Descendants("property")
                                select new
                                {
                                    UnitBalance = property.Element("unit-balance").Value,
                                    Registers = from register in property.Descendants("register")
                                                select register.Element("dials").Value
                                };
            Console.WriteLine(rawProperties);
        }
    }

    // http://stackoverflow.com/q/6267090/751090
    public class StackOverflow_6267090
    {
        static bool useHttp;
        const string baseAddressHttp = "http://localhost:8000/Bug/";
        const string baseAddressPipe = "net.pipe://localhost/Bug/";
        static Binding GetBinding()
        {
            if (useHttp)
            {
                return new BasicHttpBinding();
            }
            else
            {
                return new NetNamedPipeBinding();
            }
        }
        static string GetBaseAddress()
        {
            return useHttp ? baseAddressHttp : baseAddressPipe;
        }
        [ServiceContract]
        public interface IInner
        {
            [OperationContract]
            [FaultContract(typeof(Detail))]
            int DoStuff();
        }
        [ServiceContract]
        public interface IOuter
        {
            [OperationContract]
            [FaultContract(typeof(Detail))]
            int DoStuff();
        }
        [DataContract]
        public class Detail
        {
            [DataMember]
            public string Data { get; set; }

            public override string ToString()
            {
                return string.Format("Detail[Data={0}]", Data);
            }
        }
        [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
        public class InnerService : IInner
        {
            public int DoStuff()
            {
                //return 3;
                throw new FaultException<Detail>(new Detail { Data = "Something" }, new FaultReason("My special reason"));
            }
        }
        class OuterService : IOuter
        {
            public int DoStuff()
            {
                try
                {
                    return Caller.CallInner("In service");
                }
                catch (FaultException<Detail> e)
                {
                    throw new FaultException<Detail>(e.Detail, e.Reason);
                }
            }
        }
        public static class Caller
        {
            public static int CallInner(string where)
            {
                try
                {
                    var factory = new ChannelFactory<IInner>(GetBinding(), new EndpointAddress(GetBaseAddress() + "Inner/"));
                    var channel = factory.CreateChannel();
                    int result = channel.DoStuff();
                    return result;
                }
                catch (FaultException<Detail> e)
                {
                    Console.WriteLine("[{0} - CallInner] Error, Message={1}, Detail={2}", where, e.Message, e.Detail);
                    throw;
                }
            }

            public static int CallOuter(string where)
            {
                try
                {
                    var factory = new ChannelFactory<IOuter>(GetBinding(), new EndpointAddress(GetBaseAddress() + "Outer/"));
                    var channel = factory.CreateChannel();
                    int result = channel.DoStuff();
                    return result;
                }
                catch (FaultException<Detail> e)
                {
                    Console.WriteLine("[{0} - CallOuter] Error, Message={1}, Detail={2}", where, e.Message, e.Detail);
                    throw;
                }
            }
        }
        public static void TestWith(bool useHttp)
        {
            StackOverflow_6267090.useHttp = useHttp;
            Console.WriteLine("Using address: {0}", GetBaseAddress());
            string baseAddress = GetBaseAddress();
            ServiceHost innerHost = new ServiceHost(typeof(InnerService), new Uri(baseAddress + "Inner/"));
            ServiceHost outerHost = new ServiceHost(typeof(OuterService), new Uri(baseAddress + "Outer/"));
            innerHost.AddServiceEndpoint(typeof(IInner), GetBinding(), "");
            outerHost.AddServiceEndpoint(typeof(IOuter), GetBinding(), "");
            innerHost.Open();
            outerHost.Open();
            Console.WriteLine("Hosts opened");

            Console.WriteLine("Calling inner directly");
            try
            {
                Console.WriteLine(Caller.CallInner("client"));
            }
            catch (FaultException<Detail> e)
            {
                Console.WriteLine("In client, after CallInner, Message = {0}, Detail = {1}", e.Message, e.Detail);
            }

            Console.WriteLine("Calling outer");
            try
            {
                Console.WriteLine(Caller.CallOuter("client"));
            }
            catch (FaultException<Detail> e)
            {
                Console.WriteLine("In client, after CallOuter, Message = {0}, Detail = {1}", e.Message, e.Detail);
            }
            catch (FaultException e)
            {
                Console.WriteLine("BUG BUG - this should not have arrived here. Exception = {0}", e);
            }
        }
        public static void Test()
        {
            TestWith(true);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            TestWith(false);
        }
    }

    public class Post_b6a63705_bccd_451a_b50a_4c6376feeabc
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            string Echo(string text);
        }
        [ServiceContract]
        public interface IHelpPage
        {
            [WebGet(UriTemplate = "*")]
            Stream GetHelpPage();
        }
        public class Service : ITest, IHelpPage
        {
            public string Echo(string text)
            {
                return text;
            }

            public Stream GetHelpPage()
            {
                string page = @"<html>
  <head>
    <title>My new help page</title>
  </head>
  <body>
    <h1>This is my custom help page!</h1>
  </body>
</html>";
                WebOperationContext.Current.OutgoingResponse.ContentType = "text/html";
                return new MemoryStream(Encoding.UTF8.GetBytes(page));
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), new BasicHttpBinding(), "basic");
            host.AddServiceEndpoint(typeof(IHelpPage), new WebHttpBinding(), "").Behaviors.Add(new WebHttpBehavior());
            ServiceDebugBehavior sdb = host.Description.Behaviors.Find<ServiceDebugBehavior>();
            if (sdb != null)
            {
                sdb.HttpHelpPageEnabled = false;
                sdb.HttpsHelpPageEnabled = false;
            }

            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(new BasicHttpBinding(), new EndpointAddress(baseAddress + "/basic"));
            ITest proxy = factory.CreateChannel();
            Console.WriteLine("Service still working: {0}", proxy.Echo("Hello"));

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_55ef7692_25dc_4ece_9dde_9981c417c94a
    {
        [ServiceContract(Name = "ITest", Namespace = "http://tempuri.org/")]
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
        static Binding GetBinding()
        {
            BasicHttpBinding result = new BasicHttpBinding();
            return result;
        }
        public class MyOperationBypasser : IEndpointBehavior, IOperationBehavior
        {
            internal const string SkipServerMessageProperty = "SkipServer";
            public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
            {
            }

            public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
            {
            }

            public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
            {
                endpointDispatcher.DispatchRuntime.MessageInspectors.Add(new MyInspector(endpoint));
            }

            public void Validate(ServiceEndpoint endpoint)
            {
            }

            public void AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters)
            {
            }

            public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
            {
            }

            public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
            {
                dispatchOperation.Formatter = new MyFormatter(dispatchOperation.Formatter);
                dispatchOperation.Invoker = new MyInvoker(dispatchOperation.Invoker);
            }

            public void Validate(OperationDescription operationDescription)
            {
            }

            class MyInspector : IDispatchMessageInspector
            {
                ServiceEndpoint endpoint;
                public MyInspector(ServiceEndpoint endpoint)
                {
                    this.endpoint = endpoint;
                }

                public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
                {
                    Message result = null;
                    HttpRequestMessageProperty reqProp = null;
                    if (request.Properties.ContainsKey(HttpRequestMessageProperty.Name))
                    {
                        reqProp = request.Properties[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty;
                    }

                    if (reqProp != null)
                    {
                        string bypassServer = reqProp.Headers["X-BypassServer"];
                        if (!string.IsNullOrEmpty(bypassServer))
                        {
                            result = Message.CreateMessage(request.Version, this.FindReplyAction(request.Headers.Action), new OverrideBodyWriter(bypassServer));
                        }
                    }

                    return result;
                }

                public void BeforeSendReply(ref Message reply, object correlationState)
                {
                    Message newResult = correlationState as Message;
                    if (newResult != null)
                    {
                        reply = newResult;
                    }
                }

                private string FindReplyAction(string requestAction)
                {
                    foreach (var operation in this.endpoint.Contract.Operations)
                    {
                        if (operation.Messages[0].Action == requestAction)
                        {
                            return operation.Messages[1].Action;
                        }
                    }

                    return null;
                }

                class OverrideBodyWriter : BodyWriter
                {
                    string bypassServerHeader;
                    public OverrideBodyWriter(string bypassServerHeader)
                        : base(true)
                    {
                        this.bypassServerHeader = bypassServerHeader;
                    }

                    protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
                    {
                        writer.WriteStartElement("EchoResponse", "http://tempuri.org/");
                        writer.WriteStartElement("EchoResult");
                        writer.WriteString(this.bypassServerHeader);
                        writer.WriteEndElement();
                        writer.WriteEndElement();
                    }
                }
            }

            class MyFormatter : IDispatchMessageFormatter
            {
                IDispatchMessageFormatter originalFormatter;
                public MyFormatter(IDispatchMessageFormatter originalFormatter)
                {
                    this.originalFormatter = originalFormatter;
                }

                public void DeserializeRequest(Message message, object[] parameters)
                {
                    if (message.Properties.ContainsKey(MyOperationBypasser.SkipServerMessageProperty))
                    {
                        Message returnMessage = message.Properties[MyOperationBypasser.SkipServerMessageProperty] as Message;
                        OperationContext.Current.IncomingMessageProperties.Add(MyOperationBypasser.SkipServerMessageProperty, returnMessage);
                        OperationContext.Current.OutgoingMessageProperties.Add(MyOperationBypasser.SkipServerMessageProperty, returnMessage);
                    }
                    else
                    {
                        this.originalFormatter.DeserializeRequest(message, parameters);
                    }
                }

                public Message SerializeReply(MessageVersion messageVersion, object[] parameters, object result)
                {
                    if (OperationContext.Current.OutgoingMessageProperties.ContainsKey(MyOperationBypasser.SkipServerMessageProperty))
                    {
                        return null;
                    }
                    else
                    {
                        return this.originalFormatter.SerializeReply(messageVersion, parameters, result);
                    }
                }
            }

            class MyInvoker : IOperationInvoker
            {
                IOperationInvoker originalInvoker;

                public MyInvoker(IOperationInvoker originalInvoker)
                {
                    if (!originalInvoker.IsSynchronous)
                    {
                        throw new NotSupportedException("This implementation only supports synchronous invokers");
                    }

                    this.originalInvoker = originalInvoker;
                }

                public object[] AllocateInputs()
                {
                    return this.originalInvoker.AllocateInputs();
                }

                public object Invoke(object instance, object[] inputs, out object[] outputs)
                {
                    if (OperationContext.Current.IncomingMessageProperties.ContainsKey(MyOperationBypasser.SkipServerMessageProperty))
                    {
                        outputs = null;
                        return null; // message is stored in the context
                    }
                    else
                    {
                        return this.originalInvoker.Invoke(instance, inputs, out outputs);
                    }
                }

                public IAsyncResult InvokeBegin(object instance, object[] inputs, AsyncCallback callback, object state)
                {
                    throw new NotSupportedException();
                }

                public object InvokeEnd(object instance, out object[] outputs, IAsyncResult result)
                {
                    throw new NotSupportedException();
                }

                public bool IsSynchronous
                {
                    get { return true; }
                }
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            ServiceEndpoint endpoint = host.AddServiceEndpoint(typeof(ITest), GetBinding(), "");
            endpoint.Behaviors.Add(new MyOperationBypasser());
            foreach (var operation in endpoint.Contract.Operations)
            {
                operation.Behaviors.Add(new MyOperationBypasser());
            }

            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(GetBinding(), new EndpointAddress(baseAddress));
            ITest proxy = factory.CreateChannel();
            Console.WriteLine(proxy.Echo("Hello"));

            Console.WriteLine("And now with the bypass header");
            using (new OperationContextScope((IContextChannel)proxy))
            {
                HttpRequestMessageProperty httpRequestProp = new HttpRequestMessageProperty();
                httpRequestProp.Headers.Add("X-BypassServer", "This message will not reach the service operation");
                OperationContext.Current.OutgoingMessageProperties.Add(
                    HttpRequestMessageProperty.Name,
                    httpRequestProp);
                Console.WriteLine(proxy.Echo("Hello"));
            }

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    // http://stackoverflow.com/q/6414181/751090
    public class StackOverflow_6414181
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            [WebGet]
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
            host.Description.Behaviors.Add(new ServiceMetadataBehavior { HttpGetEnabled = true });
            //host.AddServiceEndpoint(typeof(ITest), new BasicHttpBinding(), "soap");
            host.AddServiceEndpoint(typeof(ITest), new WebHttpBinding(), "rest").Behaviors.Add(new WebHttpBehavior());
            host.Open();
            Console.WriteLine("Host opened");

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    // http://stackoverflow.com/q/6445171/751090
    public class StackOverflow_6445171
    {
        [ServiceContract]
        public class Service
        {
            [WebGet(ResponseFormat = WebMessageFormat.Json)]
            public string GetLabelPacketTags(string query, int[] statusTypes)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Query=" + query);
                sb.Append(", statusTypes=");
                if (statusTypes == null)
                {
                    sb.Append("null");
                }
                else
                {
                    sb.Append("[");
                    for (int i = 0; i < statusTypes.Length; i++)
                    {
                        if (i > 0) sb.Append(",");
                        sb.Append(statusTypes[i]);
                    }
                    sb.Append("]");
                }

                return sb.ToString();
            }
        }
        class MyWebHttpBehavior : WebHttpBehavior
        {
            protected override IDispatchMessageFormatter GetRequestDispatchFormatter(OperationDescription operationDescription, ServiceEndpoint endpoint)
            {
                return new MyArrayAwareFormatter(operationDescription, this.GetQueryStringConverter(operationDescription));
            }

            class MyArrayAwareFormatter : IDispatchMessageFormatter
            {
                OperationDescription operation;
                QueryStringConverter queryStringConverter;
                public MyArrayAwareFormatter(OperationDescription operation, QueryStringConverter queryStringConverter)
                {
                    this.operation = operation;
                    this.queryStringConverter = queryStringConverter;
                }

                public void DeserializeRequest(Message message, object[] parameters)
                {
                    if (message.Properties.ContainsKey("UriMatched") && (bool)message.Properties["UriMatched"])
                    {
                        UriTemplateMatch match = message.Properties["UriTemplateMatchResults"] as UriTemplateMatch;
                        NameValueCollection queryValues = match.QueryParameters;
                        foreach (MessagePartDescription parameterDescr in this.operation.Messages[0].Body.Parts)
                        {
                            string parameterName = parameterDescr.Name;
                            int index = parameterDescr.Index;
                            if (parameterDescr.Type.IsArray)
                            {
                                Type elementType = parameterDescr.Type.GetElementType();
                                string[] values = queryValues.GetValues(parameterName + "[]");
                                Array array = Array.CreateInstance(elementType, values.Length);
                                for (int i = 0; i < values.Length; i++)
                                {
                                    array.SetValue(this.queryStringConverter.ConvertStringToValue(values[i], elementType), i);
                                }
                                parameters[index] = array;
                            }
                            else
                            {
                                parameters[index] = this.queryStringConverter.ConvertStringToValue(queryValues.GetValues(parameterName)[0], parameterDescr.Type);
                            }
                        }
                    }
                }

                public Message SerializeReply(MessageVersion messageVersion, object[] parameters, object result)
                {
                    throw new NotSupportedException("This is a request-only formatter");
                }
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(Service), new WebHttpBinding(), "").Behaviors.Add(new MyWebHttpBehavior());
            host.Open();
            Console.WriteLine("Host opened");

            WebClient c = new WebClient();
            Console.WriteLine(c.DownloadString(baseAddress + "/GetLabelPacketTags?query=some+text&statusTypes[]=1&statusTypes[]=2"));
            Console.WriteLine(c.DownloadString(baseAddress + "/GetLabelPacketTags?query=some+text&statusTypes%5B%5D=1&statusTypes%5B%5D=2"));

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_d74d242f_bd01_4850_9971_de8d4f3b45b2
    {
        //<?xml version="1.0">
        //<api>
        //    <files>
        //    <file>
        //        <id>[int]</id>
        //        <name>[string]</id>
        //    </file>
        //    </files>
        //</api>
        [ServiceContract]
        public class Service
        {
            [WebGet(UriTemplate = "/api/files/{groupId}")]
            public Api GetCollection(string groupId)
            {
                return new Api
                {
                    Files = new Files
                    {
                        new FileItem { Id = 1, Name = "File 1" },
                        new FileItem { Id = 2, Name = "File 2" },
                    }
                };
            }
        }
        [DataContract(Name = "api", Namespace = "")]
        public class Api
        {
            [DataMember(Name = "files")]
            public Files Files { get; set; }
        }
        [CollectionDataContract(Name = "files", ItemName = "file", Namespace = "")]
        public class Files : List<FileItem> { }
        [DataContract(Namespace = "", Name = "FileItem")]
        public class FileItem
        {
            [DataMember(Name = "id")]
            public int Id { get; set; }
            [DataMember(Name = "name")]
            public string Name { get; set; }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            WebServiceHost host = new WebServiceHost(typeof(Service), new Uri(baseAddress));
            host.Open();
            Console.WriteLine("Host opened");

            WebClient c = new WebClient();
            Console.WriteLine(c.DownloadString(baseAddress + "/api/files/group"));

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_90d0f85e_5240_422d_8abc_ddadafd01637
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
        public class MyInstanceProvider : IServiceBehavior, IInstanceProvider
        {
            public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
            {
            }

            public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
            {
                foreach (ChannelDispatcher cd in serviceHostBase.ChannelDispatchers)
                {
                    foreach (EndpointDispatcher ed in cd.Endpoints)
                    {
                        if (!ed.IsSystemEndpoint)
                        {
                            ed.DispatchRuntime.InstanceProvider = this;
                        }
                    }
                }
            }

            public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
            {
            }

            public object GetInstance(InstanceContext instanceContext, Message message)
            {
                return new Service();
            }

            public object GetInstance(InstanceContext instanceContext)
            {
                return new Service();
            }

            public void ReleaseInstance(InstanceContext instanceContext, object instance)
            {
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(ITest), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), new BasicHttpBinding(), "");
            host.Description.Behaviors.Add(new MyInstanceProvider());
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(new BasicHttpBinding(), new EndpointAddress(baseAddress));
            ITest proxy = factory.CreateChannel();
            Console.WriteLine(proxy.Echo("Hello"));

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    // http://stackoverflow.com/q/5731102/751090
    public class StackOverflow_5731102
    {
        [ServiceContract]
        public class Service
        {
            [WebInvoke]
            public Stream Process(Stream input)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var header in WebOperationContext.Current.IncomingRequest.Headers.AllKeys)
                {
                    sb.AppendLine(string.Format("{0}: {1}", header, WebOperationContext.Current.IncomingRequest.Headers[header]));
                }

                string contentType = WebOperationContext.Current.IncomingRequest.ContentType;
                Encoding encoding = Encoding.GetEncoding(contentType.Substring(contentType.IndexOf('=') + 1));
                WebOperationContext.Current.OutgoingResponse.ContentType = WebOperationContext.Current.IncomingRequest.ContentType;
                return new MemoryStream(encoding.GetBytes(sb.ToString()));
            }
        }

        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            WebServiceHost host = new WebServiceHost(typeof(Service), new Uri(baseAddress));
            host.Open();
            Console.WriteLine("Host opened");

            foreach (var encoding in new Encoding[] { new UTF8Encoding(true), new UnicodeEncoding(false, true) })
            {
                Console.WriteLine("Sending encoding = {0}", encoding.WebName);
                WebClient client = new WebClient();
                client.Headers[HttpRequestHeader.ContentType] = "text/plain; charset=" + encoding.WebName;
                client.Encoding = encoding;
                string url = baseAddress + "/Process";
                string data = "hello";
                string result = client.UploadString(url, data);
                Console.WriteLine(result);

                Console.WriteLine(string.Join(",", encoding.GetBytes(data).Select(b => b.ToString("X2"))));
            }

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    // http://stackoverflow.com/q/6470463/751090
    public class StackOverflow_6470463
    {
        [ServiceContract(Namespace = "http://schemas.mycompany.com/", Name = "MyService")]
        public interface IMyService
        {
            [OperationContract(Name = "MyOperation")]
            string MyOperation(string request);
        }
        public class Service : IMyService
        {
            public string MyOperation(string request) { return request; }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(IMyService), new BasicHttpBinding(), "");
            host.Open();
            Console.WriteLine("Host opened");

            foreach (ServiceEndpoint endpoint in host.Description.Endpoints)
            {
                Console.WriteLine("Endpoint: {0}", endpoint.Name);
                foreach (var operation in endpoint.Contract.Operations)
                {
                    Console.WriteLine("  Operation: {0}", operation.Name);
                    Console.WriteLine("    Action: {0}", operation.Messages[0].Action);
                    if (operation.Messages.Count > 1)
                    {
                        Console.WriteLine("    ReplyAction: {0}", operation.Messages[1].Action);
                    }
                }
            }

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    // http://stackoverflow.com/q/6473251/751090
    public class StackOverflow_6473251
    {
        public static void Test()
        {
            string xml = @"               <movies>
                 <movie>
                   <score>8.582207</score>
                   <popularity>3</popularity>
                   <translated>true</translated>
                   <adult>false</adult>
                   <language>en</language>
                   <original_name>Transformers</original_name>
                   <name>Transformers</name>
                   <alternative_name>The Transformers</alternative_name>
                   <type>movie</type>
                   <id>1858</id>
                   <imdb_id>tt0418279</imdb_id>
                   <url>http://www.themoviedb.org/movie/1858</url>
                   <votes>28</votes>
                   <rating>7.2</rating>
                   <certification>PG-13</certification>
                   <overview>The Earth is caught in the middle of an intergalactic war /overview>
                   <released>2007-07-04</released>
                   <images>
                        <image type=""poster"" url=""http://hwcdn.themoviedb.org/posters/304/4bc91347017a3c57fe007304/transformers-original.jpg"" size=""original"" id=""4bc91347017a3c57fe007304""/>
                        <image type=""poster"" url=""http://hwcdn.themoviedb.org/posters/304/4bc91347017a3c57fe007304/transformers-mid.jpg"" size=""mid"" id=""4bc91347017a3c57fe007304""/>
                        <image type=""poster"" url=""http://hwcdn.themoviedb.org/posters/304/4bc91347017a3c57fe007304/transformers-cover.jpg"" size=""cover"" id=""4bc91347017a3c57fe007304""/>
                        <image type=""poster"" url=""http://hwcdn.themoviedb.org/posters/304/4bc91347017a3c57fe007304/transformers-thumb.jpg"" size=""thumb"" id=""4bc91347017a3c57fe007304""/>
                        <image type=""backdrop"" url=""http://hwcdn.themoviedb.org/backdrops/2ce/4bc91339017a3c57fe0072ce/transformers-original.jpg"" size=""original"" id=""4bc9133s9017a3c57fe0072ce""/>
                        <image type=""backdrop"" url=""http://hwcdn.themoviedb.org/backdrops/2ce/4bc91339017a3c57fe0072ce/transformers-poster.jpg"" size=""poster"" id=""4bc91339017a3c57fe0072ce""/>
                        <image type=""backdrop"" url=""http://hwcdn.themoviedb.org/backdrops/2ce/4bc91339017a3c57fe0072ce/transformers-thumb.jpg"" size=""thumb"" id=""4bc91339017a3c57fe0072ce""/>
                   </images>
                 <last_modified_at>2010-04-26 03:26:14</last_modified_at>
           </movie>
         </movies>";
            XmlReader r = XmlReader.Create(new StringReader(xml));
            r.ReadToFollowing("original_name");
            string title = r.ReadElementContentAsString("original_name", r.NamespaceURI);
            r.ReadToFollowing("images");
            int imageCount = 0;
            if (r.ReadToDescendant("image"))
            {
                do
                {
                    Console.WriteLine("Image {0}", ++imageCount);
                    Console.WriteLine("  Type: {0}", r.GetAttribute("type"));
                    Console.WriteLine("  URL: {0}", r.GetAttribute("url"));
                    Console.WriteLine("  Size: {0}", r.GetAttribute("size"));
                    Console.WriteLine("  ID: {0}", r.GetAttribute("id"));
                } while (r.ReadToNextSibling("image"));
            }
        }
    }

    // http://stackoverflow.com/q/2495195/751090
    public class StackOverflow_2495195
    {
        [MessageContract(WrapperName = "Browse", WrapperNamespace = "urn:schemas-upnp-org:service:ContentDirectory:1", IsWrapped = true)]
        public class BrowseRequest
        {
            [MessageBodyMember(Namespace = "", Order = 0)]
            public string ObjectID;

            [MessageBodyMember(Namespace = "", Order = 1)]
            public string BrowseFlag;

            [MessageBodyMember(Namespace = "", Order = 2)]
            public string Filter;

            [MessageBodyMember(Namespace = "", Order = 3)]
            public ulong StartingIndex;

            [MessageBodyMember(Namespace = "", Order = 4)]
            public ulong RequestedCount;

            [MessageBodyMember(Namespace = "", Order = 5)]
            public string SortCriteria;
        }

        [MessageContract(WrapperName = "BrowseResponse", WrapperNamespace = "urn:schemas-upnp-org:service:ContentDirectory:1", IsWrapped = true)]
        public class BrowseResponse
        {
            [MessageBodyMember(Namespace = "", Order = 0)]
            public string Result;

            [MessageBodyMember(Namespace = "", Order = 1)]
            public ulong NumberReturned;

            [MessageBodyMember(Namespace = "", Order = 2)]
            public ulong TotalMatches;

            [MessageBodyMember(Namespace = "", Order = 3)]
            public ulong UpdateID;
        }

        [ServiceContract(Namespace = "urn:schemas-upnp-org:service:ContentDirectory:1")]
        public interface IContentDirectory
        {
            [OperationContract(Action = "urn:schemas-upnp-org:service:ContentDirectory:1#Browse")]
            //void Browse(string ObjectID, string BrowseFlag, string Filter, ulong StartingIndex, ulong RequestedCount, string SortCriteria, out string Result, out ulong NumberReturned, out ulong TotalMatches, out ulong UpdateID);
            BrowseResponse Browse(BrowseRequest request);
        }
        public class Service : IContentDirectory
        {
            //public void Browse(string ObjectID, string BrowseFlag, string Filter, ulong StartingIndex, ulong RequestedCount, string SortCriteria, out string Result, out ulong NumberReturned, out ulong TotalMatches, out ulong UpdateID)
            //{
            //    Result = null;
            //    NumberReturned = 0;
            //    TotalMatches = 0;
            //    UpdateID = 0;
            //}
            public BrowseResponse Browse(BrowseRequest request)
            {
                return new BrowseResponse { NumberReturned = 0, Result = null, TotalMatches = 0, UpdateID = 0 };
            }
        }
        static Binding GetBinding()
        {
            return new CustomBinding(
                new TextMessageEncodingBindingElement(MessageVersion.Soap11WSAddressing10, Encoding.UTF8),
                new HttpTransportBindingElement());
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(IContentDirectory), GetBinding(), "");
            host.Description.Behaviors.Add(new ServiceMetadataBehavior { HttpGetEnabled = true });
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<IContentDirectory> factory = new ChannelFactory<IContentDirectory>(GetBinding(), new EndpointAddress(baseAddress));
            IContentDirectory proxy = factory.CreateChannel();
            //string result;
            //ulong ul1, ul2, ul3;
            //proxy.Browse(null, null, null, 0, 0, null, out result, out ul1, out ul2, out ul3);
            proxy.Browse(new BrowseRequest { BrowseFlag = null, Filter = null, ObjectID = null, RequestedCount = 0, SortCriteria = null, StartingIndex = 0 });

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_55b6f018_2944_4160_8393_62a14376c361
    {
        const string JSON = @"{""d"" : {
            ""__metadata"": {""uri"": ""http://dd-1620/ServiceData.svc/Customers('1001')"", ""type"": ""DataModel.Customer""},
            ""MasterCustomerId"": ""1001"",
            ""SubCustomerId"": ""0"",
            ""FirstName"": ""Jag"",
            ""LastName"": ""Chat""}}  ";

        [DataContract]
        public class ResponseWrapper
        {
            [DataMember]
            public Response d;

            public override string ToString()
            {
                return string.Format("ResponseWrapper[d={0}]", this.d);
            }
        }
        [DataContract]
        public class Response
        {
            [DataMember(Name = "__metadata")]
            public Metadata metadata;
            [DataMember]
            public string MasterCustomerId;
            [DataMember]
            public string SubCustomerId;
            [DataMember]
            public string FirstName;
            [DataMember]
            public string LastName;

            public override string ToString()
            {
                return string.Format("Response[metadata={0},MasterCustomerId={1},SubCustomerId={2},FirstName={3},LastName={4}]",
                    metadata, MasterCustomerId, SubCustomerId, FirstName, LastName);
            }
        }
        public class Metadata
        {
            [DataMember]
            public Uri uri;
            [DataMember]
            public string type;

            public override string ToString()
            {
                return string.Format("Metadata[uri={0},type={1}]", uri, type);
            }
        }
        public static void Test()
        {
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(JSON));
            DataContractJsonSerializer dcjs = new DataContractJsonSerializer(typeof(ResponseWrapper));
            var deserialized = dcjs.ReadObject(ms);
            Console.WriteLine(deserialized);
        }
    }

    // http://stackoverflow.com/q/6526659/751090
    public class StackOverflow_6526659
    {
        [ServiceContract]
        public interface IGeoDataService
        {
            [OperationContract]
            [WebInvoke(Method = "POST",
                BodyStyle = WebMessageBodyStyle.Wrapped,
                ResponseFormat = WebMessageFormat.Json)]
            List<BranchData> GetBranches();
        }

        public class Service : IGeoDataService
        {
            public List<BranchData> GetBranches()
            {
                return new List<BranchData>();
            }
        }

        // Use a data contract as illustrated in the sample below to add composite types to service operations.
        [DataContract]
        public class BranchData
        {
            [DataMember]
            public string BranchNumber { get; set; }

            [DataMember]
            public string BranchName { get; set; }

            [DataMember]
            public string StreetAddress { get; set; }

            [DataMember]
            public string City { get; set; }

            [DataMember]
            public string Zip { get; set; }

            [DataMember]
            public string State { get; set; }

            [DataMember]
            public string Phone { get; set; }

            [DataMember]
            public string County { get; set; }
        }

        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            WebHttpBinding binding = new WebHttpBinding { CrossDomainScriptAccessEnabled = true };
            WebHttpBehavior behavior = new WebHttpBehavior();
            host.AddServiceEndpoint(typeof(IGeoDataService), binding, "").Behaviors.Add(behavior);
            host.Open();
            Console.WriteLine("Host opened");

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(baseAddress + "/GetBranches");
            req.Method = "POST";
            req.GetRequestStream().Close();
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Console.WriteLine("HTTP/{0} {1} {2}", resp.ProtocolVersion, (int)resp.StatusCode, resp.StatusDescription);
            foreach (var header in resp.Headers.AllKeys)
            {
                Console.WriteLine("{0}: {1}", header, resp.Headers[header]);
            }
            if (resp.ContentLength > 0)
            {
                Console.WriteLine(new StreamReader(resp.GetResponseStream()).ReadToEnd());
            }

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    // http://stackoverflow.com/q/6539963/751090
    public class StackOverflow_6539963
    {
        public class MyServiceBehaviorAttribute : Attribute, IServiceBehavior
        {
            public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
            {
            }

            public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
            {
                Console.WriteLine("In MyServiceBehaviorAttribute.ApplyDispatchBehavior");
                // do whatever initialization you need
            }

            public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
            {
            }
        }
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            string Echo(string text);
        }
        [MyServiceBehavior]
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
            host.AddServiceEndpoint(typeof(ITest), new BasicHttpBinding(), "");
            host.Open();
            Console.WriteLine("Host opened");

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_c1a29404_c990_482a_901f_dbfdf0d8f270
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
        public class MyMessageEncodingBindingElement : MessageEncodingBindingElement
        {
            MessageEncodingBindingElement inner;
            public MyMessageEncodingBindingElement(MessageEncodingBindingElement inner)
            {
                this.inner = inner;
            }

            private MyMessageEncodingBindingElement(MyMessageEncodingBindingElement other)
            {
                this.inner = other.inner;
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
                return new MyMessageEncodingBindingElement(this);
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
                private MessageEncoderFactory messageEncoderFactory;

                public MyMessageEncoderFactory(MessageEncoderFactory messageEncoderFactory)
                {
                    this.messageEncoderFactory = messageEncoderFactory;
                }

                public override MessageEncoder Encoder
                {
                    get { return new MyMessageEncoder(this.messageEncoderFactory.Encoder); }
                }

                public override MessageVersion MessageVersion
                {
                    get { return this.messageEncoderFactory.MessageVersion; }
                }

                public override MessageEncoder CreateSessionEncoder()
                {
                    return new MyMessageEncoder(this.messageEncoderFactory.CreateSessionEncoder());
                }
            }

            class MyMessageEncoder : MessageEncoder
            {
                const string ApplicationJsonWithCharset = "application/json; charset=";
                private MessageEncoder messageEncoder;

                public MyMessageEncoder(MessageEncoder messageEncoder)
                {
                    this.messageEncoder = messageEncoder;
                }

                public override string ContentType
                {
                    get { return this.messageEncoder.ContentType; }
                }

                public override string MediaType
                {
                    get { return this.messageEncoder.MediaType; }
                }

                public override MessageVersion MessageVersion
                {
                    get { return this.messageEncoder.MessageVersion; }
                }

                public override bool IsContentTypeSupported(string contentType)
                {
                    return this.ContentType.StartsWith(ApplicationJsonWithCharset, StringComparison.OrdinalIgnoreCase) || this.messageEncoder.IsContentTypeSupported(contentType);
                }

                public override Message ReadMessage(ArraySegment<byte> buffer, BufferManager bufferManager, string contentType)
                {
                    if (contentType.StartsWith(ApplicationJsonWithCharset, StringComparison.OrdinalIgnoreCase))
                    {
                        Encoding encoding = Encoding.GetEncoding(contentType.Substring(ApplicationJsonWithCharset.Length));
                        if (encoding.WebName != Encoding.UTF8.WebName && encoding.WebName != Encoding.Unicode.WebName && encoding.WebName != Encoding.BigEndianUnicode.WebName)
                        {
                            // Those are supported natively, we don't need to convert them

                            string content = encoding.GetString(buffer.Array, buffer.Offset, buffer.Count);
                            byte[] utf8Content = Encoding.UTF8.GetBytes(content);
                            byte[] newBuffer = bufferManager.TakeBuffer(utf8Content.Length + buffer.Offset);
                            Array.Copy(buffer.Array, 0, newBuffer, 0, buffer.Offset);
                            Array.Copy(utf8Content, 0, newBuffer, buffer.Offset, utf8Content.Length);
                            bufferManager.ReturnBuffer(buffer.Array);
                            buffer = new ArraySegment<byte>(newBuffer, buffer.Offset, utf8Content.Length);
                            contentType = ApplicationJsonWithCharset + "utf-8";
                        }
                    }

                    return this.messageEncoder.ReadMessage(buffer, bufferManager, contentType);
                }

                public override Message ReadMessage(Stream stream, int maxSizeOfHeaders, string contentType)
                {
                    throw new NotSupportedException("Streamed not supported");
                }

                public override ArraySegment<byte> WriteMessage(Message message, int maxMessageSize, BufferManager bufferManager, int messageOffset)
                {
                    return this.messageEncoder.WriteMessage(message, maxMessageSize, bufferManager, messageOffset);
                }

                public override void WriteMessage(Message message, Stream stream)
                {
                    throw new NotSupportedException("Streamed not supported");
                }
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            CustomBinding binding = new CustomBinding(new WebHttpBinding());
            for (int i = 0; i < binding.Elements.Count; i++)
            {
                MessageEncodingBindingElement mebe = binding.Elements[i] as MessageEncodingBindingElement;
                if (mebe != null)
                {
                    binding.Elements[i] = new MyMessageEncodingBindingElement(mebe);
                }
            }
            host.AddServiceEndpoint(typeof(ITest), binding, "").Behaviors.Add(new WebHttpBehavior());
            host.Open();
            Console.WriteLine("Host opened");

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(baseAddress + "/Echo");
            req.Method = "POST";
            Encoding someEncoding = Encoding.GetEncoding("iso-8859-1");
            req.ContentType = "application/json; charset=" + someEncoding.WebName;
            string body = "\"latin: áéíóú\"";
            byte[] bodyBytes = someEncoding.GetBytes(body);
            req.GetRequestStream().Write(bodyBytes, 0, bodyBytes.Length);
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
            Console.WriteLine(new StreamReader(resp.GetResponseStream()).ReadToEnd());

            Console.WriteLine();
            Console.WriteLine("  *-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*  ");
            Console.WriteLine();
        }
    }

    // http://stackoverflow.com/q/6603028/751090
    public class StackOverflow_6603028
    {
        public static void Test()
        {
            CodeDomProvider CodeProvider = CodeDomProvider.CreateProvider("CSharp");

            // Create the Unit
            CodeCompileUnit Unit = new CodeCompileUnit();

            // Define a namespace and add Imports statements
            CodeNamespace Namespaces = new CodeNamespace("Test.CreateForm");
            Namespaces.Imports.Add(new CodeNamespaceImport("System"));
            Namespaces.Imports.Add(new CodeNamespaceImport("System.Drawing"));
            Namespaces.Imports.Add(new CodeNamespaceImport("System.Windows.Forms"));
            Namespaces.Imports.Add(new CodeNamespaceImport("System.Xml"));
            Namespaces.Imports.Add(new CodeNamespaceImport("System.Data"));

            Unit.Namespaces.Add(Namespaces);

            // Declare the type including base type
            CodeTypeDeclaration MyType = new CodeTypeDeclaration("Form1");
            MyType.IsClass = true;
            MyType.TypeAttributes = System.Reflection.TypeAttributes.Public;
            MyType.BaseTypes.Add("System.Windows.Forms.Form");

            Namespaces.Types.Add(MyType);

            // Create the constructor and add code
            CodeConstructor Constructor = new CodeConstructor();

            Constructor.Statements.Add(
              new CodeMethodInvokeExpression(
              new CodeThisReferenceExpression(), "InitializeComponent", new CodeExpression() { }));

            Constructor.Attributes = MemberAttributes.Public;

            MyType.Members.Add(Constructor);

            //// Declare component container
            //MyType.Members.Add(new CodeMemberField("System.ComponentModel.IContainer", "components"));



            ////  Implement the Dispose method
            //CodeMemberMethod DisposeMethod = new CodeMemberMethod();

            //DisposeMethod.Name = "Dispose";
            //DisposeMethod.Attributes = MemberAttributes.Family;

            //DisposeMethod.Parameters.Add(
            //    new CodeParameterDeclarationExpression(
            //    typeof(Boolean), "disposing"));

            //CodeConditionStatement Statement = new CodeConditionStatement();
            //Statement.Condition = new CodeArgumentReferenceExpression("disposing");

            //CodeConditionStatement TrueStatement = new CodeConditionStatement();
            //TrueStatement.Condition =
            //  new CodeBinaryOperatorExpression(
            //    new CodeArgumentReferenceExpression("components"),
            //    CodeBinaryOperatorType.IdentityInequality,
            //    new CodePrimitiveExpression(null));

            //TrueStatement.TrueStatements.Add(
            //  new CodeMethodInvokeExpression(
            //    new CodeFieldReferenceExpression(null,
            //        "components"), "Dispose", new CodeExpression() { }));

            //Statement.TrueStatements.Add(TrueStatement);

            //DisposeMethod.Statements.Add(Statement);

            //DisposeMethod.Statements.Add(new CodeMethodInvokeExpression(new CodeBaseReferenceExpression(), "Dispose", new CodeArgumentReferenceExpression[] { new CodeArgumentReferenceExpression("disposing") }));

            //MyType.Members.Add(DisposeMethod);


            //// InitializeComponent
            //CodeMemberMethod InitializeMethod = new CodeMemberMethod();

            //InitializeMethod.Name = "InitializeComponent";
            //InitializeMethod.Attributes = MemberAttributes.Private;

            //InitializeMethod.CustomAttributes.Add(
            //  new CodeAttributeDeclaration(
            //    "System.Diagnostics.DebuggerStepThrough"));

            //InitializeMethod.Statements.Add(
            //  new CodeAssignStatement(
            //    new CodeFieldReferenceExpression(
            //      new CodeThisReferenceExpression(), "components"),
            //        new CodeObjectCreateExpression(
            //          new CodeTypeReference(
            //            typeof(System.ComponentModel.Container)),
            //            new CodeExpression() { })));

            //MyType.Members.Add(InitializeMethod);

            //// Main entry point
            //CodeEntryPointMethod MainMethod = new CodeEntryPointMethod();
            //MainMethod.Name = "Main";
            //MyType.Members.Add(MainMethod);

            ////Add mouse move event
            //CodeMemberEvent eventstate = new CodeMemberEvent();
            //eventstate.Name = "MouseMove";
            //eventstate.Attributes = MemberAttributes.Final | MemberAttributes.Public;
            //eventstate.Type = new CodeTypeReference("System.Windows.Forms.MouseEventHandler");
            //MyType.Members.Add(eventstate);

            string OutputName = "Some.cs";

            try
            {

                CodeGeneratorOptions options = new CodeGeneratorOptions();
                options.BlankLinesBetweenMembers = true;
                options.ElseOnClosing = false;
                options.BracingStyle = "C";

                // This is what we'll write the generated code to
                IndentedTextWriter writer = new IndentedTextWriter(new StreamWriter(OutputName, false), "  ");
                try
                {
                    CodeProvider.GenerateCodeFromCompileUnit(Unit, writer, new CodeGeneratorOptions());
                    writer.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    writer.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }

    // http://stackoverflow.com/q/6618134/751090
    public class StackOverflow_6618134
    {
        public abstract class Foo<T> where T : Foo<T>, new()
        {
            T t;
            public Foo()
                : this(default(T))
            {
            }
            public Foo(T t)
            {
                this.t = t;
            }
            void Test()
            {
                if (Bar != null)
                    Bar(this.t);
            }

            public event Bar<T> Bar;
        }

        public class FooDerived : Foo<FooDerived>
        {
            string str;
            public FooDerived() : base(new FooDerived("Hello")) { }
            public FooDerived(string str)
            {
                this.str = str;
            }
        }

        public delegate void Bar<T>(T foo)
            where T : Foo<T>, new();
    }

    // http://stackoverflow.com/q/6618097/751090
    public class StackOverflow_6618097
    {
        const string XML = @"<buildings>
 <library>
  <book>
      <title>Three Little Pigs</title>
  </book>
  <book>
      <title>Batman</title>
  </book>
  <address>123 Main St.</address>
  <phone>111-111-1111</phone>
  <hidden>
   <book>
    <title>The Hidden Treasure</title>
   </book>
  </hidden>
 </library>
 <bookstore>
  <book>
    <title>Cat in the Hat</title>
  </book>
 </bookstore>
</buildings>";
        public static void Test()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(XML);
            XmlNode libraryNode = doc.SelectSingleNode("//library");
            XmlNodeList libraryBooks = libraryNode.SelectNodes(".//book");
            Console.WriteLine("Books: {0}", libraryBooks.Count);
            foreach (XmlNode node in libraryBooks)
            {
                Console.WriteLine(node.OuterXml);
            }
        }
    }

    public class Post_0424d917_89cd_43c8_be70_5d4c6934b48c
    {
        [ServiceContract]
        public interface ITest
        {
            [WebGet(UriTemplate = "/Echo?text={text}")]
            string EchoGet(string text);
            [WebInvoke(UriTemplate = "/Echo")]
            string EchoPost(string text);
            [WebGet(UriTemplate = "*")]
            Stream ErrorForGet();
            [WebInvoke(UriTemplate = "*")]
            Stream ErrorForPost();
        }
        public class Service : ITest
        {
            public string EchoGet(string text) { return text; }
            public string EchoPost(string text) { return text; }
            public Stream ErrorForPost() { return this.ErrorForGet(); }
            public Stream ErrorForGet()
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.NotFound;
                WebOperationContext.Current.OutgoingResponse.ContentType = "text/html";
                string result = @"<html>
      <head>
        <title>Resource not found</title>
      </head>
      <body>
        <h1>This resource cannot be found</h1>
      </body>
    </html>";
                return new MemoryStream(Encoding.UTF8.GetBytes(result));
            }
        }
        static void SendRequest(string uri, string method, string contentType, string body)
        {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(uri);
            req.Method = method;
            if (!String.IsNullOrEmpty(contentType))
            {
                req.ContentType = contentType;
            }

            if (body != null)
            {
                byte[] bodyBytes = Encoding.UTF8.GetBytes(body);
                req.GetRequestStream().Write(bodyBytes, 0, bodyBytes.Length);
                req.GetRequestStream().Close();
            }

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
            Console.WriteLine(new StreamReader(resp.GetResponseStream()).ReadToEnd());

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

            SendRequest(baseAddress + "/Echo?text=hello", "GET", null, null);
            SendRequest(baseAddress + "/Echo", "POST", "application/json", "\"world\"");

            SendRequest(baseAddress + "/NotFound", "GET", null, null);
            SendRequest(baseAddress + "/NotFound", "POST", "text/xml", "<body/>");

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    // http://stackoverflow.com/q/6630425/751090
    public class StackOverflow_6630425
    {
        [ServiceContract]
        public class Service
        {
            [WebGet]
            public string CollabSortFolder(int FolderId, Dictionary<int, int> Items)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("FolderId=" + FolderId);
                foreach (var key in Items.Keys)
                {
                    sb.AppendLine(string.Format("  Items[{0}] = {1}", key, Items[key]));
                }
                return sb.ToString();
            }
        }
        public class MyQueryStringConverter : QueryStringConverter
        {
            public override bool CanConvert(Type type)
            {
                return type == typeof(Dictionary<int, int>) || base.CanConvert(type);
            }

            public override object ConvertStringToValue(string parameter, Type parameterType)
            {
                if (parameterType == typeof(Dictionary<int, int>))
                {
                    parameter = parameter.Trim().Substring(1, parameter.Length - 2); // trimming the begin and end '{' / '}'
                    string[] pairs = parameter.Split(',');
                    Dictionary<int, int> result = new Dictionary<int, int>();
                    foreach (string pair in pairs)
                    {
                        string[] parts = pair.Split(':');
                        string key = parts[0].Trim();
                        string value = parts[1].Trim();
                        if (key.StartsWith("\"")) key = key.Substring(1);
                        if (key.EndsWith("\"")) key = key.Substring(0, key.Length - 1);
                        result.Add(int.Parse(key), int.Parse(value));
                    }

                    return result;
                }

                return base.ConvertStringToValue(parameter, parameterType);
            }
        }
        class MyWebHttpBehavior : WebHttpBehavior
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
            host.AddServiceEndpoint(typeof(Service), new WebHttpBinding(), "").Behaviors.Add(new MyWebHttpBehavior());
            host.Open();
            Console.WriteLine("Host opened");

            WebClient c = new WebClient();
            Console.WriteLine(c.DownloadString(baseAddress + "/CollabSortFolder?FolderId=12&Items={\"1\":3,\"4\":5,\"6\":7}"));

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }
    // http://stackoverflow.com/q/6681219/751090
    public class StackOverflow_6681219
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            List<List<object>> GetData();
        }
        public class Service : ITest
        {
            public List<List<object>> GetData()
            {
                return new List<List<object>>
                {
                    new List<object>
                    {
                        "PJ123",
                        2.0,
                        "216565",
                        new DateTime(1993, 9, 10),
                        "Timesheet W/E 11/9/93 Franklin",
                        "CONSULT OF",
                        25.0,
                        0.0,
                        25.0
                    }
                };
            }
        }
        class MyEncodingBindingElement : MessageEncodingBindingElement
        {
            MessageEncodingBindingElement inner;
            public MyEncodingBindingElement(MessageEncodingBindingElement inner)
            {
                this.inner = inner;
            }

            public override MessageEncoderFactory CreateMessageEncoderFactory()
            {
                return new MyEncoderFactory(this.inner.CreateMessageEncoderFactory());
            }

            public override MessageVersion MessageVersion
            {
                get { return this.inner.MessageVersion; }
                set { this.inner.MessageVersion = value; }
            }

            public override BindingElement Clone()
            {
                return new MyEncodingBindingElement(this.inner);
            }

            public override bool CanBuildChannelListener<TChannel>(BindingContext context)
            {
                return context.CanBuildInnerChannelListener<TChannel>();
            }

            public override IChannelListener<TChannel> BuildChannelListener<TChannel>(BindingContext context)
            {
                context.BindingParameters.Add(this);
                return context.BuildInnerChannelListener<TChannel>();
            }

            class MyEncoderFactory : MessageEncoderFactory
            {
                MessageEncoderFactory inner;
                public MyEncoderFactory(MessageEncoderFactory inner)
                {
                    this.inner = inner;
                }

                public override MessageEncoder Encoder
                {
                    get { return new MyEncoder(this.inner.Encoder); }
                }

                public override MessageVersion MessageVersion
                {
                    get { return this.inner.MessageVersion; }
                }
            }

            class MyEncoder : MessageEncoder
            {
                MessageEncoder inner;
                public MyEncoder(MessageEncoder inner)
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

                public override bool IsContentTypeSupported(string contentType)
                {
                    return this.inner.IsContentTypeSupported(contentType);
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
                    const string XmlnsNamespace = "http://www.w3.org/2000/xmlns/";
                    const string SchemaNamespace = "http://www.w3.org/2001/XMLSchema";
                    const string ArraysNamespace = "http://schemas.microsoft.com/2003/10/Serialization/Arrays";
                    const string SchemaInstanceNamespace = "http://www.w3.org/2001/XMLSchema-instance";
                    ArraySegment<byte> temp = this.inner.WriteMessage(message, maxMessageSize, bufferManager, messageOffset);
                    MemoryStream ms = new MemoryStream(temp.Array, temp.Offset, temp.Count);
                    XmlDocument doc = new XmlDocument();
                    doc.Load(ms);
                    bufferManager.ReturnBuffer(temp.Array);
                    XmlAttribute rootAttr = doc.CreateAttribute("xmlns", "sch", XmlnsNamespace);
                    rootAttr.Value = SchemaNamespace;
                    doc.DocumentElement.Attributes.Append(rootAttr);
                    XmlNamespaceManager nsManager = new XmlNamespaceManager(doc.NameTable);
                    nsManager.AddNamespace("arrays", ArraysNamespace);
                    foreach (XmlNode arrayItem in doc.SelectNodes("//arrays:anyType", nsManager))
                    {
                        XmlAttribute toRemove = null;
                        XmlAttribute typeAttr = null;
                        foreach (XmlAttribute attr in arrayItem.Attributes)
                        {
                            if (attr.Prefix == "xmlns" && attr.Value == SchemaNamespace)
                            {
                                toRemove = attr;
                            }
                            else if (attr.LocalName == "type" && attr.NamespaceURI == SchemaInstanceNamespace)
                            {
                                typeAttr = attr;
                            }
                        }

                        if (toRemove != null)
                        {
                            arrayItem.Attributes.Remove(toRemove);
                            if (typeAttr != null)
                            {
                                string prefix = toRemove.LocalName;
                                typeAttr.Value = typeAttr.Value.Replace(prefix + ":", "sch:");
                            }
                        }
                    }
                    ms = new MemoryStream();
                    doc.Save(ms);
                    byte[] buffer = bufferManager.TakeBuffer((int)ms.Length + messageOffset);
                    Array.Copy(ms.GetBuffer(), 0, buffer, messageOffset, (int)ms.Length);
                    return new ArraySegment<byte>(buffer, messageOffset, (int)ms.Length);
                }

                public override void WriteMessage(Message message, Stream stream)
                {
                    throw new NotImplementedException();
                }
            }
        }
        static Binding ReplaceEncoding(Binding original)
        {
            CustomBinding custom = new CustomBinding(original);
            for (int i = 0; i < custom.Elements.Count; i++)
            {
                MessageEncodingBindingElement mebe = custom.Elements[i] as MessageEncodingBindingElement;
                if (mebe != null)
                {
                    custom.Elements[i] = new MyEncodingBindingElement(mebe);
                    break;
                }
            }

            return custom;
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            Binding binding = ReplaceEncoding(new BasicHttpBinding());
            host.AddServiceEndpoint(typeof(ITest), binding, "");
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(new BasicHttpBinding(), new EndpointAddress(baseAddress));
            ITest proxy = factory.CreateChannel();
            Console.WriteLine(proxy.GetData());

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_97575d79_2b6c_4cc6_8c8e_450aff5008ea
    {
        [DataContract(Name = "Contact", Namespace = "http://my.namespace")]
        public class Contact
        {
            [DataMember]
            public string Name { get; set; }
            [DataMember]
            public string Email { get; set; }
            [DataMember]
            public string Address { get; set; }
        }

        [DataContract(Name = "Contact2", Namespace = "http://my.namespace")]
        public class Contact2 : Contact
        {
            public new string Address { get; set; }

            public override string ToString()
            {
                return string.Format("Name={0},Email={1},base.Address={2},Address={3}",
                    Name, Email, base.Address ?? "<<null>>", this.Address ?? "<<null>>");
            }
        }

        [DataContract(Name = "Contact", Namespace = "http://my.namespace")]
        public class ClientContact
        {
            [DataMember]
            public string Name { get; set; }
            [DataMember]
            public string Email { get; set; }
        }

        [DataContract(Name = "Contact2", Namespace = "http://my.namespace")]
        public class ClientContact2 : Contact
        {
            public new string Address { get; set; }
        }

        [ServiceContract(Name = "ITest")]
        public interface ITest
        {
            [OperationContract]
            Contact2 Echo(Contact2 input);
        }
        [ServiceContract(Name = "ITest")]
        public interface ITestClient
        {
            [OperationContract]
            ClientContact2 Echo(ClientContact2 input);
        }
        public class Service : ITest
        {
            public Contact2 Echo(Contact2 input)
            {
                Console.WriteLine(input);
                return input;
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

            var factory = new ChannelFactory<ITestClient>(new BasicHttpBinding(), new EndpointAddress(baseAddress));
            var proxy = factory.CreateChannel();
            ClientContact2 contact = new ClientContact2
            {
                Address = "This won't be sent to the server",
                Email = "a@b.c",
                Name = "My name",
            };
            proxy.Echo(contact);

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    // http://stackoverflow.com/q/6685456/751090
    public class StackOverflow_6685456
    {
        [DataContract(Name = "Get{0}Request")]
        public class GetItemRequest<T>
        {
            [DataMember]
            public T Request { get; set; }
        }
        [DataContract(Name = "Get{0}Response")]
        public class GetItemResponse<T>
        {
            [DataMember]
            public T Response { get; set; }
        }
        [DataContract]
        public class Foo
        {
            [DataMember]
            public string Bar { get; set; }
        }
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            GetItemResponse<Foo> Process(GetItemRequest<Foo> input);
        }
        public class Service : ITest
        {
            public GetItemResponse<Foo> Process(GetItemRequest<Foo> input)
            {
                return new GetItemResponse<Foo> { Response = input.Request };
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

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_669fdbf9_f12e_4cc5_8ca2_bc2f45ff20ea
    {
        [ServiceContract]
        public interface IRestMethod
        {
            [OperationContract, WebGet(ResponseFormat = WebMessageFormat.Json)]
            int Add(int x, int y);
        }
        [ServiceContract]
        public interface IAllMethods : IRestMethod
        {
            [OperationContract]
            int Subtract(int x, int y);
            [OperationContract]
            int Multiply(int x, int y);
        }
        public class Service : IRestMethod, IAllMethods
        {
            public int Add(int x, int y)
            {
                return x + y;
            }

            public int Subtract(int x, int y)
            {
                return x - y;
            }

            public int Multiply(int x, int y)
            {
                return x * y;
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(IAllMethods), new BasicHttpBinding(), "soap");
            host.AddServiceEndpoint(typeof(IRestMethod), new WebHttpBinding(), "rest").Behaviors.Add(new WebHttpBehavior());
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<IAllMethods> factory = new ChannelFactory<IAllMethods>(new BasicHttpBinding(), new EndpointAddress(baseAddress + "/soap"));
            IAllMethods proxy = factory.CreateChannel();

            Console.WriteLine("Using SOAP endpoint");
            Console.WriteLine(proxy.Add(3, 4));
            Console.WriteLine(proxy.Subtract(3, 4));
            Console.WriteLine(proxy.Multiply(3, 4));

            Console.WriteLine("Using REST endpoint");
            WebClient c = new WebClient();
            Console.WriteLine(c.DownloadString(baseAddress + "/rest/Add?x=5&y=7"));

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public static class Extensions
    {
        public static IEnumerable<T> ReplaceSequence<T>(this IEnumerable<T> original, IEnumerable<T> toSearch, IEnumerable<T> toReplace) where T : IEquatable<T>
        {
            T[] toSearchItems = toSearch.ToArray();
            List<T> window = new List<T>();
            foreach (T value in original)
            {
                window.Add(value);
                if (window.Count == toSearchItems.Length)
                {
                    bool match = true;
                    for (int i = 0; i < toSearchItems.Length; i++)
                    {
                        if (!toSearchItems[i].Equals(window[i]))
                        {
                            match = false;
                            break;
                        }
                    }

                    if (match)
                    {
                        foreach (T toReplaceValue in toReplace)
                        {
                            yield return toReplaceValue;
                        }

                        window.Clear();
                    }
                    else
                    {
                        yield return window[0];
                        window.RemoveAt(0);
                    }
                }
            }

            foreach (T value in window)
            {
                yield return value;
            }
        }
    }
    // http://stackoverflow.com/q/6751533/751090
    public class StackOverflow_6751533
    {
        public static void Test()
        {
            byte[] byteArray = new byte[] { 0x01, 0x02, 0x7E, 0x7E, 0x04 };
            byte[] escapeSequence = new byte[] { 0x7E, 0x7E };
            byte[] unescapedSequence = new byte[] { 0x7E };
            byte[] outputBytes = byteArray.ReplaceSequence(escapeSequence, unescapedSequence).ToArray();
            for (int i = 0; i < outputBytes.Length; i++)
            {
                Console.Write("{0:X2} ", (int)outputBytes[i]);
            }
            Console.WriteLine();
        }
    }

    // http://stackoverflow.com/questions/6755014/how-to-cdata-property
    public class StackOverflow_6755014
    {
        public class MyRegex
        {
            public string Regex { get; set; }
        }
        public static class SerializerHelper<T>
        {
            public static string Serialize(T myobject)
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                StringWriter stringWriter = new StringWriter();
                xmlSerializer.Serialize(stringWriter, myobject);
                string xml = stringWriter.ToString();

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xml);
                StringWriter sw = new StringWriter();
                XmlTextWriter xw = new XmlTextWriter(sw);
                xmlDoc.WriteTo(xw);

                return sw.ToString();
            }
            public static T DeSerialize(string xml)
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                StringReader stringReader = new StringReader(xml);
                return (T)xmlSerializer.Deserialize(stringReader);
            }
        }
        public static void Test()
        {
            MyRegex original = new MyRegex { Regex = "\\b[1-3]{1}\\b#Must be a value of 1 to 3" };
            string xml = SerializerHelper<MyRegex>.Serialize(original);
            Console.WriteLine("--- SERIALIZE ---");
            Console.WriteLine(xml);
            Console.WriteLine();
            Console.WriteLine();


            Console.WriteLine("--- DESERIALIZE ---");
            MyRegex deSerial = SerializerHelper<MyRegex>.DeSerialize(xml);
            Console.WriteLine("Equals? " + (deSerial.Regex.Equals(original.Regex)));

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Console.ReadKey();");
            Console.ReadKey();
        }
    }

    // http://stackoverflow.com/q/6747339/751090
    public class StackOverflow_6747339
    {
        class FacetsAsSiblingOfHints
        {
            [DataContract]
            public class AutoCompleteResponse
            {
                [DataMember(Name = "parameters")]
                public Parameter Parameters { get; set; }
                [DataMember(Name = "termHints")]
                public List<TermHints> Hints { get; set; }
                [DataMember(Name = "facets")]
                public List<Facets> Facets { get; set; }
                [DataMember(Name = "total")]
                public string Total { get; set; }

                public override string ToString()
                {
                    return string.Format("AutoCompleteResponse[Parameters={0},hints={1},facets={2},total={3}]",
                        Parameters, ListToString(Hints), ListToString(Facets), Total);
                }
            }
            [DataContract]
            public class Parameter
            {
                [DataMember(Name = "tbdb")]
                public string tbdb { get; set; }
                [DataMember(Name = "min_prefix_length")]
                public string min_prefix_length { get; set; }
                [DataMember(Name = "service")]
                public string service { get; set; }
                [DataMember(Name = "template")]
                public string template { get; set; }
                [DataMember(Name = "term_prefix")]
                public string term_prefrix { get; set; }

                public override string ToString()
                {
                    return string.Format("Parameter[tbdb={0},min_prefix_length={1},service={2},template={3},term_prefix={4}]",
                        tbdb, min_prefix_length, service, template, term_prefrix);
                }
            }
            [DataContract]
            public class TermHints
            {
                [DataMember(Name = "name")]
                public string Name { get; set; }
                [DataMember(Name = "id")]
                public string Id { get; set; }
                [DataMember(Name = "values")]
                public Values Values { get; set; }

                public override string ToString()
                {
                    return string.Format("TermHints[Name={0},Id={1},Values={2}]", Name, Id, Values);
                }
            }
            [DataContract]
            public class Values
            {
                [DataMember(Name = "value")]
                public string value_name { get; set; }
                [DataMember(Name = "pre_em")]
                public string pre_em { get; set; }
                [DataMember(Name = "em")]
                public string em { get; set; }
                [DataMember(Name = "post_em")]
                public string post_em { get; set; }
                [DataMember(Name = "nature")]
                public string nature { get; set; }
                [DataMember(Name = "id")]
                public string value_id { get; set; }

                public override string ToString()
                {
                    return string.Format("Values[value_name={0},pre_em={1},em={2},post_em={3},nature={4},value_id={5}]",
                        value_name, pre_em, em, post_em, nature, value_id);
                }
            }
            [DataContract]
            public class Facets
            {
                [DataMember(Name = "id")]
                public string facet_id { get; set; }
                [DataMember(Name = "name")]
                public string facet_name { get; set; }

                public override string ToString()
                {
                    return string.Format("Facets[facet_id={0},facet_name={1}]", facet_id, facet_name);
                }
            }

            const string json = @"{
    ""parameters"": {
        ""tbdb"": ""trudon"",
        ""min_prefix_length"": ""2"",
        ""service"": ""prefix"",
        ""template"": ""service.json"",
        ""term_prefix"": ""plu""},
    ""termHints"": [
        {
            ""name"": ""Plumbers & Sanitary Engineers"",
            ""id"":""209654"",
            ""values"": {
                ""value"":""Plumbers & Sanitary Engineers"",
                ""pre_em"":"""",
                ""em"":""Plu"",
                ""post_em"":""mbers & Sanitary Engineers"",
                ""nature"":""PT"",
                ""id"":""209654""
            }
        },
    ],
    ""facets"": [
        {
            ""id"":""209654"",
            ""name"":""Plumbers & Sanitary Engineers""
        }
    ],
    ""total"":1
}";

            internal static void Test()
            {
                Console.WriteLine("Facets as siblings of the hints");
                DataContractJsonSerializer dcjs = new DataContractJsonSerializer(typeof(AutoCompleteResponse));
                MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(json));
                AutoCompleteResponse obj = (AutoCompleteResponse)dcjs.ReadObject(ms);
                ms.Close();
                Console.WriteLine(obj);
                Console.WriteLine();
            }
        }

        class FacetsInsideHints
        {
            [DataContract]
            public class AutoCompleteResponse
            {
                [DataMember(Name = "parameters")]
                public Parameter Parameters { get; set; }
                [DataMember(Name = "termHints")]
                public List<TermHints> Hints { get; set; }
                [DataMember(Name = "total")]
                public string Total { get; set; }

                public override string ToString()
                {
                    return string.Format("AutoCompleteResponse[Parameters={0},hints={1},total={2}]",
                        Parameters, ListToString(Hints), Total);
                }
            }
            [DataContract]
            public class Parameter
            {
                [DataMember(Name = "tbdb")]
                public string tbdb { get; set; }
                [DataMember(Name = "min_prefix_length")]
                public string min_prefix_length { get; set; }
                [DataMember(Name = "service")]
                public string service { get; set; }
                [DataMember(Name = "template")]
                public string template { get; set; }
                [DataMember(Name = "term_prefix")]
                public string term_prefrix { get; set; }

                public override string ToString()
                {
                    return string.Format("Parameter[tbdb={0},min_prefix_length={1},service={2},template={3},term_prefix={4}]",
                        tbdb, min_prefix_length, service, template, term_prefrix);
                }
            }
            [DataContract]
            public class TermHints
            {
                [DataMember(Name = "name")]
                public string Name { get; set; }
                [DataMember(Name = "id")]
                public string Id { get; set; }
                [DataMember(Name = "values")]
                public Values Values { get; set; }
                [DataMember(Name = "facets")]
                public List<Facets> Facets { get; set; }

                public override string ToString()
                {
                    return string.Format("TermHints[Name={0},Id={1},Values={2},Facets={3}]", Name, Id, Values, ListToString(Facets));
                }
            }
            [DataContract]
            public class Values
            {
                [DataMember(Name = "value")]
                public string value_name { get; set; }
                [DataMember(Name = "pre_em")]
                public string pre_em { get; set; }
                [DataMember(Name = "em")]
                public string em { get; set; }
                [DataMember(Name = "post_em")]
                public string post_em { get; set; }
                [DataMember(Name = "nature")]
                public string nature { get; set; }
                [DataMember(Name = "id")]
                public string value_id { get; set; }

                public override string ToString()
                {
                    return string.Format("Values[value_name={0},pre_em={1},em={2},post_em={3},nature={4},value_id={5}]",
                        value_name, pre_em, em, post_em, nature, value_id);
                }
            }
            [DataContract]
            public class Facets
            {
                [DataMember(Name = "id")]
                public string facet_id { get; set; }
                [DataMember(Name = "name")]
                public string facet_name { get; set; }

                public override string ToString()
                {
                    return string.Format("Facets[facet_id={0},facet_name={1}]", facet_id, facet_name);
                }
            }

            const string json = @"{
    ""parameters"": {
        ""tbdb"": ""trudon"",
        ""min_prefix_length"": ""2"",
        ""service"": ""prefix"",
        ""template"": ""service.json"",
        ""term_prefix"": ""plu""},
    ""termHints"": [
        {
            ""name"": ""Plumbers & Sanitary Engineers"",
            ""id"":""209654"",
            ""values"": {
                ""value"":""Plumbers & Sanitary Engineers"",
                ""pre_em"":"""",
                ""em"":""Plu"",
                ""post_em"":""mbers & Sanitary Engineers"",
                ""nature"":""PT"",
                ""id"":""209654""
            },
            ""facets"": [
                {
                    ""id"":""209654"",
                    ""name"":""Plumbers & Sanitary Engineers""
                }
            ]
        },
    ],
    ""total"":1
}";

            internal static void Test()
            {
                Console.WriteLine("Facets inside the hints");
                DataContractJsonSerializer dcjs = new DataContractJsonSerializer(typeof(AutoCompleteResponse));
                MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(json));
                AutoCompleteResponse obj = (AutoCompleteResponse)dcjs.ReadObject(ms);
                ms.Close();
                Console.WriteLine(obj);
                Console.WriteLine();
            }
        }

        static string ListToString<T>(List<T> list)
        {
            if (list == null)
            {
                return "<<null>>";
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            for (int i = 0; i < list.Count; i++)
            {
                if (i > 0) sb.Append(",");
                sb.Append(list[i]);
            }

            sb.Append("]");
            return sb.ToString();
        }

        public static void Test()
        {
            FacetsAsSiblingOfHints.Test();
            FacetsInsideHints.Test();
        }
    }

    // http://stackoverflow.com/q/6780831/751090
    public class StackOverflow_6780831
    {
        [DataContract(Name = "Order")]
        public class Order
        {
            [DataMember(Order = 1)]
            public int Id;
            [DataMember(Order = 2)]
            public List<object> List;
            [DataMember(Order = 3)]
            public object Data;
        }
        public static void Test()
        {
            MemoryStream ms = new MemoryStream();
            Order order = new Order
            {
                Id = 1,
                List = new List<object>
                {
                    1, "some string", DateTime.Now
                },
                Data = "Some other data",
            };
            DataContractSerializer dcs = new DataContractSerializer(typeof(Order));
            XmlWriter w = XmlWriter.Create(ms, new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  ",
                OmitXmlDeclaration = true
            });
            dcs.WriteObject(w, order);
            w.Flush();
            Console.WriteLine(Encoding.UTF8.GetString(ms.ToArray()));
        }
    }

    // http://stackoverflow.com/q/6823640/751090
    public class StackOverflow_6823640
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
            WebServiceHost host = new WebServiceHost(typeof(Service), new Uri(baseAddress));
            host.Open();
            Console.WriteLine("Host opened");

            WebChannelFactory<ITest> factory = new WebChannelFactory<ITest>(new Uri(baseAddress));
            ITest proxy = factory.CreateChannel();
            Console.WriteLine(proxy.Echo("Hello"));

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_91197a62_7d30_4f79_9ffc_8bc67897d221
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
            return new WSHttpBinding();
        }
        static Binding GetBindingReplacingEncoding(Binding original)
        {
            CustomBinding custom = new CustomBinding(original);
            for (int i = 0; i < custom.Elements.Count; i++)
            {
                MessageEncodingBindingElement mebe = custom.Elements[i] as MessageEncodingBindingElement;
                if (mebe != null)
                {
                    if (mebe is BinaryMessageEncodingBindingElement)
                    {
                        custom.Elements[i] = new TextMessageEncodingBindingElement();
                    }
                    else
                    {
                        custom.Elements[i] = new BinaryMessageEncodingBindingElement();
                    }
                    break;
                }
            }

            return custom;
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            Binding binding = GetBinding();
            host.AddServiceEndpoint(typeof(ITest), binding, "ep1");
            Binding otherBinding = GetBindingReplacingEncoding(binding);
            host.AddServiceEndpoint(typeof(ITest), otherBinding, "ep2");
            host.Description.Behaviors.Add(new ServiceMetadataBehavior { HttpGetEnabled = true });
            host.Open();
            Console.WriteLine("Host opened - run svcutil / add service reference to create the bindings");

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class StackOverflow_6846012
    {
        class MyWriter : XmlWriter
        {
            XmlWriter inner;
            public MyWriter(XmlWriter inner)
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
            XmlWriter myWriter = new MyWriter(XmlDictionaryWriter.CreateTextWriter(ms));
            dcs.WriteObject(myWriter, list);
            myWriter.Flush();
            Console.WriteLine(Encoding.UTF8.GetString(ms.ToArray()));
        }
    }

    // http://stackoverflow.com/q/6846215/751090
    public class StackOverflow_6846215
    {
        [ServiceContract(Name = "ITest")]
        public interface ITest
        {
            [OperationContract]
            [WebGet]
            int Add(int x, int y);
        }
        [ServiceContract(Name = "ITest")]
        public interface ITestClient
        {
            [OperationContract(AsyncPattern = true)]
            IAsyncResult BeginAdd(int x, int y, AsyncCallback callback, object state);
            int EndAdd(IAsyncResult asyncResult);

            [OperationContract]
            [WebGet]
            int Add(int x, int y);
        }
        public class Client : ClientBase<ITestClient>, ITestClient
        {
            public Client(string baseAddress)
                : base(new WebHttpBinding(), new EndpointAddress(baseAddress))
            {
                this.Endpoint.Behaviors.Add(new WebHttpBehavior());
            }

            public IAsyncResult BeginAdd(int x, int y, AsyncCallback callback, object state)
            {
                return this.Channel.BeginAdd(x, y, callback, state);
            }

            public int EndAdd(IAsyncResult asyncResult)
            {
                return this.Channel.EndAdd(asyncResult);
            }

            public int Add(int x, int y)
            {
                return this.Channel.Add(x, y);
            }
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
            WebServiceHost host = new WebServiceHost(typeof(Service), new Uri(baseAddress));
            host.Open();
            Console.WriteLine("Host opened");

            Client client = new Client(baseAddress);
            Console.WriteLine("Sync result: {0}", client.Add(66, 77));
            client.BeginAdd(44, 55, delegate(IAsyncResult ar)
            {
                int result = client.EndAdd(ar);
                Console.WriteLine("Async result: {0}", result);
            }, null);

            Console.WriteLine("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    // http://stackoverflow.com/q/6850159/751090
    public class StackOverflow_6850159
    {
        public class Person
        {
            [XmlAttribute]
            public string Name;
            [XmlAttribute, XmlIgnore]
            public int Age;
        }
        public static void Test()
        {
            XmlSerializer xs = new XmlSerializer(typeof(Person));
            MemoryStream ms = new MemoryStream();
            xs.Serialize(ms, new Person { Name = "John Doe", Age = 23 });
            Console.WriteLine(Encoding.UTF8.GetString(ms.ToArray()));
        }
    }

    public class Post_54770c31_51df_4ba2_bf7a_56fe2b76834c
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
                var remoteAddress = OperationContext.Current.Channel.RemoteAddress;
                Console.WriteLine(remoteAddress == null ? "<<null>>" : remoteAddress.Uri.ToString());
                RemoteEndpointMessageProperty remoteEndpointProperty;
                remoteEndpointProperty = (RemoteEndpointMessageProperty)OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name];
                Console.WriteLine("Remote endpoint: {0}, port {1}", remoteEndpointProperty.Address, remoteEndpointProperty.Port);
                return text;
            }
        }
        public static void Test()
        {
            string baseAddressHttp = "http://" + Environment.MachineName + ":8000/Service";
            string baseAddressTcp = "net.tcp://" + Environment.MachineName + ":8008/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddressHttp), new Uri(baseAddressTcp));
            host.AddServiceEndpoint(typeof(ITest), new BasicHttpBinding(), "basic");
            host.AddServiceEndpoint(typeof(ITest), new WSHttpBinding(), "ws");
            host.AddServiceEndpoint(typeof(ITest), new NetTcpBinding(), "nettcp");
            host.Open();
            Console.WriteLine("Host opened");

            Dictionary<string, Binding> eps = new Dictionary<string, Binding>
            {
                { baseAddressHttp + "/basic", new BasicHttpBinding() },
                { baseAddressHttp + "/ws", new WSHttpBinding() },
                { baseAddressTcp + "/nettcp", new NetTcpBinding() }
            };

            foreach (var address in eps.Keys)
            {
                Console.WriteLine(address);
                Binding binding = eps[address];
                ChannelFactory<ITest> factory = new ChannelFactory<ITest>(binding, new EndpointAddress(address));
                ITest proxy = factory.CreateChannel();
                Console.WriteLine(proxy.Echo("Hello"));
                Console.WriteLine();

                ((IClientChannel)proxy).Close();
                factory.Close();
            }

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    // http://stackoverflow.com/q/6915554/751090
    public class StackOverflow_6915554
    {
        [DataContract]
        [KnownType(typeof(LeafExpression))]
        [KnownType(typeof(BinaryExpression))]
        public class Expression
        {
        }

        [DataContract]
        public class LeafExpression : Expression
        {
            [DataMember]
            public string Name;
        }

        [DataContract]
        public class BinaryExpression : Expression
        {
            [DataMember]
            public BinaryOperator Operator;
            [DataMember]
            public Expression Left;
            [DataMember]
            public Expression Right;
        }

        public enum BinaryOperator
        {
            And,
            Or,
        }

        public static void Test()
        {
            List<Expression> expressions = new List<Expression>();
            expressions.Add(new BinaryExpression
            {
                Left = new BinaryExpression
                {
                    Left = new LeafExpression { Name = "A" },
                    Operator = BinaryOperator.And,
                    Right = new LeafExpression { Name = "B" },
                },
                Operator = BinaryOperator.Or,
                Right = new BinaryExpression
                {
                    Left = new LeafExpression { Name = "C" },
                    Operator = BinaryOperator.And,
                    Right = new LeafExpression { Name = "D" },
                }
            });

            expressions.Add(new BinaryExpression
            {
                Left = new LeafExpression { Name = "E" },
                Operator = BinaryOperator.Or,
                Right = new LeafExpression { Name = "F" }
            });

            expressions.Add(new LeafExpression { Name = "G" });

            XmlWriterSettings ws = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  ",
                Encoding = new UTF8Encoding(false),
                OmitXmlDeclaration = true,
            };

            MemoryStream ms = new MemoryStream();
            XmlWriter w = XmlWriter.Create(ms, ws);

            DataContractSerializer dcs = new DataContractSerializer(typeof(List<Expression>));
            dcs.WriteObject(w, expressions);
            w.Flush();

            Console.WriteLine(Encoding.UTF8.GetString(ms.ToArray()));
        }
    }

    // http://stackoverflow.com/q/6932356/751090
    public class StackOverflow_6932356
    {
        [ServiceContract]
        public interface IWorker
        {
            [OperationContract]
            void Process(XmlElement data);
            [OperationContract]
            void Update(Rule rule);
        }

        [DataContract]
        public class Rule
        {
            [DataMember]
            public string Expression { get; set; }
            [DataMember]
            public List<IAction> Actions { get; set; }
        }

        public interface IAction
        {
            void Execute(XmlElement data);
        }

        public class Service : IWorker
        {
            static List<IAction> AllActions = new List<IAction>();
            public void Process(XmlElement data)
            {
                foreach (var action in AllActions)
                {
                    action.Execute(data);
                }
            }

            public void Update(Rule rule)
            {
                AllActions = rule.Actions;
            }
        }

        public class Action1 : IAction
        {
            public void Execute(XmlElement data)
            {
                Console.WriteLine("Executing {0} for data: {1}", this.GetType().Name, data.OuterXml);
            }
        }

        public class Action2 : IAction
        {
            public void Execute(XmlElement data)
            {
                Console.WriteLine("Executing {0} for data: {1}", this.GetType().Name, data.OuterXml);
            }
        }

        class NetDataContractSerializerOperationBehavior : DataContractSerializerOperationBehavior
        {
            public NetDataContractSerializerOperationBehavior(OperationDescription operationDescription)
                : base(operationDescription) { }

            public override XmlObjectSerializer CreateSerializer(Type type, string name, string ns, IList<Type> knownTypes)
            {
                return new NetDataContractSerializer(name, ns);
            }

            public override XmlObjectSerializer CreateSerializer(Type type, XmlDictionaryString name, XmlDictionaryString ns, IList<Type> knownTypes)
            {
                return new NetDataContractSerializer(name, ns);
            }
        }

        static void ReplaceDCSOB(ServiceEndpoint endpoint)
        {
            foreach (var operation in endpoint.Contract.Operations)
            {
                for (int i = 0; i < operation.Behaviors.Count; i++)
                {
                    if (operation.Behaviors[i] is DataContractSerializerOperationBehavior)
                    {
                        operation.Behaviors[i] = new NetDataContractSerializerOperationBehavior(operation);
                        break;
                    }
                }
            }
        }

        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            ServiceEndpoint endpoint = host.AddServiceEndpoint(typeof(IWorker), new BasicHttpBinding(), "");
            ReplaceDCSOB(endpoint);
            host.Open();
            Console.WriteLine("Host opened");

            var factory = new ChannelFactory<IWorker>(new BasicHttpBinding(), new EndpointAddress(baseAddress));
            ReplaceDCSOB(factory.Endpoint);
            var proxy = factory.CreateChannel();

            proxy.Update(new Rule
            {
                Expression = "Expr",
                Actions = new List<IAction> { new Action1(), new Action2() }
            });

            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<root><foo>bar</foo></root>");
            proxy.Process(doc.DocumentElement);

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_13b7040a_8665_4558_9310_5f71db410992
    {
        [XmlType(TypeName = "foo", Namespace = "")]
        public class Foo
        {
            [XmlText]
            public string text;
        }
        [XmlRoot(ElementName = "foos", Namespace = "")]
        public class MultipleFoo : List<Foo>
        {
            [XmlAttribute(AttributeName = "att")]
            public string att;
        }
        [ServiceContract]
        [XmlSerializerFormat]
        public class Service
        {
            [WebGet]
            public MultipleFoo GetData()
            {
                MultipleFoo result = new MultipleFoo();
                result.att = "qux";
                result.Add(new Foo { text = "blargh" });
                result.Add(new Foo { text = "baz" });
                return result;
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

    // http://stackoverflow.com/q/6948587/751090
    class StackOverflow_6948587
    {
        public class OtherClass
        {
            public int Data { get; set; }
        }
        public class MyClass
        {
            public List<OtherClass> OtherClassList { get; set; }
        }
        public class Model
        {
            public List<MyClass> MyClassList { get; set; }
            public IEnumerable<OtherClass> Union()
            {
                return MyClassList.SelectMany(x => x.OtherClassList).Distinct();
            }
        }
        public static void Test()
        {
            OtherClass[] instances = Enumerable.Range(0, 10).Select(i => new OtherClass { Data = i }).ToArray();
            MyClass c1 = new MyClass
            {
                OtherClassList = instances.Take(4).ToList()
            };
            MyClass c2 = new MyClass
            {
                OtherClassList = instances.Skip(2).Take(4).ToList()
            };
            MyClass c3 = new MyClass
            {
                OtherClassList = instances.Skip(4).Take(4).ToList()
            };
            Model model = new Model
            {
                MyClassList = new List<MyClass> { c1, c2, c3 }
            };

            foreach (var o in model.Union())
            {
                Console.WriteLine(o.Data);
            }
        }
    }

    public class Post_fc8cbad8_a8fc_46d6_b15f_5ae712ca56cd
    {
        public class Order { }
        public class Client { }
        [ServiceContract]
        public interface ITest
        {
            [WebInvoke(UriTemplate = "/ProcessOrder")]
            void ProcessOrder(Stream input);
            [WebInvoke(UriTemplate = "/AddClient")]
            void AddClient(Stream input);
        }
        public class Service : ITest
        {
            public void ProcessOrder(Stream input)
            {
                Order order = ParseOrder(input);
            }
            public void AddClient(Stream input)
            {
                Client client = ParseClient(input);
            }
            Order ParseOrder(Stream data) { return null; }
            Client ParseClient(Stream data) { return null; }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            WebServiceHost host = new WebServiceHost(typeof(Service), new Uri(baseAddress));
            host.Open();
            Console.WriteLine("Host opened");

            WebClient c = new WebClient();
            c.Headers[HttpRequestHeader.ContentType] = "application/octet-stream";
            byte[] order = new byte[100];
            Random rndGen = new Random();
            rndGen.NextBytes(order);
            c.UploadData(baseAddress + "/ProcessOrder", order);

            c = new WebClient();
            c.Headers[HttpRequestHeader.ContentType] = "application/x-clients";
            byte[] client = new byte[1000];
            rndGen.NextBytes(client);
            c.UploadData(baseAddress + "/AddClient", client);

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    // http://stackoverflow.com/q/6959064/751090
    public class StackOverflow_6959064
    {
        const string json = "{\"byte\":[-119,80,78,71,13,10,26,10,0,0,0,13,73,72,68,82,0,0,1,117,0,0,2,-38,8,2,0,0,0,65,61,84,57,0,0,0,1,115,82,71,66,0,-82,-50,28,-23,0,0,0,4,103,65,77,65,0,0,-79,-113,11,-4,97,5,0,0,0,9,112,72,89,115,0,0,14,-61,0,0,14,-61,1,-57,111,-88,100,0,0,-1,-91,73,68,65,84,120,94,-20,-3,9,-101,36,73,114,28,-120,-10,-1,-1,41,-17,45,-33,114,111,-18,46,120,-127,23,72,-128,32,64,-128,-72,65,-100,3,98,-114,-66,-22,62,50,50,-97,-88,-118,-102,-104,-102,-102,-7,17,-111,89,-43,-43,61,57,-97,127,61,81,-111,30,17,30,30,110,-30,-94,-94,-94,-86,95,61,60,-1,-17,-7,12,60,-97,-127,-25,51,-16,105,-50,-64,87,-97,-26,109,-97,-33,-11,-7,12,60,-97,-127,-25,51,-16,-16,-116,47,-49,23,-63,-13,25,120,62,3,-97,-22,12,60,-29,-53,-89,58,-77,-49,-17,-5,124,6,-98,-49,-64,-45,-32,-53,-3,39,-8,-33,-13,111,-13,124,6,-98,-49,-64,-113,-3,12,124,-95,-8,-14,99,63,-83,-49,-57,-1,124,6,-98,-49,0,-50,-64,51,-66,60,95,6,-49,103,-32,-7,12,124,-86,51,112,35,-66,60,58,30,122,-72,-65,-57,118,-10,127,-97,-22,-37,63,-65,-17,-13,25,120,62,3,-97,-14,12,124,90,124,-71,-36,63,-36,-78,77,-56,-13,41,-49,-64,-13,123,63,-97,-127,-25,51,-16,-87,-50,-64,89,124,-39,103,26,-105,-121,-5,-11,118,27,-66,-100,-90,54,-97,-22,-84,60,-65,-17,-13,25,120,62,3,79,113,6,42,-66,-100,-115,88,-58,-3,62,1,-66,92,17,61,-31,88,-98,-30,84,60,-65,-57,-13,25,120,62,3,79,124,6,14,-8,-53,33,-36,108,34,11,25,-51,-83,-4,5,47,116,-127,-26,10,-108,-31,-119,-31,1,63,-15,73,122,126,-69,-25,51,-16,124,6,110,58,3,107,124,-39,-126,21,95,-64,125,19,124,-28,39,-105,59,-36,6,52,122,-85,-83,-105,-73,-29,-84,72,116,-45,-87,120,126,-47,-13,25,120,62,3,79,124,6,-74,-30,-93,1,71,62,17,124,-36,6,58,-7,85,36,43,105,11,-64,121,-30,-109,-12,-4,118,-49,103,-32,-7,12,-36,116,6,58,-66,36,-50,114,12,46,-113,-121,-122,39,121,-121,9,-8,-98,-15,-27,-90,-85,-32,-7,69,-49,103,-32,-45,-100,-127,-81,-90,80,-56,-64,-27,-38,-59,-1,100,4,39,-25,-95,78,28,-58,-4,-71,-97,-26,44,61,-65,-21,-13,25,120,62,3,-73,-100,-127,-118,47,-95,-41,-34,63,-36,93,-42,-37,-75,-48,-13,73,-9,47,7,-7,44,-20,-34,114,9,60,-65,-26,-7,12,124,-78,51,0,124,25,114,52,-124,3,-14,-126,79,10,13,79,-5,-26,60,-32,-25,-1,61,-97,-127,-25,51,-16,69,-99,1,-30,-53,-26,-10,-76,40,-16,41,-34,77,105,-20,47,-22,-76,62,31,-52,-13,25,120,62,3,56,3,29,95,46,-105,7,110,5,110,62,5,40,60,-31,123,-110,127,-99,-4,45,-97,-35,49,39,79,-44,-13,110,-49,103,-32,73,-50,-64,87,92,-22,89,-56,-104,-23,-52,19,-62,-63,-45,-66,-43,85,-32,-126,-13,101,-32,-7,108,-64,123,-110,11,-25,-7,77,-98,-49,-64,-119,51,96,-8,-110,-63,-91,-84,-1,67,-121,-37,-45,-30,-59,-75,-17,86,-104,-117,114,97,91,95,92,95,-106,64,115,-30,-4,60,-17,-14,124,6,-98,-49,-64,-19,103,32,-16,5,97,-47,22,-54,124,-55,90,111,-2,-34,67,-94,29,62,-29,-43,-10,-31,-2,1,27,-65,-87,-53,-40,-49,16,115,-5,-91,-13,-4,-54,-25,51,112,120,6,-66,-78,-107,-26,-101,-16,37,47,66,-3,-43,-10,-39,-56,40,113,-83,-2,32,27,-66,30,97,37,85,42,68,-4,-77,-125,47,-126,-104,107,-61,-85,-61,-77,-7,-68,-61,-13,25,120,62,3,-7,12,24,-66,-28,-115,-32,82,33,-122,-16,-15,-72,122,-59,-49,6,64,-8,58,-122,56,-98,-80,-26,102,48,-28,-30,11,-66,-41,-5,-53,-67,0,-15,89,-117,121,94,12,-49,103,-32,-109,-98,-127,-97,62,-66,-24,-12,17,95,-58,-8,-24,57,68,-6,-92,87,-41,-13,-101,-1,-70,-97,-127,-118,47,-72,-73,23,-14,-46,-61,-97,31,21,127,-55,-28,-123,63,114,-63,23,-89,57,-49,-3,28,126,-35,23,-64,-13,-9,-1,-92,103,-32,10,124,-55,50,-57,103,11,118,110,-8,32,-58,71,-118,-116,102,-2,34,10,19,123,62,-85,-68,-97,-12,18,123,126,-13,95,-29,51,-80,-64,-105,-71,-14,-88,21,13,60,-86,95,-44,13,72,113,-37,75,-126,112,-115,5,3,20,-113,32,-66,20,117,-23,-41,-8,-89,127,-2,-22,-49,103,-32,-109,-97,-127,-97,46,-66,32,-10,73,103,-113,-50,-105,-73,-105,75,-58,23,60,-7,-4,-65,-25,51,-16,124,6,62,-35,25,-8,105,-30,11,41,88,72,48,30,43,1,74,62,-34,-35,23,-2,-30,-69,60,-1,-17,-7,12,60,-97,-127,79,117,6,-10,-16,69,-90,-104,86,81,-3,35,-120,-113,-122,66]}";
        [ServiceContract]
        public class Service
        {
            [OperationContract]
            [WebInvoke(ResponseFormat = WebMessageFormat.Json,
                RequestFormat = WebMessageFormat.Json,
                UriTemplate = "images/tag=add",
                Method = "POST",
                BodyStyle = WebMessageBodyStyle.Wrapped)]
            public bool UploadImage(sbyte[] @byte)
            {
                Console.WriteLine("Length: {0}", @byte.Length);
                return true;
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            WebServiceHost host = new WebServiceHost(typeof(Service), new Uri(baseAddress));
            host.Open();
            Console.WriteLine("Host opened");

            WebClient c = new WebClient();
            c.Headers[HttpRequestHeader.ContentType] = "application/json";
            Console.WriteLine(c.UploadString(baseAddress + "/images/tag=add", json));

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    // http://stackoverflow.com/q/6986346/751090
    public class StackOverflow_6986346
    {
        #region Code from the sample, verbatim
        // This is constants for GZip message encoding policy.
        static class GZipMessageEncodingPolicyConstants
        {
            public const string GZipEncodingName = "GZipEncoding";
            public const string GZipEncodingNamespace = "http://schemas.microsoft.com/ws/06/2004/mspolicy/netgzip1";
            public const string GZipEncodingPrefix = "gzip";
        }

        //This is the binding element that, when plugged into a custom binding, will enable the GZip encoder
        public sealed class GZipMessageEncodingBindingElement
                            : MessageEncodingBindingElement //BindingElement
                            , IPolicyExportExtension
        {

            //We will use an inner binding element to store information required for the inner encoder
            MessageEncodingBindingElement innerBindingElement;

            //By default, use the default text encoder as the inner encoder
            public GZipMessageEncodingBindingElement()
                : this(new TextMessageEncodingBindingElement()) { }

            public GZipMessageEncodingBindingElement(MessageEncodingBindingElement messageEncoderBindingElement)
            {
                this.innerBindingElement = messageEncoderBindingElement;
            }

            public MessageEncodingBindingElement InnerMessageEncodingBindingElement
            {
                get { return innerBindingElement; }
                set { innerBindingElement = value; }
            }

            //Main entry point into the encoder binding element. Called by WCF to get the factory that will create the
            //message encoder
            public override MessageEncoderFactory CreateMessageEncoderFactory()
            {
                return new GZipMessageEncoderFactory(innerBindingElement.CreateMessageEncoderFactory());
            }

            public override MessageVersion MessageVersion
            {
                get { return innerBindingElement.MessageVersion; }
                set { innerBindingElement.MessageVersion = value; }
            }

            public override BindingElement Clone()
            {
                return new GZipMessageEncodingBindingElement(this.innerBindingElement);
            }

            public override T GetProperty<T>(BindingContext context)
            {
                if (typeof(T) == typeof(XmlDictionaryReaderQuotas))
                {
                    return innerBindingElement.GetProperty<T>(context);
                }
                else
                {
                    return base.GetProperty<T>(context);
                }
            }

            public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
            {
                if (context == null)
                    throw new ArgumentNullException("context");

                context.BindingParameters.Add(this);
                return context.BuildInnerChannelFactory<TChannel>();
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

            void IPolicyExportExtension.ExportPolicy(MetadataExporter exporter, PolicyConversionContext policyContext)
            {
                if (policyContext == null)
                {
                    throw new ArgumentNullException("policyContext");
                }
                XmlDocument document = new XmlDocument();
                policyContext.GetBindingAssertions().Add(document.CreateElement(
                    GZipMessageEncodingPolicyConstants.GZipEncodingPrefix,
                    GZipMessageEncodingPolicyConstants.GZipEncodingName,
                    GZipMessageEncodingPolicyConstants.GZipEncodingNamespace));
            }
        }
        //This class is used to create the custom encoder (GZipMessageEncoder)
        internal class GZipMessageEncoderFactory : MessageEncoderFactory
        {
            MessageEncoder encoder;

            //The GZip encoder wraps an inner encoder
            //We require a factory to be passed in that will create this inner encoder
            public GZipMessageEncoderFactory(MessageEncoderFactory messageEncoderFactory)
            {
                if (messageEncoderFactory == null)
                    throw new ArgumentNullException("messageEncoderFactory", "A valid message encoder factory must be passed to the GZipEncoder");
                encoder = new GZipMessageEncoder(messageEncoderFactory.Encoder);
            }

            //The service framework uses this property to obtain an encoder from this encoder factory
            public override MessageEncoder Encoder
            {
                get { return encoder; }
            }

            public override MessageVersion MessageVersion
            {
                get { return encoder.MessageVersion; }
            }

            //This is the actual GZip encoder
            class GZipMessageEncoder : MessageEncoder
            {
                static string GZipContentType = "application/x-gzip";

                //This implementation wraps an inner encoder that actually converts a WCF Message
                //into textual XML, binary XML or some other format. This implementation then compresses the results.
                //The opposite happens when reading messages.
                //This member stores this inner encoder.
                MessageEncoder innerEncoder;

                //We require an inner encoder to be supplied (see comment above)
                internal GZipMessageEncoder(MessageEncoder messageEncoder)
                    : base()
                {
                    if (messageEncoder == null)
                        throw new ArgumentNullException("messageEncoder", "A valid message encoder must be passed to the GZipEncoder");
                    innerEncoder = messageEncoder;
                }

                public override string ContentType
                {
                    get { return GZipContentType; }
                }

                public override string MediaType
                {
                    get { return GZipContentType; }
                }

                //SOAP version to use - we delegate to the inner encoder for this
                public override MessageVersion MessageVersion
                {
                    get { return innerEncoder.MessageVersion; }
                }

                //Helper method to compress an array of bytes
                static ArraySegment<byte> CompressBuffer(ArraySegment<byte> buffer, BufferManager bufferManager, int messageOffset)
                {
                    MemoryStream memoryStream = new MemoryStream();

                    using (GZipStream gzStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
                    {
                        gzStream.Write(buffer.Array, buffer.Offset, buffer.Count);
                    }

                    byte[] compressedBytes = memoryStream.ToArray();
                    int totalLength = messageOffset + compressedBytes.Length;
                    byte[] bufferedBytes = bufferManager.TakeBuffer(totalLength);

                    Array.Copy(compressedBytes, 0, bufferedBytes, messageOffset, compressedBytes.Length);

                    bufferManager.ReturnBuffer(buffer.Array);
                    ArraySegment<byte> byteArray = new ArraySegment<byte>(bufferedBytes, messageOffset, bufferedBytes.Length - messageOffset);

                    return byteArray;
                }

                //Helper method to decompress an array of bytes
                static ArraySegment<byte> DecompressBuffer(ArraySegment<byte> buffer, BufferManager bufferManager)
                {
                    MemoryStream memoryStream = new MemoryStream(buffer.Array, buffer.Offset, buffer.Count);
                    MemoryStream decompressedStream = new MemoryStream();
                    int totalRead = 0;
                    int blockSize = 1024;
                    byte[] tempBuffer = bufferManager.TakeBuffer(blockSize);
                    using (GZipStream gzStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                    {
                        while (true)
                        {
                            int bytesRead = gzStream.Read(tempBuffer, 0, blockSize);
                            if (bytesRead == 0)
                                break;
                            decompressedStream.Write(tempBuffer, 0, bytesRead);
                            totalRead += bytesRead;
                        }
                    }
                    bufferManager.ReturnBuffer(tempBuffer);

                    byte[] decompressedBytes = decompressedStream.ToArray();
                    byte[] bufferManagerBuffer = bufferManager.TakeBuffer(decompressedBytes.Length + buffer.Offset);
                    Array.Copy(buffer.Array, 0, bufferManagerBuffer, 0, buffer.Offset);
                    Array.Copy(decompressedBytes, 0, bufferManagerBuffer, buffer.Offset, decompressedBytes.Length);

                    ArraySegment<byte> byteArray = new ArraySegment<byte>(bufferManagerBuffer, buffer.Offset, decompressedBytes.Length);
                    bufferManager.ReturnBuffer(buffer.Array);

                    return byteArray;
                }

                //One of the two main entry points into the encoder. Called by WCF to decode a buffered byte array into a Message.
                public override Message ReadMessage(ArraySegment<byte> buffer, BufferManager bufferManager, string contentType)
                {
                    //Decompress the buffer
                    ArraySegment<byte> decompressedBuffer = DecompressBuffer(buffer, bufferManager);
                    //Use the inner encoder to decode the decompressed buffer
                    Message returnMessage = innerEncoder.ReadMessage(decompressedBuffer, bufferManager);
                    returnMessage.Properties.Encoder = this;
                    return returnMessage;
                }

                //One of the two main entry points into the encoder. Called by WCF to encode a Message into a buffered byte array.
                public override ArraySegment<byte> WriteMessage(Message message, int maxMessageSize, BufferManager bufferManager, int messageOffset)
                {
                    //Use the inner encoder to encode a Message into a buffered byte array
                    ArraySegment<byte> buffer = innerEncoder.WriteMessage(message, maxMessageSize, bufferManager, 0);
                    //Compress the resulting byte array
                    return CompressBuffer(buffer, bufferManager, messageOffset);
                }

                public override Message ReadMessage(System.IO.Stream stream, int maxSizeOfHeaders, string contentType)
                {
                    //Pass false for the "leaveOpen" parameter to the GZipStream constructor.
                    //This will ensure that the inner stream gets closed when the message gets closed, which
                    //will ensure that resources are available for reuse/release.
                    GZipStream gzStream = new GZipStream(stream, CompressionMode.Decompress, false);
                    return innerEncoder.ReadMessage(gzStream, maxSizeOfHeaders);
                }

                public override void WriteMessage(Message message, System.IO.Stream stream)
                {
                    using (GZipStream gzStream = new GZipStream(stream, CompressionMode.Compress, true))
                    {
                        innerEncoder.WriteMessage(message, gzStream);
                    }

                    // innerEncoder.WriteMessage(message, gzStream) depends on that it can flush data by flushing 
                    // the stream passed in, but the implementation of GZipStream.Flush will not flush underlying
                    // stream, so we need to flush here.
                    stream.Flush();
                }
            }
        }
        #endregion
        public class InsecureGZipBasicHttpBinding : Binding
        {
            readonly HttpTransportBindingElement httpTransport;
            readonly TextMessageEncodingBindingElement textMessageEncoding;
            readonly GZipMessageEncodingBindingElement compressionMessageEncoding;

            public InsecureGZipBasicHttpBinding()
            {
                httpTransport = new HttpTransportBindingElement();
                textMessageEncoding = new TextMessageEncodingBindingElement { MessageVersion = MessageVersion.Soap11 };
                compressionMessageEncoding = new GZipMessageEncodingBindingElement(textMessageEncoding);
            }

            public override BindingElementCollection CreateBindingElements()
            {
                return new BindingElementCollection
                {
                    this.compressionMessageEncoding,
                    this.httpTransport
                };
            }

            public override string Scheme
            {
                get { return this.httpTransport.Scheme; }
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
            public string Echo(string text)
            {
                return text;
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), new InsecureGZipBasicHttpBinding(), "");
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(new InsecureGZipBasicHttpBinding(), new EndpointAddress(baseAddress));
            ITest proxy = factory.CreateChannel();
            Console.WriteLine(proxy.Echo("Hello"));

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    // http://stackoverflow.com/q/7013700/751090
    public class StackOverflow_7013700
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            string GetString(int size);
        }
        public class Service : ITest
        {
            public string GetString(int size)
            {
                return new string('r', size);
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            WebServiceHost host = new WebServiceHost(typeof(Service), new Uri(baseAddress));
            host.Open();
            Console.WriteLine("Host opened");

            WebChannelFactory<ITest> factory = new WebChannelFactory<ITest>(new Uri(baseAddress));
            (factory.Endpoint.Binding as WebHttpBinding).ReaderQuotas.MaxStringContentLength = 100000;
            ITest proxy = factory.CreateChannel();
            Console.WriteLine(proxy.GetString(100).Length);

            try
            {
                Console.WriteLine(proxy.GetString(60000).Length);
            }
            catch (Exception e)
            {
                Console.WriteLine("{0}: {1}", e.GetType().FullName, e.Message);
            }

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class MyCode_8
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract(Action = "GetAllDataRequest", ReplyAction = "GetAllDataReply")]
            Message GetAllDataAMessage();

            [OperationContract(Action = "GetTypedAllDataRequest", ReplyAction = "GetTypedAllDataReply")]
            Message GetAllDataAsTypedMessage();
        }

        public class Service : ITest
        {
            static List<Data> AllData = new List<Data>
            {
                new Data { Value = "First" },
                new Data { Value = "Second" },
                new Data { Value = "Third" },
                new Data { Value = "Fourth" },
            };

            public Message GetAllDataAMessage()
            {
                Message response = Message.CreateMessage(
                    OperationContext.Current.IncomingMessageVersion,
                    "GetAllDataReply",
                    new StreamingDataCollectionBodyWriter<Data>(AllData));
                return response;
            }

            public Message GetAllDataAsTypedMessage()
            {
                return new DataCollectionMessage<Data>("GetTypedAllDataReply", AllData);
            }
        }

        public class Data
        {
            public string Value;
        }

        public class DataCollectionMessage<T> : Message where T : class
        {
            IEnumerable<T> dataCollection;
            MessageHeaders headers;
            MessageProperties properties;
            MessageVersion messageVersion;
            public DataCollectionMessage(string actionName, IEnumerable<T> dataCollection)
                : base()
            {
                this.messageVersion = MessageVersion.Soap11;
                this.dataCollection = dataCollection;
                this.headers = new MessageHeaders(this.messageVersion);
                this.properties = new MessageProperties();
                this.headers.Action = actionName;
            }

            public override MessageHeaders Headers
            {
                get { return this.headers; }
            }

            protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
            {
                StreamingDataCollectionBodyWriter<T> bodyWriter = new StreamingDataCollectionBodyWriter<T>(this.dataCollection);
                bodyWriter.WriteBodyContents(writer);
            }

            public override MessageProperties Properties
            {
                get { return this.properties; }
            }

            public override MessageVersion Version
            {
                get { return this.messageVersion; }
            }
        }

        public class StreamingDataCollectionBodyWriter<T> : BodyWriter
        {
            IEnumerable<T> dataCollection;
            public StreamingDataCollectionBodyWriter(IEnumerable<T> dataCollection)
                : base(true)
            {
                this.dataCollection = dataCollection;
            }

            protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
            {
                writer.WriteStartElement("data");
                DataContractSerializer dcs = new DataContractSerializer(typeof(T));
                foreach (T t in this.dataCollection)
                {
                    dcs.WriteObject(writer, t);
                }

                writer.WriteEndElement();
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

            Console.WriteLine(proxy.GetAllDataAMessage());
            Console.WriteLine(proxy.GetAllDataAsTypedMessage());

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    // http://stackoverflow.com/q/7010654/751090
    public class StackOverflow_7010654
    {
        [MessageContract(IsWrapped = false)]
        public class MyMC
        {
            [MessageBodyMember]
            public string MyMessage { get; set; }
        }
        public static void Test()
        {
            TypedMessageConverter tmc = TypedMessageConverter.Create(typeof(MyMC), "Action");
            Message msg = tmc.ToMessage(new MyMC { MyMessage = "some string" }, MessageVersion.Soap11);
            Console.WriteLine(msg);

            Console.WriteLine();

            msg = Message.CreateMessage(MessageVersion.Soap11, "Action", new MyBodyWriter());
            Console.WriteLine(msg);
        }
        public class MyBodyWriter : BodyWriter
        {
            public MyBodyWriter() : base(true) { }

            protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
            {
                writer.WriteString("some string");
            }
        }
    }

    public class Post_fb6ca182_a0dc_41d0_8881_2ac3521e55a5
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
        static Binding GetBinding(bool addFixupBindingElement)
        {
            var result = new CustomBinding(
                new TextMessageEncodingBindingElement(MessageVersion.Soap12, Encoding.UTF8),
                new HttpTransportBindingElement());

            if (addFixupBindingElement)
            {
                result.Elements.Insert(1, new WrappingBindingElement());
            }

            return result;
        }

        public class WrappingBindingElement : BindingElement
        {
            public WrappingBindingElement()
            {
            }

            public override BindingElement Clone()
            {
                return new WrappingBindingElement();
            }

            public override T GetProperty<T>(BindingContext context)
            {
                return context.GetInnerProperty<T>();
            }

            public override bool CanBuildChannelListener<TChannel>(BindingContext context)
            {
                return typeof(TChannel) == typeof(IReplyChannel)
                    && context.CanBuildInnerChannelListener<TChannel>();
            }

            public override IChannelListener<TChannel> BuildChannelListener<TChannel>(BindingContext context)
            {
                if (context == null)
                {
                    throw new ArgumentNullException("context");
                }

                if (typeof(TChannel) != typeof(IReplyChannel))
                {
                    throw new InvalidOperationException("Only IReplyChannel is supported");
                }

                var result = new WrappingListener(context.BuildInnerChannelListener<IReplyChannel>());
                return (IChannelListener<TChannel>)result;
            }

            class WrappingListener : ChannelListenerBase<IReplyChannel>
            {
                IChannelListener<IReplyChannel> innerListener;
                public WrappingListener(IChannelListener<IReplyChannel> innerListener)
                {
                    this.innerListener = innerListener;
                }

                public override T GetProperty<T>()
                {
                    T baseProperty = base.GetProperty<T>();
                    if (baseProperty != null)
                    {
                        return baseProperty;
                    }

                    return this.innerListener.GetProperty<T>();
                }

                protected override IReplyChannel OnAcceptChannel(TimeSpan timeout)
                {
                    return this.WrapChannel(this.innerListener.AcceptChannel(timeout));
                }

                protected override IAsyncResult OnBeginAcceptChannel(TimeSpan timeout, AsyncCallback callback, object state)
                {
                    return this.innerListener.BeginAcceptChannel(timeout, callback, state);
                }

                protected override IReplyChannel OnEndAcceptChannel(IAsyncResult result)
                {
                    return this.WrapChannel(this.innerListener.EndAcceptChannel(result));
                }

                protected override IAsyncResult OnBeginWaitForChannel(TimeSpan timeout, AsyncCallback callback, object state)
                {
                    return this.innerListener.BeginWaitForChannel(timeout, callback, state);
                }

                protected override bool OnEndWaitForChannel(IAsyncResult result)
                {
                    return this.innerListener.EndWaitForChannel(result);
                }

                protected override bool OnWaitForChannel(TimeSpan timeout)
                {
                    return this.innerListener.WaitForChannel(timeout);
                }

                private IReplyChannel WrapChannel(IReplyChannel innerChannel)
                {
                    if (innerChannel == null)
                    {
                        return null;
                    }

                    return new WrappingChannel(this, innerChannel);
                }

                #region Other properties / methods which are delegated to the inner listener
                public override Uri Uri
                {
                    get { return this.innerListener.Uri; }
                }

                protected override void OnAbort()
                {
                    this.innerListener.Abort();
                }

                protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
                {
                    return this.innerListener.BeginClose(timeout, callback, state);
                }

                protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
                {
                    return this.innerListener.BeginOpen(timeout, callback, state);
                }

                protected override void OnClose(TimeSpan timeout)
                {
                    this.innerListener.Close(timeout);
                }

                protected override void OnEndClose(IAsyncResult result)
                {
                    this.innerListener.EndClose(result);
                }

                protected override void OnEndOpen(IAsyncResult result)
                {
                    this.innerListener.EndOpen(result);
                }

                protected override void OnOpen(TimeSpan timeout)
                {
                    this.innerListener.Open(timeout);
                }
                #endregion
            }

            class WrappingChannel : ChannelBase, IReplyChannel
            {
                IReplyChannel innerChannel;
                public WrappingChannel(ChannelManagerBase channelManager, IReplyChannel innerChannel)
                    : base(channelManager)
                {
                    if (innerChannel == null)
                    {
                        throw new ArgumentNullException("innerChannel");
                    }

                    this.innerChannel = innerChannel;
                }

                public IAsyncResult BeginReceiveRequest(TimeSpan timeout, AsyncCallback callback, object state)
                {
                    return this.innerChannel.BeginReceiveRequest(timeout, callback, state);
                }

                public IAsyncResult BeginReceiveRequest(AsyncCallback callback, object state)
                {
                    return this.BeginReceiveRequest(this.DefaultReceiveTimeout, callback, state);
                }

                public IAsyncResult BeginTryReceiveRequest(TimeSpan timeout, AsyncCallback callback, object state)
                {
                    return this.innerChannel.BeginTryReceiveRequest(timeout, callback, state);
                }

                public IAsyncResult BeginWaitForRequest(TimeSpan timeout, AsyncCallback callback, object state)
                {
                    return this.innerChannel.BeginWaitForRequest(timeout, callback, state);
                }

                public RequestContext EndReceiveRequest(IAsyncResult result)
                {
                    return this.WrapContext(this.innerChannel.EndReceiveRequest(result));
                }

                public bool EndTryReceiveRequest(IAsyncResult result, out RequestContext context)
                {
                    bool endTryResult = this.innerChannel.EndTryReceiveRequest(result, out context);
                    if (endTryResult)
                    {
                        context = this.WrapContext(context);
                    }

                    return endTryResult;
                }

                public bool EndWaitForRequest(IAsyncResult result)
                {
                    return this.innerChannel.EndWaitForRequest(result);
                }

                public RequestContext ReceiveRequest(TimeSpan timeout)
                {
                    RequestContext context = this.innerChannel.ReceiveRequest(timeout);
                    return this.WrapContext(context);
                }

                public RequestContext ReceiveRequest()
                {
                    return this.ReceiveRequest(this.DefaultReceiveTimeout);
                }

                public bool TryReceiveRequest(TimeSpan timeout, out RequestContext context)
                {
                    bool result = this.innerChannel.TryReceiveRequest(timeout, out context);
                    if (result)
                    {
                        context = this.WrapContext(context);
                    }

                    return result;
                }

                public bool WaitForRequest(TimeSpan timeout)
                {
                    return this.innerChannel.WaitForRequest(timeout);
                }

                private RequestContext WrapContext(RequestContext innerContext)
                {
                    if (innerContext == null)
                    {
                        return null;
                    }
                    else
                    {
                        return new WrappingRequestContext(this, innerContext);
                    }
                }

                #region Other properties / methods which are delegated to the inner channel
                public override T GetProperty<T>()
                {
                    return this.innerChannel.GetProperty<T>();
                }

                protected override void OnAbort()
                {
                    this.innerChannel.Abort();
                }

                protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
                {
                    return this.innerChannel.BeginClose(timeout, callback, state);
                }

                protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
                {
                    return this.innerChannel.BeginOpen(timeout, callback, state);
                }

                protected override void OnClose(TimeSpan timeout)
                {
                    this.innerChannel.Close(timeout);
                }

                protected override void OnEndClose(IAsyncResult result)
                {
                    this.innerChannel.EndClose(result);
                }

                protected override void OnEndOpen(IAsyncResult result)
                {
                    this.innerChannel.EndOpen(result);
                }

                protected override void OnOpen(TimeSpan timeout)
                {
                    this.innerChannel.Open(timeout);
                }

                public EndpointAddress LocalAddress
                {
                    get { return this.innerChannel.LocalAddress; }
                }
                #endregion

                class WrappingRequestContext : RequestContext
                {
                    RequestContext innerContext;
                    WrappingChannel channel;
                    public WrappingRequestContext(WrappingChannel channel, RequestContext innerContext)
                    {
                        this.innerContext = innerContext;
                        this.channel = channel;
                    }

                    public override void Abort()
                    {
                        this.innerContext.Abort();
                    }

                    public override IAsyncResult BeginReply(Message message, TimeSpan timeout, AsyncCallback callback, object state)
                    {
                        return this.innerContext.BeginReply(message, timeout, callback, state);
                    }

                    public override IAsyncResult BeginReply(Message message, AsyncCallback callback, object state)
                    {
                        return this.BeginReply(message, this.channel.DefaultSendTimeout, callback, state);
                    }

                    public override void Close(TimeSpan timeout)
                    {
                        this.innerContext.Close(timeout);
                    }

                    public override void Close()
                    {
                        this.innerContext.Close();
                    }

                    public override void EndReply(IAsyncResult result)
                    {
                        this.innerContext.EndReply(result);
                    }

                    public override void Reply(Message message, TimeSpan timeout)
                    {
                        this.innerContext.Reply(message, timeout);
                    }

                    public override void Reply(Message message)
                    {
                        this.Reply(message, this.channel.DefaultSendTimeout);
                    }

                    public override Message RequestMessage
                    {
                        get
                        {
                            Message innerMessage = this.innerContext.RequestMessage;
                            if (innerMessage.Headers.Action == null)
                            {
                                HttpRequestMessageProperty reqProp;
                                reqProp = (HttpRequestMessageProperty)innerMessage.Properties[HttpRequestMessageProperty.Name];
                                string soapAction = reqProp.Headers["SOAPAction"];
                                if (soapAction != null)
                                {
                                    Console.WriteLine("Adding action to the request which doesn't have any");
                                    innerMessage.Headers.Action = soapAction;
                                }
                            }

                            return innerMessage;
                        }
                    }
                }
            }
        }

        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), GetBinding(true), "");//.Behaviors.Add(new MyInspector());
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(GetBinding(false), new EndpointAddress(baseAddress));
            ITest proxy = factory.CreateChannel();
            Console.WriteLine(proxy.Echo("Hello"));

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.WriteLine();
            Console.WriteLine();

            string requestBody = @"<s:Envelope xmlns:s=""http://www.w3.org/2003/05/soap-envelope"">
  <s:Body>
    <Echo xmlns=""http://tempuri.org/"">
      <text>Hello</text>
    </Echo>
  </s:Body>
</s:Envelope>";

            Console.WriteLine("With the action parameter in the content-type");
            Util.SendRequest(baseAddress, "POST", "application/soap+xml; charset=utf-8; action=\"http://tempuri.org/ITest/Echo\"", requestBody);

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Without action parameter, but with SOAPAction header");
            var newHeaders = new Dictionary<string, string> { { "SOAPAction", "http://tempuri.org/ITest/Echo" } };
            Util.SendRequest(baseAddress, "POST", "application/soap+xml; charset=utf-8", requestBody, newHeaders);

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_5c0789c1_267b_4b7d_9008_506980da194c
    {
        static readonly string BaseAddressFinal = "http://" + Environment.MachineName + ":8000/Service";
        static readonly string BaseAddressMiddle = "http://" + Environment.MachineName + ":8000/Middle";

        [ServiceContract]
        public interface IService
        {
            [OperationContract(AsyncPattern = true)]
            [WebGet]
            IAsyncResult BeginServiceMethod(string arg, AsyncCallback callback, object state);
            string EndServiceMethod(IAsyncResult asyncResult);
        }
        class MiddleService : IService
        {
            public MiddleService()
            {
                _objectForSecondService = CreateIServiceInstance("/*Endpoint from web.config*/");
            }

            private IService _objectForSecondService;

            public static IService CreateIServiceInstance(string endpointName)
            {
                WebHttpBinding webBinding = new WebHttpBinding();

                var factoryObject = new ChannelFactory<IService>(webBinding, new EndpointAddress(BaseAddressFinal));
                factoryObject.Endpoint.Behaviors.Add(new WebHttpBehavior());
                return factoryObject.CreateChannel();
            }

            public IAsyncResult BeginServiceMethod(string arg1, AsyncCallback arg2, object arg3)
            {
                using (new OperationContextScope((IContextChannel)_objectForSecondService))
                {
                    return _objectForSecondService.BeginServiceMethod(arg1, arg2, arg3);
                }
            }

            public string EndServiceMethod(IAsyncResult arg)
            {
                using (new OperationContextScope((IContextChannel)_objectForSecondService))
                {
                    return _objectForSecondService.EndServiceMethod(arg);
                }
            }
        }
        class FinalService : IService
        {
            public IAsyncResult BeginServiceMethod(string arg, AsyncCallback callback, object state)
            {
                Func<string, string> func = x => x;
                return func.BeginInvoke(arg, callback, state);
            }

            public string EndServiceMethod(IAsyncResult asyncResult)
            {
                Func<string, string> func = (Func<string, string>)((System.Runtime.Remoting.Messaging.AsyncResult)asyncResult).AsyncDelegate;
                return func.EndInvoke(asyncResult);
            }
        }
        public static void Test()
        {
            ServiceHost middleHost = new ServiceHost(typeof(MiddleService), new Uri(BaseAddressMiddle));
            ServiceHost finalHost = new ServiceHost(typeof(MiddleService), new Uri(BaseAddressFinal));
            middleHost.AddServiceEndpoint(typeof(IService), new BasicHttpBinding(), "");
            finalHost.AddServiceEndpoint(typeof(IService), new WebHttpBinding(), "").Behaviors.Add(new WebHttpBehavior());
            middleHost.Open();
            finalHost.Open();
            Console.WriteLine("Hosts opened");

            ChannelFactory<IService> factory = new ChannelFactory<IService>(new BasicHttpBinding(), new EndpointAddress(BaseAddressMiddle));
            var proxy = factory.CreateChannel();

            proxy.BeginServiceMethod("Hello world", delegate(IAsyncResult ar)
            {
                Console.WriteLine(proxy.EndServiceMethod(ar));
            }, null);

            Console.WriteLine("Press ENTER to close");
            Console.ReadLine();
        }
    }

    public class Post_593df2bb_9505_49f2_92e9_e6e925d95830
    {
        [ServiceContract]
        [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
        public class Service
        {
            const int MillisecondsToSleep = 500;
            [WebInvoke]
            public string Echo(string text)
            {
                Console.WriteLine("Called Echo at thread {0}", Thread.CurrentThread.ManagedThreadId);
                Thread.Sleep(MillisecondsToSleep);
                return text;
            }
            [WebGet]
            public int Add(int x, int y)
            {
                Console.WriteLine("Called Add at thread {0}", Thread.CurrentThread.ManagedThreadId);
                Thread.Sleep(MillisecondsToSleep);
                return x + y;
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            WebServiceHost host = new WebServiceHost(typeof(Service), new Uri(baseAddress));
            ServiceThrottlingBehavior stb = host.Description.Behaviors.Find<ServiceThrottlingBehavior>();
            if (stb == null)
            {
                stb = new ServiceThrottlingBehavior();
                host.Description.Behaviors.Add(stb);
            }

            stb.MaxConcurrentCalls = 50;
            stb.MaxConcurrentInstances = 50;
            stb.MaxConcurrentSessions = 50;

            host.Open();
            Console.WriteLine("Host opened");

            Thread[] requestThreads = new Thread[20];
            for (int i = 0; i < requestThreads.Length; i++)
            {
                bool useGet = (i % 2) == 0;
                requestThreads[i] = new Thread(new ThreadStart(delegate
                {
                    string address = baseAddress + (useGet ? "/Add?x=3&y=9" : "/Echo");
                    HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(address);
                    req.Method = useGet ? "GET" : "POST";
                    if (!useGet)
                    {
                        req.ContentType = "application/json";
                        byte[] reqBody = Encoding.UTF8.GetBytes("\"Hello world\"");
                        Stream reqStream = req.GetRequestStream();
                        reqStream.Write(reqBody, 0, reqBody.Length);
                        reqStream.Close();
                    }

                    HttpWebResponse resp;
                    try
                    {
                        resp = (HttpWebResponse)req.GetResponse();
                    }
                    catch (WebException ex)
                    {
                        resp = ex.Response as HttpWebResponse;
                    }

                    if (resp == null)
                    {
                        Console.WriteLine("Response is null");
                    }
                    else
                    {
                        Console.WriteLine("HTTP/{0} {1} {2}", resp.ProtocolVersion, (int)resp.StatusCode, resp.StatusDescription);
                        resp.Close();
                    }
                }));
            }

            for (int i = 0; i < requestThreads.Length; i++)
            {
                requestThreads[i].Start();
            }

            for (int i = 0; i < requestThreads.Length; i++)
            {
                requestThreads[i].Join();
            }

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    // http://stackoverflow.com/q/7044842/751090
    public class StackOverflow_7044842
    {
        const string xml = @"<ajax-response>
<response>
<item>
<number></number>
<xxx>Não ok</xxx>
<error>null</error>
</item>
</response>
</ajax-response>";

        [ServiceContract]
        public class SimpleService
        {
            [WebGet]
            public Stream GetXml()
            {
                WebOperationContext.Current.OutgoingResponse.ContentType = "text/xml";
                Encoding encoding = Encoding.GetEncoding(1252);
                return new MemoryStream(encoding.GetBytes(xml));
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            WebServiceHost host = new WebServiceHost(typeof(SimpleService), new Uri(baseAddress));
            host.Open();
            Console.WriteLine("Host opened");

            WebClient client = new WebClient();
            client.Encoding = Encoding.UTF8;//.GetEncoding(1252);
            string response = client.DownloadString(baseAddress + "/GetXml");
            Console.WriteLine(response);

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    // http://stackoverflow.com/q/7068743/751090
    public class StackOverflow_7068743
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
                return text + " (via " + OperationContext.Current.IncomingMessageHeaders.To + ")";
            }
        }
        public static void Test()
        {
            string baseAddressHttp = "http://" + Environment.MachineName + ":8000/Service";
            string baseAddressPipe = "net.pipe://localhost/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddressHttp), new Uri(baseAddressPipe));
            host.Description.Behaviors.Add(new ServiceDiscoveryBehavior());
            host.AddServiceEndpoint(new UdpDiscoveryEndpoint());
            host.AddServiceEndpoint(typeof(ITest), new BasicHttpBinding(), "");
            host.AddServiceEndpoint(typeof(ITest), new NetNamedPipeBinding(NetNamedPipeSecurityMode.None), "");
            host.Open();
            Console.WriteLine("Host opened");

            DiscoveryClient discoveryClient = new DiscoveryClient(new UdpDiscoveryEndpoint());
            FindResponse findResponse = discoveryClient.Find(new FindCriteria(typeof(ITest)));
            Console.WriteLine(findResponse.Endpoints.Count);

            EndpointAddress address = null;
            Binding binding = null;
            foreach (var endpoint in findResponse.Endpoints)
            {
                if (endpoint.Address.Uri.Scheme == Uri.UriSchemeHttp)
                {
                    address = endpoint.Address;
                    binding = new BasicHttpBinding();
                }
                else if (endpoint.Address.Uri.Scheme == Uri.UriSchemeNetPipe)
                {
                    address = endpoint.Address;
                    binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
                    break; // this is the preferred
                }

                Console.WriteLine(endpoint.Address);
            }

            if (binding == null)
            {
                Console.WriteLine("No known bindings");
            }
            else
            {
                ChannelFactory<ITest> factory = new ChannelFactory<ITest>(binding, address);
                ITest proxy = factory.CreateChannel();
                Console.WriteLine(proxy.Echo("Hello"));
                ((IClientChannel)proxy).Close();
                factory.Close();
            }

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class AsyncInWCF
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
            IAsyncResult BeginAdd(int x, int y, AsyncCallback callback, object state);
            int EndAdd(IAsyncResult asyncResult);
        }
        public class AsyncService : ITestAsync
        {
            public IAsyncResult BeginAdd(int x, int y, AsyncCallback callback, object state)
            {
                Func<int, int, int> op = (a, b) => a + b;
                return op.BeginInvoke(x, y, callback, state);
            }

            public int EndAdd(IAsyncResult asyncResult)
            {
                Func<int, int, int> op = ((System.Runtime.Remoting.Messaging.AsyncResult)asyncResult).AsyncDelegate as Func<int, int, int>;
                return op.EndInvoke(asyncResult);
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(AsyncService), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITestAsync), new BasicHttpBinding(), "");
            host.Open();
            Console.WriteLine("Host opened");

            // Client can be synchronous, it doesn't matter the server mode
            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(new BasicHttpBinding(), new EndpointAddress(baseAddress));
            ITest proxy = factory.CreateChannel();
            Console.WriteLine(proxy.Add(44, 66));

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_b3530213_4c93_444a_b81c_4be882836b70
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
        public class MyInspector : IServiceBehavior, IDispatchMessageInspector
        {
            public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
            {
                Console.WriteLine("Here");
                return null;
            }

            public void BeforeSendReply(ref Message reply, object correlationState)
            {
            }

            public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
            {
            }

            public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
            {
                foreach (ChannelDispatcher cd in serviceHostBase.ChannelDispatchers)
                {
                    foreach (EndpointDispatcher ed in cd.Endpoints)
                    {
                        if (!ed.IsSystemEndpoint)
                        {
                            ed.DispatchRuntime.MessageInspectors.Add(this);
                        }
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
            host.AddServiceEndpoint(typeof(ITest), new BasicHttpBinding(), "");
            host.Description.Behaviors.Add(new MyInspector());
            host.Open();
            Console.WriteLine("Host opened");

            WebClient c = new WebClient();
            c.DownloadString(baseAddress);

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_2abb8fef_ecd0_4cc5_be3e_e38dbfd8920d
    {
        [DataContract]
        public class Row
        {
            [DataMember]
            public int ID { get; set; }
            [DataMember]
            public string Name { get; set; }
            [DataMember]
            public string Address { get; set; }
            [DataMember]
            public int Age { get; set; }
        }
        [DataContract]
        public class Query
        {
        }
        [ServiceContract]
        public interface ITabularReport
        {
            [OperationContract]
            List<Row> GetReport(Query query);
        }
    }

    // http://stackoverflow.com/q/7228102/751090
    public class StackOverflow_7228102
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            [WebInvoke(
                Method = "POST",
                BodyStyle = WebMessageBodyStyle.Bare,
                UriTemplate = "/GetData")]
            string GetData(Stream data);
        }
        public class Service : ITest
        {
            public string GetData(Stream input)
            {
                string body = new StreamReader(input).ReadToEnd();
                NameValueCollection nvc = HttpUtility.ParseQueryString(body);
                return nvc["FirstName"];
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            WebServiceHost host = new WebServiceHost(typeof(Service), new Uri(baseAddress));
            host.Open();
            Console.WriteLine("Host opened");

            WebClient c = new WebClient();
            c.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            Console.WriteLine(c.UploadString(baseAddress + "/GetData", "FirstName=John&LastName=Doe&Age=33"));

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_2d2676ac_8427_4a20_aa92_7857e1efdcd1
    {
        [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
        [System.SerializableAttribute()]
        [System.Diagnostics.DebuggerStepThroughAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
        public partial class link
        {

            private string hrefField;

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string href
            {
                get
                {
                    return this.hrefField;
                }
                set
                {
                    this.hrefField = value;
                }
            }
        }
        /// <remarks/>
        [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
        [System.SerializableAttribute()]
        [System.Diagnostics.DebuggerStepThroughAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
        public partial class order
        {

            private string clientidField;

            private string createdbyuseridField;

            private string createddateField;

            private string descriptionField;

            private string idField;

            private string itemgroupsField;

            private string lastmodifieddateField;

            private string originField;

            private string statenameField;

            private link linkField;

            private orderPrices pricesField;

            private orderProfilelink profilelinkField;

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute("client-id", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string clientid
            {
                get
                {
                    return this.clientidField;
                }
                set
                {
                    this.clientidField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute("created-by-user-id", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string createdbyuserid
            {
                get
                {
                    return this.createdbyuseridField;
                }
                set
                {
                    this.createdbyuseridField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute("created-date", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string createddate
            {
                get
                {
                    return this.createddateField;
                }
                set
                {
                    this.createddateField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string description
            {
                get
                {
                    return this.descriptionField;
                }
                set
                {
                    this.descriptionField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string id
            {
                get
                {
                    return this.idField;
                }
                set
                {
                    this.idField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute("item-groups", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string itemgroups
            {
                get
                {
                    return this.itemgroupsField;
                }
                set
                {
                    this.itemgroupsField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute("last-modified-date", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string lastmodifieddate
            {
                get
                {
                    return this.lastmodifieddateField;
                }
                set
                {
                    this.lastmodifieddateField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string origin
            {
                get
                {
                    return this.originField;
                }
                set
                {
                    this.originField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute("state-name", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string statename
            {
                get
                {
                    return this.statenameField;
                }
                set
                {
                    this.statenameField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute("link")]
            public link link
            {
                get
                {
                    return this.linkField;
                }
                set
                {
                    this.linkField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute("prices", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public orderPrices prices
            {
                get
                {
                    return this.pricesField;
                }
                set
                {
                    this.pricesField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute("profile-link", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public orderProfilelink profilelink
            {
                get
                {
                    return this.profilelinkField;
                }
                set
                {
                    this.profilelinkField = value;
                }
            }
        }

        /// <remarks/>
        [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
        [System.SerializableAttribute()]
        [System.Diagnostics.DebuggerStepThroughAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class orderProfilelink
        {

            private string idField;

            private link linkField;

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string Id
            {
                get
                {
                    return this.idField;
                }
                set
                {
                    this.idField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute("link")]
            public link link
            {
                get
                {
                    return this.linkField;
                }
                set
                {
                    this.linkField = value;
                }
            }
        }

        /// <remarks/>
        [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
        [System.SerializableAttribute()]
        [System.Diagnostics.DebuggerStepThroughAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class orderPrices
        {

            private orderPricesPrice[] priceField;

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public orderPricesPrice[] price
            {
                get
                {
                    return this.priceField;
                }
                set
                {
                    this.priceField = value;
                }
            }
        }

        /// <remarks/>
        [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
        [System.SerializableAttribute()]
        [System.Diagnostics.DebuggerStepThroughAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class orderPricesPrice
        {

            private string incvatField;

            private string typeField;

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string incvat
            {
                get
                {
                    return this.incvatField;
                }
                set
                {
                    this.incvatField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string type
            {
                get
                {
                    return this.typeField;
                }
                set
                {
                    this.typeField = value;
                }
            }
        }

        [ServiceContract]
        [XmlSerializerFormat]
        public interface ITest
        {
            [WebGet]
            order GetOrder();
        }
        public class Service : ITest
        {
            public order GetOrder()
            {
                order order = new order
                {
                    clientid = "p4u",
                    createdbyuserid = "clientUserId",
                    createddate = "2010-09-09 12:11:57",
                    description = "order1",
                    id = "order1",
                    itemgroups = null,
                    lastmodifieddate = "2010-09-09 12:12:00",
                    link = new link { href = "/rest/v1/orders/order1" },
                    origin = "DATALINK",
                    statename = "INCOMPLETE",
                    prices = new orderPrices
                    {
                        price = new orderPricesPrice[]
                        {
                            new orderPricesPrice { incvat = "0.0", type = "monthly-cost" },
                            new orderPricesPrice { incvat = "0.0", type = "cost-today" },
                        }
                    },
                    profilelink = new orderProfilelink
                    {
                        Id = "profile150000",
                        link = new link { href = "http://google.com" }
                    }
                };
                return order;
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            WebServiceHost host = new WebServiceHost(typeof(Service), new Uri(baseAddress));
            host.Open();
            Console.WriteLine("Host opened");

            WebClient c = new WebClient();
            Console.WriteLine(c.DownloadString(baseAddress + "/GetOrder"));

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    // http://stackoverflow.com/q/7276178/751090
    public class StackOverflow_7276178
    {
        const string XML = @"<group name='name1'  >
  <group name='name2'  >
    <image name='test1' >
      <image name='test2' ></image>
      <group name='test98'>
        <image name='test67' >
          <group name='test987'>
            <text name='asddd' path='myPath'></text>
          </group>
        </image>
      </group>
      <group name='name22'  >
        <image name='test3' left='myLeft'></image>
      </group>
    </image>
    <image name='test4'>
      <text name='asddd'></text>
    </image>
  </group>
</group>";

        public static void Test()
        {
            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = true;
            doc.LoadXml(XML);

            foreach (XmlAttribute pathAttr in doc.SelectNodes("//@path"))
            {
                pathAttr.Value = pathAttr.Value + "_modified";
            }

            foreach (XmlAttribute leftAttr in doc.SelectNodes("//@left"))
            {
                leftAttr.Value = leftAttr.Value + "_modified";
                XmlAttribute newAttr = doc.CreateAttribute("right");
                newAttr.Value = "right attr";
                leftAttr.OwnerElement.Attributes.Append(newAttr);
            }

            Console.WriteLine(doc.OuterXml);
        }
    }

    public class SerializationTest
    {
        [Serializable]
        public class MySer
        {
            public MySer()
            {
                Console.WriteLine("--MySer..ctor--");
            }
            public string str = "Hello world";
            [OnSerializing]
            public void OnSerializing(StreamingContext ctx)
            {
                Console.WriteLine("--OnSerializing--");
            }
            [OnSerialized]
            public void OnSerialized(StreamingContext ctx)
            {
                Console.WriteLine("--OnSerialized--");
            }
        }

        public static void SerializeDeserializeAll()
        {
            Console.WriteLine("DCS");
            MemoryStream ms = new MemoryStream();
            new DataContractSerializer(typeof(MySer)).WriteObject(ms, new MySer());
            Console.WriteLine("Serialized: {0}", Encoding.UTF8.GetString(ms.ToArray()));
            Console.WriteLine();

            Console.WriteLine("DCJS");
            ms.SetLength(0);
            new DataContractJsonSerializer(typeof(MySer)).WriteObject(ms, new MySer());
            Console.WriteLine("Serialized: {0}", Encoding.UTF8.GetString(ms.ToArray()));
            Console.WriteLine();

            Console.WriteLine("XS");
            ms.SetLength(0);
            new XmlSerializer(typeof(MySer)).Serialize(ms, new MySer());
            Console.WriteLine("Serialized: {0}", Encoding.UTF8.GetString(ms.ToArray()));
            Console.WriteLine();

            Console.WriteLine("BinaryFormatter");
            ms.SetLength(0);
            new BinaryFormatter().Serialize(ms, new MySer());
            Console.WriteLine("Serialized:");
            Util.PrintBytes(ms.ToArray());
            Console.WriteLine();

            Console.WriteLine("SoapFormatter");
            ms.SetLength(0);
            new SoapFormatter().Serialize(ms, new MySer());
            Console.WriteLine("Serialized:");
            Util.PrintBytes(ms.ToArray());
            Console.WriteLine();
        }
    }

    public class Post_4cfd1cd6_a038_420d_8cb5_ec5a2628df1a
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
                Console.WriteLine("In service, text = {0}", ReplaceControl(text));
                return text;
            }
        }
        static Binding GetBinding()
        {
            //var result = new WSHttpBinding(SecurityMode.None) { MessageEncoding = WSMessageEncoding.Text };
            var result = new BasicHttpBinding() { MessageEncoding = WSMessageEncoding.Mtom };
            return result;
        }
        static string ReplaceControl(string text)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var c in text)
            {
                if ((' ' <= c && c <= '~') && c != '\\')
                {
                    sb.Append(c);
                }
                else
                {
                    sb.AppendFormat("\\u{0:X4}", (int)c);
                }
            }

            return sb.ToString();
        }
        public class MyInspector : IEndpointBehavior, IDispatchMessageInspector, IClientMessageInspector
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
                endpointDispatcher.DispatchRuntime.MessageInspectors.Add(this);
            }

            public void Validate(ServiceEndpoint endpoint)
            {
            }

            public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
            {
                request = request.CreateBufferedCopy(int.MaxValue).CreateMessage();
                return null;
            }

            public void BeforeSendReply(ref Message reply, object correlationState)
            {
            }

            public void AfterReceiveReply(ref Message reply, object correlationState)
            {
                reply = reply.CreateBufferedCopy(int.MaxValue).CreateMessage();
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
            var endpoint = host.AddServiceEndpoint(typeof(ITest), GetBinding(), "");
            endpoint.Behaviors.Add(new MyInspector());
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(GetBinding(), new EndpointAddress(baseAddress));
            factory.Endpoint.Behaviors.Add(new MyInspector());
            ITest proxy = factory.CreateChannel();

            string input = "\t\tDoc1\tCase1\tActive";
            string output = proxy.Echo(input);
            Console.WriteLine("Input = {0}, Output = {1}", ReplaceControl(input), ReplaceControl(output));
            Console.WriteLine("input == output: {0}", input == output);

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    // http://stackoverflow.com/q/7341463/751090
    public class StackOverflow_7341463
    {
        [ServiceContract]
        public interface ICalc
        {
            [OperationContract, WebGet]
            int Add(int x, int y);
            [OperationContract, WebGet]
            int Subtract(int x, int y);
            [OperationContract, WebGet]
            int Multiply(int x, int y);
            [OperationContract, WebGet]
            int Divide(int x, int y);
        }
        [ServiceContract(CallbackContract = typeof(ICalcNotifications))]
        public interface INotifyingCalc : ICalc
        {
            [OperationContract]
            void Connect();
            [OperationContract]
            void Disconnect();
        }
        [ServiceContract]
        public interface ICalcNotifications
        {
            [OperationContract(IsOneWay = true)]
            void OperationPerformed(string text);
        }
        public class Service : INotifyingCalc
        {
            static List<ICalcNotifications> clients = new List<ICalcNotifications>();

            #region ICalc Members

            public int Add(int x, int y)
            {
                this.NotifyOperation("Add", x, y);
                return x + y;
            }

            public int Subtract(int x, int y)
            {
                this.NotifyOperation("Subtract", x, y);
                return x - y;
            }

            public int Multiply(int x, int y)
            {
                this.NotifyOperation("Multiply", x, y);
                return x * y;
            }

            public int Divide(int x, int y)
            {
                this.NotifyOperation("Divide", x, y);
                return x / y;
            }

            #endregion

            #region INotifyingCalc Members

            public void Connect()
            {
                var callback = OperationContext.Current.GetCallbackChannel<ICalcNotifications>();
                clients.Add(callback);
            }

            public void Disconnect()
            {
                var callback = OperationContext.Current.GetCallbackChannel<ICalcNotifications>();
                clients.Remove(callback);
            }

            #endregion

            private void NotifyOperation(string operationName, int x, int y)
            {
                foreach (var client in clients)
                {
                    client.OperationPerformed(string.Format("{0}({1}, {2})", operationName, x, y));
                }
            }
        }
        class MyCallback : ICalcNotifications
        {
            public void OperationPerformed(string text)
            {
                Console.WriteLine("Operation performed: {0}", text);
            }
        }
        public static void Test()
        {
            string baseAddressTcp = "net.tcp://" + Environment.MachineName + ":8008/Service";
            string baseAddressHttp = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddressHttp), new Uri(baseAddressTcp));
            host.AddServiceEndpoint(typeof(ICalc), new WebHttpBinding(), "").Behaviors.Add(new WebHttpBehavior());
            host.AddServiceEndpoint(typeof(INotifyingCalc), new NetTcpBinding(SecurityMode.None), "");
            host.Open();
            Console.WriteLine("Host opened");

            var factory = new DuplexChannelFactory<INotifyingCalc>(
                new InstanceContext(new MyCallback()),
                new NetTcpBinding(SecurityMode.None),
                new EndpointAddress(baseAddressTcp));
            var proxy = factory.CreateChannel();
            proxy.Connect();
            Console.WriteLine("Proxy connected");

            Console.WriteLine(new WebClient().DownloadString(baseAddressHttp + "/Add?x=4&y=7"));
            Console.WriteLine(new WebClient().DownloadString(baseAddressHttp + "/Multiply?x=44&y=57"));
            Console.WriteLine(new WebClient().DownloadString(baseAddressHttp + "/Divide?x=432&y=16"));

            proxy.Disconnect();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();

            ((IClientChannel)proxy).Close();
            factory.Close();
            host.Close();
        }
    }

    public class Post_e22dcf59_62b7_4de5_a664_1443be243377
    {
        public class MyGuid
        {
            public Guid value;
            public override string ToString()
            {
                return this.value.ToString();
            }

            public MyGuid()
            {
            }

            public MyGuid(Guid value)
            {
                this.value = value;
            }
        }
        [DataContract]
        public class Person
        {
            [DataMember(Name = "ID")]
            private string idField;

            public MyGuid ID;

            public override string ToString()
            {
                return string.Format("Person[ID={0}]", this.ID);
            }

            [OnSerializing]
            void OnSerializing(StreamingContext context)
            {
                this.idField = this.ID == null ? null : this.ID.ToString();
            }

            [OnDeserialized]
            void OnDeserialized(StreamingContext context)
            {
                Guid guid;
                if (Guid.TryParse(this.idField, out guid))
                {
                    this.ID = new MyGuid { value = guid };
                }
                else
                {
                    this.ID = null;
                }
            }
        }
        public static void Test()
        {
            DataContractJsonSerializer dcjs = new DataContractJsonSerializer(typeof(Person));
            MemoryStream ms = new MemoryStream();
            dcjs.WriteObject(ms, new Person { ID = new MyGuid { value = Guid.NewGuid() } });
            Console.WriteLine(Encoding.UTF8.GetString(ms.ToArray()));

            ms.Position = 0;
            Person p = (Person)dcjs.ReadObject(ms);
            Console.WriteLine(p);
        }
    }

    public class SurrogateTest
    {
        [Serializable]
        public class MyRoot
        {
            public MyType1 Type1;
        }

        [Serializable]
        public class MyType1
        {
            public string Value;
        }

        [Serializable]
        public class MyType2
        {
            public string Value;
        }

        public class MySurrogateSelector : ISurrogateSelector
        {
            #region ISurrogateSelector Members

            public void ChainSelector(ISurrogateSelector selector)
            {
                throw new NotImplementedException();
            }

            public ISurrogateSelector GetNextSelector()
            {
                throw new NotImplementedException();
            }

            public ISerializationSurrogate GetSurrogate(Type type, StreamingContext context, out ISurrogateSelector selector)
            {
                throw new NotImplementedException();
            }

            #endregion
        }

        public static void Test()
        {
        }
    }

    // http://stackoverflow.com/q/7484237/751090
    public class StackOverflow_7484237
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
                if (text == "throw")
                {
                    throw new ArgumentException("This will cause a fault to be received at the client");
                }
                else
                {
                    return text;
                }
            }
        }
        public class MyInspector : IEndpointBehavior, IClientMessageInspector
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
                if (reply.IsFault)
                {
                    Console.WriteLine("Log this fault: {0}", reply);
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
            host.AddServiceEndpoint(typeof(ITest), new BasicHttpBinding(), "");
            host.Open();
            Console.WriteLine("Host opened");

            var factory = new ChannelFactory<ITest>(new BasicHttpBinding(), new EndpointAddress(baseAddress));
            factory.Endpoint.Behaviors.Add(new MyInspector());
            var proxy = factory.CreateChannel();

            Console.WriteLine(proxy.Echo("This won't throw"));

            try
            {
                Console.WriteLine(proxy.Echo("throw"));
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e);
            }

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    // http://stackoverflow.com/q/7492678/751090
    public class StackOverflow_7492678
    {
        public class RequestMessage
        {
            public string data;
        }
        public class ResponseMessage
        {
            public string data;
        }
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            ResponseMessage GetData(RequestMessage message);
        }
        public class Service : ITest
        {
            public ResponseMessage GetData(RequestMessage message)
            {
                return new ResponseMessage { data = message.data };
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            var endpoint = host.AddServiceEndpoint(typeof(ITest), new WebHttpBinding(), "");
            endpoint.Behaviors.Add(new WebHttpBehavior());
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(new WebHttpBinding(), new EndpointAddress(baseAddress));
            factory.Endpoint.Behaviors.Add(new WebHttpBehavior());
            ITest proxy = factory.CreateChannel();

            Console.WriteLine(proxy.GetData(new RequestMessage { data = "mydata" }).data);

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    // http://stackoverflow.com/questions/7492836
    public class StackOverflow_7492836
    {
        [MessageContract(
            //WrapperName = "SingleTypeResponse{0}",
            WrapperNamespace = "urn:Foo.MessageContracts")]
        public class SingleTypeResponse<T>
        {
            [MessageBodyMember(Name = "ReturnValue")]
            public T ReturnValue { get; set; }
        }

        [MessageContract]
        public class ServiceRequest
        {
            [MessageBodyMember]
            public string Value { get; set; }
        }

        [ServiceContract]
        public interface ITest
        {
            [OperationContract(IsTerminating = false, IsInitiating = true, IsOneWay = false, AsyncPattern = false, Action = "urn:Foo.AuthServiceContract.ChangeMasterReporterPassword", ProtectionLevel = ProtectionLevel.None)]
            SingleTypeResponse<string> ChangePassword(ServiceRequest request);

            [OperationContract(IsTerminating = false, IsInitiating = true, IsOneWay = false, AsyncPattern = false, Action = "urn:Foo.AuthServiceContract.Foo", ProtectionLevel = ProtectionLevel.None)]
            SingleTypeResponse<int> Foo(ServiceRequest request);
        }

        public class Service : ITest
        {
            public SingleTypeResponse<string> ChangePassword(ServiceRequest request)
            {
                return new SingleTypeResponse<string> { ReturnValue = request.Value };
            }

            public SingleTypeResponse<int> Foo(ServiceRequest request)
            {
                return new SingleTypeResponse<int> { ReturnValue = int.Parse(request.Value) };
            }
        }

        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), new BasicHttpBinding(), "");
            host.Description.Behaviors.Add(new ServiceMetadataBehavior { HttpGetEnabled = true });
            host.Description.Behaviors.Find<ServiceDebugBehavior>().IncludeExceptionDetailInFaults = true;
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(new BasicHttpBinding(), new EndpointAddress(baseAddress));
            ITest proxy = factory.CreateChannel();

            Console.WriteLine(proxy.ChangePassword(new ServiceRequest { Value = "hello" }));
            Console.WriteLine(proxy.ChangePassword(new ServiceRequest { Value = "1234" }));

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    // http://stackoverflow.com/q/7502134/751090
    public class StackOverflow_7502134
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            string Echo(string text);
            [OperationContract]
            int Add(int x, int y);
        }
        public class Service : ITest
        {
            public string Echo(string text)
            {
                return text;
            }
            public int Add(int x, int y)
            {
                return x + y;
            }
        }
        public class MyInspector : IEndpointBehavior, IDispatchMessageInspector
        {
            string[] operationNames;

            public MyInspector(params string[] operationNames)
            {
                this.operationNames = operationNames ?? new string[0];
            }

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
                // by default, action == <serviceContractNamespace>/<serviceContractName>/<operationName>
                string operationName = request.Headers.Action.Substring(request.Headers.Action.LastIndexOf('/') + 1);
                if (this.operationNames.Contains(operationName))
                {
                    Console.WriteLine("Inspecting request to operation {0}", operationName);
                    Console.WriteLine(request);
                    Console.WriteLine();
                    return operationName;
                }
                else
                {
                    return null;
                }
            }

            public void BeforeSendReply(ref Message reply, object correlationState)
            {
                string operationName = correlationState as string;
                if (operationName != null)
                {
                    Console.WriteLine("Inspecting reply from operation {0}", operationName);
                    Console.WriteLine(reply);
                    Console.WriteLine();
                }
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            MyInspector inspector = new MyInspector("Add"); // inspecting Add, not Echo
            host.AddServiceEndpoint(typeof(ITest), new BasicHttpBinding(), "").Behaviors.Add(inspector);
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(new BasicHttpBinding(), new EndpointAddress(baseAddress));
            ITest proxy = factory.CreateChannel();

            Console.WriteLine("Calling Echo");
            Console.WriteLine(proxy.Echo("Hello world"));
            Console.WriteLine();

            Console.WriteLine("Calling Add");
            Console.WriteLine(proxy.Add(4, 5));

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_35de60cc_1401_46fc_af37_19c5129468bb
    {
        [DataContract(Name = "MyType", Namespace = "")]
        public class MyType
        {
            [DataMember]
            public string name1;
            [DataMember]
            public string name2;
        }
        [ServiceContract]
        public class Service
        {
            [WebGet(UriTemplate = "/{name1}/{name2}")]
            public MyType GetPerson(string name1, string name2)
            {
                if (name2.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                {
                    name2 = name2.Substring(0, name2.Length - 4);
                    WebOperationContext.Current.OutgoingResponse.Format = WebMessageFormat.Xml;
                }
                else
                {
                    WebOperationContext.Current.OutgoingResponse.Format = WebMessageFormat.Json;
                }

                return new MyType { name1 = name1, name2 = name2 };
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            WebServiceHost host = new WebServiceHost(typeof(Service), new Uri(baseAddress));
            host.Open();
            Console.WriteLine("Host opened");

            Console.WriteLine(new WebClient().DownloadString(baseAddress + "/resource1/resource2.xml"));
            Console.WriteLine();
            Console.WriteLine(new WebClient().DownloadString(baseAddress + "/resource1/resource2"));

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    // http://stackoverflow.com/q/7506072/751090
    public class StackOverflow_7506072
    {
        [DataContract(Name = "MyClass1", Namespace = "")]
        public class MyClass1
        {
            [DataMember]
            public string MyProperty { get; set; }
        }
        [ServiceContract]
        public class Service
        {
            [WebInvoke(UriTemplate = "", Method = "POST")]
            public MyClass1 GetItem(MyClass1 postedItem) { return postedItem; }
        }
        public class MyInspector : IEndpointBehavior, IDispatchMessageInspector
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
                MemoryStream ms = new MemoryStream();
                XmlWriter w = XmlWriter.Create(ms);
                request.WriteMessage(w);
                w.Flush();
                ms.Position = 0;
                XElement element = XElement.Load(ms);
                if (element.Name.NamespaceName == "http://schemas.datacontract.org/2004/07/MyNamespace")
                {
                    element.Name = XName.Get(element.Name.LocalName, "");
                    foreach (XElement child in element.Descendants())
                    {
                        if (child.Name.NamespaceName == "http://schemas.datacontract.org/2004/07/MyNamespace")
                        {
                            child.Name = XName.Get(child.Name.LocalName, "");
                        }
                    }

                    element.Attribute("xmlns").Remove();
                }

                XmlReader r = element.CreateReader();
                Message newRequest = Message.CreateMessage(r, int.MaxValue, request.Version);
                newRequest.Properties.CopyProperties(request.Properties);
                request = newRequest;
                return null;
            }

            public void BeforeSendReply(ref Message reply, object correlationState)
            {
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            ServiceEndpoint endpoint = host.AddServiceEndpoint(typeof(Service), new WebHttpBinding(), "");
            endpoint.Behaviors.Add(new WebHttpBehavior());
            endpoint.Behaviors.Add(new MyInspector());
            host.Open();
            Console.WriteLine("Host opened");

            WebClient c = new WebClient();
            c.Headers[HttpRequestHeader.ContentType] = "text/xml";
            string xml = @"<MyClass1>
      <MyProperty>123</MyProperty>
    </MyClass1>";
            Console.WriteLine(c.UploadString(baseAddress + "/", xml));

            c = new WebClient();
            c.Headers[HttpRequestHeader.ContentType] = "text/xml";
            xml = @"<MyClass1 xmlns=""http://schemas.datacontract.org/2004/07/MyNamespace"">
      <MyProperty>123</MyProperty>
    </MyClass1>";
            Console.WriteLine(c.UploadString(baseAddress + "/", xml));

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    // http://stackoverflow.com/q/7525850/751090
    public class StackOverflow_7525850
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
            int Add(int x, int y);
        }
        public class Service : ITest
        {
            public int Add(int x, int y)
            {
                return x + y;
            }
        }
        public static string SendRequest(string uri, string method, string contentType, string body, Dictionary<string, string> headers)
        {
            string responseBody = null;

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(uri);
            req.Method = method;
            if (headers != null)
            {
                foreach (string headerName in headers.Keys)
                {
                    req.Headers[headerName] = headers[headerName];
                }
            }
            if (!String.IsNullOrEmpty(contentType))
            {
                req.ContentType = contentType;
            }

            if (body != null)
            {
                byte[] bodyBytes = Encoding.UTF8.GetBytes(body);
                req.GetRequestStream().Write(bodyBytes, 0, bodyBytes.Length);
                req.GetRequestStream().Close();
            }

            HttpWebResponse resp;
            try
            {
                resp = (HttpWebResponse)req.GetResponse();
            }
            catch (WebException e)
            {
                resp = (HttpWebResponse)e.Response;
            }

            if (resp == null)
            {
                responseBody = null;
                Console.WriteLine("Response is null");
            }
            else
            {
                Console.WriteLine("HTTP/{0} {1} {2}", resp.ProtocolVersion, (int)resp.StatusCode, resp.StatusDescription);
                foreach (string headerName in resp.Headers.AllKeys)
                {
                    Console.WriteLine("{0}: {1}", headerName, resp.Headers[headerName]);
                }
                Console.WriteLine();
                Stream respStream = resp.GetResponseStream();
                if (respStream != null)
                {
                    responseBody = new StreamReader(respStream).ReadToEnd();
                    Console.WriteLine(responseBody);
                }
                else
                {
                    Console.WriteLine("HttpWebResponse.GetResponseStream returned null");
                }
            }

            Console.WriteLine();
            Console.WriteLine("  *-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*  ");
            Console.WriteLine();

            return responseBody;
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), new BasicHttpBinding(), "basic");
            host.AddServiceEndpoint(typeof(ITest), new WebHttpBinding(), "web").Behaviors.Add(new WebHttpBehavior());
            host.Open();
            Console.WriteLine("Host opened");

            string soapBody = @"<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">
      <s:Body>
        <Add xmlns=""http://tempuri.org/"">
          <x>44</x>
          <y>55</y>
        </Add>
      </s:Body>
    </s:Envelope>";
            SendRequest(baseAddress + "/basic", "POST", "text/xml", soapBody, new Dictionary<string, string> { { "SOAPAction", "http://tempuri.org/ITest/Add" } });

            SendRequest(baseAddress + "/web/Add", "POST", "text/xml", "<Add xmlns=\"http://tempuri.org/\"><x>55</x><y>66</y></Add>", null);

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    // http://stackoverflow.com/q/7534084/751090
    public class StackOverflow_7534084
    {
        const string MyPropertyName = "MyProp";
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
                Console.WriteLine("Information from the inspector: {0}", OperationContext.Current.IncomingMessageProperties[MyPropertyName]);
                return text;
            }
        }
        public class MyInspector : IEndpointBehavior, IDispatchMessageInspector
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
                request.Properties[MyPropertyName] = "Something from the inspector";
                return null;
            }

            public void BeforeSendReply(ref Message reply, object correlationState)
            {
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), new BasicHttpBinding(), "").Behaviors.Add(new MyInspector());
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(new BasicHttpBinding(), new EndpointAddress(baseAddress));
            ITest proxy = factory.CreateChannel();
            Console.WriteLine(proxy.Echo("Hello"));

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    // http://stackoverflow.com/q/7534955/751090
    public class StackOverflow_7534955
    {
        [XmlRoot("Plan")]
        public class EPlan
        {
            [XmlElement("Error")]
            public string Error { get; set; }

            [XmlElement("Description")]
            public string Description { get; set; }

            [XmlElement("Document")]
            public List<EDocument> Documents { get; set; }
        }

        [XmlType]
        public class EDocument
        {
            private string document;

            [XmlAnyElement]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public XmlElement[] DocumentNodes { get; set; }

            [XmlIgnore]
            public string Document
            {
                get
                {
                    if (this.document == null)
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach (var node in this.DocumentNodes)
                        {
                            sb.Append(node.OuterXml);
                        }

                        this.document = sb.ToString();
                    }

                    return this.document;
                }
            }
        }

        public static void Test()
        {
            string xml = @"<Plan>
<Error>0</Error>
<Description>1</Description>
<Document>
  <ObjectID>06098INF1761320</ObjectID>
  <ced>109340336</ced>
  <abstract>DAVID STEVENSON</abstract>
  <ced_a />
  <NAM_REC />
  <ced_ap2 />
</Document>
<Document>
  <ObjectID>id2</ObjectID>
  <ced>ced2</ced>
  <abstract>abstract2</abstract>
  <ced_a />
  <NAM_REC />
  <ced_ap2 />
</Document>
</Plan>";
            XmlSerializer xs = new XmlSerializer(typeof(EPlan));
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(xml));
            EPlan obj = xs.Deserialize(ms) as EPlan;
            Console.WriteLine(obj.Documents[0].Document);
        }
    }

    public class Post_89c6b72c_83f0_4971_a4e6_cdec3f4c0e23
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            Stream DownloadFile(string fileName);
            [OperationContract]
            int CountBytes(Stream fileContents);
        }
        public class Service : ITest
        {
            public Stream DownloadFile(string fileName)
            {
                return new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            }

            public int CountBytes(Stream fileContents)
            {
                MemoryStream ms = new MemoryStream();
                fileContents.CopyTo(ms);
                Console.WriteLine("In Service.CopyBytes, file contents:");
                Util.PrintBytes(ms.ToArray());
                return (int)ms.Length;
            }
        }
        static Binding GetBinding()
        {
            var result = new WSHttpBinding(SecurityMode.None);
            var custom = new CustomBinding(result);
            custom.Elements.Find<HttpTransportBindingElement>().TransferMode = TransferMode.Buffered;
            return custom;
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), GetBinding(), "");
            host.Open();
            Console.WriteLine("Host opened");

            string filePath = "c:\\temp\\mytest.txt";
            File.WriteAllText(filePath, "this is the content of my file");
            Console.WriteLine("Created a test file.");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(GetBinding(), new EndpointAddress(baseAddress));
            ITest proxy = factory.CreateChannel();
            using (FileStream fs = File.OpenRead(filePath))
            {
                Console.WriteLine("Bytes in the file (upload): {0}", proxy.CountBytes(fs));
            }

            using (Stream s = proxy.DownloadFile(filePath))
            {
                MemoryStream ms = new MemoryStream();
                s.CopyTo(ms);
                Console.WriteLine("Bytes read from the server:");
                Util.PrintBytes(ms.ToArray());
            }

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_436969aa_467b_4169_b0f4_dc437e9801f1
    {
        [ServiceContract]
        public interface IServiceInfo
        {
            [OperationContract]
            [Description("This method returns a string containing the current service version number.")]
            string GetCurrentServiceVersion();
        }
        public class Service : IServiceInfo
        {
            public string GetCurrentServiceVersion()
            {
                return "1.0";
            }
        }
        public class MyBehavior : IEndpointBehavior
        {
            public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
            {
            }

            public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
            {
            }

            public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
            {
                foreach (OperationDescription operation in endpoint.Contract.Operations)
                {
                    MethodInfo method = operation.SyncMethod ?? operation.BeginMethod;
                    if (method != null)
                    {
                        DescriptionAttribute descriptionAttribute = method.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault() as DescriptionAttribute;
                        if (descriptionAttribute != null)
                        {
                            Console.WriteLine("Description for operation {0}: {1}", operation.Name, descriptionAttribute.Description);
                        }
                    }
                }
            }

            public void Validate(ServiceEndpoint endpoint)
            {
            }
        }
        public static void Test()
        {
            Console.WriteLine("Simple reflection");
            MethodInfo method = typeof(IServiceInfo).GetMethod("GetCurrentServiceVersion");
            DescriptionAttribute descriptionAttribute = method.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault() as DescriptionAttribute;
            if (descriptionAttribute != null)
            {
                Console.WriteLine("Description: {0}", descriptionAttribute.Description);
            }

            Console.WriteLine();
            Console.WriteLine("Now getting the description inside a behavior");
            ServiceHost host = new ServiceHost(typeof(Service), new Uri("http://localhost:8000/Service"));
            ServiceEndpoint endpoint = host.AddServiceEndpoint(typeof(IServiceInfo), new BasicHttpBinding(), "");
            endpoint.Behaviors.Add(new MyBehavior());
            host.Open();
        }
    }

    // http://stackoverflow.com/q/7607564/751090
    public class StackOverflow_7607564
    {
        [System.ServiceModel.ServiceContractAttribute(Namespace = "http://mynamespace.com/", ConfigurationName = "ConfigName")]
        public interface MyInterfacePort
        {
            [System.ServiceModel.OperationContractAttribute(Action = "http://mynamespace.com/opName", ReplyAction = "*")]
            [System.ServiceModel.FaultContractAttribute(typeof(MyError), Action = "http://mynamespace.com/opName", Name = "opErr")]
            [System.ServiceModel.XmlSerializerFormatAttribute()]
            opNameResponse opName(opNameRequest request);
        }
        public class MyError { }
        [MessageContract(IsWrapped = false)]
        public class opNameRequest
        {
            [MessageBodyMember(Name = "opName")]
            public opRequest request;
        }
        [MessageContract(IsWrapped = false)]
        public class opNameResponse
        {
            [MessageBodyMember(Name = "opNameResponse")]
            public opResponse response;
        }
        [System.Serializable]
        public partial class opRequest
        {
            public string myProperty;
        }
        [System.Serializable]
        public partial class opResponse
        {
            public string myProperty;
        }
        public class Service : MyInterfacePort
        {
            public opNameResponse opName(opNameRequest request)
            {
                return new opNameResponse { response = new opResponse { myProperty = request.request.myProperty } };
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(MyInterfacePort), new BasicHttpBinding(), "");
            host.Open();
            Console.WriteLine("Host opened");

            var factory = new ChannelFactory<MyInterfacePort>(new BasicHttpBinding(), new EndpointAddress(baseAddress));
            var proxy = factory.CreateChannel();
            Console.WriteLine(proxy.opName(new opNameRequest { request = new opRequest { myProperty = "hello world" } }).response.myProperty);

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_0acbfef2_16a3_440a_88d6_e0d7fcf90a8e
    {
        [DataContract(Name = "Person", Namespace = "")]
        public class Person
        {
            [DataMember]
            public string Name { get; set; }
            [DataMember]
            public int Age { get; set; }
        }
        [ServiceContract]
        public class MyContentNegoService
        {
            [WebGet(ResponseFormat = WebMessageFormat.Xml)]
            public Person ResponseFormatXml()
            {
                return new Person { Name = "John Doe", Age = 33 };
            }
            [WebGet(ResponseFormat = WebMessageFormat.Json)]
            public Person ResponseFormatJson()
            {
                return new Person { Name = "John Doe", Age = 33 };
            }
            [WebGet]
            public Person ContentNegotiated()
            {
                return new Person { Name = "John Doe", Age = 33 };
            }
            [WebInvoke]
            public Person ContentNegotiatedPost(Person person)
            {
                return person;
            }
        }
        class MyVaryAddingInspector : IEndpointBehavior, IDispatchMessageInspector
        {
            public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
            {
            }

            public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
            {
            }

            public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
            {
                WebHttpBehavior webBehavior = endpoint.Behaviors.Find<WebHttpBehavior>();
                if (webBehavior != null && webBehavior.AutomaticFormatSelectionEnabled)
                {
                    endpointDispatcher.DispatchRuntime.MessageInspectors.Add(this);
                }
            }

            public void Validate(ServiceEndpoint endpoint)
            {
            }

            public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
            {
                HttpRequestMessageProperty prop;
                prop = (HttpRequestMessageProperty)request.Properties[HttpRequestMessageProperty.Name];
                if (prop.Method == "GET")
                {
                    // we shouldn't cache non-GET requests, so only returning this for such requests
                    return "Accept";
                }

                return null;
            }

            public void BeforeSendReply(ref Message reply, object correlationState)
            {
                string varyHeader = correlationState as string;
                if (varyHeader != null)
                {
                    HttpResponseMessageProperty prop;
                    prop = reply.Properties[HttpResponseMessageProperty.Name] as HttpResponseMessageProperty;
                    if (prop != null)
                    {
                        prop.Headers[HttpResponseHeader.Vary] = varyHeader;
                    }
                }
            }
        }
        public static void SendGetRequest(string uri, string acceptHeader)
        {
            SendRequest(uri, "GET", null, null, acceptHeader);
        }
        public static void SendRequest(string uri, string method, string contentType, string body, string acceptHeader)
        {
            Console.Write("{0} request to {1}", method, uri.Substring(uri.LastIndexOf('/')));
            if (contentType != null)
            {
                Console.Write(" with Content-Type:{0}", contentType);
            }

            if (acceptHeader == null)
            {
                Console.WriteLine(" (no Accept header)");
            }
            else
            {
                Console.WriteLine(" (with Accept: {0})", acceptHeader);
            }

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(uri);
            req.Method = method;
            if (contentType != null)
            {
                req.ContentType = contentType;
                Stream reqStream = req.GetRequestStream();
                byte[] bodyBytes = Encoding.UTF8.GetBytes(body);
                reqStream.Write(bodyBytes, 0, bodyBytes.Length);
                reqStream.Close();
            }

            if (acceptHeader != null)
            {
                req.Accept = acceptHeader;
            }

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
            Console.WriteLine("  *-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*  ");
            Console.WriteLine();
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(MyContentNegoService), new Uri(baseAddress));
            ServiceEndpoint endpoint = host.AddServiceEndpoint(typeof(MyContentNegoService), new WebHttpBinding(), "");
            endpoint.Behaviors.Add(new WebHttpBehavior { AutomaticFormatSelectionEnabled = true });
            endpoint.Behaviors.Add(new MyVaryAddingInspector());
            host.Open();
            Console.WriteLine("Host opened");

            foreach (string operation in new string[] { "ResponseFormatJson", "ResponseFormatXml", "ContentNegotiated" })
            {
                foreach (string acceptHeader in new string[] { null, "application/json", "text/xml", "text/json" })
                {
                    SendGetRequest(baseAddress + "/" + operation, acceptHeader);
                }
            }

            Console.WriteLine("Sending some POST requests with content-nego (but no Vary in response)");
            string jsonBody = "{\"Name\":\"John Doe\",\"Age\":33}";
            SendRequest(baseAddress + "/ContentNegotiatedPost", "POST", "text/json", jsonBody, "text/xml");
            SendRequest(baseAddress + "/ContentNegotiatedPost", "POST", "text/json", jsonBody, "text/json");

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_9bf860f1_2a01_4b26_8c79_2f6346191dc9
    {
        [DataContract(Name = "Person", Namespace = "")]
        public class Person
        {
            [DataMember]
            public string Name { get; set; }
            [DataMember]
            public int Age { get; set; }
        }
        [ServiceContract(Name = "ITest")]
        public interface ITest
        {
            [OperationContract]
            Person Create(string name, int age);
        }
        [ServiceContract(Name = "ITest")]
        public interface ITestClient
        {
            [OperationContract]
            XElement Create(string name, int age);
        }
        public class Service : ITest
        {
            public Person Create(string name, int age)
            {
                return new Person { Name = name, Age = age };
            }
        }
        public class MyBehavior : IOperationBehavior
        {
            public void AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters)
            {
            }

            public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
            {
                clientOperation.Formatter = new MyFormatter(clientOperation.Formatter);
            }

            public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
            {
            }

            public void Validate(OperationDescription operationDescription)
            {
                if ((operationDescription.Messages.Count < 2) ||
                    (operationDescription.Messages[1].Body.ReturnValue.Type != typeof(XElement)) ||
                    (operationDescription.Messages[1].Body.Parts.Count > 0))
                {
                    throw new InvalidOperationException("This behavior can only be applied to operations with a single return value of type XElement");
                }
            }
        }
        public class MyFormatter : IClientMessageFormatter
        {
            IClientMessageFormatter original;
            public MyFormatter(IClientMessageFormatter original)
            {
                this.original = original;
            }

            public object DeserializeReply(Message message, object[] parameters)
            {
                XmlReader bodyReader = message.GetReaderAtBodyContents();
                XElement result = XElement.Load(bodyReader.ReadSubtree());
                return result;
            }

            public Message SerializeRequest(MessageVersion messageVersion, object[] parameters)
            {
                return this.original.SerializeRequest(messageVersion, parameters);
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), new WSHttpBinding(SecurityMode.None), "");
            host.Open();
            Console.WriteLine("Host opened");

            var factory = new ChannelFactory<ITestClient>(new WSHttpBinding(SecurityMode.None), new EndpointAddress(baseAddress));
            factory.Endpoint.Contract.Operations.Find("Create").Behaviors.Add(new MyBehavior());
            var proxy = factory.CreateChannel();
            Console.WriteLine(proxy.Create("John Doe", 33));

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    // http://stackoverflow.com/q/7771645/751090
    public class StackOverflow_7771645
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            string Process();
        }
        public class Service : ITest
        {
            public string Process()
            {
                return "Request content type: " + WebOperationContext.Current.IncomingRequest.ContentType;
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
            using (new OperationContextScope((IContextChannel)proxy))
            {
                WebOperationContext.Current.OutgoingRequest.ContentType = "text/xml";
                Console.WriteLine(proxy.Process());
            }

            using (new OperationContextScope((IContextChannel)proxy))
            {
                WebOperationContext.Current.OutgoingRequest.ContentType = "application/xml";
                Console.WriteLine(proxy.Process());
            }

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    // http://stackoverflow.com/q/7760551/751090
    public class StackOverflow_7760551
    {
        [DataContract]
        public class Person
        {
            [DataMember]
            public string Name { get; set; }
            [DataMember]
            public int Age { get; set; }

            public override string ToString()
            {
                return string.Format("Person[Name={0},Age={1}]", this.Name, this.Age);
            }
        }

        public static void Test()
        {
            const string fileName = "test.xml";
            using (FileStream fs = File.Create(fileName))
            {
                Person[] people = new Person[]
                { 
                    new Person { Name = "John", Age = 33 },
                    new Person { Name = "Jane", Age = 28 },
                    new Person { Name = "Jack", Age = 23 }
                };

                foreach (Person p in people)
                {
                    XmlWriterSettings ws = new XmlWriterSettings
                    {
                        Indent = true,
                        IndentChars = "  ",
                        OmitXmlDeclaration = true,
                        Encoding = new UTF8Encoding(false),
                        CloseOutput = false,
                    };
                    using (XmlWriter w = XmlWriter.Create(fs, ws))
                    {
                        DataContractSerializer dcs = new DataContractSerializer(typeof(Person));
                        dcs.WriteObject(w, p);
                    }
                }
            }

            Console.WriteLine(File.ReadAllText(fileName));

            using (FileStream fs = File.OpenRead(fileName))
            {
                XmlReaderSettings rs = new XmlReaderSettings
                {
                    ConformanceLevel = ConformanceLevel.Fragment,
                };
                XmlReader r = XmlReader.Create(fs, rs);
                while (!r.EOF)
                {
                    Person p = new DataContractSerializer(typeof(Person)).ReadObject(r) as Person;
                    Console.WriteLine(p);
                }
            }

            File.Delete(fileName);
        }
    }

    public class Post_c90b0114_8dac_4996_a24d_aa08122aaf16
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
        public static void Test(params string[] args)
        {
            string baseAddress = "net.pipe://localhost/Service";
            ServiceHost host = null;
            bool clientOnly = (args.Length > 0 && args[0].StartsWith("c", StringComparison.OrdinalIgnoreCase));
            if (!clientOnly)
            {
                host = new ServiceHost(typeof(Service), new Uri(baseAddress));
                host.AddServiceEndpoint(typeof(ITest), new NetNamedPipeBinding(NetNamedPipeSecurityMode.None), "");
                host.Open();
                Console.WriteLine("Host opened");
            }

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(
                new NetNamedPipeBinding(NetNamedPipeSecurityMode.None),
                new EndpointAddress(baseAddress));
            ITest proxy = factory.CreateChannel();
            Console.WriteLine(proxy.Echo("Hello"));

            ((IClientChannel)proxy).Close();
            factory.Close();

            if (!clientOnly)
            {
                Console.Write("Press ENTER to close the host");
                Console.ReadLine();
                host.Close();
            }
        }
    }

    // http://stackoverflow.com/q/7836645/751090
    public class StackOverflow_7836645
    {
        [XmlRoot("axf", Namespace = Axf10Namespace)]
        public class AxfDocument : IXmlSerializable
        {
            public const string Axf10Namespace = "http://schemas.something.ru/axf/axf-1.0.0";

            public XmlSchema GetSchema()
            {
                return null;
            }

            public void ReadXml(XmlReader reader)
            {
            }

            public void WriteXml(XmlWriter writer)
            {
                writer.WriteStartElement("intineraries", Axf10Namespace);
                writer.WriteElementString("item", Axf10Namespace, "one value");
                writer.WriteElementString("item", Axf10Namespace, "another value");
                writer.WriteEndElement();
            }
        }

        [MessageContract(IsWrapped = false)]
        public class OperationResponse
        {
            [MessageBodyMember(Name = "axf", Namespace = AxfDocument.Axf10Namespace)]
            public AxfDocument axf;
        }

        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            OperationResponse GetAxf();
        }

        public class Service : ITest
        {
            public OperationResponse GetAxf()
            {
                return new OperationResponse { axf = new AxfDocument() };
            }
        }

        public static void Test()
        {
            Console.WriteLine("Serialization");
            MemoryStream ms = new MemoryStream();
            XmlSerializer xs = new XmlSerializer(typeof(AxfDocument));
            xs.Serialize(ms, new AxfDocument());
            Console.WriteLine(Encoding.UTF8.GetString(ms.ToArray()));

            Console.WriteLine();
            Console.WriteLine("Service");

            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), new BasicHttpBinding(), "");
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(new BasicHttpBinding(), new EndpointAddress(baseAddress));
            ITest proxy = factory.CreateChannel();
            Console.WriteLine(proxy.GetAxf());

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_2e4c484c_eb8e_4b22_94ed_bd551072bf21
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
            string baseAddressHttp = "http://" + Environment.MachineName + ":8000/Service";
            string baseAddressTcp = "net.tcp://" + Environment.MachineName + ":8500/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddressHttp), new Uri(baseAddressTcp));
            host.AddServiceEndpoint(typeof(ITest), new NetTcpBinding(SecurityMode.None), "");
            host.Description.Behaviors.Add(new ServiceMetadataBehavior { HttpGetEnabled = true });
            host.Open();
            Console.WriteLine("Host opened");
            Console.WriteLine("Browse to '{0}?wsdl' to see the service metadata", baseAddressHttp);

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_7cb0ff86_5fe1_4266_afac_bcb91eaca5ec
    {
        [DataContract()]
        public partial class TestAttachment
        {
            private byte[] fileField;
            private string filenameField;

            [DataMember()]
            public byte[] File
            {
                get
                {
                    return this.fileField;
                }
                set
                {
                    this.fileField = value;
                }
            }
            [DataMember()]
            public string Filename
            {
                get
                {
                    return this.filenameField;
                }
                set
                {
                    this.filenameField = value;
                }
            }
        }
        public static void Test()
        {
            byte[] simulatedFile = new byte[1000];
            new Random().NextBytes(simulatedFile);
            string Filename = "Image.jpg";

            TestAttachment Attachment = new TestAttachment();
            Attachment.Filename = Filename;
            Attachment.File = simulatedFile;
            MemoryStream MTOMInMemory = new MemoryStream();
            XmlDictionaryWriter TW = XmlDictionaryWriter.CreateMtomWriter(MTOMInMemory, Encoding.UTF8, Int32.MaxValue, "");
            DataContractSerializer DCS = new DataContractSerializer(Attachment.GetType());
            DCS.WriteObject(TW, Attachment);
            TW.Flush();
            Console.WriteLine(Encoding.UTF8.GetString(MTOMInMemory.ToArray()));
        }
    }

    // http://stackoverflow.com/q/7905186/751090
    public class StackOverflow_7905186
    {
        [XmlType(TypeName = "DataMessage", Namespace = "http://tempuri.org/")]
        public class DataMessage
        {
            public string a;
            public string b;
            public string c;
        }
        [XmlRoot(ElementName = "DataMessages", Namespace = "http://tempuri.org/")]
        public class DataMessages
        {
            [XmlElement(ElementName = "DataMessage")]
            public List<DataMessage> Messages;
        }
        [ServiceContract]
        public class Service
        {
            [XmlSerializerFormat]
            [OperationContract(Name = "GetData")]
            [WebGet(ResponseFormat = WebMessageFormat.Xml,
                    BodyStyle = WebMessageBodyStyle.Bare,
                    UriTemplate = "Data/{Param}")]
            [return: MessageParameter(Name = "DataMessages")]
            public DataMessages GetData(string Param)
            {
                return new DataMessages
                {
                    Messages = new List<DataMessage>
                    {
                        new DataMessage
                        {
                            a = "1",
                            b = "2",
                            c = "3",
                        }
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

            WebClient c = new WebClient();
            Console.WriteLine(c.DownloadString(baseAddress + "/Data/foo"));

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    // http://stackoverflow.com/q/7909261/751090
    public class StackOverflow_7909261
    {
        [DataContract]
        [KnownType(typeof(RegexOptions))]
        public class InboundMailbox
        {
            public const char EmailSeparator = ';';

            [DataMember]
            public string POP3Host { get; set; }

            [DataMember]
            public string EmailId { get; set; }

            [DataMember]
            public string WebServiceURL { get; set; }

            [DataMember]
            public List<Regex> Allowed { get; set; }

            [DataMember]
            public List<Regex> Disallowed { get; set; }
        }

        public static void Test()
        {
            MemoryStream ms = new MemoryStream();
            InboundMailbox obj = new InboundMailbox
            {
                POP3Host = "popHost",
                EmailId = "email",
                WebServiceURL = "http://web.service",
                Allowed = new List<Regex>
                {
                    new Regex("abcdef", RegexOptions.IgnoreCase),
                },
                Disallowed = null,
            };
            DataContractSerializer dcs = new DataContractSerializer(typeof(InboundMailbox));
            try
            {
                dcs.WriteObject(ms, obj);
                Console.WriteLine(Encoding.UTF8.GetString(ms.ToArray()));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }

    // http://stackoverflow.com/q/7919718/751090
    public class StackOverflow_7919718
    {
        [ServiceContract]
        public class Service
        {
            [WebGet(ResponseFormat = WebMessageFormat.Json)]
            public string GetData()
            {
                Console.WriteLine("If-Modified-Since header (1): {0}", WebOperationContext.Current.IncomingRequest.IfModifiedSince);
                WebOperationContext.Current.IncomingRequest.CheckConditionalRetrieve(DateTime.UtcNow);
                return "Data";
            }
        }

        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            WebServiceHost host = new WebServiceHost(typeof(Service), new Uri(baseAddress));
            host.Open();
            Console.WriteLine("Host opened");

            Console.WriteLine("Not sending If-Modified-Since header (should return 200):");
            Util.SendRequest(baseAddress + "/GetData", "GET", null, null);

            Console.WriteLine("Sending data in the past, ISO 8601 format (should return 200):");
            Util.SendRequest(baseAddress + "/GetData", "GET", null, null,
                new Dictionary<string, string> { { "If-Modified-Since", "2011-10-25T13:09:39.6242263-04:00" } });

            Console.WriteLine("Sending data in the future, ISO 8601 format (should return 304):");
            Util.SendRequest(baseAddress + "/GetData", "GET", null, null,
                new Dictionary<string, string> { { "If-Modified-Since", "2021-10-25T13:09:39.6242263-04:00" } });

            Console.WriteLine("Sending data in the past, RFC 1123 format (should return 200):");
            Util.SendRequest(baseAddress + "/GetData", "GET", null, null,
                new Dictionary<string, string> { { "If-Modified-Since", "Wed, 26 Oct 2011 01:00:00 GMT" } });

            Console.WriteLine("Sending data in the future, RFC 1123 format (should return 304):");
            Util.SendRequest(baseAddress + "/GetData", "GET", null, null,
                new Dictionary<string, string> { { "If-Modified-Since", "Mon, 27 Oct 2031 10:00:00 GMT" } });

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_3bb9b3ad_8fc4_40a6_9bb8_fb34f9011d39
    {
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(SampleService), new Uri(baseAddress));
            ServiceMetadataBehavior smb = host.Description.Behaviors.Find<ServiceMetadataBehavior>();
            if (smb == null)
            {
                host.Description.Behaviors.Add(smb = new ServiceMetadataBehavior());
            }

            smb.HttpGetEnabled = true;
            host.Open();
            Console.WriteLine("Host opened");

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    // http://stackoverflow.com/q/7930629/751090
    public class StackOverflow_7930629
    {
        [DataContract]
        public class Person
        {
            public Person() { }
            public Person(string firstname, string lastname)
            {
                this.FirstName = firstname;
                this.LastName = lastname;
            }

            [DataMember]
            public string FirstName { get; set; }

            [DataMember]
            public string LastName { get; set; }
        }

        public static string Serialize<T>(T obj)
        {
            DataContractJsonSerializer serializer =
                new DataContractJsonSerializer(typeof(T), typeof(T).Name);
            MemoryStream ms = new MemoryStream();
            XmlDictionaryWriter w = JsonReaderWriterFactory.CreateJsonWriter(ms);
            w.WriteStartElement("root");
            w.WriteAttributeString("type", "object");
            serializer.WriteObject(w, obj);
            w.WriteEndElement();
            w.Flush();
            string retVal = Encoding.Default.GetString(ms.ToArray());
            ms.Dispose();
            return retVal;
        }
        public static void Test()
        {
            Console.WriteLine(Serialize(new Person("Jane", "McDoe")));
        }
    }

    public class Post_3f28ebac_018a_4b67_becc_5abff4315d3f
    {
        public enum CurrencyCode
        {
            USD, OTH
        }
        [XmlType(TypeName = "Amount", Namespace = "")]
        public class Amount
        {
            [XmlAttribute(AttributeName = "currencyCode")]
            public CurrencyCode currencyCode;
            [XmlText]
            public string Value;

            public override string ToString()
            {
                return string.Format("Amount[currencyCode={0},value={1}", currencyCode, Value ?? "<<NULL>>");
            }
        }
        public static void Test()
        {
            MemoryStream ms = new MemoryStream();
            XmlSerializer xs = new XmlSerializer(typeof(Amount));
            xs.Serialize(ms, new Amount { currencyCode = CurrencyCode.USD, Value = null });
            Console.WriteLine(Encoding.UTF8.GetString(ms.ToArray()));
            ms.Position = 0;
            Amount amt = (Amount)xs.Deserialize(ms);
            Console.WriteLine(amt);
        }
    }

    public class Post_f75e08f6_7661_4f08_9b95_13ca44472d5b
    {
        [DataContract(Name = "Person", Namespace = "")]
        public class Person
        {
            [DataMember]
            public string Name { get; set; }
            [DataMember]
            public int Age { get; set; }
        }

        [CollectionDataContract(Name = "MyCollectionOf{0}")]
        public class MyCollection<T> : List<T>
        {
            public MyCollection() : base() { }
            public MyCollection(IEnumerable<T> collection) : base(collection) { }
        }
        
        [ServiceContract]
        public interface IPersonService
        {
            [OperationContract()]
            [WebInvoke(UriTemplate = "/person/new", Method = "POST",
                RequestFormat = WebMessageFormat.Xml,
                ResponseFormat = WebMessageFormat.Xml)]
            void AddNewPerson(Person newperson);

            [OperationContract]
            [WebGet(UriTemplate = "/all/",
                RequestFormat = WebMessageFormat.Xml,
                ResponseFormat = WebMessageFormat.Xml)]
            MyCollection<Person> ListAllPersons();
        }

        public class PersonService : IPersonService
        {
            static List<Person> allPeople = new List<Person>();

            public void AddNewPerson(Person newPerson)
            {
                allPeople.Add(newPerson);
            }

            public MyCollection<Person> ListAllPersons()
            {
                return new MyCollection<Person>(allPeople);
            }
        }

        static ServiceHost host;
        static string baseAddress = "http://" + Environment.MachineName + ":8000/Service";

        public static void Test()
        {
            // For this example only, self-hosting
            StartService();

            // on an IIS-hosted service, baseAddress is the address to the .svc file
            string addPersonOperationAddress = baseAddress + "/person/new";

            // We can send either JSON or XML request.
            Util.SendRequest(addPersonOperationAddress, "POST", "application/json", "{\"Name\":\"John Doe\",\"Age\":33}");
            Util.SendRequest(addPersonOperationAddress, "POST", "text/xml", "<Person><Age>32</Age><Name>Jane Roe</Name></Person>");

            // Now getting all people
            string getAllPersonsOperationAddress = baseAddress+ "/all/";
            Util.SendRequest(getAllPersonsOperationAddress, "GET", null, null);

            Console.WriteLine("Press ENTER to close service");
            Console.ReadLine();
            CloseService();
        }

        private static void CloseService()
        {
            host.Close();
        }

        private static void StartService()
        {
            host = new ServiceHost(typeof(PersonService), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(IPersonService), new WebHttpBinding(), "").Behaviors.Add(new WebHttpBehavior());
            host.Open();
        }
    }

    public class TestChangingBinding
    {
        class Client : ClientBase<ISampleContract>, ISampleContract
        {
            public Client(Binding binding, EndpointAddress endpointAddress)
                : base(binding, endpointAddress)
            {
            }

            public int Add(int x, int y)
            {
                return base.Channel.Add(x, y);
            }
        }
        public static void Test()
        {
            BasicHttpBinding binding = new BasicHttpBinding();
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(SampleService), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ISampleContract), binding, "");
            host.Open();
            Console.WriteLine("Host opened");

            Client proxy = new Client(binding, new EndpointAddress(baseAddress));
            Console.WriteLine(proxy.Add(4,5));
            proxy.Close();

            binding.TextEncoding = Encoding.Unicode;
            proxy = new Client(binding, new EndpointAddress(baseAddress));
            Console.WriteLine(proxy.Add(4, 5));
            proxy.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_300cd5b9_ad18_446f_b1d9_49f124d09128
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            string GetData(string data, string address);
        }
        public class Service : ITest
        {
            public string GetData(string data, string address)
            {
                if (address == null)
                {
                    return "Data from " + Environment.MachineName + ": " + data;
                }
                else
                {
                    ChannelFactory<ITest> factory = new ChannelFactory<ITest>(new BasicHttpBinding(), new EndpointAddress(address));
                    ITest proxy = factory.CreateChannel();
                    string fromOtherServer = proxy.GetData(data, null);
                    ((IClientChannel)proxy).Close();
                    factory.Close();
                    return "Data via " + Environment.MachineName + ": " + fromOtherServer;
                }
            }
        }
        public static void Test()
        {
            string baseAddress1 = "http://" + Environment.MachineName + ":8000/Service";
            string baseAddress2 = "http://" + Environment.MachineName + ":8001/Service";
            ServiceHost host1 = new ServiceHost(typeof(Service), new Uri(baseAddress1));
            ServiceHost host2 = new ServiceHost(typeof(Service), new Uri(baseAddress2));
            host1.AddServiceEndpoint(typeof(ITest), new BasicHttpBinding(), "");
            host2.AddServiceEndpoint(typeof(ITest), new BasicHttpBinding(), "");
            host1.Open();
            host2.Open();
            Console.WriteLine("Hosts opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(new BasicHttpBinding(), new EndpointAddress(baseAddress1));
            ITest proxy = factory.CreateChannel();

            Console.WriteLine("Call to server without going to the other");
            Console.WriteLine(proxy.GetData("hello", null));
            Console.WriteLine();

            Console.WriteLine("Call to server without going to the other server");
            Console.WriteLine(proxy.GetData("hello", baseAddress2));
            Console.WriteLine();

            ((IClientChannel)proxy).Close();
            factory.Close();
            host1.Close();
            host2.Close();
        }
    }

    // http://stackoverflow.com/q/8010677/751090
    public class StackOverflow_8010677
    {
        [DataContract(Name = "Person", Namespace = "")]
        public class Person
        {
            [DataMember]
            public string Name;
            [DataMember(EmitDefaultValue = false)]
            public int Age;

            private int ageSaved;
            [OnSerializing]
            void OnSerializing(StreamingContext context)
            {
                this.ageSaved = this.Age;
                this.Age = default(int); // will not be serialized
            }
            [OnSerialized]
            void OnSerialized(StreamingContext context)
            {
                this.Age = this.ageSaved;
            }

            public override string ToString()
            {
                return string.Format("Person[Name={0},Age={1}]", this.Name, this.Age);
            }
        }

        public static void Test()
        {
            Person p1 = new Person { Name = "Jane Roe", Age = 23 };
            MemoryStream ms = new MemoryStream();
            DataContractSerializer dcs = new DataContractSerializer(typeof(Person));
            Console.WriteLine("Serializing: {0}", p1);
            dcs.WriteObject(ms, p1);
            Console.WriteLine("   ==> {0}", Encoding.UTF8.GetString(ms.ToArray()));
            Console.WriteLine("   ==> After serialization: {0}", p1);
            Console.WriteLine();
            Console.WriteLine("Deserializing a XML which contains the Age member");
            const string XML = "<Person><Age>33</Age><Name>John Doe</Name></Person>";
            Person p2 = (Person)dcs.ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(XML)));
            Console.WriteLine("  ==> {0}", p2);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            StackOverflow_8010677.Test();
        }
    }
}
