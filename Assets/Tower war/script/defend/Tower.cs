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

    //public override void GameUpdate()
    //{
    //    //Debug.Log("searching");
    //    if (TrackTarget() || AcquireTarget())
    //    {
    //        Shoot();
    //    }
    //    else
    //    {
    //        laserBeam.localScale = Vector3.zero;
    //    }
    //}

    //void Shoot()
    //{
    //    Vector3 point = target.Position;
    //    turret.LookAt(point);
    //    laserBeam.localRotation = turret.localRotation;

    //    float d = Vector3.Distance(turret.position, point);
    //    laserBeamScale.z = d;
    //    laserBeam.localScale = laserBeamScale;
    //    laserBeam.localPosition = turret.localPosition + 0.5f * d * laserBeam.forward;

    //    target.Enemy.ApplyDamage(damagePerSecond * Time.deltaTime);
    //}

    protected bool AcquireTarget(out TargetPoint target)
    {
        //Vector3 a = transform.localPosition;
        //Vector3 b = a;
        //b.y += 3f;
        //int hits = Physics.OverlapCapsuleNonAlloc(a, b, targetingRange, targetBuffer, enemyLayerMask);
        //if (hits > 0)
        //{
        //    target = targetBuffer[Random.Range(0, hits)].GetComponent<TargetPoint>();
        //    Debug.Assert(target != null, "Targeted non-enemy!", targetBuffer[0]);
        //    //Debug.Log("yes");
        //    return true;
        //}
        //Debug.Log("no!!");

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
        if (target == null)
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


