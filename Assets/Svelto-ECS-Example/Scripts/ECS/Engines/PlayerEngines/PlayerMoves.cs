using System.Collections;
using Svelto.Tasks;
using UnityEngine;

namespace Svelto.ECS.Example.Survive.Characters.Player {
    public class PlayerMoves : Engine<Player>, IQueryingEntitiesEngine, IStep<PlayerDeathCondition> {
        public IEntitiesDB entitiesDB { private get; set; }

        public PlayerMoves(IRayCaster raycaster, ITime time) {
            _rayCaster = raycaster;
            _time = time;
            _taskRoutine = TaskRunner.Instance.AllocateNewTaskRoutine(StandardSchedulers.physicScheduler);
            _taskRoutine.SetEnumerator(PhysicsTick());
        }

        public void Ready() { }
        protected override void Add(ref Player view) => _taskRoutine.Start();
        protected override void Remove(ref Player view) => _taskRoutine.Stop();

        IEnumerator PhysicsTick() {
            while (true) {
                var players = entitiesDB.QueryEntities<Player>(ECSGroups.Player, out var count);
                var inputs = entitiesDB.QueryEntities<Input>(ECSGroups.Player, out count);

                for (var i = 0; i < count; i++) {
                    Move(ref players[i], ref inputs[i]);
                    Turn(ref players[i], ref inputs[i]);
                }

                yield return null; //don't forget to yield or you will enter in an infinite loop!
            }
        }

        /// <summary>
        /// In order to keep the class testable, we need to reduce the number of
        /// dependencies injected through static classes at its minimum.
        /// Implementors are the place where platform dependencies can be transformed into
        /// entity components, so that here we can use inputComponent instead of
        /// the class Input.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="input"></param>
        void Move(ref Player player, ref Input input) {
            var movement = input.value.normalized * player.speed.movementSpeed * _time.deltaTime; //Normalise the movement vector and make it proportional to the speed per second.
            
            player.transform.position = player.position.position + movement; //Move the player to it's current position plus the movement.
        }

        void Turn(ref Player player, ref Input input) {
            if (!_rayCaster.CheckHit(input.camRay, camRayLength, floorMask, out var point)) return; //Create a ray from the mouse cursor on screen in the direction of the camera. Perform the raycast and if it hits something on the floor layer...

            var playerToMouse = point - player.position.position; //Create a vector from the player to the point on the floor the raycast from the mouse hit.
            playerToMouse.y = 0f; //Ensure the vector is entirely along the floor plane.

            player.transform.rotation = Quaternion.LookRotation(playerToMouse); //Create a quaternion (rotation) based on looking down the vector from the player to the mouse. Set the player's rotation to this new rotation.
        }

        public void Step(PlayerDeathCondition condition, EGID id) {
            var player = entitiesDB.QueryEntities<Player>(ECSGroups.Player, out _)[0];
            player.body.isKinematic = true;
        }

        readonly int floorMask = LayerMask.GetMask("Floor"); //A layer mask so that a ray can be cast just at gameobjects on the floor layer.

        const float camRayLength = 100f; //The length of the ray from the camera into the scene.

        readonly IRayCaster _rayCaster;
        readonly ITaskRoutine<IEnumerator> _taskRoutine;
        readonly ITime _time;
    }
}