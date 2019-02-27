using Svelto.ECS.Unity;
using UnityEngine;

namespace Svelto.ECS.Example.Survive.HUD
{
	public class HudEntityDescriptor : GenericEntityDescriptor<HUD>
	{}
	
    [DisallowMultipleComponent]
	public class HudEntityDescriptorHolder : GenericEntityDescriptorHolder<HudEntityDescriptor>
	{}
}
