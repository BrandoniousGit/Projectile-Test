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
    private UpgradeStorageSlot fallbackSlot;
    private List<UpgradeStorageSlot> inventorySlots = new();
    private GameObject UpgradeSlotPrefab;
    private GameObject UpgradeTokenPrefab;

    public EnhanceableBase Enhanceable;
    public Sprite BackgroundTile;

    public Vector2 GridSize;
    public float TileSize;
    public List<string> UpgradePool;

    public enum UpgradeSlotType
    { 
        Upgrade,
        Storage
    }

    private void Start()
    {
        DirectoryInfo dir = new DirectoryInfo("Assets/Resources/Modifiers/Data");
        FileInfo[] info = dir.GetFiles("*.asset");
        UpgradePool = info.Select(file => file.Name.RemoveFileExtension(6)).ToList();

        UpgradeTokenPrefab = (GameObject)Resources.Load("UpgradeToken");
        UpgradeSlotPrefab = (GameObject)Resources.Load("UpgradeSlot");

        upgradeCanvas = transform.root.GetComponent<Canvas>();
        graphicRaycaster = transform.root.GetComponent<GraphicRaycaster>();
        eventSystem = GameObject.FindGameObjectWithTag("EventSystem").GetComponent<EventSystem>();

        Vector2 screenCenter = upgradeCanvas.GetComponent<RectTransform>().sizeDelta / 2;

        CreateInventory(new Vector2(screenCenter.x - (TileSize - 0.5f) * 1.5f, screenCenter.y - (TileSize - 1) * 1.5f), new Vector2(GridSize.x, GridSize.y), UpgradeSlotType.Upgrade);
    }

    private void CreateInventory(Vector2 bottomLeft, Vector2 gridSize, UpgradeSlotType slotType)
    {
        for (int i = 0; i < gridSize.x; i++)
        {
            for (int j = 0; j < gridSize.y; j++)
            {
                var uiItem = Instantiate
                (
                    UpgradeSlotPrefab, 
                    Vector3.zero, 
                    Quaternion.identity,
                    transform
                );

                var slot = uiItem.GetComponent<UpgradeStorageSlot>();
                slot.Init(new Vector2(i, j), slotType);

                var rectTrans = uiItem.GetComponent<RectTransform>();
                rectTrans.sizeDelta *= TileSize;
                rectTrans.anchoredPosition = new Vector3(i * (TileSize - 0.5f) + bottomLeft.x, j * (TileSize - 0.5f) + bottomLeft.y, 0);

                inventorySlots.Add(slot);
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

        if (TryFindFirstAvailableSlot(UpgradeSlotType.Storage, out var slot))
        {
            var newToken = Instantiate(UpgradeTokenPrefab, slot.transform.position, Quaternion.identity, transform);
            newToken.transform.localScale *= TileSize;

            var upgradeComponent = newToken.GetComponent<UpgradeToken>();
            var modifierData = (ModifierData)Resources.Load($"Modifiers/Data/{modifierTypeString.ToLower()}");
            upgradeComponent.MyWeapon = Enhanceable;
            upgradeComponent.MySlot = UpgradeSlotType.Storage;
            upgradeComponent.Init(modifierData);

            slot.currentToken = upgradeComponent;
            slot.isUsed = true;
        }
        else
        {
            Debug.Log("No free inventory slot found!");
        }
    }

    private bool TryFindFirstAvailableSlot(UpgradeSlotType slotType, out UpgradeStorageSlot slot)
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

        UpgradeStorageSlot inventorySlot = null;
        UpgradeToken upgradeToken = null;

        foreach (var result in results)
        {
            switch (result.gameObject.tag)
            {
                case "InventorySlot":
                    inventorySlot = result.gameObject.GetComponent<UpgradeStorageSlot>();
                    break;
                case "UIItem":
                    upgradeToken = result.gameObject.GetComponent<UpgradeToken>();
                    break;
                default:
                    Debug.Log("Found no matching tag");
                    break;
            }
        }

        // If not holding a token
        if (selectedItem == null)
        {
            //If hovering a token
            if (upgradeToken != null)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    selectedItem = upgradeToken.gameObject;

                    if (inventorySlot != null)
                    {
                        fallbackSlot = inventorySlot;
                        inventorySlot.currentToken = null;
                        inventorySlot.isUsed = false;
                    }
                }

                Debug.Log("I should be showing my description");
            }
        }
        else
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (inventorySlot != null && !inventorySlot.isUsed)
                {
                    selectedItem.transform.position = inventorySlot.transform.position;

                    inventorySlot.currentToken = upgradeToken;
                    inventorySlot.isUsed = true;
                    upgradeToken.MySlot = inventorySlot.slotType;

                    selectedItem.transform.localScale = Vector3.one * TileSize;

                    selectedItem = null;
                }
                else if (inventorySlot == null)
                {
                    selectedItem.transform.position = fallbackSlot.transform.position;

                    fallbackSlot.currentToken = upgradeToken;
                    fallbackSlot.isUsed = true;

                    selectedItem.transform.localScale = Vector3.one * TileSize;

                    fallbackSlot = null;
                    selectedItem = null;
                }
            }
        }

        if (selectedItem != null)
        {
            selectedItem.transform.position = Vector3.Lerp(selectedItem.transform.position, Input.mousePosition - new Vector3(selectedItem.transform.lossyScale.x / 2, selectedItem.transform.lossyScale.y / 2, 0), 0.15f);
            selectedItem.transform.localScale = Vector3.Lerp(selectedItem.transform.localScale, Vector3.one * TileSize * 1.25f, 0.1f);
        }
    }

    private void Update()
    {
        HandleUIInteraction();

        if (Input.GetKeyDown(KeyCode.G))
        {
            CreateToken("RapidFire");
        }
    }
}
