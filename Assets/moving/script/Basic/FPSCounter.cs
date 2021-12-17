using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
    public int frameRange;
    int[] fpsBuffer;
    int fpsBufferIndex;

    public int AverageFPS { get; private set; }

    private void Update()
    {
        if (fpsBuffer == null || fpsBuffer.Length != frameRange) {
            InitalizeBuffer();
        }
        UpdateBuffer();
        CalculateFPS();
        AverageFPS = (int)(1f / Time.unscaledDeltaTime);
    }

    private void CalculateFPS()
    {
        int sum = 0;
        for(int i=0; i<frameRange; i++)
        {
            sum += fpsBuffer[i];
        }
        AverageFPS = sum / frameRange;
    }

    private void UpdateBuffer()
    {
        fpsBuffer[fpsBufferIndex++] = (int)(1f / Time.unscaledDeltaTime);
        if (fpsBufferIndex >= frameRange)
        {
            fpsBufferIndex = 0;
        }
    }

    private void InitalizeBuffer() {
        if(frameRange <= 0)
        {
            frameRange = 1;
        }
        fpsBuffer = new int[frameRange];
        fpsBufferIndex = 0;
    }
}
