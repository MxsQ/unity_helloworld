using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPoint : MonoBehaviour
{
    static Collider[] buffer = new Collider[100];

    static int enemyLayerMask = -1;

    public static int BufferdCount { get; private set; }

    public static TargetPoint RandomBuffered => GetBuffered(Random.Range(0, BufferdCount));

    public Enemy Enemy { get; private set; }

    public Vector3 Position => transform.position;

    private void Awake()
    {
        Enemy = transform.root.GetComponent<Enemy>();
        Debug.Assert(Enemy != null, "Target point without Enemy root", this);
        Debug.Assert(GetComponent<SphereCollider>() != null, "Target point without sphere collider", this);
        Debug.Assert(gameObject.layer == 11, "Target point on wrong layer");

        if (enemyLayerMask == -1)
        {
            enemyLayerMask = 1 << LayerMask.NameToLayer("Enemy");
        }
    }

    public static bool FillBuffer(Vector3 position, float range)
    {
        Vector3 top = position;
        top.y += 3f;
        BufferdCount = Physics.OverlapCapsuleNonAlloc(position, top, range, buffer, enemyLayerMask);
        return BufferdCount > 0;
    }

    public static TargetPoint GetBuffered(int index)
    {
        var target = buffer[index].GetComponent<TargetPoint>();
        Debug.Assert(target != null, "Targeted non-enemy!", buffer[index]);
        return target;
    }
}
