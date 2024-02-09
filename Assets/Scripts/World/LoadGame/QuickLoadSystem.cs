using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using UnityEngine.SceneManagement;
using World.Player;

namespace World.LoadGame
{
    public class QuickLoadSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<PlayerInputComp>> _playerFilter = default;
        
        public void Run(IEcsSystems systems)
        {
            foreach (var playerEntity in _playerFilter.Value)
            {
                ref var playerInputComp = ref _playerFilter.Pools.Inc1.Get(playerEntity);

                if (playerInputComp.QuickLoad)
                {
                    SceneManager.LoadScene("World");
                    PlayerPrefs.SetInt("Load Progress", 1);
                    PlayerPrefs.Save();
                }
            }
        }
    }
}