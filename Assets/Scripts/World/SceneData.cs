using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using World.AI.Navigation;
using World.Inventory.Chest;

namespace World
{
    public class SceneData : MonoBehaviour
    {
        public Transform playerTransform;
        public Transform playerSpawnPosition;
        public CinemachineBrain mainCamera;
        public List<ChestObject> chests;
        public List<ZoneView> zones;
    }
}