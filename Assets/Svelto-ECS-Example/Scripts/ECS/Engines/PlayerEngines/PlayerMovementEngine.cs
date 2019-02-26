using System.Collections;
using Svelto.Tasks;
using UnityEngine;

namespace Svelto.ECS.Example.Survive.Characters.Player
{
    public class PlayerMovementEngine : Engine<Player>, IQueryingEntitiesEngine,
                                        IStep<PlayerDeathCondition>
    {
        public IEntitiesDB entitiesDB { private get; set; }
        
        public void Ready() {}
        
        public PlayerMovementEngine(IRayCaster raycaster, ITime time)
        {
            _rayCaster = raycaster;
            _time = time;
            _taskRoutine = TaskRunner.Instance.AllocateNewTaskRoutine(StandardSchedulers.physicScheduler);
            _taskRoutine.SetEnumerator(PhysicsTick());
        }

        protected override void Add(ref Player view) => _taskRoutine.Start();

        protected override void Remove(ref Player view) => _taskRoutine.Stop();

        IEnumerator PhysicsTick()
        {  
            while (true)
            {
                var players = entitiesDB.QueryEntities<Player>(ECSGroups.Player, out var count);
                var inputs  = entitiesDB.QueryEntities<PlayerInput>(ECSGroups.Player, out count);

                for (int i = 0; i < count; i++)
                {
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
        /// <param name="playerEntityView"></param>
        void Move(ref Player player, ref PlayerInput playerInput)
        {
            var input = playerInput.input; //Store the input axes.
            var movement = input.normalized * player.speed.movementSpeed * _time.deltaTime; //Normalise the movement vector and make it proportional to the speed per second.

            player.transform.position = player.position.position + movement; //Move the player to it's current position plus the movement.
        }

        void Turn(ref Player entityView, ref PlayerInput playerEntityView)
        {
            // Create a ray from the mouse cursor on screen in the direction of the camera.
            Ray camRay = playerEntityView.camRay;
            
            // Perform the raycast and if it hits something on the floor layer...
            Vector3 point;
            if (_rayCaster.CheckHit(camRay, camRayLength, floorMask, out point))
            {
                // Create a vector from the player to the point on the floor the raycast from the mouse hit.
                Vector3 playerToMouse = point - entityView.position.position;

                // Ensure the vector is entirely along the floor plane.
                playerToMouse.y = 0f;

                // Create a quaternion (rotation) based on looking down the vector from the player to the mouse.
                Quaternion newRotatation = Quaternion.LookRotation(playerToMouse);

                // Set the player's rotation to this new rotation.
                entityView.transform.rotation = newRotatation;
            }
        }

        public void Step(PlayerDeathCondition condition, EGID id)
        {
            int count;
            var playerEntityView = entitiesDB.QueryEntities<Player>(ECSGroups.Player, out count)[0]; 
            playerEntityView.body.isKinematic = true;
        }

        readonly int floorMask = LayerMask.GetMask("Floor");    // A layer mask so that a ray can be cast just at gameobjects on the floor layer.
        const float camRayLength = 100f;                        // The length of the ray from the camera into the scene.

        readonly IRayCaster   _rayCaster;
        readonly ITaskRoutine<IEnumerator> _taskRoutine;
        readonly ITime        _time;
    }
}
