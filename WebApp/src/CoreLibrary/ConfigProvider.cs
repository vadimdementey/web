using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;
using System;
using System.Linq;

namespace CoreLibrary
{
    public class ConfigProvider : Configuration, IConfigProvider , IServiceDescriptor
    {
        public Func<IServiceProvider, object> ImplementationFactory
        {
            get
            {
                return x => this;
            }
        }

        public object ImplementationInstance
        {
            get
            {
                return this;
            }
        }

        public Type ImplementationType
        {
            get
            {
                return GetType();
            }
        }

        public LifecycleKind Lifecycle
        {
            get
            {
                return LifecycleKind.Singleton;
            }
        }

        public Type ServiceType
        {
            get
            {
                return typeof(IConfigProvider);
            }
        }

        public string GetString(string pathKey)
        {
            string[] queryPath = pathKey.Split('\\', '/');

            IConfiguration root = this;

            for (int i = 0; i < queryPath.Length - 1; i++)
            {
                root = root.GetSubKey(queryPath[i]); 
            }

            return root.Get(queryPath.Last());
        }

    
    }
}