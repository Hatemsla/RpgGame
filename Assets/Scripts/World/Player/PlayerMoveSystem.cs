using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace World.Player
{
    public sealed class PlayerMoveSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<PlayerComp, PlayerInputComp>> _playerMove = default;

        private readonly EcsCustomInject<Configuration> _cf = default;
        private readonly EcsCustomInject<SceneData> _sd = default;
        private readonly EcsCustomInject<TimeService> _ts = default;

        private float _speed;
        private float _targetRotation;
        private float _rotationVelocity;

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _playerMove.Value)
            {
                ref var player = ref _playerMove.Pools.Inc1.Get(entity);
                ref var input = ref _playerMove.Pools.Inc2.Get(entity);

                var targetSpeed = _cf.Value.playerConfiguration.moveSpeed;
                if (input.Sprint)
                    targetSpeed = _cf.Value.playerConfiguration.sprintSpeed;
                else if (input.Walk)
                    targetSpeed = _cf.Value.playerConfiguration.walkSpeed;

                if (input.Move == Vector2.zero) targetSpeed = 0f;

                var playerVelocity = player.CharacterController.velocity;
                var currentHorizontalSpeed = new Vector3(playerVelocity.x, 0.0f, playerVelocity.z).magnitude;

                var speedOffset = 0.1f;
                var inputMagnitude = input.Move.magnitude;

                if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                    currentHorizontalSpeed > targetSpeed + speedOffset)
                {
                    _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                        _ts.Value.DeltaTime * _cf.Value.playerConfiguration.speedChangeRate);

                    _speed = Mathf.Round(_speed * 1000f) / 1000f;
                }
                else
                {
                    _speed = targetSpeed;
                }

                var inputDirection = new Vector3(input.Move.x, 0f, input.Move.y).normalized;

                if (input.Move != Vector2.zero)
                {
                    _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                      _sd.Value.mainCamera.transform.eulerAngles.y;
                    var rotation = Mathf.SmoothDampAngle(player.Transform.eulerAngles.y, _targetRotation,
                        ref _rotationVelocity,
                        _cf.Value.playerConfiguration.rotationSmoothTime);

                    player.Rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
                }

                var targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

                player.CharacterController.Move(targetDirection.normalized * (_speed * _ts.Value.DeltaTime) +
                                                new Vector3(0.0f, player.VerticalVelocity, 0.0f) * _ts.Value.DeltaTime);

                player.Position = player.Transform.position;
            }
        }
    }
}