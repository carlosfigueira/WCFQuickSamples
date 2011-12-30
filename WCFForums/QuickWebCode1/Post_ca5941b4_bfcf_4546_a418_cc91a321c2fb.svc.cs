using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Post_ca5941b4_bfcf_4546_a418_cc91a321c2fb
{
    [ServiceContract]
    public interface IService1
    {
        [OperationContract]
        int Add(int x, int y);
    }
    public class Service1 : IService1
    {
        public int Add(int x, int y)
        {
            return x + y;
        }
    }
}
