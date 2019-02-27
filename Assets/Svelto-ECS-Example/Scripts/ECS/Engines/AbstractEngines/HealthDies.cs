using System.Collections;

namespace Svelto.ECS.Example.Survive.Characters {
    public class HealthDies : IQueryingEntitiesEngine {
        public IEntitiesDB entitiesDB { set; private get; }
        
        public void Ready() => CheckHealth().Run();

        IEnumerator CheckHealth() {
            while (true) {
                entitiesDB.ExecuteOnAllEntities(ECSGroups.DamageableGroups,
                    (ref Health health, IEntitiesDB _, int __) => {
                        if (health.current <= 0)
                            health.dead = true;
                    });

                yield return null;
            }
        }
    }
}