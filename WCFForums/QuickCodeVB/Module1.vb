Imports System.Runtime.Serialization
Imports System.ServiceModel
Imports System.ServiceModel.Channels
Imports System.ServiceModel.Description
Imports System.ServiceModel.Dispatcher

Public Class Post_2cf7cd17_c963_465b_a8ce_3edf5bd0467b
    <ServiceContract()> _
    Public Interface ITest
        <OperationContract()> _
        Function Add(ByVal x As Integer, ByVal y As Integer) As Integer
    End Interface

    Public Class Service
        Implements ITest

        Private HostInstance As Post_2cf7cd17_c963_465b_a8ce_3edf5bd0467b
        Public Sub New(ByVal hostInstance As Post_2cf7cd17_c963_465b_a8ce_3edf5bd0467b)
            Me.HostInstance = hostInstance
        End Sub

        Public Function Add(ByVal x As Integer, ByVal y As Integer) As Integer Implements ITest.Add
            Me.HostInstance.ServiceCalled(String.Format("Add({0}, {1})", x, y))
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
        Console.WriteLine("[host] Service operation called: {0}", text)
    End Sub

    Public Sub HostAndWait()
        Dim baseAddress As String = "http://" + Environment.MachineName + ":8000/Service"
        Dim host As ServiceHost = New ServiceHost(GetType(Service), New Uri(baseAddress))
        Dim endpoint = host.AddServiceEndpoint(GetType(ITest), New BasicHttpBinding(), "")
        endpoint.Behaviors.Add(New MyInstanceProvider(Me))
        host.Open()
        Console.WriteLine("Host opened")

        Dim factory = New ChannelFactory(Of ITest)(New BasicHttpBinding(), New EndpointAddress(baseAddress))
        Dim proxy = factory.CreateChannel()
        Console.WriteLine(proxy.Add(5, 7))

        Console.WriteLine("Press ENTER to close")
        Console.ReadLine()

        CType(proxy, IClientChannel).Close()
        factory.Close()
        host.Close()
    End Sub

    Public Shared Sub Test()
        Dim host = New Post_2cf7cd17_c963_465b_a8ce_3edf5bd0467b
        host.HostAndWait()
    End Sub

End Class

Module Module1

    Sub Main()
        Post_2cf7cd17_c963_465b_a8ce_3edf5bd0467b.Test()
    End Sub

End Module
