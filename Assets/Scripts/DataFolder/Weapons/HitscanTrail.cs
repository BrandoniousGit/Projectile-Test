using System.Collections;
using UnityEngine;

public class HitscanTrail : MonoBehaviour
{
    public LineRenderer line;

    private void Update()
    {
        line.startWidth -= 0.1f * Time.deltaTime;
        line.endWidth -= 0.1f * Time.deltaTime;

        if (line.startWidth <= 0) Destroy(gameObject);
    }
}
