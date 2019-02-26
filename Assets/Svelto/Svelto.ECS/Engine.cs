using Svelto.ECS.Internal;

namespace Svelto.ECS
{
    public abstract class Engine<T> : EngineInfo, IHandleEntityStructEngine<T> where T : IEntityStruct
    {
        public void AddInternal(ref T view)
        { Add(ref view); }

        public void RemoveInternal(ref T view)
        { Remove(ref view); }

        protected abstract void Add(ref    T view);
        protected abstract void Remove(ref T view);
    }
    
    public abstract class Engine<T, U> : Engine<T>, IHandleEntityStructEngine<U>
        where U : IEntityStruct where T : IEntityStruct
    {
        public void AddInternal(ref U view)
        { Add(ref view); }
        public void RemoveInternal(ref U view)
        { Remove(ref view); }
        
        protected abstract void Add(ref U    view);
        protected abstract void Remove(ref U view);
    }
    
    public abstract class Engine<T, U, V> : Engine<T, U>, IHandleEntityStructEngine<V>
        where V :  IEntityStruct where U :  IEntityStruct where T :  IEntityStruct
    {
        public void AddInternal(ref V view)
        { Add(ref view); }
        public void RemoveInternal(ref V view)
        { Remove(ref view); }
        
        protected abstract void Add(ref V    view);
        protected abstract void Remove(ref V view);
    }

    /// <summary>
    ///     Please do not add more MultiEntityViewsEngine if you use more than 4 nodes, your engine has
    ///     already too many responsibilities.
    /// </summary>
    public abstract class Engine<T, U, V, W> : Engine<T, U, V>, IHandleEntityStructEngine<W>
        where W :  IEntityStruct where V :  IEntityStruct where U :  IEntityStruct where T : IEntityStruct
    {
        public void AddInternal(ref W view)
        { Add(ref view); }
        public void RemoveInternal(ref W view)
        { Remove(ref view); }
        
        protected abstract void Add(ref W    view);
        protected abstract void Remove(ref W view);
    }
}