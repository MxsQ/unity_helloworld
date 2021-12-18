using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpawnZone : GameLevelObject
{
    [SerializeField] SpawnConfiguration spawnConfig;

    public abstract Vector3 SpawnPoint { get; }

    [SerializeField, Range(0f, 50f)] float spawnSpeed;

    float spawnProgress;

    public virtual void SpawnShape()
    {
        int factoryIndex = Random.Range(0, spawnConfig.factories.Length);
        Shape shape = spawnConfig.factories[factoryIndex].GetRandom();
        shape.gameObject.layer = gameObject.layer;

        Transform t = shape.transform;
        t.localPosition = SpawnPoint;
        t.localRotation = Random.rotation;
        t.localScale = Vector3.one * spawnConfig.scale.RandomValueInRange;
        SetupColor(shape);

        float angularSpeed = spawnConfig.angularSpeed.RandomValueInRange;
        if (angularSpeed != 0f)
        {
            var rotation = shape.AddBehavior<RotationShapeBehavior>();
            rotation.AngularVelocity = Random.onUnitSphere * spawnConfig.angularSpeed.RandomValueInRange;
        }



        float speed = spawnConfig.speed.RandomValueInRange;
        if (speed != 0f)
        {
            var movement = shape.AddBehavior<MovementShapeBehavior>();
            movement.Velocity = GetDirectionVector(spawnConfig.movementDirection, t) * speed;
        }

        SetupOscillation(shape);

        Vector3 lifecycleDurations = spawnConfig.lifecycle.RandomDurations;
        int satelliteCount = spawnConfig.satellite.amount.RandomValueInRange;
        for (int i = 0; i < satelliteCount; i++)
        {
            CreateSatelliteFor(shape,
                spawnConfig.satellite.uniformLifecycles ? lifecycleDurations : spawnConfig.lifecycle.RandomDurations
                );
        }
        SetUpLifeCycle(shape, lifecycleDurations);
    }

    void SetupOscillation(Shape shape)
    {
        float amplitude = spawnConfig.oscillationAmplitude.RandomValueInRange;
        float frequcey = spawnConfig.oscillationFrequency.RandomValueInRange;
        if (amplitude == 0f || frequcey == 0f)
        {
            return;
        }
        var oscillation = shape.AddBehavior<OscillationShapeBehavior>();
        oscillation.Offset = GetDirectionVector(spawnConfig.oscillationDirection, shape.transform) * amplitude;
        oscillation.Frequency = frequcey;
    }

    Vector3 GetDirectionVector(SpawnConfiguration.MovementDirection direction, Transform t)
    {
        switch (spawnConfig.movementDirection)
        {
            case SpawnConfiguration.MovementDirection.Upward:
                return transform.up;

            case SpawnConfiguration.MovementDirection.Outward:
                return (t.localPosition - transform.position).normalized;

            case SpawnConfiguration.MovementDirection.Random:
                return Random.onUnitSphere;

            case SpawnConfiguration.MovementDirection.Forward:
                return transform.forward;
        }

        return new Vector3();
    }

    void CreateSatelliteFor(Shape focalShape, Vector3 lifecycleDurations)
    {
        int factoryIndex = Random.Range(0, spawnConfig.factories.Length);
        Shape shape = spawnConfig.factories[factoryIndex].GetRandom();
        shape.gameObject.layer = gameObject.layer;

        Transform t = shape.transform;
        t.localRotation = Random.rotation;
        t.localScale = focalShape.transform.localScale * spawnConfig.satellite.relativeScale.RandomValueInRange;
        t.localPosition = focalShape.transform.localPosition + Vector3.up;
        shape.AddBehavior<MovementShapeBehavior>().Velocity = Vector3.up;
        SetupColor(shape);
        shape.AddBehavior<SatelliteShapeBehavior>().Initialize(shape,
            focalShape,
            spawnConfig.satellite.orbitRadius.RandomValueInRange,
            spawnConfig.satellite.orbitRadius.RandomValueInRange
            );

        SetUpLifeCycle(shape, lifecycleDurations);
    }

    void SetupColor(Shape shape)
    {
        if (spawnConfig.uniformColor)
        {
            shape.SetColor(spawnConfig.color.RandomInrange);
        }
        else
        {
            for (int i = 0; i < shape.ColorCount; i++)
            {
                shape.SetColor(spawnConfig.color.RandomInrange, i);
            }
        }
    }

    void SetUpLifeCycle(Shape shape, Vector3 durations)
    {
        if (durations.x > 0f)
        {
            if (durations.y > 0 || durations.z > 0f)
            {
                shape.AddBehavior<LifecycleShapeBehavior>().Initialize(shape, durations.x, durations.y, durations.z);

            }
            else
            {
                shape.AddBehavior<GrowingShapeBehavior>().Initialize(shape, durations.x);
            }
        }
        else if (durations.z > 0f)
        {
            shape.AddBehavior<DyingShapeBehavior>().Initialize(shape, durations.z);
        }

    }

    public override void GameUpdate()
    {
        spawnProgress += Time.deltaTime * spawnSpeed;
        while (spawnProgress >= 1f)
        {
            spawnProgress -= 1f;
            SpawnShape();
        }
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(spawnProgress);
    }

    public override void Load(GameDataReader reader)
    {
        spawnProgress = reader.ReadFloat();
    }


    [System.Serializable]
    public struct SpawnConfiguration
    {
        public enum MovementDirection
        {
            Forward,
            Upward,
            Outward,
            Random,
        }

        public enum ShapeBehaviorType
        {
            Movement,
            Rotation,
            Oscillation,
            Satellite,
            Growing,
            Dying,
            FullLife,
        }

        [System.Serializable]
        public struct SatelliteConfiguration
        {
            public IntRange amount;

            [FloatRangeSlider(0.1f, 1f)]
            public FloatRange relativeScale;

            public FloatRange orbitRadius;

            public FloatRange orbitFrequency;

            public bool uniformLifecycles;
        }

        [System.Serializable]
        public struct LifecycleConfiguration
        {
            [FloatRangeSlider(0f, 2f)]
            public FloatRange growingDuration;

            [FloatRangeSlider(0, 100f)]
            public FloatRange adultDuration;

            [FloatRangeSlider(0f, 2f)]
            public FloatRange dyingDuration;

            public Vector3 RandomDurations
            {
                get
                {
                    return new Vector3(growingDuration.RandomValueInRange, adultDuration.RandomValueInRange, dyingDuration.RandomValueInRange);
                }
            }
        }

        public MovementDirection movementDirection;

        public FloatRange speed;

        public FloatRange angularSpeed;

        public FloatRange scale;

        public ColorRangeHSV color;

        public bool uniformColor;

        public ShapeFactory[] factories;

        public MovementDirection oscillationDirection;

        public FloatRange oscillationAmplitude;

        public FloatRange oscillationFrequency;

        public SatelliteConfiguration satellite;

        public LifecycleConfiguration lifecycle;
    }


}
