using System.Collections;

namespace Svelto.ECS.Example.Survive.Characters.Enemies
{
    public class EnemyMoves : IQueryingEntitiesEngine
    {
        public IEntitiesDB entitiesDB { set; private get; }

        public void Ready()
        {
            Tick().Run();
        }

        IEnumerator Tick()
        {
            while (true)
            {
                int count;
                //query all the enemies from the standard group (no disabled nor respawning)
                var enemyTargetEntityViews = entitiesDB.QueryEntities<EnemyTarget>(ECSGroups.EnemyTargets, out count);

                if (count > 0)
                {
                    var enemies = entitiesDB.QueryEntities<EnemyView>(ECSGroups.ActiveEnemies, out count);

                    for (var i = 0; i < count; i++)
                    {
                        enemies[i].movement.navMeshDestination =
                            enemyTargetEntityViews[0].targetPosition.position;
                    }
                }

                yield return null;
            }
        }
    }
}
