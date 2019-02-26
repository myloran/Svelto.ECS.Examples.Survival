﻿using System.Collections;
using Svelto.Tasks;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace Svelto.ECS.Example.Survive.Characters.Player
{
    /// <summary>
    /// if you need to test input, you can mock this class
    /// alternatively you can mock the implementor.
    /// </summary>
    public class PlayerInputEngine:SingleEntityEngine<Player>, IQueryingEntitiesEngine
    {
        public IEntitiesDB entitiesDB { private get; set; }
        public void Ready()
        {}
        
        public PlayerInputEngine()
        {
            _taskRoutine = TaskRunner.Instance.AllocateNewTaskRoutine();
            _taskRoutine.SetEnumerator(ReadInput());
        }

        IEnumerator ReadInput()
        {
            //wait for the player to spawn
            while (entitiesDB.HasAny<Player>(ECSGroups.Player) == false)
            {
                yield return null; //skip a frame
            }
            
            int targetsCount;
            var playerEntityViews = entitiesDB.QueryEntities<PlayerInput>(ECSGroups.Player, out targetsCount);
           
            while (true)
            {
                float h = CrossPlatformInputManager.GetAxisRaw("Horizontal");
                float v = CrossPlatformInputManager.GetAxisRaw("Vertical");

                playerEntityViews[0].input = new Vector3(h, 0f, v);
                playerEntityViews[0].camRay = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);
                playerEntityViews[0].fire = Input.GetButton("Fire1");
                
                yield return null;
            }
        }

        protected override void Add(ref Player entityView)
        {
            _taskRoutine.Start();
        }

        protected override void Remove(ref Player entityView)
        {
            _taskRoutine.Stop();
        }

        readonly ITaskRoutine<IEnumerator> _taskRoutine;
    }
}