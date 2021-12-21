using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct TowerFloatRange
{
    [SerializeField]
    float min, max;

    public float Min => min;
    public float MAX => max;

    public float RandomValueInRange
    {
        get
        {
            return Random.Range(min, max);
        }
    }

    public TowerFloatRange(float value)
    {
        min = max = value;
    }

    public TowerFloatRange(float min, float max)
    {
        this.min = min;
        this.max = max;
    }
}
