using System.Collections;
using UnityEngine;
using Svelto.Tasks;

namespace Svelto.ECS.Example.Survive.Characters.Player.Gun
{
    public class PlayerGunShooting : Engine<Gun, Player>, 
        IQueryingEntitiesEngine
    {
        public IEntitiesDB entitiesDB { set; private get; }

        public void Ready()
        {
            _taskRoutine.Start();
        }
        
        public PlayerGunShooting(IRayCaster rayCaster, ITime time)
        {
            _rayCaster             = rayCaster;
            _time                  = time;
            _taskRoutine           = TaskRunner.Instance.AllocateNewTaskRoutine(StandardSchedulers.physicScheduler);
            _taskRoutine.SetEnumerator(Tick());
        }

        protected override void Add(ref Gun entityView)
        {}

        protected override void Remove(ref Gun entityView)
        {
            _taskRoutine.Stop();
        }

        protected override void Add(ref Player entityView)
        {}

        protected override void Remove(ref Player entityView)
        {
            _taskRoutine.Stop();
        }

        IEnumerator Tick()
        {
            while (entitiesDB.HasAny<Player>(ECSGroups.Player) == false ||
                   entitiesDB.HasAny<Gun>(ECSGroups.Player) == false)
            {
                yield return null; //skip a frame
            }

            int count;
            var gun = entitiesDB.QueryEntities<Gun>(ECSGroups.Player, out count)[0]; //never changes
            var input = entitiesDB.QueryEntities<PlayerInput>(ECSGroups.Player, out count)[0]; //never change
            
            while (true)
            {
                var attributes = gun.attributes;
                attributes.timer += _time.deltaTime;

                if (input.fire && attributes.timer >= attributes.timeBetweenBullets) 
                    Shoot(gun);

                yield return null;
            }
        }

        /// <summary>
        /// Design note: shooting and find a target are possibly two different responsibilities
        /// and probably would need two different engines. 
        /// </summary>
        /// <param name="gun"></param>
        void Shoot(Gun gun)
        {
            var attributes    = gun.attributes;
            var playerGunHit = gun.gunHitTarget;

            attributes.timer = 0;

            Vector3 point;
            int instanceID;
            var entityHit = _rayCaster.CheckHit(attributes.shootRay,
                                                attributes.range,
                                                GAME_LAYERS.ENEMY_LAYER,
                                                GAME_LAYERS.SHOOTABLE_MASK | GAME_LAYERS.ENEMY_MASK,
                                                out point, out instanceID);
            
            if (entityHit)
            {
                var damageInfo =
                    new
                        DamageInfo(attributes.damagePerShot,
                                   point);
                
                //note how the GameObject GetInstanceID is used to identify the entity as well
                if (instanceID != -1)
                    entitiesDB.ExecuteOnEntity(instanceID, ECSGroups.PlayerTargets, ref damageInfo,
                                               (ref DamageableEntityStruct entity, ref DamageInfo info) => //
                                               { //never catch external variables so that the lambda doesn't allocate
                                                   entity.damageInfo = info;
                                               });

                attributes.lastTargetPosition = point;
                playerGunHit.targetHit.value = true;
            }
            else
                playerGunHit.targetHit.value = false;
        }

        readonly IRayCaster            _rayCaster;
        readonly ITime                 _time;
        readonly ITaskRoutine<IEnumerator>          _taskRoutine;
    }
}