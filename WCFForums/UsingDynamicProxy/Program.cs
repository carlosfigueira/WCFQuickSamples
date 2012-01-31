using System;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using WcfSamples.DynamicProxy;

namespace Post_4073aaa2_1ac9_427b_a0a7_dd0c68ee146c
{
    [ServiceContract]
    public interface ITest
    {
        [OperationContract]
        string Hello();
    }
    public class Service : ITest
    {
        public string Hello()
        {
            return "Hello, " + OperationContext.Current.ServiceSecurityContext.PrimaryIdentity.Name;
        }
    }
    class MyPasswordValidator : UserNamePasswordValidator
    {
        public override void Validate(string userName, string password)
        {
            if (userName != password)
            {
                throw new SecurityTokenException("Invalid password");
            }
        }
    }
    class MainClass
    {
        static Binding GetBinding()
        {
            var result = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
            result.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
            return result;
        }
        static bool SkipCertValidation(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            Console.WriteLine("Validating for {0}", certificate.Subject);
            return true;
        }
        public static void Test()
        {
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(SkipCertValidation);
            string baseAddress = "https://" + Environment.MachineName + ":8888/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITest), GetBinding(), "");
            host.Description.Behaviors.Add(new ServiceMetadataBehavior { HttpsGetEnabled = true });

            host.Credentials.UserNameAuthentication.UserNamePasswordValidationMode = UserNamePasswordValidationMode.Custom;
            host.Credentials.UserNameAuthentication.CustomUserNamePasswordValidator = new MyPasswordValidator();
            host.Authorization.PrincipalPermissionMode = PrincipalPermissionMode.UseAspNetRoles;

            host.Open();
            Console.WriteLine("Host opened");

            DynamicProxyFactory factory = new DynamicProxyFactory(baseAddress + "?wsdl");

            DynamicProxy proxy = factory.CreateProxy("ITest");
            ClientCredentials credentials = proxy.GetProperty("ClientCredentials") as ClientCredentials;
            credentials.UserName.UserName = "John Doe";
            credentials.UserName.Password = "John Doe";

            Console.WriteLine(proxy.CallMethod("Hello"));

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }
}

namespace Post_da7d768d_ccc2_4011_a010_40f20dfa48e4
{
    [ServiceContract]
    public interface IService
    {

        [OperationContract]
        string GetData(int value);

        [OperationContract]
        CompositeType GetDataUsingDataContract(CompositeType composite);
    }

    [DataContract]
    public class CompositeType
    {
        bool boolValue = true;
        string stringValue = "Hello ";

        [DataMember]
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }

    public class Service : IService
    {
        public string GetData(int value)
        {
            return "You entered: " + value;
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite.BoolValue)
            {
                composite.StringValue = composite.StringValue.ToUpperInvariant();
            }
            else
            {
                composite.StringValue = composite.StringValue.ToLowerInvariant();
            }

            return composite;
        }
    }

    class MainClass
    {
        public static void Test()
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(IService), new BasicHttpBinding(), "");
            host.Description.Behaviors.Add(new ServiceMetadataBehavior { HttpGetEnabled = true });

            host.Open();
            Console.WriteLine("Host opened");

            DynamicProxyFactory factory = new DynamicProxyFactory(baseAddress + "?wsdl");

            DynamicProxy dynamicProxy = factory.CreateProxy("IService");
            Console.WriteLine(dynamicProxy.CallMethod("GetData", 123));

            Type proxyType = dynamicProxy.ProxyType;
            MethodInfo getDataUsingDCMethod = proxyType.GetMethod("GetDataUsingDataContract");
            Type dcType = getDataUsingDCMethod.GetParameters()[0].ParameterType;
            DynamicObject obj = new DynamicObject(dcType);
            obj.CallConstructor();
            obj.SetProperty("BoolValue", true);
            obj.SetProperty("StringValue", "Hello world");

            DynamicObject result = new DynamicObject(
                dynamicProxy.CallMethod(
                    "GetDataUsingDataContract",
                    obj.ObjectInstance));

            Console.WriteLine(result.GetProperty("StringValue"));

            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }
}

namespace UsingDynamicProxy
{
    class Program
    {
        static void Main(string[] args)
        {
            Post_da7d768d_ccc2_4011_a010_40f20dfa48e4.MainClass.Test();
        }
    }
}
