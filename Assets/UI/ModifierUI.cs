using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ModifierUI : MonoBehaviour
{
    public GameObject ModifierCanvas;
    public GameObject ModifierContent;

    public void Start()
    {
        DirectoryInfo dir = new DirectoryInfo("Assets/Resources/Modifiers/Data");
        FileInfo[] info = dir.GetFiles("*.asset");
        var modifierNames = info.Select(file => file.Name.RemoveFileExtension(6)).ToList();

        foreach (var modifier in modifierNames)
        {
            var uiItem = Resources.Load("ModifierUIItem");
            var temp = Instantiate(uiItem, ModifierContent.transform);
            temp.GetComponent<ModifierUIItem>().InternalItemName = modifier;
        }

        var rectTrans = ModifierContent.GetComponent<RectTransform>();
        rectTrans.sizeDelta = new Vector2(0, modifierNames.Count * 80);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            ModifierCanvas.SetActive(!ModifierCanvas.activeSelf);
            PlayerManager.Instance.SetPlayerFrozen(!PlayerManager.Instance.GetPlayerFrozen());
        }
    }
}
