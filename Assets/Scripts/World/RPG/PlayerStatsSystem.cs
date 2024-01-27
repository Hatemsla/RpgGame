using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.Unity.Ugui;
using UnityEngine;
using Utils;
using World.Player;
using World.Player.Events;

namespace World.RPG
{
    public class PlayerStatsSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<PlayerComp, PlayerInputComp, LevelComp>> _filter = default;
        private readonly EcsPoolInject<TransitionCameraEvent> _transitionCameraPool = Idents.Worlds.Events;

        private readonly EcsCustomInject<SceneData> _sd = default;

        private readonly EcsWorldInject _eventWorld = Idents.Worlds.Events;
        
        [EcsUguiNamed(Idents.UI.StatsLevelCanvas)]
        private readonly GameObject _statsLevelCanvas = default;
        
        [EcsUguiNamed(Idents.UI.ConfirmToCancelStatsForm)]
        private readonly GameObject _confirmToCancelStatsForm = default;
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var playerComp = ref _filter.Pools.Inc1.Get(entity);
                ref var inputComp = ref _filter.Pools.Inc2.Get(entity);
                ref var levelComp = ref _filter.Pools.Inc3.Get(entity);

                if (inputComp.Stats)
                {
                    if (_statsLevelCanvas.activeSelf)
                    {
                        if(levelComp.SpentLevelScore > 0)
                            _confirmToCancelStatsForm.SetActive(true);
                        else
                        {
                            _statsLevelCanvas.SetActive(false);
                            playerComp.PlayerCameraRoot.Priority = 2;
                            playerComp.PlayerCameraStats.Priority = 1;
                            playerComp.CanMove = false;
                            ref var transitionCameraEvent = ref _transitionCameraPool.Value.Add(_eventWorld.Value.NewEntity());
                            transitionCameraEvent.TimeToWait = 1;
                        }
                    }
                    else
                    {
                        _statsLevelCanvas.SetActive(true);
                        playerComp.PlayerCameraRoot.Priority = 1;
                        playerComp.PlayerCameraStats.Priority = 2;
                        playerComp.CanMove = false;
                        
                        levelComp.SpentLevelScore = 0;
                        levelComp.PreviousStrength = levelComp.Strength;
                        levelComp.PreviousConstitution = levelComp.Constitution;
                        levelComp.PreviousDexterity = levelComp.Dexterity;
                        levelComp.PreviousIntelligence = levelComp.Intelligence;
                        levelComp.PreviousCharisma = levelComp.Charisma;
                        levelComp.PreviousLuck = levelComp.Luck;
                        
                        levelComp.PreviousPAtk = levelComp.PAtk;
                        levelComp.PreviousMAtk = levelComp.MAtk;
                        levelComp.PreviousSpd = levelComp.Spd;
                        levelComp.PreviousMaxHp = levelComp.MaxHp;
                        levelComp.PreviousMaxSt = levelComp.MaxSt;
                        levelComp.PreviousMaxSp = levelComp.MaxSp;
                    }
                }
            }
        }
    }
}