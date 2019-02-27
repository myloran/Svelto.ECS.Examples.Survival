using System.Collections;
using Svelto.ECS.Example.Survive.Characters.Player;

namespace Svelto.ECS.Example.Survive.Characters.Sounds {
    public class DamageTriggersSound : IQueryingEntitiesEngine, IStep<PlayerDeathCondition>, IStep {
        public IEntitiesDB entitiesDB { set; private get; }

        public void Ready() => CheckForDamage().Run();

        void TriggerDeathSound(EGID targetID) {
            var sound = entitiesDB.QueryEntitiesAndIndex<DamageSound>(targetID, out var index);
            sound[index].audioComponent.playOneShot = AudioType.death;
        }

        IEnumerator CheckForDamage() {
            while (true) {
                foreach (var group in ECSGroups.DamageableGroups) {
                    var damageables = entitiesDB.QueryEntities<Damageable>(group, out var count);
                    var sounds = entitiesDB.QueryEntities<DamageSound>(group, out count);
                    
                    for (var i = 0; i < count; i++) {
                        if (!damageables[i].damaged) continue;

                        sounds[i].audioComponent.playOneShot = AudioType.damage;
                    }
                }

                yield return null;
            }
        }

        public void Step(PlayerDeathCondition condition, EGID id) => TriggerDeathSound(id);
        public void Step(EGID id) => TriggerDeathSound(id);
    }
}