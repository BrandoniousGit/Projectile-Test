using Extensions;
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
    public Vector2 minMaxMapRadius;

    public LayerMask tileLayermask;
    public LayerMask connectorLayermask;

    private int count;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            StartGenerateLevel();
        }
    }

    private void Start()
    {
        StartGenerateLevel();
    }

    private void StartGenerateLevel()
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
            var spawnTile = Instantiate(environment.SpawnTile, Vector3.zero, Quaternion.identity);
            spawnTile.name = "SpawnTile";

            if (spawnTile.TryGetComponent<Tile>(out var spawnTileScript))
            {
                spawnedLevelTiles.Add(spawnTileScript);

                foreach (var connector in spawnTileScript.connectors)
                {
                    TrySpawnTile(connector, spawnTile.transform.position, spawnTileScript);
                }
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

    public Tile TrySpawnTile(GameObject oldConnector, Vector3 center, Tile parentTile)
    {
        count += 1;

        //Spawn tile on perimeter edge
        var spawnedTile = Instantiate(environment.Tiles[Random.Range(0, environment.Tiles.Count)], oldConnector.transform.position, Quaternion.identity);
        spawnedTile.name = $"Tile{count}";

        if (spawnedTile.TryGetComponent<Tile>(out var tileComponent))
        {
            //Pick a random connector on the new tile to align with the old one
            var num = Random.Range(0, tileComponent.connectors.Count);
            var chosenConnector = tileComponent.connectors[num].gameObject;

            var offset = oldConnector.transform.position - chosenConnector.transform.position;
            spawnedTile.transform.position += offset;

            var A = chosenConnector.transform.eulerAngles.y;
            var B = oldConnector.transform.eulerAngles.y;

            //This took 3 hours :c (Dino solved it in 3 minutes)
            var angle = B - A + 180;

            spawnedTile.transform.RotateAround(oldConnector.transform.position, Vector3.up, angle);

            spawnedLevelTiles.Add(tileComponent);
            tileComponent.backwardTile = parentTile;

            var isValid = CheckForOverlaps(tileComponent);

            if (isValid)
            {
                CheckConnectors(tileComponent);
                parentTile.forwardTiles.Add(tileComponent);
            }

            return tileComponent;
        }
        else
        {
            Destroy(spawnedTile);
            Debug.Log("Something went wrong!");

            return null;
        }
    }

    public bool CheckForOverlaps(Tile tile)
    {
        var boxCollider = tile.gameObject.GetComponent<BoxCollider>();

        var tileOverlapBox = Physics.OverlapBox(tile.transform.position + boxCollider.center, boxCollider.size / 2, tile.transform.rotation, tileLayermask, QueryTriggerInteraction.Collide).ToList();
        tileOverlapBox = tileOverlapBox.Where(x => x.name != tile.gameObject.name).ToList();

        if (tileOverlapBox.Any())
        {
            Debug.Log($"There has been a collision between {tileOverlapBox.First().name} & {tile.gameObject.name}. {tileOverlapBox.First().name} was first so I will destroy {tile.gameObject.name}!");
            spawnedLevelTiles.RemoveAtAndDestroy(spawnedLevelTiles.Count - 1);
            Destroy(tile.gameObject);

            return false;
        }

        return true;
    }

    public void CheckConnectors(Tile tile)
    {
        List<GameObject> connectorsToRemove = new();

        foreach (var connector in tile.connectors)
        {
            var connectorOverlapBox = Physics.OverlapBox(connector.transform.position, new Vector3(1, 1, 1) / 2, connector.transform.rotation, connectorLayermask, QueryTriggerInteraction.Collide).ToList();
            connectorOverlapBox = connectorOverlapBox.Where(x => x.name != connector.name && x.transform.parent.name != connector.transform.parent.name).ToList();

            if (connectorOverlapBox.Any())
            {
                var connectorObject = Instantiate(environment.ConnectorPrefab, connector.transform.position, connector.transform.rotation, tile.transform);
                connectorObject.transform.position += new Vector3(0, connectorObject.transform.localScale.y / 2, 0);
                connectorsToRemove.Add(connector);
            }

            connector.name = $"{connector.name}-{tile.name}";
        }

        foreach (var connector in connectorsToRemove)
        {
            tile.connectors.Remove(connector);
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