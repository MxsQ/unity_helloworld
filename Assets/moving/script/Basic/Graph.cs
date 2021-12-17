using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour
{
    public Transform parent;
    public Transform pointPrefab;
    [Range(10, 100)] public int resolution = 10;
    public GraphFunctionName function;

    Transform[] points;
    //GraphFunction[] funcs = {
    //    Sine2DFunction, MultiSine2DFunction, Ripple
    //};
    Graph3DFunction[] funcsss = {
          Sine2DFunction, MultiSine2DFunction, Ripple, Cylinder, Sphere, Torus
    };

    void Awake()
    {
        points = new Transform[resolution * resolution];

        float step = 2f / resolution;

        Vector3 scale = Vector3.one * step;
        Vector3 position = new Vector3();
        position.z = 0f;

        for (int i = 0; i < points.Length; i++) {
            Transform point = Instantiate(pointPrefab);
            point.localScale = scale;
            point.SetParent(parent, false);
            points[i] = point;
        }
    }

    void Update()
    {
        float step = 2f / resolution;
        
        for (int i = 0, z=0; z < resolution; z++)
        {
            float v = (z + 0.5f) * step - 1f;
            for (int x = 0; x < resolution; x++, i++) {
                float u = (x + 0.5f) * step - 1f;
                points[i].localPosition = funcsss[(int)function](u, v, Time.time);
            }
        }
    }


    static float SineFunciton(float x, float t)
    {
        return Mathf.Sin(Mathf.PI * (x + t));
    }

    static float MultiSineFunction(float x, float t)
    {
        float y = Mathf.Sin(Mathf.PI * (x + t));
        y += Mathf.Sin(2f * Mathf.PI * (x + 2f * t)) / 2f;
        y *= 2f / 3f;
        return y;
    }

    static Vector3 Sine2DFunction(float x, float z, float t)
    {
        Vector3 p;
        p.x = x;
        p.y = Mathf.Sin(Mathf.PI * (x + t));
        p.z = z;
        return p;
    }

    static Vector3 MultiSine2DFunction(float x, float z, float t)
    {
        Vector3 p;
        float y = 4f * Mathf.Sin(Mathf.PI * (x + z + t + 0.5f));
        y += Mathf.Sin(Mathf.PI * (x + t));
        y += Mathf.Sin(2f * Mathf.PI * (z + 2f * t) * 0.5f);
        y *= 1f / 5.5f;

        p.x = x;
        p.y = y;
        p.z = z;
        return p;
    }

    static Vector3 Ripple(float x, float z, float t) {
        Vector3 p;

        float d = Mathf.Sqrt(x * x + z * z);
        float y = Mathf.Sin(Mathf.PI * (4f * d - t));
        y /= 1f + 10f * d;

        p.x = x;
        p.y = y;
        p.z = z;
        return p;
    }

    static Vector3 Cylinder(float u, float v, float t) {
        Vector3 p;
        float r = 0.8f + Mathf.Sin(Mathf.PI * (6f * u + 2f * v + t)) * 0.2f;
        p.x = r * Mathf.Sin(Mathf.PI * u);
        p.y = v;
        p.z = r * Mathf.Cos(Mathf.PI * u);
        return p;
    }

    static Vector3 Sphere(float u, float v, float t) {
        Vector3 p;
        float r = 0.8f + Mathf.Sin(Mathf.PI * (6f * u + t)) * 0.1f;
        r += Mathf.Sin(Mathf.PI * (4f * v + t)) * 0.1f;
        float s = r * Mathf.Cos(Mathf.PI * 0.5f * v);

        p.x = s * Mathf.Sin(Mathf.PI * u);
        p.y = r * Mathf.Sin(Mathf.PI * 0.5f * v);
        p.z = s * Mathf.Cos(Mathf.PI * u);
        return p;
    }

    static Vector3 Torus(float u, float v, float t)
    {
        Vector3 p;

        //float r1 = 1f;
        //float r2 = 0.5f;
        float r1 = 0.65f + Mathf.Sin(Mathf.PI * (6f * u + t)) * 0.1f;
        float r2 = 0.2f + Mathf.Sin(Mathf.PI * (4f * v + t)) * 0.1f;
        float s = r2 * Mathf.Cos(Mathf.PI * v) + r1;

        p.x = s * Mathf.Sin(Mathf.PI * u);
        p.y = r2 * Mathf.Sin(Mathf.PI * v);
        p.z = s * Mathf.Cos(Mathf.PI * u);
        return p;
    }
}
