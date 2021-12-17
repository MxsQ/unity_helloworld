using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(FPSCounter))]
public class FPSDisplay : MonoBehaviour
{
    public Text fpsLabel;
    FPSCounter fpsCpunter;

    private void Awake()
    {
        fpsCpunter = GetComponent<FPSCounter>();
    }

    private void Update()
    {
        fpsLabel.text = Mathf.Clamp(fpsCpunter.AverageFPS, 0, 99).ToString();
    }

}
