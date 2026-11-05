using System.Collections.Generic;
using UnityEngine;
using Enums;

[CreateAssetMenu(fileName = "New Environment", menuName = "LevelGen/New Environment")]
public class Environment : ScriptableObject
{
    public string Name;
    public LevelType LevelType;
    public GameObject SpawnTile;
    public GameObject ExitTile;
    public List<GameObject> Tiles;
    public GameObject ConnectorPrefab;
}