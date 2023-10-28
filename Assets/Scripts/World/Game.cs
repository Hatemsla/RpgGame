using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.Unity.Ugui;
using UnityEngine;
using Utils;
using Utils.ObjectsPool;
using World.Ability;
using World.AI;
using World.AI.Navigation;
using World.Configurations;
using World.Inventory.Chest;
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
            var ps = new PoolService();
            var cs = new CursorService();
            var mainInput = new MainInput();

            _systemsUpdate
                //Init systems
                .Add(new PlayerInitSystem())
                .Add(new SpellInitSystem())
                .Add(new ChestInitSystem())
                .Add(new ZoneInitSystem())
                .Add(new EnemyInitSystem())
                
                //Run systems
                .Add(new TimeSystem())
                .Add(new PlayerInputSystem())
                .Add(new CursorControllingSystem())
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
                .Add(new ChestUpdateSystem())
                
                .Add(new EnemyMoveSystem())
                
                .AddWorld(new EcsWorld(), Idents.Worlds.Events)
#if UNITY_EDITOR
                .Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem())
                .Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem(Idents.Worlds.Events))
#endif

                .Inject(ts, ps, cs, configuration, sceneData, mainInput)
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