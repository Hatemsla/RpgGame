using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace World.Player
{
    public sealed class PlayerInputSystem : IEcsRunSystem, IEcsInitSystem
    {
        private readonly EcsFilterInject<Inc<PlayerComp, PlayerInputComp>> _players = default;
        private readonly EcsCustomInject<MainInput> _mn = default;

        public void Init(IEcsSystems systems)
        {
            _mn.Value.Player.Enable();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _players.Value)
            {
                ref var inputComp = ref _players.Pools.Inc2.Get(entity);
                var input = _mn.Value.Player;

                inputComp.Move = input.Move.ReadValue<Vector2>();
                inputComp.Look = input.Look.ReadValue<Vector2>();
                inputComp.Jump = input.Jump.WasPerformedThisFrame();
                inputComp.Sprint = input.Sprint.IsPressed();
                inputComp.Zoom = input.Zoom.ReadValue<Vector2>().y; // вращение колесика по y
                inputComp.Walk = input.Walk.WasPerformedThisFrame();
                inputComp.Dash = input.Dash.WasPerformedThisFrame();
                inputComp.FreeLook = input.FreeLook.IsPressed();
                inputComp.Alpha1 = input.Alpha1.WasPerformedThisFrame();
                inputComp.Alpha2 = input.Alpha2.WasPerformedThisFrame();
                inputComp.Alpha3 = input.Alpha3.WasPerformedThisFrame();
                inputComp.Alpha4 = input.Alpha4.WasPerformedThisFrame();
                inputComp.Alpha5 = input.Alpha5.WasPerformedThisFrame();
                inputComp.Alpha6 = input.Alpha6.WasPerformedThisFrame();
                inputComp.Inventory = input.Inventory.WasPerformedThisFrame();
                inputComp.AutoRun = input.AutoRun.WasPerformedThisFrame();
                inputComp.FreeCursor = input.FreeCursor.IsPressed();
                inputComp.ActiveAction = input.ActiveAction.WasPerformedThisFrame();
                inputComp.SkillList = input.SkillList.WasPerformedThisFrame();
                inputComp.Skill1 = input.Skill1.WasPerformedThisFrame();
                inputComp.Skill2 = input.Skill2.WasPerformedThisFrame();
                inputComp.Skill3 = input.Skill3.WasPerformedThisFrame();
                inputComp.Stats = input.Stats.WasPerformedThisFrame();
                inputComp.Attack = input.Attack.WasPerformedThisFrame();
            }
        }
    }
}