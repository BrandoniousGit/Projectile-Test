using UnityEngine;

public class DebugObjectiveLocation : MonoBehaviour
{
    public bool alwaysShow;
    public Color displayColor = new Color(0, 0, 0.75f, 0.33f);
    public float sphereSize = 0.5f;

    private void OnDrawGizmos()
    {
        if (!alwaysShow)
        {
            return;
        }

        Gizmos.color = displayColor;
        Gizmos.DrawSphere(transform.position, sphereSize);
    }

    private void OnDrawGizmosSelected()
    {
        if (alwaysShow)
        {
            return;
        }

        Gizmos.color = displayColor;
        Gizmos.DrawSphere(transform.position, sphereSize);
    }
}
