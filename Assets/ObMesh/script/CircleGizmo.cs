using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleGizmo : MonoBehaviour
{
    public int resolution = 10;

    private void OnDrawGizmosSelected()
    {
        float step = 2f / resolution;
        for (int i = 0; i <= resolution; i++)
        {
            ShowPoin(i * step - 1f, -1f);
            ShowPoin(i * step - 1f, 1f);
        }

        for (int i = 1; i < resolution; i++)
        {
            ShowPoin(-1f, i * step - 1f);
            ShowPoin(1f, i * step - 1f);
        }
    }

    private void ShowPoin(float x, float y)
    {
        Vector2 square = new Vector2(x, y);
        Vector2 circle = square.normalized;
        circle.x = square.x * Mathf.Sqrt(1f - square.y * square.y * 0.5f);
        circle.y = square.y * Mathf.Sqrt(1f - square.x * square.x * 0.5f);

        Gizmos.color = Color.black;
        Gizmos.DrawSphere(square, 0.025f);

        Gizmos.color = Color.white;
        Gizmos.DrawSphere(circle, 0.025f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(square, circle);

        Gizmos.color = Color.gray;
        Gizmos.DrawLine(circle, Vector2.zero);
    }
}
