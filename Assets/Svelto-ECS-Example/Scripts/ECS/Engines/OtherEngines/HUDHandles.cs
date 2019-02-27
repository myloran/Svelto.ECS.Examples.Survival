using Svelto.Tasks.Enumerators;
using System.Collections;
using Svelto.ECS.Example.Survive.Characters;
using Svelto.ECS.Example.Survive.Characters.Player;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Svelto.ECS.Example.Survive.HUD {
    public class HUDHandles : IQueryingEntitiesEngine, IStep<PlayerDeathCondition> {
        public IEntitiesDB entitiesDB { set; private get; }

        public HUDHandles(ITime time) {
            _time = time;
        }

        public void Ready() {
            AnimateUI().Run();
            CheckForDamage().Run();
        }

        IEnumerator AnimateUI() {
            while (true) {
                entitiesDB.ExecuteOnEntities(ECSGroups.ExtraStuff, ref _time,
                    (ref HUD hud, ref ITime time, IEntitiesDB _, int __) => {
                        var damage = hud.damageImageComponent;
                        damage.imageColor = Color.Lerp(damage.imageColor, Color.clear, damage.speed * time.deltaTime);
                    });

                yield return null;
            }
        }

        /// <summary>
        /// the damaged flag is polled. I am still torn about the poll vs push problem, so more investigation is needed
        /// Maybe solved in future with the refactored version of DispatchOnSet/Change 
        /// </summary>
        /// <param name="entitiesDb"></param>
        IEnumerator CheckForDamage() {
            while (true) {
                var damageables = entitiesDB.QueryEntities<Damageable>(ECSGroups.Player, out var count);
                var healths = entitiesDB.QueryEntities<Health>(ECSGroups.Player, out count);
                
                for (var i = 0; i < count; i++) {
                    if (!damageables[i].damaged) continue;

                    //An engine should never assume how many entities will be used, so we iterate over all the
                    //HUDEntityViews even if we know there is just one
                    entitiesDB.ExecuteOnEntities(ECSGroups.ExtraStuff, ref healths[i].current,
                        (ref HUD hud, ref int health, IEntitiesDB _, int __) => {
                            var damage = hud.damageImageComponent;
                            damage.imageColor = damage.flashColor;

                            hud.healthSliderComponent.value = health;
                        });
                }

                yield return null;
            }
        }

        IEnumerator RestartLevelAfterFewSeconds() {
            _waitForSeconds.Reset(5);
            yield return _waitForSeconds;

            var huds = entitiesDB.QueryEntities<HUD>(ECSGroups.ExtraStuff, out var count);
            
            for (var i = 0; i < count; i++)
                huds[i].HUDAnimator.playAnimation = "GameOver";

            _waitForSeconds.Reset(2);
            yield return _waitForSeconds;

            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void Step(PlayerDeathCondition condition, EGID id) {
            var huds = entitiesDB.QueryEntities<HUD>(ECSGroups.ExtraStuff, out var count);
            
            for (var i = 0; i < count; i++)
                huds[i].healthSliderComponent.value = 0;

            RestartLevelAfterFewSeconds().Run();
        }

        readonly WaitForSecondsEnumerator _waitForSeconds = new WaitForSecondsEnumerator(5);
        ITime _time;
    }
}