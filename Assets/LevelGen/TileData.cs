using UnityEngine;

[CreateAssetMenu(fileName = "New Tile", menuName = "LevelGen/New Tile")]
public class TileData : ScriptableObject
{
    public GameObject TilePrefab;
    public Vector2 TileSize;
    public int Weight;
}