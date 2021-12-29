using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshDeformer : MonoBehaviour
{
    public float springForce = 20f;
    public float damping = 5f;

    Mesh deformingMesh;
    Vector3[] originalVertices, displaceVertices;
    Vector3[] vertexVelocities;
    float uniformScale = 1f;

    private void Start()
    {
        deformingMesh = GetComponent<MeshFilter>().mesh;
        originalVertices = deformingMesh.vertices;
        displaceVertices = new Vector3[originalVertices.Length];
        for (int i = 0; i < originalVertices.Length; i++)
        {
            displaceVertices[i] = originalVertices[i];
        }

        vertexVelocities = new Vector3[originalVertices.Length];
    }

    private void Update()
    {
        uniformScale = transform.localScale.x;
        for (int i = 0; i < displaceVertices.Length; i++)
        {
            UpdateVertex(i);
        }
        deformingMesh.vertices = displaceVertices;
        deformingMesh.RecalculateNormals();
    }

    void UpdateVertex(int i)
    {
        Vector3 velocity = vertexVelocities[i];
        Vector3 dispacement = displaceVertices[i] - originalVertices[i];
        dispacement *= uniformScale;
        velocity -= dispacement * springForce * Time.deltaTime;
        velocity *= 1f - damping * Time.deltaTime;
        vertexVelocities[i] = velocity;
        displaceVertices[i] += velocity * (Time.deltaTime / uniformScale);
    }

    public void AddDeforingForce(Vector3 point, float force)
    {
        //Debug.DrawLine(Camera.main.transform.position, point);
        point = transform.InverseTransformDirection(point);
        for (int i = 0; i < displaceVertices.Length; i++)
        {
            AddForceToVertex(i, point, force);
        }
    }

    void AddForceToVertex(int i, Vector3 point, float force)
    {
        Vector3 pointToVertex = displaceVertices[i] - point;
        pointToVertex *= uniformScale;
        float attenuateForce = force / (1f + pointToVertex.sqrMagnitude);
        float velocity = attenuateForce * Time.deltaTime;
        vertexVelocities[i] += pointToVertex.normalized * velocity;
    }
}
