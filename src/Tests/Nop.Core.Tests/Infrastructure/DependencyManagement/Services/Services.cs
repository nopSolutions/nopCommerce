using Nop.Core.Infrastructure.DependencyManagement;

namespace Nop.Core.Tests.Infrastructure.DependencyManagement.Services
{
    public interface IBarometer
    {
        int GetPressure();
    }

    [Dependency(typeof(IBarometer), Configuration = "Low")]
    public class LowService : IBarometer
    {
        #region IBarometer Members

        public int GetPressure()
        {
            return 1;
        }

        #endregion
    }

    [Dependency(typeof(IBarometer), Configuration = "High")]
    public class HighService : IBarometer
    {
        #region IBarometer Members

        public int GetPressure()
        {
            return 2;
        }

        #endregion
    }

    [Dependency(Key = "Sesame")]
    public class SelfService
    {
    }

    [Dependency]
    public class DependingService
    {
        public SelfService service;
        public DependingService(SelfService service)
        {
            this.service = service;
        }
    }

    public class UnregisteredDependency
    {
    }

    [Dependency]
    public class DependingServiceWithMissingDependency
    {
        public UnregisteredDependency service;
        public DependingServiceWithMissingDependency(UnregisteredDependency service)
        {
            this.service = service;
        }
    }

    [Dependency]
    public class GenericSelfService<T>
    {
    }

    [Dependency]
    public class DependingGenericSelfService<T>
    {
        public SelfService service;
        public DependingGenericSelfService(SelfService service)
        {
            this.service = service;
        }
    }

    [Dependency]
    public class GenericDependingService
    {
        public GenericSelfService<int> service;
        public GenericDependingService(GenericSelfService<int> service)
        {
            this.service = service;
        }
    }

    public interface IService
    {
    }

    [Dependency(typeof(IService))]
    public class InterfacedService : IService
    {
    }

    public class ConcreteService : AbstractService
    {
    }

    public abstract class AbstractService : IService
    {
    }

    [Dependency(typeof(IService))]
    public class DecoratingService : IService
    {
        public IService decorated;

        public DecoratingService(IService decorated)
        {
            this.decorated = decorated;
        }
    }

    public interface IGenericService<T>
    {
    }

    [Dependency(typeof(IGenericService<>))]
    public class GenericInterfacedService<T> : IGenericService<T>
    {
    }

    [Dependency]
    public class GenericInterfaceDependingService
    {
        public IGenericService<int> service;
        public GenericInterfaceDependingService(IGenericService<int> service)
        {
            this.service = service;
        }
    }

    public class NonAttributed
    {
    }
}
