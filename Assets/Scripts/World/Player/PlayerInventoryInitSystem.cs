using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.Unity.Ugui;
using UnityEngine;
using Utils;
using World.Configurations;

namespace World.Player
{
    public class PlayerInventoryInitSystem : IEcsInitSystem
    {
        private EcsCustomInject<Configuration> _cf = default;
        
        [EcsUguiNamed(Idents.UI.PlayerInventoryViewContent)]
        private readonly GameObject _inventoryViewContent = default;
        
        public void Init(IEcsSystems systems)
        {
            
        }
    }
}