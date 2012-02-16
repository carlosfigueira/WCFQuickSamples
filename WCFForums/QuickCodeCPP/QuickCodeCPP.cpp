// QuickCodeCPP.cpp : main project file.

#include "stdafx.h"

#using "System.ServiceModel.dll"

using namespace System;
using namespace System::ServiceModel;

[ServiceContract]
public interface class ITestCallback {
    [OperationContract(IsOneWay = true)]
    void OnHello(String^ text);
};

[ServiceContract(CallbackContract = ITestCallback::typeid)]
public interface class ITest {
    [OperationContract]
    void Hello(String^ text);
};

ref class Service : public ITest {
public:
    virtual void Hello(String^ text);
};

void Service::Hello(String^ text) {
    Console::WriteLine(L"[server] {0}", text);
    OperationContext::Current->GetCallbackChannel<ITestCallback^>()->OnHello(text);
}

ref class ClientCallback : ITestCallback {
public:
    virtual void OnHello(String^ text);
};

void ClientCallback::OnHello(String^ text) {
    Console::WriteLine(L"[client] {0}", text);
}

int main(array<System::String ^> ^args)
{
    String^ baseAddress = String::Format(L"http://{0}:8000/Service", Environment::MachineName);
    ServiceHost^ host = gcnew ServiceHost(Service::typeid, gcnew Uri(baseAddress));
    WSDualHttpBinding^ binding = gcnew WSDualHttpBinding(WSDualHttpSecurityMode::None);
    host->AddServiceEndpoint(ITest::typeid, binding, "");
    host->Open();
    Console::WriteLine(L"Host opened");

    EndpointAddress^ endpointAddress = gcnew EndpointAddress(baseAddress);
    ClientCallback^ callback = gcnew ClientCallback();
    InstanceContext^ instanceContext = gcnew InstanceContext(callback);
    DuplexChannelFactory<ITest^>^ factory = gcnew DuplexChannelFactory<ITest^>(instanceContext, binding, endpointAddress);
    ITest^ proxy = factory->CreateChannel();
    proxy->Hello(L"Hello world");

    Console::WriteLine(L"Press ENTER to close");
    Console::ReadLine();
    return 0;
}
