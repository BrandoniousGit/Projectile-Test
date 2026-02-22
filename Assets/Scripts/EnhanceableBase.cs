using com.cyborgAssets.inspectorButtonPro;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class EnhanceableBase : MonoBehaviour
{
    public List<ModifierBase> modifiers = new();
    public List<string> modifierNames = new();
    public static event Action OnModifiersChanged;
    public static event Action OnActiveAbilityButtonPressed;

    void Start()
    {
        DirectoryInfo dir = new DirectoryInfo("Assets/Resources/Modifiers");
        FileInfo[] info = dir.GetFiles("*.cs");
        modifierNames = info.Select(file => file.Name.RemoveFileExtension()).ToList();
    }

    public void ClearNullModifiers()
    {
        modifiers.RemoveAll(modifier => modifier == null);
    }

    protected virtual void Update()
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
                modifiers.Add(newModifier);
            }
        }

        OnModifiersChanged?.Invoke();
    }
    #endregion
}
