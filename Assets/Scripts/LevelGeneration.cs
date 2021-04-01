using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGeneration : MonoBehaviour
{

    [SerializeField] GameObject[] rooms;
    [SerializeField] GameObject startRoom;
    [SerializeField] Row[] leGrid;

    List<GameObject> currentRooms = new List<GameObject>();
    List<bool> roomValidity = new List<bool>();
    [SerializeField] int attempts = 0;
    [SerializeField] int attemptThreshold = 1000; 

    float timeUntilNextStep = 0;
    float timeBetweenSteps = 0.1f;


    bool levelGenIsDone = false;

    void Start()
    {
        //for each Row in leGrid
        for(int i = 0; i < leGrid.Length; i++ )
        {   
            //for each column in the row
            for(int j = 0; j < leGrid[i].rowData.Length; j++)
            {
                if (i == 3 && j == 4)
                {
                    Instantiate(startRoom, leGrid[i].rowData[j], Quaternion.Euler(0, 180, 0), transform);
                }
                else if (i == 3 && j == 3)
                {
                    Instantiate(startRoom, leGrid[i].rowData[j], Quaternion.Euler(0, -90, 0), transform);
                }
                else if (i == 4 && j == 4)
                {
                    Instantiate(startRoom, leGrid[i].rowData[j], Quaternion.Euler(0, 90, 0), transform);
                }
                else if (i == 4 && j == 3)
                {
                    Instantiate(startRoom, leGrid[i].rowData[j], Quaternion.Euler(0, 0, 0), transform);
                }
                else
                {
                    int rand = Random.Range(0, rooms.Length - 2);
                    int rMultiplier = Random.Range(0, 4);
                    Vector3 rotation = new Vector3(0, 90 * rMultiplier, 0);
                    GameObject newRoom = (GameObject)Instantiate(rooms[rand], leGrid[i].rowData[j], Quaternion.Euler(rotation), transform);
                    currentRooms.Add(newRoom);
                    roomValidity.Add(false);//always assume that a room is invalid when it is created
                }
            }
        }

    }


    // Update is called once per frame
    void Update()
    {
        if (!levelGenIsDone) LevelGenTimer();
    }

    void LevelGenTimer()
    {
        if (timeUntilNextStep <= 0)
        {
            Step();
        }
        else timeUntilNextStep -= Time.deltaTime;
    }

    void Step()
    {
        // for each room in currentRooms
        for (int r = 0; r < currentRooms.Count; r++)
        {
            GameObject room = currentRooms[r];
            MazeTile mt = room.GetComponent<MazeTile>();
            room.transform.Rotate(0, 60, 0);
            QuickTimer(.5f);
            room.transform.Rotate(0, -60, 0);
            mt.CheckTileValidity();
            if (!mt.isValid)
            {
                //rotate the room to try to make it valid
                for (int i = 0; i < 4; i++)
                {
                    room.transform.Rotate(0, 90, 0);

                    //Stall for a moment so that the exit points can detect things
                    QuickTimer(.1f);

                    mt.CheckTileValidity();
                    if (mt.isValid) break;

                }
                //if the room still isn't valid, destroy it and try again.
                if (!mt.isValid)
                {
                    int rand;
                    do
                    {
                        rand = Random.Range(0, rooms.Length - 2);
                    } while (rand == mt.tileType);

                    int rMultiplier = Random.Range(0, 4);
                    Vector3 rotation = new Vector3(0, 90 * rMultiplier, 0);
                    GameObject newRoom = Instantiate(rooms[rand], room.transform.position, Quaternion.Euler(rotation), transform);
                    currentRooms[r] = newRoom;
                    Destroy(room);
                }

                roomValidity[r] = mt.isValid;
            }

        }
        if (attempts >= attemptThreshold) levelGenIsDone = true;
        attempts++;
        timeUntilNextStep = timeBetweenSteps;
    }

    void QuickTimer(float time)
    {
        float timer = time;
        while(timer <= 0)
        {
            timer -= Time.deltaTime;
        }
    }
}
