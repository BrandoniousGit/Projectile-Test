using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Mission : MonoBehaviour
{
    public TextMeshProUGUI MissionName, ObjectiveDescription;
    public ObjectiveData Objective;

    public static event Action<ObjectiveData> OnMissionSelectClicked;

    private List<ObjectiveData> ObjectiveDatas;

    private void Awake()
    {
        ObjectiveDatas = Resources.LoadAll<ObjectiveData>("Objectives/Data").ToList();

        GenerateMission();
    }

    public void OnEnable()
    {
        ObjectiveManager.OnRefreshClickedAction += GenerateMission;
    }

    public void OnDisable()
    {
        ObjectiveManager.OnRefreshClickedAction -= GenerateMission;
    }

    public void OnMissionSelectClick()
    {
        if (Objective != null)
        {
            OnMissionSelectClicked?.Invoke(Objective);
        }
        else
        {
            Debug.Log("Something went wrong!");
        }
    }

    public void GenerateMission()
    {
        var prefix = Globals.WordsList.Prefixes[UnityEngine.Random.Range(0, Globals.WordsList.Prefixes.Length)];
        var noun = Globals.WordsList.Nouns[UnityEngine.Random.Range(0, Globals.WordsList.Nouns.Length)];

        MissionName.SetText($"{prefix} {noun}");

        Objective = GetFromWeighted(ObjectiveDatas);
        ObjectiveDescription.SetText(Objective.ObjectivePreview);
    }

    public ObjectiveData GetFromWeighted(List<ObjectiveData> datas)
    {
        var totalWeight = 0;

        foreach (var data in datas)
        {
            totalWeight += data.Weight;
        }

        var selectedNumber = UnityEngine.Random.Range(0, totalWeight + 1);
        var currentWeight = 0;

        foreach (var data in datas)
        {
            currentWeight += data.Weight;

            if (selectedNumber <= currentWeight)
            {
                return data;
            }
        }

        Debug.Log($"Selected number was {selectedNumber} with total weight being {totalWeight}");

        return null;
    }
}