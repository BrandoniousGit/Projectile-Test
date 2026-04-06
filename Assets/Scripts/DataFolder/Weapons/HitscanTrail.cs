using System.Collections;
using UnityEngine;

public class HitscanTrail : MonoBehaviour
{
    public LineRenderer line;

    private void Update()
    {
        line.startWidth -= 0.3f * Time.deltaTime;
        line.endWidth -= 0.15f * Time.deltaTime;

        line.startWidth = Mathf.Clamp(line.startWidth, 0, 1);
        line.endWidth = Mathf.Clamp(line.endWidth, 0, 1);

        if (line.endWidth <= 0) Destroy(gameObject);
    }
}
