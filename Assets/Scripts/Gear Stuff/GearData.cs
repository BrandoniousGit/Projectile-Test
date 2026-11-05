using System.Collections.Generic;
using UnityEngine;
using Enums;

[CreateAssetMenu(fileName = "New GearData", menuName = "Gear/Equipment/Create Gear Data")]
public class GearData : ScriptableObject
{
    public GameObject Model;

    public GearType GearType;
    public string GearName;
    public string GearDescription;
}
