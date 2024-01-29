using System.Collections.Generic;
using System.Linq;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using World.Configurations;
using World.Player;

namespace World.UI.LookOnObject
{
    public class LookOnObjectSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<PlayerComp, PlayerInputComp>> _filter = default;
        
        private readonly EcsCustomInject<SceneData> _sd = default;
        private readonly EcsCustomInject<Configuration> _cf = default;

        private List<LookOnObject> _lookOnObjects = new();
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var inputComp = ref _filter.Pools.Inc2.Get(entity);
                
                var centerOfScreen = new Vector3(Screen.width / 2, Screen.height / 2, 0);
                var ray = _sd.Value.mainCamera.OutputCamera.ScreenPointToRay(centerOfScreen);
            
                if (Physics.Raycast(ray, out var hit, 10f, _cf.Value.uiConfiguration.lookObjectMask, QueryTriggerInteraction.Collide))
                {
                    var hitLookOnObject = hit.transform.GetComponent<LookOnObject>();
                    if (hitLookOnObject)
                    {
                        foreach (var lookOnObject in _lookOnObjects)
                        {
                            lookOnObject.canvasGroup.alpha = hitLookOnObject == lookOnObject ? 1 : 0;

                            if (hit.distance <= 5)
                            {
                                lookOnObject.lookText.color = Color.yellow;

                                if (inputComp.ActiveAction)
                                {
                                    lookOnObject.StartInteract();

                                    lookOnObject.isInteracting = !lookOnObject.isInteracting;
                                }
                            }
                            else
                            {
                                if (lookOnObject.isInteracting)
                                {
                                    lookOnObject.StopInteract();
                                    lookOnObject.isInteracting = false;
                                }
                                
                                lookOnObject.lookText.color = lookOnObject.defaultTextColor;
                            }
                        }
                    }
                }
                else
                {
                    _lookOnObjects.ForEach(o =>
                    {
                        o.canvasGroup.alpha = 0;
                        if (o.isInteracting)
                        {
                            o.StopInteract();
                            o.isInteracting = false;
                        }
                    });
                }
            }    
        }

        public void Init(IEcsSystems systems)
        {
            _lookOnObjects = Resources.FindObjectsOfTypeAll<LookOnObject>().ToList();
            _lookOnObjects.RemoveAll(obj => !Utils.Utils.IsInScene(obj.gameObject));
        }
    }
}