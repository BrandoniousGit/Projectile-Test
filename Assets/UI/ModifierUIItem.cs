using System;
using TMPro;
using UnityEngine;

public class ModifierUIItem : MonoBehaviour
{
    public TextMeshProUGUI UIItemText;
    public string InternalItemName;

    public static Action<bool, string, int> UpdateModifierEvent;

    public void OnModifierUpgradeClicked()
    {
        UpdateModifierEvent?.Invoke(true, InternalItemName, 1);
    }    
    
    public void OnModifierDowngradeClicked()
    {
        UpdateModifierEvent?.Invoke(false, InternalItemName, 1);
    }

    public void Update()
    {
        UIItemText.text = InternalItemName;
    }
}
