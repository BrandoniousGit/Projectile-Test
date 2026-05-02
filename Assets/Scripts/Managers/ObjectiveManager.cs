using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance { get; private set; }

    public Objective ObjectiveData;

    [SerializeField]
    private TextMeshProUGUI ObjectiveText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            DontDestroyOnLoad(this);
        Instance = this;
    }

    private void OnEnable()
    {
        SetupObjective();
    }

    private void SetupObjective()
    {
        // Wait for something else to finish for now I will just call on awake
        var newObjective = gameObject.AddComponent<CollectObjective>();
        newObjective.SetupObjective(ObjectiveData);
    }

    public void UpdateObjectiveText(string text)
    {
        ObjectiveText.SetText(text);
    }
}