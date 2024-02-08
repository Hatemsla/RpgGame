using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using ObjectsPool;
using UnityEngine;
using World.Configurations;
using Object = UnityEngine.Object;

namespace World.UI.PopupText
{
    public class PopupDamageTextInitSystem : IEcsInitSystem
    {
        private readonly EcsCustomInject<Configuration> _cf = default;
        private readonly EcsCustomInject<PoolService> _ps = default;
        
        private const int PopupTextPreloadCount = 5;
        
        public void Init(IEcsSystems systems)
        {
            _ps.Value.PopupDamageTextPool = new PoolBase<PopupDamageText>(Preload, GetAction, ReturnAction, PopupTextPreloadCount);
        }
        
        private PopupDamageText Preload() => Object.Instantiate(_cf.Value.uiConfiguration.popupDamageTextPrefab, Vector3.zero, Quaternion.identity);

        private void GetAction(PopupDamageText popupDamageText) => popupDamageText.gameObject.SetActive(true);
        private void ReturnAction(PopupDamageText popupDamageText) => popupDamageText.gameObject.SetActive(false);
    }
}