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
			new EntityBuilder<Damageable>(),
			new EntityBuilder<DamageSound>(),
			new EntityBuilder<CameraTargetEntityView>(),
			new EntityBuilder<Health>(),
			new EntityBuilder<EnemyTarget>(),
			new EntityBuilder<Input>()
		};

		public IEntityBuilder[] entitiesToBuild
		{
			get { return _entitiesToBuild; }
		}
	}
}
