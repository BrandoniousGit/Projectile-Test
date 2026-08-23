using UnityEngine;
using static TokenScreen;

public class UpgradeStorageSlot : MonoBehaviour
{
    public UpgradeToken currentToken;
    public bool isUsed;
    public Vector2 gridPosition;
    public UpgradeSlotType slotType;

    public void Init(Vector2 gridPosition, UpgradeSlotType slotType)
    {
        this.gridPosition = gridPosition;
        this.slotType = slotType;
    }
}
