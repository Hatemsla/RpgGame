using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.Unity.Ugui;
using UnityEngine.Scripting;
using Utils;
using World.Player;

namespace World.Inventory
{
    public class DeleteFormSystem : EcsUguiCallbackSystem
    {
        private readonly EcsFilterInject<Inc<PlayerComp, InventoryComp>> _playerFilter = default;

        private readonly EcsPoolInject<DeleteEvent> _deleteEventPool = default;
        
        [Preserve]
        [EcsUguiClickEvent(Idents.UI.YesDeleteBtn, Idents.Worlds.Events)]
        private void OnClickYesDelete(in EcsUguiClickEvent e)
        {
            foreach (var entity in _playerFilter.Value)
            {
                ref var deleteEvent = ref _deleteEventPool.Value.Add(entity);
                deleteEvent.Result = true;
            }
        }
        
        [Preserve]
        [EcsUguiClickEvent(Idents.UI.NoDeleteBtn, Idents.Worlds.Events)]
        private void OnClickNoDelete(in EcsUguiClickEvent e)
        {
            foreach (var entity in _playerFilter.Value)
            {
                _deleteEventPool.Value.Add(entity);
            }
        }
    }
}