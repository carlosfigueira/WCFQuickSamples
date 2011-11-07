Imports System.ServiceModel
Imports System.Runtime.Serialization

Public Class Template
    <ServiceContract()> _
    Public Interface ITest
        <OperationContract()> _
        Function Add(ByVal x As Integer, ByVal y As Integer) As Integer
    End Interface

    Public Class Service
        Implements ITest

        Public Function Add(ByVal x As Integer, ByVal y As Integer) As Integer Implements ITest.Add
            Return x + y
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
        Console.WriteLine(proxy.Add(5, 7))

        CType(proxy, IClientChannel).Close()
        factory.Close()
        host.Close()
    End Sub
End Class
