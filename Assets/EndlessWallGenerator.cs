using System.Collections.Generic;
using UnityEngine;

public class EndlessWallGenerator : MonoBehaviour
{
    [Header("Wall Settings")]
    public GameObject[] wallPrefabs;
    public int poolSizePerPrefab = 5;
    public float spawnDistance = 20f;
    public float wallSpacing = 5f;
    [Range(3f, 20f)] public float aisleWidth = 8f;
    public bool rotateLeftSide = true; // Toggle which side gets rotated

    [Header("Player Reference")]
    public Transform player;

    [Header("Debug")]
    public bool showDebug = true;
    public Color debugColor = Color.cyan;

    private Dictionary<GameObject, Queue<GameObject>> wallPool = new Dictionary<GameObject, Queue<GameObject>>();
    private float nextSpawnZ = 0f;
    private List<GameObject> activeWalls = new List<GameObject>();

    void Start()
    {
        InitializePool();
        nextSpawnZ = player.position.z + spawnDistance;
        SpawnInitialWalls();
    }

    void Update()
    {
        if (player.position.z > nextSpawnZ - spawnDistance)
        {
            SpawnWallPair();
            nextSpawnZ += wallSpacing;
        }
        RecycleBehindPlayer();
    }

    void InitializePool()
    {
        foreach (GameObject prefab in wallPrefabs)
        {
            Queue<GameObject> objectQueue = new Queue<GameObject>();
            for (int i = 0; i < poolSizePerPrefab; i++)
            {
                GameObject newWall = Instantiate(prefab);
                newWall.SetActive(false);
                objectQueue.Enqueue(newWall);
            }
            wallPool.Add(prefab, objectQueue);
        }
    }

    void SpawnInitialWalls()
    {
        float startZ = player.position.z - 10f;
        float endZ = player.position.z + spawnDistance;
        
        for (float z = startZ; z <= endZ; z += wallSpacing)
        {
            SpawnWallAt(z);
        }
    }

    void SpawnWallPair()
    {
        SpawnWallAt(nextSpawnZ);
    }

    void SpawnWallAt(float zPosition)
    {
        // Left wall (rotated 180 degrees if enabled)
        Quaternion leftRotation = rotateLeftSide ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;
        SpawnSingleWall(new Vector3(-aisleWidth/2, 0, zPosition), leftRotation);
        
        // Right wall (normal rotation)
        SpawnSingleWall(new Vector3(aisleWidth/2, 0, zPosition), Quaternion.identity);
    }

    void SpawnSingleWall(Vector3 position, Quaternion rotation)
    {
        GameObject randomPrefab = wallPrefabs[Random.Range(0, wallPrefabs.Length)];
        Queue<GameObject> prefabQueue = wallPool[randomPrefab];
        
        GameObject wallToSpawn = prefabQueue.Count > 0 ? prefabQueue.Dequeue() : Instantiate(randomPrefab);
        
        wallToSpawn.transform.position = position;
        wallToSpawn.transform.rotation = rotation;
        wallToSpawn.SetActive(true);
        activeWalls.Add(wallToSpawn);
    }

    void RecycleBehindPlayer()
    {
        float recycleZ = player.position.z - 10f;
        
        for (int i = activeWalls.Count - 1; i >= 0; i--)
        {
            if (activeWalls[i].transform.position.z < recycleZ)
            {
                GameObject wall = activeWalls[i];
                activeWalls.RemoveAt(i);
                
                foreach (var kvp in wallPool)
                {
                    if (wall.name.StartsWith(kvp.Key.name))
                    {
                        wall.SetActive(false);
                        kvp.Value.Enqueue(wall);
                        break;
                    }
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        if (!showDebug || player == null) return;
        
        Gizmos.color = debugColor;
        Vector3 startPos = player.position + Vector3.back * 10f;
        Vector3 endPos = player.position + Vector3.forward * spawnDistance;
        
        // Left boundary with rotation indicator
        Gizmos.DrawLine(startPos + Vector3.left * aisleWidth/2, endPos + Vector3.left * aisleWidth/2);
        if (rotateLeftSide)
        {
            Gizmos.DrawIcon(startPos + Vector3.left * aisleWidth/2 + Vector3.up * 2, "RotateTool");
        }
        
        // Right boundary
        Gizmos.DrawLine(startPos + Vector3.right * aisleWidth/2, endPos + Vector3.right * aisleWidth/2);
        
        // Spawn trigger line
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(
            new Vector3(-aisleWidth, 0, nextSpawnZ),
            new Vector3(aisleWidth, 0, nextSpawnZ)
        );
    }
}