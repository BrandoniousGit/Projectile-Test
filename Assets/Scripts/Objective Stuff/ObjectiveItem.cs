using System;

public class ObjectiveItem : Interactable
{
    public static event Action<int> OnObjectiveProgress;
    public int progressAmount;

    public override string GetDescription()
    {
        if (isOn)
        {
            return "You have already interacted with this cube!";
        }

        return $"Press 'E' to advance objective by {progressAmount}";
    }

    public override void OnInteract()
    {
        if (isOn)
        {
            return;
        }

        isOn = true;
        OnObjectiveProgress?.Invoke(progressAmount);
    }
}