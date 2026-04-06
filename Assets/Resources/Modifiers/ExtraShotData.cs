using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Modifier", menuName = "New Modifier/Extra Shot")]
public class ExtraShotData : ModifierData
{
    public List<int> ShotsUntilExtraPerLevel;
}
