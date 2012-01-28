Imports System.Collections.ObjectModel
Imports System.IO
Imports System.Net
Imports System.Runtime.Serialization
Imports System.ServiceModel
Imports System.ServiceModel.Channels
Imports System.ServiceModel.Description
Imports System.ServiceModel.Dispatcher
Imports System.ServiceModel.Web
Imports System.ServiceModel.Activation
Imports System.Text
Imports System.Xml
Imports System.ComponentModel
Imports System.Xml.Serialization
Imports System.Xml.Schema

Public Class Post_ed4dee9d_6a49_4bf9_9724_cbf873105e62
    <ServiceContract(Name:="ABCService", Namespace:="http://abc.com/WCFService/")> _
    Public Interface IService
        <OperationContract()> _
        <WebInvoke(UriTemplate:="/Add", BodyStyle:=WebMessageBodyStyle.Bare, requestFormat:=WebMessageFormat.Xml, responseFormat:=WebMessageFormat.Xml, Method:="POST")> _
        Function Add(ByVal sXML As String) As String
    End Interface

    <AspNetCompatibilityRequirements(RequirementsMode:=AspNetCompatibilityRequirementsMode.Allowed)> _
    Public Class Service
        Implements IService
        Public Function Add(ByVal sXML As String) As String Implements IService.Add
            Return "I am getting:" & sXML
        End Function
    End Class

    Public Shared Sub Test()
        Dim baseAddress As String = "http://localhost:8000/Service"
        Dim host As ServiceHost = New ServiceHost(GetType(Service), New Uri(baseAddress))
        host.AddServiceEndpoint(GetType(IService), New WebHttpBinding(), "").Behaviors.Add(New WebHttpBehavior())
        host.Open()
        Console.WriteLine("Host opened")

        Util.SendRequest("POST", baseAddress + "/Add", "text/xml", "<string xmlns=""http://schemas.microsoft.com/2003/10/Serialization/"">This is a string</string>")
    End Sub
End Class

Public Class Post_191798e0_2c9a_447c_b72a_b1ca241146e3
    '<ServiceContract()> _
    Public Interface ITest
        Property MyProp() As Integer
    End Interface
    <ServiceContract()> _
    Public Class MyService
        'Implements ITest

        Shared i As Integer

        'Public Property MyProp() As Integer Implements ITest.MyProp
        Public Property MyProp() As Integer
            <OperationContract()> _
            Get
                Console.WriteLine("[server] get_MyProp")
                Return i
            End Get
            <OperationContract()> _
            Set(ByVal value As Integer)
                Console.WriteLine("[server] set_MyProp")
                i = value
            End Set
        End Property
    End Class

    Public Shared Sub Test()
        Dim baseAddress As String = "http://localhost:8000/Service"
        Dim host As New ServiceHost(GetType(MyService), New Uri(baseAddress))
        host.AddServiceEndpoint(GetType(MyService), New BasicHttpBinding(), "")
        Dim smb As ServiceMetadataBehavior = New ServiceMetadataBehavior
        smb.HttpGetEnabled = True
        host.Description.Behaviors.Add(smb)
        host.Open()

        'Dim factory = New ChannelFactory(Of ITest)(New BasicHttpBinding(), New EndpointAddress(baseAddress))
        'Dim proxy = factory.CreateChannel()
        'proxy.MyProp = 123
        'proxy.MyProp = proxy.MyProp + 1
        'Console.WriteLine(proxy.MyProp)
        Console.WriteLine("Press ENTER to close")
        Console.ReadLine()
    End Sub
End Class

Public Class Post_ed4dee9d_6a49_4bf9_9724_cbf873105e62_AllXml
    <ServiceContract(Name:="ABCService", Namespace:="http://abc.com/WCFService/")> _
    Public Interface IService
        <OperationContractAttribute()> _
        <WebInvoke(UriTemplate:="/Add", BodyStyle:=WebMessageBodyStyle.Bare, requestFormat:=WebMessageFormat.Xml, responseFormat:=WebMessageFormat.Xml, Method:="POST")> _
        Function Add(ByVal sXML As XmlElement) As String
    End Interface

    <AspNetCompatibilityRequirements(RequirementsMode:=AspNetCompatibilityRequirementsMode.Allowed)> _
    Public Class Service
        Implements IService
        Public Function Add(ByVal sXML As XmlElement) As String Implements IService.Add
            Return "I am getting:" & sXML.OuterXml
        End Function
    End Class

    Public Shared Sub Test()
        Dim baseAddress As String = "http://localhost:8000/Service"
        Dim host As ServiceHost = New ServiceHost(GetType(Service), New Uri(baseAddress))
        host.AddServiceEndpoint(GetType(IService), New WebHttpBinding(), "").Behaviors.Add(New WebHttpBehavior())
        host.Open()
        Console.WriteLine("Host opened")

        Util.SendRequest("POST", baseAddress + "/Add", "text/xml", "<names><name>TEST</name></names>")
    End Sub
End Class

Public Class Post_ed4dee9d_6a49_4bf9_9724_cbf873105e62_Anything
    <ServiceContract(Name:="ABCService", Namespace:="http://abc.com/WCFService/")> _
    Public Interface IService
        <OperationContract()> _
        <WebInvoke(UriTemplate:="/Add", BodyStyle:=WebMessageBodyStyle.Bare, requestFormat:=WebMessageFormat.Xml, responseFormat:=WebMessageFormat.Xml, Method:="POST")> _
        Function Add(ByVal sXML As Stream) As Stream
    End Interface

    <AspNetCompatibilityRequirements(RequirementsMode:=AspNetCompatibilityRequirementsMode.Allowed)> _
    Public Class Service
        Implements IService
        Public Function Add(ByVal sXML As Stream) As Stream Implements IService.Add
            Dim result As String = "I am getting: " & New StreamReader(sXML).ReadToEnd
            WebOperationContext.Current.OutgoingResponse.ContentType = "text/plain"
            Return New MemoryStream(Encoding.UTF8.GetBytes(result))
        End Function
    End Class
    Class MyRawMapper
        Inherits WebContentTypeMapper
        Public Overrides Function GetMessageFormatForContentType(ByVal contentType As String) As WebContentFormat
            Return WebContentFormat.Raw
        End Function
    End Class
    Shared Function GetBinding()
        Dim result As CustomBinding = New CustomBinding(New WebHttpBinding())
        result.Elements.Find(Of WebMessageEncodingBindingElement).ContentTypeMapper = New MyRawMapper()
        Return result
    End Function
    Public Shared Sub Test()
        Dim baseAddress As String = "http://localhost:8000/Service"
        Dim host As ServiceHost = New ServiceHost(GetType(Service), New Uri(baseAddress))
        host.AddServiceEndpoint(GetType(IService), GetBinding(), "").Behaviors.Add(New WebHttpBehavior())
        host.Open()
        Console.WriteLine("Host opened")

        Util.SendRequest("POST", baseAddress + "/Add", "text/xml", "<names><name>TEST</name></names>")
        Util.SendRequest("POST", baseAddress + "/Add", "text/plain", "Hello world")
    End Sub
End Class

Public Class Util
    Public Shared Function SendRequest(ByVal method As String, _
                                       ByVal uri As String, _
                                       Optional ByVal contentType As String = Nothing, _
                                       Optional ByVal body As String = Nothing) As String
        Dim responseBody As String = Nothing

        Dim req As HttpWebRequest = CType(HttpWebRequest.Create(uri), HttpWebRequest)
        req.Method = method
        If Not String.IsNullOrEmpty(contentType) Then
            req.ContentType = contentType
        End If
        If Not String.IsNullOrEmpty(body) Then
            Dim bodyBytes As Byte() = Encoding.UTF8.GetBytes(body)
            req.GetRequestStream().Write(bodyBytes, 0, bodyBytes.Length)
            req.GetRequestStream().Close()
        End If

        Dim resp As HttpWebResponse
        Try
            resp = CType(req.GetResponse(), HttpWebResponse)
        Catch e As WebException
            resp = CType(e.Response, HttpWebResponse)
        End Try

        Console.WriteLine("HTTP/{0} {1} {2}", resp.ProtocolVersion, CType(resp.StatusCode, Integer), resp.StatusDescription)
        For Each headerName As String In resp.Headers.AllKeys
            Console.WriteLine("{0}: {1}", headerName, resp.Headers(headerName))
        Next
        Console.WriteLine()
        Dim respStream As Stream = resp.GetResponseStream()
        If respStream IsNot Nothing Then
            responseBody = New StreamReader(respStream).ReadToEnd()
            Console.WriteLine(responseBody)
        Else
            Console.WriteLine("HttpWebResponse.GetResponseStream returned null")
        End If
        Console.WriteLine()
        Console.WriteLine("  *-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*  ")
        Console.WriteLine()

        Return responseBody
    End Function
End Class

Public Class Post_525a3381_3457_4a09_bbd5_57080a56771d
    <ServiceKnownType(GetType(OverflowException)), ServiceContract()> _
    Public Interface IService1
        <OperationContract()> _
        Sub HandleEx(ByVal ex As Exception)
    End Interface

    <Serializable()> _
    Public Class TestException
        Implements IService1
        Public Sub HandleEx(ByVal ex As System.Exception) Implements IService1.HandleEx
            MsgBox(ex.ToString)
        End Sub
    End Class

    Public Shared Sub Test()
        Dim baseAddress As String = "http://" + Environment.MachineName + ":8000/Service"
        Dim host As ServiceHost = New ServiceHost(GetType(TestException), New Uri(baseAddress))
        host.AddServiceEndpoint(GetType(IService1), New BasicHttpBinding(), "")
        host.Open()

        Dim factory As ChannelFactory(Of IService1) = New ChannelFactory(Of IService1)(New BasicHttpBinding(), New EndpointAddress(baseAddress))
        Dim proxy As IService1 = factory.CreateChannel()
        Try
            Dim a As Integer
            a = 0
            a = 100 / a
        Catch ex As Exception
            proxy.HandleEx(ex)
        End Try
    End Sub
End Class

Public Class Post_3396ffba_4e33_40f2_b8e6_9fcb7f0b3a88

    Private Shared allNodes As List(Of String) = New List(Of String)

    <ServiceContract()> Public Interface ITest
        <OperationContract()> Function Add(ByVal x As Integer, ByVal y As Integer) As Integer
    End Interface

    Public Class Service
        Implements ITest

        Public Function Add(ByVal x As Integer, ByVal y As Integer) As Integer Implements ITest.Add
            Return x + y
        End Function
    End Class

    Public Class MyInspector
        Implements IEndpointBehavior
        Implements IClientMessageInspector

        Public Sub AddBindingParameters(ByVal endpoint As ServiceEndpoint, ByVal bindingParameters As BindingParameterCollection) Implements IEndpointBehavior.AddBindingParameters

        End Sub

        Public Sub ApplyClientBehavior(ByVal endpoint As ServiceEndpoint, ByVal clientRuntime As ClientRuntime) Implements IEndpointBehavior.ApplyClientBehavior
            clientRuntime.MessageInspectors.Add(Me)
        End Sub

        Public Sub ApplyDispatchBehavior(ByVal endpoint As ServiceEndpoint, ByVal endpointDispatcher As EndpointDispatcher) Implements IEndpointBehavior.ApplyDispatchBehavior

        End Sub

        Public Sub Validate(ByVal endpoint As ServiceEndpoint) Implements IEndpointBehavior.Validate

        End Sub

        Public Sub AfterReceiveReply(ByRef reply As Message, ByVal correlationState As Object) Implements IClientMessageInspector.AfterReceiveReply
            Dim xdr As XmlDictionaryReader = reply.GetReaderAtBodyContents() ' reply has been read; need to re-create it later
            Dim XMLDoc As XmlDocument = New XmlDocument()
            XMLDoc.Load(xdr)

            'Saving the information in a "global" variable
            For Each node As XmlElement In XMLDoc.ChildNodes
                allNodes.Add(node.InnerText)
            Next

            'Recreating the reply
            Dim memStream As New MemoryStream
            Dim writer As XmlDictionaryWriter = XmlDictionaryWriter.CreateBinaryWriter(memStream)
            XMLDoc.WriteTo(writer)
            writer.Flush()
            memStream.Position = 0
            Dim reader As XmlDictionaryReader = XmlDictionaryReader.CreateBinaryReader(memStream, XmlDictionaryReaderQuotas.Max)
            Dim newReply As Message = Message.CreateMessage(reply.Version, Nothing, reader)
            newReply.Headers.CopyHeadersFrom(reply)
            newReply.Properties.CopyProperties(reply.Properties)

            'Now adding a new property into the "reply"
            newReply.Properties.Add(name:="MyNewProperty", [property]:="Value for my property")

            reply = newReply
        End Sub

        Public Function BeforeSendRequest(ByRef request As Message, ByVal channel As IClientChannel) As Object Implements IClientMessageInspector.BeforeSendRequest
            Return Nothing
        End Function
    End Class

    Public Shared Sub Test()
        Dim baseAddress As String = "http://" + Environment.MachineName + ":8000/Service"
        Dim host As New ServiceHost(GetType(Service), New Uri(baseAddress))
        host.AddServiceEndpoint(GetType(ITest), New BasicHttpBinding(), "")
        Dim smb As ServiceMetadataBehavior = New ServiceMetadataBehavior
        smb.HttpGetEnabled = True
        host.Description.Behaviors.Add(smb)
        host.Open()

        Dim factory = New ChannelFactory(Of ITest)(New BasicHttpBinding(), New EndpointAddress(baseAddress))
        factory.Endpoint.Behaviors.Add(New MyInspector())
        Dim proxy = factory.CreateChannel()
        Using New OperationContextScope(CType(proxy, IContextChannel))
            Console.WriteLine(proxy.Add(1234, 5434))
            Console.WriteLine("All response properties:")
            For Each prop In OperationContext.Current.IncomingMessageProperties
                Console.WriteLine("{0}: {1}", prop.Key, prop.Value)
            Next
        End Using

        Console.WriteLine("All nodes read in AfterReceiveReply:")
        For Each node In allNodes
            Console.WriteLine("  " + node)
        Next
        Console.WriteLine("Press ENTER to close")
        Console.ReadLine()
    End Sub
End Class

' http://stackoverflow.com/q/6186859/751090
Public Class StackOverflow_6186859_751090

    Public Class Declarations
        Public Const SchemaVersion As String = "http://my.namespace"
    End Class

    'BinaryObjectType type 
    '-------------------------------------------------- 
    <XmlType(TypeName:="BinaryObjectType", Namespace:=Declarations.SchemaVersion), Serializable(), _
     EditorBrowsable(EditorBrowsableState.Advanced)> _
    Public Class BinaryObjectType
        <XmlAttribute(AttributeName:="format", Form:=XmlSchemaForm.Unqualified, DataType:="string", Namespace:=Declarations.SchemaVersion), _
             EditorBrowsable(EditorBrowsableState.Advanced)> _
        Public __format As String
        <XmlIgnore()> _
        Public Property format() As String
            Get
                format = __format
            End Get
            Set(ByVal Value As String)
                __format = Value
            End Set
        End Property

        <XmlAttribute(AttributeName:="mimeCode", Form:=XmlSchemaForm.Unqualified, Namespace:=Declarations.SchemaVersion), _
         EditorBrowsable(EditorBrowsableState.Advanced)> _
        Public __mimeCode As String
        <XmlIgnore()> _
        Public Property mimeCode() As String
            Get
                mimeCode = __mimeCode
            End Get
            Set(ByVal Value As String)
                __mimeCode = Value
            End Set
        End Property

        <XmlText(DataType:="base64Binary"), _
         EditorBrowsable(EditorBrowsableState.Advanced)> _
        Public __Value As Byte()
        <XmlIgnore()> _
        Public Property Value() As Byte()
            Get
                Value = __Value
            End Get
            Set(ByVal val As Byte())
                __Value = val
            End Set
        End Property

    End Class

    Public Shared Sub Test()
        Dim ms As MemoryStream = New MemoryStream()
        Dim ser As XmlSerializer = New XmlSerializer(GetType(BinaryObjectType))
        Dim obj As BinaryObjectType = New BinaryObjectType()
        obj.format = "PNG"
        obj.mimeCode = "image/png"
        obj.Value = Encoding.UTF8.GetBytes("Some random text which will be encoded as bytes")
        ser.Serialize(ms, obj)
        Console.WriteLine(Encoding.UTF8.GetString(ms.ToArray()))
    End Sub
End Class

Public Class Post_2cf7cd17_c963_465b_a8ce_3edf5bd0467b
    <ServiceContract()> _
    Public Interface ITest
        <OperationContract(), WebGet()> _
        Function Add(ByVal x As Integer, ByVal y As Integer) As Integer
    End Interface

    <ServiceBehavior(InstanceContextMode:=InstanceContextMode.PerSession, _
        ConcurrencyMode:=ConcurrencyMode.Multiple)> _
    Public Class Service
        Implements ITest

        Private HostInstance As Post_2cf7cd17_c963_465b_a8ce_3edf5bd0467b
        Public Sub New(ByVal hostInstance As Post_2cf7cd17_c963_465b_a8ce_3edf5bd0467b)
            Me.HostInstance = hostInstance
        End Sub

        Public Function Add(ByVal x As Integer, ByVal y As Integer) As Integer Implements ITest.Add
            Me.HostInstance.ServiceCalled(String.Format("Add({0}, {1})", x, y))
            Threading.Thread.Sleep(1000)
            Return x + y
        End Function
    End Class

    Public Class MyInstanceProvider
        Implements IEndpointBehavior, IInstanceProvider

        Private HostInstance As Post_2cf7cd17_c963_465b_a8ce_3edf5bd0467b
        Public Sub New(ByVal hostInstance As Post_2cf7cd17_c963_465b_a8ce_3edf5bd0467b)
            Me.HostInstance = hostInstance
        End Sub

        Public Sub AddBindingParameters(ByVal endpoint As ServiceEndpoint, ByVal bindingParameters As BindingParameterCollection) Implements IEndpointBehavior.AddBindingParameters

        End Sub

        Public Sub ApplyClientBehavior(ByVal endpoint As ServiceEndpoint, ByVal clientRuntime As ClientRuntime) Implements IEndpointBehavior.ApplyClientBehavior

        End Sub

        Public Sub ApplyDispatchBehavior(ByVal endpoint As ServiceEndpoint, ByVal endpointDispatcher As EndpointDispatcher) Implements IEndpointBehavior.ApplyDispatchBehavior
            endpointDispatcher.DispatchRuntime.InstanceProvider = Me
        End Sub

        Public Sub Validate(ByVal endpoint As ServiceEndpoint) Implements IEndpointBehavior.Validate

        End Sub

        Public Function GetInstance(ByVal instanceContext As InstanceContext) As Object Implements IInstanceProvider.GetInstance
            Return New Service(Me.HostInstance)
        End Function

        Public Function GetInstance(ByVal instanceContext As InstanceContext, ByVal message As Message) As Object Implements IInstanceProvider.GetInstance
            Return New Service(Me.HostInstance)
        End Function

        Public Sub ReleaseInstance(ByVal instanceContext As InstanceContext, ByVal instance As Object) Implements IInstanceProvider.ReleaseInstance

        End Sub
    End Class

    Public Sub ServiceCalled(ByVal text As String)
        Console.WriteLine("[host at thread {0}] Service operation called: {1}", Threading.Thread.CurrentThread.ManagedThreadId, text)
    End Sub

    Public Sub HostAndWait()
        Dim baseAddress As String = "http://" + Environment.MachineName + ":8000/Service"
        Dim host As ServiceHost = New ServiceHost(GetType(Service), New Uri(baseAddress))
        Dim endpoint = host.AddServiceEndpoint(GetType(ITest), New WebHttpBinding(), "")
        endpoint.Behaviors.Add(New WebHttpBehavior())
        endpoint.Behaviors.Add(New MyInstanceProvider(Me))
        host.Open()
        Console.WriteLine("Host opened")

        'Dim factory = New ChannelFactory(Of ITest)(New BasicHttpBinding(), New EndpointAddress(baseAddress))
        'Dim proxy = factory.CreateChannel()
        'Console.WriteLine(proxy.Add(5, 7))

        For i As Integer = 1 To 10
            Dim index As Integer = i
            Threading.ThreadPool.QueueUserWorkItem(Sub()
                                                       Dim client = New WebClient()
                                                       Console.WriteLine("Sum: {0}", client.DownloadString(String.Concat(baseAddress, "/Add?x=5&y=", index)))
                                                   End Sub)
        Next

        Console.WriteLine("Press ENTER to close")
        Console.ReadLine()

        'CType(proxy, IClientChannel).Close()
        'factory.Close()
        host.Close()
    End Sub

    Public Shared Sub Test()
        Dim host = New Post_2cf7cd17_c963_465b_a8ce_3edf5bd0467b
        host.HostAndWait()
    End Sub

End Class

'http://stackoverflow.com/q/8143837/751090
Public Class StackOverflow_8143837
    Public Class Person
        Private incrementedID As Integer = 0
        Public ReadOnly Property ID As Integer
            Get
                Return Me.incrementedID
            End Get
        End Property

        Private Shared nextId As Integer = 0

        Public Sub New()
            Me.incrementedID = System.Threading.Interlocked.Increment(nextId)
        End Sub

    End Class
End Class

Public Class Post_b2c090c1_ecd1_478c_bef3_4eef258e109e
    <ServiceContract()> _
    <AspNetCompatibilityRequirements(RequirementsMode:=AspNetCompatibilityRequirementsMode.Required)> _
    Public Class WcfServices
        <WebGet(UriTemplate:="/GetHello", ResponseFormat:=WebMessageFormat.Json)> _
        Public Function GetHello() As String
            Throw New Exception("wcf error!")
            Return "Hello World"
        End Function
    End Class
    Public Class WcfErrorHandler
        Implements IErrorHandler
        Public Function HandleError(ByVal [error] As Exception) As Boolean Implements IErrorHandler.HandleError
            'HANDLE ERR HERE
            Return True
        End Function
        Public Sub ProvideFault(ByVal [error] As Exception, ByVal version As MessageVersion, ByRef fault As Message) Implements IErrorHandler.ProvideFault
        End Sub
    End Class
    Public Class ErrorHandlingAddingBehavior
        Implements IEndpointBehavior

        Public Sub AddBindingParameters(ByVal endpoint As ServiceEndpoint, ByVal bindingParameters As BindingParameterCollection) Implements IEndpointBehavior.AddBindingParameters

        End Sub

        Public Sub ApplyClientBehavior(ByVal endpoint As ServiceEndpoint, ByVal clientRuntime As ClientRuntime) Implements IEndpointBehavior.ApplyClientBehavior

        End Sub

        Public Sub ApplyDispatchBehavior(ByVal endpoint As ServiceEndpoint, ByVal endpointDispatcher As EndpointDispatcher) Implements IEndpointBehavior.ApplyDispatchBehavior
            endpointDispatcher.ChannelDispatcher.ErrorHandlers.Add(New WcfErrorHandler())
        End Sub

        Public Sub Validate(ByVal endpoint As ServiceEndpoint) Implements IEndpointBehavior.Validate

        End Sub
    End Class
    Public Class FactoryAddingErrorHandler
        Inherits WebServiceHostFactory

        Protected Overrides Function CreateServiceHost(ByVal serviceType As Type, ByVal baseAddresses() As System.Uri) As ServiceHost
            Dim host As ServiceHost = MyBase.CreateServiceHost(serviceType, baseAddresses)
            host.Description.Endpoints(0).Behaviors.Add(New ErrorHandlingAddingBehavior())
            Return host
        End Function

    End Class
End Class

Public Class Post_21712729_972a_43c5_a584_73fd90b869dc
    <DataContract()> _
    Public Class FullName
        <DataMember(Name:="forename")> _
        Public Property ForeName As String
        <DataMember(Name:="surname")> _
        Public Property SurName As String
    End Class
    <ServiceContract()> _
    Public Interface ITest
        <WebInvoke(Method:="POST", BodyStyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Json, RequestFormat:=WebMessageFormat.Json)> _
        Function writeData(ByVal acode As String, ByVal uid As String, ByVal timestamp As String, ByVal data As FullName, ByVal route As String) As String
    End Interface

    Public Class Service
        Implements ITest

        Public Function writeData(ByVal acode As String, ByVal uid As String, ByVal timestamp As String, ByVal data As FullName, ByVal route As String) As String Implements ITest.writeData
            Console.WriteLine("acode: {0}", acode)
            Console.WriteLine("uid: {0}", uid)
            Console.WriteLine("timestamp: {0}", timestamp)
            Console.WriteLine("route: {0}", route)
            Console.WriteLine("data.forename: {0}", data.ForeName)
            Console.WriteLine("data.surname: {0}", data.SurName)
            Return "ok"
        End Function
    End Class

    Public Shared Sub Test()
        Dim baseAddress As String = "http://" + Environment.MachineName + ":8000/Service"
        Dim host As WebServiceHost = New WebServiceHost(GetType(Service), New Uri(baseAddress))
        host.Open()
        Console.WriteLine("Host opened")

        Dim client As WebClient = New WebClient()
        client.Headers(HttpRequestHeader.ContentType) = "application/json"
        Dim request As String = "{""timestamp"":""2011-01-01 00:00:00"",""uid"":""abc123xyz""," + _
            """acode"":""1"",""data"":{""forename"":""Test"",""surname"":""Test""},""route"":""eqr""}"
        Console.WriteLine(client.UploadString(baseAddress + "/writeData", request))

        host.Close()
    End Sub
End Class

Public Class Post_9d1a0c6e_8e83_420a_8bd3_7a13cc8eadb4
    <DataContract()> _
    Public Class TheData
        <DataMember()> _
        Property forename As String
        <DataMember()> _
        Property surname As String
        <DataMember(Name:="q-125")> _
        Property Q125 As String
    End Class

    <ServiceContract()> _
    Public Interface ITest
        <WebInvoke()> _
        Function Process(ByVal data As TheData) As String
    End Interface

    Public Class Service
        Implements ITest

        Public Function Process(ByVal data As TheData) As String Implements ITest.Process
            Return String.Format("TheData[fore={0},sur={1},q-125={2}]", data.forename, data.surname, data.Q125)
        End Function
    End Class

    Public Shared Sub Test()
        Dim baseAddress As String = "http://" + Environment.MachineName + ":8000/Service"
        Dim host As WebServiceHost = New WebServiceHost(GetType(Service), New Uri(baseAddress))
        host.Open()
        Console.WriteLine("Host opened")

        Dim client As WebClient = New WebClient()
        client.Headers(HttpRequestHeader.ContentType) = "application/json"
        Dim json As String = "{""surname"":""Pitt"",""forename"":""Mike"",""q-125"":""No""}"
        Console.WriteLine(client.UploadString(baseAddress + "/Process", json))

        host.Close()
    End Sub
End Class

Public Class Post_194aced7_905f_495f_bfbc_4cee9f420440
    <DataContract()> _
    Public Class CompositeType
        <DataMember()> _
        Public Property Name As String
        <DataMember()> _
        Public Property Age As Integer
    End Class
    <ServiceContract()> _
    Public Interface ITest
        <OperationContract()> _
        Function EchoComposite(ByVal input As CompositeType) As CompositeType
    End Interface

    Public Class Service
        Implements ITest

        Public Function EchoComposite(ByVal input As CompositeType) As CompositeType Implements ITest.EchoComposite
            Console.WriteLine("[service] input = {0}/{1}", input.Name, input.Age)
            Return input
        End Function
    End Class

    Public Shared Sub Test()
        Dim baseAddress As String = "http://" + Environment.MachineName + ":8000/Service"
        Dim host As ServiceHost = New ServiceHost(GetType(Service), New Uri(baseAddress))
        host.AddServiceEndpoint(GetType(ITest), New BasicHttpBinding(), "")
        host.Open()
        Console.WriteLine("Host opened")

        Dim factory = New ChannelFactory(Of ITest)(New BasicHttpBinding(), New EndpointAddress(baseAddress))
        Dim proxy = factory.CreateChannel()
        Dim input = New CompositeType
        input.Name = "John Doe"
        input.Age = 33
        Dim output = proxy.EchoComposite(input)
        Console.WriteLine("[client] output={0}/{1}", output.Name, output.Age)

        CType(proxy, IClientChannel).Close()
        factory.Close()
        host.Close()
    End Sub
End Class

' http://stackoverflow.com/q/8387789/751090
Public Class StackOverflow_8387789
    Public Class A
        Public Property myStringProp() As String
        Public Property colB() As Collection(Of B)
    End Class

    Public Class B
        Public Property myStringProp() As String
    End Class

    Public Class Both
        Public Property col1 As Collection(Of A)
        Public Property col2 As Collection(Of B)
    End Class

    Public Shared Sub Test()
        Dim both = New Both()
        both.col2 = New Collection(Of B)
        both.col2.Add(New B With {.myStringProp = "B1"})
        both.col2.Add(New B With {.myStringProp = "B2"})
        both.col2.Add(New B With {.myStringProp = "B3"})
        both.col1 = New Collection(Of A)
        Dim colBForA1 = New Collection(Of B)
        colBForA1.Add(both.col2(0))
        colBForA1.Add(both.col2(1))
        Dim colBForA2 = New Collection(Of B)
        colBForA2.Add(both.col2(1))
        colBForA2.Add(both.col2(2))
        both.col1.Add(New A With {.myStringProp = "A1", .colB = colBForA1})
        both.col1.Add(New A With {.myStringProp = "A2", .colB = colBForA2})
        Dim dcs = New DataContractSerializer(GetType(Both), Nothing, Integer.MaxValue, False, True, Nothing)
        Dim ms = New MemoryStream()
        Dim ws = New XmlWriterSettings With { _
                .Encoding = Encoding.UTF8,
                .Indent = True,
                .IndentChars = "  ",
                .OmitXmlDeclaration = True
            }
        Dim xw = XmlWriter.Create(ms, ws)
        dcs.WriteObject(xw, both)
        xw.Flush()
        Console.WriteLine("Serialized: {0}", Text.Encoding.UTF8.GetString(ms.ToArray()))

        ms.Position = 0
        Console.WriteLine("Now deserializing:")
        Dim both2 = CType(dcs.ReadObject(ms), Both)
        Console.WriteLine("Is both.col1(0).colB(0) = both.col2(0)? {0}", both2.col1(0).colB(0) Is both2.col2(0))
        Console.WriteLine("Is both.col1(1).colB(1) = both.col2(2)? {0}", both2.col1(1).colB(1) Is both2.col2(2))
        Console.WriteLine("Is both.col1(0).colB(0) = both.col2(2) (should be False)? {0}", both2.col1(0).colB(0) Is both2.col2(2))
    End Sub
End Class

Public Class Post_7ba47b64_696b_48dc_9688_5a679d62e643
    Public Class SampleOptions
        Public Property id As Integer
        Public Property title As String
    End Class
    Public Class Sample
        Public Property status As String
        Public Property userId As Integer
        Public Property name As String
        Public Property rol As Integer
        Public Property options As List(Of SampleOptions)
    End Class
    <ServiceContract()> _
    <AspNetCompatibilityRequirements(RequirementsMode:=AspNetCompatibilityRequirementsMode.Allowed)> _
    <ServiceBehavior(InstanceContextMode:=InstanceContextMode.PerCall)> _
    Public Class MobileRestServices
        <WebGet(UriTemplate:="Option1", _
            ResponseFormat:=WebMessageFormat.Json, _
            BodyStyle:=WebMessageBodyStyle.Bare)>
        Public Function ManageMobileUsers1() As Stream
            Dim Sample As String = "{""status"":""ok"",""userId"":23847,""name"":""Marco Casario"",""rol"":0,""options"":[{""id"":1,""title"":""Ventas""},{""id"":2,""title"":""Compras""},{""id"":3,""title"":""Recursos Humanos""}]}"
            WebOperationContext.Current.OutgoingResponse.ContentType = "application/json; charset=utf-8"
            Return New MemoryStream(Encoding.UTF8.GetBytes(Sample))
        End Function
        <WebGet(UriTemplate:="Option2", _
            ResponseFormat:=WebMessageFormat.Json, _
            BodyStyle:=WebMessageBodyStyle.Bare)>
        Public Function ManageMobileUsers2() As Sample
            Dim sample As New Sample
            sample.name = "Marco Casario"
            sample.rol = 0
            sample.status = "ok"
            sample.userId = 23847
            sample.options = New List(Of SampleOptions)
            sample.options.Add(New SampleOptions With {.id = 1, .title = "Ventas"})
            sample.options.Add(New SampleOptions With {.id = 2, .title = "Compras"})
            sample.options.Add(New SampleOptions With {.id = 3, .title = "Recursos Humanos"})
            Return sample   ' A simply string JSON.
        End Function
    End Class

    Public Shared Sub Test()
        Dim baseAddress As String = "http://" + Environment.MachineName + ":8000/Service"
        Dim host As ServiceHost = New ServiceHost(GetType(MobileRestServices), New Uri(baseAddress))
        Dim behavior = New WebHttpBehavior()
        host.AddServiceEndpoint(GetType(MobileRestServices), New WebHttpBinding(), "").Behaviors.Add(behavior)
        host.Open()
        Console.WriteLine("Host opened")

        Dim wc As WebClient = New WebClient()
        Dim datos As String = wc.DownloadString(baseAddress + "/Option1")
        Console.WriteLine(datos)

        datos = wc.DownloadString(baseAddress + "/Option2")
        Console.WriteLine(datos)

        host.Close()
    End Sub
End Class

Public Class Post_5098d8ba_e6d1_4c63_8b72_7147c591b6d1
    <DataContract()> _
    Public Class AreaOptions

        <DataMember(name:="id", Order:=5)> _
        Public Property id As Integer

        <DataMember(name:="title", Order:=6)> _
        Public Property title As String

    End Class

    <DataContract()> _
    Public Class Logins

        <DataMember(name:="status", Order:=0)> _
        Public Property status As String

        <DataMember(name:="userId", Order:=1)> _
        Public Property userId As Integer

        <DataMember(name:="name", Order:=2)> _
        Public Property name As String

        <DataMember(name:="rol", Order:=3)> _
        Public Property rol As Integer

        <DataMember(name:="areas", Order:=4)> _
        Public Property areas As List(Of AreaOptions)

    End Class

    <ServiceContract()>
    <AspNetCompatibilityRequirements(RequirementsMode:=AspNetCompatibilityRequirementsMode.Allowed)>
    <ServiceBehavior(InstanceContextMode:=InstanceContextMode.PerCall)>
    Public Class MobileRestServices
        <WebGet(UriTemplate:="ManageMobileUsers/{UsrNmb}/{UsrPwd}/{Device}",
            ResponseFormat:=WebMessageFormat.Json,
            BodyStyle:=WebMessageBodyStyle.Bare)>
        Public Function ManageMobileUsers(ByVal UsrNmb As String, ByVal UsrPwd As String, ByVal Device As String) As Logins
            Dim Login As New Logins
            Login.name = "Usuario Demo"
            Login.rol = 0
            Login.status = "ok"
            Login.userId = 12345
            Login.areas = New List(Of AreaOptions)
            Login.areas.Add(New AreaOptions With {.id = 1, .title = "COSTOS_Y_GASTOS"})
            Login.areas.Add(New AreaOptions With {.id = 2, .title = "VENTAS_BULK"})
            Login.areas.Add(New AreaOptions With {.id = 3, .title = "VENTAS_CR"})
            Login.areas.Add(New AreaOptions With {.id = 4, .title = "CARTERA_ANEJAMIENTOS"})
            Login.areas.Add(New AreaOptions With {.id = 5, .title = "BULK"})
            Login.areas.Add(New AreaOptions With {.id = 6, .title = "VENTAS_GASES_ENVASADOS"})
            Login.areas.Add(New AreaOptions With {.id = 7, .title = "COSTOS_Y_GASTOS_CR"})
            Login.areas.Add(New AreaOptions With {.id = 8, .title = "CILINDROS"})
            Login.areas.Add(New AreaOptions With {.id = 9, .title = "VENTAS_INTEGRADAS_GENVASADOS"})
            Login.areas.Add(New AreaOptions With {.id = 10, .title = "COSTOS_Y_GASTOS_JDE_90"})

            Return Login   ' A simply string JSON.
        End Function
    End Class

    Public Shared Sub Test()
        Dim baseAddress As String = "http://" + Environment.MachineName + ":8000/Service"
        Dim host As WebServiceHost = New WebServiceHost(GetType(MobileRestServices), New Uri(baseAddress))
        host.Open()
        Console.WriteLine("Host opened")

        Dim client As WebClient = New WebClient()
        Console.WriteLine(client.DownloadString(baseAddress + "/ManageMobileUsers/john/foo/dev"))

        host.Close()
    End Sub

End Class

Module Module1

    Sub Main()
        Post_5098d8ba_e6d1_4c63_8b72_7147c591b6d1.Test()
    End Sub

End Module
