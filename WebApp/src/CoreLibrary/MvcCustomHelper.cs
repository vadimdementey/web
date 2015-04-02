using Microsoft.AspNet.Mvc;
using Microsoft.Framework.DependencyInjection;
using System;
using System.Linq;
using Microsoft.Framework.ConfigurationModel;


namespace CoreLibrary
{
    public static class MvcCustomHelper
    {
        public static IServiceCollection AddCustomMvc(this IServiceCollection @this)
        {
            ServiceDescriber serviceDescriber = new ServiceDescriber();

            @this.Add(serviceDescriber.Transient<IControllerFactory, ControllerFactory>());
            @this.Add(new TypeProvider());

            return @this;
        }

        public static IServiceCollection AddCustomController<TController>(this IServiceCollection @this)
        {
            string typeName = typeof(TController).Name.ToLowerInvariant().Replace("controller", string.Empty);

            ITypeProvider typeProvider = @this.OfType<ITypeProvider>().First();

            typeProvider.SetType(typeName, typeof(TController));

            return @this;
        }


        public static IServiceCollection AddConfig(this IServiceCollection @this, string jsonFilePath)
        {
            ConfigProvider provider = new ConfigProvider();
            provider.AddJsonFile(jsonFilePath);

            @this.Add(provider);
            return @this;
        }
    }
}