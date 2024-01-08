using DG.Tweening;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using Utils;
using World.Configurations;
using World.Player;

namespace World.RPG
{
    public class PlayerLevelSystem : IEcsRunSystem, IEcsInitSystem
    {
        private readonly EcsFilterInject<Inc<PlayerComp, LevelComp>> _filter = default;
        private readonly EcsFilterInject<Inc<LevelChangedEvent>> _levelChangedEvent = Idents.Worlds.Events;

        private readonly EcsCustomInject<Configuration> _cf = default;
        private readonly EcsCustomInject<SceneData> _sd = default;

        public void Init(IEcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var level = ref _filter.Pools.Inc2.Get(entity);

                _sd.Value.uiSceneData.experienceSliderView.experienceSlider.maxValue = level.ExperienceToNextLevel;
                _sd.Value.uiSceneData.experienceSliderView.experienceSlider.value = level.Experience;
                _sd.Value.uiSceneData.experienceSliderView.experienceInfoText.text =
                    $"{level.Experience}/{level.ExperienceToNextLevel}";
                _sd.Value.uiSceneData.experienceSliderView.canvasGroup.DOFade(0f, 2f);
            }
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var levelChangedEntity in _levelChangedEvent.Value)
            {
                ref var levelChangedEvent = ref _levelChangedEvent.Pools.Inc1.Get(levelChangedEntity);

                foreach (var entity in _filter.Value)
                {
                    ref var level = ref _filter.Pools.Inc2.Get(entity);

                    level.Experience = levelChangedEvent.NewExperience;

                    if (level.Experience >= level.ExperienceToNextLevel)
                    {
                        level.Level++;
                        level.Experience = 0;
                        level.ExperienceToNextLevel =
                            _cf.Value.playerConfiguration.experienceToNextLevel[level.Level - 1];
                    }

                    _sd.Value.uiSceneData.experienceSliderView.experienceSlider.maxValue = level.ExperienceToNextLevel;
                    _sd.Value.uiSceneData.experienceSliderView.experienceSlider.value = level.Experience;
                    _sd.Value.uiSceneData.experienceSliderView.experienceInfoText.text =
                        $"{level.Experience}/{level.ExperienceToNextLevel}";

                    _sd.Value.uiSceneData.experienceSliderView.canvasGroup.DOFade(1f, 2f).onComplete = () =>
                        _sd.Value.uiSceneData.experienceSliderView.canvasGroup.DOFade(0f, 2f);
                }
            }
        }
    }
}