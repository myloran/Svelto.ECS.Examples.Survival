using UnityEngine;

namespace Svelto.ECS.Example.Survive.Characters.Enemies
{
    public interface IEnemyMovement: IComponent
    {
        bool navMeshEnabled {  set; get; }
        Vector3 navMeshDestination { set; }
        bool setCapsuleAsTrigger { set; }
    }

    public interface IEnemyTrigger: IComponent
    {
        EnemyCollisionData entityInRange { get; }
    }

    public struct EnemyCollisionData
    {
        public EGID otherEntityID;
        public readonly bool collides;

        public EnemyCollisionData(EGID otherEntityID, bool collides)
        {
            this.otherEntityID = otherEntityID;
            this.collides = collides;
        }
    }

    public interface IEnemyVFX: IComponent
    {
        Vector3 position { set; }
        bool    play     { set; }
    }
}