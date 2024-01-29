using UnityEngine;
using World.Ability;
using World.Ability.StatusEffects;
using World.Inventory;
using World.Player;

namespace World.Configurations
{
    [CreateAssetMenu(fileName = "MainConfiguration", menuName = "World Configurations/Main Configuration")]
    public class Configuration : ScriptableObject
    {
        public PlayerConfiguration playerConfiguration; 
        public UiConfiguration uiConfiguration;
        public AbilityConfiguration abilityConfiguration;
        public StatusEffectConfiguration statusEffectConfiguration;
        public InventoryConfiguration inventoryConfiguration;
        public ChestConfiguration chestConfiguration;
        public EnemyConfiguration enemyConfiguration;
    }
}