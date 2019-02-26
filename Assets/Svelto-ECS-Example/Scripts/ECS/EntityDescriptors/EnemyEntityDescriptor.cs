using Svelto.ECS.Example.Survive.Characters.Sounds;
using Svelto.ECS.Example.Survive.HUD;

namespace Svelto.ECS.Example.Survive.Characters.Enemies
{
    public class EnemyEntityDescriptor : IEntityDescriptor
    {
        static readonly IEntityBuilder[] _entitiesToBuild = {
                                                                new EntityBuilder <Enemy>(),
                                                                new EntityBuilder <EnemyView>(),
                                                                new EntityBuilder <EnemyAttack>(),
                                                                new EntityBuilder <DamageSoundEntityView>(),
                                                                new EntityBuilder <EnemyAttackStruct>(),
                                                                new EntityBuilder <HealthEntityStruct>(),
                                                                new EntityBuilder <ScoreValueEntityStruct>(),
                                                                new EntityBuilder<EnemySink>(), 
                                                                new EntityBuilder <Damageable>()};
        public IEntityBuilder[] entitiesToBuild
        {
            get { return _entitiesToBuild; }
        }
    }
}