namespace Svelto.ECS.Example.Survive.Characters
{
    public struct Health : IEntityStruct
    {
        public int current;
        public bool dead;

        public EGID ID { get; set; }
    }
}