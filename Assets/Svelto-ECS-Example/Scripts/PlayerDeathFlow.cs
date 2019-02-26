using Svelto.ECS.Example.Survive.Characters.Enemies;
using Svelto.ECS.Example.Survive.Characters.Player;
using Svelto.ECS.Example.Survive.Characters.Sounds;
using Svelto.ECS.Example.Survive.HUD;

namespace Svelto.ECS.Example.Survive {
    public class PlayerDeathFlow : Flow<PlayerDeathFlow> {
        public void SetSequence(PlayerDies playerDies, PlayerMoves playerMoves, PlayerAnimates playerAnimates, EnemyAnimates enemyAnimates, DamageTriggersSound damageTriggersSound, HUDHandles hudHandles) {
            base.SetSequence(
                new Steps ( //sequence of steps, this is a dictionary!
                    new Step {
                        from = playerDies, //when the player dies
                        to = new To<PlayerDeathCondition> { //all these engines in the list will be called in order (which in this case was not important at all, so stretched!!)
                            { PlayerDeathCondition.Death, playerMoves, playerAnimates, enemyAnimates, damageTriggersSound, hudHandles }
                        }
                    }
                ));
        }
    }
}