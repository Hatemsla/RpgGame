using System;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.Unity.Ugui;
using TMPro;
using UnityEngine;
using UnityEngine.Scripting;
using Utils;
using World.Player;
using World.RPG.UI;

namespace World.RPG
{
    public class HandleStatsButtonsSystem : EcsUguiCallbackSystem
    {
        private readonly EcsFilterInject<Inc<PlayerComp, LevelComp>> _filter = default;
        
        private readonly EcsPoolInject<CloseStatsEvent> _closeStatsPool = Idents.Worlds.Events;
        private readonly EcsPoolInject<StatsEvent> _statsEventPool = Idents.Worlds.Events;

        private readonly EcsCustomInject<SceneData> _sd = default;
        
        [EcsUguiNamed(Idents.UI.StatsLevelCanvas)]
        private readonly GameObject _statsLevelCanvas = default;
        
        [EcsUguiNamed(Idents.UI.ConfirmToCancelStatsForm)]
        private readonly GameObject _confirmToCancelStatsForm = default;
        
        [EcsUguiNamed(Idents.UI.CurrentLevelStats)]
        private readonly TMP_Text _currentStatsScore = default;

        [Preserve]
        [EcsUguiClickEvent(Idents.UI.CancelStatsBtn, Idents.Worlds.Events)]
        private void OnClickCancelStats(in EcsUguiClickEvent e)
        {
            _statsLevelCanvas.SetActive(false);

            foreach (var entity in _filter.Value)
            {
                ref var levelComp = ref _filter.Pools.Inc2.Get(entity);

                var remainLevelScores = levelComp.Strength + levelComp.Dexterity + levelComp.Constitution +
                                        levelComp.Intelligence + levelComp.Charisma + levelComp.Luck -
                                        levelComp.PreviousStrength -
                                        levelComp.PreviousDexterity - levelComp.PreviousConstitution -
                                        levelComp.PreviousIntelligence -
                                        levelComp.PreviousCharisma - levelComp.PreviousLuck;

                levelComp.LevelScore += remainLevelScores;

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
            }
        }
        
        [Preserve]
        [EcsUguiClickEvent(Idents.UI.ConfirmStatsBtn, Idents.Worlds.Events)]
        private void OnClickConfirmStats(in EcsUguiClickEvent e)
        {
            _statsLevelCanvas.SetActive(false);
        }
        
        [Preserve]
        [EcsUguiClickEvent(Idents.UI.CloseStatsBtn, Idents.Worlds.Events)]
        private void OnClickCloseStats(in EcsUguiClickEvent e)
        {
            foreach (var entity in _filter.Value)
            {
                ref var levelComp = ref _filter.Pools.Inc2.Get(entity);

                if(levelComp.SpentLevelScore > 0)
                    _confirmToCancelStatsForm.SetActive(true);
                else
                    _statsLevelCanvas.SetActive(false);
            }
        }
        
        [Preserve]
        [EcsUguiClickEvent(Idents.UI.AddStrengthBtn, Idents.Worlds.Events)]
        private void OnClickAddStrength(in EcsUguiClickEvent e)
        {
            foreach (var entity in _filter.Value)
            {
                ref var levelComp = ref _filter.Pools.Inc2.Get(entity);

                if (levelComp.LevelScore > 0)
                {
                    levelComp.LevelScore--;
                    levelComp.SpentLevelScore++;
                    levelComp.Strength++;
                    _currentStatsScore.text = $"Количество очков: {levelComp.LevelScore}";
                    foreach (var statView in _sd.Value.uiSceneData.statsViews)
                    {
                        if (statView.statType == StatType.Str)
                        {
                            statView.valueText.text = levelComp.Strength.ToString();
                            break;
                        }
                    }
                }
            }
        }
        
        [Preserve]
        [EcsUguiClickEvent(Idents.UI.AddDexterityBtn, Idents.Worlds.Events)]
        private void OnClickAddDexterity(in EcsUguiClickEvent e)
        {
            foreach (var entity in _filter.Value)
            {
                ref var levelComp = ref _filter.Pools.Inc2.Get(entity);

                if (levelComp.LevelScore > 0)
                {
                    levelComp.LevelScore--;
                    levelComp.SpentLevelScore++;
                    levelComp.Dexterity++;
                    _currentStatsScore.text = $"Количество очков: {levelComp.LevelScore}";
                    foreach (var statView in _sd.Value.uiSceneData.statsViews)
                    {
                        if (statView.statType == StatType.Dex)
                        {
                            statView.valueText.text = levelComp.Dexterity.ToString();
                            break;
                        }
                    }
                }
            }
        }
        
        [Preserve]
        [EcsUguiClickEvent(Idents.UI.AddConstitutionBtn, Idents.Worlds.Events)]
        private void OnClickAddConstitution(in EcsUguiClickEvent e)
        {
            foreach (var entity in _filter.Value)
            {
                ref var levelComp = ref _filter.Pools.Inc2.Get(entity);

                if (levelComp.LevelScore > 0)
                {
                    levelComp.LevelScore--;
                    levelComp.SpentLevelScore++;
                    levelComp.Constitution++;
                    _currentStatsScore.text = $"Количество очков: {levelComp.LevelScore}";
                    foreach (var statView in _sd.Value.uiSceneData.statsViews)
                    {
                        if (statView.statType == StatType.Con)
                        {
                            statView.valueText.text = levelComp.Constitution.ToString();
                            break;
                        }
                    }
                }
            }
        }
        
        [Preserve]
        [EcsUguiClickEvent(Idents.UI.AddIntelligenceBtn, Idents.Worlds.Events)]
        private void OnClickAddIntelligence(in EcsUguiClickEvent e)
        {
            foreach (var entity in _filter.Value)
            {
                ref var levelComp = ref _filter.Pools.Inc2.Get(entity);

                if (levelComp.LevelScore > 0)
                {
                    levelComp.LevelScore--;
                    levelComp.SpentLevelScore++;
                    levelComp.Intelligence++;
                    _currentStatsScore.text = $"Количество очков: {levelComp.LevelScore}";
                    foreach (var statView in _sd.Value.uiSceneData.statsViews)
                    {
                        if (statView.statType == StatType.Int)
                        {
                            statView.valueText.text = levelComp.Intelligence.ToString();
                            break;
                        }
                    }
                }
            }
        }
        
        [Preserve]
        [EcsUguiClickEvent(Idents.UI.AddCharismaBtn, Idents.Worlds.Events)]
        private void OnClickAddCharisma(in EcsUguiClickEvent e)
        {
            foreach (var entity in _filter.Value)
            {
                ref var levelComp = ref _filter.Pools.Inc2.Get(entity);

                if (levelComp.LevelScore > 0)
                {
                    levelComp.LevelScore--;
                    levelComp.SpentLevelScore++;
                    levelComp.Charisma++;
                    _currentStatsScore.text = $"Количество очков: {levelComp.LevelScore}";
                    foreach (var statView in _sd.Value.uiSceneData.statsViews)
                    {
                        if (statView.statType == StatType.Cha)
                        {
                            statView.valueText.text = levelComp.Charisma.ToString();
                            break;
                        }
                    }
                }
            }
        }
        
        [Preserve]
        [EcsUguiClickEvent(Idents.UI.AddLuckBtn, Idents.Worlds.Events)]
        private void OnClickAddLuck(in EcsUguiClickEvent e)
        {
            foreach (var entity in _filter.Value)
            {
                ref var levelComp = ref _filter.Pools.Inc2.Get(entity);

                if (levelComp.LevelScore > 0)
                {
                    levelComp.LevelScore--;
                    levelComp.SpentLevelScore++;
                    levelComp.Luck++;
                    _currentStatsScore.text = $"Количество очков: {levelComp.LevelScore}";
                    foreach (var statView in _sd.Value.uiSceneData.statsViews)
                    {
                        if (statView.statType == StatType.Luck)
                        {
                            statView.valueText.text = levelComp.Luck.ToString();
                            break;
                        }
                    }
                }
            }
        }
        
        [Preserve]
        [EcsUguiClickEvent(Idents.UI.RemoveStrengthBtn, Idents.Worlds.Events)]
        private void OnClickRemoveStrength(in EcsUguiClickEvent e)
        {
            foreach (var entity in _filter.Value)
            {
                ref var levelComp = ref _filter.Pools.Inc2.Get(entity);

                if (levelComp.Strength > levelComp.PreviousStrength)
                {
                    levelComp.LevelScore++;
                    levelComp.SpentLevelScore--;
                    levelComp.Strength--;
                    _currentStatsScore.text = $"Количество очков: {levelComp.LevelScore}";
                    foreach (var statView in _sd.Value.uiSceneData.statsViews)
                    {
                        if (statView.statType == StatType.Str)
                        {
                            statView.valueText.text = levelComp.Strength.ToString();
                            break;
                        }
                    }
                }
            }
        }
        
        [Preserve]
        [EcsUguiClickEvent(Idents.UI.RemoveDexterityBtn, Idents.Worlds.Events)]
        private void OnClickRemoveDexterity(in EcsUguiClickEvent e)
        {
            foreach (var entity in _filter.Value)
            {
                ref var levelComp = ref _filter.Pools.Inc2.Get(entity);

                if (levelComp.Dexterity > levelComp.PreviousDexterity)
                {
                    levelComp.LevelScore++;
                    levelComp.SpentLevelScore--;
                    levelComp.Dexterity--;
                    _currentStatsScore.text = $"Количество очков: {levelComp.LevelScore}";
                    foreach (var statView in _sd.Value.uiSceneData.statsViews)
                    {
                        if (statView.statType == StatType.Dex)
                        {
                            statView.valueText.text = levelComp.Dexterity.ToString();
                            break;
                        }
                    }
                }
            }
        }
        
        [Preserve]
        [EcsUguiClickEvent(Idents.UI.RemoveConstitutionBtn, Idents.Worlds.Events)]
        private void OnClickRemoveConstitution(in EcsUguiClickEvent e)
        {
            foreach (var entity in _filter.Value)
            {
                ref var levelComp = ref _filter.Pools.Inc2.Get(entity);

                if (levelComp.Constitution > levelComp.PreviousConstitution)
                {
                    levelComp.LevelScore++;
                    levelComp.SpentLevelScore--;
                    levelComp.Constitution--;
                    _currentStatsScore.text = $"Количество очков: {levelComp.LevelScore}";
                    foreach (var statView in _sd.Value.uiSceneData.statsViews)
                    {
                        if (statView.statType == StatType.Con)
                        {
                            statView.valueText.text = levelComp.Constitution.ToString();
                            break;
                        }
                    }
                }
            }
        }
        
        [Preserve]
        [EcsUguiClickEvent(Idents.UI.RemoveIntelligenceBtn, Idents.Worlds.Events)]
        private void OnClickRemoveIntelligence(in EcsUguiClickEvent e)
        {
            foreach (var entity in _filter.Value)
            {
                ref var levelComp = ref _filter.Pools.Inc2.Get(entity);

                if (levelComp.Intelligence > levelComp.PreviousIntelligence)
                {
                    levelComp.LevelScore++;
                    levelComp.SpentLevelScore--;
                    levelComp.Intelligence--;
                    _currentStatsScore.text = $"Количество очков: {levelComp.LevelScore}";
                    foreach (var statView in _sd.Value.uiSceneData.statsViews)
                    {
                        if (statView.statType == StatType.Int)
                        {
                            statView.valueText.text = levelComp.Intelligence.ToString();
                            break;
                        }
                    }
                }
            }
        }
        
        [Preserve]
        [EcsUguiClickEvent(Idents.UI.RemoveCharismaBtn, Idents.Worlds.Events)]
        private void OnClickRemoveCharisma(in EcsUguiClickEvent e)
        {
            foreach (var entity in _filter.Value)
            {
                ref var levelComp = ref _filter.Pools.Inc2.Get(entity);

                if (levelComp.Charisma > levelComp.PreviousCharisma)
                {
                    levelComp.LevelScore++;
                    levelComp.SpentLevelScore--;
                    levelComp.Charisma--;
                    _currentStatsScore.text = $"Количество очков: {levelComp.LevelScore}";
                    foreach (var statView in _sd.Value.uiSceneData.statsViews)
                    {
                        if (statView.statType == StatType.Cha)
                        {
                            statView.valueText.text = levelComp.Charisma.ToString();
                            break;
                        }
                    }
                }
            }
        }
        
        [Preserve]
        [EcsUguiClickEvent(Idents.UI.RemoveLuckBtn, Idents.Worlds.Events)]
        private void OnClickRemoveLuck(in EcsUguiClickEvent e)
        {
            foreach (var entity in _filter.Value)
            {
                ref var levelComp = ref _filter.Pools.Inc2.Get(entity);

                if (levelComp.Luck > levelComp.PreviousLuck)
                {
                    levelComp.LevelScore++;
                    levelComp.SpentLevelScore--;
                    levelComp.Luck--;
                    _currentStatsScore.text = $"Количество очков: {levelComp.LevelScore}";
                    foreach (var statView in _sd.Value.uiSceneData.statsViews)
                    {
                        if (statView.statType == StatType.Luck)
                        {
                            statView.valueText.text = levelComp.Luck.ToString();
                            break;
                        }
                    }
                }
            }
        }
    }
}
