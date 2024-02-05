using System.Collections;
using Leopotam.EcsLite;
using UnityEngine;
using World.AI.Navigation;
using World.Player;
using World.RPG;

namespace World.AI
{
    public sealed class EnemyView : MonoBehaviour
    {
        public EcsPackedEntity ZonePackedIdx;
        public EcsPackedEntity EnemyPackedIdx;

        public float currentAttackDelay;

        private int _enemyEntity;
        private EcsWorld _world;
        private EcsPool<HasEnemies> _hasEnemiesPool;
        private EcsPool<EnemyComp> _enemyPool;
        private EcsPool<RpgComp> _rpgPool;
        private EcsPool<ZoneComp> _zonePool;
        private EcsPool<AnimationComp> _animationPool;

        public void SetWorld(EcsWorld world, int entity)
        {
            _world = world;
            _enemyEntity = entity;
            _hasEnemiesPool = _world.GetPool<HasEnemies>();
            _enemyPool = _world.GetPool<EnemyComp>();
            _rpgPool = _world.GetPool<RpgComp>();
            _zonePool = _world.GetPool<ZoneComp>();
            _animationPool = _world.GetPool<AnimationComp>();
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

        public IEnumerator Respawn(float respawnDelay)
        {
            yield return new WaitForSeconds(respawnDelay);
            
            gameObject.SetActive(true);
        }
    }
}