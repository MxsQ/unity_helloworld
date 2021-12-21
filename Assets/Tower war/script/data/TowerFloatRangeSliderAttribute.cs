using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerFloatRangeSliderAttribute : PropertyAttribute
{
    public float Min { get; private set; }
    public float Max { get; private set; }

    public TowerFloatRangeSliderAttribute(float min, float max)
    {
        if (max < min)
        {
            max = min;
        }
        Min = min;
        Max = max;
    }
}
