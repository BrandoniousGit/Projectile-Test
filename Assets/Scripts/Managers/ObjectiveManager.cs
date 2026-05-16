using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance { get; private set; }

    public ObjectiveBase CurrentObjective;
    public ObjectiveData CurrentData;

    public static event Action OnRefreshClickedAction;

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
        Mission.OnMissionSelectClicked += SetData;
    }    
    
    private void OnDisable()
    {
        Mission.OnMissionSelectClicked -= SetData;
    }

    private void SetData(ObjectiveData data)
    {
        CurrentData = data;
    }

    public void OnRefreshClicked()
    {
        OnRefreshClickedAction?.Invoke();
        CurrentData = null;
    }

    public void OnStartGameClicked()
    {
        if (CurrentData == null)
        {
            Debug.Log("You need to pick an objective first");
            return;
        }

        var type = Type.GetType(CurrentData.Type.ToString(), false, true);

        if (type == null)
        {
            Debug.Log($"{CurrentData.Type.ToString()} is not a valid type");
            return;
        }

        CurrentObjective = (ObjectiveBase)gameObject.AddComponent(type);
        CurrentObjective.Data = CurrentData;
    }

    public void UpdateObjectiveText(string text)
    {
        ObjectiveText.SetText(text);
    }
}