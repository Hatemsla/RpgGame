using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.Unity.Ugui;
using Leopotam.EcsLite.UnityEditor;
using UnityEngine;
using Utils;
using World.Player;

namespace World
{
    public sealed class Game : MonoBehaviour
    {
        [SerializeField] private SceneData sceneData;
        [SerializeField] private Configuration configuration;
        [SerializeField] private EcsUguiEmitter uguiEmitter;
        private EcsSystems _systemsUpdate;
        private EcsSystems _systemsFixedUpdate;
        private EcsSystems _systemsLateUpdate;

        private void Start()
        {
            var world = new EcsWorld();
            _systemsUpdate = new EcsSystems(world);
            _systemsFixedUpdate = new EcsSystems(world);
            _systemsLateUpdate = new EcsSystems(world);
            var ts = new TimeService();
            var mainInput = new MainInput();


            _systemsUpdate
                .Add(new PlayerInitSystem())
                .Add(new TimeSystem())
                .Add(new PlayerInputSystem())
                .Add(new PlayerDeathSystem())
                .Add(new PlayerJumpAndGravitySystem())
                .Add(new PlayerGroundedSystem())
                .Add(new PlayerMoveSystem())
                .Add(new PlayerCameraRotateSystem())
                .Add(new PlayerDashSystem())
                .Add(new CameraZoomSystem())
                .Add(new PlayerStaminaSystem())
                .Add(new PlayerFallSystem())
                .Add(new PlayerHealthSystem())
                .Add(new PlayerSpellCastSystem())
                .Add(new PlayerManaSystem())
                .Add(new PlayerGetItemSystem())
                
                .AddWorld(new EcsWorld(), Idents.Worlds.Events)
#if UNITY_EDITOR
                .Add(new EcsWorldDebugSystem())
                .Add(new EcsWorldDebugSystem(Idents.Worlds.Events))
#endif

                .Inject(ts, configuration, sceneData, mainInput)
                .InjectUgui(uguiEmitter, Idents.Worlds.Events)
                .Init();
            _systemsFixedUpdate
                .Init();

            _systemsLateUpdate
                .Inject(ts, configuration, sceneData, mainInput)
                .Init();
        }

        private void Update()
        {
            _systemsUpdate?.Run();
        }

        private void FixedUpdate()
        {
            _systemsFixedUpdate?.Run();
        }

        private void LateUpdate()
        {
            _systemsLateUpdate?.Run();
        }

        private void OnDestroy()
        {
            _systemsUpdate?.Destroy();
            _systemsUpdate?.GetWorld().Destroy();
            _systemsUpdate = null;

            _systemsFixedUpdate?.Destroy();
            _systemsFixedUpdate?.GetWorld().Destroy();
            _systemsFixedUpdate = null;

            _systemsLateUpdate?.Destroy();
            _systemsLateUpdate?.GetWorld().Destroy();
            _systemsLateUpdate = null;
        }
    }
}