using System.Collections;
using Svelto.Tasks;
using UnityEngine;

namespace Svelto.ECS.Example.Survive.Camera {
    //First step identify the entity type we want the engine to handle: CameraEntity
    //Second step name the engine according the behaviour and the entity: I.E.: CameraFollowTargetEngine
    //Third step start to write the code and create classes/fields as needed using refactoring tools 
    public class CameraFollowsTarget : Engine<Camera, CameraTarget>, IQueryingEntitiesEngine {
        public IEntitiesDB entitiesDB { get; set; }
        
        public CameraFollowsTarget(ITime time) {
            _time = time;
            _taskRoutine = TaskRunner.Instance.AllocateNewTaskRoutine(StandardSchedulers.physicScheduler);
            _taskRoutine.SetEnumerator(PhysicUpdate());
        }

        public void Ready() => _taskRoutine.Start();
        protected override void Add(ref Camera view) { }
        protected override void Remove(ref Camera view) => _taskRoutine.Stop();
        protected override void Add(ref CameraTarget view) { }
        protected override void Remove(ref CameraTarget view) => _taskRoutine.Stop();

        IEnumerator PhysicUpdate() {
            while (!entitiesDB.HasAny<CameraTarget>(ECSGroups.CameraTarget) ||
                   !entitiesDB.HasAny<Camera>(ECSGroups.ExtraStuff))
                yield return null; //skip a frame

            var targets = entitiesDB.QueryEntities<CameraTarget>(ECSGroups.CameraTarget, out _);
            var cameras = entitiesDB.QueryEntities<Camera>(ECSGroups.ExtraStuff, out _);
            var smoothing = 5.0f;
            var offset = cameras[0].position.position - targets[0].targetComponent.position;

            while (true) {
                var targetCameraPosition = targets[0].targetComponent.position + offset;

                cameras[0].transform.position = Vector3.Lerp(
                    cameras[0].position.position, targetCameraPosition, smoothing * _time.deltaTime);

                yield return null;
            }
        }

        readonly ITime _time;
        readonly ITaskRoutine<IEnumerator> _taskRoutine;
    }
}