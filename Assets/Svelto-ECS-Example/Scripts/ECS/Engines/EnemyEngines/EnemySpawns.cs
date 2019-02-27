using System.Collections;
using Svelto.Tasks.Enumerators;
using System.IO;
using Svelto.Common;

namespace Svelto.ECS.Example.Survive.Characters.Enemies {
    public class EnemySpawns : IQueryingEntitiesEngine, IStep {
        public IEntitiesDB entitiesDB { private get; set; }

        public EnemySpawns(IEnemyFactory enemyFactory, IEntityFunctions entityFunctions) {
            _entityFunctions = entityFunctions;
            _enemyFactory = enemyFactory;
            _numberOfEnemyToSpawn = 15;
        }

        public void Ready() => IntervaledTick().Run();

        IEnumerator IntervaledTick() {
//this is of fundamental importance: Never create implementors as Monobehaviour just to hold 
//data (especially if read only data). Data should always been retrieved through a service layer
//regardless the data source.
//The benefit are numerous, including the fact that changing data source would require
//only changing the service code. In this simple example I am not using a Service Layer
//but you can see the point.          
//Also note that I am loading the data only once per application run, outside the 
//main loop. You can always exploit this trick when you now that the data you need
//to use will never change            
            var spawns = ReadEnemySpawns();
            var attacks = ReadEnemyAttacks();
            var spawningTimes = new float[spawns.Length];

            for (var i = spawns.Length - 1; i >= 0 && _numberOfEnemyToSpawn > 0; --i)
                spawningTimes[i] = spawns[i].enemySpawnData.spawnTime;

            _enemyFactory.Preallocate();

            while (true) {
//Svelto.Tasks can yield Unity YieldInstructions but this comes with a performance hit
//so the fastest solution is always to use custom enumerators. To be honest the hit is minimal
//but it's better to not abuse it.                
                yield return _waitForSecondsEnumerator;

                var profiler = new PlatformProfiler();
                
                using (profiler.StartNewSession("EnemySpawning")) {
                    for (var i = spawns.Length - 1; i >= 0 && _numberOfEnemyToSpawn > 0; --i) { //cycle around the enemies to spawn and check if it can be spawned
                        if (spawningTimes[i] <= 0.0f) {
                            var spawnData = spawns[i];

                            //In this example every kind of enemy generates the same list of EntityViews
                            //therefore I always use the same EntityDescriptor. However if the 
                            //different enemies had to create different EntityViews for different
                            //engines, this would have been a good example where EntityDescriptorHolder
                            //could have been used to exploit the the kind of polymorphism explained
                            //in my articles.
                            var attack = new EnemyAttackStruct {
                                attackDamage = attacks[i].enemyAttackData.attackDamage,
                                timeBetweenAttack = attacks[i].enemyAttackData.timeBetweenAttacks
                            };

                            //has got a compatible entity previously disabled and can be reused?
                            //Note, pooling make sense only for Entities that use implementors.
                            //A pure struct based entity doesn't need pooling because it never allocates.
                            //to simplify the logic, we use a recycle group for each entity type
                            var groupId = (int)ECSGroups.EnemiesToRecycleGroups + (int)spawnData.enemySpawnData.targetType;

                            if (entitiesDB.HasAny<EnemyView>(groupId))
                                using (profiler.Sample("Recycling"))
                                    ReuseEnemy(groupId, spawnData);
                            else
                                using (profiler.Sample("Building"))
                                    _enemyFactory.Build(spawnData.enemySpawnData, ref attack);

                            spawningTimes[i] = spawnData.enemySpawnData.spawnTime;
                            _numberOfEnemyToSpawn--;
                        }

                        spawningTimes[i] -= SECONDS_BETWEEN_SPAWNS;
                    }
                }
            }
        }

        /// <summary>
        /// Reset all the component values when an Enemy is ready to be recycled.
        /// it's important to not forget to reset all the states.
        /// note that the only reason why we pool it the entities here is to reuse the implementors,
        /// pure entity structs entities do not need pool and can be just recreated
        /// </summary>
        /// <param name="spawnData"></param>
        /// <returns></returns>

        void ReuseEnemy(int fromGroupId, JSonEnemySpawnData spawnData) {
            var healths = entitiesDB.QueryEntities<Health>(fromGroupId, out var count);
            if (count <= 0) return;

            healths[0].current = 100;
            healths[0].dead = false;

            var enemies = entitiesDB.QueryEntities<EnemyView>(fromGroupId, out count);
            enemies[0].transform.position = spawnData.enemySpawnData.spawnPoint;
            enemies[0].movement.navMeshEnabled = true;
            enemies[0].movement.setCapsuleAsTrigger = false;
            enemies[0].layerComponent.layer = GAME_LAYERS.ENEMY_LAYER;
            enemies[0].animation.reset = true;

            _entityFunctions.SwapEntityGroup<EnemyEntityDescriptor>(enemies[0].ID, ECSGroups.ActiveEnemies);
        }

        static JSonEnemySpawnData[] ReadEnemySpawns() {
            var json = File.ReadAllText("EnemySpawningData.json");
            return JsonHelper.getJsonArray<JSonEnemySpawnData>(json);
        }

        static JSonEnemyAttackData[] ReadEnemyAttacks() {
            var json = File.ReadAllText("EnemyAttackData.json");
            return JsonHelper.getJsonArray<JSonEnemyAttackData>(json);
        }

        public void Step(EGID id) => _numberOfEnemyToSpawn++;

        const int SECONDS_BETWEEN_SPAWNS = 1;
        readonly WaitForSecondsEnumerator _waitForSecondsEnumerator = new WaitForSecondsEnumerator(SECONDS_BETWEEN_SPAWNS);
        readonly IEnemyFactory _enemyFactory;
        readonly IEntityFunctions _entityFunctions;
        int _numberOfEnemyToSpawn;
    }
}