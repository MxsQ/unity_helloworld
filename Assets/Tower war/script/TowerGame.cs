using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerGame : MonoBehaviour
{
    static TowerGame instance;

    [SerializeField] Vector2Int boardSize = new Vector2Int(11, 11);

    [SerializeField] GameBoard board = default;

    [SerializeField] GameTileContentFactory titleContentFactory = default;

    [SerializeField] EnemyFactory enemyFactory = default;

    [SerializeField] WarFactory warFactory = default;

    [SerializeField, Range(0.1f, 10f)] float spwanSpeed = 1f;

    Ray TouchRay => Camera.main.ScreenPointToRay(Input.mousePosition);

    float spawnProgress;

    GameBehaviorCollection enemies = new GameBehaviorCollection();
    GameBehaviorCollection nonEnemies = new GameBehaviorCollection();

    Tower.TYPE seletedTowerType;

    private void Awake()
    {
        board.Initialize(boardSize, titleContentFactory);
        board.ShowGrid = true;
    }

    private void OnValidate()
    {
        if (boardSize.x < 2)
        {
            boardSize.x = 2;
        }
        if (boardSize.y < 2)
        {
            boardSize.y = 2;
        }
    }

    private void OnEnable()
    {
        instance = this;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleTouch();
        }
        else if (Input.GetMouseButtonDown(1))
        {
            HandleAlternativeTouch();
        }
        else if (Input.GetKeyDown(KeyCode.V))
        {
            board.ShowPaths = !board.ShowPaths;
        }
        else if (Input.GetKeyDown(KeyCode.G))
        {
            board.ShowGrid = !board.ShowGrid;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            seletedTowerType = Tower.TYPE.Laser;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            seletedTowerType = Tower.TYPE.Mortar;
        }

        spawnProgress += spwanSpeed * Time.deltaTime;
        while (spawnProgress >= 1f)
        {
            spawnProgress -= 1f;
            SpawnEnemy();
        }

        enemies.GameUpdate();
        Physics.SyncTransforms();
        board.GameUpate();
        nonEnemies.GameUpdate();
    }

    void HandleAlternativeTouch()
    {
        GameTile tile = board.GetTile(TouchRay);
        if (tile != null)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                board.ToggleDestination(tile);
            }
            else
            {
                board.ToggleSpawnPoint(tile);
            }
        }
    }

    void HandleTouch()
    {
        GameTile tile = board.GetTile(TouchRay);
        if (tile != null)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                board.ToggleTower(tile, seletedTowerType);
            }
            else
            {

                board.ToggleWall(tile);
            }
        }
    }

    void SpawnEnemy()
    {
        GameTile spawnPoint = board.GetSpawnPoint(Random.Range(0, board.SpawnPointCount));
        Enemy enemy = enemyFactory.Get();
        enemy.SpawnOn(spawnPoint);
        enemies.Add(enemy);
    }

    public static Shell SpawnShell()
    {
        Shell shell = instance.warFactory.Shell;
        instance.nonEnemies.Add(shell);
        return shell;
    }

    public static Explosion SpawnExplosion()
    {
        Explosion explosion = instance.warFactory.Explosion;
        instance.nonEnemies.Add(explosion);
        return explosion;
    }
}