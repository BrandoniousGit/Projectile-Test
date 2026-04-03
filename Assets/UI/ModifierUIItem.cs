using System;
using TMPro;
using UnityEngine;

public class ModifierUIItem : MonoBehaviour
{
    public TextMeshProUGUI UIItemText;
    public string InternalItemName;

    public static Action<bool, string> UpdateModifierEvent;

    public void OnModifierUpgradeClicked()
    {
        UpdateModifierEvent?.Invoke(true, InternalItemName);
    }    
    
    public void OnModifierDowngradeClicked()
    {
        UpdateModifierEvent?.Invoke(false, InternalItemName);
    }

    public void Update()
    {
        UIItemText.text = InternalItemName;
    }
}
