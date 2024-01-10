using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using World.Configurations;
using World.RPG;

namespace World.Player
{
    public enum MoveState
    {
        RunBackward,
        Idle,
        Walk,
        RunForward,
        Sprint
    }

    public sealed class PlayerMoveSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<PlayerComp, PlayerInputComp, RpgComp, AnimationComp>>
            _playerMove = default;

        private readonly EcsCustomInject<Configuration> _cf = default;
        private readonly EcsCustomInject<SceneData> _sd = default;
        private readonly EcsCustomInject<TimeService> _ts = default;

        private float _speed;
        private float _targetRotation;
        private float _rotationVelocity;
        private float _previousTargetRotation;
        private float _targetSpeed;

        private float _currentMoveSpeed; // Текущее значение MoveSpeed
        private float _targetMoveSpeed; // Целевое значение MoveSpeed
        private const float SpeedChangeRate = 5f;

        private MoveState _moveState = MoveState.Idle;

        private static readonly int MoveSpeed = Animator.StringToHash("MoveSpeed");

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _playerMove.Value)
            {
                ref var playerComp = ref _playerMove.Pools.Inc1.Get(entity);
                ref var inputComp = ref _playerMove.Pools.Inc2.Get(entity);
                ref var rpgComp = ref _playerMove.Pools.Inc3.Get(entity);
                ref var animationComp = ref _playerMove.Pools.Inc4.Get(entity);

                if (rpgComp.IsDead) return;

                _targetSpeed = _cf.Value.playerConfiguration.moveSpeed;
                _moveState = MoveState.RunForward;

                if (playerComp.IsWalking)
                {
                    _targetSpeed = _cf.Value.playerConfiguration.walkSpeed;
                    _moveState = MoveState.Walk;
                }

                var sprintEndurance =
                    rpgComp.Stamina - _cf.Value.playerConfiguration.sprintEndurance * _ts.Value.DeltaTime;
                rpgComp.CanRun = sprintEndurance > 0;

                if (inputComp.Sprint)
                {
                    if (rpgComp.CanRun)
                    {
                        rpgComp.Stamina = sprintEndurance;
                        _targetSpeed = _cf.Value.playerConfiguration.sprintSpeed;
                        _moveState = MoveState.Sprint;
                    }
                }
                else if (inputComp.Walk)
                {
                    if (playerComp.IsWalking)
                    {
                        _targetSpeed = _cf.Value.playerConfiguration.moveSpeed;
                        _moveState = MoveState.RunForward;
                    }
                    else
                    {
                        _targetSpeed = _cf.Value.playerConfiguration.walkSpeed;
                        _moveState = MoveState.Walk;
                    }

                    playerComp.IsWalking = !playerComp.IsWalking;
                }

                if (inputComp.Move == Vector2.zero)
                {
                    _targetSpeed = 0f;
                    _moveState = MoveState.Idle;
                }

                var playerVelocity = playerComp.CharacterController.velocity;
                var currentHorizontalSpeed = new Vector3(playerVelocity.x, 0.0f, playerVelocity.z).magnitude;

                var speedOffset = 0.1f;
                var inputMagnitude = inputComp.Move.magnitude;

                if (currentHorizontalSpeed < _targetSpeed - speedOffset ||
                    currentHorizontalSpeed > _targetSpeed + speedOffset)
                {
                    _speed = Mathf.Lerp(currentHorizontalSpeed, _targetSpeed * inputMagnitude,
                        _ts.Value.DeltaTime * _cf.Value.playerConfiguration.speedChangeRate);

                    _speed = Mathf.Round(_speed * 1000f) / 1000f;
                }
                else
                {
                    _speed = _targetSpeed;
                }

                var inputDirection = new Vector3(inputComp.Move.x, 0f, inputComp.Move.y).normalized;

                if (inputComp.Move != Vector2.zero)
                {
                    _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                      _sd.Value.mainCamera.transform.eulerAngles.y;

                    float moveRotation;
                    if (inputComp.Move.y < 0)
                    {
                        moveRotation = _sd.Value.mainCamera.transform.eulerAngles.y;
                        _moveState = MoveState.RunBackward;
                    }
                    else
                    {
                        moveRotation = _targetRotation;
                    }

                    Debug.Log(moveRotation);

                    var rotation = Mathf.SmoothDampAngle(playerComp.Transform.eulerAngles.y, moveRotation,
                        ref _rotationVelocity,
                        _cf.Value.playerConfiguration.rotationSmoothTime);

                    playerComp.Rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
                    playerComp.Transform.rotation = playerComp.Rotation;
                }

                var targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

                playerComp.CharacterController.Move(targetDirection.normalized * (_speed * _ts.Value.DeltaTime) +
                                                    new Vector3(0.0f, playerComp.VerticalVelocity, 0.0f) *
                                                    _ts.Value.DeltaTime);

                HandleAnimationSpeed(animationComp);

                playerComp.Position = playerComp.Transform.position;
            }
        }

        private void HandleAnimationSpeed(AnimationComp animationComp)
        {
            switch (_moveState)
            {
                case MoveState.RunBackward:
                    _targetMoveSpeed = -0.75f;
                    break;
                case MoveState.Idle:
                    _targetMoveSpeed = 0.0f;
                    break;
                case MoveState.Walk:
                    _targetMoveSpeed = 0.25f;
                    break;
                case MoveState.RunForward:
                    _targetMoveSpeed = 0.75f;
                    break;
                case MoveState.Sprint:
                    _targetMoveSpeed = 1.0f;
                    break;
            }

            // Используем линейную интерполяцию для плавного изменения MoveSpeed
            _currentMoveSpeed = Mathf.Lerp(_currentMoveSpeed, _targetMoveSpeed, Time.deltaTime * SpeedChangeRate);

            // Устанавливаем значение MoveSpeed в аниматоре
            animationComp.Animator.SetFloat(MoveSpeed, _currentMoveSpeed);
        }
    }
}