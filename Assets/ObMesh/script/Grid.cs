using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Grid : MonoBehaviour
{
    public int xSize, ySize;

    private Vector3[] vertices;

    private Mesh mesh;

    private void Awake()
    {
        Generate();
    }


    private void Generate()
    {
        WaitForSeconds wait = new WaitForSeconds(0.05f);

        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Procedural Grid";

        vertices = new Vector3[(xSize + 1) * (ySize + 1)];
        Vector2[] uv = new Vector2[vertices.Length];
        Vector4[] tangents = new Vector4[vertices.Length];
        Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);
        for (int i = 0, y = 0; y <= ySize; y++)
        {
            for (int x = 0; x <= xSize; x++, i++)
            {
                vertices[i] = new Vector3(x, y);
                uv[i] = new Vector2((float)x / xSize, (float)y / ySize);
                tangents[i] = tangent;
            }
        }
        mesh.vertices = vertices;
        mesh.uv = uv;
        //mesh.tangents = tangents;

        int[] triangles = new int[xSize * ySize * 6];
        for (int t1 = 0, v1 = 0, y = 0; y < ySize; y++, v1++)
        {
            for (int x = 0; x < xSize; x++, t1 += 6, v1++)
            {
                triangles[t1] = v1;
                triangles[t1 + 3] = triangles[t1 + 2] = v1 + 1;
                triangles[t1 + 4] = triangles[t1 + 1] = v1 + xSize + 1;
                triangles[t1 + 5] = v1 + xSize + 2;
            }
        }

        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    private void OnDrawGizmos()
    {
        //if (vertices == null)
        //{
        //    return;
        //}
        //Gizmos.color = Color.black;
        //for (int i = 0; i < vertices.Length; i++)
        //{
        //    Gizmos.DrawSphere(vertices[i], 0.1f);
        //}
    }
}
