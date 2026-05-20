using System;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;

public abstract class ObjectiveBase : MonoBehaviour
{
    public ObjectiveData Data;
    public static event Action OnObjectiveCompleted;

    [SerializeField]
    private int progress;

    public abstract void SetupObjectiveItems();
    public abstract void SetData(ObjectiveData data);

    protected virtual void OnEnable()
    {
        ObjectiveItem.OnObjectiveProgress += OnProgressUpdate;
    }    
    
    protected virtual void OnDisable()
    {
        ObjectiveItem.OnObjectiveProgress -= OnProgressUpdate;
    }

    protected virtual void OnProgressUpdate(int progressValue)
    {
        progress += progressValue;
        UpdateObjectiveText();
    }

    protected virtual void UpdateObjectiveText()
    {
        var newProgressionText = FormatObjectiveText(Data.ObjectiveStringFormatting);

        ObjectiveManager.Instance.UpdateProgressionText(newProgressionText);
    }

    private string FormatObjectiveText(string inputString)
    {
        inputString = inputString.Replace("{PROGRESS}", progress.ToString());
        inputString = inputString.Replace("{TARGET}", Data.ProgressNeeded.ToString());

        return inputString;
    }
}
