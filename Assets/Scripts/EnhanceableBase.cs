using com.cyborgAssets.inspectorButtonPro;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class EnhanceableBase : MonoBehaviour
{
    public List<ModifierBase> modifiers;
    public static event Action OnActionPerformed;
    public static event Action OnModifiersChanged;
    public static event Action OnActiveAbilityButtonPressed;
    public float CalculatedDamage;

    public void ClearNullModifiers()
    {
        modifiers.RemoveAll(modifier => modifier == null);
    }

    public void CallMyActionPerformedEvent()
    {
        OnActionPerformed?.Invoke();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            OnActiveAbilityButtonPressed?.Invoke();
        }
    }

    #region editor buttons
    [ProButton]
    public void UpdateModifier(bool IsUpgrading, string ModifierTypeString, string ItemName)
    {
        if (string.IsNullOrWhiteSpace(ModifierTypeString)) return;

        var type = Type.GetType(ModifierTypeString, false, true);

        if (type != null)
        {
            if (modifiers.Select(item => item.Data.InternalName.ToLower()).Contains(ModifierTypeString.ToLower()))
            {
                ModifierBase existingModifier = (ModifierBase)gameObject.GetComponent(type);

                if (IsUpgrading)
                {
                    if (existingModifier.ModifierLevel < existingModifier.Data.MaxLevel)
                    {
                        existingModifier.ModifierLevel++;
                    }
                }
                else
                {
                    if (existingModifier.ModifierLevel > 1)
                    {
                        existingModifier.ModifierLevel--;
                    }
                    else
                    {
                        modifiers.Remove(existingModifier);
                        Destroy(existingModifier);
                    }
                }
            }
            else if (IsUpgrading)
            {
                var newModifier = (ModifierBase)gameObject.AddComponent(type);
                newModifier.Data = (ModifierData)Resources.Load($"Modifiers/Data/{ModifierTypeString}");
                newModifier.AttachedEnhanceableBase = this;
                modifiers.Add(newModifier);
            }
        }

        OnModifiersChanged?.Invoke();
    }
    #endregion
}
