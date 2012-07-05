using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Web;
using System.Globalization;

namespace StackOverflow_11105856
{
    [ServiceContract]
    public interface IService1
    {
        [OperationContract]
        [WebInvoke(Method = "POST",
                   RequestFormat = WebMessageFormat.Json,
                   ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Wrapped)]
        string UpdateEmployee(Employee Employee);
    }

    public class Service : IService1
    {
        public string UpdateEmployee(Employee Employee)
        {
            return string.Format("Name={0},Hired={1}", Employee.Name, Employee.Hired.ToString("yyyy-MM-dd HH:mm:ss"));
        }
    }

    [DataContract]
    public class Employee
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Department { get; set; }

        [DataMember]
        public int Salary { get; set; }

        public DateTime Hired { get; set; }

        [DataMember(Name = "Hired")]
        private string HiredForSerialization { get; set; }

        [OnSerializing]
        void OnSerializing(StreamingContext ctx)
        {
            this.HiredForSerialization = this.Hired.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        [OnDeserializing]
        void OnDeserializing(StreamingContext ctx)
        {
            this.HiredForSerialization = "1900-01-01";
        }

        [OnDeserialized]
        void OnDeserialized(StreamingContext ctx)
        {
            this.Hired = DateTime.ParseExact(this.HiredForSerialization, "MM/dd/yyyy", CultureInfo.InvariantCulture);
        }
    }
}
