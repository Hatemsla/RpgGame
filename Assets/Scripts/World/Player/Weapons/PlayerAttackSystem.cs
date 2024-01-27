using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using Utils;
using World.Inventory;
using World.Inventory.ItemTypes.Weapons;
using World.Inventory.WeaponObject;

namespace World.Player.Weapons
{
    public sealed class PlayerAttackSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<PlayerComp, PlayerInputComp, AnimationComp>> _playerFilter = default;
        private readonly EcsPoolInject<ItemComp> _itemsPool = default;
        private readonly EcsPoolInject<HasItems> _hasItemsPool = default;
        private readonly EcsPoolInject<OneHandedMeleeAttackEvent> _oneHandedMeleeAttackPool = Idents.Worlds.Events;

        private readonly EcsCustomInject<CursorService> _cf = default;

        private readonly EcsWorldInject _eventsWorld = Idents.Worlds.Events;
        private readonly EcsWorldInject _defaultWorld = default;
        
        private static readonly int IsMeleeAttack = Animator.StringToHash("IsMeleeAttack");

        public void Run(IEcsSystems systems)
        {
            if (_cf.Value.CursorVisible) return;
            
            foreach (var entity in _playerFilter.Value)
            {
                ref var inputComp = ref _playerFilter.Pools.Inc2.Get(entity);
                ref var animationComp = ref _playerFilter.Pools.Inc3.Get(entity);

                if (inputComp.Attack)
                {
                    animationComp.Animator.SetTrigger(IsMeleeAttack);
                    _oneHandedMeleeAttackPool.Value.Add(_eventsWorld.Value.NewEntity());

                    ref var hasItems = ref _hasItemsPool.Value.Get(entity);
                    foreach (var itemPacked in hasItems.Entities)
                    {
                        if (itemPacked.Unpack(_defaultWorld.Value, out var unpackedEntity))
                        {
                            ref var itemComp = ref _itemsPool.Value.Get(unpackedEntity);

                            if (itemComp.ItemView.itemObject && itemComp.ItemView.itemObject.gameObject.activeSelf)
                            {
                                if(itemComp.ItemView.itemObject is WeaponObject weaponObject)
                                    weaponObject.Attack();
                            }
                        }
                    }
                }
            }
        }
    }
}