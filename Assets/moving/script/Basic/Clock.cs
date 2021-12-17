using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock : MonoBehaviour {
    public Transform hourTransform;
    public Transform minutesTransform;
    public Transform SecondTransform;
    public bool continuous;

    const float degreesPerHours = 30f;
    const float degressPerMinute = 6;
    const float degressPerSecond = 6f;

    void Update(){
        if (continuous) {
            updateContinuous();
        }
        else {
            updateDistance();
        }
    }

    void updateContinuous() {
        TimeSpan time = DateTime.Now.TimeOfDay;

        hourTransform.localRotation = Quaternion.Euler(0f, (float)(time.TotalHours * degreesPerHours), 0f);
        minutesTransform.localRotation = Quaternion.Euler(0f, (float)(time.TotalMinutes * degressPerMinute), 0f);
        SecondTransform.localRotation = Quaternion.Euler(0f, (float)(time.TotalSeconds * degressPerSecond), 0f);
    }

    void updateDistance() {
        DateTime time = DateTime.Now;
        hourTransform.localRotation = Quaternion.Euler(0f, time.Hour * degreesPerHours, 0f);
        minutesTransform.localRotation = Quaternion.Euler(0f, time.Minute * degressPerMinute, 0f);
        SecondTransform.localRotation = Quaternion.Euler(0f, time.Second * degressPerSecond, 0f);
    }
}    