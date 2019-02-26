using System.Collections;
using Svelto.Tasks;
using UnityEngine;

namespace Svelto.ECS.Example.Survive.Characters.Enemies {
    public class EnemyAttacks : Engine<EnemyTarget>, IQueryingEntitiesEngine {
        public IEntitiesDB entitiesDB { set; private get; }

        public EnemyAttacks(ITime time) {
            _time = time;
            _taskRoutine = TaskRunner.Instance.AllocateNewTaskRoutine(StandardSchedulers.physicScheduler);
            _taskRoutine.SetEnumerator(CheckIfHittingEnemyTarget());
        }

        public void Ready() { }
        protected override void Add(ref EnemyTarget view) => _taskRoutine.Start();
        protected override void Remove(ref EnemyTarget view) => _taskRoutine.Stop();

        IEnumerator CheckIfHittingEnemyTarget() {
            while (true) {
                // Pay attention to this bit. The engine is querying a
                // EnemyTargetEntityView and not a PlayerEntityView.
                // this is more than a sophistication, it actually the implementation
                // of the rule that every engine must use its own set of
                // EntityViews to promote encapsulation and modularity
                while (entitiesDB.HasAny<Damageable>(ECSGroups.EnemyTargets) == false ||
                       entitiesDB.HasAny<EnemyAttack>(ECSGroups.ActiveEnemies) == false)
                    yield return null;

                var damageables = entitiesDB.QueryEntities<Damageable>(ECSGroups.EnemyTargets, out var damageableCount);
                var attacks = entitiesDB.QueryEntities<EnemyAttackStruct>(ECSGroups.ActiveEnemies, out var enemyCount);
                var enemies = entitiesDB.QueryEntities<EnemyAttack>(ECSGroups.ActiveEnemies, out enemyCount);

                //this is more complex than needed code is just to show how you can use entity structs
                //this case is banal, entity structs should be use to handle hundreds or thousands
                //of entities in a cache friendly and multi threaded code. However entity structs would allow
                //the creation of entity without any allocation, so they can be handy for
                //cases where entity should be built fast! Theoretically is possible to create
                //a game using only entity structs, but entity structs make sense ONLY if they
                //hold value types, so they come with a lot of limitations
                for (var i = 0; i < enemyCount; i++)
                    attacks[i].entityInRange = enemies[i].targetTrigger.entityInRange;

                for (var i = 0; i < damageableCount; i++) {
                    for (var j = 0; j < enemyCount; j++) {
                        if (!attacks[j].entityInRange.collides) continue;
                        //the IEnemyTriggerComponent implementors sets the collides boolean
                        //whenever anything enters in the trigger range, but there is not more logic
                        //we have to check here if the colliding entity is actually an EnemyTarget
                        if (attacks[j].entityInRange.otherEntityID != damageables[i].ID) continue;
                        
                        attacks[j].timer += _time.deltaTime;
                        if (!(attacks[j].timer >= attacks[j].timeBetweenAttack)) continue;
                        
                        attacks[j].timer = 0.0f;
                        damageables[i].damageInfo = new DamageInfo(attacks[j].attackDamage, Vector3.zero);
                    }
                }

                yield return null;
            }
        }

        readonly ITime _time;
        readonly ITaskRoutine<IEnumerator> _taskRoutine;
    }
}