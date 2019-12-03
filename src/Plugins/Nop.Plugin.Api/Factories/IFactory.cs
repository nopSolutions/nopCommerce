namespace Nop.Plugin.Api.Factories
{
    public interface IFactory<T>
    {
        T Initialize();
    }
}