using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarEntity : GameBehavior
{
    WarFactory originFactory;

    public WarFactory OriginFactory
    {
        get => originFactory;
        set
        {
            Debug.Assert(OriginFactory == null, "Redefined origin factory!");
            originFactory = value;
        }
    }

    public override void Recycle()
    {
        originFactory.Reclaim(this);
    }
}
