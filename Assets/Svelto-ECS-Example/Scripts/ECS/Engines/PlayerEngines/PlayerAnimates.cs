using System.Collections;
using Svelto.Tasks;

namespace Svelto.ECS.Example.Survive.Characters.Player {
    public class PlayerAnimates : Engine<Player>, IQueryingEntitiesEngine, IStep<PlayerDeathCondition> {
        public IEntitiesDB entitiesDB { get; set; }

        public PlayerAnimates() {
            _taskRoutine = TaskRunner.Instance.AllocateNewTaskRoutine(StandardSchedulers.physicScheduler);
            _taskRoutine.SetEnumerator(PhysicsTick());
        }
        
        public void Ready() => _taskRoutine.Start();
        protected override void Add(ref Player view) { }
        protected override void Remove(ref Player view) => _taskRoutine.Stop();

        IEnumerator PhysicsTick() {
            while (!entitiesDB.HasAny<Player>(ECSGroups.Player)) //wait for the player to spawn
                yield return null; //skip a frame

            var players = entitiesDB.QueryEntities<Player>(ECSGroups.Player, out _);
            var inputs = entitiesDB.QueryEntities<Input>(ECSGroups.Player, out _);

            while (true) {
                var input = inputs[0].input;
                var walking = input.x != 0f || input.z != 0f; //Create a boolean that is true if either of the input axes is non-zero.
                
                players[0].animation.animationState = new AnimationState("IsWalking", walking); //Tell the animator whether or not the player is walking.

                yield return null;
            }
        }

        public void Step(PlayerDeathCondition condition, EGID id) {
            var players = entitiesDB.QueryEntitiesAndIndex<Player>(id, out var index);
            players[index].animation.playAnimation = "Die";
        }

        readonly ITaskRoutine<IEnumerator> _taskRoutine;
    }
}