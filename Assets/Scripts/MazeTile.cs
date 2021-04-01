using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeTile : MonoBehaviour
{
    [SerializeField] public ExitPoint[] exitPoints = new ExitPoint[4];
    [SerializeField] public bool isValid = false;
    [SerializeField] public int tileType;

    public void CheckTileValidity()
    {
        bool tempValidity = true;
        foreach(ExitPoint exit in exitPoints)
        {
            tempValidity = tempValidity &&  exit.isSatisfied;
        }

        if (tempValidity) isValid = true;
        else isValid = false;
    }
}
