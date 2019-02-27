using System.Collections;

namespace Svelto.ECS.Example.Survive.Characters.Enemies {
    public class EnemyMoves : IQueryingEntitiesEngine {
        public IEntitiesDB entitiesDB { set; private get; }

        public void Ready() => Tick().Run();

        IEnumerator Tick() {
            while (true) {
                var targets = entitiesDB.QueryEntities<EnemyTarget>(ECSGroups.EnemyTargets, out var count); //query all the enemies from the standard group (no disabled nor respawning)
                if (count <= 0) {
                    yield return null;
                    continue;
                }
                
                var enemies = entitiesDB.QueryEntities<EnemyView>(ECSGroups.ActiveEnemies, out count);

                for (var i = 0; i < count; i++)
                    enemies[i].movement.navMeshDestination = targets[0].targetPosition.position;

                yield return null;
            }
        }
    }
}