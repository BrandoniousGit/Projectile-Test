using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public List<GameObject> connectors;
    public List<Tile> forwardTiles;
    public Tile backwardTile;
    public bool hasBeenChecked { get; set; }

    private void OnDrawGizmos()
    {
        foreach (var door in connectors)
        {
            if (door == null) return;

            Gizmos.color = new Color(1, 0.2f, 0.2f, 1);
            Gizmos.DrawCube(
                new Vector3(door.transform.position.x, door.transform.position.y + 3, door.transform.position.z),
                new Vector3(Mathf.Clamp(Mathf.Abs(door.transform.forward.z) * 3, 1, 3), 6, Mathf.Clamp(Mathf.Abs(door.transform.forward.x) * 3, 1, 3))
            );
            Gizmos.color = new Color(0.5f, 0.5f, 1, 1);
            Gizmos.DrawLine(door.transform.position + door.transform.forward * 0.25f, door.transform.position + door.transform.forward);
            Gizmos.DrawLine(door.transform.position + door.transform.forward * 0.75f + door.transform.right * 0.125f, door.transform.position + door.transform.forward);
            Gizmos.DrawLine(door.transform.position + door.transform.forward * 0.75f - door.transform.right * 0.125f, door.transform.position + door.transform.forward);
        }
    }
}
