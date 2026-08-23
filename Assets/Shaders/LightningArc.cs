using System.Collections.Generic;
using UnityEngine;

public class LightningArc : MonoBehaviour
{
    public Transform Start;
    public Transform End;

    public int positionCount;

    private Material material;
    private LineRenderer lr;

    private void OnEnable()
    {
        lr = GetComponent<LineRenderer>();
        material = lr.material;

        lr.positionCount = positionCount;

        List<Vector3> positions = new List<Vector3>();

        positions.Add(Start.position);

        var distance = Vector3.Distance(Start.position, End.position);
        var startToEnd = End.position - Start.position;

        for (int i = 1; i < positionCount; i++)
        {
            positions.Add(Start.position + (startToEnd * ((float)i / (float)positionCount)));
        }

        lr.SetPositions(positions.ToArray());

        material.SetVector("_StartPos", Start.position);
        material.SetVector("_EndPos", End.position);
    }

    private void Update()
    {

    }
}