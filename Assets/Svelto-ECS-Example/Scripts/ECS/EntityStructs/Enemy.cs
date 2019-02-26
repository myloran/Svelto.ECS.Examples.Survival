using Svelto.ECS.Example.Survive.Characters.Player;

namespace Svelto.ECS.Example.Survive.Characters.Enemies
{
    struct Enemy:IEntityStruct
    {
        public PlayerTargetType enemyType;
        public EGID ID { get; set; }
    }
}