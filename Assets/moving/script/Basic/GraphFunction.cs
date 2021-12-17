using UnityEngine;

public delegate float GraphFunction(float x, float z, float t);

public delegate Vector3 Graph3DFunction(float u, float v, float t);

public enum GraphFunctionName {
    Sine,
    MultiSine,
    Ripple,
    Cylinder,
    Sphere,
    Torus
}