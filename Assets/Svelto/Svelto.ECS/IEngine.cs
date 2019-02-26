namespace Svelto.ECS.Internal
{
    public interface IHandleEntityViewEngineAbstracted : IEngine
    {}
    
    public interface IHandleEntityStructEngine<T> : IHandleEntityViewEngineAbstracted
    {
        void AddInternal(ref    T view);
        void RemoveInternal(ref T view);
    }
    
    public class EngineInfo
    {
#if ENABLE_PLATFORM_PROFILER
        protected EngineInfo()
        {
            name = GetType().FullName;
        }
#endif    
        internal readonly string name = string.Empty;
    }
}

namespace Svelto.ECS
{
    public interface IEngine
    {}
}