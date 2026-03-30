using System.Collections.Generic;
using UnityEngine;
using static Globals;

[CreateAssetMenu(fileName = "New GearData", menuName = "Gear/Equipment/Create Gear Data")]
public class GearData : ScriptableObject
{
    public GameObject Model;

    public GearType GearType;
    public string GearName;
    public string GearDescription;
}
