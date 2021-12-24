using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : WarEntity
{

    Vector3 launchPoint, targetPoint, launchVelocity;

    float age, blastRadius, damage;

    public void Initialize(Vector3 launchPoint, Vector3 targetPoint, Vector3 launchVelocity,
        float blastRaidus, float damage)
    {
        this.launchPoint = launchPoint;
        this.targetPoint = targetPoint;
        this.launchVelocity = launchVelocity;
        this.blastRadius = blastRaidus;
        this.damage = damage;
    }

    public override bool GameUpdate()
    {
        age += Time.deltaTime;
        Vector3 p = launchPoint + launchVelocity * age;
        p.y -= 0.5f * 9.81f * age * age;
        transform.localPosition = p;

        if (p.y <= 0f)
        {
            TowerGame.SpawnExplosion().Initialize(targetPoint, blastRadius, damage);
            OriginFactory.Reclaim(this);
            return false;
        }

        TowerGame.SpawnExplosion().Initialize(p, 0.1f);
        return true;
    }
}
