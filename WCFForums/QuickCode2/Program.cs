using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IdentityModel.Claims;
using System.IdentityModel.Policy;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Security.AccessControl;
using System.Security.Permissions;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Security;
using System.ServiceModel.Syndication;
using System.ServiceModel.Web;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;
using UtilCS;
using System.Web;

namespace QuickCode2
{
    // Endpoints collection indexer
    public class Post1598492
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            string Echo(string str);
        }
        public class Test : ITest
        {
            public string Echo(string str)
            {
                return str;
            }
            public static void Run()
            {
                Configuration appConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                ServiceModelSectionGroup serviceModelSectionGroup = ServiceModelSectionGroup.GetSectionGroup(appConfig);
                ServiceElement serviceElement = serviceModelSectionGroup.Services.Services["WcfForums.Test"];
                foreach (object obj in serviceElement.Endpoints)
                {
                    Console.WriteLine(obj);
                }
                ServiceEndpointElement serviceEndpointElement = serviceElement.Endpoints["WS"];
                Console.WriteLine(serviceEndpointElement);
            }
        }
    }

    #region Post 1599621 - IList as editable list
    public class Post1599621
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            Data GetData();
        }
        [DataContract]
        [KnownType("GetKnownTypes")]
        public class Data
        {
            [DataMember(
                 Name = "Strings",
                 IsRequired = true)]
            private IList<string> _strings;
            public IList<string> Strings
            {
                get { return _strings; }
                set { _strings = value; }
            }

            public static Type[] GetKnownTypes()
            {
                return new Type[] { typeof(List<string>) };
            }
        }
        public class Service : ITest
        {
            public Data GetData()
            {
                Data data = new Data();
                data.Strings = new List<string>("ads dasda dasda dasdadw".Split(' '));
                return data;
            }
        }
        public static void Test()
        {
            string baseAddress = "http://localhost:8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), new BasicHttpBinding(), "");
            host.Open();

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(new BasicHttpBinding(), new EndpointAddress(baseAddress));
            ITest proxy = factory.CreateChannel();
            Data data = proxy.GetData();
            data.Strings.Insert(0, "Hello");
            Console.WriteLine(data.Strings.GetType());
            ((IClientChannel)proxy).Close();
            factory.Close();
            host.Close();
        }
    }
    #endregion

    #region Post 1596045 - Properties in Windows Communication Foundation
    public class Post1596045
    {
        [ServiceContract]
        public interface ITest
        {
            string MyStr
            {
                [OperationContract]
                get;
            }
        }
        public class Service : ITest
        {
            public string MyStr
            {
                get { return "Hello"; }
            }
        }
        public static void Test()
        {
            string baseAddress = "http://localhost:8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), new BasicHttpBinding(), "");
            ServiceMetadataBehavior metadataBehavior = new ServiceMetadataBehavior();
            metadataBehavior.HttpGetEnabled = true;
            host.Description.Behaviors.Add(metadataBehavior);
            host.Open();

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(new BasicHttpBinding(), new EndpointAddress(baseAddress));
            ITest proxy = factory.CreateChannel();
            string foo = proxy.MyStr;
            Console.WriteLine("foo = " + foo);
            ((IClientChannel)proxy).Close();
            factory.Close();
            Console.WriteLine("Press ENTER to close service");
            Console.ReadLine();
            host.Close();
        }
    }
    #endregion

    #region Post about image serialization - 1033270
    public class Post1033270
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            [ServiceKnownType(typeof(System.Drawing.Size))]
            Icon Echo(Icon foo);
        }
        public class Service : ITest
        {
            public Icon Echo(Icon foo)
            {
                WebOperationContext.Current.OutgoingResponse.Headers[HttpResponseHeader.ContentLength] = 12345.ToString();
                return foo;
            }
        }
        public static void Test()
        {
            string baseAddress = "http://localhost:8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), new BasicHttpBinding(), "");
            host.Open();

            Icon icon = new Icon("C:\\temp\\hermes.ico");
            Console.WriteLine(icon.Height);
            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(new BasicHttpBinding(), new EndpointAddress(baseAddress));
            ITest proxy = factory.CreateChannel();
            Icon newIcon = proxy.Echo(icon);
            Console.WriteLine(newIcon.Height);
            ((IClientChannel)proxy).Close();
            factory.Close();
            host.Close();
        }
    }
    #endregion

    //JSON and WebInvoke
    public class Post2348599
    {
        [DataContract]
        public class Person
        {
            [DataMember]
            public string Name;
            [DataMember]
            public int Age;
            public Person(string name, int age)
            {
                this.Name = name;
                this.Age = age;
            }
        }
        [DataContract]
        public class PaginationHelper<T>
        {
            [DataMember]
            public List<T> list;
            public PaginationHelper()
            {
                this.list = new List<T>();
            }
        }
        [ServiceContract(Namespace = "Namespace.PersonService", Name = "PersonService")]
        public interface IPersonService
        {
            [OperationContract]
            [WebInvoke(
                BodyStyle = WebMessageBodyStyle.Wrapped,
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json,
                UriTemplate = "/Modify")]
            void Modify(Person person);

            [OperationContract]
            [WebInvoke(
                BodyStyle = WebMessageBodyStyle.Bare,
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json,
                UriTemplate = "/GetAll")]
            PaginationHelper<Person> GetAll();
        }

        public class PersonService : IPersonService
        {
            public void Modify(Person person)
            {
                //do something
            }

            public PaginationHelper<Person> GetAll()
            {
                PaginationHelper<Person> result = new PaginationHelper<Person>();
                result.list.Add(new Person("Alice", 27));
                result.list.Add(new Person("Bob", 30));
                result.list.Add(new Person("Charlie", 25));
                return result;
            }
        }

        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(PersonService), new Uri(baseAddress));
            ServiceEndpoint endpoint = host.AddServiceEndpoint(typeof(IPersonService), new WebHttpBinding(), "");
            endpoint.Behaviors.Add(new WebHttpBehavior());
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<IPersonService> factory = new ChannelFactory<IPersonService>(new WebHttpBinding(), new EndpointAddress(baseAddress));
            factory.Endpoint.Behaviors.Add(new WebHttpBehavior());
            IPersonService proxy = factory.CreateChannel();

            PaginationHelper<Person> helper = proxy.GetAll();
            Console.WriteLine(helper.list.Count);
            proxy.Modify(helper.list[0]);
            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.WriteLine("Press ENTER to close service");
            Console.ReadLine();
            host.Close();
        }
    }

    //Consuming arbitrary XML
    public class Post2348599b
    {
        [ServiceContract]
        public interface ITestService
        {
            [OperationContract, WebInvoke(
                    BodyStyle = WebMessageBodyStyle.Bare,
                    RequestFormat = WebMessageFormat.Xml,
                    ResponseFormat = WebMessageFormat.Xml)]
            string ProcessXml(XmlElement input);
        }

        public class TestService : ITestService
        {
            public string ProcessXml(XmlElement input)
            {
                return input.OuterXml;
            }
        }

        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(TestService), new Uri(baseAddress));
            ServiceEndpoint endpoint = host.AddServiceEndpoint(typeof(ITestService), new WebHttpBinding(), "");
            endpoint.Behaviors.Add(new WebHttpBehavior());
            host.Open();
            Console.WriteLine("Host opened");

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(baseAddress + "/ProcessXml");
            req.Method = "POST";
            req.ContentType = "application/xml; charset=utf-8";
            string reqBody = @"<TextBlock x:Name=""text"" IsHitTestVisible=""false"" Text=""Hello"" Foreground=""black"" xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""/>";
            byte[] reqBodyBytes = Encoding.UTF8.GetBytes(reqBody);
            Stream reqStream = req.GetRequestStream();
            reqStream.Write(reqBodyBytes, 0, reqBodyBytes.Length);
            reqStream.Close();

            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Console.WriteLine("HTTP/{0} {1} {2}", resp.ProtocolVersion, (int)resp.StatusCode, resp.StatusDescription);
            Console.WriteLine(new StreamReader(resp.GetResponseStream()).ReadToEnd());
            resp.Close();

            Console.WriteLine("Press ENTER to close service");
            Console.ReadLine();
            host.Close();
        }
    }

    //Consuming arbitrary XML once more
    public class Post2348599c
    {
        [ServiceContract]
        public interface ITestService
        {
            [OperationContract, WebInvoke]
            string ProcessXml(Stream input);
        }

        public class TestService : ITestService
        {
            public string ProcessXml(Stream input)
            {
                return new StreamReader(input).ReadToEnd();
            }
        }

        public class MyContentTypeMapper : WebContentTypeMapper
        {
            public override WebContentFormat GetMessageFormatForContentType(string contentType)
            {
                return WebContentFormat.Raw;
            }
        }
        static Binding CreateBinding()
        {
            WebMessageEncodingBindingElement webBE = new WebMessageEncodingBindingElement();
            webBE.ContentTypeMapper = new MyContentTypeMapper();
            HttpTransportBindingElement httpBE = new HttpTransportBindingElement();
            httpBE.ManualAddressing = true;
            CustomBinding result = new CustomBinding(webBE, httpBE);
            return result;
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(TestService), new Uri(baseAddress));
            WebHttpBinding serverBinding = new WebHttpBinding();
            ServiceEndpoint endpoint = host.AddServiceEndpoint(typeof(ITestService), CreateBinding(), "");
            endpoint.Behaviors.Add(new WebHttpBehavior());
            host.Open();
            Console.WriteLine("Host opened");

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(baseAddress + "/ProcessXml");
            req.Method = "POST";
            req.ContentType = "application/xml; charset=utf-8";
            string reqBody = @"<?xml version=""1.0"" encoding=""utf-8""?>
<firstElement/>
<secondElement/>
<you can even have invalid XML here/>";
            byte[] reqBodyBytes = Encoding.UTF8.GetBytes(reqBody);
            Stream reqStream = req.GetRequestStream();
            reqStream.Write(reqBodyBytes, 0, reqBodyBytes.Length);
            reqStream.Close();

            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Console.WriteLine("HTTP/{0} {1} {2}", resp.ProtocolVersion, (int)resp.StatusCode, resp.StatusDescription);
            Console.WriteLine(new StreamReader(resp.GetResponseStream()).ReadToEnd());
            resp.Close();

            Console.WriteLine("Press ENTER to close service");
            Console.ReadLine();
            host.Close();
        }
    }

    //no-config web-host
    public class Post2348599d
    {
        [ServiceContract]
        public interface ITestService
        {
            [OperationContract, WebInvoke]
            string ProcessXml(Stream input);
        }

        public class TestService : ITestService
        {
            public string ProcessXml(Stream input)
            {
                return new StreamReader(input).ReadToEnd();
            }
        }

        public class MyServiceHostFactory : ServiceHostFactory
        {
            protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
            {
                return new MyServiceHost(typeof(TestService), baseAddresses);
            }
        }
        public class MyServiceHost : ServiceHost
        {
            public MyServiceHost(Type serviceType, params Uri[] baseAddresses)
                : base(serviceType, baseAddresses)
            {
            }
            protected override void ApplyConfiguration()
            {
                base.ApplyConfiguration();
                foreach (ServiceEndpoint endpoint in this.Description.Endpoints)
                {
                    if (endpoint.Name == "TheEndpointThatNeedsToBeChanged")
                    {
                        CustomBinding newBinding = new CustomBinding(endpoint.Binding);
                        WebMessageEncodingBindingElement webMEBE = newBinding.Elements.Find<WebMessageEncodingBindingElement>();
                        webMEBE.ContentTypeMapper = new MyWebContentTypeMapper();
                    }
                }
            }
        }
        public class MyWebContentTypeMapper : WebContentTypeMapper
        {
            public override WebContentFormat GetMessageFormatForContentType(string contentType)
            {
                return WebContentFormat.Raw;
            }
        }
    }

    //How to route to a default service method
    public class Post2265886
    {
        [ServiceContract]
        public interface ITestService
        {
            [OperationContract]
            [WebGet(UriTemplate = "")]
            Stream DefaultPage();
        }

        public class TestService : ITestService
        {
            public Stream DefaultPage()
            {
                string html = "<html><head><title>This is the default page</title></head>" +
                    "<body><h1>This is my default page</h1></body></html>";
                WebOperationContext.Current.OutgoingResponse.ContentType = "text/html";
                return new MemoryStream(Encoding.UTF8.GetBytes(html));
            }
        }

        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName;// +":8000";
            ServiceHost host = new ServiceHost(typeof(TestService), new Uri(baseAddress));
            ServiceEndpoint endpoint = host.AddServiceEndpoint(typeof(ITestService), new WebHttpBinding(), "");
            endpoint.Behaviors.Add(new WebHttpBehavior());
            ServiceDebugBehavior debugBehavior = host.Description.Behaviors.Find<ServiceDebugBehavior>();
            debugBehavior.HttpHelpPageEnabled = false;
            debugBehavior.HttpsHelpPageEnabled = false;
            host.Open();
            Console.WriteLine("Host opened at " + baseAddress);

            Console.WriteLine("Press ENTER to close service");
            Console.ReadLine();
            host.Close();
        }
    }

    //POX in 3.0 - WebContentFormat.Raw
    public class Post2436679
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract(Action = "*", ReplyAction = "*")]
            Message Process(Message input);
        }
        public class Service : ITest
        {
            public Message Process(Message message)
            {
                XmlReader bodyReader = message.GetReaderAtBodyContents();
                if (bodyReader.NodeType == XmlNodeType.Element)
                {
                    bodyReader.Read();
                }
                byte[] input = new byte[10000];
                int bytes = bodyReader.ReadContentAsBase64(input, 0, input.Length);
                string inputString = Encoding.UTF8.GetString(input, 0, bytes);
                string resultXml = @"<html><head><title>Result</title></head>
<body>
<h1>Result</h1>
<p>{{result}}</p>
</body></html>";
                inputString = inputString.Replace("&", "&amp;");
                resultXml = resultXml.Replace("{{result}}", inputString);
                XmlReader resultReader = XmlReader.Create(new StringReader(resultXml));
                return Message.CreateMessage(message.Version, "Return", resultReader);
            }
        }
        public class MyXmlByteArrayReader : XmlReader
        {
            private ArraySegment<byte> input;
            private int currentCount;
            private Mode mode;
            private MyNameTable nameTable;
            enum Mode { Start, ReadingElement, ReadingData, ReadingEndElement, End }
            public class MyNameTable : XmlNameTable
            {
                Dictionary<string, string> table;
                public MyNameTable()
                    : base()
                {
                    this.table = new Dictionary<string, string>();
                    this.Add(MyXmlByteArrayReader.ElementName);
                }
                public override string Add(char[] array, int offset, int length)
                {
                    string val = new string(array, offset, length);
                    return Add(val);
                }
                public override string Add(string val)
                {
                    if (table.ContainsKey(val))
                    {
                        return table[val];
                    }
                    else
                    {
                        this.table.Add(val, val);
                        return val;
                    }
                }
                public override string Get(char[] array, int offset, int length)
                {
                    string val = new string(array, offset, length);
                    return Get(val);
                }
                public override string Get(string val)
                {
                    if (table.ContainsKey(val))
                    {
                        return table[val];
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            const string ElementName = "Wrapper";
            public MyXmlByteArrayReader(ArraySegment<byte> input)
            {
                this.input = input;
                this.currentCount = 0;
                this.mode = Mode.Start;
                this.nameTable = new MyNameTable();
            }
            public override int AttributeCount
            {
                get { return 0; }
            }

            public override string BaseURI
            {
                get { return ""; }
            }

            public override void Close()
            {
            }

            public override int Depth
            {
                get { throw new NotImplementedException(); }
            }

            public override bool EOF
            {
                get { return (this.mode == Mode.End); }
            }

            public override string GetAttribute(int i)
            {
                return null;
            }

            public override string GetAttribute(string name, string namespaceURI)
            {
                return null;
            }

            public override string GetAttribute(string name)
            {
                return null;
            }

            public override bool HasValue
            {
                get { return false; }
            }

            public override bool IsEmptyElement
            {
                get { return false; }
            }

            public override string LocalName
            {
                get
                {
                    return (mode == Mode.ReadingElement || mode == Mode.ReadingEndElement) ? this.nameTable.Get(ElementName) : (string)null;
                }
            }

            public override string LookupNamespace(string prefix)
            {
                return null;
            }

            public override bool MoveToAttribute(string name, string ns)
            {
                return false;
            }

            public override bool MoveToAttribute(string name)
            {
                return false;
            }

            public override bool MoveToElement()
            {
                return false;
            }

            public override bool MoveToFirstAttribute()
            {
                return false;
            }

            public override bool MoveToNextAttribute()
            {
                return false;
            }

            public override XmlNameTable NameTable
            {
                get
                {
                    return this.nameTable;
                }
            }

            public override string NamespaceURI
            {
                get { return ""; }
            }

            public override XmlNodeType NodeType
            {
                get
                {
                    switch (this.mode)
                    {
                        case Mode.Start: return XmlNodeType.None;
                        case Mode.ReadingElement: return XmlNodeType.Element;
                        case Mode.ReadingData: return XmlNodeType.Text;
                        case Mode.ReadingEndElement: return XmlNodeType.EndElement;
                        default: return XmlNodeType.None;
                    }
                }
            }

            public override string Prefix
            {
                get { return ""; }
            }

            public override bool Read()
            {
                switch (this.mode)
                {
                    case Mode.Start:
                        this.mode = Mode.ReadingElement;
                        break;
                    case Mode.ReadingElement:
                        this.mode = Mode.ReadingData;
                        break;
                    case Mode.ReadingData:
                        this.mode = Mode.ReadingEndElement;
                        break;
                    case Mode.ReadingEndElement:
                        this.mode = Mode.End;
                        break;
                    case Mode.End:
                        return false;
                }
                return true;
            }

            public override int ReadContentAsBase64(byte[] buffer, int index, int count)
            {
                if (this.mode == Mode.ReadingData)
                {
                    int result = Math.Min(count, this.input.Count - this.currentCount);
                    if (result > 0)
                    {
                        Array.Copy(this.input.Array, this.input.Offset + this.currentCount,
                            buffer, index, result);
                        this.currentCount += result;
                    }
                    return result;
                }
                return 0;
            }

            public override bool ReadAttributeValue()
            {
                return false;
            }

            public override ReadState ReadState
            {
                get
                {
                    switch (this.mode)
                    {
                        case Mode.ReadingElement:
                        case Mode.ReadingData:
                        case Mode.ReadingEndElement:
                            return ReadState.Interactive;
                        case Mode.Start:
                            return ReadState.Initial;
                        case Mode.End:
                            return ReadState.EndOfFile;
                        default:
                            throw new InvalidOperationException();
                    }
                }
            }

            public override void ResolveEntity()
            {
            }

            public override string Value
            {
                get
                {
                    if (mode == Mode.ReadingData)
                    {
                        return "...Value...";
                    }
                    else
                    {
                        return "";
                    }
                }
            }
        }
        public class MyMessageEncodingBindingElement : MessageEncodingBindingElement
        {
            public override MessageEncoderFactory CreateMessageEncoderFactory()
            {
                return new MyMessageEncoderFactory();
            }

            public override MessageVersion MessageVersion
            {
                get
                {
                    return MessageVersion.None;
                }
                set
                {
                    if (value != MessageVersion.None) throw new ArgumentException("value");
                }
            }

            public override BindingElement Clone()
            {
                return new MyMessageEncodingBindingElement();
            }

            public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
            {
                context.BindingParameters.Add(this);
                return context.BuildInnerChannelFactory<TChannel>();
            }
            public override bool CanBuildChannelFactory<TChannel>(BindingContext context)
            {
                return context.CanBuildInnerChannelFactory<TChannel>();
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
            public override T GetProperty<T>(BindingContext context)
            {
                if (typeof(T) == typeof(MessageVersion))
                {
                    return (T)(object)MessageVersion.None;
                }
                return base.GetProperty<T>(context);
            }
        }
        public class MyMessageEncoderFactory : MessageEncoderFactory
        {
            private MyMessageEncoder myEncoder = new MyMessageEncoder();
            public override MessageEncoder Encoder
            {
                get { return myEncoder; }
            }

            public override MessageVersion MessageVersion
            {
                get { return MessageVersion.None; }
            }
        }
        public class MyMessageEncoder : MessageEncoder
        {
            public override string ContentType
            {
                get { return "text/html"; }
            }

            public override string MediaType
            {
                get { return "text/html"; }
            }

            public override bool IsContentTypeSupported(string contentType)
            {
                return contentType == this.ContentType || contentType == "application/x-www-form-urlencoded";
            }

            public override MessageVersion MessageVersion
            {
                get { return MessageVersion.None; }
            }

            public override Message ReadMessage(ArraySegment<byte> buffer, BufferManager bufferManager, string contentType)
            {
                MyXmlByteArrayReader reader = new MyXmlByteArrayReader(buffer);
                return Message.CreateMessage(MessageVersion.None, "Message", reader);
            }

            public override Message ReadMessage(Stream stream, int maxSizeOfHeaders, string contentType)
            {
                throw new NotSupportedException();
            }

            public override ArraySegment<byte> WriteMessage(Message message, int maxMessageSize, BufferManager bufferManager, int messageOffset)
            {
                MemoryStream stream = new MemoryStream();
                XmlWriter writer = XmlWriter.Create(stream);
                message.WriteMessage(writer);
                writer.Close();

                byte[] messageBytes = stream.GetBuffer();
                int messageLength = (int)stream.Position;

                int totalLength = messageLength + messageOffset;
                byte[] totalBytes = bufferManager.TakeBuffer(totalLength);
                Array.Copy(messageBytes, 0, totalBytes, messageOffset, messageLength);
                stream.Close();

                ArraySegment<byte> byteArray = new ArraySegment<byte>(totalBytes, messageOffset, messageLength);
                return byteArray;
            }

            public override void WriteMessage(Message message, Stream stream)
            {
                throw new NotSupportedException();
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            CustomBinding binding = new CustomBinding(
                new MyMessageEncodingBindingElement(),
                new HttpTransportBindingElement());
            ServiceEndpoint endpoint = host.AddServiceEndpoint(typeof(ITest), binding, "");
            host.Open();
            Console.WriteLine("Host opened");

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(baseAddress);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            string reqBody = @"First=Where+am+I&Second=I+am+here";
            byte[] reqBodyBytes = Encoding.UTF8.GetBytes(reqBody);
            Stream reqStream = req.GetRequestStream();
            reqStream.Write(reqBodyBytes, 0, reqBodyBytes.Length);
            reqStream.Close();

            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Console.WriteLine("HTTP/{0} {1} {2}", resp.ProtocolVersion, (int)resp.StatusCode, resp.StatusDescription);
            Console.WriteLine(new StreamReader(resp.GetResponseStream()).ReadToEnd());
            resp.Close();

            Console.WriteLine("Press ENTER to close service");
            Console.ReadLine();
            host.Close();
        }
    }

    //POX in 3.0 - WebContentFormat.Raw
    public class Post2436679b
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            Stream ProcessArbitraryInput(Stream input);
        }
        public class Service : ITest
        {
            public Stream ProcessArbitraryInput(Stream input)
            {
                string strInput = new StreamReader(input).ReadToEnd();
                string htmlResult = @"<html><head><title>Result</title></head>
<body>
<h1>First: {{first}}</h1>
<h1>Second: {{second}}</h1>
</body></html>";
                Regex inputRegex = new Regex(@"First=([^\&]+)\&Second=(.+)");
                Match match = inputRegex.Match(strInput);
                string first, second;
                if (match.Success)
                {
                    first = match.Groups[1].Value.Replace('+', ' ');
                    second = match.Groups[2].Value.Replace('+', ' ');
                }
                else
                {
                    first = second = "Error";
                }
                htmlResult = htmlResult.Replace("{{first}}", first).Replace("{{second}}", second);
                WebOperationContext.Current.OutgoingResponse.ContentType = "text/html";
                return new MemoryStream(Encoding.UTF8.GetBytes(htmlResult));
            }
        }
    }

    //DataContracts Break at Large Sizes
    public class Post2468830
    {
        [DataContract]
        public class Datam
        {
            [DataMember]
            public String Field1;
            [DataMember]
            public String Field2;
            [DataMember]
            public String Field3;
            [DataMember]
            public String Field4;
            [DataMember]
            public String Field5;
            [DataMember]
            public String Field6;

            public Datam(Random r)
            {
                this.Field1 = RandomString(64, r);
                this.Field2 = RandomString(64, r);
                this.Field3 = RandomString(64, r);
                this.Field4 = RandomString(64, r);
                this.Field5 = RandomString(64, r);
                this.Field6 = RandomString(64, r);
            }

            static string RandomString(int size, Random r)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < size; i++)
                {
                    sb.Append((char)r.Next('a', 'z' + 1));
                }
                return sb.ToString();
            }
        }
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            Datam[] GetDataContracts(int size);
        }
        public class Service : ITest
        {
            public Datam[] GetDataContracts(int size)
            {
                Datam[] result = new Datam[size];
                Random rndGen = new Random(1);
                for (int i = 0; i < size; i++)
                {
                    result[i] = new Datam(rndGen);
                }
                return result;
            }
        }
        static Binding GetBinding()
        {
            BasicHttpBinding result = new BasicHttpBinding();
            result.MaxReceivedMessageSize = int.MaxValue;
            return result;
        }
        public static void Test()
        {
            string baseAddress = "http://localhost:8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), new BasicHttpBinding(), "");
            DataContractSerializerOperationBehavior dcsob = host.Description.Endpoints[0].Contract.Operations[0].Behaviors.Find<DataContractSerializerOperationBehavior>();
            Console.WriteLine("Original MIIOG: " + dcsob.MaxItemsInObjectGraph);
            dcsob.MaxItemsInObjectGraph = 1000000;
            host.Open();

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(GetBinding(), new EndpointAddress(baseAddress));
            dcsob = factory.Endpoint.Contract.Operations[0].Behaviors.Find<DataContractSerializerOperationBehavior>();
            dcsob.MaxItemsInObjectGraph = int.MaxValue;
            ITest proxy = factory.CreateChannel();

            try
            {
                Datam[] result = proxy.GetDataContracts(9363);
                Console.WriteLine("Result.length: " + result.Length);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            ((IClientChannel)proxy).Close();
            factory.Close();
            host.Close();
        }
    }

    //Custom XmlObjectSerializer
    public class Post2448577
    {
        public class MySerializer : XmlObjectSerializer
        {
            DataContractSerializer dcs;
            public MySerializer(Type type)
            {
                this.dcs = new DataContractSerializer(type);
            }
            public override bool IsStartObject(XmlDictionaryReader reader)
            {
                return dcs.IsStartObject(reader);
            }

            public override object ReadObject(XmlDictionaryReader reader, bool verifyObjectName)
            {
                return dcs.ReadObject(reader, verifyObjectName);
            }

            public override void WriteEndObject(XmlDictionaryWriter writer)
            {
                dcs.WriteEndObject(writer);
            }

            public override void WriteObjectContent(XmlDictionaryWriter writer, object graph)
            {
                dcs.WriteObjectContent(writer, graph);
            }

            public override void WriteStartObject(XmlDictionaryWriter writer, object graph)
            {
                dcs.WriteStartObject(writer, graph);
            }
        }
        [DataContract]
        public class MyDC
        {
            [DataMember]
            public int i = 1234;
            [DataMember]
            public string str = "Hello";
        }
        public static void Test()
        {
            Test2();
            MySerializer s = new MySerializer(typeof(MyDC));
            MemoryStream ms = new MemoryStream();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;
            settings.Encoding = new UTF8Encoding();
            XmlWriter writer = XmlWriter.Create(ms, settings);
            s.WriteObject(writer, new MyDC());
            writer.Flush();
            Console.WriteLine(Encoding.UTF8.GetString(ms.ToArray()));
        }
        public static void Test2()
        {
            DataContractSerializer dcs = new DataContractSerializer(typeof(string));
            MemoryStream ms = new MemoryStream();
            XmlWriterSettings writerSettings = new XmlWriterSettings();
            writerSettings.OmitXmlDeclaration = true;
            XmlWriter writer = XmlWriter.Create(ms, writerSettings);
            dcs.WriteObject(writer, "Hello world");
            writer.Flush();
            string serialized = Encoding.UTF8.GetString(ms.ToArray());
            Console.WriteLine(serialized);
        }
    }

    //Create a proxy if i dynamically get a Type of contract
    public class Post2572103
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            string Hello(string name);
        }
        public class Service : ITest
        {
            public string Hello(string name)
            {
                return "Hello " + name.ToString();
            }
        }
        public static void Test()
        {
            string baseAddress = "http://localhost:8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), new BasicHttpBinding(), "");
            host.Open();

            Assembly theAssembly = Assembly.GetExecutingAssembly();
            string typeName = "MyNamespace.ITest";

            Type interfaceType = theAssembly.GetType(typeName);
            Type channelFactoryGenericType = typeof(ChannelFactory<>);
            Type channelFactoryType = channelFactoryGenericType.MakeGenericType(interfaceType);

            Binding binding = new BasicHttpBinding();
            EndpointAddress endpointAddress = new EndpointAddress(baseAddress);
            object factory = Activator.CreateInstance(channelFactoryType, binding, endpointAddress);
            Console.WriteLine("Created the factory");
            Type factoryType = factory.GetType();

            object proxy = factoryType.GetMethod("CreateChannel", new Type[0]).Invoke(factory, new object[0]);
            Console.WriteLine("Created the proxy");
            Type proxyType = proxy.GetType();

            string result = (string)proxyType.GetMethod("Hello").Invoke(proxy, new object[] { "John Doe" });
            Console.WriteLine("Result: " + result);

            ((IClientChannel)proxy).Close();
            ((ChannelFactory)factory).Close();
            host.Close();
        }
    }
    public class Post2578655
    {
        [DataContract]
        public class Message
        {
            [DataMember]
            public String to;

            [DataMember]
            public String from;

            [DataMember]
            public MyISer data;
        }
        [Serializable]
        public class MyISer : ISerializable
        {
            private SerializationInfo info;
            public MyISer() { }
            public MyISer(SerializationInfo info, StreamingContext context)
            {
                this.info = info;
            }
            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                //not used
            }
            public T GetValue<T>(string name)
            {
                return (T)info.GetValue(name, typeof(T));
            }
            public object GetValue(string name, Type type)
            {
                return info.GetValue(name, type);
            }
        }
        public static void Test()
        {
            string arbitraryJson = @"{""to"":""you"", ""from"":""me"",
""data"":{""foo"":1.23,""bar"":true,""hello"":""world""}}
";
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(arbitraryJson));
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(MyISer));
            MyISer myISer = (MyISer)ser.ReadObject(ms);
            Console.WriteLine(myISer.GetValue<string>("from"));
            Console.WriteLine(myISer.GetValue<string>("to"));
            object obj = (object)myISer.GetValue("data", typeof(object));
            Console.WriteLine(obj);
        }
    }

    //WebHttpBinding issues - WebHttpBinding client
    public class Post2592218a
    {
        [DataContract]
        public class MyComplexObject
        {
            [DataMember]
            public string str;
            [DataMember]
            public int i;
            public MyComplexObject(int i, string str)
            {
                this.i = i;
                this.str = str;
            }
            public override string ToString()
            {
                return String.Format("MyComplexObject[i={0},str={1}]", i, str);
            }
        }
        [ServiceContract]
        public interface ITest
        {
            [OperationContract, WebGet]
            MyComplexObject GetComplex(string input);
            [OperationContract, WebInvoke]
            MyComplexObject Process(MyComplexObject input);
        }

        public class Service : ITest
        {
            public MyComplexObject GetComplex(string input)
            {
                return new MyComplexObject(input.Length, input);
            }
            public MyComplexObject Process(MyComplexObject input)
            {
                input.str = "Result: " + input.str;
                input.i = input.str.Length;
                return input;
            }
        }

        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            ServiceEndpoint endpoint = host.AddServiceEndpoint(typeof(ITest), new WebHttpBinding(), "");
            endpoint.Behaviors.Add(new WebHttpBehavior());
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(new WebHttpBinding(), new EndpointAddress(baseAddress));
            factory.Endpoint.Behaviors.Add(new WebHttpBehavior());
            ITest proxy = factory.CreateChannel();

            Console.WriteLine("Result of Get operation: {0}", proxy.GetComplex("This was via GET"));
            Console.WriteLine("Result of EchoPost: {0}", proxy.Process(new MyComplexObject(123, "This was via POST")));

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.WriteLine("Press ENTER to close service");
            Console.ReadLine();
            host.Close();
        }
    }

    //WebHttpBinding issues - enums on GET
    public class Post2592218b
    {
        [Flags]
        public enum MyEnum
        {
            One = 1,
            Two = 2,
            Four = 4,
        }
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            [WebGet(UriTemplate = "/input?value={value}")]
            string Process(MyEnum value);
        }

        public class Service : ITest
        {
            public string Process(MyEnum value)
            {
                return value.ToString();
            }
        }

        static void SendRequest(string uri)
        {
            Console.WriteLine("Request to {0}", uri);
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(uri);
            req.Method = "GET";

            HttpWebResponse resp = null;
            try
            {
                resp = (HttpWebResponse)req.GetResponse();
            }
            catch (WebException e)
            {
                resp = (HttpWebResponse)e.Response;
            }
            Console.WriteLine("HTTP/{0} {1} {2}", resp.ProtocolVersion, (int)resp.StatusCode, resp.StatusDescription);
            Console.WriteLine(new StreamReader(resp.GetResponseStream()).ReadToEnd());
            Console.WriteLine();
            resp.Close();
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            ServiceEndpoint endpoint = host.AddServiceEndpoint(typeof(ITest), new WebHttpBinding(), "");
            endpoint.Behaviors.Add(new WebHttpBehavior());
            host.Open();
            Console.WriteLine("Host opened");

            SendRequest(baseAddress + "/input?value=3");
            SendRequest(baseAddress + "/input?value=First");
            SendRequest(baseAddress + "/input?value=Two");

            Console.WriteLine("Press ENTER to close service");
            Console.ReadLine();
            host.Close();
        }
    }
    //WebHttpBinding issues - enums on GET
    public class Post2592218b2
    {
        [Flags]
        [TypeConverter(typeof(MyTypeConverter))]
        public enum MyEnum
        {
            One = 1,
            Two = 2,
            Four = 4,
        }
        public class MyTypeConverter : TypeConverter
        {
            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                if (sourceType == typeof(string))
                {
                    return true;
                }
                return base.CanConvertFrom(context, sourceType);
            }
            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                if (destinationType == typeof(string))
                {
                    return true;
                }
                return base.CanConvertTo(context, destinationType);
            }
            public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
            {
                string strValue = value as string;
                if (strValue != null)
                {
                    return Enum.Parse(typeof(MyEnum), strValue);
                }
                return base.ConvertFrom(context, culture, value);
            }
            public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
            {
                if (destinationType == typeof(string) && value is MyEnum)
                {
                    return ((MyEnum)value).ToString();
                }
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            [WebGet(UriTemplate = "/input/{value}")]
            string Process(MyEnum value);
        }

        public class Service : ITest
        {
            public string Process(MyEnum value)
            {
                return value.ToString();
            }
        }

        static void SendRequest(string uri)
        {
            Console.WriteLine("Request to {0}", uri);
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(uri);
            req.Method = "GET";

            HttpWebResponse resp = null;
            try
            {
                resp = (HttpWebResponse)req.GetResponse();
            }
            catch (WebException e)
            {
                resp = (HttpWebResponse)e.Response;
            }
            Console.WriteLine("HTTP/{0} {1} {2}", resp.ProtocolVersion, (int)resp.StatusCode, resp.StatusDescription);
            Console.WriteLine(new StreamReader(resp.GetResponseStream()).ReadToEnd());
            Console.WriteLine();
            resp.Close();
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            ServiceEndpoint endpoint = host.AddServiceEndpoint(typeof(ITest), new WebHttpBinding(), "");
            endpoint.Behaviors.Add(new WebHttpBehavior());
            host.Open();
            Console.WriteLine("Host opened");

            //SendRequest(baseAddress + "/input?value=3");
            //SendRequest(baseAddress + "/input?value=First");
            //SendRequest(baseAddress + "/input?value=Two");
            SendRequest(baseAddress + "/input/3");
            SendRequest(baseAddress + "/input/First");
            SendRequest(baseAddress + "/input/Two");

            Console.WriteLine("Press ENTER to close service");
            Console.ReadLine();
            host.Close();
        }
    }

    //Custom request
    public class Post2656956
    {
        [ServiceContract]
        public interface IServiceUpper
        {
            [OperationContract]
            string GetData(int value1, int value2);
        }
        [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
        public class ServiceUpper : IServiceUpper
        {
            public string GetData(int value1, int value2)
            {
                return string.Format("You entered: {0} and {1} and answers are: {2} and {3}", value1, value2, value1 * 10, value2 * 20);
            }
        }
        static WSHttpBinding GetBinding()
        {
            WSHttpBinding result = new WSHttpBinding();
            result.Security.Mode = SecurityMode.None;
            return result;
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8080/ServiceUpper";
            ServiceHost host = new ServiceHost(typeof(ServiceUpper), new Uri(baseAddress));
            //host.AddServiceEndpoint(typeof(IServiceUpper), GetBinding(), "");
            host.Open();

            ChannelFactory<IServiceUpper> factory = new ChannelFactory<IServiceUpper>(GetBinding(), new EndpointAddress(baseAddress));
            IServiceUpper proxy = factory.CreateChannel();
            string result = proxy.GetData(0, 0);
            Console.WriteLine(result);

            ((IClientChannel)proxy).Close();
            factory.Close();

            using (StreamReader readerXml = new StreamReader("request.xml"))
            {
                string requestXml = readerXml.ReadToEnd();
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(baseAddress);
                request.Method = "POST";
                request.Credentials = CredentialCache.DefaultCredentials;
                request.ContentType = "application/soap+xml; charset=utf-8";
                request.ContentLength = requestXml.Length;
                System.IO.StreamWriter streamWriter =
                new System.IO.StreamWriter(request.GetRequestStream());
                streamWriter.Write(requestXml);
                streamWriter.Close();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Console.WriteLine(response.StatusDescription);
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                Console.WriteLine(responseFromServer);
                reader.Close();
                dataStream.Close();
                response.Close();
            }

            host.Close();
        }
    }

    public class DataTableOnJson
    {
        public static void Test()
        {
            DataTable t = new DataTable("Customers");
            t.Columns.Add(new DataColumn("FirstName", typeof(string)));
            t.Columns.Add(new DataColumn("LastName", typeof(string)));
            t.Columns.Add(new DataColumn("DOB", typeof(DateTime)));
            t.Columns.Add(new DataColumn("Salary", typeof(double)));
            t.Rows.Add("John", "Doe", new DateTime(1956, 10, 8), 123456.78);
            t.Rows.Add("Jane", "Roe", new DateTime(1960, 1, 31), 99999.99);

            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(DataTable));
            MemoryStream ms = new MemoryStream();
            ser.WriteObject(ms, t);
            Console.WriteLine(Encoding.UTF8.GetString(ms.ToArray()));
        }
    }

    //custom JSON serialization
    public class Post2681482
    {
        [Serializable]
        public class MyFancyObject : ISerializable
        {
            int id = 2;
            DateTime createDate = new DateTime(2008, 1, 12, 3, 43, 12, DateTimeKind.Utc);
            public MyFancyObject() { }
            public MyFancyObject(SerializationInfo info, StreamingContext context)
            {
                this.id = info.GetInt32("id");
                this.createDate = info.GetDateTime("createdate");
            }
            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue("id", this.id);
                info.AddValue("createdate", this.createDate);
            }
        }
        public static void Test()
        {
            MemoryStream ms = new MemoryStream();
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(MyFancyObject));
            ser.WriteObject(ms, new MyFancyObject());
            Console.WriteLine(Encoding.UTF8.GetString(ms.ToArray()));
        }
    }

    //Finding out on which vdir the service is running
    public class Post2692941
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            string GetVDir();
        }
        public class Service : ITest
        {
            public string GetVDir()
            {
                Uri endpointUri = OperationContext.Current.EndpointDispatcher.EndpointAddress.Uri;
                string path = endpointUri.AbsolutePath;
                if (path.StartsWith("/")) path = path.Substring(1);
                int indexOfSlash = path.IndexOf('/');
                if (indexOfSlash >= 0)
                {
                    return path.Substring(0, indexOfSlash);
                }
                return "Not hosted in IIS?";
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
            host.AddServiceEndpoint(typeof(ITest), GetBinding(), "ITest");
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(GetBinding(), new EndpointAddress(baseAddress + "/ITest"));
            ITest proxy = factory.CreateChannel();
            Console.WriteLine(proxy.GetVDir());

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }
    //Creating bindings via reflection
    public class Post2718449
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
        static Binding GetBinding()
        {
            BasicHttpBinding result = new BasicHttpBinding();
            //Change binding settings here
            return result;
        }
        public class WcfProxyHelper<T>
        {
            /// <summary>
            /// Generic helper method that returns a typed proxy based upon type and and wcf base address string
            /// </summary>        
            public static T GetProxy(string agentBaseAddr)
            {
                System.ServiceModel.Channels.Binding binding = null;
                ChannelFactory<T> proxyFactory;
                T proxy;
                Type listenerBindingClass = null;

                if (agentBaseAddr.Contains("net.tcp://"))
                {
                    listenerBindingClass = typeof(System.ServiceModel.NetTcpBinding);
                }
                else if (agentBaseAddr.Contains("http://"))
                {
                    listenerBindingClass = typeof(System.ServiceModel.BasicHttpBinding);
                }

                binding = (System.ServiceModel.Channels.Binding)Activator.CreateInstance(listenerBindingClass);

                proxyFactory = new ChannelFactory<T>(binding, new EndpointAddress(agentBaseAddr));
                proxy = proxyFactory.CreateChannel();

                ((IClientChannel)proxy).Closed += delegate { proxyFactory.Close(); };

                return proxy;
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.Open();
            Console.WriteLine("Host opened");

            ITest proxy = WcfProxyHelper<ITest>.GetProxy(baseAddress);
            Console.WriteLine(proxy.Echo(new string('r', 2000)));

            ((IClientChannel)proxy).Close();
            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    //Custom binding for WSHttpBinding
    public class Post2744356
    {
        public static void Test()
        {
            WSHttpBinding binding = new WSHttpBinding("SonetoBinding");
            CustomBinding cb = new CustomBinding(binding);
            for (int i = 0; i < cb.Elements.Count; i++)
            {
                //Console.WriteLine(cb.Elements[i]);
            }
            Console.WriteLine(System.AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
            Console.WriteLine();
            CustomBinding custom = new CustomBinding("MyCustom");
            for (int i = 0; i < custom.Elements.Count; i++)
            {
                Console.WriteLine(custom.Elements[i]);
            }
        }
    }

    //Custom binding for NetTcp security
    public class Post2762744
    {
        public static void Test()
        {
            NetTcpBinding netTcp = new NetTcpBinding("netTcp");
            BindingElementCollection elements = netTcp.CreateBindingElements();
            WindowsStreamSecurityBindingElement secBE = elements.Find<WindowsStreamSecurityBindingElement>();
            Console.WriteLine(secBE);

            CustomBinding custom = new CustomBinding("MyCustom");
            WindowsStreamSecurityBindingElement secBE2 = custom.Elements.Find<WindowsStreamSecurityBindingElement>();
            Console.WriteLine(secBE2);

            Console.WriteLine(secBE == secBE2);
        }
    }

    public class CharZero
    {
        [DataContract]
        public class MyDC
        {
            [DataMember]
            public char c = '\0';
        }
        public static void Test()
        {
            MemoryStream ms = new MemoryStream();
            DataContractJsonSerializer dcjs = new DataContractJsonSerializer(typeof(MyDC));
            dcjs.WriteObject(ms, new MyDC());
            Console.WriteLine(Encoding.UTF8.GetString(ms.ToArray()));
        }
    }

    public class Post2790925
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            [WebInvoke(Method = "POST", UriTemplate = "/orders/shipped-orders", RequestFormat = WebMessageFormat.Json)]
            void ShipOrder(Message message);
        }
        public class Service : ITest
        {
            public void ShipOrder(Message message)
            {
                Console.WriteLine(message.ToString());
                Root root = message.GetBody<Root>(new DataContractJsonSerializer(typeof(Root)));
                if (root == null)
                {
                    Console.WriteLine("Root is null");
                }
                else
                {
                    Console.WriteLine(root.order);
                }
            }
        }
        [DataContract]
        public class Root
        {
            [DataMember]
            public Order order;
        }
        [DataContract]
        public class Order
        {
            [DataMember]
            public int OrderId;
            [DataMember]
            public string OrderDesc;

            public override string ToString()
            {
                return String.Format("Order[Id={0},Desc={1}]", OrderId, OrderDesc);
            }
        }

        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            WebServiceHost host = new WebServiceHost(typeof(Service), new Uri(baseAddress));
            host.Open();

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(baseAddress + "/orders/shipped-orders");
            req.Method = "POST";
            req.ContentType = "application/json";
            Stream reqStream = req.GetRequestStream();
            byte[] reqBytes = Encoding.UTF8.GetBytes("{\"order\":{\"OrderId\":12,\"OrderDesc\":\"Black Shoes\"}}");
            reqStream.Write(reqBytes, 0, reqBytes.Length);
            reqStream.Close();

            HttpWebResponse resp = null;
            try
            {
                resp = (HttpWebResponse)req.GetResponse();
            }
            catch (WebException e)
            {
                resp = (HttpWebResponse)e.Response;
            }
            Console.WriteLine("HTTP/{0} {1} {2}", resp.ProtocolVersion, (int)resp.StatusCode, resp.StatusDescription);
            if (resp.ContentLength > 0)
            {
                Console.WriteLine(new StreamReader(resp.GetResponseStream()).ReadToEnd());
            }
            resp.Close();

            Console.Write("Press ENTER to close service");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post2790925b
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            [WebInvoke(Method = "POST", UriTemplate = "/orders/shipped-orders", RequestFormat = WebMessageFormat.Json)]
            void ShipOrder(Order order);
        }
        public class Service : ITest
        {
            public void ShipOrder(Order order)
            {
                Console.WriteLine("Received order = {0}", order.ToString());
            }
        }
        [DataContract]
        public class Order
        {
            [DataMember]
            public int OrderId;
            [DataMember]
            public string OrderDesc;

            public override string ToString()
            {
                return String.Format("Order[Id={0},Desc={1}]", OrderId, OrderDesc);
            }
        }

        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            WebServiceHost host = new WebServiceHost(typeof(Service), new Uri(baseAddress));
            host.Open();

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(baseAddress + "/orders/shipped-orders");
            req.Method = "POST";
            req.ContentType = "application/json";
            Stream reqStream = req.GetRequestStream();
            byte[] reqBytes = Encoding.UTF8.GetBytes("{\"OrderId\":12,\"OrderDesc\":\"Black Shoes\"}");
            reqStream.Write(reqBytes, 0, reqBytes.Length);
            reqStream.Close();

            HttpWebResponse resp = null;
            try
            {
                resp = (HttpWebResponse)req.GetResponse();
            }
            catch (WebException e)
            {
                resp = (HttpWebResponse)e.Response;
            }
            Console.WriteLine("HTTP/{0} {1} {2}", resp.ProtocolVersion, (int)resp.StatusCode, resp.StatusDescription);
            if (resp.ContentLength > 0)
            {
                Console.WriteLine(new StreamReader(resp.GetResponseStream()).ReadToEnd());
            }
            resp.Close();

            Console.Write("Press ENTER to close service");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post2791297
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract(Action = "*", ReplyAction = "*")]
            Message Process(Message message);
        }
        [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
        public class Service : ITest
        {
            public Message Process(Message request)
            {
                XmlReader requestReader = request.GetReaderAtBodyContents();
                Message newMessage = Message.CreateMessage(request.Version, request.Headers.Action, requestReader);
                MessageBuffer buffer = newMessage.CreateBufferedCopy(int.MaxValue);
                XPathNavigator navigator = buffer.CreateNavigator();
                navigator.MoveToFollowing("Foo", "");
                int value = navigator.ValueAsInt;
                return Message.CreateMessage(request.Version, "", value);
            }
            string ElementName = "Something";
            public Message ProcessMessage(Message request)
            {
                XmlReader requestReader = request.GetReaderAtBodyContents();
                Message newMessage = Message.CreateMessage(request.Version, request.Headers.Action, requestReader);
                MessageBuffer buffer = newMessage.CreateBufferedCopy(int.MaxValue);
                //find the element value from the request XML
                XPathNavigator messageNavigator = buffer.CreateNavigator();
                messageNavigator.MoveToFollowing(ElementName, "");
                int myValue = messageNavigator.ValueAsInt;
                return null;
            }
            public Message ProcessNew(Message request)
            {
                MessageBuffer buffer = request.CreateBufferedCopy(int.MaxValue);
                XPathNavigator navigator = buffer.CreateNavigator();
                navigator.MoveToFollowing("Foo", "");
                int value = navigator.ValueAsInt;
                return Message.CreateMessage(request.Version, "", value);
            }
        }
        static Binding GetBinding()
        {
            TextMessageEncodingBindingElement textBE = new TextMessageEncodingBindingElement(MessageVersion.None, Encoding.UTF8);
            HttpTransportBindingElement httpBE = new HttpTransportBindingElement();
            return new CustomBinding(textBE, httpBE);
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), GetBinding(), "");
            host.Open();
            Console.WriteLine("Host opened");

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(baseAddress);
            req.Method = "POST";
            req.ContentType = "application/xml";
            Stream reqStream = req.GetRequestStream();
            byte[] reqBytes = Encoding.UTF8.GetBytes("<Foo attr=\"val\">123</Foo>");
            reqStream.Write(reqBytes, 0, reqBytes.Length);
            reqStream.Close();

            HttpWebResponse resp = null;
            try
            {
                resp = (HttpWebResponse)req.GetResponse();
            }
            catch (WebException e)
            {
                resp = (HttpWebResponse)e.Response;
            }
            Console.WriteLine("HTTP/{0} {1} {2}", resp.ProtocolVersion, (int)resp.StatusCode, resp.StatusDescription);
            if (resp.ContentLength > 0)
            {
                Console.WriteLine(new StreamReader(resp.GetResponseStream()).ReadToEnd());
            }
            resp.Close();

            Console.Write("Press ENTER to close service");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post2790925c
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            [WebInvoke(Method = "POST", UriTemplate = "/orders/shipped-orders", RequestFormat = WebMessageFormat.Json)]
            void ShipOrder(Stream stream);
        }
        public class Service : ITest
        {
            public void ShipOrder(Stream stream)
            {
                Console.WriteLine("Received order = {0}", new StreamReader(stream).ReadToEnd());
            }
        }

        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            WebServiceHost host = new WebServiceHost(typeof(Service), new Uri(baseAddress));
            host.Open();

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(baseAddress + "/orders/shipped-orders");
            req.Method = "POST";
            req.ContentType = "application/json";
            Stream reqStream = req.GetRequestStream();
            byte[] reqBytes = Encoding.UTF8.GetBytes("{\"OrderId\":12,\"OrderDesc\":\"Black Shoes\"}");
            reqStream.Write(reqBytes, 0, reqBytes.Length);
            reqStream.Close();

            HttpWebResponse resp = null;
            try
            {
                resp = (HttpWebResponse)req.GetResponse();
            }
            catch (WebException e)
            {
                resp = (HttpWebResponse)e.Response;
            }
            Console.WriteLine("HTTP/{0} {1} {2}", resp.ProtocolVersion, (int)resp.StatusCode, resp.StatusDescription);
            if (resp.ContentLength > 0)
            {
                Console.WriteLine(new StreamReader(resp.GetResponseStream()).ReadToEnd());
            }
            resp.Close();

            Console.Write("Press ENTER to close service");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post2790925d
    {
        [DataContract]
        public class OrderConfirmation
        {
            [DataMember]
            public int OrderId;
            [DataMember]
            public string ConfirmationCode;
        }
        [DataContract]
        public class Order
        {
            [DataMember]
            public int OrderId;
            [DataMember]
            public string OrderDesc;

            public override string ToString()
            {
                return String.Format("Order[Id={0},Desc={1}]", OrderId, OrderDesc);
            }
            public static Order FromJson(string jsonString)
            {
                DataContractJsonSerializer dcjs = new DataContractJsonSerializer(typeof(Order));
                return (Order)dcjs.ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(jsonString)));
            }
        }
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            [WebInvoke(Method = "POST", UriTemplate = "/orders/shipped-orders",
                RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
            OrderConfirmation ShipOrder(Stream stream);
            [OperationContract]
            [WebInvoke(Method = "POST", UriTemplate = "/orders/order-with-message",
                RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Xml)]
            OrderConfirmation ShipOrder2(Message message);
        }
        public class Service : ITest
        {
            public OrderConfirmation ShipOrder(Stream stream)
            {
                string str = new StreamReader(stream).ReadToEnd();
                Console.WriteLine("Received order = {0}", str);
                Order order = Order.FromJson(str);
                OrderConfirmation result = new OrderConfirmation();
                result.OrderId = order.OrderId;
                result.ConfirmationCode = "abcd";
                return result;
            }
            public OrderConfirmation ShipOrder2(Message message)
            {
                XmlDictionaryReader reader = message.GetReaderAtBodyContents();
                reader.Read();
                byte[] bytes = reader.ReadContentAsBase64();
                string str = Encoding.UTF8.GetString(bytes);
                Console.WriteLine("Request: {0}", str);
                Order order = Order.FromJson(str);
                OrderConfirmation result = new OrderConfirmation();
                result.OrderId = order.OrderId;
                result.ConfirmationCode = "hjkl";
                return result;
            }
        }
        static void GetAndPrintResponse(HttpWebRequest req)
        {
            HttpWebResponse resp = null;
            try
            {
                resp = (HttpWebResponse)req.GetResponse();
            }
            catch (WebException e)
            {
                resp = (HttpWebResponse)e.Response;
            }
            Console.WriteLine("HTTP/{0} {1} {2}", resp.ProtocolVersion, (int)resp.StatusCode, resp.StatusDescription);
            if (resp.ContentLength > 0)
            {
                Console.WriteLine(new StreamReader(resp.GetResponseStream()).ReadToEnd());
            }
            resp.Close();
        }
        class MyMapper : WebContentTypeMapper
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
            CustomBinding binding = new CustomBinding(new WebHttpBinding());
            binding.Elements.Find<WebMessageEncodingBindingElement>().ContentTypeMapper = new MyMapper();
            host.AddServiceEndpoint(typeof(ITest), binding, "").Behaviors.Add(new WebHttpBehavior());
            host.Open();

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(baseAddress + "/orders/shipped-orders");
            req.Method = "POST";
            req.ContentType = "application/json";
            Stream reqStream = req.GetRequestStream();
            byte[] reqBytes = Encoding.UTF8.GetBytes("{\"OrderId\":12,\"OrderDesc\":\"Black Shoes\"}");
            reqStream.Write(reqBytes, 0, reqBytes.Length);
            reqStream.Close();

            GetAndPrintResponse(req);

            req = (HttpWebRequest)HttpWebRequest.Create(baseAddress + "/orders/order-with-message");
            req.Method = "POST";
            req.ContentType = "application/json";
            reqStream = req.GetRequestStream();
            reqBytes = Encoding.UTF8.GetBytes("{\"OrderId\":12,\"OrderDesc\":\"Black Shoes\"}");
            reqStream.Write(reqBytes, 0, reqBytes.Length);
            reqStream.Close();

            GetAndPrintResponse(req);

            Console.Write("Press ENTER to close service");
            Console.ReadLine();
            host.Close();
        }
    }

    //Authorization in WebHttpBinding
    public class Post2801538a
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            string EchoForAll(string str);
            [OperationContract]
            string EchoAuthenticated(string str);
        }
        public class Service : ITest
        {
            public string EchoForAll(string str)
            {
                Console.WriteLine("In EchoForAll");
                PrintHeaders(WebOperationContext.Current.IncomingRequest.Headers);
                Console.WriteLine();
                return str;
            }
            public string EchoAuthenticated(string str)
            {
                Console.WriteLine("In EchoAuthenticated");
                PrintHeaders(WebOperationContext.Current.IncomingRequest.Headers);
                Console.WriteLine();

                WebOperationContext context = WebOperationContext.Current;
                if (!IsValidRequest(context))
                {
                    context.OutgoingResponse.StatusCode = HttpStatusCode.Unauthorized;
                    context.OutgoingResponse.Headers.Add("X-WWW-Authenticate", "WSSE realm=\"foo\", profile=\"UsernameToken\"");
                    return null;
                }
                return "Authenticated " + str;
            }
            bool IsValidRequest(WebOperationContext context)
            {
                string authHeader = context.IncomingRequest.Headers[HttpRequestHeader.Authorization];
                if (authHeader != "WSSE profile=\"UsernameToken\"")
                {
                    return false;
                }
                string xWsseHeader = context.IncomingRequest.Headers["X-WSSE"];
                return xWsseHeader != null && xWsseHeader.Contains("MyMagicPassword");
            }
        }
        static HttpWebResponse GetResponse(string uri, string body)
        {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(uri);
            req.Method = "POST";
            req.ContentType = "application/xml";
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
            return resp;
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            CustomBinding binding = new CustomBinding(new WebHttpBinding());
            host.AddServiceEndpoint(typeof(ITest), binding, "").Behaviors.Add(new WebHttpBehavior());
            host.Open();

            HttpWebResponse resp = GetResponse(baseAddress + "/EchoForAll", "<string xmlns=\"http://schemas.microsoft.com/2003/10/Serialization/\">Hello</string>");
            PrintResponse("FirstResponse", resp);

            resp = GetResponse(baseAddress + "/EchoAuthenticated", "<string xmlns=\"http://schemas.microsoft.com/2003/10/Serialization/\">Hello</string>");
            PrintResponse("SecondResponse", resp);

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(baseAddress + "/EchoAuthenticated");
            req.Method = "POST";
            req.ContentType = "application/xml";
            req.Headers[HttpRequestHeader.Authorization] = "WSSE profile=\"UsernameToken\"";
            req.Headers["X-WSSE"] = "MyName:MyMagicPassword";
            byte[] reqBytes = Encoding.UTF8.GetBytes("<string xmlns=\"http://schemas.microsoft.com/2003/10/Serialization/\">Hello</string>");
            req.GetRequestStream().Write(reqBytes, 0, reqBytes.Length);
            req.GetRequestStream().Close();
            resp = (HttpWebResponse)req.GetResponse();
            PrintResponse("ThirdResponse", resp);

            host.Close();
        }

        static void PrintHeaders(WebHeaderCollection headers)
        {
            for (int i = 0; i < headers.AllKeys.Length; i++)
            {
                string header = headers.AllKeys[i];
                Console.WriteLine("{0}: {1}", header, headers[header]);
            }
        }
        private static void PrintResponse(string name, HttpWebResponse resp)
        {
            if (resp == null)
            {
                Console.WriteLine("Response: NULL");
            }
            else
            {
                Console.WriteLine("{0}: HTTP/{1} {2} {3}", name, resp.ProtocolVersion, (int)resp.StatusCode, resp.StatusDescription);
                PrintHeaders(resp.Headers);
                if (resp.ContentLength > 0)
                {
                    Console.WriteLine(new StreamReader(resp.GetResponseStream()).ReadToEnd());
                }
                Console.WriteLine();
                resp.Close();
            }
        }
    }
    //Authorization in WebHttpBinding
    public class Post2801538
    {
        [ServiceContract]
        public interface ITestFree
        {
            [OperationContract]
            string EchoForAll(string str);
        }
        [ServiceContract]
        public interface ITestProtected
        {
            [OperationContract]
            string EchoAuthenticated(string str);
        }
        public class Service : ITestFree, ITestProtected
        {
            public string EchoForAll(string str)
            {
                return str;
            }
            public string EchoAuthenticated(string str)
            {
                return str;
            }
        }
        static HttpWebResponse GetResponse(string uri, string body)
        {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(uri);
            req.Method = "POST";
            req.ContentType = "application/xml";
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
            return resp;
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITestFree), new WebHttpBinding(), "Free").Behaviors.Add(new WebHttpBehavior());
            CustomBinding binding = new CustomBinding(new WebHttpBinding());
            binding.Elements.Find<HttpTransportBindingElement>().AuthenticationScheme = AuthenticationSchemes.Negotiate;
            host.AddServiceEndpoint(typeof(ITestProtected), binding, "Auth").Behaviors.Add(new WebHttpBehavior());
            host.Open();

            HttpWebResponse resp = GetResponse(baseAddress + "/Free/EchoForAll", "<string xmlns=\"http://schemas.microsoft.com/2003/10/Serialization/\">Hello</string>");
            PrintResponse("FirstResponse", resp);

            resp = GetResponse(baseAddress + "/Auth/EchoAuthenticated", "<string xmlns=\"http://schemas.microsoft.com/2003/10/Serialization/\">Hello</string>");
            PrintResponse("SecondResponse", resp);
        }

        static void PrintHeaders(WebHeaderCollection headers)
        {
            for (int i = 0; i < headers.AllKeys.Length; i++)
            {
                string header = headers.AllKeys[i];
                Console.WriteLine("{0}: {1}", header, headers[header]);
            }
        }
        private static void PrintResponse(string name, HttpWebResponse resp)
        {
            if (resp == null)
            {
                Console.WriteLine("Response: NULL");
            }
            else
            {
                Console.WriteLine("{0}: HTTP/{1} {2} {3}", name, resp.ProtocolVersion, (int)resp.StatusCode, resp.StatusDescription);
                PrintHeaders(resp.Headers);
                if (resp.ContentLength > 0)
                {
                    Console.WriteLine(new StreamReader(resp.GetResponseStream()).ReadToEnd());
                }
                Console.WriteLine();
                resp.Close();
            }
        }
    }

    //Test client
    public class Post2804093
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            string Hello(string name);
        }
        public class Service : ITest
        {
            public string Hello(string name)
            {
                return "Hello " + name.ToString();
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
            smb.HttpGetEnabled = true;
            host.Description.Behaviors.Add(smb);
            host.AddServiceEndpoint(typeof(ITest), new BasicHttpBinding(), "");
            host.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexHttpBinding(), "mex");
            host.Open();
            Console.WriteLine("Host opened");
            Console.Write("Press ENTER to close");
            Console.ReadLine();
            host.Close();
        }
    }

    //Stream with other information ([MessageContract])
    public class Post2826961
    {
        [MessageContract]
        public class MyFileUploadInfo
        {
            [MessageHeader]
            public string FileId;
            [MessageBodyMember]
            public Stream TheStream;
        }
        [ServiceContract]
        public interface IFileUpload
        {
            [OperationContract]
            void UploadFile(MyFileUploadInfo info);
        }
        public class Service : IFileUpload
        {
            public void UploadFile(MyFileUploadInfo info)
            {
                FileStream newFile = File.Create(info.FileId);
                byte[] buffer = new byte[10000];
                int bytesRead = 0;
                do
                {
                    bytesRead = info.TheStream.Read(buffer, 0, buffer.Length);
                    newFile.Write(buffer, 0, bytesRead);
                } while (bytesRead > 0);
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            BasicHttpBinding binding = new BasicHttpBinding();
            binding.TransferMode = TransferMode.Streamed;
            binding.MaxReceivedMessageSize = int.MaxValue;
            host.AddServiceEndpoint(typeof(IFileUpload), binding, "");
            host.Open();

            ChannelFactory<IFileUpload> factory = new ChannelFactory<IFileUpload>(binding, new EndpointAddress(baseAddress));
            IFileUpload proxy = factory.CreateChannel();
            MyFileUploadInfo request = new MyFileUploadInfo();
            request.FileId = "NewFile123";
            string codeBase = Assembly.GetEntryAssembly().CodeBase;
            request.TheStream = File.OpenRead(Path.GetFullPath(codeBase));
            proxy.UploadFile(request);

            ((IClientChannel)proxy).Close();
            factory.Close();
            host.Close();
        }
    }

    //Events from the service instances
    public class Post2827655
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            string Echo(string str);
        }
        public class Service : ITest
        {
            public event EventHandler<EventArgs> EchoCalled;
            public string Echo(string str)
            {
                EchoCalled(this, new EventArgs());
                return str;
            }
        }
        public class MyInstanceProvider : IInstanceProvider
        {
            EventHandler<EventArgs> echoCalledDelegate;
            public MyInstanceProvider(EventHandler<EventArgs> echoCalledDelegate)
            {
                this.echoCalledDelegate = echoCalledDelegate;
            }
            public object GetInstance(InstanceContext instanceContext, Message message)
            {
                Service service = new Service();
                service.EchoCalled += this.echoCalledDelegate;
                return service;
            }

            public object GetInstance(InstanceContext instanceContext)
            {
                Service service = new Service();
                service.EchoCalled += this.echoCalledDelegate;
                return service;
            }

            public void ReleaseInstance(InstanceContext instanceContext, object instance)
            {
                ((Service)instance).EchoCalled -= this.echoCalledDelegate;
            }
        }
        public class MyBehavior : IEndpointBehavior
        {
            EventHandler<EventArgs> echoCalledDelegate;
            public MyBehavior(EventHandler<EventArgs> echoCalledDelegate)
            {
                this.echoCalledDelegate = echoCalledDelegate;
            }
            public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
            {
            }
            public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
            {
            }

            public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
            {
                endpointDispatcher.DispatchRuntime.InstanceProvider = new MyInstanceProvider(this.echoCalledDelegate);
            }
            public void Validate(ServiceEndpoint endpoint)
            {
            }
        }
        static void EchoCalled(object sender, EventArgs args)
        {
            Console.WriteLine("Echo was called!");
        }
        public static void Test()
        {
            string baseAddress = "http://localhost:8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            ServiceEndpoint endpoint = host.AddServiceEndpoint(typeof(ITest), new BasicHttpBinding(), "");
            endpoint.Behaviors.Add(new MyBehavior(new EventHandler<EventArgs>(Post2827655.EchoCalled)));
            host.Open();
            Console.WriteLine("Host opened");

            for (int i = 0; i < 10; i++)
            {
                CallService(baseAddress);
            }

            host.Close();
        }
        static void CallService(string endpointAddress)
        {
            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(new BasicHttpBinding(), new EndpointAddress(endpointAddress));
            ITest proxy = factory.CreateChannel();
            Console.WriteLine(proxy.Echo("Calling the service"));
            ((IClientChannel)proxy).Close();
            factory.Close();
        }
    }

    //Deriving from WSHttpBinding to use Binary
    public class Post2832285
    {
        public class WSBindingWithBinary : WSHttpBinding
        {
            public override BindingElementCollection CreateBindingElements()
            {
                BindingElementCollection elements = base.CreateBindingElements();
                for (int i = 0; i < elements.Count; i++)
                {
                    if (elements[i] is TextMessageEncodingBindingElement || elements[i] is MtomMessageEncodingBindingElement)
                    {
                        elements[i] = new BinaryMessageEncodingBindingElement();
                    }
                }
                return elements;
            }
        }
    }
    //Using ServiceHost in IIS
    public class Post2843619
    {

        //<% @ServiceHost Service="MyNamespace.MyService" Factory="MyServiceHostFactory" Language="C#" debug="true" %>
        public class MyServiceHostFactory : ServiceHostFactory
        {
            protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
            {
                return new MyServiceHost(typeof(MyService), baseAddresses);
            }
        }
        public class MyServiceHost : ServiceHost
        {
            public MyServiceHost(Type serviceType, params Uri[] baseAddresses)
                : base(serviceType, baseAddresses)
            {
            }
            protected override void ApplyConfiguration()
            {
                base.ApplyConfiguration();
                this.AddServiceEndpoint(typeof(ITest), new BasicHttpBinding(), "");
                ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
                smb.HttpGetEnabled = true;
                this.Description.Behaviors.Add(smb);
            }
        }
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            string Echo(string input);
        }
        public class MyService : ITest
        {
            public string Echo(string input) { return input; }
        }



    }

    //Exception on SH.Open
    public class Post2861218
    {
        public sealed class HelloWCF : IHelloWCF
        {
            // indicate when a HelloWCF object is created
            HelloWCF() { Console.WriteLine("HelloWCF object created"); }

            public static void Test()
            {
                HttpListener dummy = new HttpListener();
                // define where to listen for messages
                Uri address = new Uri("http://localhost:4000/IHelloWCF");
                // define how to exchange messages
                BasicHttpBinding binding = new BasicHttpBinding();
                // instantiate a ServiceHost, passing the type to instantiate
                // when the application receives a message
                ServiceHost svc = new ServiceHost(typeof(HelloWCF));
                // add an endpoint, passing the address, binding, and contract
                svc.AddServiceEndpoint(typeof(IHelloWCF), binding, address);
                // begin listening
                try
                {
                    svc.Open();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    throw;
                }
                // indicate that the receiving application is ready and
                // keep the application from exiting immediately
                Console.WriteLine("The HelloWCF receiving application is ready");
                // wait for incoming messages
                Console.ReadLine();
                // close the service host
                svc.Close();
            }

            // received messages are dispatched to this instance
            // method as per the service contract
            public void Say(String input)
            {
                Console.WriteLine("Message received, the body contains: {0}", input);
            }
        }

        [ServiceContract]
        public interface IHelloWCF
        {
            [OperationContract]
            void Say(String input);
        }

        public static void Test()
        {
            HelloWCF.Test();
        }
    }

    public class TestDTO
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            DateTimeOffset Echo(DateTimeOffset input);
        }
        public class Service : ITest
        {
            public DateTimeOffset Echo(DateTimeOffset input)
            {
                return input;
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
            Console.WriteLine(proxy.Echo(DateTimeOffset.Now));
            Console.WriteLine(proxy.Echo(DateTimeOffset.UtcNow));
            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post2879006
    {
        [DataContract]
        public class CompositeType
        {
            ClassX x;
            [DataMember]
            public string Field
            {
                get { return x.X; }
                set { x.X = value; }
            }
            public CompositeType()
            {
                Initialize();
            }
            [OnDeserializing]
            public void OnDeserializing(StreamingContext ctx)
            {
                Initialize();
            }
            private void Initialize()
            {
                this.x = new ClassX();
            }
        }
        public class ClassX
        {
            public string X;
        }
        public static void Test()
        {
            DataContractSerializer dcs = new DataContractSerializer(typeof(CompositeType));
            MemoryStream ms = new MemoryStream();
            CompositeType instance = new CompositeType();
            instance.Field = "Hello";
            dcs.WriteObject(ms, instance);
            ms.Position = 0;
            object o = dcs.ReadObject(ms);
            Console.WriteLine(o);
        }
    }

    //Serializing extra headers
    public class Post2895712
    {
        [DataContract]
        public class HeaderContext
        {
            [DataMember]
            public Guid ID;
            [DataMember]
            public bool bTest;

            public override string ToString()
            {
                return String.Format("HeaderContext[ID={0},bTest={1}]", ID, bTest);
            }
        }
        public class TLSContext : IDisposable
        {
            OperationContextScope sc = null;
            public TLSContext(IClientChannel channel, HeaderContext token)
            {
                sc = new OperationContextScope(channel);
                MessageHeader<HeaderContext> msgHeader = new MessageHeader<HeaderContext>(token);
                MessageHeader untyped = msgHeader.GetUntypedHeader("token", "http://company/2007/Context/Token");
                OperationContext.Current.OutgoingMessageHeaders.Add(untyped);
            }
            public void Dispose()
            {
                if (sc != null)
                    sc.Dispose();
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
                int idx = OperationContext.Current.IncomingMessageHeaders.FindHeader("token", "http://company/2007/Context/Token");
                HeaderContext ctx = null;
                string response;
                if (idx >= 0)
                {
                    ctx = OperationContext.Current.IncomingMessageHeaders.GetHeader<HeaderContext>(idx);
                    response = ctx.ToString();
                }
                else
                {
                    response = "No headers";
                }
                HeaderContext outgoing = new HeaderContext();
                outgoing.ID = new Guid("11112222-3333-4444-5555-666677778888");
                outgoing.bTest = true;
                OperationContext.Current.OutgoingMessageHeaders.Add(
                    new MessageHeader<HeaderContext>(outgoing).GetUntypedHeader(
                        "outToken", "http://company/2007/Context/Token"));
                return response;
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

            HeaderContext headerContext = new HeaderContext();
            headerContext.ID = new Guid("01234567-89ab-cdef-0123-456789abcdef");
            headerContext.bTest = true;
            using (TLSContext context = new TLSContext((IClientChannel)proxy, headerContext))
            {
                Console.WriteLine(proxy.Echo("Hello"));
            }
            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post2898359b
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            //[FaultContract(typeof(string))]
            int GetLength(string input);
            [OperationContract]
            //[FaultContract(typeof(string))]
            int Divide(int x, int y);
        }
        public class Service : ITest
        {
            public int GetLength(string input) { return input.Length; }
            public int Divide(int x, int y) { return x / y; }
        }
        public class MyErrorHandler : IErrorHandler
        {
            public bool HandleError(Exception error)
            {
                Console.WriteLine("This error occurred somewhere: {0}", error.Message);
                return true;
            }
            public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
            {
                fault = Message.CreateMessage(
                    version,
                    new FaultException<string>(error.Message, new FaultReason(error.Message)).CreateMessageFault(),
                    "http://the.fault.action");
            }
        }
        public class MyServiceBehavior : IServiceBehavior
        {
            public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
            {
            }
            public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
            {
                foreach (ChannelDispatcher channelDispatcher in serviceHostBase.ChannelDispatchers)
                {
                    channelDispatcher.ErrorHandlers.Add(new MyErrorHandler());
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
            host.Description.Behaviors.Add(new MyServiceBehavior());
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(new BasicHttpBinding(), new EndpointAddress(baseAddress));
            ITest proxy = factory.CreateChannel();
            Console.WriteLine(proxy.GetLength("Hello"));
            try
            {
                Console.WriteLine(proxy.GetLength(null));
            }
            catch (FaultException e)
            {
                Console.WriteLine("FaultException: {0}", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception on GetLength(null): {0}", e);
            }
            Console.WriteLine(proxy.Divide(4, 3));
            try
            {
                Console.WriteLine(proxy.Divide(4, 0));
            }
            catch (FaultException e)
            {
                Console.WriteLine("FaultException: {0}", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception on Divide(4, 0): {0}", e);
            }

            ((IClientChannel)proxy).Close();
            factory.Close();
            host.Close();
        }
    }

    public class Post2927177
    {
        [DataContract]
        public class MyDC
        {
            [DataMember]
            public int[] intArray;
            [DataMember]
            public int[] intArray2;
            public MyDC(int size)
            {
                this.intArray = new int[size];
                this.intArray2 = new int[size];
                for (int i = 0; i < size; i++)
                {
                    this.intArray[i] = this.intArray2[i] = i;
                }
            }
        }
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            MyDC Get(int size);
            [OperationContract]
            int Put(MyDC dc);
        }
        [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
        public class Service : ITest
        {
            public MyDC Get(int size)
            {
                return new MyDC(size);
            }
            public int Put(MyDC dc)
            {
                return dc.intArray.Length;
            }
        }
        static Binding GetBinding()
        {
            BasicHttpBinding result = new BasicHttpBinding();
            result.MaxReceivedMessageSize = 1000000000;
            result.ReaderQuotas.MaxArrayLength = 1000000;
            return result;
        }
        static void IncreaseMIIOG(ServiceEndpoint endpoint)
        {
            foreach (OperationDescription operation in endpoint.Contract.Operations)
            {
                DataContractSerializerOperationBehavior dcsob = operation.Behaviors.Find<DataContractSerializerOperationBehavior>();
                if (dcsob != null)
                {
                    dcsob.MaxItemsInObjectGraph = int.MaxValue;
                }
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            ServiceEndpoint endpoint = host.AddServiceEndpoint(typeof(ITest), GetBinding(), "");
            IncreaseMIIOG(endpoint);
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(GetBinding(), new EndpointAddress(baseAddress));
            IncreaseMIIOG(factory.Endpoint);
            ITest proxy = factory.CreateChannel();

            try
            {
                Console.WriteLine(proxy.Put(new MyDC(35000)));
                Console.WriteLine(proxy.Get(35000));
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

    public class Post2939375
    {
        public class ErrorBehavior : BehaviorExtensionElement, IEndpointBehavior
        {
            public void Validate(ServiceEndpoint endpoint) { }
            public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters) { }
            public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
            {
                endpointDispatcher.ChannelDispatcher.ErrorHandlers.Clear();
                endpointDispatcher.ChannelDispatcher.ErrorHandlers.Add(new ErrorHandler());
            }
            public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime) { }
            public override Type BehaviorType { get { return typeof(ErrorBehavior); } }
            protected override object CreateBehavior() { return new ErrorBehavior(); }
        }
        // Error Handler class
        public class ErrorHandler : IErrorHandler
        {
            public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
            {
                Console.WriteLine("In ProvideFault");
            }
            public bool HandleError(Exception error)
            {
                if (error is NullReferenceException)
                {
                    return true;
                }
                return false;
            }
        }
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            string Hello(string name);
        }
        public class Service : ITest
        {
            public string Hello(string name)
            {
                return "Hello " + name.ToString();
            }
        }
        public static void Test()
        {
            ServiceHost host = new ServiceHost(typeof(Service));
            host.Open();
            Console.WriteLine("Host opened");
            Console.WriteLine(host.Description.Endpoints[0].Address.Uri);
            host.Close();
        }
    }

    //Sending binary blob - web programming model way
    public class Post2944384
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract, WebInvoke]
            void Process(Stream input);
        }

        public class Service : ITest
        {
            public void Process(Stream input)
            {
                int bytesRead = 0, totalBytesRead = 0;
                byte[] buffer = new byte[10000];
                do
                {
                    bytesRead = input.Read(buffer, 0, buffer.Length);
                    totalBytesRead += bytesRead;
                } while (bytesRead > 0);
                Console.WriteLine("Received {0} bytes from the client", totalBytesRead);
            }
        }

        public class MyContentTypeMapper : WebContentTypeMapper
        {
            public override WebContentFormat GetMessageFormatForContentType(string contentType)
            {
                return WebContentFormat.Raw;
            }
        }
        static Binding CreateBinding()
        {
            WebMessageEncodingBindingElement webBE = new WebMessageEncodingBindingElement();
            webBE.ContentTypeMapper = new MyContentTypeMapper();
            HttpTransportBindingElement httpBE = new HttpTransportBindingElement();
            httpBE.MaxReceivedMessageSize = int.MaxValue;
            httpBE.ManualAddressing = true;
            CustomBinding result = new CustomBinding(webBE, httpBE);
            return result;
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            WebHttpBinding serverBinding = new WebHttpBinding();
            ServiceEndpoint endpoint = host.AddServiceEndpoint(typeof(ITest), CreateBinding(), "");
            endpoint.Behaviors.Add(new WebHttpBehavior());
            host.Open();
            Console.WriteLine("Host opened");

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(baseAddress + "/Process");
            req.Method = "POST";
            req.ContentType = "application/octet-stream";
            string reqBody = @"whatever you want to send to the service in a binary blob";
            byte[] reqBodyBytes = Encoding.UTF8.GetBytes(reqBody);
            Stream reqStream = req.GetRequestStream();
            reqStream.Write(reqBodyBytes, 0, reqBodyBytes.Length);
            reqStream.Close();

            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Console.WriteLine("HTTP/{0} {1} {2}", resp.ProtocolVersion, (int)resp.StatusCode, resp.StatusDescription);
            Console.WriteLine(new StreamReader(resp.GetResponseStream()).ReadToEnd());
            resp.Close();

            Console.WriteLine("Now using the programming model to send data to the service...");
            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(CreateBinding(), new EndpointAddress(baseAddress));
            factory.Endpoint.Behaviors.Add(new WebHttpBehavior());
            ITest proxy = factory.CreateChannel();

            FileStream fs = new FileStream(Assembly.GetEntryAssembly().Location, FileMode.Open, FileAccess.Read, FileShare.Read);
            proxy.Process(fs);
            fs.Close();

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.WriteLine("Press ENTER to close service");
            Console.ReadLine();
            host.Close();
        }
    }

    //Sending binary blob - binary way
    public class Post2944384b
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            void Process(byte[] input);
        }

        public class Service : ITest
        {
            public void Process(byte[] input)
            {
                Console.WriteLine("Received {0} bytes from the client", input.Length);
            }
        }

        static Binding CreateBinding()
        {
            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Mode = SecurityMode.None;
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.ReaderQuotas.MaxArrayLength = int.MaxValue;
            return binding;
        }
        public static void Test()
        {
            string baseAddress = "net.tcp://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), CreateBinding(), "");
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(CreateBinding(), new EndpointAddress(baseAddress));
            ITest proxy = factory.CreateChannel();

            byte[] input = new byte[23456];
            Random rndGen = new Random(1);
            rndGen.NextBytes(input);
            proxy.Process(input);

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.WriteLine("Press ENTER to close service");
            Console.ReadLine();
            host.Close();
        }
    }

    public class RegexTest
    {
        public static void Test()
        {
            string regexStr = @"([^,\s]+)\s*,\s*([^,\s]+)\s*,\s*(Version=[^\s,]+)\s*,\s*(Culture=[^\s,]+)\s*,\s*(PublicKeyToken=[^\s,]+)";
            Regex regex = new Regex(regexStr);
            Match match = regex.Match(typeof(Dictionary<string, int>).AssemblyQualifiedName);
            Console.WriteLine("Type: {0}", match.Groups[1].Value);
            Console.WriteLine("Assembly name: {0}", match.Groups[2].Value);
            Console.WriteLine("Assembly version: {0}", match.Groups[3].Value);
            Console.WriteLine("Assembly culture: {0}", match.Groups[4].Value);
            Console.WriteLine("Assembly key: {0}", match.Groups[5].Value);
        }
    }

    public class Post2956635
    {
        [DataContract]
        public class Person
        {
            [DataMember]
            public string FirstName;
            [DataMember]
            public string LastName;
            [DataMember]
            public string Address;
            [DataMember]
            public string City;
            [DataMember]
            public string State;
            [DataMember]
            public string Zipcode;
            [DataMember]
            public byte[] SoundFile;
        }
        [MessageContract]
        public class PersonMC
        {
            [MessageHeader]
            public string FirstName;
            [MessageHeader]
            public string LastName;
            [MessageHeader]
            public string Address;
            [MessageHeader]
            public string City;
            [MessageHeader]
            public string State;
            [MessageHeader]
            public string Zipcode;
            [MessageBodyMember]
            public Stream SoundFile;
        }
        public static void Test() { }
    }

    public class Post2965772
    //    public class SelfHostedServiceWithSilverlightPolicy
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            string Echo(string text);
        }
        [ServiceContract]
        public interface IPolicyRetriever
        {
            [OperationContract, WebGet(UriTemplate = "/clientaccesspolicy.xml")]
            Stream GetSilverlightPolicy();
            [OperationContract, WebGet(UriTemplate = "/crossdomain.xml")]
            Stream GetFlashPolicy();
        }
        public class Service : ITest, IPolicyRetriever
        {
            public string Echo(string text) { return text; }
            Stream StringToStream(string result)
            {
                WebOperationContext.Current.OutgoingResponse.ContentType = "application/xml";
                return new MemoryStream(Encoding.UTF8.GetBytes(result));
            }
            public Stream GetSilverlightPolicy()
            {
                string result = @"<?xml version=""1.0"" encoding=""utf-8""?>
<access-policy>
  <cross-domain-access>
    <policy>
      <allow-from>
        <domain uri=""*""/>
      </allow-from>
      <grant-to>
        <resource path=""/"" include-subpaths=""true""/>
      </grant-to>
    </policy>
  </cross-domain-access>
</access-policy>";
                return StringToStream(result);
            }
            public Stream GetFlashPolicy()
            {
                string result = @"<?xml version=""1.0""?>
<!DOCTYPE cross-domain-policy SYSTEM ""http://www.macromedia.com/xml/dtds/cross-domain-policy.dtd"">
<cross-domain-policy>
    <allow-access-from domain=""*"" />
</cross-domain-policy>";
                return StringToStream(result);
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), new BasicHttpBinding(), "basic");
            host.AddServiceEndpoint(typeof(IPolicyRetriever), new WebHttpBinding(), "").Behaviors.Add(new WebHttpBehavior());
            ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
            smb.HttpGetEnabled = true;
            host.Description.Behaviors.Add(smb);
            host.Open();
            Console.WriteLine("Host opened");

            Console.Write("Press ENTER to close");
            Console.ReadLine();
            host.Close();
        }
    }

    //FaultException<T> not getting caught
    public class Post2994848
    {
        public class SynchronizationException : Exception
        {
            public DateTime ExceptionEventTime = DateTime.Now;
            public string m_errorId = "errorId";
            public string ExtendedData = "ExtendedData";
        }
        [DataContract()]
        [Serializable()]
        public class SynchronizationExceptionData
        {
            private string m_Message = string.Empty;
            private string m_StackTrace = string.Empty;
            private string m_ExtendedData = string.Empty;
            private DateTime m_EventDate = DateTime.UtcNow;
            private string m_ErrorID = string.Empty;
            [DataMember(Order = 0)]
            public DateTime EventTime
            {
                get { return m_EventDate; }
                set { m_EventDate = value; }
            }
            [DataMember(Order = 1)]
            public string ErrorID
            {
                get { return m_ErrorID; }
                set { m_ErrorID = value; }
            }
            [DataMember(Order = 2)]
            public string Message
            {
                get { return m_Message; }
                set { m_Message = value; }
            }
            [DataMember(Order = 3)]
            public string StackTrace
            {
                get { return m_StackTrace; }
                set { m_StackTrace = value; }
            }
            [DataMember(Order = 4)]
            public string ExtendedData
            {
                get { return m_ExtendedData; }
                set { m_ExtendedData = value; }
            }
            public SynchronizationExceptionData(SynchronizationException sExcep)
            {
                try
                {
                    m_EventDate = sExcep.ExceptionEventTime;
                    m_Message = sExcep.Message;
                    m_StackTrace = sExcep.StackTrace;
                    m_ErrorID = sExcep.m_errorId;
                    m_ExtendedData = sExcep.ExtendedData;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            public SynchronizationExceptionData(string message, string stackTrace, string extendedData, string errorID, DateTime eventTime)
            {
                try
                {
                    m_Message = message;
                    m_StackTrace = stackTrace;
                    m_ExtendedData = extendedData;
                    m_ErrorID = errorID;
                    m_EventDate = eventTime;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            [FaultContract(typeof(SynchronizationExceptionData))]
            string Echo(string input);
        }
        public class Service : ITest
        {
            public string Echo(string input)
            {
                SynchronizationExceptionData exceptionData = new SynchronizationExceptionData("message", "stackTrace", "extendedData", "errorId", DateTime.Now);
                throw new FaultException<SynchronizationExceptionData>(exceptionData, new FaultReason("Hi message"), new FaultCode("FAILED"));
            }
        }
        public static void Test()
        {
            Console.WriteLine(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
            string baseAddress = "http://" + Environment.MachineName + ":8000";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), new BasicHttpBinding(), "");
            ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
            smb.HttpGetEnabled = true;
            host.Description.Behaviors.Add(smb);
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(new BasicHttpBinding(), new EndpointAddress(baseAddress));
            ITest proxy = factory.CreateChannel();

            try
            {
                proxy.Echo("hello");
            }
            catch (FaultException<SynchronizationExceptionData> ex)
            {
                SynchronizationExceptionData sExcep = ex.Detail;
                Console.WriteLine(sExcep.GetType().ToString());
            }
            catch (FaultException fEx)
            {
                Console.WriteLine("Action : " + fEx.Action);
                Console.WriteLine("Code : " + fEx.Code);
                Console.WriteLine("Reason : " + fEx.Reason.ToString());
                Console.WriteLine("Message : " + fEx.Message);
                Console.WriteLine("StackTrace : " + fEx.StackTrace);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message + Environment.NewLine + ex.StackTrace);
            }

            Console.Write("Press ENTER to close");
            Console.ReadLine();
            host.Close();
        }

    }

    //inheritance and DCS
    public class Post2999301
    {
        [DataContract]
        [KnownType(typeof(DocumentDescriptor))]
        public class IndexInformation
        {
            [DataMember]
            public string str = "Hello";
            [DataMember]
            public char c = 'r';
        }

        [DataContract]
        //[KnownType(typeof(IndexInformation))]
        public class DocumentDescriptor : IndexInformation
        {
            [DataMember]
            public int iDerived = 123;
            [DataMember]
            public double dblDerived = 1234.5678;
            [DataMember]
            public string strDerived = "World";
        }
        public static void Test()
        {
            // Write (indexInformation is acually a derived class (DocumentDescriptor)
            IndexInformation indexInformation = new DocumentDescriptor();
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.ConformanceLevel = ConformanceLevel.Auto;
            settings.CloseOutput = true;
            settings.OmitXmlDeclaration = true;
            settings.Encoding = Encoding.ASCII;
            using (MemoryStream ms = new MemoryStream(0x0400))
            {
                using (XmlWriter writer = XmlWriter.Create(ms, settings))
                {
                    new DataContractSerializer(indexInformation.GetType()).WriteObject(writer, indexInformation);
                }
                sb.Append(Encoding.ASCII.GetString(ms.ToArray()));
            }
            Console.WriteLine("Serialized: " + sb.ToString());
            XmlDocument document = new XmlDocument();
            document.LoadXml(sb.ToString());

            // Read the same information back, expecting to get the derived class.
            // examining the xml reveals that the derived class attributes are in fact present.
            DataContractSerializer serializer = new DataContractSerializer(typeof(IndexInformation));
            XmlReader reader = XmlReader.Create(new StringReader(sb.ToString()));

            object instance = serializer.ReadObject(reader, true);

            Console.WriteLine(instance);
            if (instance != null)
            {
                Console.WriteLine(instance.GetType().FullName);
            }
        }
    }

    //changing serializer in code
    public class Post3018387
    {
        [DataContract]
        public class MyDC
        {
            [DataMember]
            [XmlElementAttribute(ElementName = "myStr")]
            public string str;
            public MyDC() { }
            public MyDC(string str) { this.str = str; }
            public override string ToString()
            {
                return String.Format("MyDC[str={0}]", str);
            }
        }
        [ServiceContract]
        [XmlSerializerFormat]
        public interface ITest
        {
            [OperationContract]
            MyDC Echo(MyDC input);
        }
        [ServiceContract]
        [XmlSerializerFormat]
        public interface IXmlSerTest : ITest
        {
        }
        public class Service : ITest, IXmlSerTest
        {
            public MyDC Echo(MyDC input)
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
            ServiceEndpoint dcsEndpoint = host.AddServiceEndpoint(typeof(ITest), GetBinding(), "dcs");
            ServiceEndpoint xmlSerEndpoint = host.AddServiceEndpoint(typeof(IXmlSerTest), GetBinding(), "xmlser");
            ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
            smb.HttpGetEnabled = true;
            host.Description.Behaviors.Add(smb);
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(GetBinding(), new EndpointAddress(baseAddress + "/dcs"));
            ITest proxy = factory.CreateChannel();
            Console.WriteLine(proxy.Echo(new MyDC("Hello")));

            ((IClientChannel)proxy).Close();
            factory.Close();

            ChannelFactory<IXmlSerTest> factory2 = new ChannelFactory<IXmlSerTest>(GetBinding(), new EndpointAddress(baseAddress + "/xmlser"));
            IXmlSerTest proxy2 = factory2.CreateChannel();
            Console.WriteLine(proxy2.Echo(new MyDC("Hello")));
            ((IClientChannel)proxy2).Close();
            factory2.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    //Modifying messages
    public class Post3022028
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            string SendMessage(string text);
        }
        public class Service : ITest
        {
            public string SendMessage(string text)
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
        public class ClientOutputMessageInspector : IClientMessageInspector, IEndpointBehavior
        {
            #region IClientMessageInspector Members
            public void AfterReceiveReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
            {
                return;
            }
            public object BeforeSendRequest(ref System.ServiceModel.Channels.Message request, IClientChannel channel)
            {
                string action = request.Headers.GetHeader<string>("Action", request.Headers[0].Namespace);
                if (action.Contains("SendMessage"))
                {
                    XmlDocument doc = new XmlDocument();
                    MemoryStream ms = new MemoryStream();
                    XmlWriter writer = XmlWriter.Create(ms);
                    request.WriteMessage(writer);
                    writer.Flush();
                    ms.Position = 0;
                    doc.Load(ms);
                    Console.WriteLine(doc.OuterXml);

                    ChangeMessage(doc);

                    ms.SetLength(0);
                    writer = XmlWriter.Create(ms);
                    doc.WriteTo(writer);
                    writer.Flush();
                    ms.Position = 0;
                    XmlReader reader = XmlReader.Create(ms);
                    request = Message.CreateMessage(reader, int.MaxValue, request.Version);
                }
                return null;
            }
            void ChangeMessage(XmlDocument doc)
            {
                XmlNamespaceManager nsManager = new XmlNamespaceManager(doc.NameTable);
                nsManager.AddNamespace("s", "http://schemas.xmlsoap.org/soap/envelope/");
                nsManager.AddNamespace("tempuri", "http://tempuri.org/");
                XmlNode node = doc.SelectSingleNode("//s:Body/tempuri:SendMessage/tempuri:text", nsManager);
                if (node != null)
                {
                    XmlText text = node.FirstChild as XmlText;
                    if (text != null)
                    {
                        text.Value = "Modified: " + text.Value;
                    }
                }
            }
            #endregion

            #region IEndpointBehavior Members

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

            #endregion
        }

        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), GetBinding(), "");
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(GetBinding(), new EndpointAddress(baseAddress));
            factory.Endpoint.Behaviors.Add(new ClientOutputMessageInspector());
            ITest proxy = factory.CreateChannel();
            Console.WriteLine(proxy.SendMessage("Hello"));

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post3062729
    {
        [CollectionDataContract(ItemName = "MyID", Namespace = "MyNamespace")]
        public class MySoapHeader : List<int>
        {
            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("MySoapHeader[");
                for (int i = 0; i < this.Count; i++)
                {
                    if (i > 0) sb.Append(',');
                    sb.Append(this[i]);
                }
                sb.Append("]");
                return sb.ToString();
            }
        }

        [MessageContract]
        public class MyMessage
        {
            [MessageHeader(Namespace = "MyNamespace")]
            public MySoapHeader myHeader;
            [MessageBodyMember]
            public string str;

            public override string ToString()
            {
                return String.Format("MyMessage[str={0},myHeader={1}]", str, myHeader);
            }
        }

        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            void Process(MyMessage message);
        }
        public class Service : ITest
        {
            public void Process(MyMessage message)
            {
                Console.WriteLine(message);
            }
        }
        static Binding GetBinding()
        {
            WSHttpBinding result = new WSHttpBinding();
            result.Security.Mode = SecurityMode.None;
            return result;
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), GetBinding(), "");
            ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
            smb.HttpGetEnabled = true;
            host.Description.Behaviors.Add(smb);
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(GetBinding(), new EndpointAddress(baseAddress));
            ITest proxy = factory.CreateChannel();

            MyMessage input = new MyMessage();
            input.str = "Body member";
            input.myHeader = new MySoapHeader();
            input.myHeader.Add(11);
            input.myHeader.Add(22);
            proxy.Process(input);

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post3081695
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
                WebOperationContext.Current.OutgoingResponse.Headers.Add("X-MyHeader", "My header value");
                return text;
            }
        }
        static Binding GetBinding()
        {
            WebHttpBinding result = new WebHttpBinding();
            return result;
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), GetBinding(), "").Behaviors.Add(new WebHttpBehavior());
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(GetBinding(), new EndpointAddress(baseAddress));
            factory.Endpoint.Behaviors.Add(new WebHttpBehavior());
            ITest proxy = factory.CreateChannel();

            using (OperationContextScope scope = new OperationContextScope((IContextChannel)proxy))
            {
                Console.WriteLine(proxy.Echo("Hello"));

                string myHeader = WebOperationContext.Current.IncomingResponse.Headers["X-MyHeader"];
                Console.WriteLine("X-MyHeader: {0}", myHeader);
            }

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post3108334
    {
        [DataContract]
        public class Person
        {
            [DataMember]
            public string name;
            [DataMember]
            public int age;
        }
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            string Echo(string text);
            [OperationContract]
            string GetPersonName(Person person);
        }
        public class Service : ITest
        {
            public string Echo(string text)
            {
                return text;
            }
            public string GetPersonName(Person person)
            {
                return person.name;
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
            ITest proxy = factory.CreateChannel();
            Console.WriteLine(proxy.Echo("Hello"));
            Console.WriteLine(proxy.GetPersonName(new Person
            {
                age = 21,
                name = "John Doe"
            }));

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class IndiTalk20080415_095300
    {
        [DataContract]
        public class MyDC
        {
            [DataMember]
            public MyDC left;
            [DataMember]
            public MyDC right;
            [DataMember]
            public int data;

            public static MyDC CreateInstance()
            {
                return new MyDC
                {
                    data = 4,
                    left = new MyDC
                    {
                        data = 2,
                        left = new MyDC { data = 1 },
                        right = new MyDC { data = 3 }
                    },
                    right = new MyDC
                    {
                        data = 6,
                        left = new MyDC { data = 5 },
                        right = new MyDC { data = 7 }
                    }
                };
            }
        }
        public static void Test()
        {
            Message original = Message.CreateMessage(MessageVersion.Soap12WSAddressing10, "http://tempuri.org/Process", MyDC.CreateInstance());
            Console.WriteLine(original.ToString());
            MessageBuffer buffer = original.CreateBufferedCopy(int.MaxValue);
            Message duplicate = buffer.CreateMessage();
            Console.WriteLine(duplicate.ToString());
        }
    }

    //serializing empty data tables
    public class Post3174560
    {
        public static void Test()
        {
            DataContractSerializer dcs = new DataContractSerializer(typeof(DataTable));
            MemoryStream ms = new MemoryStream();
            DataTable table = new DataTable("People");
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Age", typeof(int));
            table.Rows.Add("John Doe", 31);
            try
            {
                dcs.WriteObject(ms, table);
                Console.WriteLine(Encoding.UTF8.GetString(ms.ToArray()));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    //deserializing SoapFault
    public class Post3177859
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract(Action = Service.Action, IsOneWay = true)]
            void MyError(Message message);
        }
        public class Service : ITest
        {
            public const string Action = "http://my.action";
            public void MyError(Message message)
            {
                MessageFault fault = MessageFault.CreateFault(message, int.MaxValue);
                FaultException ex = FaultException.CreateFault(fault);
                Console.WriteLine(ex);
                //FaultException ex = FaultException.CreateFault(message);
                //Console.WriteLine(ex);
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

            FaultException e = new FaultException("reason");
            Message m = Message.CreateMessage(GetBinding().MessageVersion, e.CreateMessageFault(), Service.Action);
            proxy.MyError(m);

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post3178891
    {
        [ServiceContract(SessionMode = SessionMode.Required)]
        public interface ITest
        {
            [OperationContract(IsOneWay = true)]
            void Echo(string text);
        }
        [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Reentrant)]
        public class Service : ITest
        {
            public void Echo(string text)
            {
                int num = OperationContext.Current.IncomingMessageHeaders.FindHeader("DataHeader", "http://Test");
                Console.WriteLine(num);
                if (num >= 0)
                {
                    string headerValue = OperationContext.Current.IncomingMessageHeaders.GetHeader<string>(num);
                    Console.WriteLine(headerValue);
                }
            }
        }
        static Binding GetBinding()
        {
            //BasicHttpBinding result = new BasicHttpBinding();
            NetTcpBinding result = new NetTcpBinding();
            //Change binding settings here
            return result;
        }
        public static void Test()
        {
            //string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            string baseAddress = "net.tcp://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), GetBinding(), "");
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(GetBinding(), new EndpointAddress(baseAddress));
            ITest proxy = factory.CreateChannel();

            using (OperationContextScope scope = new OperationContextScope((IContextChannel)proxy))
            {
                MessageHeader header = MessageHeader.CreateHeader(
                    "DataHeader",
                    "http://Test",
                    "some data");
                OperationContext.Current.OutgoingMessageHeaders.Add(header);
                proxy.Echo("Hello");
            }

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post3366855
    {
        [DataContract]
        public class JsonResponse
        {
            [DataMember]
            public string totalCount
            {
                set { }
                get
                {
                    int result = 0;
                    foreach (ItemType item in this.items)
                    {
                        result += int.Parse(item.count);
                    }
                    return result.ToString();
                }
            }
            [DataMember]
            public ItemType[] items;
        }
        [DataContract]
        public class ItemType
        {
            [DataMember]
            public string id;
            [DataMember]
            public string name;
            [DataMember]
            public string count;
        }
        [ServiceContract]
        public interface ITest
        {
            [OperationContract, WebGet(BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
            JsonResponse GetResponse();
        }
        public class Service : ITest
        {
            public JsonResponse GetResponse()
            {
                JsonResponse result = new JsonResponse
                {
                    items = new ItemType[] {
                        new ItemType { count = "2", id="1", name="Red"},
                        new ItemType { count = "4", id="2", name="Blue"},
                        new ItemType { count = "6", id="3", name="Yellow"},
                    }
                };
                return result;
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            ServiceEndpoint endpoint = host.AddServiceEndpoint(typeof(ITest), new WebHttpBinding(), "");
            endpoint.Behaviors.Add(new WebHttpBehavior());
            host.Open();
            Console.WriteLine("Host opened");

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(baseAddress + "/GetResponse");
            req.Method = "GET";
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Console.WriteLine("HTTP/{0} {1} {2}", resp.ProtocolVersion, (int)resp.StatusCode, resp.StatusDescription);
            Console.WriteLine(new StreamReader(resp.GetResponseStream()).ReadToEnd());
            resp.Close();

            Console.WriteLine("Press ENTER to close service");
            Console.ReadLine();
            host.Close();
        }
    }
    public class Post3365906
    {
        [CollectionDataContract(Name = "Campaigns", Namespace = "", ItemName = "Campaign")]
        public class Campaigns : List<Campaign>
        {
        }
        [DataContract(Namespace = "")]
        public class Campaign
        {
            [DataMember]
            public int CampaignID;
        }
        [ServiceContract(Namespace = "")]
        public interface ITest
        {
            [OperationContract, WebGet(BodyStyle = WebMessageBodyStyle.Bare)]
            Campaigns GetCampaigns();
        }
        public class Service : ITest
        {
            public Campaigns GetCampaigns()
            {
                return new Campaigns
                {
                    new Campaign { CampaignID = 1 },
                    new Campaign { CampaignID = 2 },
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

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(baseAddress + "/GetCampaigns");
            req.Method = "GET";
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Console.WriteLine("HTTP/{0} {1} {2}", resp.ProtocolVersion, (int)resp.StatusCode, resp.StatusDescription);
            Console.WriteLine(new StreamReader(resp.GetResponseStream()).ReadToEnd());
            resp.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    //Customized default page
    public class Post3387770
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            string Echo(string text);
        }
        [ServiceContract]
        public interface IDefaultPage
        {
            [OperationContract, WebGet(UriTemplate = "")]
            Stream GetDefaultPage();
        }
        public class Service : ITest, IDefaultPage
        {
            public string Echo(string text)
            {
                return text;
            }
            public Stream GetDefaultPage()
            {
                string result = "<HTML><HEAD><TITLE>The page</TITLE></HEAD>" +
                    "<BODY><H1>This is a heading!</H1></BODY></HTML>";
                WebOperationContext.Current.OutgoingResponse.ContentType = "text/html";
                return new MemoryStream(Encoding.UTF8.GetBytes(result));
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), new BasicHttpBinding(), "endpoint");
            host.AddServiceEndpoint(typeof(IDefaultPage), new WebHttpBinding(), "").Behaviors.Add(new WebHttpBehavior());
            ServiceDebugBehavior sdb = host.Description.Behaviors.Find<ServiceDebugBehavior>();
            if (sdb == null)
            {
                sdb = new ServiceDebugBehavior();
                host.Description.Behaviors.Add(sdb);
            }
            sdb.HttpHelpPageEnabled = false;
            host.Open();
            Console.WriteLine("Host opened");

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }
    //posting data with WCF
    public class Post3409504
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract, WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "")]
            void PostData(Stream data);
        }
        public static void Test()
        {
            string address = "http://<server>/Information/Receive.dll";
            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(new WebHttpBinding(), new EndpointAddress(address));
            factory.Endpoint.Behaviors.Add(new WebHttpBehavior());
            FileStream fileToSend = File.OpenRead("TheFile.xml");
            ITest proxy = factory.CreateChannel();
            using (OperationContextScope scope = new OperationContextScope((IContextChannel)proxy))
            {
                WebOperationContext.Current.OutgoingRequest.ContentType = "text/xml";
                proxy.PostData(fileToSend);
            }
            ((IClientChannel)proxy).Close();
            factory.Close();
        }
    }

    //HTTP error codes and WebHttpBinding
    public class Post3472975
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
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.Ambiguous;
                return text;
            }
        }
        static Binding GetBinding()
        {
            WebHttpBinding result = new WebHttpBinding();
            return result;
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), GetBinding(), "").Behaviors.Add(new WebHttpBehavior());
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(GetBinding(), new EndpointAddress(baseAddress));
            factory.Endpoint.Behaviors.Add(new WebHttpBehavior());
            ITest proxy = factory.CreateChannel();
            try
            {
                Console.WriteLine(proxy.Echo("Hello"));
            }
            catch (ProtocolException e)
            {
                Console.WriteLine("Caught {0}: {1}", e.GetType().FullName, e.Message);
                WebException inner = e.InnerException as WebException;
                if (inner != null)
                {
                    HttpWebResponse resp = (HttpWebResponse)inner.Response;
                    Console.WriteLine("HTTP/{0} {1} {2}", resp.ProtocolVersion, (int)resp.StatusCode, resp.StatusDescription);
                    Console.WriteLine(new StreamReader(resp.GetResponseStream()).ReadToEnd());
                }
            }

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }
    public class Post3469454
    {
        [DataContract]
        public class CompositeType
        {
            [DataMember]
            public string str1 = "Hello";
            [DataMember]
            public string str2 = "World";
        }
        [CollectionDataContract]
        public class CompositeDict : IDictionary<int, CompositeType>
        {
            Dictionary<int, CompositeType> inner = new Dictionary<int, CompositeType>();
            #region IDictionary<int,CompositeType> Members

            public void Add(int key, CompositeType value)
            {
                inner.Add(key, value);
            }

            public bool ContainsKey(int key)
            {
                return inner.ContainsKey(key);
            }

            public ICollection<int> Keys
            {
                get { return inner.Keys; }
            }

            public bool Remove(int key)
            {
                return inner.Remove(key);
            }

            public bool TryGetValue(int key, out CompositeType value)
            {
                return inner.TryGetValue(key, out value);
            }

            public ICollection<CompositeType> Values
            {
                get { return inner.Values; }
            }

            public CompositeType this[int key]
            {
                get
                {
                    return inner[key];
                }
                set
                {
                    inner[key] = value;
                }
            }

            #endregion

            #region ICollection<KeyValuePair<int,CompositeType>> Members

            public void Add(KeyValuePair<int, CompositeType> item)
            {
                ((ICollection<KeyValuePair<int, CompositeType>>)inner).Add(item);
            }

            public void Clear()
            {
                ((ICollection<KeyValuePair<int, CompositeType>>)inner).Clear();
            }

            public bool Contains(KeyValuePair<int, CompositeType> item)
            {
                return ((ICollection<KeyValuePair<int, CompositeType>>)inner).Contains(item);
            }

            public void CopyTo(KeyValuePair<int, CompositeType>[] array, int arrayIndex)
            {
                ((ICollection<KeyValuePair<int, CompositeType>>)inner).CopyTo(array, arrayIndex);
            }

            public int Count
            {
                get { return ((ICollection<KeyValuePair<int, CompositeType>>)inner).Count; }
            }

            public bool IsReadOnly
            {
                get { return ((ICollection<KeyValuePair<int, CompositeType>>)inner).IsReadOnly; }
            }

            public bool Remove(KeyValuePair<int, CompositeType> item)
            {
                return ((ICollection<KeyValuePair<int, CompositeType>>)inner).Remove(item);
            }

            #endregion

            #region IEnumerable<KeyValuePair<int,CompositeType>> Members

            public IEnumerator<KeyValuePair<int, CompositeType>> GetEnumerator()
            {
                return ((IEnumerable<KeyValuePair<int, CompositeType>>)inner).GetEnumerator();
            }

            #endregion

            #region IEnumerable Members

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return ((System.Collections.IEnumerable)inner).GetEnumerator();
            }

            #endregion
        }

        [CollectionDataContract]
        public class SecDict : IDictionary<int, CompositeDict>
        {
            Dictionary<int, CompositeDict> inner = new Dictionary<int, CompositeDict>();
            #region IDictionary<int,CompositeDict> Members

            public void Add(int key, CompositeDict value)
            {
                inner.Add(key, value);
            }

            public bool ContainsKey(int key)
            {
                return inner.ContainsKey(key);
            }

            public ICollection<int> Keys
            {
                get { return inner.Keys; }
            }

            public bool Remove(int key)
            {
                return inner.Remove(key);
            }

            public bool TryGetValue(int key, out CompositeDict value)
            {
                return inner.TryGetValue(key, out value);
            }

            public ICollection<CompositeDict> Values
            {
                get { return inner.Values; }
            }

            public CompositeDict this[int key]
            {
                get
                {
                    return inner[key];
                }
                set
                {
                    inner[key] = value; ;
                }
            }

            #endregion

            #region ICollection<KeyValuePair<int,CompositeDict>> Members

            public void Add(KeyValuePair<int, CompositeDict> item)
            {
                ((ICollection<KeyValuePair<int, CompositeDict>>)inner).Add(item);
            }

            public void Clear()
            {
                ((ICollection<KeyValuePair<int, CompositeDict>>)inner).Clear();
            }

            public bool Contains(KeyValuePair<int, CompositeDict> item)
            {
                return ((ICollection<KeyValuePair<int, CompositeDict>>)inner).Contains(item);
            }

            public void CopyTo(KeyValuePair<int, CompositeDict>[] array, int arrayIndex)
            {
                ((ICollection<KeyValuePair<int, CompositeDict>>)inner).CopyTo(array, arrayIndex);
            }

            public int Count
            {
                get { return ((ICollection<KeyValuePair<int, CompositeDict>>)inner).Count; }
            }

            public bool IsReadOnly
            {
                get { return ((ICollection<KeyValuePair<int, CompositeDict>>)inner).IsReadOnly; }
            }

            public bool Remove(KeyValuePair<int, CompositeDict> item)
            {
                return ((ICollection<KeyValuePair<int, CompositeDict>>)inner).Remove(item);
            }

            #endregion

            #region IEnumerable<KeyValuePair<int,CompositeDict>> Members

            public IEnumerator<KeyValuePair<int, CompositeDict>> GetEnumerator()
            {
                return ((IEnumerable<KeyValuePair<int, CompositeDict>>)inner).GetEnumerator();
            }

            #endregion

            #region IEnumerable Members

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return ((System.Collections.IEnumerable)inner).GetEnumerator();
            }

            #endregion
        }

        public static void Test()
        {
            DataContractSerializer dcs = new DataContractSerializer(typeof(SecDict));
            CompositeDict cd1 = new CompositeDict();
            cd1.Add(1, new CompositeType { str1 = "First 1 1", str2 = "Second 1 1" });
            cd1.Add(2, new CompositeType { str1 = "First 1 2", str2 = "Second 1 2" });
            CompositeDict cd2 = new CompositeDict();
            cd2.Add(1, new CompositeType { str1 = "First 2 1", str2 = "Second 2 1" });
            cd2.Add(2, new CompositeType { str1 = "First 2 2", str2 = "Second 2 2" });
            SecDict sectDict = new SecDict();
            sectDict.Add(1, cd1);
            sectDict.Add(2, cd2);
            MemoryStream ms = new MemoryStream();
            XmlWriterSettings writerSettings = new XmlWriterSettings();
            writerSettings.Indent = true;
            writerSettings.IndentChars = "  ";
            XmlWriter writer = XmlWriter.Create(ms, writerSettings);
            dcs.WriteObject(writer, sectDict);
            writer.Flush();
            Console.WriteLine(Encoding.UTF8.GetString(ms.ToArray()));
        }
    }

    public class Post3484461
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
        public class LiveTraceListener : TraceListener
        {
            public LiveTraceListener(string initializeData) { }
            public override void Write(string message)
            {
                Console.Write("[LiveTraceListener-{0}] {1}", DateTime.Now.ToString("hh:mm:ss.fff"), message);
            }

            public override void WriteLine(string message)
            {
                Console.WriteLine("[LiveTraceListener-{0}] {1}", DateTime.Now.ToString("hh:mm:ss.fff"), message);
            }
        }
    }

    //Transferring image
    public class Post3499341
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            void GetImageDimensions(byte[] imageBytes, out int width, out int height);
        }
        public class Service : ITest
        {
            public void GetImageDimensions(byte[] imageBytes, out int width, out int height)
            {
                width = height = 0;
                Image image = new Bitmap(new MemoryStream(imageBytes));
                width = image.Width;
                height = image.Height;
            }
        }
        static Binding GetBinding()
        {
            BasicHttpBinding result = new BasicHttpBinding();
            result.MaxReceivedMessageSize = int.MaxValue;
            result.ReaderQuotas.MaxArrayLength = int.MaxValue;
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

            byte[] bitmapBytes = File.ReadAllBytes(@"c:\temp\image.bmp");
            int width, height;
            proxy.GetImageDimensions(bitmapBytes, out width, out height);
            Console.WriteLine("Image dimensions: {0}, {1}", width, height);

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    //Changing DCS on server
    public class Post3475483
    {
        [DataContract]
        public class MyCircularList
        {
            [DataMember]
            public int data;
            [DataMember]
            MyCircularList next;
            public static MyCircularList Create()
            {
                MyCircularList result = new MyCircularList
                {
                    data = 1,
                    next = new MyCircularList
                    {
                        data = 2,
                        next = new MyCircularList
                        {
                            data = 3,
                            next = null,
                        }
                    }
                };
                result.next.next.next = result;
                return result;
            }
        }
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            MyCircularList EchoList(MyCircularList list);
        }
        public class Service : ITest
        {
            public MyCircularList EchoList(MyCircularList list)
            {
                return list;
            }
        }
        static Binding GetBinding()
        {
            BasicHttpBinding result = new BasicHttpBinding();
            return result;
        }
        class PORDCSOB : DataContractSerializerOperationBehavior
        {
            public PORDCSOB(OperationDescription od) : base(od) { }

            public override XmlObjectSerializer CreateSerializer(Type type, string name, string ns, IList<Type> knownTypes)
            {
                return new DataContractSerializer(
                    type,
                    name,
                    ns,
                    knownTypes,
                    this.MaxItemsInObjectGraph,
                    this.IgnoreExtensionDataObject,
                    true /*preserveObjectReferences*/,
                    this.DataContractSurrogate);
            }

            public override XmlObjectSerializer CreateSerializer(Type type, XmlDictionaryString name, XmlDictionaryString ns, IList<Type> knownTypes)
            {
                return new DataContractSerializer(
                    type,
                    name,
                    ns,
                    knownTypes,
                    this.MaxItemsInObjectGraph,
                    this.IgnoreExtensionDataObject,
                    true /*preserveObjectReferences*/,
                    this.DataContractSurrogate);
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), GetBinding(), "");
            ServiceEndpoint endpoint = null;
            foreach (ServiceEndpoint ep in host.Description.Endpoints)
            {
                if (ep.Contract.ContractType == typeof(ITest))
                {
                    endpoint = ep;
                    break;
                }
            }
            OperationDescription odServer = endpoint.Contract.Operations[0];
            odServer.Behaviors.Remove<DataContractSerializerOperationBehavior>();
            odServer.Behaviors.Add(new PORDCSOB(odServer));
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(GetBinding(), new EndpointAddress(baseAddress));
            OperationDescription od = factory.Endpoint.Contract.Operations[0];
            od.Behaviors.Remove<DataContractSerializerOperationBehavior>();
            od.Behaviors.Add(new PORDCSOB(od));
            ITest proxy = factory.CreateChannel();
            Console.WriteLine(proxy.EchoList(MyCircularList.Create()));

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    //Mex endpoint on named pipes
    public class Post3540307
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
        static Binding GetBinding()
        {
            return new NetNamedPipeBinding();
        }
        public static void Test()
        {
            string baseAddress = "net.pipe://" + Environment.MachineName + "/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), GetBinding(), "");
            ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
            host.Description.Behaviors.Add(smb);
            host.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexNamedPipeBinding(), "mex");
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

    public class Post3598667
    {
        [ServiceContract]
        public interface ITest
        {
            [WebGet, OperationContract]
            string GetData(int[] intArray, MyDC dc);
            [WebInvoke, OperationContract]
            string PostData(int[] intArray, MyDC dc);
        }
        public class Service : ITest
        {
            public string GetData(int[] intArray, MyDC dc)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("intArray=");
                if (intArray == null)
                {
                    sb.Append("<<null>>");
                }
                else
                {
                    sb.Append('[');
                    for (int i = 0; i < intArray.Length; i++)
                    {
                        if (i > 0) sb.Append(',');
                        sb.Append(intArray[i]);
                    }
                    sb.Append(']');
                }
                sb.Append(", dc=");
                if (dc == null)
                {
                    sb.Append("<<null>>");
                }
                else
                {
                    sb.Append(dc.ToString());
                }
                return sb.ToString();
            }
            public string PostData(int[] intArray, MyDC dc)
            {
                return GetData(intArray, dc);
            }
        }
        [DataContract]
        public class MyDC
        {
            [DataMember]
            public int i = 1234;
            [DataMember]
            public string str = "Hello world";
            public override string ToString()
            {
                return string.Format("MyDC[i={0},str={1}]", this.i, this.str);
            }
        }
        static string ToJson(object obj)
        {
            if (obj == null) return "null";
            DataContractJsonSerializer dcjs = new DataContractJsonSerializer(obj.GetType());
            MemoryStream ms = new MemoryStream();
            dcjs.WriteObject(ms, obj);
            return Encoding.UTF8.GetString(ms.ToArray());
        }
        public static void Test()
        {
            string baseAddress = "http://localhost:8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            ServiceEndpoint endpoint = host.AddServiceEndpoint(typeof(ITest), new WebHttpBinding(), "");
            endpoint.Behaviors.Add(new WebScriptEnablingBehavior());
            host.Open();
            Console.WriteLine("Host opened");

            int[] intArray = { 1, 2, 3, 5, -6, 0 };
            MyDC dc = new MyDC
            {
                i = int.MaxValue,
                str = "An instance of MyDC",
            };
            StringBuilder uri = new StringBuilder();
            uri.Append(baseAddress);
            uri.Append("/GetData");
            Dictionary<string, object> parameters = new Dictionary<string, object> {
				{ "intArray", intArray },
				{ "dc", dc },
			};
            bool first = true;
            foreach (string paramName in parameters.Keys)
            {
                if (parameters[paramName] != null)
                {
                    if (first)
                    {
                        uri.Append("?");
                        first = false;
                    }
                    else
                    {
                        uri.Append("&");
                    }
                    uri.Append(paramName);
                    uri.Append('=');
                    uri.Append(Uri.EscapeDataString(ToJson(parameters[paramName])));
                }
            }
            Console.WriteLine("Request URI: {0}", uri);
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(uri.ToString());
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
            if (resp.ContentLength > 0)
            {
                Console.WriteLine(new StreamReader(resp.GetResponseStream()).ReadToEnd());
            }
            Console.WriteLine();

            req = (HttpWebRequest)HttpWebRequest.Create(baseAddress + "/PostData");
            req.Method = "POST";
            req.ContentType = "application/json; charset=utf-8";
            string reqBodyString = String.Format("{{\"intArray\":{0},\"dc\":{1}}}",
                ToJson(intArray), ToJson(dc));
            byte[] reqBodyBytes = Encoding.UTF8.GetBytes(reqBodyString);
            req.GetRequestStream().Write(reqBodyBytes, 0, reqBodyBytes.Length);
            req.GetRequestStream().Close();
            Console.WriteLine("Request URI: {0}/PostData", baseAddress);
            Console.WriteLine("Request body: {0}", reqBodyString);
            try
            {
                resp = (HttpWebResponse)req.GetResponse();
            }
            catch (WebException e)
            {
                resp = (HttpWebResponse)e.Response;
            }
            Console.WriteLine("HTTP/{0} {1} {2}", resp.ProtocolVersion, (int)resp.StatusCode, resp.StatusDescription);
            if (resp.ContentLength > 0)
            {
                Console.WriteLine(new StreamReader(resp.GetResponseStream()).ReadToEnd());
            }
            Console.WriteLine();

            Console.Write("Press ENTER to close host");
            Console.ReadLine();
            ServiceEndpoint ep = new ServiceEndpoint(null);
            ep.Name = "hello";
            host.Close();
        }
    }

    //Content-Disposition header
    public class Post3374213
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationBehavior, WebGet]
            Stream GetData();
        }
        public class Service : ITest
        {
            public Stream GetData()
            {
                WebOperationContext.Current.OutgoingResponse.ContentType = "application/json";
                WebOperationContext.Current.OutgoingResponse.Headers.Add("Content-Disposition", "attachment; filename=MyFile.json");
                string jsonResponse = "{\"a\":123,\"b\":[false, true, false],\"c\":{\"foo\":\"bar\"}}";
                MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonResponse));
                return ms;
            }
        }
    }

    public class MyCode_1
    {
        public const string HeaderNamespace = "http://WCFService_TestForInheritance.ServiceContracts/2007/01";
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
        public class MyInspector : IDispatchMessageInspector, IEndpointBehavior
        {

            #region IDispatchMessageInspector Members

            public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
            {
                string userName = request.Headers.GetHeader<string>("UserName", MyCode_1.HeaderNamespace);
                string password = request.Headers.GetHeader<string>("Password", MyCode_1.HeaderNamespace);
                Console.WriteLine("In the inspector, user/pwd={0}/{1}", userName, password);
                return null;
            }

            public void BeforeSendReply(ref Message reply, object correlationState)
            {
            }

            #endregion

            #region IEndpointBehavior Members

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

            #endregion
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), GetBinding(), "").Behaviors.Add(new MyInspector());
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(GetBinding(), new EndpointAddress(baseAddress));
            ITest proxy = factory.CreateChannel();
            using (OperationContextScope scope = new OperationContextScope((IContextChannel)proxy))
            {
                OperationContext.Current.OutgoingMessageHeaders.Add(MessageHeader.CreateHeader("UserName", MyCode_1.HeaderNamespace, "AValidUserName"));
                OperationContext.Current.OutgoingMessageHeaders.Add(MessageHeader.CreateHeader("Password", MyCode_1.HeaderNamespace, "AValidPassword"));
                Console.WriteLine(proxy.Echo("Hello"));
            }

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    //Two net.pipe services
    public class Post3824528
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
                return String.Format("Received {0} at {1}", text, OperationContext.Current.EndpointDispatcher.EndpointAddress.Uri);
            }
        }
        static Binding GetBinding()
        {
            return new NetNamedPipeBinding();
        }
        public static void Test()
        {
            string baseAddress1 = "net.pipe://localhost/Service1";
            string baseAddress2 = "net.pipe://localhost/Service2";
            ServiceHost host1 = new ServiceHost(typeof(Service), new Uri(baseAddress1));
            ServiceHost host2 = new ServiceHost(typeof(Service), new Uri(baseAddress2));
            host1.AddServiceEndpoint(typeof(ITest), GetBinding(), "endpoint1");
            host2.AddServiceEndpoint(typeof(ITest), GetBinding(), "endpoint2");
            host1.Open();
            host2.Open();
            Console.WriteLine("Hosts opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(GetBinding(), new EndpointAddress(baseAddress1 + "/endpoint1"));
            ITest proxy = factory.CreateChannel();
            Console.WriteLine(proxy.Echo("Hello"));
            ((IClientChannel)proxy).Close();
            factory.Close();

            factory = new ChannelFactory<ITest>(GetBinding(), new EndpointAddress(baseAddress2 + "/endpoint2"));
            proxy = factory.CreateChannel();
            Console.WriteLine(proxy.Echo("Hello"));
            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the hosts");
            Console.ReadLine();
            host1.Close();
            host2.Close();
        }
    }
    // Special JSON result
    public class Post825e1250_f80d_46e5_8406_f19c7597f95f
    {
        class JsonParenMessageEncoder : MessageEncoder
        {
            private MessageEncoder jsonEncoder;
            public JsonParenMessageEncoder(MessageEncoder jsonEncoder)
            {
                this.jsonEncoder = jsonEncoder;
            }
            public override string ContentType
            {
                get { return jsonEncoder.ContentType; }
            }

            public override string MediaType
            {
                get { return jsonEncoder.MediaType; }
            }

            public override MessageVersion MessageVersion
            {
                get { return jsonEncoder.MessageVersion; }
            }

            public override T GetProperty<T>()
            {
                return jsonEncoder.GetProperty<T>();
            }

            public override bool IsContentTypeSupported(string contentType)
            {
                return this.jsonEncoder.IsContentTypeSupported(contentType);
            }

            public override Message ReadMessage(ArraySegment<byte> buffer, BufferManager bufferManager, string contentType)
            {
                return this.jsonEncoder.ReadMessage(buffer, bufferManager, contentType);
            }

            public override Message ReadMessage(Stream stream, int maxSizeOfHeaders, string contentType)
            {
                throw new NotSupportedException();
            }

            public override ArraySegment<byte> WriteMessage(Message message, int maxMessageSize, BufferManager bufferManager, int messageOffset)
            {
                ArraySegment<byte> temp = this.jsonEncoder.WriteMessage(message, maxMessageSize, bufferManager, messageOffset);
                byte[] newBuffer = bufferManager.TakeBuffer(temp.Array.Length + 2);
                newBuffer[temp.Offset] = (byte)'(';
                Array.Copy(temp.Array, temp.Offset, newBuffer, temp.Offset + 1, temp.Count);
                newBuffer[temp.Offset + temp.Count + 1] = (byte)')';
                bufferManager.ReturnBuffer(temp.Array);
                return new ArraySegment<byte>(newBuffer, temp.Offset, temp.Count + 2);
            }

            public override void WriteMessage(Message message, Stream stream)
            {
                throw new NotSupportedException();
            }
        }
        class JsonParenMessageEncoderFactory : MessageEncoderFactory
        {
            public MessageEncoder encoder;
            public JsonParenMessageEncoderFactory()
            {
                this.encoder = new JsonParenMessageEncoder(new WebMessageEncodingBindingElement().CreateMessageEncoderFactory().Encoder);
            }
            public override MessageEncoder Encoder
            {
                get { return this.encoder; }
            }

            public override MessageVersion MessageVersion
            {
                get { return MessageVersion.None; }
            }
        }
        public class JsonParenMessageEncodingBindingElement : MessageEncodingBindingElement
        {
            WebMessageEncodingBindingElement webElement = new WebMessageEncodingBindingElement();
            public override MessageEncoderFactory CreateMessageEncoderFactory()
            {
                return new JsonParenMessageEncoderFactory();
            }

            public override MessageVersion MessageVersion
            {
                get
                {
                    return MessageVersion.None;
                }
                set
                {
                    if (value != MessageVersion.None)
                    {
                        throw new ArgumentException("Unsupported message version: " + value);
                    }
                }
            }

            public override BindingElement Clone()
            {
                return new JsonParenMessageEncodingBindingElement();
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
        }

        [DataContract]
        public class MyDC
        {
            [DataMember]
            public string str = "Hello";
            [DataMember]
            public int i = 123;
            [DataMember]
            public bool[] boolArray = new bool[] { false, true, false };
        }
        [ServiceContract]
        public interface ITest
        {
            [WebGet(ResponseFormat = WebMessageFormat.Json), OperationContract]
            MyDC GetDC();
        }
        public class Service : ITest
        {
            public MyDC GetDC()
            {
                return new MyDC();
            }
        }
        static Binding GetBinding()
        {
            CustomBinding result = new CustomBinding(new WebHttpBinding());
            for (int i = 0; i < result.Elements.Count; i++)
            {
                if (result.Elements[i] is MessageEncodingBindingElement)
                {
                    result.Elements[i] = new JsonParenMessageEncodingBindingElement();
                    break;
                }
            }
            return result;
        }
        public static void Test()
        {
            string baseAddress = "http://localhost:8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), GetBinding(), "").Behaviors.Add(new WebHttpBehavior());
            host.Open();

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(baseAddress + "/GetDC");
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
            foreach (string key in resp.Headers.AllKeys)
            {
                string headerValue = resp.Headers[key];
                Console.WriteLine("{0}: {1}", key, headerValue);
            }
            Console.WriteLine();
            if (resp.ContentLength > 0)
            {
                Console.WriteLine(new StreamReader(resp.GetResponseStream()).ReadToEnd());
            }
        }
    }

    public class CustomBindingWithoutTransport
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
        static Binding GetBinding()
        {
            CustomBinding result = new CustomBinding(new TcpTransportBindingElement());
            return result;
        }
        public static void Test()
        {
            string baseAddressTcp = "net.tcp://" + Environment.MachineName + ":8008/Service";
            string baseAddressHttp = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddressHttp), new Uri(baseAddressTcp));
            ServiceEndpoint dcsEndpoint = host.AddServiceEndpoint(typeof(ITest), GetBinding(), "");
            ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
            smb.HttpGetEnabled = true;
            host.Description.Behaviors.Add(smb);
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(GetBinding(), new EndpointAddress(baseAddressTcp));
            ITest proxy = factory.CreateChannel();
            Console.WriteLine(proxy.Echo("Hello world"));

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post49b505b2_f596_48e9_a206_a6f96c31ce9c
    {
        [DataContract]
        public class ComplexNumber
        {
            [DataMember]
            public double real;
            [DataMember]
            public double imaginary;

            public override string ToString()
            {
                return String.Format("Complex[{0} {1} {2}i]", real, this.imaginary < 0 ? "-" : "+", this.imaginary);
            }
        }
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            [MyOperationBehavior]
            string Echo(string text);
            [OperationContract]
            int Add(int x, int y);
            [OperationContract]
            //[MyOperationBehavior]
            ComplexNumber Multiply(ComplexNumber n1, ComplexNumber n2);
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
            public ComplexNumber Multiply(ComplexNumber n1, ComplexNumber n2)
            {
                return new ComplexNumber
                {
                    real = n1.real * n2.real - n1.imaginary * n2.imaginary,
                    imaginary = n1.real * n2.imaginary + n2.real + n1.imaginary,
                };
            }
        }
        static Binding GetBinding()
        {
            BasicHttpBinding result = new BasicHttpBinding();
            //Change binding settings here
            return result;
        }
        public class MyClientMessageFormatter : IClientMessageFormatter
        {
            IClientMessageFormatter originalFormatter;
            public MyClientMessageFormatter(IClientMessageFormatter originalFormatter)
            {
                this.originalFormatter = originalFormatter;
            }

            public object DeserializeReply(Message message, object[] parameters)
            {
                Console.WriteLine("Deserializing reply with {0} parameters", parameters.Length);
                return this.originalFormatter.DeserializeReply(message, parameters);
            }

            public Message SerializeRequest(MessageVersion messageVersion, object[] parameters)
            {
                Console.WriteLine("Serializing request with {0} parameters", parameters.Length);
                Message result = Message.CreateMessage(messageVersion, "http://tempuri.org/ITest/Echo");
                result.Headers.To = new Uri("http://" + Environment.MachineName + ":8000/Service");
                return result; // this.originalFormatter.SerializeRequest(messageVersion, parameters);
            }
        }
        [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
        public class MyOperationBehaviorAttribute : Attribute, IOperationBehavior
        {
            public void AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters)
            {
            }
            public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
            {
                clientOperation.SerializeRequest = true;
                clientOperation.Formatter = new MyClientMessageFormatter(clientOperation.Formatter);
            }
            public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
            {
                Console.WriteLine("ApplyDispatchBehavior - Formatter: {0}", dispatchOperation.Formatter);
            }
            public void Validate(OperationDescription operationDescription)
            {
                Console.WriteLine("Validate - Behaviors.Length: {0}", operationDescription.Behaviors.Count);
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
            Console.WriteLine(proxy.Echo("Hello"));
            Console.WriteLine(proxy.Add(3, 4));
            ComplexNumber n1 = new ComplexNumber { real = 1, imaginary = 1 };
            ComplexNumber n2 = new ComplexNumber { real = 1, imaginary = -1 };
            Console.WriteLine(proxy.Multiply(n1, n2));

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Postaacbacd9_ed68_468d_90a0_a95129fe962a
    {
        [DataContract]
        public class MTOMQuotaTest
        {
            private byte[] _MyByteArr1;
            private byte[] _MyByteArr2;

            public MTOMQuotaTest()
            {

            }

            [DataMember]
            public byte[] MyByteArr1
            {
                get { return this._MyByteArr1; }
                set { this._MyByteArr1 = value; }
            }

            [DataMember]
            public byte[] MyByteArr2
            {
                get { return this._MyByteArr2; }
                set { this._MyByteArr2 = value; }
            }

            public override string ToString()
            {
                return String.Format("MTOMQuotaTest[MyByteArr1.Len={0}, MyByteArr2.Len={1}]", MyByteArr1.Length, MyByteArr2.Length);
            }
        }
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            MTOMQuotaTest[] TestMTOM(int size);
        }
        public class Service : ITest
        {
            Random rndGen = new Random(1);
            public MTOMQuotaTest[] TestMTOM(int size)
            {
                MTOMQuotaTest[] testArr = new MTOMQuotaTest[size];

                for (int x = 0; x < size; x++)
                {
                    testArr[x] = new MTOMQuotaTest();
                    testArr[x].MyByteArr1 = new byte[2500000];
                    rndGen.NextBytes(testArr[x].MyByteArr1);
                    testArr[x].MyByteArr2 = new byte[2500000];
                    rndGen.NextBytes(testArr[x].MyByteArr2);
                }

                return testArr;
            }
        }
        static Binding GetBinding()
        {
            BasicHttpBinding result = new BasicHttpBinding();
            result.MaxReceivedMessageSize = int.MaxValue;
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
            Console.WriteLine(proxy.TestMTOM(20));

            try
            {
                Console.WriteLine(proxy.TestMTOM(40));
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

    public class Poste0187dcd_553b_49a9_9214_24df1ef1cacd
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract(Action = "align")]
            string align(string text);
        }
        public class Service : ITest
        {
            public string align(string text)
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
            Console.WriteLine(proxy.align("Hello"));

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post4487a977_4a54_49fe_b85d_1f901b87bd5a
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            [WebGet]
            Stream RegisterJS();
        }
        public class Service : ITest
        {
            public Stream RegisterJS()
            {
                WebOperationContext.Current.OutgoingResponse.ContentType = "application/javascript";
                StringBuilder sb = new StringBuilder();
                sb.Append(@"alert('This is a JS returned from the server');");
                return new MemoryStream(Encoding.UTF8.GetBytes(sb.ToString()));
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/services";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), new WebHttpBinding(), "").Behaviors.Add(new WebHttpBehavior());
            host.Open();
            Console.WriteLine("Host opened");

            WebClient client = new WebClient();
            string result = client.DownloadString(baseAddress + "/RegisterJS");
            Console.WriteLine(result);

            Console.Write("Press ENTER to close service");
            Console.ReadLine();
            host.Close();
        }
    }

    //"Real" JSON dictionary
    public class Post8bef40bc_8466_4c6f_a717_15f3d6e61e3c
    {
        [Serializable]
        public class MyJsonDictionary : ISerializable
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            public MyJsonDictionary() { }
            protected MyJsonDictionary(SerializationInfo info, StreamingContext context)
            {
                foreach (var entry in info)
                {
                    dict.Add(entry.Name, entry.Value);
                }
            }

            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                foreach (string key in dict.Keys)
                {
                    info.AddValue(key, dict[key], dict[key] == null ? typeof(object) : dict[key].GetType());
                }
            }

            public void Add(string key, object value)
            {
                dict.Add(key, value);
            }
        }
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            [WebGet(ResponseFormat = WebMessageFormat.Json)]
            MyJsonDictionary Get();
        }
        public class Service : ITest
        {
            public MyJsonDictionary Get()
            {
                MyJsonDictionary result = new MyJsonDictionary();
                result.Add("foo", "bar");
                result.Add("Name", "John Doe");
                result.Add("Age", 32);
                MyJsonDictionary address = new MyJsonDictionary();
                result.Add("Address", address);
                address.Add("Street", "30 Rockefeller Plaza");
                address.Add("City", "New York City");
                address.Add("State", "NY");
                return result;
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            WebServiceHost host = new WebServiceHost(typeof(Service), new Uri(baseAddress));
            host.Open();
            Console.WriteLine("Host opened");

            WebClient client = new WebClient();
            string result = client.DownloadString(baseAddress + "/Get");
            Console.WriteLine(result);

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    //Updating MTOM requests
    public class IndiTalk_20090206
    {
        class MyMtomMessageEncoder : MessageEncoder
        {
            private MessageEncoder mtomEncoder;
            public MyMtomMessageEncoder(MessageEncoder mtomEncoder)
            {
                this.mtomEncoder = mtomEncoder;
            }
            public override string ContentType
            {
                get { return mtomEncoder.ContentType; }
            }

            public override string MediaType
            {
                get { return mtomEncoder.MediaType; }
            }

            public override MessageVersion MessageVersion
            {
                get { return mtomEncoder.MessageVersion; }
            }

            public override T GetProperty<T>()
            {
                return mtomEncoder.GetProperty<T>();
            }

            public override bool IsContentTypeSupported(string contentType)
            {
                return this.mtomEncoder.IsContentTypeSupported(contentType);
            }

            public override Message ReadMessage(ArraySegment<byte> buffer, BufferManager bufferManager, string contentType)
            {
                return this.mtomEncoder.ReadMessage(buffer, bufferManager, contentType);
            }

            public override Message ReadMessage(Stream stream, int maxSizeOfHeaders, string contentType)
            {
                throw new NotSupportedException();
            }

            void UnescapeXopIncludeHref(byte[] bytes, int offset, ref int count)
            {
                byte[] bytesToSearch = Encoding.UTF8.GetBytes("<xop:Include href=\"");
                int i = 0;
                while (i < count)
                {
                    bool found = true;
                    for (int j = 0; j < bytesToSearch.Length; j++)
                    {
                        if (bytes[offset + i + j] != bytesToSearch[j])
                        {
                            found = false;
                            break;
                        }
                    }
                    if (found)
                    {
                        int startHrefIndex = i + offset + bytesToSearch.Length;
                        int endQuoteIndex = i + offset + bytesToSearch.Length;
                        while (bytes[endQuoteIndex] != (byte)'\"')
                        {
                            endQuoteIndex++;
                        }
                        string href = Encoding.UTF8.GetString(bytes, startHrefIndex, endQuoteIndex - startHrefIndex - 1);
                        href = Uri.UnescapeDataString(href);
                        byte[] hrefBytes = Encoding.UTF8.GetBytes(href);
                        Array.Copy(hrefBytes, 0, bytes, startHrefIndex, hrefBytes.Length);
                        if (hrefBytes.Length != endQuoteIndex - startHrefIndex - 1)
                        {
                            int lastByte = offset + count;
                            for (int j = 0; j < lastByte - endQuoteIndex; j++)
                            {
                                bytes[startHrefIndex + hrefBytes.Length + j] = bytes[endQuoteIndex + j];
                            }
                            count -= (endQuoteIndex - startHrefIndex - 1) - hrefBytes.Length;
                        }
                        i = startHrefIndex + hrefBytes.Length + 1;
                    }
                    i++;
                }
            }
            public override ArraySegment<byte> WriteMessage(Message message, int maxMessageSize, BufferManager bufferManager, int messageOffset)
            {
                ArraySegment<byte> temp = this.mtomEncoder.WriteMessage(message, maxMessageSize, bufferManager, messageOffset);
                int count = temp.Count;
                UnescapeXopIncludeHref(temp.Array, temp.Offset, ref count);
                return new ArraySegment<byte>(temp.Array, temp.Offset, count);
            }

            public override void WriteMessage(Message message, Stream stream)
            {
                throw new NotSupportedException();
            }
        }
        class MyMtomMessageEncoderFactory : MessageEncoderFactory
        {
            MessageEncoder encoder;
            MessageEncoderFactory innerFactory;
            public MyMtomMessageEncoderFactory(MessageEncoderFactory innerFactory)
            {
                this.innerFactory = innerFactory;
                this.encoder = new MyMtomMessageEncoder(innerFactory.Encoder);
            }
            public override MessageEncoder Encoder
            {
                get { return this.encoder; }
            }

            public override MessageVersion MessageVersion
            {
                get { return this.innerFactory.MessageVersion; }
            }
        }
        public class MyMtomMessageEncodingBindingElement : MessageEncodingBindingElement
        {
            MtomMessageEncodingBindingElement mtomElement;
            public MyMtomMessageEncodingBindingElement(MtomMessageEncodingBindingElement innerBindingElement)
            {
                this.mtomElement = innerBindingElement;
            }
            public override MessageEncoderFactory CreateMessageEncoderFactory()
            {
                return new MyMtomMessageEncoderFactory(mtomElement.CreateMessageEncoderFactory());
            }

            public override MessageVersion MessageVersion
            {
                get
                {
                    return this.mtomElement.MessageVersion;
                }
                set
                {
                    this.mtomElement.MessageVersion = value;
                }
            }

            public override BindingElement Clone()
            {
                return new MyMtomMessageEncodingBindingElement(this.mtomElement);
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
        }

        [DataContract]
        public class MyDC
        {
            [DataMember]
            public string str = "Hello";
            [DataMember]
            public int i = 123;
            [DataMember]
            public byte[] byteArray = Encoding.UTF8.GetBytes(new string('r', 1000));
        }
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            MyDC EchoDC(MyDC dc);
        }
        public class Service : ITest
        {
            public MyDC EchoDC(MyDC dc)
            {
                return dc;
            }
        }
        static Binding GetBinding()
        {
            CustomBinding result = new CustomBinding(new BasicHttpBinding { MessageEncoding = WSMessageEncoding.Mtom });
            for (int i = 0; i < result.Elements.Count; i++)
            {
                if (result.Elements[i] is MtomMessageEncodingBindingElement)
                {
                    result.Elements[i] = new MyMtomMessageEncodingBindingElement((MtomMessageEncodingBindingElement)result.Elements[i]);
                    break;
                }
            }
            return result;
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), GetBinding(), "");
            host.Open();

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(GetBinding(), new EndpointAddress(baseAddress));
            ITest proxy = factory.CreateChannel();
            Console.WriteLine(proxy.EchoDC(new MyDC()));

            ((IClientChannel)proxy).Close();
            factory.Close();
            host.Close();
        }
    }

    public class Post_4074f4c5_16cc_470c_9546_a6fb79c998fc
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare)]
            void SaveString(string text);
        }
        public class Service : ITest
        {
            public void SaveString(string text)
            {
                Console.WriteLine("At server, text = {0}", text);
            }
        }
        static Binding GetBinding()
        {
            WebHttpBinding result = new WebHttpBinding();
            return result;
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            WebServiceHost host = new WebServiceHost(typeof(Service), new Uri(baseAddress));
            host.Open();
            Console.WriteLine("Host opened");

            WebClient client = new WebClient();
            client.Headers[HttpRequestHeader.ContentType] = "text/xml";
            client.UploadString(baseAddress + "/SaveString", "<string xmlns=\"http://schemas.microsoft.com/2003/10/Serialization/\">This is a string that will be sent to the service</string>");

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_4074f4c5_16cc_470c_9546_a6fb79c998fc_b
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            [WebInvoke]
            void SaveString(Stream stream);
        }
        public class Service : ITest
        {
            public void SaveString(Stream text)
            {
                string stringText = new StreamReader(text).ReadToEnd();
                Console.WriteLine("At server, text = {0}", stringText);
            }
        }
        static Binding GetBinding()
        {
            WebHttpBinding result = new WebHttpBinding();
            return result;
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            WebServiceHost host = new WebServiceHost(typeof(Service), new Uri(baseAddress));
            host.Open();
            Console.WriteLine("Host opened");

            WebClient client = new WebClient();
            client.Headers[HttpRequestHeader.ContentType] = "text/plain";
            client.UploadString(baseAddress + "/SaveString", "This is a string that will be sent to the service");

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_fd7f8df9_4a83_4285_be91_b9adb0c1888a
    {
        [DataContract(Name = "DummyContract")]
        public class DummyContract
        {
            [DataMember]
            public string firstName;

            [DataMember]
            public string secondName;
        }
        [MessageContract]
        public class CheckCustomerRequest
        {
            [MessageBodyMember]
            public DummyContract request;
        }
        [MessageContract]
        public class CheckCustomerResponse
        {
            [MessageBodyMember]
            public bool response;
        }
        [ServiceContract]
        public interface ICustomerService
        {
            [OperationContract(Action = "http://tempuri.org/ICustomerService/CheckCustomer")]
            CheckCustomerResponse CheckCustomer(CheckCustomerRequest request);
        }
        public class Service : ICustomerService
        {
            public bool CheckCustomer(DummyContract request)
            {
                return true;
            }

            public CheckCustomerResponse CheckCustomer(CheckCustomerRequest request)
            {
                return new CheckCustomerResponse { response = CheckCustomer(request.request) };
            }
        }
        static Binding GetBinding()
        {
            BasicHttpBinding result = new BasicHttpBinding();
            return result;
        }
        public class MyInspector : IClientMessageInspector, IEndpointBehavior
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
                TypedMessageConverter conv = TypedMessageConverter.Create(typeof(CheckCustomerRequest), "http://tempuri.org/ICustomerService/CheckCustomer");
                CheckCustomerRequest req = (CheckCustomerRequest)conv.FromMessage(request);
                Console.WriteLine("Request: {0} {1}", req.request.firstName, req.request.secondName);
                request = conv.ToMessage(req, GetBinding().MessageVersion);
                return null;
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ICustomerService), GetBinding(), "");
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ICustomerService> factory = new ChannelFactory<ICustomerService>(GetBinding(), new EndpointAddress(baseAddress));
            factory.Endpoint.Behaviors.Add(new MyInspector());
            ICustomerService proxy = factory.CreateChannel();
            Console.WriteLine(proxy.CheckCustomer(new CheckCustomerRequest
            {
                request = new DummyContract
                {
                    firstName = "John",
                    secondName = "Doe",
                }
            }));

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_7a2ca17e_4dba_41f5_8cef_2e788940dd7b
    {
        [DataContract]
        public class Communities
        {
            [DataMember]
            public string name;
        }
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            [WebGet(ResponseFormat = WebMessageFormat.Xml,
                    BodyStyle = WebMessageBodyStyle.Bare,
                    UriTemplate = "/communities/{accountName}")]
            Communities CommunitiesByMemberAsXml(string accountName);
        }
        public class Service : ITest
        {
            public Communities CommunitiesByMemberAsXml(string accountName)
            {
                return new Communities { name = accountName };
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
            ITest proxy = factory.CreateChannel();
            Console.WriteLine(proxy.CommunitiesByMemberAsXml("abc:def").name);

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_65778aea_f872_45a8_85c9_d2bbc2838baf
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            string Echo(string text);
            [OperationContract]
            [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
            string LongEcho(string text, int delay);
            [OperationContract]
            [WebGet]
            int Add(int x, int y);
        }
        public class Service : ITest
        {
            public string Echo(string text)
            {
                return text;
            }

            public string LongEcho(string text, int delay)
            {
                Thread.Sleep(delay);
                return text;
            }

            public int Add(int x, int y)
            {
                Thread.Sleep(500);
                return x + y;
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

            Dictionary<Guid, KeyValuePair<string, DateTime>> callTimes = new Dictionary<Guid, KeyValuePair<string, DateTime>>();
            static object syncRoot = new object();
            Guid StartCallingMethod(string action)
            {
                lock (syncRoot)
                {
                    Guid correlation = Guid.NewGuid();
                    callTimes.Add(correlation, new KeyValuePair<string, DateTime>(action, DateTime.Now));
                    return correlation;
                }
            }
            void EndCallingMethod(Guid correlation)
            {
                DateTime now = DateTime.Now;
                KeyValuePair<string, DateTime> beginData;
                lock (syncRoot)
                {
                    beginData = callTimes[correlation];
                    callTimes.Remove(correlation);
                }
                Console.WriteLine("Request to {0} took {1}", beginData.Key, now.Subtract(beginData.Value));
            }
            public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
            {
                return StartCallingMethod(WebOperationContext.Current.IncomingRequest.UriTemplateMatch.RequestUri.ToString());
            }

            public void BeforeSendReply(ref Message reply, object correlationState)
            {
                EndCallingMethod((Guid)correlationState);
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
            Console.WriteLine(proxy.LongEcho("World", 1234));
            Console.WriteLine(proxy.Add(3, 4));

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_3a1da912_90fe_4a4a_a66e_d61bac53e77d
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
            [WebGet]
            Person GetPerson(string name, int age);
        }
        public class Service : ITest
        {
            public Person GetPerson(string name, int age)
            {
                string acceptHeader = WebOperationContext.Current.IncomingRequest.Headers[HttpRequestHeader.Accept];
                if (acceptHeader.Contains("json"))
                {
                    WebBodyFormatMessageProperty prop = new WebBodyFormatMessageProperty(WebContentFormat.Json);
                    OperationContext.Current.OutgoingMessageProperties.Add(WebBodyFormatMessageProperty.Name, prop);
                }
                return new Person { Name = name, Age = age };
            }
        }
        public class MyWebHttpBehavior : WebHttpBehavior
        {
            protected override IDispatchMessageFormatter GetReplyDispatchFormatter(OperationDescription operationDescription, ServiceEndpoint endpoint)
            {
                IDispatchMessageFormatter jsonFormatter = null;
                IDispatchMessageFormatter xmlFormatter = null;
                WebGetAttribute wga = operationDescription.Behaviors.Find<WebGetAttribute>();
                WebInvokeAttribute wia = operationDescription.Behaviors.Find<WebInvokeAttribute>();
                WebMessageFormat originalResponseFormat;
                if (wga != null)
                {
                    originalResponseFormat = wga.ResponseFormat;
                    wga.ResponseFormat = WebMessageFormat.Json;
                    jsonFormatter = base.GetReplyDispatchFormatter(operationDescription, endpoint);
                    wga.ResponseFormat = WebMessageFormat.Xml;
                    xmlFormatter = base.GetReplyDispatchFormatter(operationDescription, endpoint);
                    wga.ResponseFormat = originalResponseFormat;
                }
                else if (wia != null)
                {
                    originalResponseFormat = wia.ResponseFormat;
                    wia.ResponseFormat = WebMessageFormat.Json;
                    jsonFormatter = base.GetReplyDispatchFormatter(operationDescription, endpoint);
                    wia.ResponseFormat = WebMessageFormat.Xml;
                    xmlFormatter = base.GetReplyDispatchFormatter(operationDescription, endpoint);
                    wia.ResponseFormat = originalResponseFormat;
                }
                else
                {
                    wia = new WebInvokeAttribute();
                    wia.BodyStyle = this.DefaultBodyStyle;
                    wia.RequestFormat = this.DefaultOutgoingRequestFormat;
                    wia.ResponseFormat = WebMessageFormat.Json;
                    operationDescription.Behaviors.Add(wia);
                    jsonFormatter = base.GetReplyDispatchFormatter(operationDescription, endpoint);
                    wia.ResponseFormat = WebMessageFormat.Xml;
                    xmlFormatter = base.GetReplyDispatchFormatter(operationDescription, endpoint);
                    operationDescription.Behaviors.Remove<WebInvokeAttribute>();
                }
                return new MyJsonOrXmlReplyDispatchFormatter(jsonFormatter, xmlFormatter);
            }

            class MyJsonOrXmlReplyDispatchFormatter : IDispatchMessageFormatter
            {
                IDispatchMessageFormatter jsonFormatter;
                IDispatchMessageFormatter xmlFormatter;
                public MyJsonOrXmlReplyDispatchFormatter(IDispatchMessageFormatter jsonFormatter, IDispatchMessageFormatter xmlFormatter)
                {
                    this.jsonFormatter = jsonFormatter;
                    this.xmlFormatter = xmlFormatter;
                }

                public void DeserializeRequest(Message message, object[] parameters)
                {
                    throw new NotSupportedException("This should be used for reply only");
                }

                public Message SerializeReply(MessageVersion messageVersion, object[] parameters, object result)
                {
                    IDispatchMessageFormatter formatterToUse = this.xmlFormatter; // default
                    if (OperationContext.Current.OutgoingMessageProperties.ContainsKey(WebBodyFormatMessageProperty.Name))
                    {
                        WebBodyFormatMessageProperty prop = OperationContext.Current.OutgoingMessageProperties[WebBodyFormatMessageProperty.Name] as WebBodyFormatMessageProperty;
                        if (prop != null && prop.Format == WebContentFormat.Json)
                        {
                            formatterToUse = this.jsonFormatter;
                        }
                    }
                    return formatterToUse.SerializeReply(messageVersion, parameters, result);
                }
            }
        }
        public static string SendRequest(string uri, string acceptHeader)
        {
            string responseBody = null;

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(uri);
            req.Method = "GET";
            if (!String.IsNullOrEmpty(acceptHeader))
            {
                req.Accept = acceptHeader;
            }

            HttpWebResponse resp;
            Exception ex = null;
            try
            {
                resp = (HttpWebResponse)req.GetResponse();
            }
            catch (WebException e)
            {
                resp = (HttpWebResponse)e.Response;
                ex = e;
            }
            if (resp == null)
            {
                Console.WriteLine("Error: {0}", ex);
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
            host.AddServiceEndpoint(typeof(ITest), new WebHttpBinding(), "").Behaviors.Add(new MyWebHttpBehavior());
            host.Open();
            Console.WriteLine("Host opened");

            SendRequest(baseAddress + "/GetPerson?name=John%20Doe&age=27", null);
            SendRequest(baseAddress + "/GetPerson?name=John%20Doe&age=27", "application/json");
            SendRequest(baseAddress + "/GetPerson?name=John%20Doe&age=27", "application/xml");

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_a9ba7a39_c71f_46bb_ac3d_b0042e4476fd
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            [WebInvoke(
                Method = "POST",
                BodyStyle = WebMessageBodyStyle.WrappedRequest,
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json,
                UriTemplate = "/GetMyResults")]
            string SaveMyResults(string sessiontoken, List<Dictionary<String, String>> MyResults);
        }
        public class Service : ITest
        {
            public string SaveMyResults(string sessiontoken, List<Dictionary<String, String>> MyResults)
            {
                return sessiontoken + MyResults.Count;
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            WebServiceHost host = new WebServiceHost(typeof(Service), new Uri(baseAddress));
            host.Open();
            Console.WriteLine("Host opened");

            string requestBody = @"{
  ""sessiontoken"":
    ""whatever"",
  ""MyResults"":
    [
      [{""Key"":""one"",""Value"":""1""},{""Key"":""two"",""Value"":""2""}],
      [{""Key"":""uno"",""Value"":""1""},{""Key"":""dos"",""Value"":""2""}],
      [{""Key"":""eins"",""Value"":""1""},{""Key"":""zwei"",""Value"":""2""}]
    ]
}";
            Util.SendRequest(baseAddress + "/GetMyResults", "POST", "application/json", requestBody);

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class MyCode_2
    {
        const string MyFieldsPropName = "FieldsToIncludeInResponse";
        [DataContract(Name = "Ts", Namespace = "")]
        public class MyType
        {
            [DataMember]
            public int A { get; set; }
            [DataMember]
            public int B { get; set; }
            [DataMember]
            public int C { get; set; }
            [DataMember]
            public int D { get; set; }
            [DataMember]
            public int E { get; set; }
            [DataMember]
            public int F { get; set; }

            public MyType()
            {
                A = 1; B = 2; C = 3; D = 4; E = 5; F = 6;
            }
        }
        [ServiceContract]
        public interface ITest
        {
            [OperationContract, WebGet]
            MyType T1();
            [OperationContract, WebGet]
            MyType T2();
            [OperationContract, WebGet]
            MyType T3();
        }
        public class Service : ITest
        {
            public MyType T1()
            {
                OperationContext.Current.OutgoingMessageProperties.Add(MyFieldsPropName, "A B C".Split());
                return new MyType();
            }

            public MyType T2()
            {
                OperationContext.Current.OutgoingMessageProperties.Add(MyFieldsPropName, "C D E".Split());
                return new MyType();
            }

            public MyType T3()
            {
                OperationContext.Current.OutgoingMessageProperties.Add(MyFieldsPropName, "A C E".Split());
                return new MyType();
            }
        }
        public class MyInspector : IDispatchMessageInspector, IEndpointBehavior
        {
            public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
            {
                return null;
            }

            public void BeforeSendReply(ref Message reply, object correlationState)
            {
                object prop;
                if (reply.Properties.TryGetValue(MyFieldsPropName, out prop))
                {
                    string[] fields = (string[])prop;
                    XElement body = XElement.Load(reply.GetReaderAtBodyContents());
                    //Console.WriteLine(body);
                    List<XElement> toRemove = new List<XElement>();
                    foreach (var child in body.Elements())
                    {
                        XElement elementChild = child as XElement;
                        if (elementChild != null)
                        {
                            if (fields.Contains(elementChild.Name.ToString()))
                            {
                                //Console.WriteLine("Keeping element " + elementChild.Name);
                            }
                            else
                            {
                                toRemove.Add(elementChild);
                            }
                        }
                    }
                    foreach (var child in toRemove)
                    {
                        child.Remove();
                    }

                    string newBody = body.ToString();
                    Message newReply = Message.CreateMessage(reply.Version, null, XmlReader.Create(new StringReader(newBody)));
                    newReply.Headers.CopyHeadersFrom(reply);
                    reply = newReply;
                }
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
        }
        static void SendRequest(string uri)
        {
            WebClient client = new WebClient();
            string response = client.DownloadString(uri);
            Console.WriteLine("Request to {0}:", uri);
            Console.WriteLine(response);
            Console.WriteLine();
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

            SendRequest(baseAddress + "/T1");
            SendRequest(baseAddress + "/T2");
            SendRequest(baseAddress + "/T3");
        }
    }

    public class Post_1d9d73d0_240c_46a8_853f_d7c3dfc85c6c
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            string Process(Stream input);
        }
        public class Service : ITest
        {
            public string Process(Stream input)
            {
                return new StreamReader(input).ReadToEnd();
            }
        }
        public class MyInspector : IDispatchMessageInspector, IEndpointBehavior
        {
            #region IDispatchMessageInspector Members

            public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
            {
                XmlDictionaryReader bodyReader = request.GetReaderAtBodyContents();
                byte[] input = bodyReader.ReadElementContentAsBase64();
                Console.WriteLine("Incoming message: {0}", Encoding.UTF8.GetString(input));

                //Now recreating the message
                MemoryStream ms = new MemoryStream();
                XmlDictionaryWriter w = XmlDictionaryWriter.CreateTextWriter(ms);
                w.WriteStartElement("Binary");
                w.WriteBase64(input, 0, input.Length);
                w.WriteEndElement();
                w.Flush();

                // need to recreate the message, since it has already been read
                ms.Position = 0;
                Message newRequest = Message.CreateMessage(XmlReader.Create(ms), int.MaxValue, request.Version);
                newRequest.Properties.CopyProperties(request.Properties);

                request = newRequest;

                return null;
            }

            public void BeforeSendReply(ref Message reply, object correlationState)
            {
            }

            #endregion

            #region IEndpointBehavior Members

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

            #endregion
        }
        static Binding GetBinding()
        {
            CustomBinding cb = new CustomBinding(new WebHttpBinding());
            cb.Elements.Find<WebMessageEncodingBindingElement>().ContentTypeMapper = new MyRawMapper();
            return cb;
        }
        class MyRawMapper : WebContentTypeMapper
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
            ServiceEndpoint endpoint = host.AddServiceEndpoint(typeof(ITest), GetBinding(), "");
            endpoint.Behaviors.Add(new WebHttpBehavior());
            endpoint.Behaviors.Add(new MyInspector());
            host.Open();
            Console.WriteLine("Host opened");

            string body = "<Test><Parameters>" +
                "<Parameter><Name>FirstName</Name><DataType>string</DataType><Value>Maulik</Value></Parameter>" +
                "<Parameter><Name>LastName</Name><DataType>string</DataType><Value>Patel</Value></Parameter>" +
                "</Parameters></Test>";
            Util.SendRequest(baseAddress + "/Process", "POST", "text/xml", body);

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_dfefacb1_ebb1_4022_8064_11dceb7c5f3d
    {
        [ServiceContract]
        public interface IMyService
        {
            [OperationContract, WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped)]
            int InsertMessage(string message);
        }
        public class Service : IMyService
        {
            public int InsertMessage(string message)
            {
                return message.Length;
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            WebHttpBinding serverBinding = new WebHttpBinding();
            serverBinding.ReaderQuotas.MaxStringContentLength = 1000000;
            serverBinding.MaxReceivedMessageSize = int.MaxValue;
            host.AddServiceEndpoint(typeof(IMyService), serverBinding, "").Behaviors.Add(new WebHttpBehavior());
            host.Open();
            Console.WriteLine("Host opened");

            string message = "Long Message" + (new string('r', 98765));

            int resultID = 0;
            WebHttpBinding binding = new WebHttpBinding();
            using (ChannelFactory<IMyService> cf = new ChannelFactory<IMyService>(binding, baseAddress))
            {
                WebHttpBehavior behave = new WebHttpBehavior();
                cf.Endpoint.Behaviors.Add(behave);
                IMyService channel = cf.CreateChannel();

                resultID = channel.InsertMessage(message);
                Console.WriteLine(resultID);
                cf.Close();
            }

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
                return type == typeof(int[]) || inner.CanConvert(type);
            }
            public override object ConvertStringToValue(string parameter, Type parameterType)
            {
                if (parameterType == typeof(int[]))
                {
                    Console.WriteLine("parameter length: {0}", parameter.Length);
                    string[] parts = parameter.Split(',');
                    int[] result = new int[parts.Length];
                    for (int i = 0; i < parts.Length; i++) result[i] = int.Parse(parts[i]);
                    return result;
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
            StringBuilder sb = new StringBuilder();
            sb.Append(baseAddress);
            sb.Append("/Sum?ids=1");
            for (int i = 2; i <= 3486; i++)
            {
                sb.Append(',');
                sb.Append(i);
            }
            Console.WriteLine("URI length: {0}", sb.Length);
            Util.SendRequest(sb.ToString(), "GET", null, null);
            Util.SendRequest(baseAddress + "/invoice/123,345,121", "GET", null, null);

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_0ee2b5cd_cfec_4459_9de4_789023266730
    {
        [ServiceContract]
        public interface INewInterface
        {
            int Add(int x, int y);
        }
        public class MyNewInterfaceAdder : IServiceBehavior
        {
            public class MyAdder : INewInterface
            {
                public int Add(int x, int y)
                {
                    return x + y;
                }
            }
            public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
            {
            }

            public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
            {
                ContractDescription cd = ContractDescription.GetContract(typeof(INewInterface));
                OperationDescription od = new OperationDescription("Add", cd);
                MessageDescription addInput = new MessageDescription(cd.Namespace + cd.Name + "/Add", MessageDirection.Input);
                MessageDescription addOutput = new MessageDescription(cd.Namespace + cd.Name + "/AddResponse", MessageDirection.Output);
                MessagePartDescription addReturn = new MessagePartDescription("AddResult", cd.Namespace);
                addReturn.Type = typeof(int);
                addOutput.Body.ReturnValue = addReturn;
                od.Messages.Add(addInput);
                od.Messages.Add(addOutput);
                od.Behaviors.Add(new DataContractSerializerOperationBehavior(od));
                cd.Operations.Add(od);

                Uri address = new Uri(serviceHostBase.BaseAddresses[0].AbsoluteUri + "./Adder");
                EndpointDispatcher endpointDispatcher = new EndpointDispatcher(new EndpointAddress(address), cd.Name, cd.Namespace, true);
                endpointDispatcher.DispatchRuntime.Type = typeof(MyAdder);
                DispatchOperation operation = new DispatchOperation(endpointDispatcher.DispatchRuntime, od.Name, od.Messages[0].Action, od.Messages[1].Action);
                endpointDispatcher.DispatchRuntime.Operations.Add(operation);
                operation.Invoker = new AddInvoker();
                BasicHttpBinding binding = new BasicHttpBinding();
                BindingParameterCollection parameters = new BindingParameterCollection();
                IChannelListener<IReplyChannel> listener = null;
                if (binding.CanBuildChannelListener<IReplyChannel>(parameters))
                {
                    listener = binding.BuildChannelListener<IReplyChannel>(address, parameters);
                }
                ChannelDispatcher channelDispatcher = new ChannelDispatcher(listener);
                channelDispatcher.MessageVersion = binding.MessageVersion;
                channelDispatcher.Endpoints.Add(endpointDispatcher);
                serviceHostBase.ChannelDispatchers.Add(channelDispatcher);
            }

            class AddInvoker : IOperationInvoker
            {
                public object[] AllocateInputs()
                {
                    return new object[2] { 0, 0 };
                }

                public object Invoke(object instance, object[] inputs, out object[] outputs)
                {
                    outputs = new object[0];
                    return (int)inputs[0] + (int)inputs[1];
                }

                public IAsyncResult InvokeBegin(object instance, object[] inputs, AsyncCallback callback, object state)
                {
                    throw new NotImplementedException();
                }

                public object InvokeEnd(object instance, out object[] outputs, IAsyncResult result)
                {
                    throw new NotImplementedException();
                }

                public bool IsSynchronous
                {
                    get { return true; }
                }
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
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), GetBinding(), "");
            host.Description.Behaviors.Add(new MyNewInterfaceAdder());
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(GetBinding(), new EndpointAddress(baseAddress));
            ITest proxy = factory.CreateChannel();
            Console.WriteLine(proxy.Echo("Hello"));

            ChannelFactory<INewInterface> factory2 = new ChannelFactory<INewInterface>(new BasicHttpBinding(), new EndpointAddress(baseAddress + "/Adder"));
            INewInterface proxy2 = factory2.CreateChannel();
            Console.WriteLine(proxy2.Add(222, 444));

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_f6284415_e9ec_454d_b075_8c4024999b0e
    {
        [DataContract]
        [KnownType(typeof(Y))]
        public class X { }
        [DataContract]
        public class Y : X { }

        [ServiceContract]
        public interface ITest
        {
            [OperationContract, WebGet]
            X GetX();
        }
        public class Service : ITest
        {
            public X GetX()
            {
                return new Y();
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), new BasicHttpBinding(), "basic");
            host.AddServiceEndpoint(typeof(ITest), new WebHttpBinding(), "web").Behaviors.Add(new WebHttpBehavior());
            host.Open();
            Console.WriteLine("Host opened");

            string basicRequest = "<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\"><s:Header/><s:Body><GetX xmlns=\"http://tempuri.org/\"/></s:Body></s:Envelope>";
            Util.SendRequest(baseAddress + "/basic", "POST", "text/xml", basicRequest,
                new Dictionary<string, string> { { "SOAPAction", "http://tempuri.org/ITest/GetX" } });
            Util.SendRequest(baseAddress + "/web/GetX", "GET", null, null);

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_2e49f1b1_f08c_43bd_ad9b_6e9e65bda551
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
        static Binding GetBinding()
        {
            WSHttpBinding result = new WSHttpBinding("SimpleWS");
            return result;
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), GetBinding(), "");
            host.Description.Behaviors.Add(new ServiceMetadataBehavior { HttpGetEnabled = true });
            host.Open();
            Console.WriteLine("Host opened; listening at:");
            foreach (ChannelDispatcher cd in host.ChannelDispatchers)
            {
                foreach (EndpointDispatcher ed in cd.Endpoints)
                {
                    Console.WriteLine("  {0}", ed.EndpointAddress.Uri);
                }
            }

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

    public class Post_23c97dc8_ad9e_4795_a6c3_fac02d46da39
    {
        [ServiceContract(Namespace = "Services.EM", Name = "AlertService")]
        public interface IAlertService
        {
            [OperationContract]
            [WebInvoke(Method = "POST", UriTemplate = "tryMe/{myId}")]//, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
            void TryMe(string myId, XElement myConnection);
        }
        public class Service : IAlertService
        {
            public void TryMe(string myId, XElement myConnection)
            {
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(IAlertService), new WebHttpBinding(), "").Behaviors.Add(new WebHttpBehavior());
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<IAlertService> factory = new ChannelFactory<IAlertService>(new WebHttpBinding(), new EndpointAddress(baseAddress));
            factory.Endpoint.Behaviors.Add(new WebHttpBehavior());
            IAlertService proxy = factory.CreateChannel();

            XElement connectionParam = new XElement("myConnection");
            connectionParam.Add(new XElement("SomeParam"));
            XElement package = new XElement("AddComplex");
            package.Add(connectionParam);

            Guid myId = Guid.NewGuid();
            string idStr = myId.ToString();

            proxy.TryMe(idStr, package);

            ((IClientChannel)proxy).Close();
            factory.Close();

            WebClient client = new WebClient();
            client.Headers[HttpRequestHeader.ContentType] = "application/xml";
            Uri postUri = new Uri(String.Format(baseAddress + "/tryMe/{0}", myId));
            client.UploadString(postUri, package.ToString());

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_e55386d2_2204_4013_8385_b790d7340452
    {
        [XmlRoot(ElementName = "Root", Namespace = "")]
        [DataContract(Namespace = "", Name = "Root")]
        public class Root
        {
            [XmlElement(Namespace = "")]
            [DataMember]
            public string oldProp1 = "old1";
            [XmlElement(Namespace = "")]
            [DataMember]
            public string oldProp2 = "old2";
            [XmlElement(Namespace = "")]
            [DataMember]
            public string oldProp3 = "old3";
            [XmlElement(Namespace = "")]
            [DataMember]
            public string newProp1 = "new1";
            [XmlElement(Namespace = "")]
            [DataMember]
            public string newProp2 = "new2";

            public override string ToString()
            {
                return String.Format("Root[old1={0},old2={1},old3={2},new1={3},new2={4}]",
                    oldProp1, oldProp2, oldProp3, newProp1, newProp2);
            }
        }

        static string Serialize(bool useDataContractSerializer, Root obj)
        {
            MemoryStream ms = new MemoryStream();
            XmlWriterSettings ws = new XmlWriterSettings
            {
                Indent = false,
                OmitXmlDeclaration = true,
                Encoding = Encoding.UTF8
            };
            XmlWriter w = XmlWriter.Create(ms, ws);
            if (useDataContractSerializer)
            {
                new DataContractSerializer(typeof(Root)).WriteObject(w, obj);
            }
            else
            {
                new XmlSerializer(typeof(Root)).Serialize(w, obj);
            }
            w.Flush();
            return Encoding.UTF8.GetString(ms.ToArray());
        }

        static Root Deserialize(bool useDataContractSerializer, string serialized)
        {
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(serialized));
            if (useDataContractSerializer)
            {
                return (Root)(new DataContractSerializer(typeof(Root)).ReadObject(ms));
            }
            else
            {
                return (Root)(new XmlSerializer(typeof(Root)).Deserialize(ms));
            }
        }

        static void Test(bool useDataContractSerializer)
        {
            Console.WriteLine("Testing with {0}", useDataContractSerializer ? "DCS" : "XS");
            Root root = new Root();
            Console.WriteLine("Original object: {0}", root);
            string serialized = Serialize(useDataContractSerializer, new Root());
            Console.WriteLine("Serialized: {0}", serialized);
            Root deserialized = Deserialize(useDataContractSerializer, serialized);
            Console.WriteLine("New object: {0}", deserialized);
            Console.WriteLine();
            Console.WriteLine("Deserializing out of order:");
            serialized = @"<Root>
  <oldProp3>old3</oldProp3>
  <newProp1>new1</newProp1>
  <oldProp1>old1</oldProp1>
  <newProp2>new2</newProp2>
  <oldProp2>old2</oldProp2>
</Root>";
            deserialized = Deserialize(useDataContractSerializer, serialized);
            Console.WriteLine("New object (2): {0}", deserialized); Console.WriteLine();
            Console.WriteLine("=-=-=-=-=-=-=-=--=-=-=-=-=-=-=-=-=");
            Console.WriteLine();
        }

        public static void Test()
        {
            Test(true);
            Test(false);
        }
    }

    public class Post_764b32ef_2394_4cdb_90b6_46bd209b7cb3
    {
        public static void Test()
        {
            var config = System.Configuration.ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);
            var configGroup = ServiceModelSectionGroup.GetSectionGroup(config);
            var webHttpBindingCollection = configGroup.Bindings["webHttpBinding"];
            Console.WriteLine(webHttpBindingCollection);
            var newBinding = new WebHttpBindingElement("foo");
        }
    }

    public class Post_7c30db85_3867_4f01_9c0b_4a07015cb519
    {
        const string EntireMessage_xml = @"<s:Envelope xmlns:a=""http://www.w3.org/2005/08/addressing"" 
            xmlns:s=""http://www.w3.org/2003/05/soap-envelope"">
  <s:Header>
    <a:Action s:mustUnderstand=""1"">urn:SomeAction</a:Action>
  </s:Header>
  <s:Body>
    <string xmlns=""http://schemas.microsoft.com/2003/10/Serialization/"">
      Hello Message
    </string>
  </s:Body>
</s:Envelope>";
        const string BodyContent_xml = @"<string xmlns=""http://schemas.microsoft.com/2003/10/Serialization/"">
  Hello Message
</string>";
        const int MAXHEADERSIZE = int.MaxValue;

        public static void Test()
        {
            // ENVELOPE READER EXAMPLE
            // Get data from the file that contains the entire message.
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(EntireMessage_xml));
            XmlDictionaryReader envelopeReader = XmlDictionaryReader.CreateTextReader(stream, new XmlDictionaryReaderQuotas());
            Message msg = Message.CreateMessage(envelopeReader, MAXHEADERSIZE, MessageVersion.Soap12WSAddressing10);
            PrintMessage(ref msg);

            //BODY READER EXAMPLE
            // Get data from a file that contains just the body
            stream = new MemoryStream(Encoding.UTF8.GetBytes(BodyContent_xml));
            XmlDictionaryReader bodyReader = XmlDictionaryReader.CreateTextReader(stream, new XmlDictionaryReaderQuotas());
            msg = Message.CreateMessage(MessageVersion.Soap12WSAddressing10, "urn:SomeAction", bodyReader);
            PrintMessage(ref msg);
        }

        static void PrintMessage(ref Message msg)
        {
            Console.WriteLine("{0}\n", msg.ToString());
            MessageBuffer mb = msg.CreateBufferedCopy(int.MaxValue);
            msg = mb.CreateMessage();

            MemoryStream ms = new MemoryStream();
            XmlWriterSettings ws = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  ",
                OmitXmlDeclaration = true,
                Encoding = Encoding.UTF8,
            };
            XmlWriter w = XmlWriter.Create(ms, ws);
            Message toPrint = mb.CreateMessage();
            toPrint.WriteMessage(w);
            w.Flush();
            Console.WriteLine(Encoding.UTF8.GetString(ms.ToArray()));
            Console.WriteLine();
        }
    }

    public class Post_8b6a4d60_b286_41fc_997b_9fde81d0f14a
    {
        public static void Test()
        {
            List<SyndicationItem> items = new List<SyndicationItem>();
            items.Add(new SyndicationItem("Item 1", "Item 1 text content", null));
            items.Add(new SyndicationItem
            {
                Title = SyndicationContent.CreatePlaintextContent("Item 2"),
                Content = SyndicationContent.CreateUrlContent(new Uri("http://contoso.com"), "application/json"),
            });
            items.Add(new SyndicationItem
            {
                Title = SyndicationContent.CreatePlaintextContent("Item 3"),
                Content = SyndicationContent.CreateHtmlContent("<html><head><title>Test</title></head><body><h1>Hello world</h1></body></html>"),
            });
            items.Add(new SyndicationItem
            {
                Title = SyndicationContent.CreatePlaintextContent("Item 4"),
                Content = SyndicationContent.CreateXmlContent(1234),
            });
            SyndicationFeed feed = new SyndicationFeed("Feed title", "Feed description", null, "FeedId", DateTimeOffset.Now, items);
            SyndicationFeedFormatter formatter = new Rss20FeedFormatter(feed);
            MemoryStream ms = new MemoryStream();
            XmlWriterSettings ws = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  ",
                OmitXmlDeclaration = true,
                Encoding = new UTF8Encoding(false),
            };
            XmlWriter w = XmlWriter.Create(ms, ws);
            formatter.WriteTo(w);
            w.Flush();
            //Console.WriteLine(Encoding.UTF8.GetString(ms.ToArray()));
            ms.Position = 0;
            XmlReader r = XmlReader.Create(ms);
            formatter = new Rss20FeedFormatter();
            formatter.ReadFrom(r);
            feed = formatter.Feed;
            foreach (var item in feed.Items)
            {
                ms = new MemoryStream();
                w = XmlWriter.Create(ms, ws);
                item.Content.WriteTo(w, "Content", "");
                w.Flush();
                Console.WriteLine(Encoding.UTF8.GetString(ms.ToArray()));
                TextSyndicationContent textContent = item.Content as TextSyndicationContent;
                if (textContent != null)
                {
                    Console.WriteLine("   {0}", textContent.Text);
                }
                Console.WriteLine();
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

    public class Post_e86ed773_ac1f_4df4_b1fe_db1a3bae1b38
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            KeyValuePair<int, string> Echo(int key, string value);
        }
        public class Service : ITest
        {
            public KeyValuePair<int, string> Echo(int key, string value)
            {
                return new KeyValuePair<int, string>(key, value);
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
            var kvp = proxy.Echo(123, "one two three");
            Console.WriteLine("{0} {1}", kvp.Key, kvp.Value);

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_cc8ed381_bddc_4ec3_9adc_85e7b13b7747
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
        const string HeaderName = "AppInfo";
        const string HeaderNamespace = "http://my.application";
        [DataContract]
        public class AppInfo
        {
            [DataMember]
            public string AppName { get; set; }
            [DataMember]
            public string OtherAppData { get; set; }
        }
        public class MyClientInspector : IClientMessageInspector, IEndpointBehavior
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
                AppInfo appInfo = new AppInfo
                {
                    AppName = "MyApplication",
                    OtherAppData = "Something else",
                };
                request.Headers.Add(MessageHeader.CreateHeader(HeaderName, HeaderNamespace, appInfo));
                return null;
            }
        }
        public class MyDispatchInspector : IDispatchMessageInspector, IEndpointBehavior
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
                int headerIndex = request.Headers.FindHeader(HeaderName, HeaderNamespace);
                if (headerIndex < 0)
                {
                    throw new Exception("Header not present");
                    // or ignore it
                }
                else
                {
                    AppInfo appInfo = request.Headers.GetHeader<AppInfo>(headerIndex);
                    Console.WriteLine("Request to {0} by {1}", request.Headers.Action, appInfo.AppName);
                    // or any logging mechanism
                }

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
            endpoint.Behaviors.Add(new MyDispatchInspector());
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(new BasicHttpBinding(), new EndpointAddress(baseAddress));
            factory.Endpoint.Behaviors.Add(new MyClientInspector());
            ITest proxy = factory.CreateChannel();
            Console.WriteLine(proxy.Echo("Hello world"));
            ((IClientChannel)proxy).Close();
            factory.Close();

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
        public class MySurrogate : ISerializationSurrogate
        {
            public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
            {
                throw new NotImplementedException();
            }

            public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
            {
                throw new NotImplementedException();
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
            xs.Serialize(w, obj);
            w.Flush();
            Console.WriteLine(Encoding.UTF8.GetString(ms.ToArray()));
        }
    }

    public class Post_0d576dbc_b10c_4dc3_b7ab_fd1173bc9f82
    {
        [ServiceContract(Name = "IMyContract")]
        public interface IMyContract
        {
            [OperationContract]
            int Add(int x, int y);
        }
        [ServiceContract(Name = "IMyContract")]
        public interface IMyContract_V2
        {
            [OperationContract]
            int Add(int x, int y, int? z);
        }
        public class Service : IMyContract_V2
        {
            public int Add(int x, int y, int? z)
            {
                if (z.HasValue)
                {
                    return x + y + z.Value;
                }
                else
                {
                    throw new FaultException("Please update your contract");
                }
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(IMyContract_V2), new BasicHttpBinding(), "");
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<IMyContract> factory = new ChannelFactory<IMyContract>(new BasicHttpBinding(), new EndpointAddress(baseAddress));
            IMyContract proxy = factory.CreateChannel();

            try
            {
                Console.WriteLine(proxy.Add(3, 4));
            }
            catch (FaultException e)
            {
                Console.WriteLine(e.Reason);
            }

            ((IClientChannel)proxy).Close();
            factory.Close();
            host.Close();
        }
    }

    public class Post_13ac88d1_1727_4d1a_9f7d_a4b543d79162
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            [WebGet(UriTemplate = "Test?format=XML&records={records}", ResponseFormat = WebMessageFormat.Xml)]
            string TestXml(string records);
        }
        public class Service : ITest
        {
            public string TestXml(string records)
            {
                return "Hello XML, records = " + records;
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            WebServiceHost host = new WebServiceHost(typeof(Service), new Uri(baseAddress));
            host.Open();
            Console.WriteLine("Host opened");

            Util.SendRequest(baseAddress + "/Test?format=XML&records=10", "GET", null, null);
            Util.SendRequest(baseAddress + "/Test?format=XML", "GET", null, null);

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_a14dd9a5_edf3_464b_8e45_8001a0be8fcc
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract, WebGet(UriTemplate = "/clientaccesspolicy.xml")]
            [XmlSerializerFormat]
            AccessPolicy GetSLAccessPolicy();
        }
        [XmlRoot(ElementName = "access-policy", Namespace = "")]
        public class AccessPolicy
        {
            [XmlArray(ElementName = "cross-domain-access")]
            [XmlArrayItem(ElementName = "policy")]
            public Policy[] Policies { get; set; }
        }
        public class Policy
        {
            [XmlElement(ElementName = "allow-from")]
            public AllowFrom AllowFrom { get; set; }
            [XmlElement(ElementName = "grant-to")]
            public GrantTo GrantTo { get; set; }
        }
        public class AllowFrom
        {
            [XmlAttribute(AttributeName = "http-request-headers")]
            public string HttpRequestHeaders { get; set; }
            [XmlElement(ElementName = "domain")]
            public Domain[] Domains { get; set; }
        }
        public class Domain
        {
            [XmlAttribute(AttributeName = "uri")]
            public string Uri { get; set; }
        }
        public class GrantTo
        {
            [XmlElement(ElementName = "resource")]
            public Resource[] Resources { get; set; }
        }
        public class Resource
        {
            [XmlAttribute(AttributeName = "path")]
            public string Path { get; set; }
            [XmlAttribute(AttributeName = "include-subpaths")]
            public bool IncludeSubpaths { get; set; }
        }
        public static void Test()
        {
            WebChannelFactory<ITest> factory = new WebChannelFactory<ITest>(new Uri("http://www.bing.com"));
            ITest proxy = factory.CreateChannel();
            AccessPolicy policy = proxy.GetSLAccessPolicy();
            Console.WriteLine("The client access policy at www.bing.com has {0} policies", policy.Policies.Length);
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
        public class Book : IXmlSerializable
        {
            public System.Xml.Schema.XmlSchema GetSchema()
            {
                return null;
            }

            public void ReadXml(XmlReader reader)
            {
            }

            public void WriteXml(XmlWriter writer)
            {
                writer.WriteElementString("title", "The Catcher in the Rye");
            }
        }
        public class Publisher { }
        public class Library { }

        public class PListBehavior : Attribute, IOperationBehavior
        {
            #region IOperationBehavior Members

            public void AddBindingParameters(OperationDescription operationDescription, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
            {
                return;
            }

            public void ApplyClientBehavior(OperationDescription operationDescription, System.ServiceModel.Dispatcher.ClientOperation clientOperation)
            {
                ReplaceDataContractSerializerOperationBehavior(operationDescription);
            }

            public void ApplyDispatchBehavior(OperationDescription operationDescription, System.ServiceModel.Dispatcher.DispatchOperation dispatchOperation)
            {
                ReplaceDataContractSerializerOperationBehavior(operationDescription);
            }

            public void Validate(OperationDescription operationDescription)
            {
                return;
            }

            private static void ReplaceDataContractSerializerOperationBehavior(OperationDescription description)
            {
                for (int i = 0; i < description.Behaviors.Count; i++)
                {
                    if (description.Behaviors[i] is DataContractSerializerOperationBehavior)
                    {
                        var plistBehavior = new PListSerializerOperationBehavior(description);
                        description.Behaviors[i] = plistBehavior;
                    }
                }
            }
            #endregion
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
                this.dcs.WriteObjectContent(writer, graph);
            }

            public override void WriteStartObject(XmlDictionaryWriter writer, object graph)
            {
                this.dcs.WriteStartObject(writer, graph);
                writer.WriteAttributeString("__origin", "This was written by my custom serializer");
            }
        }

        public class PListSerializerOperationBehavior : DataContractSerializerOperationBehavior
        {
            public PListSerializerOperationBehavior(OperationDescription description)
                : base(description)
            {
                Console.WriteLine("In PListSerializerOperationBehavior..ctor");
            }

            public override System.Runtime.Serialization.XmlObjectSerializer CreateSerializer(Type type, string name, string ns, IList<Type> knownTypes)
            {
                Console.WriteLine("In PListSerializerOperationBehavior.CreateSerializer(Type, string, string, IList<Type>)");
                return new DataContractPListSerializer(type);
            }

            public override System.Runtime.Serialization.XmlObjectSerializer CreateSerializer(Type type, XmlDictionaryString name, XmlDictionaryString ns, IList<Type> knownTypes)
            {
                Console.WriteLine("In PListSerializerOperationBehavior.CreateSerializer(Type, XmlDictionaryString, XmlDictionaryString, IList<Type>)");
                return new DataContractPListSerializer(type);
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
                return new string[] { "hello" };
            }

            public Book[] GetBooksByTags(string tag)
            {
                return new Book[] { new Book() };
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
                foreach (var od in endpoint.Contract.Operations)
                {
                    Console.WriteLine("Operation: {0}", od.Name);
                    foreach (var ob in od.Behaviors)
                    {
                        Console.WriteLine("  {0}", ob.GetType().FullName);
                    }
                }
            }

            public void Validate(ServiceEndpoint endpoint)
            {
            }
        }

        public static void Test()
        {
            string baseAddress = "http://localhost:8000/Service";
            ServiceHost host = new ServiceHost(typeof(LibraryService), new Uri(baseAddress));

            WebHttpBinding webBinding = new WebHttpBinding(WebHttpSecurityMode.None);
            BasicHttpBinding basicBinding = new BasicHttpBinding(BasicHttpSecurityMode.None);

            ServiceMetadataBehavior metadataBehavior = new ServiceMetadataBehavior { HttpGetEnabled = true, HttpGetUrl = new Uri(host.BaseAddresses[0].AbsoluteUri + "/mex") };
            WebHttpBehavior httpBehavior = new WebHttpBehavior { DefaultOutgoingResponseFormat = System.ServiceModel.Web.WebMessageFormat.Xml };
            //WebHttpBehavior httpBehavior = new PListWebHttpBehavior { DefaultOutgoingResponseFormat = System.ServiceModel.Web.WebMessageFormat.Xml };

            ServiceEndpoint httpEndpoint = host.AddServiceEndpoint(typeof(ILibrary), webBinding, string.Empty);
            host.AddServiceEndpoint(typeof(ILibrary), basicBinding, "basic");

            host.Description.Behaviors.Add(metadataBehavior);
            httpEndpoint.Behaviors.Add(new MyEndpointBehavior());
            httpEndpoint.Behaviors.Add(httpBehavior);

            host.Open();

            Util.SendRequest(baseAddress + "/GetBooks", "GET", null, null);

            Console.WriteLine("Press ENTER.");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_b8513225_2b84_4913_8017_125edd55e4be_b
    {
        [DataContract]
        public class MyDC
        {
            [DataMember]
            public string str;

            public override string ToString()
            {
                return String.Format("MyDC[str={0}]", str);
            }
        }

        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            [WebGet]
            MyDC Echo(string text);
        }

        public class Service : ITest
        {
            public MyDC Echo(string text)
            {
                return new MyDC { str = text };
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
                foreach (var od in endpoint.Contract.Operations)
                {
                    ReplaceDCSOB(od);
                }
            }

            private void ReplaceDCSOB(OperationDescription od)
            {
                for (int i = 0; i < od.Behaviors.Count; i++)
                {
                    if (od.Behaviors[i] is DataContractSerializerOperationBehavior)
                    {
                        od.Behaviors[i] = new MyDCSOB(od);
                        break;
                    }
                }
            }

            public void Validate(ServiceEndpoint endpoint)
            {
            }
        }

        public class MyDCSOB : DataContractSerializerOperationBehavior
        {
            public MyDCSOB(OperationDescription od) : base(od) { }

            public override XmlObjectSerializer CreateSerializer(Type type, string name, string ns, IList<Type> knownTypes)
            {
                Console.WriteLine("Inside CreateSerializer(Type type, string name, string ns, IList<Type> knownTypes)");
                return base.CreateSerializer(type, name, ns, knownTypes);
                //return new MySerializer(type);
            }

            public override XmlObjectSerializer CreateSerializer(Type type, XmlDictionaryString name, XmlDictionaryString ns, IList<Type> knownTypes)
            {
                Console.WriteLine("Inside CreateSerializer(Type type, XmlDictionaryString name, XmlDictionaryString ns, IList<Type> knownTypes)");
                return base.CreateSerializer(type, name, ns, knownTypes);
                //return new MySerializer(type);
            }
        }

        public class MySerializer : XmlObjectSerializer
        {
            DataContractSerializer dcs;
            public MySerializer(Type type)
            {
                this.dcs = new DataContractSerializer(type);
            }

            public override bool IsStartObject(XmlDictionaryReader reader)
            {
                bool result = this.dcs.IsStartObject(reader);
                return result;
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
                this.dcs.WriteObjectContent(writer, graph);
            }

            public override void WriteStartObject(XmlDictionaryWriter writer, object graph)
            {
                this.dcs.WriteStartObject(writer, graph);
                writer.WriteAttributeString("__serializer", this.GetType().Name);
            }

            public override bool IsStartObject(XmlReader reader)
            {
                return this.dcs.IsStartObject(reader);
            }
        }

        public class MyWebHttpBehavior : WebHttpBehavior
        {
            public MyWebHttpBehavior() : base() { }

            protected override IDispatchMessageFormatter GetReplyDispatchFormatter(OperationDescription operationDescription, ServiceEndpoint endpoint)
            {
                return base.GetReplyDispatchFormatter(operationDescription, endpoint);
            }

            public class MyReplyDispatchFormatter : IDispatchMessageFormatter
            {
                OperationDescription od;
                ServiceEndpoint endpoint;
                public MyReplyDispatchFormatter(OperationDescription od, ServiceEndpoint endpoint)
                {
                    this.od = od;
                    this.endpoint = endpoint;
                }

                public void DeserializeRequest(Message message, object[] parameters)
                {
                    throw new NotSupportedException();
                }

                public Message SerializeReply(MessageVersion messageVersion, object[] parameters, object result)
                {
                    throw new NotImplementedException();
                }
            }
        }

        public static void Test()
        {
            MemoryStream ms = new MemoryStream();
            object obj = new MyDC { str = "hello" };
            MySerializer ser = new MySerializer(obj.GetType());
            ser.WriteObject(ms, obj);
            Console.WriteLine(Encoding.UTF8.GetString(ms.ToArray()));
            ms.Position = 0;
            object newObj = ser.ReadObject(ms);
            Console.WriteLine(newObj);
            ms.SetLength(0);
            new DataContractSerializer(obj.GetType()).WriteObject(ms, obj);
            Console.WriteLine(Encoding.UTF8.GetString(ms.ToArray()));
            ms.Position = 0;
            newObj = ser.ReadObject(ms);
            Console.WriteLine(newObj);

            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            ServiceEndpoint basic = host.AddServiceEndpoint(typeof(ITest), new BasicHttpBinding(), "basic");
            ServiceEndpoint web = host.AddServiceEndpoint(typeof(ITest), new WebHttpBinding(), "web");

            basic.Behaviors.Add(new MyBehavior());
            web.Behaviors.Add(new MyBehavior());
            web.Behaviors.Add(new WebHttpBehavior());

            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(new BasicHttpBinding(), new EndpointAddress(baseAddress + "/basic"));
            ITest proxy = factory.CreateChannel();
            Console.WriteLine(proxy.Echo("Hello world"));
            ((IClientChannel)proxy).Close();
            factory.Close();

            factory = new ChannelFactory<ITest>(new WebHttpBinding(), new EndpointAddress(baseAddress + "/web"));
            factory.Endpoint.Behaviors.Add(new WebHttpBehavior());
            proxy = factory.CreateChannel();
            Console.WriteLine(proxy.Echo("Hello world"));
            ((IClientChannel)proxy).Close();
            factory.Close();
        }
    }

    public class Post_3588c5e3_3a1b_4252_a437_de560effc624
    {
        [XmlRoot(ElementName = "A_list")]
        public class A_list : List<A>
        {
            public A this[string name]
            {
                get
                {
                    foreach (A a in this)
                    {
                        if (a.Name == name) return a;
                    }

                    return null;
                }
            }
        }

        public class A
        {
            B_list listB;

            string name;

            public A() { listB = new B_list(); }

            [XmlArray(ElementName = "B_list")]
            [XmlArrayItem(ElementName = "B")]
            public B_list ListB
            {
                get { return listB; }
                set { listB = value; }
            }

            [XmlAttribute(AttributeName = "name")]
            public string Name
            {
                get { return name; }
                set { name = value; }
            }
        }

        public class B_list : List<B>
        {
            public B this[string name]
            {
                get
                {
                    foreach (B b in this)
                    {
                        if (b.Name == name) return b;
                    }

                    return null;
                }
            }
        }

        public class B
        {
            String name;
            String id;

            [XmlAttribute(AttributeName = "name")]
            public String Name
            {
                get { return name; }
                set { name = value; }
            }

            [XmlAttribute(AttributeName = "id")]
            public String Id
            {
                get { return id; }
                set { id = value; }
            }
        }

        const string xml = @"<A_list>
    <A name=""a1_name"">
        <B_list>
            <B name=""b1_name"" id=""b1_id"" />
            <B name=""b2_name"" id=""b2_id"" />
            <B name=""b3_name"" id=""b3_id"" />
        </B_list>
    </A>
    <A name=""a2_name"">
        <B_list>
            <B name=""b1_name"" id=""b1_id"" />
            <B name=""b2_name"" id=""b2_id"" />
            <B name=""b3_name"" id=""b3_id"" />
        </B_list>
    </A>
</A_list>";

        public static void Test()
        {
            XmlSerializer ser = new XmlSerializer(typeof(A_list));
            XmlReader xmlReader = XmlReader.Create(new MemoryStream(Encoding.UTF8.GetBytes(xml)));
            A_list listA = (A_list)ser.Deserialize(xmlReader);
            Console.WriteLine(listA[0].ListB[1].Id);
            Console.WriteLine(listA[0].ListB[1].Name);
            Console.WriteLine(listA["a1_name"].ListB["b2_name"].Id);
        }
    }

    public class Inditalk_20100617_140000
    {
        [ServiceContract]
        public interface IServerInterface
        {
            [OperationContract]
            [WebInvoke(UriTemplate = "/WrappedComplexJson")]
            Stream Process(Stream input);
        }

        public class SomeData
        {
            public string str = "hello";
        }

        [ServiceContract]
        public interface IClientInterface
        {
            [OperationContract]
            [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
            SomeData WrappedComplexJson(string a, int b, SomeData c);
        }

        public class Service : IServerInterface
        {
            public Stream Process(Stream input)
            {
                XmlDictionaryReader jsonReader = JsonReaderWriterFactory.CreateJsonReader(input, XmlDictionaryReaderQuotas.Max);
                jsonReader.Read(); // root
                jsonReader.Read(); // to first param
                Console.WriteLine(Deserialize(jsonReader, "a", typeof(string)));
                Console.WriteLine(Deserialize(jsonReader, "b", typeof(int)));
                Console.WriteLine(Deserialize(jsonReader, "c", typeof(SomeData)));
                return Stream.Null;
            }

            private object Deserialize(XmlDictionaryReader jsonReader, string paramName, Type paramType)
            {
                DataContractJsonSerializer dcjs = new DataContractJsonSerializer(paramType, paramName);
                object result = dcjs.ReadObject(jsonReader);
                return result;
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
            WebHttpBinding binding = new WebHttpBinding { ContentTypeMapper = new RawMapper() };
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(IServerInterface), binding, "").Behaviors.Add(new WebHttpBehavior());
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<IClientInterface> factory = new ChannelFactory<IClientInterface>(new WebHttpBinding(), new EndpointAddress(baseAddress));
            factory.Endpoint.Behaviors.Add(new WebHttpBehavior());
            IClientInterface proxy = factory.CreateChannel();
            Console.WriteLine(proxy.WrappedComplexJson("foo bar", 1234, new SomeData()));
        }
    }

    public class Post_b144473a_f432_4ef3_98b2_e54c73cc96a5
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
            ServiceContractGenerator gen = new ServiceContractGenerator(unit);
            gen.GenerateServiceContractType(allContracts[0]);
            string bindingSectionName, configurationName;
            ChannelEndpointElement channelElement;
            gen.GenerateServiceEndpoint(allEndpoints[0], out channelElement);
            Console.WriteLine(channelElement);
            gen.GenerateBinding(allBindings[0], out bindingSectionName, out configurationName);
            gen.Configuration.SaveAs(@"c:\temp\deleteme\myoutput.config");

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

    public class Post_c2855d39_76ea_470f_96fa_d1e6424bd4ce
    {
        public static class Constants
        {
            public const string EnqueueAction = "http://tempuri.org/Enqueue";
            public const string EnqueueReplyAction = "http://tempuri.org/EnqueueResponse";
            public const string DequeueAction = "http://tempuri.org/Dequeue";
            public const string DequeueReplyAction = "http://tempuri.org/DequeueResponse";
        }

        [ServiceContract]
        public interface ITest
        {
            [OperationContract(Action = Constants.EnqueueAction, ReplyAction = Constants.EnqueueReplyAction)]
            Message Enqueue(Message message);
            [OperationContract(Action = Constants.DequeueAction, ReplyAction = Constants.DequeueReplyAction)]
            Message Dequeue(Message message);
        }

        public class Service : ITest
        {
            static Queue<byte[]> messages = new Queue<byte[]>();
            static object syncRoot = new object();

            public Message Enqueue(Message input)
            {
                MemoryStream ms = new MemoryStream();
                XmlWriter w = XmlWriter.Create(ms);
                input.WriteMessage(w);
                w.Flush();
                lock (syncRoot)
                {
                    messages.Enqueue(ms.ToArray());
                }
                return Message.CreateMessage(input.Version, Constants.EnqueueReplyAction, "Message enqueued");
            }

            public Message Dequeue(Message input)
            {
                byte[] messageBytes = null;
                lock (syncRoot)
                {
                    if (messages.Count > 0)
                    {
                        messageBytes = messages.Dequeue();
                    }
                }

                Message result;
                if (messageBytes == null)
                {
                    result = Message.CreateMessage(input.Version, Constants.DequeueReplyAction, "no messages");
                }
                else
                {
                    XmlReader r = XmlReader.Create(new MemoryStream(messageBytes));
                    result = Message.CreateMessage(r, int.MaxValue, input.Version);
                    result.Headers.Action = Constants.DequeueReplyAction;
                }

                return result;
            }
        }

        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            BasicHttpBinding binding = new BasicHttpBinding();
            host.AddServiceEndpoint(typeof(ITest), binding, "");
            host.Open();
            Console.WriteLine("Host opened");

            Message input1 = Message.CreateMessage(binding.MessageVersion, Constants.EnqueueAction, "This is the first message to be enqueued");
            Message input2 = Message.CreateMessage(binding.MessageVersion, Constants.EnqueueAction, "This is the second message to be enqueued");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(binding, new EndpointAddress(baseAddress));
            ITest proxy = factory.CreateChannel();

            Console.WriteLine(proxy.Enqueue(input1));
            Console.WriteLine(proxy.Enqueue(input2));

            Console.WriteLine("Messages enqueued");

            Message dequeueInput = Message.CreateMessage(binding.MessageVersion, Constants.DequeueAction, (object)null);
            Console.WriteLine("Dequeue result: {0}", proxy.Dequeue(dequeueInput));
            dequeueInput = Message.CreateMessage(binding.MessageVersion, Constants.DequeueAction, (object)null);
            Console.WriteLine("Dequeue result: {0}", proxy.Dequeue(dequeueInput));
            dequeueInput = Message.CreateMessage(binding.MessageVersion, Constants.DequeueAction, (object)null);
            Console.WriteLine("Dequeue result: {0}", proxy.Dequeue(dequeueInput));
        }
    }

    public class Post_1bad2198_65cb_445d_84cb_1fae17828eef_Server
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            [WebGet(UriTemplate = "/Add1?x={x}&y={y}")]
            int Add1(int x, int y);

            [OperationContract]
            [WebGet(UriTemplate = "/Sub1?x={x}&y={y}")]
            int Sub1(int x, int y);

            [OperationContract]
            [WebGet(UriTemplate = "/UppercaseAll?name={Name}", BodyStyle = WebMessageBodyStyle.Bare)]
            string UppercaseAll(string Name);

            [OperationContract]
            [WebGet(UriTemplate = "/ConcatenateDaStrings/{oneStr}/{twoStr}")] //note format 
            string ConcatenateStrs(string oneStr, string twoStr);

            [OperationContract]
            [WebGet(UriTemplate = "/customers/{id}")]
            Customer GetCustomer(string id);

            [OperationContract]
            [WebInvoke(UriTemplate = "/customers", Method = "POST")]
            Customer PostCustomer(Customer c);
        }

        [DataContract(Name = "Customer", Namespace = "")]
        public class Customer
        {
            [DataMember]
            public string ID { get; set; }
            [DataMember]
            public string Name { get; set; }
        }

        public class Service : ITest
        {
            public int Add1(int x, int y)
            {
                return x + y;
            }

            public int Sub1(int x, int y)
            {
                return x - y;
            }

            public string UppercaseAll(string Name)
            {
                return Name.ToUpper();
            }

            public string ConcatenateStrs(string oneStr, string twoStr)
            {
                return oneStr + twoStr;
            }

            public Customer GetCustomer(string id)
            {
                return new Customer { ID = id, Name = "John Doe" };
            }

            public Customer PostCustomer(Customer c)
            {
                return c;
            }
        }

        internal static readonly string BaseAddress = "http://" + Environment.MachineName + ":8000/Service";
        ServiceHost host;
        public void StartService()
        {
            host = new ServiceHost(typeof(Service), new Uri(BaseAddress));
            host.AddServiceEndpoint(typeof(ITest), new WebHttpBinding(), "").Behaviors.Add(new WebHttpBehavior());
            host.Open();
            Console.WriteLine("Host opened");
        }
        public void EndService()
        {
            host.Close();
            host = null;
        }
    }

    public class Post_1bad2198_65cb_445d_84cb_1fae17828eef_Client
    {
        [ServiceContract]
        public interface IService100
        {
            [OperationContract]
            [WebGet(UriTemplate = "/Add1?x={x}&y={y}")]
            int Add1(int x, int y);

            [OperationContract]
            [WebGet(UriTemplate = "/Sub1?x={x}&y={y}")]
            int Sub1(int x, int y);

            [OperationContract]
            [WebGet(UriTemplate = "/UppercaseAll?name={Name}", BodyStyle = WebMessageBodyStyle.Bare)]
            string UppercaseAll(string Name);

            [OperationContract]
            [WebGet(UriTemplate = "/ConcatenateDaStrings/{oneStr}/{twoStr}")] //note format 
            string ConcatenateStrs(string oneStr, string twoStr);

            [OperationContract]
            [WebGet(UriTemplate = "/customers/{id}")]
            Customer GetCustomer(string id);

            [OperationContract]
            [WebInvoke(UriTemplate = "/customers", Method = "POST")]
            Customer PostCustomer(Customer c);
        }

        [DataContract(Name = "Customer", Namespace = "")]
        public class Customer
        {
            [DataMember]
            public string ID { get; set; }
            [DataMember]
            public string Name { get; set; }

            public override string ToString()
            {
                return String.Format("Customer[ID={0},Name={1}]", ID, Name);
            }
        }

        public static void Test()
        {
            //The server part is not needed if the server is hosted in IIS;
            //having it here makes it simpler to have a self-contained code.
            Post_1bad2198_65cb_445d_84cb_1fae17828eef_Server server = new Post_1bad2198_65cb_445d_84cb_1fae17828eef_Server();
            server.StartService();

            string url = Post_1bad2198_65cb_445d_84cb_1fae17828eef_Server.BaseAddress;
            try
            {
                WebChannelFactory<IService100> factory = new WebChannelFactory<IService100>(new Uri(url));
                IService100 proxy = factory.CreateChannel();

                // invoke -- int Add1(int x, int y);
                int intI1 = proxy.Add1(2, 3);
                Console.WriteLine(".Add1 web method gives output: " + intI1.ToString()); //works, gives 5
                // 

                // invoke -- int Sub1(int x, int y);
                int intI2 = proxy.Sub1(2, 3);
                Console.WriteLine(".Sub1 web method gives output: " + intI2.ToString()); //works, gives -1
                //

                // invoke  -- string UppercaseAll(string Name);
                string strS2 = proxy.UppercaseAll("a lowercase string");
                Console.WriteLine(".UppercaseAll web method gives output: " + strS2); //works .UppercaseAll web method gives output: A LOWERCASE STRING

                string strS3 = proxy.ConcatenateStrs("anna one", "beta two");
                Console.WriteLine(".ConcatenateStrs method gives output: " + strS3); //works to give .ConcatenateStrs method gives output: anna onebeta two

                Customer c = proxy.GetCustomer("123");
                Console.WriteLine("GetCustomer returned: {0}", c);

                Customer c2 = new Customer { ID = "111", Name = "Jane Roe" };
                Customer c3 = proxy.PostCustomer(c2);
                Console.WriteLine("PostCustomer returned: {0}", c3);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            server.EndService();
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
        }
        [ServiceContract(Name = "ITest", Namespace = "http://tempuri.org")]
        public interface ITestClient
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
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITestServer), new BasicHttpBinding(), "");
            host.Open();

            ChannelFactory<ITestClient> factory = new ChannelFactory<ITestClient>(new BasicHttpBinding(), new EndpointAddress(baseAddress));
            ITestClient proxy = factory.CreateChannel();

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

            Array array = Array.CreateInstance(typeof(DateTime), 10);
            Console.WriteLine(array.GetType().AssemblyQualifiedName);
            for (int i = 0; i < array.Length; i++)
            {
                array.SetValue(new DateTime(2010, 6, i + 1), i);
            }
            DataContractSerializer dcs = new DataContractSerializer(typeof(Array));
            MemoryStream ms = new MemoryStream();
            try
            {
                dcs.WriteObject(ms, array);
                Console.WriteLine(Encoding.UTF8.GetString(ms.ToArray()));
            }
            catch (Exception e)
            {
                Console.WriteLine("{0}: {1}", e.GetType().FullName, e.Message);
            }

            Console.WriteLine(typeof(DateTime[]).BaseType.AssemblyQualifiedName);
        }
    }

    public class Post_ec687986_bdcf_4cae_8d1a_3231196ab142
    {
        [DataContract]
        public class MyType
        {
            [DataMember]
            public string Remark { get; set; }
            [DataMember]
            public int CompanyID { get; set; }
            [DataMember]
            public DateTime MonthYear { get; set; }

            public override string ToString()
            {
                return string.Format("MyType[Remark={0},CompanyID={1},MonthYear={2}]", Remark, CompanyID, MonthYear);
            }
        }
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
            string Process(MyType d);
        }
        public class Service : ITest
        {
            public string Process(MyType d)
            {
                return d.ToString();
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000";
            WebServiceHost host = new WebServiceHost(typeof(Service), new Uri(baseAddress));
            host.Open();
            Console.WriteLine("Host opened");

            string request = "{\"d\":{\"Remark\":\"\", \"CompanyID\": 123, \"MonthYear\": \"\\/Date(1277913600000)\\/\"}}";
            Console.WriteLine(request);
            Util.SendRequest(baseAddress + "/Process", "POST", "application/json", request);

            Console.WriteLine("Press ENTER to close");
            Console.ReadLine();
            host.Close();
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
                    Console.WriteLine("Incoming content-length: {0}", prop.Headers[HttpRequestHeader.ContentLength]);
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

    public class Post_b9e06bde_a455_467a_b80e_18837276ee4c
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            int rtest(out int a, string UID);
        }
        public class Service : ITest
        {
            public int rtest(out int a, string UID)
            {
                a = 123;
                return 333;
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
            int a;
            Console.WriteLine("{0}, {1}", proxy.rtest(out a, "Hello"), a);

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_d2f11fd8_3aa5_4713_9ad9_030149cf6af0
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
        public class Service : ITestAsync
        {
            public IAsyncResult BeginAdd(int x, int y, AsyncCallback callback, object state)
            {
                Func<int, int, int> func = (a, b) => a + b;
                return func.BeginInvoke(x, y, callback, state);
            }

            public int EndAdd(IAsyncResult asyncResult)
            {
                Func<int, int, int> func = (Func<int, int, int>)((System.Runtime.Remoting.Messaging.AsyncResult)asyncResult).AsyncDelegate;
                return func.EndInvoke(asyncResult);
            }
        }
        static Binding GetBinding()
        {
            var result = new WSHttpBinding(SecurityMode.None);
            //Change binding settings here
            return result;
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITestAsync), GetBinding(), "");
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(GetBinding(), new EndpointAddress(baseAddress));
            ITest proxy = factory.CreateChannel();
            Console.WriteLine("Result (sync): {0}", proxy.Add(5, 8));

            ChannelFactory<ITestAsync> factory2 = new ChannelFactory<ITestAsync>(GetBinding(), new EndpointAddress(baseAddress));
            ITestAsync proxy2 = factory2.CreateChannel();
            AutoResetEvent evt = new AutoResetEvent(false);
            Console.WriteLine(proxy2.BeginAdd(6, 9, delegate(IAsyncResult ar)
            {
                Console.WriteLine("Result (async): {0}", proxy2.EndAdd(ar));
                evt.Set();
            }, null));

            evt.WaitOne();

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class RotTest
    {
        private static byte Rotate(byte b, int rotNumber)
        {
            int i = b;
            if (rotNumber < -25 || rotNumber > 25)
            {
                throw new ArgumentException("The rotation count must be between -25 and 25");
            }

            if ('a' <= i && i <= 'z')
            {
                i = ((i - 'a' + rotNumber + 26) % 26) + 'a';
            }
            else if ('A' <= i && i <= 'Z')
            {
                i = ((i - 'A' + rotNumber + 26) % 26) + 'A';
            }

            return (byte)i;
        }
        public static void Test()
        {
            string abc = "abcdef ghijkl mnopqr stuvw xyz 012345 6789 ABCDEF GHIJKL MNOPQR STUVW XYZ";
            for (int i = 0; i < 26; i++)
            {
                byte[] abcBytes = Encoding.UTF8.GetBytes(abc);
                for (int j = 0; j < abcBytes.Length; j++)
                {
                    abcBytes[j] = Rotate(abcBytes[j], i);
                }

                string rotated = Encoding.UTF8.GetString(abcBytes);
                for (int j = 0; j < abcBytes.Length; j++)
                {
                    abcBytes[j] = Rotate(abcBytes[j], -i);
                }

                string backToNormal = Encoding.UTF8.GetString(abcBytes);

                Console.WriteLine("{0}:", i);
                Console.WriteLine(abc);
                Console.WriteLine(rotated);
                Console.WriteLine("Can unrotate: {0}", abc == backToNormal);
                Console.WriteLine();
            }
        }
    }

    public class Post_da54a7a9_bc5e_45c4_b1ac_15e1016df4aa
    {
        [DataContract]
        public class Customer
        {
            [DataMember]
            public string Id { get; set; }
            [DataMember]
            public string Name { get; set; }
            [DataMember]
            public string Address { get; set; }
        }
        [ServiceContract]
        public interface ICustomerManager
        {
            [WebGet(UriTemplate = "/Customers")]
            [OperationContract]
            List<Customer> GetCustomers();

            [WebGet(UriTemplate = "/Customer/{id}")]
            Customer GetCustomer(string id);
        }
        public class CustomerManagerService : ICustomerManager
        {
            List<Customer> AllCustomers = new List<Customer>
            {
                new Customer { Id = "1", Name = "Homer", Address = "234 Main St., Springfield" },
                new Customer { Id = "2", Name = "Marge", Address = "234 Main St., Springfield" },
                new Customer { Id = "3", Name = "Moe", Address = "100 5th Ave., Springfield" },
                new Customer { Id = "4", Name = "Flanders", Address = "236 Main St., Springfield" },
            };
            public List<Customer> GetCustomers()
            {
                return new List<Customer>(AllCustomers);
            }

            public Customer GetCustomer(string id)
            {
                return AllCustomers.Where(c => c.Id == id).FirstOrDefault();
            }
        }
        public class MySoapAndWebFactory : ServiceHostFactory
        {
            protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
            {
                ServiceHost host = new ServiceHost(serviceType, baseAddresses);
                host.AddServiceEndpoint(typeof(ICustomerManager), new BasicHttpBinding(), "soap");
                host.AddServiceEndpoint(typeof(ICustomerManager), new WebHttpBinding(), "web").Behaviors.Add(new WebHttpBehavior());
                ServiceMetadataBehavior smb = host.Description.Behaviors.Find<ServiceMetadataBehavior>();
                if (smb == null)
                {
                    smb = new ServiceMetadataBehavior();
                    host.Description.Behaviors.Add(smb);
                }

                smb.HttpGetEnabled = true;
                return host;
            }
        }
    }

    // http://stackoverflow.com/q/8012009/751090
    public class StackOverflow_8012009
    {
        const string XML = @"<Person><Enabled>true</Enabled><Name>John Smith</Name></Person>";
        [DataContract(Namespace = "", Name = "Person")]
        public class Person
        {
            [DataMember]
            public bool Enabled { get; set; }
            [DataMember]
            public string Name { get; set; }

            public override string ToString()
            {
                return string.Format("Person[Enabled={0},Name={1}]", this.Enabled, this.Name);
            }
        }
        public static void Test()
        {
            DataContractSerializer dcs = new DataContractSerializer(typeof(Person));
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(XML));
            object obj = dcs.ReadObject(ms);
            Console.WriteLine(obj);
        }
    }

    // http://stackoverflow.com/q/8022154/751090
    public class StackOverflow_8022154
    {
        const string XML = @"<places xmlns=""http://where.yahooapis.com/v1/schema.rng""  
        xmlns:yahoo=""http://www.yahooapis.com/v1/base.rng""  
        yahoo:start=""0"" yahoo:count=""247"" yahoo:total=""247""> 
        <place yahoo:uri=""http://where.yahooapis.com/v1/place/23424966""  
            xml:lang=""en-US""> 
            <woeid>23424966</woeid> 
            <placeTypeName code=""12"">Country</placeTypeName> 
            <name>Sao Tome and Principe</name> 
        </place> 
        <place yahoo:uri=""http://where.yahooapis.com/v1/place/23424824""  
            xml:lang=""en-US""> 
            <woeid>23424824</woeid> 
            <placeTypeName code=""12"">Country</placeTypeName> 
            <name>Ghana</name> 
        </place> 
    </places>";

        const string ElementsNamespace = "http://where.yahooapis.com/v1/schema.rng";
        const string YahooNamespace = "http://www.yahooapis.com/v1/base.rng";
        const string XmlNamespace = "http://www.w3.org/XML/1998/namespace";

        [XmlType(Namespace = ElementsNamespace, TypeName = "places")]
        [XmlRoot(ElementName = "places", Namespace = ElementsNamespace)]
        public class Places
        {
            [XmlAttribute(AttributeName = "start", Namespace = YahooNamespace)]
            public int Start { get; set; }
            [XmlAttribute(AttributeName = "count", Namespace = YahooNamespace)]
            public int Count;
            [XmlAttribute(AttributeName = "total", Namespace = YahooNamespace)]
            public int Total;
            [XmlElement(ElementName = "place", Namespace = ElementsNamespace)]
            public List<Place> AllPlaces { get; set; }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("Places[start={0},count={1},total={2}]:", this.Start, this.Count, this.Total);
                sb.AppendLine();
                foreach (var place in this.AllPlaces)
                {
                    sb.AppendLine("   " + place.ToString());
                }

                return sb.ToString();
            }
        }
        [XmlType(TypeName = "place", Namespace = ElementsNamespace)]
        public class Place
        {
            [XmlAttribute(AttributeName = "uri", Namespace = YahooNamespace)]
            public string Uri { get; set; }
            [XmlAttribute(AttributeName = "lang", Namespace = XmlNamespace)]
            public string Lang { get; set; }
            [XmlElement(ElementName = "woeid")]
            public string Woeid { get; set; }
            [XmlElement(ElementName = "placeTypeName")]
            public PlaceTypeName PlaceTypeName;
            [XmlElement(ElementName = "name")]
            public string Name { get; set; }
            
            public override string ToString()
            {
                return string.Format("Place[Uri={0},Lang={1},Woeid={2},PlaceTypeName={3},Name={4}]",
                    this.Uri, this.Lang, this.Woeid, this.PlaceTypeName, this.Name);
            }
        }
        [XmlType(TypeName = "placeTypeName", Namespace = ElementsNamespace)]
        public class PlaceTypeName
        {
            [XmlAttribute(AttributeName = "code")]
            public string Code { get; set; }
            [XmlText]
            public string Value { get; set; }

            public override string ToString()
            {
                return string.Format("TypeName[Code={0},Value={1}]", this.Code, this.Value);
            }
        }
        [ServiceContract]
        [XmlSerializerFormat]
        public interface IConsumeGeoPlanet
        {
            [OperationContract]
            [WebGet(
                UriTemplate = "countries?appid={appId}",
                ResponseFormat = WebMessageFormat.Xml,
                BodyStyle = WebMessageBodyStyle.Bare
            )]
            Places Countries(string appId);
        }

        public sealed class GeoPlanetConsumer : ClientBase<IConsumeGeoPlanet>
        {
            public GeoPlanetConsumer(string address)
                : base(new WebHttpBinding(), new EndpointAddress(address))
            {
                this.Endpoint.Behaviors.Add(new WebHttpBehavior());
            }

            public Places Countries(string appId)
            {
                return Channel.Countries(appId);
            }
        }

        [ServiceContract]
        public class SimulatedYahooService
        {
            [WebGet(UriTemplate = "*")]
            public Stream GetData()
            {
                WebOperationContext.Current.OutgoingResponse.ContentType = "text/xml";
                return new MemoryStream(Encoding.UTF8.GetBytes(XML));
            }
        }

        public static void Test()
        {
            Console.WriteLine("First a simpler test with serialization only.");
            XmlSerializer xs = new XmlSerializer(typeof(Places));
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(XML));
            object o = xs.Deserialize(ms);
            Console.WriteLine(o);

            Console.WriteLine();
            Console.WriteLine("Now in a real service");
            Console.WriteLine();
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            WebServiceHost host = new WebServiceHost(typeof(SimulatedYahooService), new Uri(baseAddress));
            host.Open();
            Console.WriteLine("Host opened");

            GeoPlanetConsumer consumer = new GeoPlanetConsumer(baseAddress);
            Places places = consumer.Countries("abcdef");
            Console.WriteLine(places);
        }
    }

    public class Post_aa392e74_e6ad_481b_93e2_293c3a2b371a
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            string Helloworld(string yourname);
        }
        public class Service : ITest
        {
            public string Helloworld(string yourname)
            {
                return "Hello, yourname has " + yourname.Length + " characters";
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
                MessageBuffer buffer = request.CreateBufferedCopy(9999999);
                Message copied = buffer.CreateMessage();
                request = copied;
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
            var endpoint = host.AddServiceEndpoint(typeof(ITest), new BasicHttpBinding(), "");
            endpoint.Behaviors.Add(new MyInspector());
            host.Open();
            Console.WriteLine("Host opened");

            ChannelFactory<ITest> factory = new ChannelFactory<ITest>(new BasicHttpBinding(), new EndpointAddress(baseAddress));
            ITest proxy = factory.CreateChannel();
            Console.WriteLine(proxy.Helloworld("John Doe"));
            string largeName = new string('r', 2000);
            Console.WriteLine(proxy.Helloworld(largeName));

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_01a72aad_cb4c_41fe_97e0_29f59cdcc1ef
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
        public class MyOperationSelector : IDispatchOperationSelector
        {
            public string SelectOperation(ref Message message)
            {
                throw new NotImplementedException();
            }
        }
        static Binding GetBinding()
        {
            var result = new WSHttpBinding(SecurityMode.None);
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

    public class Post_7eda47aa_ef9b_40e5_a8d4_f1f78e5d00be
    {
        public static void Test()
        {
            string address = "http://service-name/vdir/PicService.svc/GetPicReport/myImageName";
            HttpWebRequest req = HttpWebRequest.Create(address) as HttpWebRequest;
            byte[] myImage = new byte[3000];
            new Random().NextBytes(myImage); // some image content
            req.Method = "POST";
            req.ContentType = "application/octet-stream";
            Stream reqStream = req.GetRequestStream();
            reqStream.Write(myImage, 0, myImage.Length);
            reqStream.Close();
            HttpWebResponse resp;
            try
            {
                resp = req.GetResponse() as HttpWebResponse;
            }
            catch (WebException e)
            {
                resp = e.Response as HttpWebResponse;
            }

            Console.WriteLine("HTTP/{0} {1} {2}", resp.ProtocolVersion, (int)resp.StatusCode, resp.StatusDescription);
            foreach (var headerName in resp.Headers.AllKeys)
            {
                Console.WriteLine("{0}: {1}", headerName, resp.Headers[headerName]);
            }
            Console.WriteLine();
            if (resp.ContentLength > 0)
            {
                Console.WriteLine(new StreamReader(resp.GetResponseStream()).ReadToEnd());
            }

            resp.Close();
        }
    }

    // http://stackoverflow.com/q/8152252/751090
    public class StackOverflow_8152252
    {
        [DataContract]
        public class CustomBranches
        {
            [DataMember]
            public int Id { get; set; }

            [DataMember]
            public string branch_name { get; set; }

            [DataMember]
            public string address_line_1 { get; set; }

            [DataMember]
            public string city_name { get; set; }

            public int NonDataMember { get; set; }

            [DataMember]
            public string FieldDataMember;

            [DataMember]
            internal string NonPublicMember { get; set; }
        }

        public static void Test()
        {
            BindingFlags instancePublicAndNot = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var memberNames = typeof(CustomBranches)
                .GetProperties(instancePublicAndNot)
                .OfType<MemberInfo>()
                .Union(typeof(CustomBranches).GetFields(instancePublicAndNot))
                .Where(x => Attribute.IsDefined(x, typeof(DataMemberAttribute)))
                .Select(x => x.Name);
            Console.WriteLine("All data member names");
            foreach (var memberName in memberNames)
            {
                Console.WriteLine("  {0}", memberName);
            }
        }
    }

    public class Post_184d2fb2_2004_4601_aaf2_ea0ff9d55cd4
    {
        [DataContract(Name = "Item", Namespace = "")]
        public class Item
        {
            [DataMember]
            public string Name { get; set; }
            [DataMember]
            public object Data { get; set; }
        }
        public static void Test()
        {
            List<Item> l = new List<Item>
            {
                new Item { Name = "Name", Data = "John" },
                new Item { Name = "Age", Data = 33 },
            };
            DataContractSerializer dcs = new DataContractSerializer(l.GetType());
            MemoryStream ms = new MemoryStream();
            dcs.WriteObject(ms, l);
            Console.WriteLine(Encoding.UTF8.GetString(ms.ToArray()));
        }
    }

    // http://stackoverflow.com/q/8274306/751090
    public class StackOverflow_8274306
    {
        public static byte[] SerializeFileACL(string path)
        {
            var acl = File.GetAccessControl(path, AccessControlSections.All);
            var serializer = new BinaryFormatter();

            using (var ms = new MemoryStream())
            {
                serializer.Serialize(ms, acl);
                return ms.ToArray();
            }
        }
        public static void Test()
        {
            byte[] acl = SerializeFileACL(@"c:\temp\log.txt");
            Util.PrintBytes(acl);
        }
    }

    // http://stackoverflow.com/q/8281703/751090
    public class StackOverflow_8281703
    {
        [XmlType(Namespace = "")]
        public class Foo
        {
            [XmlText]
            public string Value { set; get; }
            [XmlAttribute]
            public string id { set; get; }
        }
        public static void Test()
        {
            MemoryStream ms = new MemoryStream();
            XmlSerializer xs = new XmlSerializer(typeof(Foo));
            Foo foo = new Foo { id = "bar", Value = "some value" };
            xs.Serialize(ms, foo);
            Console.WriteLine(Encoding.UTF8.GetString(ms.ToArray()));
        }
    }

    public class InternalProblem_1
    {
        [ServiceContract]
        public class WebService
        {
            [OperationContract]
            [WebGet(UriTemplate = "*", ResponseFormat = WebMessageFormat.Json)]
            public Message AllUris(Message message)
            {
                WebOperationContext context = WebOperationContext.Current;
                IncomingWebRequestContext incomingContext = context.IncomingRequest;

                string uri = incomingContext.UriTemplateMatch.RequestUri.ToString();
                Console.WriteLine("Request to {0}.", uri);

                if (incomingContext.Method != "GET")
                {
                    Console.WriteLine(
                        "Incoming {0} request:\n{1}\n",
                        incomingContext.Method,
                        message.GetReaderAtBodyContents().ReadOuterXml());
                }
                else
                {
                    Console.WriteLine("Incoming GET request.");
                }

                NameValueCollection query = incomingContext.UriTemplateMatch.QueryParameters;
                if (query.Count > 0)
                {
                    Console.WriteLine("Query parameters:");
                    foreach (var queryParameterName in query.AllKeys)
                    {
                        Console.WriteLine("{0} = \"{1}\"", queryParameterName, query[queryParameterName]);
                    }
                }

                Message response = Message.CreateMessage(
                    MessageVersion.None, 
                    "*", 
                    "Response string: JSON format specified", 
                    new DataContractJsonSerializer(typeof(string)));
                response.Properties.Add(
                    WebBodyFormatMessageProperty.Name, 
                    new WebBodyFormatMessageProperty(WebContentFormat.Json));
                OutgoingWebResponseContext responseContext = context.OutgoingResponse;
                responseContext.Headers["X-CustomHeader"] = "Value3";
                return response;
            }
        }

        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            WebServiceHost host = new WebServiceHost(typeof(WebService), new Uri(baseAddress));
            host.Open();
            Console.WriteLine("Host opened");

            WebClient c = new WebClient();
            Console.WriteLine(c.DownloadString(baseAddress + "/Add?x=6&y=8"));

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    class TransportCredentialOnlyTest
    {
        [ServiceContract]
        interface ISecureService
        {
            [OperationContract]
            string Method1(string request);
        }

        [ServiceBehavior]
        class SecureService : ISecureService
        {
            [PrincipalPermission(SecurityAction.Demand, Role = "everyone")]
            public string Method1(string request)
            {
                return String.Format("Hello, \"{0}\"", Thread.CurrentPrincipal.Identity.Name);
            }
        }
        static Binding GetBinding()
        {
            BasicHttpBinding result = new BasicHttpBinding();
            result.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
            result.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
            return result;
        }
        class CustomAuthorizationPolicy : IAuthorizationPolicy
        {
            string id = Guid.NewGuid().ToString();

            public string Id
            {
                get { return this.id; }
            }

            public ClaimSet Issuer
            {
                get { return ClaimSet.System; }
            }

            public bool Evaluate(EvaluationContext context, ref object state)
            {
                object obj;
                if (!context.Properties.TryGetValue("Identities", out obj))
                    return false;

                IList<IIdentity> identities = obj as IList<IIdentity>;
                if (obj == null || identities.Count <= 0)
                    return false;

                context.Properties["Principal"] = new CustomPrincipal(identities[0]);
                return true;
            }
        }

        class CustomPrincipal : IPrincipal
        {
            IIdentity identity;
            public CustomPrincipal(IIdentity identity)
            {
                this.identity = identity;
            }

            public IIdentity Identity
            {
                get { return this.identity; }
            }

            public bool IsInRole(string role)
            {
                return true;
            }
        }
        class MyPasswordValidator : UserNamePasswordValidator
        {
            public override void Validate(string userName, string password)
            {
                if (userName != password)
                {
                    throw new SecurityTokenException("Unauthorized");
                }
            }
        }
        public static void Test()
        {
            Uri serviceUri = new Uri("http://" + Environment.MachineName + ":8000/Service");
            ServiceHost host = new ServiceHost(typeof(SecureService));
            host.AddServiceEndpoint(typeof(ISecureService), GetBinding(), serviceUri);
            List<IAuthorizationPolicy> policies = new List<IAuthorizationPolicy>();
            policies.Add(new CustomAuthorizationPolicy());
            host.Authorization.ExternalAuthorizationPolicies = policies.AsReadOnly();
            host.Authorization.PrincipalPermissionMode = PrincipalPermissionMode.Custom;

            host.Credentials.UserNameAuthentication.UserNamePasswordValidationMode = UserNamePasswordValidationMode.Custom;
            host.Credentials.UserNameAuthentication.CustomUserNamePasswordValidator = new MyPasswordValidator();

            host.Open();

            EndpointAddress sr = new EndpointAddress(serviceUri);
            ChannelFactory<ISecureService> cf = new ChannelFactory<ISecureService>(GetBinding(), sr);
            cf.Credentials.UserName.UserName = "John Doe";
            cf.Credentials.UserName.Password = "John Doe";
            ISecureService client = cf.CreateChannel();
            Console.WriteLine("Client received response from Method1: {0}", client.Method1("hello"));
            ((IChannel)client).Close();
            Console.ReadLine();
            host.Close();
        }
    }

    // http://blogs.msdn.com/b/carlosfigueira/archive/2011/04/19/wcf-extensibility-message-inspectors.aspx
    public class BlogPost_MessageInspectors_20111226
    {
        [ServiceContract]
        public class Service
        {
            [WebGet]
            public int AddGet(int x, int y) { return x + y; }
            [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
            public int AddPost(int x, int y) { return x + y; }
        }
        class MyBehavior : WebHttpBehavior, IDispatchMessageInspector
        {
            public override void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
            {
                base.ApplyDispatchBehavior(endpoint, endpointDispatcher);
                endpointDispatcher.DispatchRuntime.MessageInspectors.Add(this);
            }

            public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
            {
                Console.WriteLine(request.Headers.To);
                HttpRequestMessageProperty prop = request.Properties[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty;
                foreach (var header in prop.Headers.AllKeys)
                {
                    Console.WriteLine("{0}: {1}", header, prop.Headers[header]);
                }
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
            host.AddServiceEndpoint(typeof(Service), new WebHttpBinding(), "").Behaviors.Add(new MyBehavior());
            host.Open();
            Console.WriteLine("Host opened");

            WebClient c = new WebClient();
            Console.WriteLine(c.DownloadString(baseAddress + "/AddGet?x=2&y=5"));

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_fa34384b_9ffb_45f8_a59d_bfef91da4c52
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
        static Binding GetBinding()
        {
            var result = new WSHttpBinding(SecurityMode.None);
            //Change binding settings here
            return result;
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), GetBinding(), "");

            ServiceBehaviorAttribute sba = host.Description.Behaviors.Find<ServiceBehaviorAttribute>();
            if (sba == null)
            {
                sba = new ServiceBehaviorAttribute();
                host.Description.Behaviors.Add(sba);
            }

            sba.UseSynchronizationContext = false;

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

    public class MyCode_3
    {
        public class TranBalance
        {
            public float Commission;
            public float Deposit;
            public float Freeze;
        }
        public class Transaction
        {
            public TranBalance AfterTranBalance;
            public string AgentId;
            public float Amount;
            public TranBalance BeforeTranBalance;
            public string Channel;
            public string OrigSystemTransTimeStamp;
            public string OrigSysTransNo;
            public string RegionCode;
            public string TranType;
        }
        public class Header
        {
            public DateTime OriginalRequestDateTime;
            public string OriginalRequestId;
            public int PageNumber;
            public int TotalPages;
        }
        public class Result
        {
            public string ReturnCode;
            public string ReturnDescription;
        }
        public class TransactionResponse
        {
            public Header Header;
            public Result Result;
        }
        private static List<Transaction> GetTransactionResponseTransactions(int numberOfTransactions)
        {
            // Create the transactions
            List<Transaction> transactions = new List<Transaction>(numberOfTransactions);

            for (int i = 0; i < numberOfTransactions; i++)
            {
                Transaction transaction = new Transaction()
                {
                    AfterTranBalance =
                        new TranBalance() { Commission = 100F, Deposit = 200F, Freeze = 300F },
                    AgentId = "100",
                    Amount = 400F,
                    BeforeTranBalance =
                        new TranBalance() { Commission = 1F, Deposit = 2F, Freeze = 3F },
                    Channel = "10001",
                    OrigSystemTransTimeStamp = DateTime.Now.ToString(),
                    OrigSysTransNo = "12345",
                    RegionCode = "LeftRegion",
                    TranType = "Commission"
                };
                transactions.Add(transaction);
            }

            return transactions;
        }
        static TransactionResponse GetTransactionResponse()
        {
            TransactionResponse transactionResponse = new TransactionResponse();

            // Create the Header
            transactionResponse.Header = new Header();
            transactionResponse.Header.OriginalRequestDateTime = DateTime.Now;
            transactionResponse.Header.OriginalRequestId = Guid.NewGuid().ToString();
            transactionResponse.Header.PageNumber = 1;
            transactionResponse.Header.TotalPages = 100;

            // Create the result
            transactionResponse.Result = new Result();
            transactionResponse.Result.ReturnCode = "0";
            transactionResponse.Result.ReturnDescription = "SUCCESS";

            return transactionResponse;
        }
        public static void Test()
        {
            TransactionResponse response = GetTransactionResponse();
            int numberOfTransactions = 3;
            using (FileStream fs = File.Create("a.json"))
            {
                using (XmlDictionaryWriter w = JsonReaderWriterFactory.CreateJsonWriter(fs))
                {
                    w.WriteStartElement("root");
                    w.WriteAttributeString("type", "object");
                    WriteHeader(w, response.Header);
                    WriteStartResult(w, response.Result);

                    for (int i = 0; i < 10; i++)
                    {
                        List<Transaction> transactions = GetTransactionResponseTransactions(numberOfTransactions);
                        WriteTransactions(w, transactions);
                    }

                    w.WriteEndElement(); // Result
                    w.WriteEndElement(); // root
                }
            }
        }
        private static void WriteTransactions(XmlDictionaryWriter writer, List<Transaction> transactions)
        {
            DataContractJsonSerializer dcjs = new DataContractJsonSerializer(typeof(List<Transaction>), "Transactions");
            dcjs.WriteObject(writer, transactions);
        }
        private static void WriteStartResult(XmlDictionaryWriter writer, Result result)
        {
            DataContractJsonSerializer dcjs = new DataContractJsonSerializer(typeof(Result), "Result");
            dcjs.WriteStartObject(writer, result);
            dcjs.WriteObjectContent(writer, result);
        }
        static void WriteHeader(XmlDictionaryWriter writer, Header header)
        {
            DataContractJsonSerializer dcjs = new DataContractJsonSerializer(typeof(Header), "Header");
            dcjs.WriteObject(writer, header);
        }
    }

    // http://stackoverflow.com/q/8669406/751090
    public class StackOverflow_8669406
    {
        public class GetOrdersMessageFormatter : IClientMessageFormatter
        {
            readonly IClientMessageFormatter original;

            public GetOrdersMessageFormatter(IClientMessageFormatter actual)
            {
                original = actual;
            }

            public void AddArrayNamespace(XmlNode node)
            {
                if (node != null)
                {
                    var attribute = node.OwnerDocument.CreateAttribute("test");
                    attribute.Value = "test";
                    node.Attributes.Append(attribute);
                }
            }

            public object DeserializeReply(Message message, object[] parameters)
            {
                return original.DeserializeReply(message, parameters);
            }

            public Message SerializeRequest(MessageVersion messageVersion, object[] parameters)
            {
                Message newMessage = null;

                var message = original.SerializeRequest(messageVersion, parameters);

                if (message.Headers.Action == "urn:Mage_Api_Model_Server_HandlerAction")
                {
                    var doc = new XmlDocument();

                    using (var reader = message.GetReaderAtBodyContents())
                    {
                        doc.Load(reader);
                    }

                    if (doc.DocumentElement != null)
                    {
                        switch (doc.DocumentElement.LocalName)
                        {
                            case "call":
                                AddArrayNamespace(doc.SelectSingleNode("//args"));
                                break;
                        }
                    }

                    var ms = new MemoryStream();

                    XmlWriterSettings ws = new XmlWriterSettings
                    {
                        CloseOutput = false,
                    };

                    using (var xw = XmlWriter.Create(ms, ws))
                    {
                        doc.Save(xw);
                        xw.Flush();
                    }

                    Console.WriteLine(Encoding.UTF8.GetString(ms.ToArray()));

                    ms.Position = 0;
                    var xr = XmlReader.Create(ms);
                    newMessage = Message.CreateMessage(message.Version, null, xr);
                    newMessage.Headers.CopyHeadersFrom(message);
                    newMessage.Properties.CopyProperties(message.Properties);
                }

                return newMessage;
            }
        }

        [ServiceContract(Namespace = "")]
        public interface ITest
        {
            [OperationContract(Action = "urn:Mage_Api_Model_Server_HandlerAction")]
            int call(string args);
        }
        public class Service : ITest
        {
            public int call(string args)
            {
                return int.Parse(args);
            }
        }
        class MyBehavior : IOperationBehavior
        {
            public void AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters)
            {
            }

            public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
            {
                clientOperation.Formatter = new GetOrdersMessageFormatter(clientOperation.Formatter);
            }

            public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
            {
            }

            public void Validate(OperationDescription operationDescription)
            {
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
            foreach (OperationDescription operation in factory.Endpoint.Contract.Operations)
            {
                operation.Behaviors.Add(new MyBehavior());
            }

            ITest proxy = factory.CreateChannel();
            Console.WriteLine(proxy.call("4455"));

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class NoSecurityWSHttpBinding
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
            var result = new WSHttpBinding(SecurityMode.None);
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
            Console.WriteLine(proxy.Add(4, 5));

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_aa531fac_f9e6_4e24_8f76_780cfec6cb87
    {
        [ServiceContract]
        public class Service
        {
            [WebInvoke(Method = "PUT", UriTemplate = "Graphs/{library}/{subjectLocalPart}/{predicateLocalPart}/{objectPart}/{languageCode}")]
            public string CreateTriple(string library, string subjectLocalPart, string predicateLocalPart, string objectPart, string languageCode)
            {
                return string.Format("{0}-{1}-{2}-{3}-{4}", library, subjectLocalPart, predicateLocalPart, objectPart, languageCode);
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            WebServiceHost host = new WebServiceHost(typeof(Service), new Uri(baseAddress));
            host.Open();
            Console.WriteLine("Host opened");

            Console.WriteLine("No '#'");
            Util.SendRequest(baseAddress + "/Graphs/myLib/123abc/content-HasA/456def-ghik/en-us", "PUT", "application/json", "0");

            Console.WriteLine("Simple '#' (encoded)");
            Util.SendRequest(baseAddress + "/Graphs/myLib/123abc/content%23HasA/456def%23ghik/en-us", "PUT", "application/json", "0");

            Console.WriteLine("Escaped '#'");
            Util.SendRequest(baseAddress + "/Graphs/myLib/123abc/content%2523HasA/456def%2523ghik/en-us", "PUT", "application/json", "0");

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_8e4e4812_9f8a_4a3a_b1af_b8e14b2af3d4
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            [Description("Returns the summary for a email report")]
            [WebInvoke(Method = "POST", UriTemplate = "{Category}/Report/")]
            [XmlSerializerFormat]
            XmlDocument GetReport(string Category, XmlElement xmlData);
        }
        public class Service : ITest
        {
            public XmlDocument GetReport(string Category, XmlElement xmlData)
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlData.OuterXml);
                return doc;
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            WebHttpBehavior behavior = new WebHttpBehavior { HelpEnabled = true };
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), new WebHttpBinding(), "").Behaviors.Add(behavior);
            host.Open();
            Console.WriteLine("Host opened");

            string input = "<root><foo>bar</foo><list><item id='1'>Bread</item><item id='2'>Milk</item></list></root>";
            WebClient c = new WebClient();
            c.Headers[HttpRequestHeader.ContentType] = "text/xml";
            Console.WriteLine(c.UploadString(baseAddress + "/Cat/Report/", input));

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_f2a70b26_fb1c_4c6a_a48a_646b2dd70760
    {
        [DataContract]
        public class Employee
        {
            [DataMember]
            public string Name { get; set; }
            [DataMember]
            public string Address { get; set; }
            [IgnoreDataMember]
            public int Age { get; set; }

            public string Nickname { get; set; }
            [DataMember]
            public List<Order> theOrders { get; set; }
            [DataMember]
            public List<Payment> thePayments { get; set; }
        }
        public class Order { }
        public class Payment { }
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            Employee Echo(Employee input);
        }
        public class Service : ITest
        {
            public Employee Echo(Employee input)
            {
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
            Console.WriteLine("Host opened at {0}", baseAddress);

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    // http://stackoverflow.com/q/8951319/751090
    public class StackOverflow_8951319
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            string Echo(string text);
            [OperationContract, XmlSerializerFormat]
            XmlDocument GetDocument();
        }
        public class Service : ITest
        {
            public string Echo(string text)
            {
                return text;
            }

            public XmlDocument GetDocument()
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(@"<products>
      <product id='1'>
        <name>Bread</name>
      </product>
      <product id='2'>
        <name>Milk</name>
      </product>
      <product id='3'>
        <name>Coffee</name>
      </product>
    </products>");
                return doc;
            }
        }
        static Binding GetBinding()
        {
            var result = new WSHttpBinding(SecurityMode.None);
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

            Console.WriteLine(proxy.GetDocument().OuterXml);

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_bcd94cf9_2881_4081_a05b_771a6e6f9c06
    {
        public class LogMessageAttribute : Attribute, IOperationBehavior
        {
            public void AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters)
            {
            }

            public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
            {
            }

            public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
            {
                LoggerInspector inspector = dispatchOperation.Parent.MessageInspectors
                    .Where(x => x is LoggerInspector)
                    .FirstOrDefault() as LoggerInspector;

                if (inspector != null)
                {
                    inspector.AddOperation(operationDescription);
                }
                else
                {
                    inspector = new LoggerInspector(operationDescription);
                    dispatchOperation.Parent.MessageInspectors.Add(inspector);
                }
            }

            public void Validate(OperationDescription operationDescription)
            {
            }
        }

        class LoggerInspector : IDispatchMessageInspector
        {
            List<string> operationsToLog = new List<string>();
            public LoggerInspector(OperationDescription operation)
            {
                this.AddOperation(operation);
            }

            internal void AddOperation(OperationDescription operation)
            {
                this.operationsToLog.Add(operation.Messages[0].Action);
            }

            public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
            {
                if (this.operationsToLog.Contains(request.Headers.Action))
                {
                    Console.WriteLine("Request: {0}", request);
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public void BeforeSendReply(ref Message reply, object correlationState)
            {
                bool toLog = (bool)correlationState;
                if (toLog)
                {
                    Console.WriteLine("Reply: {0}", reply);
                }
            }
        }

        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            string Echo(string text);

            [OperationContract]
            [LogMessage]
            int Add(int x, int y);
            
            [OperationContract]
            [LogMessage]
            int Subtract(int x, int y);
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

            public int Subtract(int x, int y)
            {
                return x - y;
            }
        }
        static Binding GetBinding()
        {
            var result = new WSHttpBinding(SecurityMode.None);
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
            Console.WriteLine(proxy.Add(4, 5));
            Console.WriteLine(proxy.Subtract(4, 5));

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_b5b4dd05_50cf_4799_a892_be706868f5a7
    {
        [ServiceContract]
        public interface ISecureCalculator
        {
            [OperationContract, WebGet]
            int Add(int x, int y);
            [OperationContract, WebGet]
            int Subtract(int x, int y);
        }
        public class SecureCalculatorService : ISecureCalculator
        {
            public int Add(int x, int y)
            {
                return x + y;
            }

            public int Subtract(int x, int y)
            {
                return x - y;
            }
        }
        class MyApiKeyInspector : IEndpointBehavior, IDispatchMessageInspector
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
                int index = endpointDispatcher.DispatchRuntime.ChannelDispatcher.ErrorHandlers.Count - 1;
                IErrorHandler webErrorHandler = endpointDispatcher.DispatchRuntime.ChannelDispatcher.ErrorHandlers[index];
                endpointDispatcher.DispatchRuntime.ChannelDispatcher.ErrorHandlers[index] = new MyErrorHandler(webErrorHandler);
            }

            public void Validate(ServiceEndpoint endpoint)
            {
            }

            public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
            {
                HttpRequestMessageProperty prop;
                prop = (HttpRequestMessageProperty)request.Properties[HttpRequestMessageProperty.Name];
                string apiKey = prop.Headers["X-ApiKey"];
                if (apiKey != null && apiKey.StartsWith("TheSecretKey:"))
                {
                    string user = apiKey.Substring("TheSecretKey:".Length);
                    GenericPrincipal principal = new GenericPrincipal(new GenericIdentity(user), new string[] { "user" });
                    request.Properties.Add("Principal", principal);
                }
                else
                {
                    throw new UnauthorizedAccessException();
                }

                return null;
            }

            public void BeforeSendReply(ref Message reply, object correlationState)
            {
            }
        }

        class MyErrorHandler : IErrorHandler
        {
            IErrorHandler original;
            public MyErrorHandler(IErrorHandler original)
            {
                this.original = original;
            }

            public bool HandleError(Exception error)
            {
                return error is UnauthorizedAccessException
                    || this.original.HandleError(error);
            }

            public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
            {
                if (error is UnauthorizedAccessException)
                {
                    fault = Message.CreateMessage(version, "action", (object)null);
                    HttpResponseMessageProperty prop = new HttpResponseMessageProperty();
                    prop.StatusCode = HttpStatusCode.Unauthorized;
                    prop.SuppressEntityBody = true; 
                    fault.Properties.Add(HttpResponseMessageProperty.Name, prop);
                }
                else
                {
                    this.original.ProvideFault(error, version, ref fault);
                }
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(SecureCalculatorService), new Uri(baseAddress));
            ServiceEndpoint webEndpoint = host.AddServiceEndpoint(typeof(ISecureCalculator), new WebHttpBinding(), "web");
            webEndpoint.Behaviors.Add(new WebHttpBehavior());
            webEndpoint.Behaviors.Add(new MyApiKeyInspector());

            ServiceEndpoint wsEndpoint = host.AddServiceEndpoint(typeof(ISecureCalculator), new WSHttpBinding(), "ws");
            host.Open();
            Console.WriteLine("Host opened");

            Util.SendRequest(baseAddress + "/web/Add?x=4&y=8", "GET", null, null);
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("X-ApiKey", "TheSecretKey:JohnDoe");
            Util.SendRequest(baseAddress + "/web/Subtract?x=4&y=8", "GET", null, null, headers);

            ChannelFactory<ISecureCalculator> factory = new ChannelFactory<ISecureCalculator>(new WSHttpBinding(), new EndpointAddress(baseAddress + "/ws"));
            ISecureCalculator proxy = factory.CreateChannel();
            Console.WriteLine(proxy.Add(4, 5));
            Console.WriteLine(proxy.Subtract(4, 5));

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    // http://stackoverflow.com/q/9153898/751090
    public class StackOverflow_9153898
    {
        [ServiceContract]
        public class Service
        {
            [WebInvoke(UriTemplate = "/", Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
            public Stream ProcessPost(string p1, string p2, string p3, string p4)
            {
                return Execute(p1, p2, p3, p4);
            }

            private Stream Execute(string p1, string p2, string p3, string p4)
            {
                WebOperationContext.Current.OutgoingResponse.ContentType = "text/plain";
                string response = p1 + "-" + p2 + "-" + p3 + "-" + p4;
                return new MemoryStream(Encoding.UTF8.GetBytes(response));
            }
        }

        public class MyFormsUrlEncodedBehavior : IEndpointBehavior
        {
            public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
            {
            }

            public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
            {
            }

            public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
            {
                foreach (OperationDescription operationDescription in endpoint.Contract.Operations)
                {
                    var dispatchOperation = endpointDispatcher.DispatchRuntime.Operations[operationDescription.Name];
                    dispatchOperation.Formatter = new MyFormsUrlEncodedDispatchFormatter(operationDescription, dispatchOperation.Formatter);
                }
            }

            public void Validate(ServiceEndpoint endpoint)
            {
            }
        }

        class MyFormsUrlEncodedDispatchFormatter : IDispatchMessageFormatter
        {
            OperationDescription operation;
            IDispatchMessageFormatter originalFormatter;

            public MyFormsUrlEncodedDispatchFormatter(OperationDescription operation, IDispatchMessageFormatter originalFormatter)
            {
                this.operation = operation;
                this.originalFormatter = originalFormatter;
            }

            public void DeserializeRequest(Message message, object[] parameters)
            {
                var reqProp = message.Properties[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty;
                if (reqProp.Headers[HttpRequestHeader.ContentType] == "application/x-www-form-urlencoded")
                {
                    var bodyReader = message.GetReaderAtBodyContents();
                    var bodyBytes = bodyReader.ReadElementContentAsBase64();
                    var body = Encoding.UTF8.GetString(bodyBytes);
                    NameValueCollection pairs = HttpUtility.ParseQueryString(body);
                    DeserializeParameters(pairs, parameters);
                    return;
                }

                this.originalFormatter.DeserializeRequest(message, parameters);
            }

            private void DeserializeParameters(NameValueCollection pairs, object[] parameters)
            {
                foreach (var part in this.operation.Messages[0].Body.Parts)
                {
                    string name = part.Name;
                    string value = pairs[name];
                    switch (Type.GetTypeCode(part.Type))
                    {
                        case TypeCode.Boolean:
                            parameters[part.Index] = Convert.ToBoolean(value);
                            break;
                        case TypeCode.Byte:
                            parameters[part.Index] = Convert.ToByte(value);
                            break;
                        case TypeCode.Char:
                            parameters[part.Index] = Convert.ToChar(value);
                            break;
                        // Skipped many others
                        case TypeCode.String:
                            parameters[part.Index] = value;
                            break;
                        default:
                            throw new NotImplementedException("Not implemented for type " + part.Type);
                    }
                }
            }

            public Message SerializeReply(MessageVersion messageVersion, object[] parameters, object result)
            {
                return this.originalFormatter.SerializeReply(messageVersion, parameters, result);
            }
        }

        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new WebServiceHost(typeof(Service), new Uri(baseAddress));
            ServiceEndpoint endpoint = host.AddServiceEndpoint(typeof(Service), new WebHttpBinding(), "");
            endpoint.Behaviors.Add(new WebHttpBehavior());
            endpoint.Behaviors.Add(new MyFormsUrlEncodedBehavior());
            host.Open();
            Console.WriteLine("Host opened");

            WebClient c = new WebClient();
            c.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            Console.WriteLine(c.UploadString(baseAddress + "/", "p1=str1&p2=str2&p3=str3&p4=str4"));

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_e8019882_3424_40bd_a13a_97a7de9cf51e
    {
        [ServiceContract]
        public class Service
        {
            [OperationContract]
            [WebInvoke(Method = "GET",
                 ResponseFormat = WebMessageFormat.Json,
                UriTemplate = "Event")]
            public WrappedResponse selectEvent()
            {
                WrappedResponse result = new WrappedResponse
                {
                    success = true,
                    message = "Loaded data",
                    data = new List<Event>
                    {
                        new Event
                        {
                            id = 1,
                            cid = 1,
                            title = "Test Event",
                            start = "11/02/2012 15:30:27",
                            end ="11/02/2012 17:30:27",
                            notes= "Don't forget the tickets",
                            url= "",
                            ad = false,
                            loc = "Pune",
                            rem = "NA"
                        }
                    }
                };

                return result;
            }
        }
        [DataContract(Namespace = "")]
        public class Event
        {
            [DataMember(Order = 0)]
            public int id { get; set; }
            [DataMember(Order = 1)]
            public int cid { get; set; }
            [DataMember(Order = 2)]
            public string title { get; set; }
            [DataMember(Order = 3)]
            public string start { get; set; }
            [DataMember(Order = 4)]
            public string end { get; set; }
            [DataMember(Order = 5)]
            public string notes { get; set; }
            [DataMember(Order = 6)]
            public string url { get; set; }
            [DataMember(Order = 7)]
            public Boolean ad { get; set; }
            [DataMember(Order = 8)]
            public string loc { get; set; }
            [DataMember(Order = 9)]
            public string rem { get; set; }
        }
        [DataContract]
        public class WrappedResponse
        {
            [DataMember(Order = 1)]
            public bool success;
            [DataMember(Order = 2)]
            public string message;
            [DataMember(Order = 3)]
            public List<Event> data;
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            WebServiceHost host = new WebServiceHost(typeof(Service), new Uri(baseAddress));
            host.Open();
            Console.WriteLine("Host opened");

            WebClient c = new WebClient();
            Console.WriteLine(c.DownloadString(baseAddress + "/Event"));

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    // http://stackoverflow.com/q/9342355/751090
    public class StackOverflow_9342355
    {
        public static void Test()
        {
            string output = "test: {0} matches";
            string test = "AND [Field] =";

            Regex r = new Regex(@"(AND|OR)\s\[?\w+\]?\s?=");

            if (r.IsMatch(test))
                Console.WriteLine(string.Format(output, test));

            test = "OR [Field] =";
            if (r.IsMatch(test))
                Console.WriteLine(string.Format(output, test));
        }
    }

    public class Post_7904f704_dd4c_4a13_b352_657412ffff6e
    {
        [ServiceContract(
            Name = "ICashierCallback",
            Namespace = "WEBCashier")]
        public interface ICashierCallback
        {
            [OperationContract(IsOneWay = true)]
            void CallbackFunction(double result);
        }
        [ServiceContract(
            CallbackContract = typeof(ICashierCallback),
            Namespace = "WEBCashier",
            Name = "ICashierService",
            SessionMode = SessionMode.Required)]
        public interface ICashierService
        {
            [OperationContract]
            string GetData(int value);
        }
        public class CashierService : ICashierService
        {
            public string GetData(int value)
            {
                return "XXXXXXXXXXX";
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(CashierService), new Uri(baseAddress));
            WSDualHttpBinding binding = new WSDualHttpBinding(WSDualHttpSecurityMode.None);
            host.AddServiceEndpoint(typeof(ICashierService), binding, "");
            host.Description.Behaviors.Add(new ServiceMetadataBehavior { HttpGetEnabled = true });
            host.Open();
            Console.WriteLine("Host opened");
            Console.WriteLine("Press ENTER to close");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_fdb4ea47_18e3_4169_8e1d_0d1ca2b449dc
    {
        [ServiceContract]
        public interface ITest
        {
            [WebGet(ResponseFormat = WebMessageFormat.Json)]
            string Echo(string text);
        }
        public class Service : ITest
        {
            public string Echo(string text)
            {
                if (text == "exception")
                {
                    throw new ArgumentException("Simple exception");
                }
                else if (text == "nestedException")
                {
                    throw new ArgumentException("Outer exception",
                        new InvalidOperationException("Inner exception 1",
                            new ArgumentOutOfRangeException("text", "Inner exception 2")));
                }
                else
                {
                    return text;
                }
            }
        }
        class MyErrorHandler : IEndpointBehavior, IErrorHandler
        {
            public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
            {
            }

            public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
            {
            }

            public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
            {
                endpointDispatcher.ChannelDispatcher.ErrorHandlers.Add(this);
            }

            public void Validate(ServiceEndpoint endpoint)
            {
            }

            public bool HandleError(Exception error)
            {
                return true;
            }

            public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("{0}: {1}", error.GetType().FullName, error.Message);
                Exception inner = error.InnerException;
                while (inner != null)
                {
                    sb.AppendLine();
                    sb.AppendFormat("    {0}: {1}", inner.GetType().FullName, inner.Message);
                    inner = inner.InnerException;
                }

                fault = Message.CreateMessage(version, null, new MyBodyWriter(sb.ToString()));
                fault.Properties.Add(WebBodyFormatMessageProperty.Name, new WebBodyFormatMessageProperty(WebContentFormat.Raw));
            }

            class MyBodyWriter : BodyWriter
            {
                string text;
                public MyBodyWriter(string text)
                    : base(true)
                {
                    this.text = text;
                }

                protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
                {
                    writer.WriteStartElement("Binary");
                    byte[] bytes = Encoding.UTF8.GetBytes(this.text);
                    writer.WriteBase64(bytes, 0, bytes.Length);
                    writer.WriteEndElement();
                }
            }
        }
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            ServiceEndpoint endpoint = host.AddServiceEndpoint(typeof(ITest), new WebHttpBinding(), "");
            endpoint.Behaviors.Add(new WebHttpBehavior());
            endpoint.Behaviors.Add(new MyErrorHandler());
            host.Open();
            Console.WriteLine("Host opened");

            Util.SendRequest(baseAddress + "/Echo?text=no+exception", "GET", null, null);
            Util.SendRequest(baseAddress + "/Echo?text=exception", "GET", null, null);
            Util.SendRequest(baseAddress + "/Echo?text=nestedException", "GET", null, null);

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_334b9b23_950c_4b58_b120_b19dc9478ad5
    {
        [ServiceContract]
        public class Service
        {
            [WebGet]
            public int Add(int x, int y)
            {
                return x + y;
            }

            [WebGet]
            public int Subtract(int x, int y)
            {
                return x - y;
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
                HttpRequestMessageProperty prop;
                prop = request.Properties[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty;
                if (prop != null)
                {
                    if (prop.Method == "GET")
                    {
                        NameValueCollection nvc = HttpUtility.ParseQueryString(prop.QueryString);
                        string callback = nvc["callback"];
                        if (!string.IsNullOrEmpty(callback))
                        {
                            Regex letterNumberOnly = new Regex(@"^[0-9a-zA-Z]+$");
                            if (!letterNumberOnly.IsMatch(callback))
                            {
                                throw new Exception("This will abort the request, causing a 400 to be returned to the client");
                            }
                        }
                    }
                }

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
            WebHttpBinding binding = new WebHttpBinding();
            binding.CrossDomainScriptAccessEnabled = true;
            ServiceEndpoint endpoint = host.AddServiceEndpoint(typeof(Service), binding, "");
            WebHttpBehavior behavior = new WebHttpBehavior();
            behavior.DefaultOutgoingResponseFormat = WebMessageFormat.Json;
            endpoint.Behaviors.Add(behavior);
            endpoint.Behaviors.Add(new MyInspector());
            host.Open();
            Console.WriteLine("Host opened");

            WebClient c;

            c = new WebClient();
            Console.WriteLine("No callback:");
            Console.WriteLine(c.DownloadString(baseAddress + "/Add?x=6&y=8"));
            Console.WriteLine();

            c = new WebClient();
            Console.WriteLine("Valid callback:");
            Console.WriteLine(c.DownloadString(baseAddress + "/Add?x=6&y=8&callback=Func"));
            Console.WriteLine();

            c = new WebClient();
            try
            {
                Console.WriteLine("Invalid callback:");
                Console.WriteLine(c.DownloadString(baseAddress + "/Add?x=6&y=8&callback=Fu-nc"));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            Console.WriteLine();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_5e77450e_7574_4c48_ae64_71e3cf649fbb
    {
        [ServiceContract, XmlSerializerFormat]
        public interface ITest
        {
            [OperationContract]
            void ResidentAddress(int residentID, XmlNode addressData);
        }
        public class Service : ITest
        {
            public void ResidentAddress(int residentID, XmlNode addressData)
            {
                Console.WriteLine("In ResidentAddress: {0}", addressData.OuterXml);
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

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(@"<?xml version=""1.0"" encoding=""utf-8""?>
      <ResidentData xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
       <Address>
            <ResidentAddress>
                 <AddressType>REGULAR</AddressType>
                 <Adr1>123 Main St</Adr1>
                 <Zip>75000</Zip>
                 <Email>nobody@nowhere.com</Email>
             </ResidentAddress>
         </Address>
 </ResidentData>");
            proxy.ResidentAddress(1, doc);

            doc = new XmlDocument();
            doc.AppendChild(doc.CreateElement("ResidentData"));
            doc.DocumentElement.AppendChild(doc.CreateElement("Address"));
            XmlElement residentAddress = doc.CreateElement("ResidentAddress");
            doc.DocumentElement.FirstChild.AppendChild(residentAddress);
            residentAddress.AppendChild(CreateElementWithText(doc, "AddressType", "REGULAR"));
            residentAddress.AppendChild(CreateElementWithText(doc, "Add1", "123 Main St"));
            residentAddress.AppendChild(CreateElementWithText(doc, "Zip", "75000"));
            residentAddress.AppendChild(CreateElementWithText(doc, "Email", "nobody@nowhere.com"));
            proxy.ResidentAddress(2, doc);

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }

        static XmlElement CreateElementWithText(XmlDocument doc, string elementName, string text)
        {
            XmlElement result = doc.CreateElement(elementName);
            result.AppendChild(doc.CreateTextNode(text));
            return result;
        }
    }

    public class Post_938156c7_ccb5_4436_833d_d560b9901750
    {
        public static void Test()
        {
            for (int stringSize = 680; stringSize < 700; stringSize++)
            {
                string str = new string((char)0x6cd5, stringSize);
                string jsonString = "\"" + str + "\"";
                //MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
                //XmlDictionaryReader jsonReader = JsonReaderWriterFactory.CreateJsonReader(ms, XmlDictionaryReaderQuotas.Max);
                byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonString);
                XmlDictionaryReader jsonReader = JsonReaderWriterFactory.CreateJsonReader(jsonBytes, XmlDictionaryReaderQuotas.Max);
                DataContractJsonSerializer dcjs = new DataContractJsonSerializer(typeof(string));
                try
                {
                    // old - string str2 = (string)dcjs.ReadObject(ms);
                    string str2 = (string)dcjs.ReadObject(jsonReader);
                    if (str == str2)
                    {
                        Console.Write(".");
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine("Error, different strings for stringSize = {0}", stringSize);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine();
                    Console.WriteLine("{0}: {1}", e.GetType().FullName, e.Message);
                }
                if ((stringSize % 50) == 49) Console.WriteLine();
            }
        }
    }

    public class Post_ac21a51a_a436_4b89_91d1_29dc6c789487
    {
        [ServiceContract]
        public interface ITest
        {
            [OperationContract]
            string Echo(string text);
        }
        public class Service : ITest, IDisposable
        {
            public Service()
            {
                Console.WriteLine("I'm being created now");
            }

            public string Echo(string text)
            {
                return text;
            }

            public void Dispose()
            {
                Console.WriteLine("I'm being disposed now");
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
            Console.WriteLine("Calling the service");
            string result = proxy.Echo("Hello");
            Console.WriteLine("Result: {0}", result);

            Console.WriteLine();

            Console.WriteLine("Calling the service again");
            result = proxy.Echo("World");
            Console.WriteLine("Result: {0}", result);

            ((IClientChannel)proxy).Close();
            factory.Close();

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }

    public class Post_a72c9431_ce05_43c7_be9e_4aa514c842d7
    {
        public class MyJsonPEnabledWebServiceHostFactory : WebServiceHostFactory
        {
            protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
            {
                ServiceHost host = base.CreateServiceHost(serviceType, baseAddresses);
                foreach (var endpoint in host.Description.Endpoints)
                {
                    WebHttpBinding webBinding = endpoint.Binding as WebHttpBinding;
                    if (webBinding != null)
                    {
                        webBinding.CrossDomainScriptAccessEnabled = true;
                    }
                }

                return host;
            }
        }
    }

    public class Post_4d4bec90_15ee_49d5_810a_eca0c03e6c2d
    {
        public static void Test()
        {
            Compare(MessageVersion.None,
                MessageVersion.CreateVersion(EnvelopeVersion.None, AddressingVersion.None));

            Compare(MessageVersion.Soap11,
                MessageVersion.CreateVersion(EnvelopeVersion.Soap11, AddressingVersion.None));

            Compare(MessageVersion.Soap11WSAddressing10,
                MessageVersion.CreateVersion(EnvelopeVersion.Soap11, AddressingVersion.WSAddressing10));

            Compare(MessageVersion.Soap11WSAddressingAugust2004,
                MessageVersion.CreateVersion(EnvelopeVersion.Soap11, AddressingVersion.WSAddressingAugust2004));

            Compare(MessageVersion.Soap12,
                MessageVersion.CreateVersion(EnvelopeVersion.Soap12, AddressingVersion.None));

            Compare(MessageVersion.Soap12WSAddressing10,
                MessageVersion.CreateVersion(EnvelopeVersion.Soap12, AddressingVersion.WSAddressing10));

            Compare(MessageVersion.Soap12WSAddressingAugust2004,
                MessageVersion.CreateVersion(EnvelopeVersion.Soap12, AddressingVersion.WSAddressingAugust2004));
        }

        static void Compare(MessageVersion mv1, MessageVersion mv2)
        {
            Console.WriteLine(object.ReferenceEquals(mv1, mv2));
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Post_4d4bec90_15ee_49d5_810a_eca0c03e6c2d.Test();
            Console.WriteLine(DateTime.UtcNow);
        }
    }
}
