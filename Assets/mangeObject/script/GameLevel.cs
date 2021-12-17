using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLevel : PersisableObject
{
    [SerializeField] int populationLimit;

    public static GameLevel Current { get; private set; }

    [SerializeField] SpawnZone spawnZone;
    [SerializeField] PersisableObject[] persisableObjects;

    public int PopulationLimit
    {
        get
        {
            return populationLimit;
        }
    }

    private void OnEnable()
    {
        Current = this;
        if (persisableObjects == null)
        {
            persisableObjects = new PersisableObject[0];
        }
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(persisableObjects.Length);
        for (int i = 0; i < persisableObjects.Length; i++)
        {
            persisableObjects[i].Save(writer);
        }
    }

    public override void Load(GameDataReader reader)
    {
        int saveCount = reader.ReadInt();
        for (int i = 0; i < saveCount; i++)
        {
            persisableObjects[i].Load(reader);
        }
    }

    public void SpawnShape()
    {
        spawnZone.SpawnShape();
    }
}
