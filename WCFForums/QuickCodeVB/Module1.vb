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

Module Module1

    Sub Main()
        Post_7ba47b64_696b_48dc_9688_5a679d62e643.Test()
    End Sub

End Module
