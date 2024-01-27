using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.Unity.Ugui;
using TMPro;
using UnityEngine;
using Utils;
using World.Configurations;
using World.Player;

namespace World.RPG
{
    public class PlayerStatsInitSystem : IEcsInitSystem
    {
        private readonly EcsFilterInject<Inc<PlayerComp, LevelComp>> _filter = default;

        private readonly EcsCustomInject<SceneData> _sd = default;
        private readonly EcsCustomInject<Configuration> _cf = default;

        [EcsUguiNamed(Idents.UI.StatsLevelCanvas)]
        private readonly GameObject _levelCanvas = default;
        
        [EcsUguiNamed(Idents.UI.CurrentLevelStats)]
        private readonly TMP_Text _currentStatsScore = default;
        
        public void Init(IEcsSystems systems)
        {
            _levelCanvas.SetActive(false);

            _currentStatsScore.text = "Количество очков: 0";
            
            foreach (var levelStatView in _sd.Value.uiSceneData.levelStatsViews)
                levelStatView.valueText.text = "1";

            foreach (var stateView in _sd.Value.uiSceneData.statsViews)
                stateView.valueText.text = "1";

            foreach (var entity in _filter.Value)
            {
                ref var levelComp = ref _filter.Pools.Inc2.Get(entity);

                levelComp.Strength = 1;
                levelComp.Dexterity = 1;
                levelComp.Constitution = 1;
                levelComp.Intelligence = 1;
                levelComp.Charisma = 1;
                levelComp.Luck = 1;
                
                levelComp.PAtk = _cf.Value.playerConfiguration.startPAtk;
                levelComp.MAtk = _cf.Value.playerConfiguration.startMAtk;
                levelComp.Spd = _cf.Value.playerConfiguration.startSpd;
                levelComp.MaxHp = _cf.Value.playerConfiguration.startMaxHp;
                levelComp.MaxSt = _cf.Value.playerConfiguration.startMaxSt;
                levelComp.MaxSp = _cf.Value.playerConfiguration.startMaxSp;
            }
        }
    }
}