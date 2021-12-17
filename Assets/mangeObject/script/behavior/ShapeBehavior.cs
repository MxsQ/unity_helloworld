using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SpawnZone.SpawnConfiguration;

public abstract class ShapeBehavior
#if UNITY_EDITOR
     : ScriptableObject
#endif

{
    public abstract ShapeBehaviorType behaviorType { get; }

    public abstract bool GameUpdate(Shape shape);

    public abstract void Save(GameDataWriter writer);

    public abstract void Load(GameDataReader reader);

    public abstract void Recycle();

    public bool IsReclaimed { get; set; }
#if UNITY_EDI

    private void OnEnable()
    {
        if (IsReclaimed)
        {
            Recycle();
        }
    }
#endif

    public virtual void ResolveShapeInstance() { }

}
