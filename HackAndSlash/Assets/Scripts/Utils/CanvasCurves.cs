using System;
using UnityEngine;

public class CanvasCurves : MonoBehaviour
{
    public static CanvasCurves Instance;
    
    [Header("Canvas Curves")]
    public AnimationCurve fadeInCurve;
    public AnimationCurve fadeOutCurve;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
}
