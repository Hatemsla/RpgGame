using System.IO;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Newtonsoft.Json;
using UnityEngine;
using Utils;
using World.Inventory;
using World.Inventory.Chest;
using World.Player;
using World.RPG;
using World.Trader;

namespace World.SaveGame
{
    public class SaveSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<SaveEventComp>> _saveEventFilter = Idents.Worlds.Events;
        private readonly EcsFilterInject<Inc<PlayerComp, RpgComp, LevelComp, InventoryComp, HasItems>> _playerFilter =
            default;
        private readonly EcsFilterInject<Inc<ChestComp, HasItems, InventoryComp>> _chestFilter = default;
        private readonly EcsFilterInject<Inc<TraderComp>> _traderFilter = default;

        private readonly EcsPoolInject<ItemComp> _itemPool = default;

        private readonly EcsWorldInject _defaultWorld = default;
        
        
        public void Run(IEcsSystems systems)
        {
            foreach (var saveEventEntity in _saveEventFilter.Value)
            {
                SavePlayerData();
                SaveChestsData();
                SaveTradersData();
            }
        }

        private void SaveTradersData()
        {
            var traderDatas = new TraderSaveDatas();
            foreach (var traderEntity in _traderFilter.Value)
            {
                ref var traderComp = ref _traderFilter.Pools.Inc1.Get(traderEntity);

                var traderSaveData = new TraderSaveData()
                {
                    GoldAmount = traderComp.Trader.goldAmount
                };

                traderDatas.Traders.Add(traderSaveData);
            }

            var traderJsonData = JsonConvert.SerializeObject(traderDatas, Formatting.Indented,
                new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

            var traderFilePath = Path.Combine(Application.persistentDataPath, "traderData.json");
            File.WriteAllText(traderFilePath, traderJsonData);
        }

        private void SaveChestsData()
        {
            var chestDatas = new ChestSaveDatas();
            foreach (var chestEntity in _chestFilter.Value)
            {
                ref var chestComp = ref _chestFilter.Pools.Inc1.Get(chestEntity);
                ref var hasItemsComp = ref _chestFilter.Pools.Inc2.Get(chestEntity);
                ref var inventoryComp = ref _chestFilter.Pools.Inc3.Get(chestEntity);

                var itemDatas = new ItemDatas();
                foreach (var itemPackedEntity in hasItemsComp.Entities)
                {
                    if (itemPackedEntity.Unpack(_defaultWorld.Value, out var unpackedItemEntity))
                    {
                        ref var itemComp = ref _itemPool.Value.Get(unpackedItemEntity);

                        var itemData = ConvertToItemData(itemComp);
                        itemDatas.Items.Add(itemData);
                    }
                }

                var chestSaveData = new ChestSaveData
                {
                    ChestData = new ChestData(),
                    ItemDatas = itemDatas,
                    InventoryData = ConvertToInventoryData(inventoryComp)
                };

                chestDatas.ChestDatas.Add(chestSaveData);
            }

            var chestJsonData = JsonConvert.SerializeObject(chestDatas, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            var chestFilePath = Path.Combine(Application.persistentDataPath, "chestData.json");
            File.WriteAllText(chestFilePath, chestJsonData);
        }

        private void SavePlayerData()
        {
            foreach (var playerEntity in _playerFilter.Value)
            {
                ref var playerComp = ref _playerFilter.Pools.Inc1.Get(playerEntity);
                ref var rpgComp = ref _playerFilter.Pools.Inc2.Get(playerEntity);
                ref var levelComp = ref _playerFilter.Pools.Inc3.Get(playerEntity);
                ref var inventoryComp = ref _playerFilter.Pools.Inc4.Get(playerEntity);
                ref var hasItems = ref _playerFilter.Pools.Inc5.Get(playerEntity);

                var itemDatas = new ItemDatas();
                foreach (var itemPackedEntity in hasItems.Entities)
                {
                    if (itemPackedEntity.Unpack(_defaultWorld.Value, out var unpackedItemEntity))
                    {
                        ref var itemComp = ref _itemPool.Value.Get(unpackedItemEntity);

                        var itemData = ConvertToItemData(itemComp);
                        itemDatas.Items.Add(itemData);
                    }
                }

                var playerSaveData = new PlayerSaveData
                {
                    PlayerData = ConvertToPlayerData(playerComp),
                    RpgData = ConvertToRpgData(rpgComp),
                    LevelData = ConvertToLevelData(levelComp),
                    InventoryData = ConvertToInventoryData(inventoryComp),
                    ItemDatas = itemDatas
                };

                var playerJsonData = JsonConvert.SerializeObject(playerSaveData, Formatting.Indented, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
                var playerFilePath = Path.Combine(Application.persistentDataPath, "playerData.json");
                File.WriteAllText(playerFilePath, playerJsonData);
            }
        }

        private ItemData ConvertToItemData(ItemComp itemComp)
        {
            var itemData = new ItemData
            {
                ItemName = itemComp.ItemName,
                ItemDescription = itemComp.ItemDescription,
                Cost = itemComp.Cost,
                Weight = itemComp.Weight
            };
                
            return itemData;
        } 

        private InventoryData ConvertToInventoryData(InventoryComp inventoryComp)
        {
            var inventoryData = new InventoryData
            {
                MaxWeight = inventoryComp.MaxWeight,
                CurrentWeight = inventoryComp.CurrentWeight
            };
                
            return inventoryData;
        }

        private LevelData ConvertToLevelData(LevelComp levelComp)
        {
            var levelData = new LevelData
            {
                Level = levelComp.Level,
                Experience = levelComp.Experience,
                ExperienceToNextLevel = levelComp.ExperienceToNextLevel,
                AwardExperienceDiv = levelComp.AwardExperienceDiv,

                LevelScore = levelComp.LevelScore,
                SpentLevelScore = levelComp.SpentLevelScore,

                Strength = levelComp.Strength,
                Dexterity = levelComp.Dexterity,
                Constitution = levelComp.Constitution,
                Intelligence = levelComp.Intelligence,
                Charisma = levelComp.Charisma,
                Luck = levelComp.Luck,

                PreviousStrength = levelComp.PreviousStrength,
                PreviousDexterity = levelComp.PreviousDexterity,
                PreviousConstitution = levelComp.PreviousConstitution,
                PreviousIntelligence = levelComp.PreviousIntelligence,
                PreviousCharisma = levelComp.PreviousCharisma,
                PreviousLuck = levelComp.PreviousLuck,

                PAtk = levelComp.PAtk,
                MAtk = levelComp.MAtk,
                Spd = levelComp.Spd,
                MaxHp = levelComp.MaxHp,
                MaxSt = levelComp.MaxSt,
                MaxSp = levelComp.MaxSp,

                PreviousPAtk = levelComp.PreviousPAtk,
                PreviousMAtk = levelComp.PreviousMAtk,
                PreviousSpd = levelComp.PreviousSpd,
                PreviousMaxHp = levelComp.PreviousMaxHp,
                PreviousMaxSt = levelComp.PreviousMaxSt,
                PreviousMaxSp = levelComp.PreviousMaxSp
            };
            
            return levelData;
        }

        private RpgData ConvertToRpgData(RpgComp rpgComp)
        {
            var rpgData = new RpgData
            {
                Health = rpgComp.Health,
                Mana = rpgComp.Mana,
                Stamina = rpgComp.Stamina,
                CanRun = rpgComp.CanRun,
                CanDash = rpgComp.CanDash,
                CanJump = rpgComp.CanJump,
                CastDelay = rpgComp.CastDelay,
                IsDead = rpgComp.IsDead
            };

            return rpgData;
        }

        private PlayerData ConvertToPlayerData(PlayerComp playerComp)
        {
            var playerData = new PlayerData
            {
                Position = playerComp.Position,
                Rotation = playerComp.Rotation,
                Grounded = playerComp.Grounded,
                VerticalVelocity = playerComp.VerticalVelocity,
                IsWalking = playerComp.IsWalking,
                CameraSense = playerComp.CameraSense,
                GoldAmount = playerComp.GoldAmount,
                CanMove = playerComp.CanMove,
            };

            return playerData;
        }
    }
}