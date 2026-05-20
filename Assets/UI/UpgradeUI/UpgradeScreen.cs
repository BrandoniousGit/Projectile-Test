using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradeScreen : MonoBehaviour
{
    private Canvas upgradeCanvas;
    private GraphicRaycaster graphicRaycaster;
    private EventSystem eventSystem;
    private GameObject selectedItem;

    public GameObject InventorySlot;
    public Sprite OctagonTile;
    public Sprite StraightTile;
    public Sprite CornerTile;
    public Sprite BackgroundTile;

    public Vector2 GridSize;
    public float TileSize;

    public static event Action<bool, string, int> UpdateModifierEvent;

    private void Start()
    {
        upgradeCanvas = GetComponent<Canvas>();
        graphicRaycaster = GetComponent<GraphicRaycaster>();
        eventSystem = GameObject.FindGameObjectWithTag("EventSystem").GetComponent<EventSystem>();

        var modifiedTileSize = TileSize * upgradeCanvas.transform.localScale.x;

        for (int i = 0; i < GridSize.x; i++)
        {
            for (int j = 0; j < GridSize.y; j++)
            {
                var uiItem = Instantiate(InventorySlot, new Vector3(i * modifiedTileSize, j * modifiedTileSize, 0), Quaternion.identity, upgradeCanvas.gameObject.transform);
                uiItem.transform.localScale *= TileSize;
                uiItem.GetComponent<Image>().sprite = BackgroundTile;
            }
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PointerEventData pointerData = new PointerEventData(eventSystem)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> results = new();
            graphicRaycaster.Raycast(pointerData, results);      
            
            InventorySlot inventorySlot = null;
            UpgradeToken upgradeToken = null;

            foreach (var result in results)
            {
                switch (result.gameObject.tag)
                {
                    case "InventorySlot":
                        inventorySlot = result.gameObject.GetComponent<InventorySlot>();
                        break;
                    case "UIItem":
                        upgradeToken = result.gameObject.GetComponent<UpgradeToken>();
                        break;
                    default:
                        Debug.Log("Found no matching tag");
                        break;
                }
            }

            // if no selected item
            if (selectedItem == null)
            {
                if (upgradeToken != null)
                {
                    selectedItem = upgradeToken.gameObject;

                    if (inventorySlot != null)
                    {
                        inventorySlot.isUsed = false;
                        UpdateModifierEvent?.Invoke(false, upgradeToken.UpgradeName, upgradeToken.Level);
                    }
                }
            }
            else
            {
                // if inventoryslot under cursor and is not in use
                if (inventorySlot != null && !inventorySlot.isUsed)
                {
                    selectedItem.transform.position = inventorySlot.transform.position;
                    UpdateModifierEvent?.Invoke(true, upgradeToken.UpgradeName, upgradeToken.Level);

                    inventorySlot.isUsed = true;

                    selectedItem = null;
                }
                else if (inventorySlot == null)
                {
                    selectedItem = null;
                }
            }
        }

        if (selectedItem != null)
        {
            selectedItem.transform.position = Input.mousePosition - new Vector3(selectedItem.transform.localScale.x / 2, selectedItem.transform.localScale.y / 2, 0);
        }
    }
}
