using Autofac;

namespace Nop.Core.Infrastructure.DependencyManagement
{
    public interface IDependencyRegistar
    {
        void Register(ContainerBuilder builder, ITypeFinder typeFinder);

        int Order { get; }
    }
}
