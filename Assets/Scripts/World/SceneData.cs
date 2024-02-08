using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using World.Ability;
using World.AI.Navigation;
using World.Inventory;
using World.Inventory.Chest;

namespace World
{
    public class SceneData : MonoBehaviour
    {
        public Transform playerSpawnPosition;
        public CinemachineBrain mainCamera;
        public UISceneData uiSceneData;
        public List<FastItemView> fastItemViews;
        public List<FastSkillView> fastSkillViews;
        public List<ChestObject> chests;
        public List<ZoneView> zones;
        public List<Trader.Trader> traders;
    }
}