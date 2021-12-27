using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tower : GameTileContent
{
    static Collider[] targetBuffer = new Collider[100];

    [SerializeField, Range(1.5f, 10.5f)] protected float targetingRange = 1.5f;

    //TargetPoint target;

    static int enemyLayerMask = -1;

    public abstract TYPE TowerType { get; }

    public void Awake()
    {
        if (enemyLayerMask == -1)
        {
            enemyLayerMask = 1 << LayerMask.NameToLayer("Enemy");
        }
    }

    protected bool AcquireTarget(out TargetPoint target)
    {
        if (TargetPoint.FillBuffer(transform.localPosition, targetingRange))
        {
            target = TargetPoint.RandomBuffered;
            return true;
        }

        target = null;
        return false;
    }

    protected bool TrackTarget(ref TargetPoint target)
    {
        if (target == null || !target.Enemy.IsValidTarget)
        {
            return false;
        }
        Vector3 a = transform.localPosition;
        Vector3 b = target.Position;
        float x = a.x - b.x;
        float z = a.z - b.z;
        float r = targetingRange + 0.125f * target.Enemy.Scale;

        // the const number be add is radius of collider
        if (x * x + z * z > r * r)
        {
            target = null;
            return false;
        }

        return true;
    }

    private void OnDrawGizmosSelected()
    {
        //if (target != null)
        //{
        Gizmos.color = Color.yellow;
        Vector3 position = transform.localPosition;
        position.y += 0.01f;
        Gizmos.DrawWireSphere(position, targetingRange);
        //Gizmos.DrawLine(position, target.Position);
        //}
    }


    public enum TYPE
    {
        Laser, Mortar
    }
}


