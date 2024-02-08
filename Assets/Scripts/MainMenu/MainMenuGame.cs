using System;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Unity.Ugui;
using UnityEngine;

namespace Utils.MainMenu
{
    public sealed class MainMenuGame : MonoBehaviour
    {
        [SerializeField] private EcsUguiEmitter uguiEmitter;
        private EcsSystems _systemsUpdate;
        
        private void Start()
        {
            var world = new EcsWorld();
            _systemsUpdate = new EcsSystems(world);
            
            _systemsUpdate
                .AddWorld(new EcsWorld(), Idents.Worlds.Events)
                .Add(new MainMenuHandleSystem())
                
                .InjectUgui(uguiEmitter, Idents.Worlds.Events)
                .Init();
            
        }
        
        
        private void Update()
        {
            _systemsUpdate?.Run();
        }

        private void OnDestroy()
        {
            _systemsUpdate?.Destroy();
            _systemsUpdate?.GetWorld().Destroy();
            _systemsUpdate = null;
        }
    }
}