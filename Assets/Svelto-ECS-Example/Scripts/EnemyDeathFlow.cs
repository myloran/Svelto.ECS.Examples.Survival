using Svelto.ECS.Example.Survive.Characters.Enemies;
using Svelto.ECS.Example.Survive.Characters.Sounds;
using Svelto.ECS.Example.Survive.HUD;

namespace Svelto.ECS.Example.Survive
{
    public class EnemyDeathFlow : Flow<EnemyDeathFlow>
    {
        public void SetSequence(EnemyDies     enemyDies, 
                                ScoreCalculates          scoreCalculates, 
                                DamageTriggersSound    damageTriggersSound, 
                                EnemyAnimates enemyAnimates, 
                                EnemySpawns   enemySpawns)
        {
            base.SetSequence(
                             new Steps //sequence of steps, this is a dictionary!
                                 (
                                  new Step
                                  {
                                      @from = enemyDies,
                                      to = new To
                                          (
                                           //TIP: use GO To Type Declaration to go directly to the Class code of the 
                                           //engine instance
                                           scoreCalculates, damageTriggersSound
                                          )
                                  },
                                  new Step
                                  {
                                      //second step
                                      @from = enemyAnimates, 
                                      //after the death animation is actually finished
                                      to = new To
                                          (
                                           enemySpawns //call the spawner engine
                                          )
                                  }
                                 )
                            );
        }
    }
}