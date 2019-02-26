using System.Collections;

namespace Svelto.ECS.Example.Survive.Characters.Enemies
{
    public class EnemyDies : IQueryingEntitiesEngine
    {
        public EnemyDies(IEntityFunctions entityFunctions, EnemyDeathFlow enemyDeadFlow)
        {
            _entityFunctions = entityFunctions;

            _enemyDeadFlow = enemyDeadFlow;
        }

        public IEntitiesDB entitiesDB { get; set; }

        public void Ready()
        {
            CheckIfDead().Run();
        }

        IEnumerator CheckIfDead()
        {
            while (true)
            {
                //wait for enemies to be created
                while (entitiesDB.HasAny<Enemy>(ECSGroups.ActiveEnemies) == false) yield return null;

                //Groups affect the memory layour. Entity views are split according groups, so that even if entity
                //views are used by entities outside a specific group, those entity views won't be present 
                //in the array returned by QueryEntities.
                int count;
                var enemyEntitiesViews =
                    entitiesDB.QueryEntities<EnemyView>(ECSGroups.ActiveEnemies, out count);
                var enemyEntitiesHealth =
                    entitiesDB.QueryEntities<Health>(ECSGroups.ActiveEnemies, out count);

                for (int index = 0; index < count; index++)
                {
                    if (enemyEntitiesHealth[index].dead == false) continue;

                    SetParametersForDeath(ref enemyEntitiesViews[index]);
                    
                    _enemyDeadFlow.Next(this, enemyEntitiesViews[index].ID);

                    _entityFunctions.SwapEntityGroup<EnemyEntityDescriptor>(enemyEntitiesViews[index].ID, ECSGroups.DeadEnemiesGroups);
                }

                yield return null;
            }
        }

        static void SetParametersForDeath(ref EnemyView enemyView)
        {
            enemyView.layerComponent.layer                  = GAME_LAYERS.NOT_SHOOTABLE_MASK;
            enemyView.movement.navMeshEnabled      = false;
            enemyView.movement.setCapsuleAsTrigger = true;
        }

        readonly IEntityFunctions    _entityFunctions;
        readonly EnemyDeathFlow _enemyDeadFlow;
    }
}
