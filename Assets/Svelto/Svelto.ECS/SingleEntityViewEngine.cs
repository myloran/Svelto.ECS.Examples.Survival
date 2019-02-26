using Svelto.ECS.Internal;

namespace Svelto.ECS
{
    public abstract class SingleEntityViewEngine<T> : EngineInfo, IHandleEntityStructEngine<T> where T : class, IEntityStruct
    {
        public void AddInternal(ref T view)
        { Add(view); }

        public void RemoveInternal(ref T view)
        { Remove(view); }

        protected abstract void Add(T entityView);
        protected abstract void Remove(T entityView);
    }
}