using Enums;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Objective", menuName = "New Objective")]
public class ObjectiveData : ScriptableObject
{
    public string ObjectiveName;
    public string ObjectiveStringFormatting;
    public string ObjectivePreview;
    public string ObjectiveItemResourceName;
    public int Weight;
    public int ProgressNeeded;
    public ObjectiveType Type;
}