using System;
using System.Linq;
using UnityEngine;
using static Globals;

public abstract class ModifierBase : MonoBehaviour
{
    public ModifierData Data;
    public int ModifierLevel = 1;

    public float ProcChance;
    public float Cooldown;
    public float EffectAmount;
    public float SecondaryEffectAmount;    
    public float EffectAmountCap;
    public float SecondaryEffectAmountCap;

    public string DisplayName;

    #region virtual methods
    protected virtual void OnActionPerformedEvent() { return; }
    #endregion

    protected virtual void OnEnable()
    {
        EnhanceableBase.OnModifiersChanged += UpdateData;
    }

    protected virtual void OnDisable()
    {
        EnhanceableBase.OnModifiersChanged -= UpdateData;
    }

    public void UpdateData()
    {
        if (Data != null)
        {
            var level = ModifierLevel - 1;

            if (Data.ProcChancePerLevel.Count >= ModifierLevel) ProcChance = Data.ProcChancePerLevel[level];
            if (Data.CooldownPerLevel.Count >= ModifierLevel) Cooldown = Data.CooldownPerLevel[level];
            if (Data.EffectAmountPerLevel.Count >= ModifierLevel) EffectAmount = Data.EffectAmountPerLevel[level];
            if (Data.SecondaryEffectAmountPerLevel.Count >= ModifierLevel) SecondaryEffectAmount = Data.SecondaryEffectAmountPerLevel[level];
            if (Data.EffectCapPerLevel.Count >= ModifierLevel) EffectAmountCap = Data.EffectCapPerLevel[level]; 
            if (Data.SecondaryEffectCapPerLevel.Count >= ModifierLevel) SecondaryEffectAmountCap = Data.SecondaryEffectCapPerLevel[level]; 

            DisplayName = $"{Data.ModifierName} {ModifierLevel.IntToRomalNumeral()}";
        }
    }

    public bool TryProcModifier()
    {
        return ProcChance > 100 - UnityEngine.Random.Range(1, 101);
    }
}
