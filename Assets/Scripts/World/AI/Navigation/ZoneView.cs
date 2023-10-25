using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace World.AI.Navigation
{
    public sealed class ZoneView : MonoBehaviour
    {
        public int enemiesCount;
        public List<EnemyData> enemiesType;
        public List<TargetView> targets;

        private void Start()
        {
            targets = GetComponentsInChildren<TargetView>().ToList();
        }
    }
}