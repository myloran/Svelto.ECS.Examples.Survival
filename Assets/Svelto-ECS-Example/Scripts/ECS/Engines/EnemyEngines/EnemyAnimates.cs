using System;
using System.Collections;
using Svelto.ECS.Example.Survive.Characters.Player;

namespace Svelto.ECS.Example.Survive.Characters.Enemies {
    public class EnemyAnimates : IQueryingEntitiesEngine, IStep<PlayerDeathCondition> {
        public IEntitiesDB entitiesDB { set; private get; }

        public EnemyAnimates(ITime time, EnemyDeathFlow enemyDeadOrder, IEntityFunctions entityFunctions) {
            _time = time;
            _enemyDeadFlow = enemyDeadOrder;
            _entityFunctions = entityFunctions;
        }

        public void Ready() {
            AnimateOnDamage().Run();
            AnimateOnDeath().Run();
        }

        public void Step(PlayerDeathCondition condition, EGID id) {
            var enemy = entitiesDB.QueryEntities<EnemyView>(ECSGroups.ActiveEnemies, out var count); //if player is dead, the enemy cheers

            for (var i = 0; i < count; i++)
                enemy[i].animation.playAnimation = "PlayerDead";
        }

        IEnumerator AnimateOnDamage() {
            while (true) {
                var damageables = entitiesDB.QueryEntities<Damageable>(ECSGroups.ActiveEnemies, out var count);
                var enemies = entitiesDB.QueryEntities<EnemyView>(ECSGroups.ActiveEnemies, out count);

                for (var i = 0; i < count; i++) {
                    if (damageables[i].damaged == false) continue;

                    enemies[i].vfx.position = damageables[i].damageInfo.damagePoint;
                    enemies[i].vfx.play = true;
                }

                yield return null;
            }
        }

        IEnumerator AnimateOnDeath() {
            while (true) {
                var enemies = entitiesDB.QueryEntities<EnemyView>(ECSGroups.DeadEnemiesGroups, out var count);
                var sinks = entitiesDB.QueryEntities<EnemySink>(ECSGroups.DeadEnemiesGroups, out count);

                for (var i = 0; i < count; i++) {
                    if (enemies[i].animation.playAnimation != "Dead") {
                        enemies[i].animation.playAnimation = "Dead";
                        sinks[i].animationTime = DateTime.UtcNow.AddSeconds(2);
                        continue;
                    }

                    if (DateTime.UtcNow < sinks[i].animationTime) {
                        enemies[i].transform.position = enemies[i].position.position
                            - UnityEngine.Vector3.up * sinks[i].sinkAnimSpeed * _time.deltaTime;
                        continue;
                    }

                    var enemyStructs = entitiesDB.QueryEntities<Enemy>(ECSGroups.DeadEnemiesGroups, out count);
                    
                    _entityFunctions.SwapEntityGroup<EnemyEntityDescriptor>(enemies[i].ID, 
                        ECSGroups.EnemiesToRecycleGroups + (int) enemyStructs[i].enemyType);

                    _enemyDeadFlow.Next(this, enemies[i].ID);
                }

                yield return null;
            }
        }

        readonly ITime _time;
        readonly EnemyDeathFlow _enemyDeadFlow;
        readonly IEntityFunctions _entityFunctions;
    }
}