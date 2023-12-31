using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using Utils;

namespace World.Network
{
    public class NetworkInitSystem : IEcsInitSystem
    {
        private readonly EcsPoolInject<NetworkComp> _networkPool = default;
        
        private readonly EcsWorldInject _world = default;
        
        public void Init(IEcsSystems systems)
        {
            var networkEntity = _world.Value.NewEntity();
            var networkComp = _networkPool.Value.Add(networkEntity);

            networkComp.token = ConnectionTokenUtils.HashToken(ConnectionTokenUtils.NewToken());
        }
    }
}