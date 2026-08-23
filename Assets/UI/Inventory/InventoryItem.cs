using TMPro;
using UnityEngine;
using UnityEngine.UI;

#nullable enable

public class InventoryItem : MonoBehaviour
{
    private ItemData Data;
    public int itemCount;
    public TextMeshProUGUI itemCountText;
    public Sprite fallbackSprite;
    public string InternalName => Data.ItemName.Replace(" ", "_").ToLower();

    public void Init(ItemData itemData, int itemCount)
    {
        Data = itemData;
        this.itemCount = itemCount;

        Sprite? sprite = Resources.Load<Sprite>($"Items/{InternalName}");

        if (sprite != null)
        {
            GetComponent<Image>().sprite = sprite;
        }
        else
        {
            GetComponent<Image>().sprite = fallbackSprite;
        }
    }

    public ItemData GetData()
    {
        return Data;
    }    

    public int GetItemId()
    {
        return Data.ItemID;
    }

    public int GetItemMaxStackSize()
    {
        return Data.MaxStackSize;
    }

    private void Update()
    {
        itemCountText.SetText(itemCount.ToString());
    }
}
