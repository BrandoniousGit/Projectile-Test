using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance { get; private set; }

    public ObjectiveBase currentObjective;
    public ObjectiveData currentData;
    public GameObject playerPrefab;

    public static event Action OnRefreshClickedAction;

    [SerializeField]
    private Canvas ObjectiveCanvas;

    [SerializeField]
    private TextMeshProUGUI ObjectiveText;
    [SerializeField]
    private TextMeshProUGUI ProgressionText;    

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
        Mission.OnMissionSelectClicked += SetDataFromMissionSelect;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }    
    
    private void OnDisable()
    {
        Mission.OnMissionSelectClicked -= SetDataFromMissionSelect;
    }

    private void SetDataFromMissionSelect(ObjectiveData data)
    {
        currentData = data;
        ObjectiveText.SetText(data.ObjectivePreview);
    }

    public void OnRefreshClicked()
    {
        OnRefreshClickedAction?.Invoke();
        currentData = null;
    }

    public void OnStartGameClicked()
    {
        if (currentData == null)
        {
            Debug.Log("You need to pick an objective first");
            return;
        }

        var type = Type.GetType(currentData.Type.ToString(), false, true);

        if (type == null)
        {
            Debug.Log($"{currentData.Type.ToString()} is not a valid type");
            return;
        }

        currentObjective = (ObjectiveBase)gameObject.AddComponent(type);
        currentObjective.SetData(currentData);

        //Add fade or whatever scene transision here
        SceneManager.LoadScene("TestScene");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "TestScene")
        {
            var spawnPoint = GameObject.FindGameObjectWithTag("PlayerSpawn");
            Instantiate(playerPrefab, spawnPoint != null ? spawnPoint.transform.position : Vector3.zero, Quaternion.identity);

            ObjectiveCanvas.gameObject.SetActive(true);
            currentObjective.SetupObjectiveItems();
        }
    }

    public void UpdateProgressionText(string text)
    {
        ProgressionText.SetText(text);
    }
}