using UnityEngine;

namespace Svelto.ECS.Example.Survive.Characters.Player
{
    public struct PlayerInput : IEntityStruct
    {
        public Vector3 input;
        public Ray     camRay;
        public bool    fire;
        public EGID    ID { get; set; }
    }
}