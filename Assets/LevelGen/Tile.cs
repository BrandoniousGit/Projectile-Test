using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public List<GameObject> connectors;
    public List<Tile> forwardTiles;
    public Vector2 tileCoordinates;
    public Tile backwardTile;
    public bool hasBeenChecked { get; set; }

    private void OnDrawGizmos()
    {
        foreach (var connector in connectors)
        {
            if (connector == null) return;

            Gizmos.color = new Color(1, 0.2f, 0.2f, 1);
            Gizmos.DrawCube(
                new Vector3(connector.transform.position.x, connector.transform.position.y + 3, connector.transform.position.z),
                new Vector3(Mathf.Clamp(Mathf.Abs(connector.transform.forward.z) * 3, 1, 3), 6, Mathf.Clamp(Mathf.Abs(connector.transform.forward.x) * 3, 1, 3))
            );
            Gizmos.color = new Color(0.5f, 0.5f, 1, 1);
            Gizmos.DrawLine(connector.transform.position + connector.transform.forward * 0.25f, connector.transform.position + connector.transform.forward);
            Gizmos.DrawLine(connector.transform.position + connector.transform.forward * 0.75f + connector.transform.right * 0.125f, connector.transform.position + connector.transform.forward);
            Gizmos.DrawLine(connector.transform.position + connector.transform.forward * 0.75f - connector.transform.right * 0.125f, connector.transform.position + connector.transform.forward);
        }
    }
}
