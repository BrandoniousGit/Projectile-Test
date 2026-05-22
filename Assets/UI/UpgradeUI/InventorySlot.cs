using UnityEngine;
using static TokenScreen;

public class InventorySlot : MonoBehaviour
{
    public UpgradeToken currentToken;
    public bool isUsed;
    public Vector2 gridPosition;
    public InventorySlotType slotType;

    public void Init(Vector2 gridPosition, InventorySlotType slotType)
    {
        this.gridPosition = gridPosition;
        this.slotType = slotType;
    }
}
