using System.Collections.Generic;
using UnityEngine;
using static Globals;

[CreateAssetMenu(fileName = "New Modifier", menuName = "Scriptable Objects/New Modifier")]
public class ModifierData : ScriptableObject
{
    public string EnhancementName;
    public string InternalName;
    public string PrerequisiteEnhancement;

    public Rarity Rarity;

    public List<float> EffectAmountPerLevel;
    public List<float> CooldownPerLevel;
    public List<float> ProcChancePerLevel;

    public bool HasActiveAbility;
    public int MaxLevel;
}
