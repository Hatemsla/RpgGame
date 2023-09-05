using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.UnityEditor;
using UnityEngine;

public sealed class Game : MonoBehaviour
{
    [SerializeField] private SceneData sceneData;
    [SerializeField] private Configuration configuration;
    private EcsSystems _systems;

    private void Start()
    {
        var world = new EcsWorld();
        _systems = new EcsSystems(world);
        
        _systems
            .Add(new TimeSystem())
#if UNITY_EDITOR
            .Add(new EcsWorldDebugSystem())
#endif
            
            .Inject(sceneData)
            .Init();
    }

    private void Update()
    {
        _systems?.Run();
    }

    private void OnDestroy()
    {
        _systems?.Destroy();
        _systems?.GetWorld().Destroy();
        _systems = null;
    }
}