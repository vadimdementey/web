using Microsoft.AspNet.Mvc;
using Microsoft.Framework.DependencyInjection;
using System;

namespace CoreLibrary
{




    public class ControllerFactory : IControllerFactory
    {
        private IServiceProvider     mvc_service_provider    ;
        private ITypeActivator       mvc_type_activator      ;
        private IControllerActivator mvc_controller_activator;


        public ControllerFactory(IServiceProvider serviceProvider, ITypeActivator typeActivator, IControllerActivator controllerActivator)
        {
            mvc_service_provider     = serviceProvider;
            mvc_type_activator       = typeActivator;
            mvc_controller_activator = controllerActivator;
        }

        public object CreateController(ActionContext actionContext)
        {

            ControllerActionDescriptor actionDescriptor = actionContext.ActionDescriptor as ControllerActionDescriptor;

            Type   controllerType = mvc_service_provider.GetRequiredService<ITypeProvider>().GetType(actionDescriptor.ControllerName);
            object controller     = mvc_type_activator.CreateInstance(mvc_service_provider, controllerType);

            mvc_controller_activator.Activate(controller, actionContext);

            return controller;
        }

        public void ReleaseController(object controller)
        {
            IDisposable obj = controller as IDisposable;

            if (obj != null)
            {
                obj.Dispose();
            }
        }
    }
}