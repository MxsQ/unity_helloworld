using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameTile;

[SelectionBase]
public class GameTileContent : MonoBehaviour
{
    GameTileContentFactory originFactory;

    [SerializeField] GameTileContentType type = default;

    public GameTileContentType Type => type;

    public bool BlocksPath => Type == GameTileContentType.Wall || Type == GameTileContentType.Tower;

    public GameTileContentFactory OriginFactory
    {
        get => originFactory;
        set
        {
            Debug.Assert(originFactory == null, "Redefined origin factoty");
            originFactory = value;
        }
    }

    public void Recycle()
    {
        originFactory.Reclaim(this);
    }

    public virtual void GameUpdate() { }
}
