using System;
using System.Reflection;

namespace CoreLibrary
{
    public class ServiceCore
    {
        public ServiceCore()
        {

        }

        public object GetData()
        {
            return MethodBase.GetCurrentMethod().Name;
        }
    }
}
