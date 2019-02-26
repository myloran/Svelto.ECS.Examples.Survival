namespace Svelto.ECS.Example.Survive.Camera
{
    public struct CameraEntityView: IEntityViewStruct
    {
        public ITransform transform;
        public IPosition  position;
        
        public EGID ID { get; set; }
    }
}