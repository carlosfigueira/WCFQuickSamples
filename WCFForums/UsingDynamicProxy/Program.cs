using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.ServiceModel.Security;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using WcfSamples.DynamicProxy;

namespace UsingDynamicProxy
{
    public class Post_4073aaa2_1ac9_427b_a0a7_dd0c68ee146c
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

    class Program
    {
        static void Main(string[] args)
        {
            Post_4073aaa2_1ac9_427b_a0a7_dd0c68ee146c.Test();
        }
    }
}
