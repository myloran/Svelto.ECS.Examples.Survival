namespace Svelto.ECS.Example.Survive.Camera
{
    public struct Camera: IEntityViewStruct
    {
        public ITransform transform;
        public IPosition  position;
        
        public EGID ID { get; set; }
    }
}