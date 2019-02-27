namespace Svelto.ECS.Example.Survive.HUD {
    public class ScoreCalculates : IQueryingEntitiesEngine, IStep {
        public IEntitiesDB entitiesDB { get; set; }
        
        public void Ready() { }

        public void Step(EGID id) {
            var huds = entitiesDB.QueryEntities<HUD>(ECSGroups.ExtraStuff, out var count);
            if (count <= 0) return;

            var scores = entitiesDB.QueryEntitiesAndIndex<Score>(id, out var index);

            huds[0].scoreComponent.score += scores[index].score;
        }
    }

    public struct Score : IEntityStruct {
        public int score;
        public EGID ID { get; set; }
    }
}