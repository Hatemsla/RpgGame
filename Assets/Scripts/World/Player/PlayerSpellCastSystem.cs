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
            foreach (var entity in _player.Value)
            {
                var world = systems.GetWorld();
                ref var player = ref _player.Pools.Inc1.Get(entity);
                ref var input = ref _player.Pools.Inc2.Get(entity);
                ref var rpg = ref _player.Pools.Inc3.Get(entity);

                if(rpg.IsDead) return;
                
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

                        spellObject.spellTime = spellObjectPrefab.lifeTime;
                        spellObject.spellSpeed = spellObjectPrefab.speed;
                        spellObject.spellDirection = spellDirection.direction;
                    }
                }
            }
        }

        private void CreateSpell(int playerEntity, EcsWorld world, SpellView sv)
        {
            var spellEntity = world.NewEntity();
            ref var spell = ref _spell.Value.Add(spellEntity);
            
            spell.spellView = sv;
            spell.spellOwner = playerEntity;
        }
    }
}