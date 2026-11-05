using System;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Splines;
using UnityEngine.UIElements;
using static UnityEngine.UI.GridLayoutGroup;

public class ConnectedWires : MonoBehaviour
{
    public Transform startPos;
    public Transform endPos;
    public LayerMask groundMask;
    public bool addRandomness;

    public NavMeshAgent walkerAgent;
    public NavMeshSurface surface;

    private Vector3 lastPosition;

    private SplineContainer wireSpline;

    public float stepDistance = 0.2f;

    private void Start()
    {
        wireSpline = GetComponent<SplineContainer>();

        if (surface != null)
        {
            surface.AddData();
            surface.BuildNavMesh();

            walkerAgent.gameObject.SetActive(true);
        }

        GenerateWireSpline();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            GenerateWireSpline();
        }
    }

    private void GenerateWireSpline()
    {
        wireSpline.Spline.Clear();

        Physics.Raycast(startPos.position + (Vector3.up * 0.25f), Vector3.down, out var startPosFloor, Mathf.Infinity, groundMask);

        CreateWirePosition(startPosFloor.point);

        NavMeshPath newPath = new();
        walkerAgent.CalculatePath(endPos.position, newPath);

        for (int i = 0; i < newPath.corners.Length; i++)
        {
            CreateWirePosition(newPath.corners[i]);

            try
            {
                var disToNextPos = Vector3.Distance(newPath.corners[i], newPath.corners[i + 1]);
                var dirToNextPos = (newPath.corners[i + 1] - newPath.corners[i]).normalized;

                var count = Mathf.Floor(disToNextPos / stepDistance);

                for (int j = 1; j < count; j++)
                {
                    var wirePos = newPath.corners[i] + new Vector3(dirToNextPos.x * stepDistance * j, dirToNextPos.y * stepDistance * j, dirToNextPos.z * stepDistance * j);

                    if (Physics.Raycast(wirePos + (Vector3.up * 0.25f), Vector3.down, out var hitInfo, Mathf.Infinity, groundMask))
                    {
                        wirePos = new Vector3(wirePos.x, hitInfo.point.y, wirePos.z);
                    }

                    CreateWirePosition(wirePos);
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                Debug.Log("Final point in line, no calculation needed!");
            }
        }

        Physics.Raycast(endPos.position + (Vector3.up * 0.25f), Vector3.down, out var endPosFloor, Mathf.Infinity, groundMask);

        CreateWirePosition(endPosFloor.point);
    }

    private void CreateWirePosition(Vector3 nodePos)
    {
        var newPosition = nodePos;
        //var newPosition = new Vector3(Mathf.RoundToInt(nodePos.x * 1 / stepDistance) * stepDistance, nodePos.y, Mathf.RoundToInt(nodePos.z * 1 / stepDistance) * stepDistance);

        if (addRandomness)
        {
            var thirdPoint = ((lastPosition - newPosition) * 0.5f) + Vector3.up;

            var a = newPosition - thirdPoint;
            var b = lastPosition - thirdPoint;

            var cross = Vector3.Cross(a, b).normalized;
            var random = UnityEngine.Random.Range(-(stepDistance / 4), stepDistance / 4);

            newPosition += new Vector3(cross.x + random, 0, cross.z * random);
        }

        wireSpline.Spline.Add(newPosition);
        lastPosition = newPosition;
    }
}