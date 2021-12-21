using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EnemyFactory : GameObjectFactory
{
    [SerializeField] Enemy prefab = default;

    [SerializeField, TowerFloatRangeSlider(0.5f, 2f)] TowerFloatRange scale = new TowerFloatRange(1f);

    [SerializeField, TowerFloatRangeSlider(0.2f, 5f)] TowerFloatRange speed = new TowerFloatRange(1f);

    [SerializeField, TowerFloatRangeSlider(-0.4f, 0.4f)] TowerFloatRange pathOffset = new TowerFloatRange(0f);


    public Enemy Get()
    {
        Enemy instance = CreateGameObjectInstance(prefab);
        instance.OriginFactory = this;
        instance.Initilize(scale.RandomValueInRange, speed.RandomValueInRange, pathOffset.RandomValueInRange);
        return instance;
    }

    public void Reclaim(Enemy enemy)
    {
        Debug.Assert(enemy.OriginFactory == this, "Wrong factory reclaimed!");
        Destroy(enemy.gameObject);
    }
}
