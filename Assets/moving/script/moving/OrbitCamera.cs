using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitCamera : MonoBehaviour
{
    [SerializeField]
    Transform focus = default;

    [SerializeField, Range(1f, 20f)]
    float distance = 10f;

    [SerializeField, Min(0f)]
    float focusRadius = 12f;

    [SerializeField]
    float focusCentering = 0.5f;

    [SerializeField]
    float rotationSpeed = 90f;

    [SerializeField, Range(-89f, 89f)]
    float minVerticalAngle = -30f, maxVerticalAngle = 60f;

    [SerializeField, Range(0f, 90f)]
    float alignSmoothRange = 45f;

    [SerializeField]
    LayerMask obstructionMask = -1;

    public int alignDelay = 0;

    Camera regularCamera;
    Vector3 focusPoint, previousFocusPoint;
    Vector2 orbitAngels = new Vector2(45f, 0f);

    Vector3 CameraHalfExtends
    {
        get
        {
            Vector3 halfExtends;
            halfExtends.y = regularCamera.nearClipPlane * Mathf.Tan(0.5f * Mathf.Deg2Rad * regularCamera.fieldOfView);
            halfExtends.x = halfExtends.y * regularCamera.aspect;
            halfExtends.z = 0f;
            return halfExtends;
        }
    }

    float lastManualRotationTime;

    private void Awake()
    {
        focusPoint = focus.position;
        transform.localRotation = Quaternion.Euler(orbitAngels);
        regularCamera = GetComponent<Camera>();
    }

    private void OnValidate()
    {
        if(maxVerticalAngle < minVerticalAngle)
        {
            maxVerticalAngle = minVerticalAngle;
        }
    }

    bool ManualRotation()
    {
        Vector2 input = new Vector2(
            Input.GetAxis("Vertical Camera"),
            Input.GetAxis("Horizontal Camera")
            );
        const float e = 0.001f;
        if(input.x < -e || input.x > e || input.y < -e || input.y > e)
        {
            orbitAngels += rotationSpeed * Time.unscaledDeltaTime * input;
            lastManualRotationTime = Time.unscaledTime;
            return true;
        }

        return false;
    }

    bool AutomaticRotation()
    {
        if (Time.unscaledTime - lastManualRotationTime < alignDelay)
        {
            return false;
        }

        Vector2 movment = new Vector2(focusPoint.x - previousFocusPoint.x, focusPoint.z - previousFocusPoint.z);
        float movementDeltaSqr = movment.sqrMagnitude;
        if (movementDeltaSqr < 0.000001f)
        {
            return false;
        }

        float headingAngle = GetAngle(movment / Mathf.Sqrt(movementDeltaSqr));
        float deltaAbs = Mathf.Abs(Mathf.DeltaAngle(orbitAngels.y, headingAngle));
        float rotationChange = rotationSpeed * Mathf.Min(Time.unscaledDeltaTime, movementDeltaSqr);
            
        if(deltaAbs < alignSmoothRange)
        {
            rotationChange *= deltaAbs / alignSmoothRange;
        }
        else if(180f - deltaAbs < alignSmoothRange)
        {
            rotationChange *= (180f - deltaAbs) / alignSmoothRange;
        }
        orbitAngels.y = Mathf.MoveTowardsAngle(orbitAngels.y, headingAngle, rotationChange);

        return true;
    }

    static float GetAngle (Vector2 direction)
    {
        float angle = Mathf.Acos(direction.y) * Mathf.Rad2Deg;
        return direction.x < 0f ? 360f - angle : angle;
    }

    private void LateUpdate()
    {
        UpdateFocusPoint();
        Quaternion lookRotation;
        if (ManualRotation() && AutomaticRotation())
        {
            ConstrainAngles();
            lookRotation = Quaternion.Euler(orbitAngels);
        }
        else
        {
            lookRotation = transform.localRotation;
        }
        Vector3 lookDirection = lookRotation * Vector3.forward;
        Vector3 lookPosition = focusPoint - lookDirection * distance;

        Vector3 rectOffset = lookDirection * regularCamera.nearClipPlane;
        Vector3 rectPosition = lookPosition + rectOffset;
        Vector3 castFrom = focus.position;
        Vector3 castLine = rectPosition - castFrom;
        float castDistance = castLine.magnitude;
        Vector3 castDirection = castLine / castDistance;

        if(Physics.BoxCast(castFrom, CameraHalfExtends, castDirection, out RaycastHit hit, lookRotation, castDistance, obstructionMask))
        {
            rectPosition = castFrom + castDirection * hit.distance;
            lookPosition = rectPosition - rectOffset;
        }

        transform.SetPositionAndRotation(lookPosition, lookRotation);
    }

    void UpdateFocusPoint()
    {   
        previousFocusPoint = focusPoint;
        Vector3 targetPoint = focus.position;

        float distanfce = Vector3.Distance(targetPoint, focusPoint);
        float t = 1f;
        if(distance > 0.01f && focusCentering > 0f)
        {
            t = Mathf.Pow(1f - focusCentering, Time.unscaledDeltaTime);
        }
        if(distance > focusRadius)
        {
            t = Mathf.Min(t, focusRadius / distance);
        }
        focusPoint = Vector3.Lerp(targetPoint, focusPoint, t);
    }

    void ConstrainAngles()
    {
        orbitAngels.x = Mathf.Clamp(orbitAngels.x, minVerticalAngle, maxVerticalAngle);
        if(orbitAngels.y < 0f)
        {
            orbitAngels.y += 360f;
        }
        else if(orbitAngels.y >= 360f)
        {
            orbitAngels.y -= 360;
        }
    }
}
