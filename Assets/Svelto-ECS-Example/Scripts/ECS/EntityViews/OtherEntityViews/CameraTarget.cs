namespace Svelto.ECS.Example.Survive.Camera
{
    public struct CameraTarget: IEntityViewStruct
    {
        public ICameraTargetComponent targetComponent;
        public EGID ID { get; set; }
    }
}