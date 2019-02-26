namespace Svelto.ECS.Example.Survive.Characters.Player
{
    public struct Player : IEntityViewStruct
    {
        public ISpeed         speed;
        public IBody     body;
        public IPosition      position;
        public IAnimation     animation;
        public ITransform     transform;
        public EGID ID { get; set; }
    }
}

namespace Svelto.ECS.Example.Survive.Characters.Player.Gun
{
    public struct Gun : IEntityViewStruct
    {
        public IAttributes   attributes;
        public IFX           fx;
        public IIsHit    isHit;
        public EGID ID { get; set; }
    }
}
