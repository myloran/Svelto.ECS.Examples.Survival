namespace Svelto.ECS.Example.Survive.Characters.Sounds
{
    public struct DamageSound: IEntityViewStruct
    {
        public IDamageSoundComponent    audioComponent;
        public EGID ID { get; set; }
    }
}
