using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Fractal : MonoBehaviour{
    public Mesh mesh;
    public Material material;
    public float childScale;
    public int maxDepth = 4;
    public float spawnProbability;
    public float maxRotationSeed;
    public float maxTvist;

    int depth;
    private Material[] materials;
    private float rotateSeed;

    private static Vector3[] childDirections = {
        Vector3.up,
        Vector3.right,
        Vector3.left,
        Vector3.forward,
        Vector3.back,
    };
    private static Quaternion[] childOrientation = {
        Quaternion.identity,
        Quaternion.Euler(0f, 0f, -90f),
        Quaternion.Euler(0f, 0f, 90f),
        Quaternion.Euler(90f, 0f, 0f),
        Quaternion.Euler(-90f, 0f, 0f)
    };

    private void Update()
    {
        transform.Rotate(0f, rotateSeed * Time.deltaTime, 0f);
    }

    private void Start()
    {
        rotateSeed = Random.Range(-maxRotationSeed, maxRotationSeed);
        transform.Rotate(Random.Range(-maxTvist, maxTvist), 0f, 0f);
        if (materials == null) {
            InitializeMaterials();
        }
        gameObject.AddComponent<MeshFilter>().mesh = mesh;
        gameObject.AddComponent<MeshRenderer>().material = materials[depth];
       

        if (depth < maxDepth) {
            StartCoroutine(CreateChildren());
        }
    }

    private void InitializeMaterials()
    {
        materials = new Material[maxDepth + 1];
        for (int i = 0; i <= maxDepth; i++) {
            float t = i / (maxDepth - 1f);
            t *= t;
            materials[i] = new Material(material);
            materials[i].color = Color.Lerp(Color.white, Color.yellow, t);
        }
        materials[maxDepth].color = Color.magenta;
    }

    private void Initialize(Fractal parent, Vector3 direction, Quaternion orientation)
    {
        maxTvist = parent.maxTvist;
        maxRotationSeed = parent.maxRotationSeed;
        spawnProbability = parent.spawnProbability;
        mesh = parent.mesh;
        material = parent.material;
        maxDepth = parent.maxDepth;
        depth = parent.depth + 1;
        childScale = parent.childScale;

        transform.parent = parent.transform;
        transform.localScale = Vector3.one * childScale;
        transform.localPosition = direction * (0.5f + 0.5f * childScale);
        transform.localRotation = orientation;
    }

    private IEnumerator CreateChildren()
    {
        for (int i = 0; i < childDirections.Length; i++)
        {
            if (Random.value < spawnProbability) {
                yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
                new GameObject("Fractal Child").AddComponent<Fractal>().Initialize(this, childDirections[i], childOrientation[i]);
            }
            
        }
    }
      
}
