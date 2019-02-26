using System.Collections;
using Svelto.ECS.Example.Survive.Characters;
using Svelto.ECS.Example.Survive.Characters.Player;

namespace Svelto.ECS.Example.Survive {
    public class PlayerDies : IQueryingEntitiesEngine {
        public IEntitiesDB entitiesDB { get; set; }

        public PlayerDies(PlayerDeathFlow playerDeathSequence, IEntityFunctions functions) {
            _playerDeathSequence = playerDeathSequence;
            _functions = functions;
        }
        
        public void Ready() => CheckIfDead().Run();

        IEnumerator CheckIfDead() {
            while (true) {
                var players = entitiesDB.QueryEntities<Health>(ECSGroups.Player, out var count);
                
                for (var i = 0; i < count; i++) {
                    if (!players[i].dead) continue;
                    
                    _playerDeathSequence.Next(this, PlayerDeathCondition.Death, players[i].ID);
                    _functions.RemoveEntity<PlayerEntityDescriptor>(players[i].ID);
                }

                yield return null;
            }
        }

        readonly PlayerDeathFlow _playerDeathSequence;
        readonly IEntityFunctions _functions;
    }
}