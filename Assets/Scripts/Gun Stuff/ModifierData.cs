using System;
using System.Collections.Generic;
using UnityEngine;
using static Globals;

public class ModifierData : ScriptableObject
{
    public List<EnhanceableItem> ApplicableToItems;
    public string ModifierName = "New Modifier";
    public string InternalName = "NewModifier";
    public string PrerequisiteModifier;
    [TextArea]
    public string Description = "Enter a description.";
    public int MaxLevel = 1;

    public float ProcChance = 0;
    public Rarity Rarity;
}
