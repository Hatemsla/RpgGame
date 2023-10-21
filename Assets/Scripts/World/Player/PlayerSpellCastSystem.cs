using System;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.Unity.Ugui;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Utils;
using World.Ability;
using Object = UnityEngine.Object;
using Quaternion = UnityEngine.Quaternion;

namespace World.Player
{
    public sealed class PlayerSpellCastSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<PlayerComp, PlayerInputComp, RpgComp>> _player = default;
        private readonly EcsCustomInject<Configuration> _cf = default;
        private readonly EcsCustomInject<SceneData> _sd = default;
        
        private readonly EcsPoolInject<SpellComp> _spell = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var playerEntity in _player.Value)
            {
                var world = systems.GetWorld();
                ref var player = ref _player.Pools.Inc1.Get(playerEntity);
                ref var input = ref _player.Pools.Inc2.Get(playerEntity);
                ref var rpg = ref _player.Pools.Inc3.Get(playerEntity);

                if(rpg.IsDead || input.FreeCursor) return;
                
                if(EventSystem.current.IsPointerOverGameObject()) return;
                
                if (input.UseAbility)
                {
                    var spellObjectPrefab = _cf.Value.abilityConfiguration.abilityDatas[0];

                    if (rpg.Mana >= spellObjectPrefab.costPoint)
                    {
                        rpg.Mana -= spellObjectPrefab.costPoint;

                        var spellDirection = 
                            _sd.Value.mainCamera.OutputCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
                        var spellObject = 
                            Object.Instantiate(spellObjectPrefab.spell, player.Transform.position + player.Transform.forward, 
                                Quaternion.identity);
                        
                        var spellEntity = world.NewEntity();
                        var spellPackedEntity = world.PackEntity(spellEntity);
                        ref var spell = ref _spell.Value.Add(spellEntity);
            
                        spell.spellObject = spellObject;
                        spell.spellOwner = playerEntity;
                        
                        spellObject.spellTime = spellObjectPrefab.lifeTime;
                        spellObject.spellSpeed = spellObjectPrefab.speed;
                        spellObject.spellDirection = spellDirection.direction;
                        spellObject.spellIdx = spellPackedEntity;
                        spellObject.SetWorld(world, playerEntity);
                    }
                }
            }
        }
    }
}