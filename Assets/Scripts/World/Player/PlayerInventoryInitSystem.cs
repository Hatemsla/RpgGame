using Leopotam.EcsLite;
using Leopotam.EcsLite.Unity.Ugui;
using UnityEngine;
using Utils;

namespace World.Player
{
    public class PlayerInventoryInitSystem : IEcsInitSystem
    {
        
        [EcsUguiNamed(Idents.UI.InventoryViewContent)]
        private readonly GameObject _inventoryViewContent = default;
        
        public void Init(IEcsSystems systems)
        {
            
        }
    }
}