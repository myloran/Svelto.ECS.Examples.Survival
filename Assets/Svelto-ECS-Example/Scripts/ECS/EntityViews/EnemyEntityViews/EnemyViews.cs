namespace Svelto.ECS.Example.Survive.Characters.Enemies
{
    public struct EnemyEntityViewStruct:IEntityViewStruct
    {
        public IEnemyMovementComponent    movementComponent;
        public IEnemyVFXComponent         vfxComponent;
    
        public IAnimation        animation;
        public ITransform        transform;
        public IPosition         position;
        public ILayerComponent            layerComponent;
        
        public EGID ID { get; set; }
    }

    public struct EnemyAttackEntityView:IEntityViewStruct
    {
        public IEnemyTriggerComponent    targetTriggerComponent;
        public EGID ID { get; set; }
    }

    public struct EnemyTargetEntityViewStruct : IEntityViewStruct
    {
        public IPosition targetPosition;
        public EGID ID { get; set; }
    }
}
