using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameLevel : PersisableObject
{
    [SerializeField] int populationLimit;

    public static GameLevel Current { get; private set; }

    [SerializeField] SpawnZone spawnZone;
    [UnityEngine.Serialization.FormerlySerializedAs("levelObjects")]
    [SerializeField] GameLevelObject[] levelObjects;

    public int PopulationLimit
    {
        get
        {
            return populationLimit;
        }
    }

    //public bool HasMissingLevelObjects
    //{
    //    get
    //    {
    //        if (levelObjects != null)
    //        {
    //            for (int i = 0; i < levelObjects.Length; i++)
    //            {
    //                if (levelObjects[i] == null)
    //                {
    //                    return true;
    //                }
    //            }
    //        }

    //        return false;
    //    }
    //}

    public void GameUpdate()
    {
        for (int i = 0; i < levelObjects.Length; i++)
        {
            levelObjects[i].GameUpdate();
        }
    }


    private void OnEnable()
    {
        Current = this;
        if (levelObjects == null)
        {
            levelObjects = new GameLevelObject[0];
        }
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(levelObjects.Length);
        for (int i = 0; i < levelObjects.Length; i++)
        {
            levelObjects[i].Save(writer);
        }
    }

    public override void Load(GameDataReader reader)
    {
        int saveCount = reader.ReadInt();
        for (int i = 0; i < saveCount; i++)
        {
            levelObjects[i].Load(reader);
        }
    }

    public void SpawnShape()
    {
        spawnZone.SpawnShape();
    }

    //public void RemoveMissingLevelObjects()
    //{
    //    if (Application.isPlaying)
    //    {
    //        Debug.LogError("Do not invoke in play mode");
    //        return;
    //    }

    //    int holes = 0;
    //    for (int i = 0; i < levelObjects.Length - holes; i++)
    //    {
    //        if (levelObjects[i] == null)
    //        {
    //            holes++;
    //            System.Array.Copy(levelObjects, i + 1, levelObjects, i, levelObjects.Length - i - holes);
    //            i -= 1;
    //        }
    //    }

    //    System.Array.Resize(ref levelObjects, levelObjects.Length - holes);
    //}

    //public void RegisterLevelObject(GameLevelObject o)
    //{
    //    if (Application.isPlaying)
    //    {
    //        Debug.LogError("Do not invoke in play mode");
    //        return;
    //    }

    //    if (HasLevelObject(o))
    //    {
    //        return;
    //    }

    //    if (levelObjects == null)
    //    {
    //        levelObjects = new GameLevelObject[] { o };
    //    }
    //    else
    //    {
    //        System.Array.Resize(ref levelObjects, levelObjects.Length + 1);
    //        levelObjects[levelObjects.Length - 1] = o;
    //    }
    //}

    //public bool HasLevelObject(GameLevelObject o)
    //{
    //    if (levelObjects != null)
    //    {
    //        for (int i = 0; i < levelObjects.Length; i++)
    //        {
    //            if (levelObjects[i] == o)
    //            {
    //                return true;
    //            }
    //        }
    //    }

    //    return false;
    //}
}
