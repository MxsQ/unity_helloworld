using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingObject : PersisableObject
{
    [SerializeField] Vector3 angularVelocity;

    private void FixedUpdate()
    {
        transform.Rotate(angularVelocity * Time.deltaTime);
    }
}