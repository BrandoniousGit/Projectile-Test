using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "New Item", menuName = "Items/New Item")]
public class ItemData : ScriptableObject
{
    public int ItemID;
    public string ItemName;
    public string ItemDescription;
    public int MaxStackSize;
}
