using System;
using UnityEngine;

public class GroundItem : MonoBehaviour
{
    public int itemId;
    public int itemCount;

    private bool initialized;
    public float pickupTimer;
    private bool canBePickedUp;

    public static event Action OnItemPickedUpEvent;

    private void OnTriggerStay(Collider other)
    {
        if (!canBePickedUp || other.tag != "Player" || !initialized) return;

        var pickupSuccessful = MenuManager.Instance.PlayerInventory.TryPickupItem(itemId, itemCount);

        if (pickupSuccessful)
        {
            OnItemPickedUpEvent?.Invoke();
            Destroy(gameObject);
        }
    }

    public void Init(int itemId, int itemCount)
    {
        this.itemId = itemId;
        this.itemCount = itemCount;

        pickupTimer = 2.5f;

        initialized = true;
    }

    private void Update()
    {
        initialized = true;

        if (!initialized) return;

        if (pickupTimer > 0)
        {
            pickupTimer -= Time.deltaTime;
        }
        else
        {
            canBePickedUp = true;
        }
    }
}
