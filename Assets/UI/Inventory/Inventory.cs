using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEditor;
using TMPro;
using System.IO;

public class Inventory : MonoBehaviour
{
    public GameObject selectedItem;
    public InventorySlot fallbackSlot;

    private Canvas inventoryCanvas;
    private GraphicRaycaster graphicRaycaster;
    private EventSystem eventSystem;
    private List<InventorySlot> inventorySlots = new();
    private GameObject InventorySlotPrefab;
    private GameObject InventoryItemPrefab;
    private bool initialized;

    public TMP_Dropdown selectDropdown;
    public TMP_InputField inputField;
    public Toggle stackable;

    public Dictionary<int, ItemData> ItemLookup;

    public Sprite BackgroundTile;

    public Vector2 GridSize;
    public float TileSize;

    private string SavePath => Path.Combine(Application.persistentDataPath, "inventory.json");

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        InventoryItemPrefab = (GameObject)Resources.Load("InventoryItem");
        InventorySlotPrefab = (GameObject)Resources.Load("InventorySlot");

        string[] guids = AssetDatabase.FindAssets("t:ItemData", new[] { "Assets/Resources/Items" });

        ItemLookup = guids
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<ItemData>)
            .OrderBy(item => item.ItemID)
            .ToDictionary(x => x.ItemID, x => x);

        inventoryCanvas = transform.root.GetComponent<Canvas>();
        graphicRaycaster = transform.root.GetComponent<GraphicRaycaster>();
        eventSystem = GameObject.FindGameObjectWithTag("EventSystem").GetComponent<EventSystem>();

        Vector2 canvasCenter = inventoryCanvas.GetComponent<RectTransform>().sizeDelta / 2;

        CreateInventory(new Vector2(canvasCenter.x - (TileSize * (GridSize.x / 2)), canvasCenter.y + (TileSize * (GridSize.y / 2) - TileSize)), new Vector2(GridSize.x, GridSize.y));

        selectDropdown.AddOptions(ItemLookup.Select(item => new TMP_Dropdown.OptionData() { text = item.Value.ItemName }).ToList());

        LoadInventoryState();

        initialized = true;
    }

    private void CreateInventory(Vector2 topLeft, Vector2 gridSize)
    {
        for (int i = 0; i < gridSize.y; i++)
        {
            for (int j = 0; j < gridSize.x; j++)
            {
                var uiItem = Instantiate
                (
                    InventorySlotPrefab,
                    Vector3.zero,
                    Quaternion.identity,
                    transform
                );

                var slot = uiItem.GetComponent<InventorySlot>();
                slot.Init(new Vector2(j, i));

                var rectTrans = uiItem.GetComponent<RectTransform>();
                rectTrans.sizeDelta *= TileSize;
                rectTrans.anchoredPosition = new Vector3(j * (TileSize - 0.5f) + topLeft.x, -i * (TileSize - 0.5f) + topLeft.y, 0);

                inventorySlots.Add(slot);
                uiItem.GetComponent<Image>().sprite = BackgroundTile;
            }
        }
    }

    private void SaveInventoryState()
    {
        var slotsWithItems = inventorySlots.Where(slot => slot.currentItem != null).ToList();

        var data = slotsWithItems.Select(slot => new InventorySlotContext()
        {
            ItemId = slot.currentItem.GetItemId(),
            ItemCount = slot.currentItem.itemCount,
            GridX = slot.gridPosition.x,
            GridY = slot.gridPosition.y
        }).ToList();

        SaveLoadFile.SaveFileToJson(data, SavePath);
    }

    private void LoadInventoryState()
    {
        ClearInventory();

        var inventoryFile = SaveLoadFile.LoadInventoryFromFile(SavePath);

        foreach (var item in inventoryFile)
        {
            AddItemToGrid(item.ItemId, item.ItemCount, new Vector2(item.GridX, item.GridY));
        }
    }

    private void ClearInventory()
    {
        var slotsWithItems = inventorySlots.Where(slot => slot.currentItem != null).ToList();

        foreach (var slot in slotsWithItems)
        {
            Destroy(slot.currentItem.gameObject);
            slot.isUsed = false;
        }
    }

    private void AddItemToGrid(int itemId, int itemCount, Vector2 gridPos)
    {
        var slot = inventorySlots.FirstOrDefault(slot => slot.gridPosition.x == gridPos.x && slot.gridPosition.y == gridPos.y);
        
        if (slot != null && !slot.isUsed)
        {
            var newItem = Instantiate(InventoryItemPrefab, slot.transform.position, Quaternion.identity, slot.transform);
            newItem.transform.localScale *= TileSize;

            var inventoryItem = newItem.GetComponent<InventoryItem>();
            inventoryItem.Init(ItemLookup[itemId], itemCount);

            slot.currentItem = inventoryItem;
            slot.isUsed = true;
        }
    }

    public void PickupItem(int itemId, int itemCount, bool createNewStackOnAdd = false)
    {
        if (!ItemLookup.ContainsKey(itemId))
        {
            Debug.LogError("Something has gone horribly fucking wrong and this item DOES NOT EXIST!!!!!");
            return;
        }

        if (TryFindFirstAvailableSlot(itemId, out var slot, createNewStackOnAdd))
        {
            if (slot.isUsed)
            {
                var currentItem = slot.currentItem;

                if (currentItem.itemCount + itemCount > ItemLookup[itemId].MaxStackSize)
                {
                    var excess = currentItem.itemCount + itemCount - ItemLookup[itemId].MaxStackSize;
                    currentItem.itemCount += itemCount - excess;

                    PickupItem(itemId, excess, createNewStackOnAdd);
                }
                else
                {
                    currentItem.itemCount += itemCount;
                }
            }
            else
            {
                var newItem = Instantiate(InventoryItemPrefab, slot.transform.position, Quaternion.identity, slot.transform);
                newItem.transform.localScale *= TileSize;

                if (itemCount > ItemLookup[itemId].MaxStackSize)
                {
                    var newItemCount = ItemLookup[itemId].MaxStackSize;

                    var inventoryItem = newItem.GetComponent<InventoryItem>();
                    inventoryItem.Init(ItemLookup[itemId], newItemCount);

                    slot.currentItem = inventoryItem;
                    slot.isUsed = true;

                    PickupItem(itemId, itemCount - newItemCount, createNewStackOnAdd);
                }
                else
                {
                    var inventoryItem = newItem.GetComponent<InventoryItem>();
                    inventoryItem.Init(ItemLookup[itemId], itemCount);

                    slot.currentItem = inventoryItem;
                }
            }
        }
        else
        {
            Debug.Log("No free inventory slot found!");
        }
    }

    private bool TryFindFirstAvailableSlot(int itemId, out InventorySlot slot, bool dontMatchSlots = false)
    {
        slot = null;

        var existingUnfilledSlot = inventorySlots.FirstOrDefault(slot => 
            slot.isUsed && 
            slot.currentItem.GetItemId() == itemId && 
            slot.currentItem.itemCount < ItemLookup[itemId].MaxStackSize
        );

        if (existingUnfilledSlot != null && !dontMatchSlots)
        {
            slot = existingUnfilledSlot;
            return true;
        }

        var freeSlot = inventorySlots.FirstOrDefault(slot => !slot.isUsed);

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

        InventorySlot targetInventorySlot = null;
        InventoryItem targetInventoryItem = null;

        foreach (var result in results)
        {
            switch (result.gameObject.tag)
            {
                case "InventorySlot":
                    targetInventorySlot = result.gameObject.GetComponent<InventorySlot>();
                    break;
                case "UIItem":
                    targetInventoryItem = result.gameObject.GetComponent<InventoryItem>();
                    break;
                default:
                    Debug.Log("Found no matching tag");
                    break;
            }
        }

        // If not holding an item
        if (selectedItem == null)
        {
            // If hovering an item
            if (targetInventoryItem != null)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    selectedItem = targetInventoryItem.gameObject;
                    selectedItem.transform.SetParent(inventoryCanvas.transform);
                    selectedItem.transform.SetAsLastSibling();

                    if (targetInventorySlot != null)
                    {
                        fallbackSlot = targetInventorySlot;
                        targetInventorySlot.currentItem = null;
                    }
                }

                //// Split stack in half
                //if (Input.GetMouseButtonDown(1))
                //{
                //    if (targetInventorySlot != null && targetInventorySlot.isUsed && targetInventoryItem.itemCount > 1)
                //    {
                //        var halfValue = Mathf.FloorToInt(targetInventoryItem.itemCount / 2);

                //        var newItem = Instantiate(InventoryItemPrefab, targetInventorySlot.transform.position, Quaternion.identity, inventoryCanvas.transform);
                //        newItem.transform.localScale *= TileSize;

                //        var inventoryItem = newItem.GetComponent<InventoryItem>();
                //        inventoryItem.Init(ItemLookup[targetInventorySlot.currentItem.GetItemId()], halfValue);

                //        targetInventoryItem.itemCount -= halfValue;

                //        selectedItem = newItem;
                //    }
                //    else
                //    {
                //        selectedItem = targetInventoryItem.gameObject;
                //        selectedItem.transform.SetParent(inventoryCanvas.transform);
                //        selectedItem.transform.SetAsLastSibling();

                //        if (targetInventorySlot != null)
                //        {
                //            fallbackSlot = targetInventorySlot;
                //            targetInventorySlot.currentItem = null;
                //        }
                //    }
                //}
            }
        }
        // If holding an item
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (targetInventorySlot != null && !targetInventorySlot.isUsed)
                {
                    selectedItem.transform.position = targetInventorySlot.transform.position;
                    selectedItem.transform.SetParent(targetInventorySlot.transform);

                    targetInventorySlot.currentItem = targetInventoryItem;

                    selectedItem.transform.localScale = Vector3.one * TileSize;

                    selectedItem = null;
                }
                else if (targetInventorySlot != null && targetInventorySlot.isUsed)
                {
                    var selectedItemScript = selectedItem.GetComponent<InventoryItem>();

                    if (targetInventorySlot.currentItem.GetItemId() == selectedItemScript.GetItemId() && targetInventorySlot.currentItem.itemCount < targetInventoryItem.GetItemMaxStackSize())
                    {
                        var targetMaxSize = targetInventoryItem.GetItemMaxStackSize();
                        var targetSlotAvailableSpace = targetMaxSize - targetInventoryItem.itemCount;

                        if (targetSlotAvailableSpace >= selectedItemScript.itemCount)
                        {
                            targetInventorySlot.currentItem.itemCount += selectedItemScript.itemCount;
                            Destroy(selectedItemScript.gameObject);
                        }
                        else
                        {
                            targetInventorySlot.currentItem.itemCount = targetMaxSize;
                            selectedItemScript.itemCount -= targetSlotAvailableSpace;
                        }
                    }
                    else
                    {
                        targetInventorySlot.currentItem.gameObject.transform.position = fallbackSlot.transform.position;
                        targetInventorySlot.currentItem.gameObject.transform.SetParent(fallbackSlot.transform);
                        targetInventorySlot.currentItem = selectedItemScript;

                        selectedItem.transform.position = targetInventorySlot.transform.position;
                        selectedItem.transform.SetParent(targetInventorySlot.transform);
                        selectedItem.transform.localScale = Vector3.one * TileSize;
                        fallbackSlot.currentItem = targetInventoryItem;

                        selectedItem = null;
                    }
                }
                else if (targetInventorySlot == null)
                {
                    SetItemToFallback(targetInventoryItem);

                    // TO BE REPLACED WITH "DropItem()"
                }
            }
        }

        if (selectedItem != null)
        {
            selectedItem.transform.position = Vector3.Lerp(selectedItem.transform.position, Input.mousePosition - new Vector3(selectedItem.transform.lossyScale.x / 2, selectedItem.transform.lossyScale.y / 2, 0), 0.15f);
            selectedItem.transform.localScale = Vector3.Lerp(selectedItem.transform.localScale, Vector3.one * TileSize * 1.25f, 0.1f);
        }
    }

    private void SetItemToFallback(InventoryItem item)
    {
        selectedItem.transform.position = fallbackSlot.transform.position;
        selectedItem.transform.SetParent(fallbackSlot.transform);
        fallbackSlot.currentItem = item;
        selectedItem.transform.localScale = Vector3.one * TileSize;

        selectedItem = null;
    }

    public void OnAddCheatedItemClicked()
    {
        var itemId = ItemLookup.FirstOrDefault(item => item.Value.ItemName == selectDropdown.options[selectDropdown.value].text).Key;

        if (int.TryParse(inputField.text, out var result))
        {
            PickupItem(itemId, result, stackable.isOn);
        }
        else
        {
            PickupItem(itemId, 1, stackable.isOn);
        }
    }

    private void Update()
    {
        if (!initialized) return;

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.C))
        {
            SaveInventoryState();
        }        
        
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.V))
        {
            LoadInventoryState();
        }        
        
        if (Input.GetKey(KeyCode.A) && Input.GetKeyDown(KeyCode.N))
        {
            ClearInventory();
        }

        HandleUIInteraction();
    }
}
