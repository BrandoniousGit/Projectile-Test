using System;
using System.Linq;
using UnityEngine;
using static Globals;

public abstract class ModifierBase : MonoBehaviour
{
    public EnhanceableBase AttachedEnhanceableBase { get; set; }

    public ModifierData Data;
    public int ModifierLevel = 1;

    public float ProcChance;
    public float Cooldown;
    public float EffectAmount;

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
            if (Data.ProcChancePerLevel.Count >= ModifierLevel)
            {
                ProcChance = Data.ProcChancePerLevel[ModifierLevel - 1];
            }
 
            if (Data.CooldownPerLevel.Count >= ModifierLevel)
            {
                Cooldown = Data.CooldownPerLevel[ModifierLevel - 1];
            }

            if (Data.EffectAmountPerLevel.Count >= ModifierLevel)
            {
                EffectAmount = Data.EffectAmountPerLevel[ModifierLevel - 1];
            }

            DisplayName = $"{Data.ModifierName} {(ModifierLevel).IntToRomalNumeral()}";
        }
    }

    public bool TryProcModifier()
    {
        return ProcChance > 100 - UnityEngine.Random.Range(1, 101);
    }
}
