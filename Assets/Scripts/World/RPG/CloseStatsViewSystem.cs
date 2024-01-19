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

                foreach (var statView in _sd.Value.uiSceneData.statsViews)
                {
                    switch (statView.statType)
                    {
                        case StatType.Str:
                            statView.valueText.text = levelComp.Strength.ToString();
                            break;
                        case StatType.Dex:
                            statView.valueText.text = levelComp.Dexterity.ToString();
                            break;
                        case StatType.Con:
                            statView.valueText.text = levelComp.Constitution.ToString();
                            break;
                        case StatType.Int:
                            statView.valueText.text = levelComp.Intelligence.ToString();
                            break;
                        case StatType.Cha:
                            statView.valueText.text = levelComp.Charisma.ToString();
                            break;
                        case StatType.Luck:
                            statView.valueText.text = levelComp.Luck.ToString();
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