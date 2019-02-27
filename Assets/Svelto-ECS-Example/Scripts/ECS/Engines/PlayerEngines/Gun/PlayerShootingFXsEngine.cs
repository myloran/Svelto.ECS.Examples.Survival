using UnityEngine;
using System.Collections;
using System.Diagnostics;
using Svelto.Tasks;
using Svelto.Tasks.Enumerators;

namespace Svelto.ECS.Example.Survive.Characters.Player.Gun {
    public class PlayerGunShotSpawnsFX : Engine<Gun>, IQueryingEntitiesEngine {
        public IEntitiesDB entitiesDB { set; private get; }

        public void Ready() {
            //In this case a taskroutine is used because we want to have control over when it starts
            //and we want to reuse it.
            _taskRoutine = TaskRunner.Instance.AllocateNewTaskRoutine();
            _taskRoutine.SetEnumeratorProvider(DisableFxAfterTime);
        }

        /// <summary>
        /// Using the Add/Remove method to hold a local reference of an entity
        /// is not necessary. Do it only if you find convenient, otherwise
        /// querying is always cleaner.
        /// </summary>
        /// <param name="view"></param>
        protected override void Add(ref Gun view) {
            view.isHit.Bool.NotifyOnValueSet(PlayerHasShot);
            _waitForSeconds = new WaitForSecondsEnumerator(view.attributes.timeBetweenBullets * view.fx.effectsDisplayTime);
        }

        protected override void Remove(ref Gun view) { }

        void PlayerHasShot(int id, bool targetHasBeenHit) {
            var guns = entitiesDB.QueryEntitiesAndIndex<Gun>(new EGID(id, ECSGroups.Player), out var index);
            var attributes = guns[index].attributes;
            var shootRay = attributes.shootRay;

            var fx = guns[index].fx;
            fx.playAudio = true; //Play the gun shot audioclip.
            fx.lightEnabled = true; //Enable the light.
            fx.play = false; //Stop the particles from playing if they were, then start the particles.
            fx.play = true;
            fx.lineEnabled = true; //Enable the line renderer and set it's first position to be the end of the gun.
            fx.lineStartPosition = shootRay.origin;

            if (targetHasBeenHit) //Perform the raycast against gameobjects on the shootable layer and if it hits something...
                fx.lineEndPosition = attributes.lastTargetPosition; //Set the second position of the line renderer to the point the raycast hit.                                                              
            else
                fx.lineEndPosition = shootRay.origin + shootRay.direction * attributes.range; //... set the second position of the line renderer to the fullest extent of the gun's range.

            _taskRoutine.Start();
        }

        IEnumerator DisableFxAfterTime() {
            yield return _waitForSeconds;

            DisableEffects(); //... disable the effects.
        }

        void DisableEffects() {
            var guns = entitiesDB.QueryEntities<Gun>(ECSGroups.Player, out _);

            var fx = guns[0].fx;
            fx.lineEnabled = false; //Disable the line renderer and the light.
            fx.lightEnabled = false;
            fx.play = false;
        }

        ITaskRoutine<IEnumerator> _taskRoutine;
        WaitForSecondsEnumerator _waitForSeconds;
    }
}