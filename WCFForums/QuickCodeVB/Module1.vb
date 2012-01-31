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

Public Class Post_4c22e180_cebd_4a5b_9d9f_5055469a7d94

    Class MyEncodingBindingElement
        Inherits MessageEncodingBindingElement

        Private inner As MessageEncodingBindingElement

        Public Sub New(ByRef inner As MessageEncodingBindingElement)
            Me.inner = inner
        End Sub

        Public Overrides Function Clone() As BindingElement
            Return New MyEncodingBindingElement(CType(Me.inner.Clone, MessageEncodingBindingElement))
        End Function

        Public Overrides Function CreateMessageEncoderFactory() As MessageEncoderFactory
            Return New MyEncoderFactory(Me.inner.CreateMessageEncoderFactory())
        End Function

        Public Overrides Property MessageVersion As MessageVersion
            Get
                Return Me.inner.MessageVersion
            End Get
            Set(value As MessageVersion)
                Me.inner.MessageVersion = value
            End Set
        End Property

        Public Overrides Function CanBuildChannelFactory(Of TChannel)(context As BindingContext) As Boolean
            Return context.CanBuildInnerChannelFactory(Of TChannel)()
        End Function

        Public Overrides Function BuildChannelFactory(Of TChannel)(context As BindingContext) As IChannelFactory(Of TChannel)
            context.BindingParameters.Add(Me)
            Return context.BuildInnerChannelFactory(Of TChannel)()
        End Function
    End Class

    Class MyEncoderFactory
        Inherits MessageEncoderFactory

        Private inner As MessageEncoderFactory

        Sub New(inner As MessageEncoderFactory)
            Me.inner = inner
        End Sub

        Public Overrides ReadOnly Property Encoder As MessageEncoder
            Get
                Return New MyEncoder(Me.inner.Encoder)
            End Get
        End Property

        Public Overrides ReadOnly Property MessageVersion As MessageVersion
            Get
                Return Me.inner.MessageVersion
            End Get
        End Property
    End Class

    Class MyEncoder
        Inherits MessageEncoder

        Private inner As MessageEncoder

        Sub New(inner As MessageEncoder)
            Me.inner = inner
        End Sub

        Public Overrides ReadOnly Property ContentType As String
            Get
                Return Me.inner.ContentType
            End Get
        End Property

        Public Overrides ReadOnly Property MediaType As String
            Get
                Return Me.inner.MediaType
            End Get
        End Property

        Public Overrides ReadOnly Property MessageVersion As MessageVersion
            Get
                Return Me.inner.MessageVersion
            End Get
        End Property

        Public Overrides Function IsContentTypeSupported(contentType As String) As Boolean
            Return Me.inner.IsContentTypeSupported(contentType) Or contentType = "text/xml; charset=ISO-8859-1"
        End Function

        Public Overloads Overrides Function ReadMessage(buffer As ArraySegment(Of Byte), bufferManager As BufferManager, contentType As String) As Message
            If contentType = "text/xml; charset=ISO-8859-1" Then
                Dim msgContents As Byte()
                msgContents = New Byte(buffer.Count - 1) {}
                Array.Copy(buffer.Array, buffer.Offset, msgContents, 0, buffer.Count)
                bufferManager.ReturnBuffer(buffer.Array)
                Return Me.ReadMessage(New MemoryStream(msgContents), Integer.MaxValue, contentType)
            Else
                Return Me.inner.ReadMessage(buffer, bufferManager, contentType)
            End If
        End Function

        Public Overloads Overrides Function ReadMessage(stream As Stream, maxSizeOfHeaders As Integer, contentType As String) As Message
            Dim reader As XmlReader
            reader = XmlReader.Create(stream)
            Return Message.CreateMessage(reader, maxSizeOfHeaders, Me.MessageVersion)
        End Function

        Public Overloads Overrides Function WriteMessage(message As Message, maxMessageSize As Integer, bufferManager As BufferManager, messageOffset As Integer) As ArraySegment(Of Byte)
            Return Me.inner.WriteMessage(message, maxMessageSize, bufferManager, messageOffset)
        End Function

        Public Overloads Overrides Sub WriteMessage(message As Message, stream As Stream)
            Me.inner.WriteMessage(message, stream)
        End Sub
    End Class

    Public Shared Sub Test()
        Dim noaa_service As New ReferencePost_4c22e180_cebd_4a5b_9d9f_5055469a7d94.ndfdXMLPortTypeClient

        Dim customBinding As CustomBinding
        customBinding = New CustomBinding(noaa_service.Endpoint.Binding)
        Dim i
        For i = 0 To customBinding.Elements.Count - 1
            Dim mebe As MessageEncodingBindingElement
            mebe = TryCast(customBinding.Elements(i), MessageEncodingBindingElement)
            If mebe IsNot Nothing Then
                customBinding.Elements(i) = New MyEncodingBindingElement(CType(customBinding.Elements(i), MessageEncodingBindingElement))
                Exit For
            End If
        Next
        noaa_service.Endpoint.Binding = customBinding

        Dim noaa_weather_type As New ReferencePost_4c22e180_cebd_4a5b_9d9f_5055469a7d94.weatherParametersType

        noaa_weather_type.tstmprb = True
        noaa_weather_type.wdir = True
        noaa_weather_type.wdir_r = True
        noaa_weather_type.tmpabv14d = True

        noaa_service.Open()
        Try
            Dim s As String = noaa_service.NDFDgen(CDec(45.0), _
                                                   -105, _
                                                   ReferencePost_4c22e180_cebd_4a5b_9d9f_5055469a7d94.productType.timeseries, _
                                                   Date.Now, _
                                                   Date.Now.AddDays(7), _
                                                   ReferencePost_4c22e180_cebd_4a5b_9d9f_5055469a7d94.unitType.e, _
                                                   noaa_weather_type)
            Console.WriteLine(s)
        Catch ex As Exception
            Console.WriteLine(ex)
        End Try
    End Sub

End Class


Module Module1

    Sub Main()
        Post_4c22e180_cebd_4a5b_9d9f_5055469a7d94.Test()
    End Sub

End Module
