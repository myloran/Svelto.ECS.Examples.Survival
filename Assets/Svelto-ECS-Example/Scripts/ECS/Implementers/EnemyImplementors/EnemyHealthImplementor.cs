using UnityEngine;

namespace Svelto.ECS.Example.Survive.Enemies
{
    public class EnemyHealthImplementor : MonoBehaviour, IImplementor, IDestroyComponent, IHealthComponent
    {
        public int startingHealth = 100;            // The amount of health the enemy starts the game with.
        public int currentHealth { get { return _currentHealth; } set { _currentHealth = value; } }

        void Awake ()
        {    // Setting the current health when the enemy first spawns.
            _currentHealth = startingHealth;
            
            mustDestroy = new DispatchOnChange<bool>(GetInstanceID());
            mustDestroy.NotifyOnValueSet(OnDestroyed);
        }

        void OnDestroyed(int sender, bool isDestroyed)
        {
            Destroy(gameObject);
        }

        public DispatchOnChange<bool> mustDestroy { get; private set; }

        int             _currentHealth;        // The current health the enemy has.
    }
}
