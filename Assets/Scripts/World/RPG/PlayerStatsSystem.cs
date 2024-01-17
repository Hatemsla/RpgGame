using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.Unity.Ugui;
using UnityEngine;
using Utils;
using World.Player;

namespace World.RPG
{
    public class PlayerStatsSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<PlayerComp, PlayerInputComp, LevelComp>> _filter = default;

        private readonly EcsCustomInject<SceneData> _sd = default;
        
        [EcsUguiNamed(Idents.UI.StatsLevelCanvas)]
        private readonly GameObject _statsLevelCanvas = default;
        
        [EcsUguiNamed(Idents.UI.ConfirmToCancelStatsForm)]
        private readonly GameObject _confirmToCancelStatsForm = default;
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var inputComp = ref _filter.Pools.Inc2.Get(entity);
                ref var levelComp = ref _filter.Pools.Inc3.Get(entity);

                if (inputComp.Stats)
                {
                    if (_statsLevelCanvas.activeSelf)
                    {
                        if(levelComp.SpentLevelScore > 0)
                            _confirmToCancelStatsForm.SetActive(true);
                        else
                            _statsLevelCanvas.SetActive(false);   
                    }
                    else
                    {
                        _statsLevelCanvas.SetActive(true);
                        levelComp.SpentLevelScore = 0;
                        levelComp.PreviousStrength = levelComp.Strength;
                        levelComp.PreviousConstitution = levelComp.Constitution;
                        levelComp.PreviousDexterity = levelComp.Dexterity;
                        levelComp.PreviousIntelligence = levelComp.Intelligence;
                        levelComp.PreviousCharisma = levelComp.Charisma;
                        levelComp.PreviousLuck = levelComp.Luck;
                    }
                }
            }
        }
    }
}