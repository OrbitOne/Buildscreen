using System.Web.Mvc;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace OrbitOne.BuildScreen.DependencyInjection
{
    public class ControllerRegistry : IWindsorInstaller
    {
       
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Classes.FromAssemblyNamed("OrbitOne.BuildScreen")
                                .BasedOn<IController>()
                                .LifestyleTransient());
        }
    }
}
