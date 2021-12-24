using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerGame : MonoBehaviour
{
    static TowerGame instance;

    const float pausedTimeScale = 0f;

    [SerializeField] Vector2Int boardSize = new Vector2Int(11, 11);

    [SerializeField] GameBoard board = default;

    [SerializeField] GameTileContentFactory titleContentFactory = default;

    //[SerializeField] EnemyFactory enemyFactory = default;

    [SerializeField] WarFactory warFactory = default;

    //[SerializeField, Range(0.1f, 10f)] float spwanSpeed = 1f;

    [SerializeField] GameScenario scenario = default;

    [SerializeField, Range(0, 100)] int startingPlayerHealth = 10;

    [SerializeField, Range(1f, 10f)] float playSpeed = 1f;

    Ray TouchRay => Camera.main.ScreenPointToRay(Input.mousePosition);

    //float spawnProgress;

    GameBehaviorCollection enemies = new GameBehaviorCollection();
    GameBehaviorCollection nonEnemies = new GameBehaviorCollection();

    Tower.TYPE seletedTowerType;

    GameScenario.State activeScenario;

    int playerHealth;

    public static void EnemyReachedDestination()
    {
        instance.playerHealth -= 1;
    }

    private void Awake()
    {
        playerHealth = startingPlayerHealth;
        board.Initialize(boardSize, titleContentFactory);
        board.ShowGrid = true;
        activeScenario = scenario.Begin();
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
        else if (Input.GetKeyDown(KeyCode.B))
        {
            BeginNewGame();
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            Time.timeScale = Time.timeScale > pausedTimeScale ? pausedTimeScale : 1f;
        }
        else if (Time.timeScale > pausedTimeScale)
        {
            Time.timeScale = playSpeed;
        }

        if (playerHealth <= 0 && startingPlayerHealth > 0)
        {
            Debug.Log("Defeat!");
            BeginNewGame();
        }

        if (!activeScenario.Progress() && enemies.IsEmpty)
        {
            Debug.Log("Victory");
            BeginNewGame();
            activeScenario.Progress();
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

    public static void SpawnEnemy(EnemyFactory factory, Enemy.TYPE type)
    {
        GameTile spawnPoint = instance.board.GetSpawnPoint(Random.Range(0, instance.board.SpawnPointCount));
        Enemy enemy = factory.Get(type);
        enemy.SpawnOn(spawnPoint);
        instance.enemies.Add(enemy);
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

    void BeginNewGame()
    {
        playerHealth = startingPlayerHealth;
        enemies.Clear();
        nonEnemies.Clear();
        board.Clear();
        activeScenario = scenario.Begin();
    }
}
