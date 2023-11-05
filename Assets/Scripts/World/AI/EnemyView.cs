using System.Collections;
using Leopotam.EcsLite;
using UnityEngine;
using World.AI.Navigation;
using World.Player;

namespace World.AI
{
    public sealed class EnemyView : MonoBehaviour
    {
        public EcsPackedEntity ZonePacked;
        public EcsPackedEntity EnemyPacked;

        public float currentAttackDelay;

        private int _enemyEntity;
        private EcsWorld _world;
        private EcsPool<HasEnemies> _hasEnemiesPool;
        private EcsPool<EnemyComp> _enemyPool;
        private EcsPool<RpgComp> _rpgPool;
        private EcsPool<ZoneComp> _zonePool;

        public void SetWorld(EcsWorld world, int entity)
        {
            _world = world;
            _enemyEntity = entity;
            _hasEnemiesPool = _world.GetPool<HasEnemies>();
            _enemyPool = _world.GetPool<EnemyComp>();
            _rpgPool = _world.GetPool<RpgComp>();
            _zonePool = _world.GetPool<ZoneComp>();
        }

        public IEnumerator AttackDelay(float targetAttackDelay)
        {
            currentAttackDelay = 0;
            while (currentAttackDelay < targetAttackDelay)
            {
                currentAttackDelay += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }
    }
}