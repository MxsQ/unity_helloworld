using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MortarTower : Tower
{
    [SerializeField, Range(0.5f, 2f)] float shotsPerSecond = 1f;

    [SerializeField, Range(0.5f, 3f)] float shellBlastRadius = 1f;

    [SerializeField, Range(1f, 100f)] float shellDamage = 10f;

    [SerializeField] Transform mortar = default;

    public override TYPE TowerType => TYPE.Mortar;

    float launchSpeed;

    float lauchProgress;

    void Awake()
    {
        base.Awake();
        OnValidate();
    }

    void OnValidate()
    {
        float x = targetingRange + 0.25001f;  // for get the valid speed depend on scale range of enemy.
        float y = -mortar.position.y;
        launchSpeed = Mathf.Sqrt(9.81f * Mathf.Sqrt(x * x + y * y));
    }

    public override void GameUpdate()
    {
        lauchProgress += shotsPerSecond * Time.deltaTime;
        while (lauchProgress >= 1f)
        {
            if (AcquireTarget(out TargetPoint target))
            {
                Launch(target);
                lauchProgress -= 1f;
            }
            else
            {
                lauchProgress = 0.999f;
            }
        }
    }

    public void Launch(TargetPoint target)
    {
        Vector3 launchPoint = mortar.position;
        Vector3 targetPoint = target.Position;
        targetPoint.y = 0f;

        Vector2 dir;
        dir.x = targetPoint.x - launchPoint.x;
        dir.y = targetPoint.z - launchPoint.z;
        float x = dir.magnitude;
        float y = -launchPoint.y;
        dir /= -x;

        float g = 9.81f;
        float s = launchSpeed;
        float s2 = s * s;

        float r = s2 * s2 - g * (g * x * x + 2f * y * s2);
        Debug.Assert(r > -0f, "Launch velocaity insufficient for range");
        float tanTheta = (s2 + Mathf.Sqrt(r)) / (g * x);
        float cosTheta = Mathf.Cos(Mathf.Atan(tanTheta));
        float sinTheta = cosTheta * tanTheta;

        mortar.localRotation = Quaternion.LookRotation(new Vector3(dir.x, tanTheta, dir.y));

        TowerGame.SpawnShell().Initialize(
            launchPoint, targetPoint,
            new Vector3(-s * cosTheta * dir.x, s * sinTheta, -s * cosTheta * dir.y),
            shellBlastRadius, shellDamage);
    }

}
