using Svelto.ECS.Example.Survive.Characters.Enemies;
using Svelto.ECS.Example.Survive.Characters.Sounds;
using Svelto.ECS.Example.Survive.HUD;

namespace Svelto.ECS.Example.Survive {
    public class EnemyDeathFlow : Flow<EnemyDeathFlow> {
        public void SetSequence(EnemyDies enemyDies, ScoreCalculates scoreCalculates, DamageTriggersSound damageTriggersSound, EnemyAnimates enemyAnimates, EnemySpawns enemySpawns) {
            base.SetSequence(
                new Steps ( //sequence of steps, this is a dictionary!
                    new Step {
                        from = enemyDies,
                        to = new To (
                            scoreCalculates, damageTriggersSound //TIP: use GO To Type Declaration to go directly to the Class code of the engine instance
                        )
                    },
                    new Step { 
                        from = enemyAnimates, //second step 
                        to = new To ( //after the death animation is actually finished
                            enemySpawns //call the spawner engine
                        )
                    }
                )
            );
        }
    }
}