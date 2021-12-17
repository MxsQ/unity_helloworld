using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SpawnZone.SpawnConfiguration;

public class LifecycleShapeBehavior : ShapeBehavior
{
    Vector3 originalScale;
    float adultDuration, dyingDuration, dyingAge;

    public override ShapeBehaviorType behaviorType
    {
        get
        {
            return ShapeBehaviorType.FullLife;
        }
    }

    public void Initialize(Shape shape, float growingDuration, float adultDuration, float dyingDuration)
    {
        this.adultDuration = adultDuration;
        this.dyingDuration = dyingDuration;
        dyingAge = growingDuration + adultDuration;

        if (growingDuration > 0f)
        {
            shape.AddBehavior<GrowingShapeBehavior>().Initialize(shape, growingDuration);
        }
    }

    public override bool GameUpdate(Shape shape)
    {
        if (shape.Age >= dyingAge)
        {
            if (dyingDuration <= 0f)
            {
                shape.Die();
                return true;
            }

            if (!shape.IsMarkedDying)
            {
                shape.AddBehavior<DyingShapeBehavior>().Initialize(
               shape, dyingDuration + dyingAge - shape.Age);
            }

            return false;
        }
        return true;
    }

    public override void Load(GameDataReader reader)
    {
        adultDuration = reader.ReadFloat();
        dyingDuration = reader.ReadFloat();
        dyingAge = reader.ReadFloat();
    }

    public override void Recycle()
    {
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(adultDuration);
        writer.Write(dyingDuration);
        writer.Write(dyingAge);
    }
}
