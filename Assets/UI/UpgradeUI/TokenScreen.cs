using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TokenScreen : MonoBehaviour
{
    private Canvas upgradeCanvas;
    private GraphicRaycaster graphicRaycaster;
    private EventSystem eventSystem;
    private GameObject selectedItem;
    private InventorySlot fallbackSlot;
    private List<InventorySlot> inventorySlots = new();

    public EnhanceableBase Enhanceable;

    public GameObject InventorySlot;
    public GameObject UpgradeToken;
    public Sprite OctagonTile;
    public Sprite StraightTile;
    public Sprite CornerTile;
    public Sprite BackgroundTile;

    public Vector2 GridSize;
    public float TileSize;
    public List<string> UpgradePool;

    public enum InventorySlotType
    { 
        Upgrade,
        Storage
    }

    private void Start()
    {
        DirectoryInfo dir = new DirectoryInfo("Assets/Resources/Modifiers/Data");
        FileInfo[] info = dir.GetFiles("*.asset");
        UpgradePool = info.Select(file => file.Name.RemoveFileExtension(6)).ToList();

        upgradeCanvas = GetComponent<Canvas>();
        graphicRaycaster = GetComponent<GraphicRaycaster>();
        eventSystem = GameObject.FindGameObjectWithTag("EventSystem").GetComponent<EventSystem>();

        CreateInventory(new Vector2(180, 150), new Vector2(3,3), InventorySlotType.Upgrade);
        CreateInventory(new Vector2(480, 100), new Vector2(3,5), InventorySlotType.Storage);
    }

    private void CreateInventory(Vector2 bottomLeft, Vector2 gridSize, InventorySlotType slotType)
    {
        var modifiedTileSize = TileSize * upgradeCanvas.transform.localScale.x;
        var modifiedOffset = bottomLeft * upgradeCanvas.transform.localScale.x;

        for (int i = 0; i < gridSize.x; i++)
        {
            for (int j = 0; j < gridSize.y; j++)
            {
                var uiItem = Instantiate
                (
                    InventorySlot, 
                    new Vector3(i * modifiedTileSize + modifiedOffset.x, j * modifiedTileSize + modifiedOffset.y, 0), 
                    Quaternion.identity, 
                    upgradeCanvas.gameObject.transform
                );

                var slot = uiItem.GetComponent<InventorySlot>();
                slot.Init(new Vector2(i, j), slotType);
                inventorySlots.Add(slot);
                uiItem.transform.localScale *= TileSize;
                uiItem.GetComponent<Image>().sprite = BackgroundTile;
            }
        }
    }

    public void CreateToken(string modifierTypeString = "")
    {
        if (string.IsNullOrEmpty(modifierTypeString))
        {
            try
            {
                modifierTypeString = UpgradePool[UnityEngine.Random.Range(0, UpgradePool.Count)];
            }
            catch (IndexOutOfRangeException ex)
            {
                Debug.Log($"No modifier was entered and no suitable replacement could be found, leaving method! {modifierTypeString}");
                return;
            }
        }

        if (TryFindFirstAvailableSlot(InventorySlotType.Storage, out var slot))
        {
            var newToken = Instantiate(UpgradeToken, slot.transform.position, Quaternion.identity, upgradeCanvas.transform);
            newToken.transform.localScale *= TileSize;

            var upgradeComponent = newToken.GetComponent<UpgradeToken>();
            var modifierData = (ModifierData)Resources.Load($"Modifiers/Data/{modifierTypeString.ToLower()}");
            upgradeComponent.Init(modifierData);

            slot.currentToken = upgradeComponent;
            slot.isUsed = true;
        }
        else
        {
            Debug.Log("No free inventory slot found!");
        }
    }

    private bool TryFindFirstAvailableSlot(InventorySlotType slotType, out InventorySlot slot)
    {
        slot = null;

        var freeSlot = inventorySlots.FirstOrDefault(slot => slot.slotType == slotType && !slot.isUsed);

        if (freeSlot != null)
        {
            slot = freeSlot;
            return true;
        }

        return false;
    }

    private void HandleUIInteraction()
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

        if (Input.GetMouseButtonDown(0))
        {
            // If not holding a token
            if (selectedItem == null)
            {
                if (upgradeToken != null)
                {
                    selectedItem = upgradeToken.gameObject;

                    if (inventorySlot != null)
                    {
                        fallbackSlot = inventorySlot;
                        inventorySlot.currentToken = null;
                        inventorySlot.isUsed = false;
                    }
                }
            }
            else
            {
                if (inventorySlot != null && !inventorySlot.isUsed)
                {
                    selectedItem.transform.position = inventorySlot.transform.position;

                    inventorySlot.currentToken = upgradeToken;
                    inventorySlot.isUsed = true;

                    selectedItem = null;
                }
                else if (inventorySlot == null)
                {

                    selectedItem.transform.position = fallbackSlot.transform.position;

                    fallbackSlot.currentToken = upgradeToken;
                    fallbackSlot.isUsed = true;

                    fallbackSlot = null;
                    selectedItem = null;
                }
            }
        }

        if (upgradeToken != null && selectedItem == null)
        {
            // Show Upgrade description
        }

        if (selectedItem != null)
        {
            selectedItem.transform.position = Input.mousePosition - new Vector3(selectedItem.transform.localScale.x / 2, selectedItem.transform.localScale.y / 2, 0);
        }
    }

    private void Update()
    {
        HandleUIInteraction();

        if (Input.GetKeyDown(KeyCode.G))
        {
            CreateToken();
        }
    }
}
