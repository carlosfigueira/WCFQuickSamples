using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace QuickCode1
{
    [ServiceContract]
    public interface ISampleContract
    {
        [OperationContract]
        int Add(int x, int y);
    }
    public class SampleService : ISampleContract
    {
        public int Add(int x, int y)
        {
            return x + y;
        }
    }
}
