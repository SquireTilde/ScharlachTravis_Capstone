using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshBaker : MonoBehaviour
{
    LevelGenerationRedux lGR;


    // Start is called before the first frame update
    void Awake()
    {
        lGR = GetComponent<LevelGenerationRedux>();
    }

    void OnEnable()
    {
        lGR.BakeNavMesh += BuildNavMeshes;
    }

    void OnDisable()
    {
        lGR.BakeNavMesh -= BuildNavMeshes;
    }

    void BuildNavMeshes()
    {
        NavMeshSurface surface = gameObject.GetComponent<NavMeshSurface>();
        surface.BuildNavMesh();
    }
}
