using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }

    private Canvas menuCanvas;
    private List<CanvasGroup> menuScreenGroups;
    public Inventory PlayerInventory;

    private bool canvasIsActive;

    private void ToggleAllMenus()
    {
        foreach(var menu in menuScreenGroups)
        {
            menu.alpha = 0;
            menu.blocksRaycasts = false;
            menu.interactable = false;
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            DontDestroyOnLoad(this);
        Instance = this;
    }

    public void Start()
    {
        menuCanvas = GetComponent<Canvas>();
        menuScreenGroups = FindObjectsByType<CanvasGroup>(FindObjectsSortMode.None).ToList();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            canvasIsActive = !canvasIsActive;
            PlayerManager.Instance.SetPlayerFrozen(canvasIsActive);
        }

        if (canvasIsActive)
        {
            menuCanvas.enabled = true;
        }
        else
        {
            menuCanvas.enabled = false;
        }
    }

    public void ToggleMenu(CanvasGroup menuScreenGroup)
    {
        ToggleAllMenus();

        menuScreenGroup.alpha = 1;
        menuScreenGroup.blocksRaycasts = true;
        menuScreenGroup.interactable = true;
    }
}
