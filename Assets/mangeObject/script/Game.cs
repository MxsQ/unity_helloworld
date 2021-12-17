using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Shape;

public class Game : PersisableObject
{
    public static Game Intance { get; private set; }

    [SerializeField] PersistentStorage storage;

    [SerializeField] KeyCode createKey = KeyCode.C;
    [SerializeField] KeyCode newGameKey = KeyCode.N;
    [SerializeField] KeyCode saveKey = KeyCode.S;
    [SerializeField] KeyCode loadKey = KeyCode.L;
    [SerializeField] KeyCode destroyKey = KeyCode.X;
    [SerializeField] int levelCount = 1;
    [SerializeField] bool reseedOnLoad;
    [SerializeField] Slider creationSpeedSlider;
    [SerializeField] Slider destructionSpeedSlider;
    [SerializeField] float destroyDuration;

    [SerializeField] ShapeFactory[] shapeFactories;

    List<Shape> shapes;
    List<ShapeInstance> killList, markAsDyingList;

    const int saveVersion = 6;

    int dyingShapeCount;

    public float CreationSpeed
    {
        get; set;
    }
    float creationProgress, destructionProgress;

    public float DestructionSpeed
    {
        get; set;
    }

    int loadedLevelBuildIndex;
    Random.State mainRandomState;

    bool inGameUpdateLoop;

    private void Start()
    {
        mainRandomState = Random.state;
        shapes = new List<Shape>();
        killList = new List<ShapeInstance>();
        markAsDyingList = new List<ShapeInstance>();

        if (Application.isEditor)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene loadedLevel = SceneManager.GetSceneAt(i);
                if (loadedLevel.name.Contains("Level "))
                {
                    loadedLevelBuildIndex = loadedLevel.buildIndex;
                    SceneManager.SetActiveScene(loadedLevel);
                    return;
                }
            }

        }

        BeginNewGame();
        StartCoroutine(LoadLevel(1));

    }

    private void FixedUpdate()
    {
        inGameUpdateLoop = true;
        for (int i = 0; i < shapes.Count; i++)
        {
            shapes[i].GameUpdate();
        }
        inGameUpdateLoop = false;
        if (killList.Count > 0)
        {
            for (int i = 0; i < killList.Count; i++)
            {
                if (killList[i].IsValid)
                {
                    KillImmediately(killList[i].Shape);
                }
            }
            killList.Clear();
        }
        if (markAsDyingList.Count > 0)
        {
            for (int i = 0; i < markAsDyingList.Count; i++)
            {
                if (markAsDyingList[i].IsValid)
                {
                    MarkAsDyingImmediately(markAsDyingList[i].Shape);
                }
                markAsDyingList.Clear();
            }
        }

        if (Input.GetKeyDown(createKey))
        {
            GameLevel.Current.SpawnShape();
        }
        else if (Input.GetKey(newGameKey))
        {
            BeginNewGame();
            StartCoroutine(LoadLevel(loadedLevelBuildIndex));
        }
        else if (Input.GetKeyDown(saveKey))
        {
            storage.Save(this, saveVersion);
        }
        else if (Input.GetKeyDown(loadKey))
        {
            BeginNewGame();
            storage.Load(this);
        }
        else if (Input.GetKeyDown(destroyKey))
        {
            DestroyShape();
        }
        else
        {
            for (int i = 1; i <= levelCount; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha0 + i))
                {
                    BeginNewGame();
                    StartCoroutine(LoadLevel(i));
                    return;
                }
            }
        }

        creationProgress += Time.deltaTime * CreationSpeed;
        while (creationProgress >= 1f)
        {
            creationProgress -= 1f;
            GameLevel.Current.SpawnShape();
        }

        destructionProgress += Time.deltaTime * DestructionSpeed;
        while (destructionProgress >= 1f)
        {
            destructionProgress -= 1f;
            DestroyShape();
        }

        int limit = GameLevel.Current.PopulationLimit;
        if (limit > 0)
        {
            while (shapes.Count - dyingShapeCount > limit)
            {
                DestroyShape();
            }
        }
    }

    private void OnEnable()
    {
        Intance = this;
        if (shapeFactories[0].FactoryId != 0)
        {
            for (int i = 0; i < shapeFactories.Length; i++)
            {
                shapeFactories[i].FactoryId = i;
            }
        }
    }

    public void AddShape(Shape shape)
    {
        shape.SaveIndex = shapes.Count;
        shapes.Add(shape);
    }

    public Shape GetShape(int index)
    {
        return shapes[index];
    }

    void DestroyShape()
    {
        if (shapes.Count - dyingShapeCount > 0)
        {
            Shape shape = shapes[Random.Range(dyingShapeCount, shapes.Count)];
            if (destroyDuration <= 0)
            {

                KillImmediately(shape);
            }
            else
            {
                shape.AddBehavior<DyingShapeBehavior>().Initialize(shape, destroyDuration);
            }
        }

    }

    public void Kill(Shape shape)
    {
        if (inGameUpdateLoop)
        {
            killList.Add(shape);
        }
        else
        {
            KillImmediately(shape);
        }


    }

    void KillImmediately(Shape shape)
    {
        int index = shape.SaveIndex;
        shape.Recycle();

        if (index < dyingShapeCount && index < --dyingShapeCount)
        {
            shapes[dyingShapeCount].SaveIndex = index;
            shapes[index] = shapes[dyingShapeCount];
            index = dyingShapeCount;
        }

        int lastIndex = shapes.Count - 1;
        if (index < lastIndex)
        {
            shapes[lastIndex].SaveIndex = index;
            shapes[index] = shapes[lastIndex];
        }

        shapes.RemoveAt(lastIndex);
    }

    public void MarkAsDying(Shape shape)
    {
        if (inGameUpdateLoop)
        {
            markAsDyingList.Add(shape);
        }
        else
        {
            MarkAsDyingImmediately(shape);
        }
    }

    void MarkAsDyingImmediately(Shape shape)
    {
        int index = shape.SaveIndex;
        if (index < dyingShapeCount)
        {
            return;
        }
        shapes[dyingShapeCount].SaveIndex = index;
        shapes[index] = shapes[dyingShapeCount];
        shape.SaveIndex = dyingShapeCount;
        shapes[dyingShapeCount++] = shape;
    }

    public bool IsMarkedAsDying(Shape shape)
    {
        return shape.SaveIndex < dyingShapeCount;
    }

    void BeginNewGame()
    {
        Random.state = mainRandomState;
        int seed = Random.Range(0, int.MaxValue);
        mainRandomState = Random.state;
        Random.InitState(seed);

        creationSpeedSlider.value = CreationSpeed = 0;
        destructionSpeedSlider.value = DestructionSpeed = 0;

        for (int i = 0; i < shapes.Count; i++)
        {
            shapes[i].Recycle();
        }
        shapes.Clear();

        dyingShapeCount = 0;
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(shapes.Count);
        writer.Write(Random.state);
        writer.Write(CreationSpeed);
        writer.Write(creationProgress);
        writer.Write(DestructionSpeed);
        writer.Write(destructionProgress);
        writer.Write(loadedLevelBuildIndex);
        GameLevel.Current.Save(writer);
        for (int i = 0; i < shapes.Count; i++)
        {
            writer.Write(shapes[i].OriginFactory.FactoryId);
            writer.Write(shapes[i].ShapeId);
            writer.Write(shapes[i].MaterialId);
            shapes[i].Save(writer);
        }
    }

    public override void Load(GameDataReader reader)
    {
        int version = reader.Version;
        StartCoroutine(LoadGame(reader));
    }

    IEnumerator LoadGame(GameDataReader reader)
    {
        int version = reader.Version;

        int count = version <= 0 ? -version : reader.ReadInt();

        if (version >= 3)
        {
            Random.State state = reader.ReadRandomState();
            if (!reseedOnLoad)
            {
                Random.state = state;
            }
            creationSpeedSlider.value = CreationSpeed = reader.ReadFloat();
            creationProgress = reader.ReadFloat();
            destructionSpeedSlider.value = DestructionSpeed = reader.ReadFloat();
            destructionProgress = reader.ReadFloat();
        }

        int level = reader.ReadInt();
        Debug.Log("load level " + level);
        yield return LoadLevel(version < 2 ? 1 : level);

        if (version >= 3)
        {
            GameLevel.Current.Load(reader);
        }

        for (int i = 0; i < count; i++)
        {
            int factoryId = version >= 5 ? reader.ReadInt() : 0;
            int shapeId = reader.ReadInt();
            int meterialId = reader.ReadInt();
            Shape instance = shapeFactories[factoryId].Get(shapeId, meterialId);
            instance.Load(reader);
        }

        for (int i = 0; i < shapes.Count; i++)
        {
            shapes[i].ResolveShapeInstance();
        }
    }

    IEnumerator LoadLevel(int levelBuildIndex)
    {
        enabled = false;
        if (loadedLevelBuildIndex > 0)
        {
            yield return SceneManager.UnloadSceneAsync(loadedLevelBuildIndex);
        }
        yield return SceneManager.LoadSceneAsync(
            levelBuildIndex, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(levelBuildIndex));
        loadedLevelBuildIndex = levelBuildIndex;
        enabled = true;
    }

}
