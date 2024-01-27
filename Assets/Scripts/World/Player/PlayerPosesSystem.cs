using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace World.Player
{
    public class PlayerPosesSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<PlayerComp, PlayerInputComp, AnimationComp>> _playerFilter = default;

        private readonly EcsCustomInject<TimeService> _ts = default;

        private static readonly int OffPoses = Animator.StringToHash("OffPoses");
        private static readonly int Pose0 = Animator.StringToHash("Pose0");
        private static readonly int Pose1 = Animator.StringToHash("Pose1");
        private static readonly int Pose2 = Animator.StringToHash("Pose2");

        private readonly float _durationTransition = 0.3f;
        private float _currentDuration;

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _playerFilter.Value)
            {
                ref var playerComp = ref _playerFilter.Pools.Inc1.Get(entity);
                ref var animationComp = ref _playerFilter.Pools.Inc3.Get(entity);
                ref var inputComp = ref _playerFilter.Pools.Inc2.Get(entity);

                if (playerComp.IsPose)
                    _currentDuration += _ts.Value.DeltaTime;

                if (_currentDuration >= _durationTransition && animationComp.Animator.GetCurrentAnimatorStateInfo(0).IsName("Locomotion"))
                {
                    playerComp.IsPose = false;
                }

                if (inputComp.Pose0)
                {
                    playerComp.IsPose = true;
                    _currentDuration = 0;
                    animationComp.Animator.SetTrigger(OffPoses);
                    animationComp.Animator.SetTrigger(Pose0);
                }
                
                if (inputComp.Pose1)
                {
                    playerComp.IsPose = true;
                    _currentDuration = 0;
                    animationComp.Animator.SetTrigger(OffPoses);
                    animationComp.Animator.SetTrigger(Pose1);
                }
                
                if (inputComp.Pose2)
                {
                    playerComp.IsPose = true;
                    _currentDuration = 0;
                    animationComp.Animator.SetTrigger(OffPoses);
                    animationComp.Animator.SetTrigger(Pose2);
                }
            }
        }
    }
}