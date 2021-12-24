using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EnemyFactory : GameObjectFactory
{
    [SerializeField] EnemyConfig small = default, medium = default, large = default;

    EnemyConfig GetConfig(Enemy.TYPE type)
    {
        switch (type)
        {
            case Enemy.TYPE.Small: return small;
            case Enemy.TYPE.Medium: return medium;
            case Enemy.TYPE.Large: return large;
        }

        Debug.Assert(false, "Unsupport enemy type!");
        return null;
    }

    public Enemy Get(Enemy.TYPE type = Enemy.TYPE.Medium)
    {
        EnemyConfig config = GetConfig(type);
        Enemy instance = CreateGameObjectInstance(config.prefab);
        instance.OriginFactory = this;
        instance.Initilize(
            config.scale.RandomValueInRange,
            config.speed.RandomValueInRange,
            config.pathOffset.RandomValueInRange,
            config.health.RandomValueInRange);
        return instance;

    }

    public void Reclaim(Enemy enemy)
    {
        Debug.Assert(enemy.OriginFactory == this, "Wrong factory reclaimed!");
        Destroy(enemy.gameObject);
    }


    [System.Serializable]
    class EnemyConfig
    {
        public Enemy prefab = default;

        [TowerFloatRangeSlider(0.5f, 2f)] public TowerFloatRange scale = new TowerFloatRange(1f);

        [TowerFloatRangeSlider(0.2f, 5f)] public TowerFloatRange speed = new TowerFloatRange(1f);

        [TowerFloatRangeSlider(-0.4f, 0.4f)] public TowerFloatRange pathOffset = new TowerFloatRange(0f);

        [TowerFloatRangeSlider(10f, 1000f)] public TowerFloatRange health = new TowerFloatRange(100f);

        enum Type
        {
            Small, Medium, Large
        }
    }

}
