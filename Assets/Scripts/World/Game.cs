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
using World.Player;
using World.Player.Events;
using World.Player.Weapons;
using World.RPG;
using World.UI;
using World.UI.LookOnObject;
using World.UI.PopupText;

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
                .AddWorld(new EcsWorld(), Idents.Worlds.Events)
                    
                //Init systems
                .Add(new PlayerInitSystem())
                .Add(new ItemsInitSystem())
                .Add(new AbilityInitSystem())
                .Add(new AbilityObjectsInitSystem())
                .Add(new ChestInitSystem())
                .Add(new ZoneInitSystem())
                .Add(new EnemyInitSystem())
                .Add(new PlayerStatsInitSystem())
                .Add(new PopupDamageTextInitSystem())
                .Add(new UIBillboardingSystem())
                
                //Run systems
                .Add(new TimeSystem())
                .Add(new PlayerInputSystem())
                .Add(new CursorControllingSystem())
                .Add(new PlayerDeathSystem())
                .Add(new PlayerJumpAndGravitySystem())
                .Add(new PlayerGroundedSystem())
                .Add(new PlayerMoveSystem())
                .Add(new PlayerDashSystem())
                .Add(new CameraZoomSystem())
                .Add(new PlayerFallSystem())
                .Add(new PlayerHealthSystem())
                .Add(new PlayerStaminaSystem())
                .Add(new PlayerManaSystem())
                .Add(new PlayerSpellCastSystem())
                .Add(new AbilityDelaySystem())
                .Add(new PlayerGetItemSystem())
                .Add(new PassiveGetExperienceSystem())
                .Add(new PlayerLevelSystem())
                .Add(new PlayerWaitForEndDeathAnimationSystem())
                .Add(new PlayerStatsSystem())
                .Add(new HandleStatsButtonsSystem())
                .Add(new CloseStatsViewSystem())
                .Add(new PlayerCameraTransitionSystem())
                .Add(new PlayerAttackSystem())
                .Add(new PlayerPosesSystem())
                .Add(new LookOnObjectSystem())
                
                .Add(new AnimatePopupDamageTextSystem())
                // .Add(new OneMeleeAttackDelaySystem())
                
                .DelHere<DeleteEvent>(Idents.Worlds.Events)
                .DelHere<LevelChangedEvent>(Idents.Worlds.Events)
                .DelHere<DeathAnimationEvent>(Idents.Worlds.Events)
                .DelHere<StatsEvent>(Idents.Worlds.Events)
                .DelHere<CloseStatsEvent>(Idents.Worlds.Events)
                .DelHere<TransitionCameraEvent>(Idents.Worlds.Events)
                .DelHere<OneHandedMeleeAttackEvent>(Idents.Worlds.Events)
                .Add(new DeleteFormSystem())
                
                .Add(new EnemyPatrolSystem())
                .Add(new EnemyChaseSystem())
                .Add(new EnemyAttackSystem())
                .Add(new EnemyRecoverySystem())
                .Add(new EnemyRespawnSystem())
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
                .Add(new PlayerCameraRotateSystem())
                .Inject(ts, ps, cs, configuration, sceneData, mainInput)
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