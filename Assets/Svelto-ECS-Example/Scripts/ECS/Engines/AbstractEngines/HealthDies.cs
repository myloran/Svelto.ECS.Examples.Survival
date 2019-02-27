using System.Collections;

namespace Svelto.ECS.Example.Survive.Characters
{
    public class Dies:IQueryingEntitiesEngine
    {
        public void Ready()
        {
            CheckEnergy().Run();
        }

        IEnumerator CheckEnergy()
        {
            while (true)
            {
                entitiesDB.ExecuteOnAllEntities(ECSGroups.DamageableGroups,
                                                (ref Health health, IEntitiesDB entitiesdb, int index) =>
                        {
                            if (health.current <= 0)
                                health.dead = true;
                        });

                yield return null;
            }
        }

        public IEntitiesDB entitiesDB { set; private get; }
    }
}