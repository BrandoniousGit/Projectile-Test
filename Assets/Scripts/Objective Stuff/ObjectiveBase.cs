using System;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectiveBase : MonoBehaviour
{
    public ObjectiveData Data;
    public static event Action OnObjectiveCompleted;

    [SerializeField]
    private int progress;

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
        UpdateObjectiveText();
    }

    protected virtual void UpdateObjectiveText()
    {
        var newObjectiveText = FormatObjectiveText(Data.ObjectiveStringFormatting);

        ObjectiveManager.Instance.UpdateObjectiveText(newObjectiveText);
    }

    private string FormatObjectiveText(string inputString)
    {
        inputString = inputString.Replace("{PROGRESS}", progress.ToString());
        inputString = inputString.Replace("{TARGET}", Data.ProgressNeeded.ToString());

        return inputString;
    }
}
