using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitPoint : MonoBehaviour
{
    [SerializeField] public bool isSatisfied = false;
    [SerializeField] public bool isExposed = true;
    int failures = 0;
    [SerializeField] int failureTolerance = 5;

    bool isTriggered;
    Collider other;

    void OnTriggerEnter(Collider other)
    {
        isTriggered = true;
        this.other = other;

        isExposed = false;
        if (other.gameObject.GetComponent<ExitPoint>())
        {
            isSatisfied = true;
        }

    }

    void OnTriggerExit(Collider other)
    {
        isExposed = true;
        isSatisfied = false;
    }

    void Update()
    {
        if(isTriggered && !other)
        {
            isExposed = true;
            isSatisfied = false;
        }
    }
}
