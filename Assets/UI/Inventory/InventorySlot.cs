using UnityEngine;

public class InventorySlot : MonoBehaviour
{
    public InventoryItem currentItem;
    public bool isUsed;
    public Vector2 gridPosition;

    public void Init(Vector2 gridPosition)
    {
        this.gridPosition = gridPosition;
    }

    private void Update()
    {
        isUsed = currentItem != null;
    }

    public void SetItemInSlot(InventoryItem item)
    {
        currentItem = item;
        isUsed = true;
    }

    public void RemoveItemFromSlot()
    {
        if (currentItem != null)
        {
            Destroy(currentItem.gameObject);
        }

        isUsed = false;
    }
}
