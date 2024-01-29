using System.Collections.Generic;
using System.Linq;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using World.Player;

namespace World.UI
{
    public class UIBillboardingSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<PlayerComp>> _filter = default;
        
        private List<UIBillboardingView> _billboardingViews = new();
        
        public void Init(IEcsSystems systems)
        {
            _billboardingViews = Resources.FindObjectsOfTypeAll<UIBillboardingView>().ToList();
            _billboardingViews.RemoveAll(obj => !Utils.Utils.IsInScene(obj.gameObject));
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var playerComp = ref _filter.Pools.Inc1.Get(entity);

                foreach (var billboardingView in _billboardingViews)
                {
                    billboardingView.transform.forward = playerComp.PlayerCameraRootTransform.forward;
                }
            }
        }
    }
}