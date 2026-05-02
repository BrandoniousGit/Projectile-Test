using System;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectiveBase : MonoBehaviour
{
    public static event Action OnObjectiveCompleted;

    public Objective Objective;
    private int progress;

    public virtual void SetupObjective(Objective objectiveSetup)
    {
        Objective = objectiveSetup;
        UpdateObjectiveText();
    }

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

        if (progress >= Objective.ProgressNeeded)
        {
            OnObjectiveCompleted?.Invoke();
        }
    }

    protected virtual void UpdateObjectiveText()
    {
        var newObjectiveText = FormatObjectiveText(Objective.ObjectiveTextFormatting);

        ObjectiveManager.Instance.UpdateObjectiveText(newObjectiveText);
    }

    private string FormatObjectiveText(string inputString)
    {
        inputString = inputString.Replace("{PROGRESS}", progress.ToString());
        inputString = inputString.Replace("{TARGET}", Objective.ProgressNeeded.ToString());

        return inputString;
    }
}
