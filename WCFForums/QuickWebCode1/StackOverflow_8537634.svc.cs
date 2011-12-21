using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Web;

namespace StackOverflow_8537634
{
    public class EduLink : IEduLink
    {
        public string SaveUserData(UserInfo userInfo)
        {
            return string.Format("LevelId:{0}, EMailId:{1}", userInfo.LevelID, userInfo.EmailID);
        }
    }

    [ServiceContract(Namespace = "")]
    public interface IEduLink
    {
        [OperationContract]
        [WebInvoke(
                Method = "POST",
                ResponseFormat = WebMessageFormat.Json,
                RequestFormat = WebMessageFormat.Json,
                BodyStyle = WebMessageBodyStyle.WrappedRequest,
                UriTemplate = "/SaveUserData")]
        string SaveUserData(UserInfo userInfo);
    }

    [DataContract]
    public class UserInfo
    {
        [DataMember]
        public string EmailID { get; set; }
        [DataMember]
        public int LevelID { get; set; }
    }
}
