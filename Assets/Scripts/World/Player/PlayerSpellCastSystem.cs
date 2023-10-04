﻿using System;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Unity.Mathematics;
using UnityEngine;
using Object = UnityEngine.Object;
using Quaternion = UnityEngine.Quaternion;

namespace World.Player
{
    public sealed class PlayerSpellCastSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<PlayerComp, PlayerInputComp, RpgComp>> _playerMove = default;
        private readonly EcsCustomInject<Configuration> _cf = default;
        private readonly EcsCustomInject<SceneData> _sd = default;
        

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _playerMove.Value)
            {
                ref var player = ref _playerMove.Pools.Inc1.Get(entity);
                ref var input = ref _playerMove.Pools.Inc2.Get(entity);

                if (input.UseAbility)
                {
                    var spellObjectPrefab = _cf.Value.abilityConfiguration.abilityDatas[0];
                    Debug.Log(_sd.Value.mainCamera);
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
}