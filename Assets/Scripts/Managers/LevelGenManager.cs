using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class LevelGenManager : MonoBehaviour
{
    public static LevelGenManager Instance { get; private set; }

    public Environment environment;
    public int seed;
    public List<Tile> spawnedLevelTiles;
    public int maxTiles, failsafeCount;
    public float generationSpeed;

    public Vector2 minMaxExitDistance;

    public LayerMask tileLayermask;
    public LayerMask doorLayermask;

    private int count;

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.X))
        //{
        //    GenerateLevel();
        //}
    }

    private void GenerateLevel()
    {
        Random.InitState(seed);

        count = 0;

        RemoveAllTiles();

        if (!environment.Tiles.Any())
        {
            Debug.LogError("Environment has no tiles!");
            return;
        }

        if (environment.SpawnTile != null)
        {
            var spawnTile = Instantiate(environment.SpawnTile, Vector3.up * 50, Quaternion.identity);
            spawnTile.name = "SpawnTile";

            if (spawnTile.TryGetComponent<Tile>(out var spawnTileScript))
            {
                spawnedLevelTiles.Add(spawnTileScript);
            }
            else
            {
                Debug.LogError("Spawn Tile has no tile script!");
            }
        }
        else
        {
            Debug.LogError("Environment has no spawn tile!");
            return;
        }
    }

    private void RemoveAllTiles()
    {
        if (spawnedLevelTiles.Any())
        {
            foreach (var tile in spawnedLevelTiles)
            {
                Destroy(tile.gameObject);
            }

            spawnedLevelTiles.Clear();
        }
    }
}