using System;
using System.Globalization;
using DG.Tweening;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using Utils.ObjectsPool;
using World.Configurations;

namespace World.UI.PopupText
{
    public class AnimatePopupDamageTextSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<PopupDamageTextComp>> _filter = default;

        private readonly EcsWorldInject _world = default;

        private readonly EcsCustomInject<PoolService> _ps = default;
        private readonly EcsCustomInject<TimeService> _ts = default;
        private readonly EcsCustomInject<Configuration> _cf = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var popupDamageTextComp = ref _filter.Pools.Inc1.Get(entity);

                if (!popupDamageTextComp.IsVisible) continue;

                popupDamageTextComp.LifeTime -= _ts.Value.DeltaTime;

                var popupDamageText = popupDamageTextComp.PopupDamageText;

                var color = popupDamageText.damageText.color;
                popupDamageText.damageText.color = new Color(color.r, color.g, color.b,
                    popupDamageText.opacityCurve.Evaluate(popupDamageText
                        .currentTime));

                popupDamageText.transform.localScale =
                    Vector3.one * popupDamageText.scaleCurve.Evaluate(popupDamageText.currentTime);

                popupDamageText.transform.position = popupDamageTextComp.Position + new Vector3(0,
                    1 + popupDamageText.heightCurve.Evaluate(popupDamageText.currentTime), 0);

                popupDamageText.currentTime += _ts.Value.DeltaTime;


                if (popupDamageTextComp.LifeTime <= 0)
                {
                    _ps.Value.PopupDamageTextPool.Return(popupDamageTextComp.PopupDamageText);
                    _world.Value.DelEntity(entity);

                    // var comp = popupDamageTextComp;
                    // popupDamageTextComp.PopupDamageText.damageText.DOFade(0f, 0.5f).OnComplete(() => {
                    //     _ps.Value.PopupDamageTextPool.Return(comp.PopupDamageText);
                    //     comp.PopupDamageText.damageText.alpha = 1;
                    //     _world.Value.DelEntity(entity);
                    //     
                    //     Debug.Log("Complete");
                    // });
                }
            }
        }
    }
}