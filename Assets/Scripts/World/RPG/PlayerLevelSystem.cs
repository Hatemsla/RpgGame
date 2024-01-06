using DG.Tweening;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using World.Configurations;
using World.Player;

namespace World.RPG
{
    public class PlayerLevelSystem : IEcsRunSystem, IEcsInitSystem
    {
        private readonly EcsFilterInject<Inc<PlayerComp, LevelComp>> _filter = default;

        private readonly EcsCustomInject<Configuration> _cf = default;
        private readonly EcsCustomInject<SceneData> _sd = default;
        
        public void Init(IEcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var player = ref _filter.Pools.Inc1.Get(entity);
                ref var level = ref _filter.Pools.Inc2.Get(entity);
                
                _sd.Value.uiSceneData.experienceSliderView.experienceSlider.maxValue = level.ExperienceToNextLevel;
                _sd.Value.uiSceneData.experienceSliderView.experienceSlider.value = level.Experience;
                _sd.Value.uiSceneData.experienceSliderView.experienceInfoText.text = $"{level.Experience}/{level.ExperienceToNextLevel}";
                _sd.Value.uiSceneData.experienceSliderView.canvasGroup.DOFade(0f, 2f);
            }   
        }
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var player = ref _filter.Pools.Inc1.Get(entity);
                ref var level = ref _filter.Pools.Inc2.Get(entity);
                
                _sd.Value.uiSceneData.experienceSliderView.experienceSlider.maxValue = level.ExperienceToNextLevel;
                _sd.Value.uiSceneData.experienceSliderView.experienceSlider.value = level.Experience;
                _sd.Value.uiSceneData.experienceSliderView.experienceInfoText.text = $"{level.Experience}/{level.ExperienceToNextLevel}";
            }
        }
    }
}