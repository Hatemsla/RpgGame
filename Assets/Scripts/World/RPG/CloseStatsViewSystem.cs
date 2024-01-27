using System.Globalization;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.Unity.Ugui;
using TMPro;
using UnityEngine;
using UnityEngine.Scripting;
using Utils;
using World.Player;
using World.Player.Events;
using World.RPG.UI;

namespace World.RPG
{
    public class CloseStatsViewSystem : EcsUguiCallbackSystem
    {
        private readonly EcsFilterInject<Inc<PlayerComp, LevelComp>> _filter = default;
        private readonly EcsPoolInject<TransitionCameraEvent> _transitionCameraPool = Idents.Worlds.Events;
        
        private readonly EcsCustomInject<SceneData> _sd = default;
        
        private readonly EcsWorldInject _eventWorld = Idents.Worlds.Events;
        
        [EcsUguiNamed(Idents.UI.ConfirmToCancelStatsForm)]
        private readonly GameObject _confirmToCancelStatsForm = default;
        
        [EcsUguiNamed(Idents.UI.StatsLevelCanvas)]
        private readonly GameObject _statsLevelCanvas = default;
        
        [EcsUguiNamed(Idents.UI.CurrentLevelStats)]
        private readonly TMP_Text _currentStatsScore = default;
        
        [Preserve]
        [EcsUguiClickEvent(Idents.UI.YesConfirmToExitFromStatsBtn, Idents.Worlds.Events)]
        private void OnClickYesCloseStatsView(in EcsUguiClickEvent e)
        {
            foreach (var entity in _filter.Value)
            {
                ref var playerComp = ref _filter.Pools.Inc1.Get(entity);
                ref var levelComp = ref _filter.Pools.Inc2.Get(entity);

                var remainLevelScores = levelComp.Strength + levelComp.Dexterity + levelComp.Constitution +
                                        levelComp.Intelligence + levelComp.Charisma + levelComp.Luck -
                                        levelComp.PreviousStrength -
                                        levelComp.PreviousDexterity - levelComp.PreviousConstitution -
                                        levelComp.PreviousIntelligence -
                                        levelComp.PreviousCharisma - levelComp.PreviousLuck;

                levelComp.LevelScore += remainLevelScores;
                _currentStatsScore.text = $"Количество очков: {levelComp.LevelScore}";

                levelComp.Strength = levelComp.PreviousStrength;
                levelComp.Dexterity = levelComp.PreviousDexterity;
                levelComp.Constitution = levelComp.PreviousConstitution;
                levelComp.Intelligence = levelComp.PreviousIntelligence;
                levelComp.Charisma = levelComp.PreviousCharisma;
                levelComp.Luck = levelComp.PreviousLuck;
                
                levelComp.PAtk = levelComp.PreviousPAtk;
                levelComp.MAtk = levelComp.PreviousMAtk;
                levelComp.Spd = levelComp.PreviousSpd;
                levelComp.MaxHp = levelComp.PreviousMaxHp;
                levelComp.MaxSt = levelComp.PreviousMaxSt;
                levelComp.MaxSp = levelComp.PreviousMaxSp;

                foreach (var levelStatView in _sd.Value.uiSceneData.levelStatsViews)
                {
                    switch (levelStatView.levelStatType)
                    {
                        case LevelStatType.Str:
                            levelStatView.valueText.text = levelComp.Strength.ToString();
                            break;
                        case LevelStatType.Dex:
                            levelStatView.valueText.text = levelComp.Dexterity.ToString();
                            break;
                        case LevelStatType.Con:
                            levelStatView.valueText.text = levelComp.Constitution.ToString();
                            break;
                        case LevelStatType.Int:
                            levelStatView.valueText.text = levelComp.Intelligence.ToString();
                            break;
                        case LevelStatType.Cha:
                            levelStatView.valueText.text = levelComp.Charisma.ToString();
                            break;
                        case LevelStatType.Luck:
                            levelStatView.valueText.text = levelComp.Luck.ToString();
                            break;
                    }
                }
                
                foreach (var statView in _sd.Value.uiSceneData.statsViews)
                {
                    switch (statView.statType)
                    {
                        case StatType.PAtk:
                            statView.valueText.text = levelComp.PAtk.ToString("F1", CultureInfo.InvariantCulture);
                            break;
                        case StatType.MAtk:
                            statView.valueText.text = levelComp.MAtk.ToString("F1", CultureInfo.InvariantCulture);
                            break;
                        case StatType.Spd:
                            statView.valueText.text = levelComp.Spd.ToString("F1", CultureInfo.InvariantCulture);
                            break;
                        case StatType.MaxHp:
                            statView.valueText.text = levelComp.MaxHp.ToString("F1", CultureInfo.InvariantCulture);
                            break;
                        case StatType.MaxSt:
                            statView.valueText.text = levelComp.MaxSt.ToString("F1", CultureInfo.InvariantCulture);
                            break;
                        case StatType.MaxSp:
                            statView.valueText.text = levelComp.MaxSp.ToString("F1", CultureInfo.InvariantCulture);
                            break;
                    }
                }
                
                _confirmToCancelStatsForm.SetActive(false);   
                _statsLevelCanvas.SetActive(false);
                playerComp.PlayerCameraRoot.Priority = 2;
                playerComp.PlayerCameraStats.Priority = 1;
                playerComp.CanMove = false;
                ref var transitionCameraEvent = ref _transitionCameraPool.Value.Add(_eventWorld.Value.NewEntity());
                transitionCameraEvent.TimeToWait = 1;
            }
        }
        
        [Preserve]
        [EcsUguiClickEvent(Idents.UI.NoConfirmToExitFromStatsBtn, Idents.Worlds.Events)]
        private void OnClickNoCloseStatsView(in EcsUguiClickEvent e)
        {
            _confirmToCancelStatsForm.SetActive(false);   
        }
    }
}