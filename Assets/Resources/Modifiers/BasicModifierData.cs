using static Globals;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Modifier", menuName = "New Modifier/Basic Modifier")]
public class BasicModifierData : ModifierData
{
    public ModifiesStat ModifiesStat;
    public ModifierApplyType ApplyType;
    public List<float> ValuePerLevel;
}