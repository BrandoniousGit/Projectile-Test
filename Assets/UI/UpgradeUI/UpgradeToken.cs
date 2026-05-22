using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static Globals;

public class UpgradeToken : MonoBehaviour
{
    [SerializeField]
    private Image tokenImage;
    public string upgradeName;
    public string description;
    public int level;

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
        upgradeName = modifierData.ModifierName;
        description = modifierData.Description;
        level = 1;

        if (ColorUtility.TryParseHtmlString(RarityLookup.First(rarity => rarity.Key == modifierData.Rarity).Value, out Color color))
        {
            tokenImage.color = color;
        }
    }
}
