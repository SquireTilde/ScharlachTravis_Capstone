using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GroundDetector : MonoBehaviour
{
    public event Action GroundDetected = delegate { };
    public event Action GroundVanished = delegate { };

    void OnTriggerEnter()
    {
        GroundDetected?.Invoke();
    }

    void OnTriggerExit()
    {
        GroundVanished.Invoke();
    }
}
