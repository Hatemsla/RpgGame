using System.IO;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Newtonsoft.Json;
using UnityEngine;
using Utils;
using World.SaveGame;

namespace World.LoadGame
{
    public class LoadDataSystem : IEcsInitSystem
    {
        private readonly EcsPoolInject<LoadDataEventComp> _loadDataEventPool = Idents.Worlds.Events;

        private readonly EcsWorldInject _eventWorld = Idents.Worlds.Events;
        
        public void Init(IEcsSystems systems)
        {
            var load = PlayerPrefs.GetInt("Load Progress");
            
            var loadDataEventEntity = _eventWorld.Value.NewEntity();
            ref var loadDataEventComp = ref _loadDataEventPool.Value.Add(loadDataEventEntity);

            if (load == 1)
            {
                loadDataEventComp.IsLoadData = true;
                
                var playerFilePath = Path.Combine(Application.persistentDataPath, "playerData.json");
                var playerJsonData = File.ReadAllText(playerFilePath);
                
                var chestFilePath = Path.Combine(Application.persistentDataPath, "chestData.json");
                var chestJsonData = File.ReadAllText(chestFilePath);
                
                var traderFilePath = Path.Combine(Application.persistentDataPath, "traderData.json");
                var traderJsonData = File.ReadAllText(traderFilePath);
                
                loadDataEventComp.PlayerSaveData = JsonConvert.DeserializeObject<PlayerSaveData>(playerJsonData);
                loadDataEventComp.ChestSaveDatas = JsonConvert.DeserializeObject<ChestSaveDatas>(chestJsonData);
                loadDataEventComp.TraderSaveDatas = JsonConvert.DeserializeObject<TraderSaveDatas>(traderJsonData);
            }
            else
            {
                loadDataEventComp.IsLoadData = false;
            }
        }
    }
}