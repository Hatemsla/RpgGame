using System;
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
    public class HandleStatsButtonsSystem : EcsUguiCallbackSystem
    {
        private readonly EcsFilterInject<Inc<PlayerComp, LevelComp>> _filter = default;
        
        private readonly EcsPoolInject<TransitionCameraEvent> _transitionCameraPool = Idents.Worlds.Events;

        private readonly EcsCustomInject<SceneData> _sd = default;
        
        private readonly EcsWorldInject _eventWorld = Idents.Worlds.Events;
        
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
        [EcsUguiClickEvent(Idents.UI.ConfirmStatsBtn, Idents.Worlds.Events)]
        private void OnClickConfirmStats(in EcsUguiClickEvent e)
        {
            foreach (var entity in _filter.Value)
            {
                ref var playerComp = ref _filter.Pools.Inc1.Get(entity);
                
                _statsLevelCanvas.SetActive(false);
                playerComp.PlayerCameraRoot.Priority = 2;
                playerComp.PlayerCameraStats.Priority = 1;
                playerComp.CanMove = false;
                ref var transitionCameraEvent = ref _transitionCameraPool.Value.Add(_eventWorld.Value.NewEntity());
                transitionCameraEvent.TimeToWait = 1;   
            }
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
                {
                    ref var playerComp = ref _filter.Pools.Inc1.Get(entity);
                
                    _statsLevelCanvas.SetActive(false);
                    playerComp.PlayerCameraRoot.Priority = 2;
                    playerComp.PlayerCameraStats.Priority = 1;
                    playerComp.CanMove = false;
                    ref var transitionCameraEvent = ref _transitionCameraPool.Value.Add(_eventWorld.Value.NewEntity());
                    transitionCameraEvent.TimeToWait = 1;   
                }
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
                    levelComp.PAtk++;
                    levelComp.MaxHp++;
                    _currentStatsScore.text = $"Количество очков: {levelComp.LevelScore}";
                    foreach (var statView in _sd.Value.uiSceneData.levelStatsViews)
                    {
                        if (statView.levelStatType == LevelStatType.Str)
                        {
                            statView.valueText.text = levelComp.Strength.ToString();
                            break;
                        }
                    }
                    
                    foreach (var statView in _sd.Value.uiSceneData.statsViews)
                    {
                        if (statView.statType == StatType.PAtk)
                        {
                            statView.valueText.text = levelComp.PAtk.ToString(CultureInfo.InvariantCulture);
                            break;
                        }
                        if (statView.statType == StatType.MaxHp)
                        {
                            statView.valueText.text = levelComp.MaxHp.ToString(CultureInfo.InvariantCulture);
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
                    levelComp.Spd++;
                    _currentStatsScore.text = $"Количество очков: {levelComp.LevelScore}";
                    foreach (var levelStatView in _sd.Value.uiSceneData.levelStatsViews)
                    {
                        if (levelStatView.levelStatType == LevelStatType.Dex)
                        {
                            levelStatView.valueText.text = levelComp.Dexterity.ToString();
                            break;
                        }
                    }

                    foreach (var statView in _sd.Value.uiSceneData.statsViews)
                    {
                        if (statView.statType == StatType.Spd)
                        {
                            statView.valueText.text = levelComp.Spd.ToString(CultureInfo.InvariantCulture);
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
                    levelComp.MaxSt++;
                    _currentStatsScore.text = $"Количество очков: {levelComp.LevelScore}";
                    foreach (var statView in _sd.Value.uiSceneData.levelStatsViews)
                    {
                        if (statView.levelStatType == LevelStatType.Con)
                        {
                            statView.valueText.text = levelComp.Constitution.ToString();
                            break;
                        }
                    }
                    
                    foreach (var statView in _sd.Value.uiSceneData.statsViews)
                    {
                        if (statView.statType == StatType.MaxSt)
                        {
                            statView.valueText.text = levelComp.MaxSt.ToString(CultureInfo.InvariantCulture);
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
                    levelComp.MaxSp++;
                    levelComp.MAtk++;
                    _currentStatsScore.text = $"Количество очков: {levelComp.LevelScore}";
                    foreach (var statView in _sd.Value.uiSceneData.levelStatsViews)
                    {
                        if (statView.levelStatType == LevelStatType.Int)
                        {
                            statView.valueText.text = levelComp.Intelligence.ToString();
                            break;
                        }
                    }
                    
                    foreach (var statView in _sd.Value.uiSceneData.statsViews)
                    {
                        if (statView.statType == StatType.MaxSp)
                        {
                            statView.valueText.text = levelComp.MaxSp.ToString(CultureInfo.InvariantCulture);
                            break;
                        }
                        if (statView.statType == StatType.MAtk)
                        {
                            statView.valueText.text = levelComp.MAtk.ToString(CultureInfo.InvariantCulture);
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
                    foreach (var statView in _sd.Value.uiSceneData.levelStatsViews)
                    {
                        if (statView.levelStatType == LevelStatType.Cha)
                        {
                            statView.valueText.text = levelComp.Charisma.ToString();
                            break;
                        }
                    }
                    
                    //TODO Хуй знает что делает харизма?
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

                    levelComp.PAtk += 0.2f;
                    levelComp.MAtk += 0.2f;
                    levelComp.Spd += 0.2f;
                    levelComp.MaxHp += 0.2f;
                    levelComp.MaxSt += 0.2f;
                    levelComp.MaxSp += 0.2f;
                    
                    _currentStatsScore.text = $"Количество очков: {levelComp.LevelScore}";
                    foreach (var statView in _sd.Value.uiSceneData.levelStatsViews)
                    {
                        if (statView.levelStatType == LevelStatType.Luck)
                        {
                            statView.valueText.text = levelComp.Luck.ToString();
                            break;
                        }
                    }
                    
                    foreach (var statView in _sd.Value.uiSceneData.statsViews)
                    {
                        switch (statView.statType)
                        {
                            case StatType.PAtk:
                                statView.valueText.text = levelComp.PAtk.ToString(CultureInfo.InvariantCulture);
                                break;
                            case StatType.MAtk:
                                statView.valueText.text = levelComp.MAtk.ToString(CultureInfo.InvariantCulture);
                                break;
                            case StatType.Spd:
                                statView.valueText.text = levelComp.Spd.ToString(CultureInfo.InvariantCulture);
                                break;
                            case StatType.MaxHp:
                                statView.valueText.text = levelComp.MaxHp.ToString(CultureInfo.InvariantCulture);
                                break;
                            case StatType.MaxSp:
                                statView.valueText.text = levelComp.MaxSp.ToString(CultureInfo.InvariantCulture);
                                break;
                            case StatType.MaxSt:
                                statView.valueText.text = levelComp.MaxSt.ToString(CultureInfo.InvariantCulture);
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
                    levelComp.PAtk--;
                    levelComp.MaxHp--;
                    _currentStatsScore.text = $"Количество очков: {levelComp.LevelScore}";
                    foreach (var statView in _sd.Value.uiSceneData.levelStatsViews)
                    {
                        if (statView.levelStatType == LevelStatType.Str)
                        {
                            statView.valueText.text = levelComp.Strength.ToString();
                            break;
                        }
                    }
                    
                    foreach (var statView in _sd.Value.uiSceneData.statsViews)
                    {
                        if (statView.statType == StatType.PAtk)
                        {
                            statView.valueText.text = levelComp.PAtk.ToString(CultureInfo.InvariantCulture);
                            break;
                        }
                        if (statView.statType == StatType.MaxHp)
                        {
                            statView.valueText.text = levelComp.MaxHp.ToString(CultureInfo.InvariantCulture);
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
                    levelComp.Spd--;
                    _currentStatsScore.text = $"Количество очков: {levelComp.LevelScore}";
                    foreach (var statView in _sd.Value.uiSceneData.levelStatsViews)
                    {
                        if (statView.levelStatType == LevelStatType.Dex)
                        {
                            statView.valueText.text = levelComp.Dexterity.ToString();
                            break;
                        }
                    }
                    
                    foreach (var statView in _sd.Value.uiSceneData.statsViews)
                    {
                        if (statView.statType == StatType.Spd)
                        {
                            statView.valueText.text = levelComp.Spd.ToString(CultureInfo.InvariantCulture);
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
                    levelComp.MaxSt--;
                    _currentStatsScore.text = $"Количество очков: {levelComp.LevelScore}";
                    foreach (var statView in _sd.Value.uiSceneData.levelStatsViews)
                    {
                        if (statView.levelStatType == LevelStatType.Con)
                        {
                            statView.valueText.text = levelComp.Constitution.ToString();
                            break;
                        }
                    }
                    
                    foreach (var statView in _sd.Value.uiSceneData.statsViews)
                    {
                        if (statView.statType == StatType.MaxSt)
                        {
                            statView.valueText.text = levelComp.MaxSt.ToString(CultureInfo.InvariantCulture);
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
                    levelComp.MAtk--;
                    levelComp.MaxSp--;
                    _currentStatsScore.text = $"Количество очков: {levelComp.LevelScore}";
                    foreach (var statView in _sd.Value.uiSceneData.levelStatsViews)
                    {
                        if (statView.levelStatType == LevelStatType.Int)
                        {
                            statView.valueText.text = levelComp.Intelligence.ToString();
                            break;
                        }
                    }
                    
                    foreach (var statView in _sd.Value.uiSceneData.statsViews)
                    {
                        if (statView.statType == StatType.MaxSp)
                        {
                            statView.valueText.text = levelComp.MaxSp.ToString(CultureInfo.InvariantCulture);
                            break;
                        }
                        if (statView.statType == StatType.MAtk)
                        {
                            statView.valueText.text = levelComp.MAtk.ToString(CultureInfo.InvariantCulture);
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
                    foreach (var statView in _sd.Value.uiSceneData.levelStatsViews)
                    {
                        if (statView.levelStatType == LevelStatType.Cha)
                        {
                            statView.valueText.text = levelComp.Charisma.ToString();
                            break;
                        }
                    }
                    
                    // TODO Хуй знает что оно убирает?
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
                    
                    levelComp.PAtk -= 0.2f;
                    levelComp.MAtk -= 0.2f;
                    levelComp.Spd -= 0.2f;
                    levelComp.MaxHp -= 0.2f;
                    levelComp.MaxSt -= 0.2f;
                    levelComp.MaxSp -= 0.2f;
                    
                    _currentStatsScore.text = $"Количество очков: {levelComp.LevelScore}";
                    foreach (var statView in _sd.Value.uiSceneData.levelStatsViews)
                    {
                        if (statView.levelStatType == LevelStatType.Luck)
                        {
                            statView.valueText.text = levelComp.Luck.ToString();
                            break;
                        }
                    }
                    
                    foreach (var statView in _sd.Value.uiSceneData.statsViews)
                    {
                        switch (statView.statType)
                        {
                            case StatType.PAtk:
                                statView.valueText.text = levelComp.PAtk.ToString(CultureInfo.InvariantCulture);
                                break;
                            case StatType.MAtk:
                                statView.valueText.text = levelComp.MAtk.ToString(CultureInfo.InvariantCulture);
                                break;
                            case StatType.Spd:
                                statView.valueText.text = levelComp.Spd.ToString(CultureInfo.InvariantCulture);
                                break;
                            case StatType.MaxHp:
                                statView.valueText.text = levelComp.MaxHp.ToString(CultureInfo.InvariantCulture);
                                break;
                            case StatType.MaxSp:
                                statView.valueText.text = levelComp.MaxSp.ToString(CultureInfo.InvariantCulture);
                                break;
                            case StatType.MaxSt:
                                statView.valueText.text = levelComp.MaxSt.ToString(CultureInfo.InvariantCulture);
                                break;
                        }
                    }
                }
            }
        }
    }
}
