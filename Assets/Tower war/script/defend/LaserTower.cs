using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTower : Tower
{

    [SerializeField] Transform turret = default, laserBeam = default;

    [SerializeField, Range(1f, 100f)] float damagePerSecond = 10f;

    TargetPoint target;

    Vector3 laserBeamScale;

    public override Tower.TYPE TowerType => Tower.TYPE.Laser;

    private void Awake()
    {
        base.Awake();
        laserBeamScale = laserBeam.localScale;
    }

    public override void GameUpdate()
    {
        //Debug.Log("searching");
        if (TrackTarget(ref target) || AcquireTarget(out target))
        {
            Shoot();
        }
        else
        {
            laserBeam.localScale = Vector3.zero;
        }
    }

    void Shoot()
    {
        Vector3 point = target.Position;
        turret.LookAt(point);
        laserBeam.localRotation = turret.localRotation;

        float d = Vector3.Distance(turret.position, point);
        laserBeamScale.z = d;
        laserBeam.localScale = laserBeamScale;
        laserBeam.localPosition = turret.localPosition + 0.5f * d * laserBeam.forward;

        target.Enemy.ApplyDamage(damagePerSecond * Time.deltaTime);
    }

}
