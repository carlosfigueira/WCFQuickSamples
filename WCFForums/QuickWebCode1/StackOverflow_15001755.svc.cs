using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace StackOverflow_15001755
{
    [ServiceContract]
    public class Service
    {
        static Dictionary<string, int> dictionary;

        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public void setDictionary(Dictionary<string, int> myDictionary)
        {
            dictionary = myDictionary;
        }

        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public Dictionary<string, int> getDictionary()
        {
            return dictionary;
        }
    }
}
