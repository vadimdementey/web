using Microsoft.Framework.DependencyInjection;
using System;
using System.Collections.Generic;

namespace CoreLibrary
{
    internal class TypeProvider : Dictionary<string, Type> , ITypeProvider  , IServiceDescriptor
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
                return typeof(ITypeProvider);
            }
        }

        public Type GetType(string typeName)
        {
            Type x;

            if (TryGetValue(typeName.ToLowerInvariant(), out x))
            {
                return x;
            }

            return typeof(void);
        }

        public void SetType(string typeName, Type typeInstance)
        {
            base[typeName.ToLowerInvariant()] = typeInstance;
        }
    }
}