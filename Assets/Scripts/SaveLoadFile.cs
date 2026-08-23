using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveLoadFile
{
    public static void SaveFileToJson<T>(T data, string savePath)
    {
        string serialisedJson = JsonConvert.SerializeObject(data, Formatting.Indented);

        File.WriteAllText(savePath, serialisedJson);
        Debug.Log("Saved to: " + savePath);
    }

    public static List<InventorySlotContext> LoadInventoryFromFile(string savePath)
    {
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("No save file found!");
            return new List<InventorySlotContext>();
        }

        string json = File.ReadAllText(savePath);

        Debug.Log("Inventory Successfully loaded!");

        return JsonConvert.DeserializeObject<List<InventorySlotContext>>(json);
    }
}
