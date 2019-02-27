using System.Collections;

namespace Svelto.ECS.Example.Survive.Characters.Enemies {
    public class EnemyDies : IQueryingEntitiesEngine {
        public IEntitiesDB entitiesDB { get; set; }
        
        public EnemyDies(IEntityFunctions entityFunctions, EnemyDeathFlow enemyDeadFlow) {
            _entityFunctions = entityFunctions;
            _enemyDeadFlow = enemyDeadFlow;
        }

        public void Ready() => CheckIfDead().Run();

        IEnumerator CheckIfDead() {
            while (true) {
                while (!entitiesDB.HasAny<Enemy>(ECSGroups.ActiveEnemies)) 
                    yield return null; //wait for enemies to be created

                //Groups affect the memory layour. Entity views are split according groups, so that even if entity
                //views are used by entities outside a specific group, those entity views won't be present 
                //in the array returned by QueryEntities.
                var enemies = entitiesDB.QueryEntities<EnemyView>(ECSGroups.ActiveEnemies, out var count);
                var healths = entitiesDB.QueryEntities<Health>(ECSGroups.ActiveEnemies, out count);

                for (var i = 0; i < count; i++) {
                    if (!healths[i].dead) continue;

                    SetParametersForDeath(ref enemies[i]);

                    _enemyDeadFlow.Next(this, enemies[i].ID);
                    _entityFunctions.SwapEntityGroup<EnemyEntityDescriptor>(enemies[i].ID, ECSGroups.DeadEnemiesGroups);
                }

                yield return null;
            }
        }

        static void SetParametersForDeath(ref EnemyView enemy) {
            enemy.layerComponent.layer = GAME_LAYERS.NOT_SHOOTABLE_MASK;
            enemy.movement.navMeshEnabled = false;
            enemy.movement.setCapsuleAsTrigger = true;
        }

        readonly IEntityFunctions _entityFunctions;
        readonly EnemyDeathFlow _enemyDeadFlow;
    }
}