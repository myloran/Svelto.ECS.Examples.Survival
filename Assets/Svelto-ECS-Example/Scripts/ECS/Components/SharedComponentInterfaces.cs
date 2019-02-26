using UnityEngine;

namespace Svelto.ECS.Example.Survive
{
    public interface IAnimation: IComponent
    {
        string playAnimation { set; get; }
        AnimationState animationState { set; }
        bool reset { set; }
    }

    public interface IPosition: IComponent
    {
        Vector3 position { get; }
    }

    public interface ITransform: IPosition
    {
        new Vector3 position { set; }
        Quaternion rotation { set; }
    }
    
    public interface ILayerComponent
    {
        int layer { set; }
    }

    public interface IBody: IComponent
    {
        bool isKinematic { set; }
    }

    public interface ISpeed: IComponent
    {
        float movementSpeed { get; }
    }

    public interface IDamageSoundComponent: IComponent
    {
        AudioType playOneShot { set; }
    }
}
