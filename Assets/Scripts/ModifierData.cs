using System.Collections.Generic;
using UnityEngine;
using static Globals;

[CreateAssetMenu(fileName = "New Modifier", menuName = "Scriptable Objects/New Modifier")]
public class ModifierData : ScriptableObject
{
    public List<EnhanceableItem> ApplicableToItems;
    public string ModifierName;
    public string InternalName;
    public string PrerequisiteModifier;
    public string Description;

    public Rarity Rarity;
    public ModifierApplyType ApplyType;
    public ModifiesStat ModifiesStat;

    public List<float> EffectAmountPerLevel;
    public List<float> SecondaryEffectAmountPerLevel;    
    public List<float> EffectCapPerLevel;
    public List<float> SecondaryEffectCapPerLevel;
    public List<float> CooldownPerLevel;
    public List<float> ProcChancePerLevel;

    public bool HasActiveAbility;
    public int MaxLevel;
}
