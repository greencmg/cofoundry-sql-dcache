using Cofoundry.Core.DependencyInjection;
using Cofoundry.BasicTestSite.Caching;
using Cofoundry.Core.Caching;

namespace Cofoundry.BasicTestSite.DependencyRegistration
{
    public class CachingDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            var registrationOptions = new RegistrationOptions()
            {
                ReplaceExisting = false
            };

            container
                .Register<IObjectCacheFactory, SqlDistributedObjectCacheFactory>(registrationOptions);

 
        }
    }
}
