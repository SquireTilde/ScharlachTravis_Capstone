using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LevelGenerationRedux : MonoBehaviour
{
    public event Action SpawnPlayers = delegate { };
    public event Action BakeNavMesh = delegate { };

    [SerializeField] GameObject[] tileList;
    [SerializeField] GameObject[] startTiles;
    [SerializeField] Column[] leGrid;
    GameObject[,] tileGrid = new GameObject[8,8];
    public List<GameObject> currentTiles = new List<GameObject>();

    float levelGenTimer = 0;
    [SerializeField] float timerReload = 2f;

    [SerializeField] int triesWithoutSuccess = 0;
    [SerializeField] int failThreshold = 50;

    [SerializeField] int triesWithoutTileFound = 0;
    [SerializeField] int findTileFailThreshold = 50;

    public bool isLevelGenDone = false;

    int gridX;
    int gridY;

    struct CoordinatePair
    {
        public CoordinatePair(int X, int Y)
        {
            x= X;
            y= Y;
        }

        public int x;
        public int y;

        public override string ToString() => $"({x},{y})";
    }

    CoordinatePair gridMax = new CoordinatePair(7, 7);
    CoordinatePair gridMin = new CoordinatePair(0, 0);

    // Start is called before the first frame update
    void Start()
    {
        InstantiateStartTile(3, 3, -90, 0);
        InstantiateStartTile(4, 3, 180, 1);
        InstantiateStartTile(4, 4, 90, 2);
        InstantiateStartTile(3, 4, 0, 3);
    }

    // Update is called once per frame
    void Update()
    {
        if(!isLevelGenDone) Timer();
    }

    void Timer()
    {
        if (levelGenTimer <= 0)
        {
            if (triesWithoutSuccess > failThreshold)
            {
                triesWithoutSuccess = 0;
                StartCoroutine(FillHoles());
            }
            else
            {
                CreateNewTile();
                levelGenTimer = timerReload;
            }
        }
        else
        {
            levelGenTimer -= Time.deltaTime;
        }
    }


    void InstantiateStartTile(int x, int y, int rotation, int tile)
    {
        GameObject newTile = Instantiate(startTiles[tile], leGrid[x].columnData[y], Quaternion.Euler(0, rotation, 0), transform);
        tileGrid[x, y] = newTile;
        currentTiles.Add(newTile);
        CoordinatePair check = FindTileCoordinates(newTile);
        Debug.Log("New Tile Created. Tile Coordinates: " + check.ToString());

    }

    CoordinatePair FindTileCoordinates(GameObject tile)
    {
        int w = tileGrid.GetLength(0);
        int h = tileGrid.GetLength(1);

        for(int i = 0; i < w; i++)
        {
            for(int j = 0; j < h; j++)
            {
                if (tileGrid[i, j] != null)
                {
                    if (tileGrid[i, j].Equals(tile))
                    {
                        return new CoordinatePair(i, j); // found it
                    }
                }
            }
        }
        return new CoordinatePair(-1, -1); //fail to find
    }

    void CreateNewTile()
    {
        GameObject tile;
        MazeTile mt;
        bool tileFound = false;
        do
        {
            int randomTile = UnityEngine.Random.Range(0, currentTiles.Count);
            tile = currentTiles[randomTile];
            mt = tile.GetComponent<MazeTile>();
            mt.CheckTileValidity();
            if (!mt.isValid) tileFound = true;
            else triesWithoutTileFound++;
            if(triesWithoutTileFound >= findTileFailThreshold)
            {
                LevelGenComplete();
                return;
            }
        } while (!tileFound);
        triesWithoutTileFound = 0;

        int randomExit = UnityEngine.Random.Range(0, mt.exitPoints.Length);
        ExitPoint exit = mt.exitPoints[randomExit];
        if (!exit.isSatisfied && exit.isExposed)
        {
            CoordinatePair coords = FindTileCoordinates(tile);
            Vector3 exitDirection = exit.gameObject.transform.forward;
            CoordinatePair increment = new CoordinatePair((int)exitDirection.x, (int)exitDirection.z);
            Debug.Log($"{increment.x}, {increment.y}");
            CoordinatePair newCoords = new CoordinatePair(coords.x + increment.x, coords.y + increment.y);
            if((newCoords.x <= gridMax.x) && (newCoords.x >= gridMin.x) && (newCoords.y <= gridMax.y) && (newCoords.y >= gridMin.y))
            {
                StartCoroutine(InstantiateRandomTile(newCoords.x, newCoords.y));

                if(currentTiles.Count >= tileGrid.Length)
                {
                    LevelGenComplete();
                    
                }
            }

            

        }
    }

    IEnumerator InstantiateRandomTile(int x, int y)
    {
        int rand = UnityEngine.Random.Range(0, tileList.Length - 2); //cannot randomly create a deadend or a start tile.
        int randRotation = UnityEngine.Random.Range(0, 4) * 90;
        GameObject newTile = Instantiate(tileList[rand], leGrid[x].columnData[y], Quaternion.Euler(0, randRotation, 0), transform);
        MazeTile newMT = newTile.GetComponent<MazeTile>();
        bool isTileOK = false;
        yield return new WaitForSeconds(.001f);
        foreach(ExitPoint exit in newMT.exitPoints)
        {
            Debug.Log($"{exit.name} Satisfied?: {exit.isSatisfied}");
            Debug.Log($"{exit.name} Exposed?: {exit.isExposed}");
            isTileOK = isTileOK || (exit.isSatisfied);
            bool check = (!exit.isSatisfied && !exit.isExposed);
            if (check)
            {
                isTileOK = false;
                break;
            }
        }
        Debug.Log($"Tile OK?: {isTileOK}");
        if (isTileOK)
        {
            tileGrid[x, y] = newTile;
            currentTiles.Add(newTile);
            CoordinatePair check = FindTileCoordinates(newTile);
            Debug.Log("New Tile Created. Tile Coordinates: " + check.ToString());
            triesWithoutSuccess = 0;
        }
        else
        {
            Debug.Log($"{newTile.name} not in valid placement. Destroying Tile....");
            Destroy(newTile);
            triesWithoutSuccess++;
        }

    }

    IEnumerator FillHoles()
    {
        int w = tileGrid.GetLength(0);
        int h = tileGrid.GetLength(1);

        for (int i = 0; i < w; i++)
        {
            for (int j = 0; j < h; j++)
            {
                if(tileGrid[i,j] == null)
                {
                    Debug.Log("Empty Tile Detected. Filling Hole.");
                    int rand = UnityEngine.Random.Range(0, tileList.Length - 2);
                    int randRotation = UnityEngine.Random.Range(0, 4) * 90;
                    GameObject newTile = Instantiate(tileList[rand], leGrid[i].columnData[j], Quaternion.Euler(0, randRotation, 0), transform);
                    MazeTile newMT = newTile.GetComponent<MazeTile>();
                    for (int k = 0; k < 4; k++)
                    {
                        bool isTileOK = false;
                        yield return new WaitForSeconds(.001f);
                        foreach (ExitPoint exit in newMT.exitPoints)
                        {
                            Debug.Log($"{exit.name} Satisfied?: {exit.isSatisfied}");
                            Debug.Log($"{exit.name} Exposed?: {exit.isExposed}");
                            isTileOK = isTileOK || (exit.isSatisfied);
                        }
                        if (isTileOK) break;
                        else
                        {
                            newTile.transform.Rotate(0, 90, 0);
                        }
                    }
                    tileGrid[i, j] = newTile;
                    currentTiles.Add(newTile);
                    CoordinatePair check = FindTileCoordinates(newTile);
                    Debug.Log("New Tile Created. Tile Coordinates: " + check.ToString());
                }
            }
        }
        LevelGenComplete();
    }

    void LevelGenComplete()
    {
        foreach(GameObject tile in currentTiles)
        {
            MazeTile mt = tile.GetComponent<MazeTile>();
            foreach(ExitPoint exit in mt.exitPoints)
            {
                exit.gameObject.SetActive(false);
            }
        }

        isLevelGenDone = true;
        BakeNavMesh?.Invoke();
        SpawnPlayers?.Invoke();
    }


}
