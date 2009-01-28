using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Duologue.Audio
{

    public interface IService
    {
    }

    public class ServiceDescriptor
    {
        private IService service;

        public ServiceDescriptor()
        {
        }

        public bool Matches(ServiceDescriptor comparator)
        {
            bool retval = false;
            Type hickory = comparator.GetType();
            Type dickory = this.GetType();
            if (hickory == dickory)
            {
                retval = true;
            }
            return retval;
        }
    }

    public static class ServiceLocator
    {
        private static List<IService> services = new List<IService>();

        public static void RegisterService(IService service)
        {
            services.Add(service);
        }

        public static T GetService<T>()
        {
            T retObj = default(T);
            //walk the dictionary until we find a matching service
            foreach (IService service in services)
            {
                if (service is T)
                {
                    retObj = (T)service;
                }
            }
            return retObj;
        }

    }
}
