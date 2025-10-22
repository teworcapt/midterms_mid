using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [Header("Wall Prefabs")]
    [SerializeField] private GameObject smallWallPrefab;
    [SerializeField] private GameObject mediumWallPrefab;
    [SerializeField] private GameObject largeWallPrefab;

    [Header("Start / Finish References (assign in scene)")]
    [SerializeField] private Transform startLine;
    [SerializeField] private Transform finishLine;

    [Header("Platform Settings")]
    [SerializeField] private Transform platform;
    [SerializeField] private Vector3 platformSize = new Vector3(7f, 1f, 7f);

    [Header("Wall Counts")]
    [SerializeField] private int largeWallCount = 3;
    [SerializeField] private int mediumWallCount = 6;
    [SerializeField] private int smallWallCount = 10;

    [Header("Spawn Rules")]
    [SerializeField] private int maxPlacementAttemptsPerWall = 25;
    [SerializeField] private float wallY = 1.5f;
    [SerializeField] private float wallSpacingFactor = 1.15f;
    [SerializeField] private float startFinishClearRadius = 4f;

    private readonly List<GameObject> spawnedWalls = new List<GameObject>();
    private Bounds platformBounds;

    private void Start()
    {
        if (platform == null)
        {
            Debug.LogError("Please assign a platform Transform in the inspector.");
            return;
        }

        if (startLine == null || finishLine == null)
        {
            Debug.LogError("Please assign StartLine and FinishLine Transforms in the inspector.");
            return;
        }

        Collider platCollider = platform.GetComponent<Collider>();
        Renderer platRenderer = platform.GetComponent<Renderer>();
        if (platCollider != null)
        {
            platformSize = platCollider.bounds.size;
        }
        else if (platRenderer != null)
        {
            platformSize = platRenderer.bounds.size;
        }
        else
        {
            Debug.LogWarning("Platform has no collider or renderer, using default size.");
        }

        platformBounds = new Bounds(platform.position, platformSize);

        StartCoroutine(SpawnWallsRoutine());
    }

    private IEnumerator SpawnWallsRoutine()
    {
        yield return SpawnWallsOfType(largeWallPrefab, largeWallCount);
        yield return SpawnWallsOfType(mediumWallPrefab, mediumWallCount);
        yield return SpawnWallsOfType(smallWallPrefab, smallWallCount);
    }

    private IEnumerator SpawnWallsOfType(GameObject prefab, int count)
    {
        if (!prefab)
        {
            Debug.LogWarning("Missing wall prefab reference.");
            yield break;
        }

        Vector3 wallSize = Vector3.one;
        BoxCollider tempCollider = null;
        GameObject temp = Instantiate(prefab);
        tempCollider = temp.GetComponentInChildren<BoxCollider>();
        if (tempCollider != null)
            wallSize = tempCollider.bounds.size;
        Destroy(temp);

        for (int i = 0; i < count; i++)
        {
            bool placed = false;
            int attempts = 0;

            while (!placed && attempts < maxPlacementAttemptsPerWall)
            {
                attempts++;

                Quaternion rot = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

                Vector2 wallDir = new Vector2(Mathf.Cos(rot.eulerAngles.y * Mathf.Deg2Rad), Mathf.Sin(rot.eulerAngles.y * Mathf.Deg2Rad));
                float halfLength = wallSize.x / 2f;
                float halfDepth = wallSize.z / 2f;
                float projectedHalfX = Mathf.Abs(wallDir.x) * halfLength + Mathf.Abs(wallDir.y) * halfDepth;
                float projectedHalfZ = Mathf.Abs(wallDir.y) * halfLength + Mathf.Abs(wallDir.x) * halfDepth;

                float minX = platform.position.x - (platformSize.x / 2f) + projectedHalfX;
                float maxX = platform.position.x + (platformSize.x / 2f) - projectedHalfX;
                float minZ = platform.position.z - (platformSize.z / 2f) + projectedHalfZ;
                float maxZ = platform.position.z + (platformSize.z / 2f) - projectedHalfZ;

                float rx = Random.Range(minX, maxX);
                float rz = Random.Range(minZ, maxZ);
                Vector3 candidatePos = new Vector3(rx, wallY, rz);

                if (Vector3.Distance(candidatePos, startLine.position) < startFinishClearRadius) continue;
                if (Vector3.Distance(candidatePos, finishLine.position) < startFinishClearRadius) continue;

                Vector3 halfExtents = (wallSize * 0.5f) * wallSpacingFactor;
                Collider[] overlaps = Physics.OverlapBox(candidatePos, halfExtents, rot);

                bool collides = false;
                foreach (var c in overlaps)
                {
                    if (c == null) continue;
                    if (spawnedWalls.Contains(c.gameObject))
                    {
                        collides = true;
                        break;
                    }
                }

                if (collides)
                {
                    yield return null;
                    continue;
                }

                GameObject wall = Instantiate(prefab, candidatePos, rot, transform);
                wall.name = $"{prefab.name}_spawned_{i}";
                spawnedWalls.Add(wall);
                placed = true;
                yield return null;
            }

            if (!placed)
            {
                Vector3 fallback = RandomPointOnPlatformXZ();
                GameObject wall = Instantiate(prefab, fallback, Quaternion.Euler(0, Random.Range(0, 360f), 0), transform);
                spawnedWalls.Add(wall);
            }
        }
    }

    private Vector3 RandomPointOnPlatformXZ()
    {
        float rx = Random.Range(platform.position.x - platformSize.x / 2f, platform.position.x + platformSize.x / 2f);
        float rz = Random.Range(platform.position.z - platformSize.z / 2f, platform.position.z + platformSize.z / 2f);
        return new Vector3(rx, wallY, rz);
    }

    private void OnDrawGizmosSelected()
    {
        if (!platform) return;

        Gizmos.color = new Color(0f, 0.6f, 1f, 0.15f);
        Gizmos.matrix = Matrix4x4.TRS(platform.position, Quaternion.identity, Vector3.one);
        Gizmos.DrawCube(Vector3.zero, platformSize);

        Gizmos.color = new Color(1f, 0.3f, 0f, 0.2f);
        if (startLine) Gizmos.DrawSphere(startLine.position, startFinishClearRadius);
        if (finishLine) Gizmos.DrawSphere(finishLine.position, startFinishClearRadius);
    }
}
