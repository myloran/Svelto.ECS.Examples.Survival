using System.Collections;

namespace Svelto.ECS.Example.Survive.Characters {
    /// <summary>
    ///
    /// The responsibility of this engine is to apply the damage to any
    /// damageable entity. If the logic applied to the enemy was different
    /// than the logic applied to the player, I would have created two
    /// different engines
    /// 
    /// </summary>
    public class HealthTakesDamage : IQueryingEntitiesEngine {
        public IEntitiesDB entitiesDB { set; private get; }

        public void Ready() => ApplyDamage().Run();

        IEnumerator ApplyDamage() {
            while (true) {
                foreach (var group in ECSGroups.TargetGroups) {
                    var damageable = entitiesDB.QueryEntities<Damageable>(group, out var count);
                    var health = entitiesDB.QueryEntities<Health>(group, out count);

                    for (var i = 0; i < count; i++) {
                        if (damageable[i].damageInfo.shotDamageToApply <= 0) {
                            damageable[i].damaged = false;
                            continue;
                        }

                        health[i].current -= damageable[i].damageInfo.shotDamageToApply;
                        damageable[i].damageInfo.shotDamageToApply = 0;
                        damageable[i].damaged = true;
                    }
                }

                yield return null;
            }
        }
    }
}