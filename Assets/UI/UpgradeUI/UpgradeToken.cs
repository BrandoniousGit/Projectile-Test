using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static Globals;
using static TokenScreen;

public class UpgradeToken : MonoBehaviour
{
    [SerializeField]
    private Image tokenImage;
    public string upgradeName;
    [TextArea]
    public string description;
    public int level;

    public ModifierBase MyModifer;
    public EnhanceableBase MyWeapon;
    public InventorySlotType MySlot;

    public bool UpgradeIsEnabled => MyWeapon.gameObject.activeSelf && MySlot == InventorySlotType.Upgrade;

    public void Init(string upgradeName, string description, int level, Rarity itemRarity)
    {
        this.upgradeName = upgradeName;
        this.description = description;
        this.level = level;
        if (ColorUtility.TryParseHtmlString(RarityLookup.First(rarity => rarity.Key == itemRarity).Value, out Color color))
        {
            tokenImage.color = color;
        }
    }    
    
    public void Init(ModifierData modifierData)
    {
        var type = Type.GetType(modifierData.InternalName);
        MyModifer = (ModifierBase)gameObject.AddComponent(type);
        MyModifer.SetData(modifierData);
        MyModifer.Enhanceable = MyWeapon;

        upgradeName = modifierData.ModifierName;
        description = MyModifer.GetDescription();
        level = 1;

        if (ColorUtility.TryParseHtmlString(RarityLookup.First(rarity => rarity.Key == modifierData.Rarity).Value, out Color color))
        {
            tokenImage.color = color;
        }
    }

    private void Update()
    {
        MyModifer.enabled = UpgradeIsEnabled;
    }
}
