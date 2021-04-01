using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    bool isPlayerSpawned = false;
    LevelGenerationRedux lGR;

    [SerializeField] GameObject player;

    void Awake()
    {
        lGR = FindObjectOfType<LevelGenerationRedux>();
    }

    void OnEnable()
    {
        lGR.SpawnPlayers += SpawnPlayers;
    }

    void OnDisable()
    {
        lGR.SpawnPlayers -= SpawnPlayers;
    }


    void SpawnPlayers()
    {
        Instantiate(player, transform.position, transform.rotation);
    }
}
