using System.Collections;
using UnityEngine;
using Svelto.Tasks;

namespace Svelto.ECS.Example.Survive.Characters.Player.Gun {
    public class ShootingPlayerGun : Engine<Gun, Player>, IQueryingEntitiesEngine {
        public IEntitiesDB entitiesDB { set; private get; }

        public ShootingPlayerGun(IRayCaster rayCaster, ITime time) {
            _rayCaster = rayCaster;
            _time = time;
            _taskRoutine = TaskRunner.Instance.AllocateNewTaskRoutine(StandardSchedulers.physicScheduler);
            _taskRoutine.SetEnumerator(Tick());
        }

        public void Ready() => _taskRoutine.Start();
        protected override void Add(ref Gun view) { }
        protected override void Remove(ref Gun view) => _taskRoutine.Stop();
        protected override void Add(ref Player view) { }
        protected override void Remove(ref Player view) => _taskRoutine.Stop();

        IEnumerator Tick() {
            while (entitiesDB.HasAny<Player>(ECSGroups.Player) == false ||
                   entitiesDB.HasAny<Gun>(ECSGroups.Player) == false) {
                yield return null; //skip a frame
            }

            var guns = entitiesDB.QueryEntities<Gun>(ECSGroups.Player, out _); //never changes
            var inputs = entitiesDB.QueryEntities<Input>(ECSGroups.Player, out _); //never change

            while (true) {
                var attributes = guns[0].attributes;
                attributes.timer += _time.deltaTime;

                if (inputs[0].fire && attributes.timer >= attributes.timeBetweenBullets)
                    Shoot(guns[0]);

                yield return null;
            }
        }

        /// <summary>
        /// Design note: shooting and find a target are possibly two different responsibilities
        /// and probably would need two different engines. 
        /// </summary>
        /// <param name="gun"></param>
        void Shoot(Gun gun) {
            var attributes = gun.attributes;
            attributes.timer = 0;

            var isHit = _rayCaster.CheckHit(attributes.shootRay,
                attributes.range,
                GAME_LAYERS.ENEMY_LAYER,
                GAME_LAYERS.SHOOTABLE_MASK | GAME_LAYERS.ENEMY_MASK,
                out var point, out var instanceId);

            if (isHit) {
                var damageInfo = new DamageInfo(attributes.damagePerShot, point);

                if (instanceId != -1) //note how the GameObject GetInstanceID is used to identify the entity as well
                    entitiesDB.ExecuteOnEntity(instanceId, ECSGroups.PlayerTargets, ref damageInfo,
                        (ref DamageableEntityStruct entity, ref DamageInfo info) => entity.damageInfo = info);

                attributes.lastTargetPosition = point;
            }

            gun.isHit.Bool.value = isHit;
        }

        readonly IRayCaster _rayCaster;
        readonly ITime _time;
        readonly ITaskRoutine<IEnumerator> _taskRoutine;
    }
}