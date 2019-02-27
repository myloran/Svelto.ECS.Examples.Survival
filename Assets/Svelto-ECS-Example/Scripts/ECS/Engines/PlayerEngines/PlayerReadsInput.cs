using System.Collections;
using Svelto.Tasks;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace Svelto.ECS.Example.Survive.Characters.Player {
    /// <summary>
    /// if you need to test input, you can mock this class
    /// alternatively you can mock the implementor.
    /// </summary>
    public class PlayerReadsInput : Engine<Player>, IQueryingEntitiesEngine {
        public IEntitiesDB entitiesDB { private get; set; }
        
        public PlayerReadsInput() {
            _taskRoutine = TaskRunner.Instance.AllocateNewTaskRoutine();
            _taskRoutine.SetEnumerator(ReadInput());
        }
        
        public void Ready() { }
        protected override void Add(ref Player view) => _taskRoutine.Start();
        protected override void Remove(ref Player view) => _taskRoutine.Stop();

        IEnumerator ReadInput() {
            while (!entitiesDB.HasAny<Player>(ECSGroups.Player)) //wait for the player to spawn
                yield return null; //skip a frame

            var inputs = entitiesDB.QueryEntities<Input>(ECSGroups.Player, out _);

            while (true) {
                var horizontal = CrossPlatformInputManager.GetAxisRaw("Horizontal");
                var vertical = CrossPlatformInputManager.GetAxisRaw("Vertical");

                inputs[0].input = new Vector3(horizontal, 0f, vertical);
                inputs[0].camRay = UnityEngine.Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
                inputs[0].fire = UnityEngine.Input.GetButton("Fire1");

                yield return null;
            }
        }
        
        readonly ITaskRoutine<IEnumerator> _taskRoutine;
    }
}