namespace Svelto.ECS.Example.Survive.Characters.Enemies
{
    public struct EnemyView:IEntityViewStruct
    {
        public IEnemyMovement    movement;
        public IEnemyVFX         vfx;
    
        public IAnimation        animation;
        public ITransform        transform;
        public IPosition         position;
        public ILayerComponent            layerComponent;
        
        public EGID ID { get; set; }
    }

    public struct EnemyAttack:IEntityViewStruct
    {
        public IEnemyTrigger    targetTrigger;
        public EGID ID { get; set; }
    }

    public struct EnemyTarget : IEntityViewStruct
    {
        public IPosition targetPosition;
        public EGID ID { get; set; }
    }
}
