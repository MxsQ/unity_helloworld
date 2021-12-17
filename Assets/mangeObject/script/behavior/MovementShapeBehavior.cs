using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SpawnZone.SpawnConfiguration;

public sealed class MovementShapeBehavior : ShapeBehavior
{
    public override ShapeBehaviorType behaviorType
    {
        get
        {
            return ShapeBehaviorType.Movement;
        }
    }


    public Vector3 Velocity { get; set; }

    public override bool GameUpdate(Shape shape)
    {
        shape.transform.localPosition += Velocity * Time.deltaTime;
        return true;
    }

    public override void Load(GameDataReader reader)
    {
        Velocity = reader.ReadVector3();
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(Velocity);
    }

    public override void Recycle()
    {
        ShapeBehaviorPool<MovementShapeBehavior>.Reclaim(this);
    }
}
