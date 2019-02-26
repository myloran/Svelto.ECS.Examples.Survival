namespace Svelto.ECS.Example.Survive.Characters
{
    struct Damageable:IEntityStruct
    {
        public DamageInfo damageInfo;
        public bool       damaged;
        public EGID       ID { get; set; }
    }
}