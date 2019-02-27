namespace Svelto.ECS.Example.Survive.HUD
{
    public struct HUD : IEntityViewStruct
    {
        public IAnimation      HUDAnimator;
        public IDamageHUDComponent      damageImageComponent;
        public IHealthSliderComponent   healthSliderComponent;
        public IScoreComponent          scoreComponent;
        public EGID ID { get; set; }
    }
}
