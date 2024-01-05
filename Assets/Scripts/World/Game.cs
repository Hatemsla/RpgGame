using System;
using System.Threading.Tasks;
using Fusion;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.ExtendedSystems;
using Leopotam.EcsLite.Unity.Ugui;
using UnityEngine;
using Utils;
using Utils.ObjectsPool;
using World.Ability;
using World.AI;
using World.AI.Navigation;
using World.Configurations;
using World.Inventory;
using World.Inventory.Chest;
using World.Network;
using World.Network.Input;
using World.Player;

namespace World
{
    public sealed class Game : NetworkBehaviour
    {
        [SerializeField] private SceneData sceneData;
        [SerializeField] private Configuration configuration;
        [SerializeField] private EcsUguiEmitter uguiEmitter;
        private EcsSystems _systemsUpdate;
        private EcsSystems _systemsFixedUpdate;
        private EcsSystems _systemsLateUpdate;

        private EcsWorld _world;
        private NetworkRunner _networkRunner;
        private NetworkRunnerService _networkRunnerService;

        private void Awake()
        {
            var networkRunnerInScene = FindObjectOfType<NetworkRunner>();

            if (networkRunnerInScene != null)
                _networkRunner = networkRunnerInScene;
        }

        private async void Start()
        {
            var input = new InputService();
            _networkRunnerService = new NetworkRunnerService(configuration.networkConfiguration.networkRunnerPrefab, _networkRunner, input);
            
            await WaitForPlayerJoined();
            
            Utils.Utils.DebugLog("Game Start");
            
            _world = new EcsWorld();
            _systemsUpdate = new EcsSystems(_world);
            _systemsFixedUpdate = new EcsSystems(_world);
            _systemsLateUpdate = new EcsSystems(_world);
            
            var ts = new TimeService();
            var ps = new PoolService();
            var cs = new CursorService();
            var mainInput = new MainInput();

            _systemsUpdate
                    
                //Init systems
                
                
                //Run systems
                .Add(new TimeSystem())
                .Add(new PlayerInputSystem())
                

                .Inject(ts, ps, cs, configuration, sceneData, mainInput, _networkRunnerService, input)
                .Init();
            _systemsFixedUpdate
                .Add(new PlayerInitSystem())
                .Add(new ItemsInitSystem())
                .Add(new SpellInitSystem())
                .Add(new ChestInitSystem())
                .Add(new ZoneInitSystem())
                .Add(new EnemyInitSystem())
                //Run systems
                .Add(new CursorControllingSystem())
                .Add(new PlayerDeathSystem())
                .Add(new PlayerJumpAndGravitySystem())
                .Add(new PlayerGroundedSystem())
                .Add(new PlayerMoveSystem())
                .Add(new PlayerCameraRotateSystem())
                .Add(new PlayerDashSystem())
                .Add(new CameraZoomSystem())
                .Add(new PlayerFallSystem())
                .Add(new PlayerHealthSystem())
                .Add(new PlayerStaminaSystem())
                .Add(new PlayerManaSystem())
                .Add(new PlayerSpellCastSystem())
                .Add(new PlayerGetItemSystem())
                .Add(new ChestUpdateSystem())
                
                .DelHere<DeleteEvent>()
                .Add(new DeleteFormSystem())
                
                .Add(new EnemyPatrolSystem())
                .Add(new EnemyChaseSystem())
                .Add(new EnemyAttackSystem())
                .Add(new EnemyRecoverySystem())
                .Add(new EnemyRespawnSystem())
                
                .AddWorld(new EcsWorld(), Idents.Worlds.Events)
#if UNITY_EDITOR
                .Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem())
                .Add(new Leopotam.EcsLite.UnityEditor.EcsSystemsDebugSystem())
                .Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem(Idents.Worlds.Events))
#endif

                .Inject(ts, ps, cs, configuration, sceneData, mainInput, _networkRunnerService)
                .InjectUgui(uguiEmitter, Idents.Worlds.Events)
                .Init();

            _systemsLateUpdate
                .Inject(ts, configuration, sceneData, mainInput)
                .Init();
        }
        
        private async Task WaitForPlayerJoined()
        {
            while (!_networkRunnerService.isPlayerJoined)
            {
                await Task.Yield();
            }
        }

        private void Update()
        {
            if(_networkRunnerService.isPlayerJoined)
                _systemsUpdate?.Run();
        }

        public override void FixedUpdateNetwork()
        {
            if(_networkRunnerService.isPlayerJoined)
                _systemsFixedUpdate?.Run();
        }

        private void FixedUpdate()
        { 
            // _systemsFixedUpdate?.Run();
        }
        
        // private void LateUpdate()
        // {
        //     if(_networkRunnerService.isPlayerJoined)
        //         _systemsLateUpdate?.Run();
        // }

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