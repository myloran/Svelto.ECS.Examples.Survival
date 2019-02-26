using UnityEngine;
using System.Collections;
using System.Diagnostics;
using Svelto.Tasks;
using Svelto.Tasks.Enumerators;

namespace Svelto.ECS.Example.Survive.Characters.Player.Gun
{
    public class PlayerGunShootingFXsEngine : Engine<Gun>, IQueryingEntitiesEngine
    {
        public IEntitiesDB entitiesDB { set; private get; }

        public void Ready()
        {
            //In this case a taskroutine is used because we want to have control over when it starts
            //and we want to reuse it.
            _taskRoutine = TaskRunner.Instance.AllocateNewTaskRoutine();
                _taskRoutine.SetEnumeratorProvider(DisableFXAfterTime);
        }

        /// <summary>
        /// Using the Add/Remove method to hold a local reference of an entity
        /// is not necessary. Do it only if you find convenient, otherwise
        /// querying is always cleaner.
        /// </summary>
        /// <param name="view"></param>
        protected override void Add(ref Gun view)
        {
            view.isHit.Bool.NotifyOnValueSet(PlayerHasShot);
            
            _waitForSeconds = new WaitForSecondsEnumerator(view.attributes.timeBetweenBullets * view.fx.effectsDisplayTime);
        }

        protected override void Remove(ref Gun view)
        {}

        void PlayerHasShot(int ID, bool targetHasBeenHit)
        {
            uint index;
            var structs = entitiesDB.QueryEntitiesAndIndex<Gun>(new EGID(ID, ECSGroups.Player), out index);

            var gunFXComponent = structs[index].fx;

            // Play the gun shot audioclip.
            gunFXComponent.playAudio = true;

            // Enable the light.
            gunFXComponent.lightEnabled = true;

            // Stop the particles from playing if they were, then start the particles.
            gunFXComponent.play = false;
            gunFXComponent.play = true;

            var gunComponent = structs[index].attributes;
            var shootRay = gunComponent.shootRay;

            // Enable the line renderer and set it's first position to be the end of the gun.
            gunFXComponent.lineEnabled = true;
            gunFXComponent.lineStartPosition = shootRay.origin;

            // Perform the raycast against gameobjects on the shootable layer and if it hits something...
            if (targetHasBeenHit == true)
            {
                // Set the second position of the line renderer to the point the raycast hit.
                gunFXComponent.lineEndPosition = gunComponent.lastTargetPosition;
            }
            // If the raycast didn't hit anything on the shootable layer...
            else
            {
                // ... set the second position of the line renderer to the fullest extent of the gun's range.
                 gunFXComponent.lineEndPosition = shootRay.origin + shootRay.direction * gunComponent.range;
            }

            _taskRoutine.Start();
        }

        IEnumerator DisableFXAfterTime()
        {   
            yield return _waitForSeconds;
            // ... disable the effects.
            DisableEffects();
        }

        void DisableEffects ()
        {
            int targetsCount;
            var gunEntityViews = entitiesDB.QueryEntities<Gun>(ECSGroups.Player, out targetsCount);

            var fxComponent = gunEntityViews[0].fx;
            // Disable the line renderer and the light.
            fxComponent.lineEnabled = false;
            fxComponent.lightEnabled = false;
            fxComponent.play = false;
        }

        ITaskRoutine<IEnumerator>             _taskRoutine;
        WaitForSecondsEnumerator _waitForSeconds;
    }
}
