using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using World.Configurations;
using World.Player;
using World.RPG;

namespace World.AI
{
    public sealed class EnemyRecoverySystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<EnemyComp, RpgComp>> _enemyFilter = default;

        private readonly EcsCustomInject<Configuration> _cf = default;
        private readonly EcsCustomInject<TimeService> _ts = default;
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _enemyFilter.Value)
            {
                ref var enemyComp = ref _enemyFilter.Pools.Inc1.Get(entity);
                ref var rpgComp = ref _enemyFilter.Pools.Inc2.Get(entity);
                
                if (rpgComp.Health < _cf.Value.enemyConfiguration.enemiesData[enemyComp.EnemyIndex].health)
                    rpgComp.Health += _cf.Value.enemyConfiguration.enemiesData[enemyComp.EnemyIndex].healthRecovery * _ts.Value.DeltaTime;

                if (rpgComp.Health > _cf.Value.enemyConfiguration.enemiesData[enemyComp.EnemyIndex].health)
                    rpgComp.Health = _cf.Value.enemyConfiguration.enemiesData[enemyComp.EnemyIndex].health;
                
                if(rpgComp.Stamina < _cf.Value.enemyConfiguration.enemiesData[enemyComp.EnemyIndex].stamina)
                    rpgComp.Stamina += _cf.Value.enemyConfiguration.enemiesData[enemyComp.EnemyIndex].staminaRecovery * _ts.Value.DeltaTime;
                
                if (rpgComp.Stamina > _cf.Value.enemyConfiguration.enemiesData[enemyComp.EnemyIndex].stamina)
                    rpgComp.Stamina = _cf.Value.enemyConfiguration.enemiesData[enemyComp.EnemyIndex].stamina;
                
                if(rpgComp.Mana < _cf.Value.enemyConfiguration.enemiesData[enemyComp.EnemyIndex].mana)
                    rpgComp.Mana += _cf.Value.enemyConfiguration.enemiesData[enemyComp.EnemyIndex].manaRecovery * _ts.Value.DeltaTime;
                
                if (rpgComp.Mana > _cf.Value.enemyConfiguration.enemiesData[enemyComp.EnemyIndex].mana)
                    rpgComp.Mana = _cf.Value.enemyConfiguration.enemiesData[enemyComp.EnemyIndex].mana;
            }
        }
    }
}