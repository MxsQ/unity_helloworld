using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameTile;

[CreateAssetMenu]
public class GameTileContentFactory : GameObjectFactory
{
    [SerializeField] GameTileContent destinationPrefab = default;
    [SerializeField] GameTileContent emptyPrefab = default;
    [SerializeField] GameTileContent wallPrefab = default;
    [SerializeField] GameTileContent spawnPointPrefab = default;
    [SerializeField] GameTileContent towarPrefab = default;
    [SerializeField] Tower[] towerPrefabs = default;

    public void Reclaim(GameTileContent content)
    {
        Debug.Assert(content.OriginFactory == this, "Wrng factory reclaimed");
        Destroy(content.gameObject);
    }

    public GameTileContent Get(GameTileContentType type)
    {
        switch (type)
        {
            case GameTileContentType.Destination:
                return Get(destinationPrefab);

            case GameTileContentType.Empty:
                return Get(emptyPrefab);

            case GameTileContentType.Wall:
                return Get(wallPrefab);

            case GameTileContentType.SpawnPoint:
                return Get(spawnPointPrefab);

        }

        Debug.Assert(false, "Unsupporte type: " + type);
        return null;
    }

    public Tower Get(Tower.TYPE type)
    {
        Debug.Assert((int)type < towerPrefabs.Length, "Unsupport tower type");
        Tower prefab = towerPrefabs[(int)type];
        Debug.Assert(type == prefab.TowerType, "Tower prefab at wrong index");
        return Get(prefab);
    }

    T Get<T>(T prefab) where T : GameTileContent
    {
        T instance = CreateGameObjectInstance(prefab);
        instance.OriginFactory = this;
        return instance;
    }
}
