using System.Collections.Generic;
using UnityEngine;
using static SpawnZone.SpawnConfiguration;

public class Shape : PersisableObject
{
    static int colorPropertyId = Shader.PropertyToID("_Color");
    static MaterialPropertyBlock sharedPropertyBlock;

    [SerializeField]
    MeshRenderer[] meshRenders;

    public float Age { get; private set; }

    public int InstanceId { get; private set; }

    List<ShapeBehavior> behaviorList = new List<ShapeBehavior>();

    private void Awake()
    {
        colors = new Color[meshRenders.Length];
    }


    public int ShapeId
    {
        get
        {
            return shapeId;
        }
        set
        {
            if (shapeId == int.MinValue && value != int.MinValue)
            {
                shapeId = value;
            }
            else
            {
                Debug.LogError("Not allowed to change shapeId.");
            }
        }
    }

    int shapeId = int.MinValue;

    public int MaterialId
    {
        get;
        private set;
    }

    public void SetMaterial(Material material, int materialId)
    {
        for (int i = 0; i < meshRenders.Length; i++)
        {
            meshRenders[i].material = material;
        }
        MaterialId = materialId;
    }

    Color[] colors;

    public int ColorCount
    {
        get
        {
            return colors.Length;
        }
    }

    public void SetColor(Color color)
    {
        if (sharedPropertyBlock == null)
        {
            sharedPropertyBlock = new MaterialPropertyBlock();
        }
        sharedPropertyBlock.SetColor(colorPropertyId, color);
        for (int i = 0; i < meshRenders.Length; i++)
        {
            colors[i] = color;
            meshRenders[i].SetPropertyBlock(sharedPropertyBlock);
        }
    }

    public void SetColor(Color color, int index)
    {
        if (sharedPropertyBlock == null)
        {
            sharedPropertyBlock = new MaterialPropertyBlock();
        }
        sharedPropertyBlock.SetColor(colorPropertyId, color);
        colors[index] = color;
        meshRenders[index].SetPropertyBlock(sharedPropertyBlock);
    }

    public int SaveIndex { get; set; }

    ShapeFactory originFactory;
    public ShapeFactory OriginFactory
    {
        get
        {
            return originFactory;
        }
        set
        {
            if (originFactory == null)
            {
                originFactory = value;
            }
            else
            {
                Debug.LogError("Not allowed to change origin factory");
            }
        }
    }

    public bool IsMarkedDying
    {
        get
        {
            return Game.Intance.IsMarkedAsDying(this);
        }
    }


    public T AddBehavior<T>() where T : ShapeBehavior, new()
    {
        T behavior = ShapeBehaviorPool<T>.Get();
        behaviorList.Add(behavior);
        return behavior;
    }

    public override void Save(GameDataWriter writer)
    {
        base.Save(writer);
        writer.Write(colors.Length);
        for (int i = 0; i < colors.Length; i++)
        {
            writer.Write(colors[i]);

        }

        writer.Write(Age);
        writer.Write(behaviorList.Count);
        for (int i = 0; i < behaviorList.Count; i++)
        {
            writer.Write((int)behaviorList[i].behaviorType);
            behaviorList[i].Save(writer);
        }
    }

    public override void Load(GameDataReader reader)
    {
        base.Load(reader);
        if (reader.Version >= 5)
        {
            LoadColor(reader);
        }
        else
        {
            SetColor(reader.Version > 0 ? reader.ReadColor() : Color.white);
        }

        if (reader.Version >= 6)
        {
            Age = reader.ReadFloat();
            int behaviorCount = reader.ReadInt();
            for (int i = 0; i < behaviorCount; i++)
            {
                ShapeBehavior behavior = ((ShapeBehaviorType)reader.ReadInt()).GetInstance();
                behaviorList.Add(behavior);
                behavior.Load(reader);
            }
        }
        else if (reader.Version >= 4)
        {

            AddBehavior<RotationShapeBehavior>().AngularVelocity = reader.ReadVector3();
            AddBehavior<MovementShapeBehavior>().Velocity = reader.ReadVector3();
        }
    }

    private void LoadColor(GameDataReader reader)
    {
        int count = reader.ReadInt();
        int max = count <= colors.Length ? count : colors.Length;
        int i = 0;
        for (; i < max; i++)
        {
            SetColor(reader.ReadColor(), 1);
        }

        if (count > colors.Length)
        {
            for (; i < colors.Length; i++)
            {
                reader.ReadColor();
            }
        }
        else if (count < colors.Length)
        {
            for (; i < colors.Length; i++)
            {
                SetColor(Color.white, i);

            }
        }
    }

    public void GameUpdate()
    {
        Age += Time.deltaTime;
        for (int i = 0; i < behaviorList.Count; i++)
        {
            if (!behaviorList[i].GameUpdate(this))
            {
                behaviorList[i].Recycle();
                behaviorList.RemoveAt(i--);
            }

        }
    }

    public void ResolveShapeInstance()
    {
        for (int i = 0; i < behaviorList.Count; i++)
        {
            behaviorList[i].ResolveShapeInstance();
        }
    }

    public void Die()
    {
        Game.Intance.Kill(this);
    }

    public void MarkAsDying()
    {
        Game.Intance.MarkAsDying(this);
    }

    public void Recycle()
    {
        Age = 0;
        InstanceId += 1;
        for (int i = 0; i < behaviorList.Count; i++)
        {
            behaviorList[i].Recycle();
        }
        behaviorList.Clear();
        OriginFactory.Reclaim(this);
    }


    [System.Serializable]
    public struct ShapeInstance
    {
        public Shape Shape { get; private set; }

        int instanceIdOfSavenIndex;

        public ShapeInstance(Shape shape)
        {
            Shape = shape;
            instanceIdOfSavenIndex = shape.InstanceId;
        }

        public ShapeInstance(int saveIndex)
        {
            Shape = null;
            instanceIdOfSavenIndex = saveIndex;
        }

        public bool IsValid
        {
            get
            {
                return Shape && instanceIdOfSavenIndex == Shape.InstanceId;
            }
        }

        public void Resolve()
        {
            if (instanceIdOfSavenIndex >= 0)
            {
                Shape = Game.Intance.GetShape(instanceIdOfSavenIndex);
                instanceIdOfSavenIndex = Shape.InstanceId;
            }
        }
    }

    public static implicit operator ShapeInstance(Shape shape)
    {
        return new ShapeInstance(shape);
    }
}
