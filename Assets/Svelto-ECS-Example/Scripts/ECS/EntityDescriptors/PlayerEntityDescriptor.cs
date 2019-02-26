using Svelto.ECS.Example.Survive.Camera;
using Svelto.ECS.Example.Survive.Characters.Enemies;
using Svelto.ECS.Example.Survive.Characters.Sounds;

namespace Svelto.ECS.Example.Survive.Characters.Player
{
	public class PlayerEntityDescriptor : IEntityDescriptor
	{
		static readonly IEntityBuilder[] _entitiesToBuild =
		{
			new EntityBuilder<Player>(),
			new EntityBuilder<DamageableEntityStruct>(),
			new EntityBuilder<DamageSoundEntityView>(),
			new EntityBuilder<CameraTargetEntityView>(),
			new EntityBuilder<HealthEntityStruct>(),
			new EntityBuilder<EnemyTargetEntityViewStruct>(),
			new EntityBuilder<Input>()
		};

		public IEntityBuilder[] entitiesToBuild
		{
			get { return _entitiesToBuild; }
		}
	}
}
